// Sección ADMINISTRADORES (modo ADMIN): lista de admins, CREAR ADMIN, REGENERAR CONTRASEÑA y ELIMINAR (doble confirmación)
using System.Collections.ObjectModel;
using COT_0.Host.Models;
using COT_0.Host.Services.Administradores;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Dialogos;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Hasta Admin:Maximo; al llegar, CREAR ADMIN se apaga con "Ya hay 5 administradores". Cada contraseña nueva
/// (crear o regenerar) se genera sola y se muestra una vez con AdminPasswordDialogService.
/// ELIMINAR pide la respuesta secreta del admin y tu contraseña; si te eliminas a ti mismo, se cierra tu sesión.
/// </summary>
public partial class AdminsViewModel(AdminService admins, AdminPasswordDialogService passwordDialog, ConfirmDialogService confirm,
    DeleteAdminDialogService deleteDialog) : ObservableObject
{
    [ObservableProperty]
    private string _newName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    private bool _isFull;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    private bool _isBusy;

    public ObservableCollection<AdminRowViewModel> Admins { get; } = [];

    public NoticeViewModel Notice { get; } = new();

    public bool CanCreate => !IsFull && !IsBusy;

    public string FullText => AdminTexts.Error(AdminError.Full);

    public async Task LoadAsync()
    {
        try
        {
            var names = await admins.GetNamesAsync();
            Admins.Clear();
            foreach (var name in names)
                Admins.Add(new AdminRowViewModel(name, string.Equals(name, ActiveUser.AdminName, StringComparison.OrdinalIgnoreCase)));
            IsFull = names.Count >= AdminService.Maximum;
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(AdminTexts.Error(AdminError.Storage), InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private Task CreateAsync() => RunAsync(async () =>
    {
        var name = NewName;
        if (await ShowPasswordAsync(name, await admins.CreateAsync(name)))
        {
            NewName = string.Empty;
            Notice.Show(StoreTexts.Format("Text.Admins.Created", name), InfoBarSeverity.Success);
        }
    });

    [RelayCommand]
    private Task RegenerateAsync(AdminRowViewModel row) => RunAsync(async () =>
    {
        if (!await confirm.ConfirmAsync(Texts("Text.Admins.ConfirmRegenerate", "Text.Admins.Regenerate", row.Name))) return;
        if (await ShowPasswordAsync(row.Name, await admins.RegenerateAsync(row.Name)))
            Notice.Show(StoreTexts.Format("Text.Admins.Regenerated", row.Name), InfoBarSeverity.Success);
    });

    [RelayCommand]
    private Task DeleteAsync(AdminRowViewModel row) => RunAsync(async () =>
    {
        if (await admins.GetQuestionKeyAsync(row.Name) is not { } questionKey)
        {
            Notice.Show(StoreTexts.Format("Text.Admin.Error.QuestionMissing"), InfoBarSeverity.Warning);
            return;
        }
        if (await deleteDialog.AskAsync(row.Name, questionKey, row.IsYou) is not { } request) return;

        var error = await admins.DeleteAsync(row.Name, request);
        if (error != AdminError.None) Notice.Show(AdminTexts.Error(error), InfoBarSeverity.Error);
        else if (row.IsYou) SignOutSelf();
        else Notice.Show(StoreTexts.Format("Text.Admins.Deleted", row.Name), InfoBarSeverity.Success);
    });

    // Te eliminaste: tu sesión ya no existe y regresas al login (si eras el último, ADMIN pedirá crear el primero)
    private static void SignOutSelf()
    {
        ActiveUser.SignOut();
        Navigator.ShowLogin();
    }

    // Muestra la contraseña una vez; al volver, el diálogo ya la soltó
    private async Task<bool> ShowPasswordAsync(string name, AdminPasswordResult result)
    {
        if (result.Password is null)
        {
            Notice.Show(AdminTexts.Error(result.Error), InfoBarSeverity.Error);
            return false;
        }
        await passwordDialog.ShowAsync(name, result.Password);
        return true;
    }

    // Una acción a la vez; al terminar se vuelve a leer admin.json (si sigues con sesión de admin)
    private async Task RunAsync(Func<Task> action)
    {
        if (IsBusy) return;
        IsBusy = true;
        Notice.Hide();
        try
        {
            await action();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(AdminTexts.Error(AdminError.Storage), InfoBarSeverity.Error);
        }
        finally
        {
            IsBusy = false;
        }
        if (ActiveUser.IsAdmin) await LoadAsync();
    }

    private static ConfirmTexts Texts(string messageKey, string confirmKey, string name) =>
        new(StoreTexts.Format(messageKey, name), StoreTexts.Format(confirmKey), StoreTexts.Format("Text.Admins.Cancel"));
}
