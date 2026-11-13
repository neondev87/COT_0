// Pantalla USUARIOS (04): tarjetas con foto o inicial; ADMIN: VER FACTURA, VER SUS EMOTES y ELIMINAR USUARIO; dueño: NOMBRE, CONTRASEÑA y FOTO
using System.Collections.ObjectModel;
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Dialogos;
using COT_0.Host.Services.Perfiles;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Modo HOST: solo tu tarjeta (nunca datos de otros). Modo ADMIN: todas, con sus tickets (solo lectura), su stock y ELIMINAR.
/// Borrar tu propia cuenta cierra la sala (si hay) y regresa al login. ESC cierra el recuadro o regresa.
/// </summary>
public partial class UsersScreenViewModel(PurchaseService purchases, AccountService accounts, ConfirmDialogService confirm)
    : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStockOpen), nameof(IsOverlayOpen))]
    private UserStockViewModel? _stock;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormOpen), nameof(IsOverlayOpen))]
    private AccountFormViewModel? _form;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsInvoicesOpen), nameof(IsOverlayOpen))]
    private UserInvoicesViewModel? _invoices;

    [ObservableProperty]
    private bool _isEmpty;

    public ObservableCollection<UserCardViewModel> Users { get; } = [];

    public NoticeViewModel Notice { get; } = new();

    public bool IsStockOpen => Stock is not null;

    public bool IsFormOpen => Form is not null;

    public bool IsInvoicesOpen => Invoices is not null;

    public bool IsOverlayOpen => IsStockOpen || IsFormOpen || IsInvoicesOpen;

    public string Hint => StoreTexts.Format(ActiveUser.IsAdmin ? "Text.Users.Hint.Admin" : "Text.Users.Hint.Host");

    public async Task LoadAsync()
    {
        try
        {
            var accounts = await HostAccounts.LoadAsync();
            Users.Clear();
            foreach (var account in accounts)
            {
                if (!ActiveUser.IsAdmin && !ActiveUser.Is(account.UserName)) continue;
                var card = new UserCardViewModel(account.UserName);
                Users.Add(card);
                await card.LoadPhotoAsync();
            }
            IsEmpty = Users.Count == 0;
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Error.Storage"), InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private async Task OpenAsync(UserCardViewModel card)
    {
        if (!card.CanManage && !ActiveUser.IsAdmin) return;
        try
        {
            var user = await purchases.GetAsync(card.Name);
            Stock = new UserStockViewModel(card, user, QuotaCalculator.For(user, DateTime.UtcNow));
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.Error.Storage"), InfoBarSeverity.Error);
        }
    }

    /// <summary>VER FACTURA: solo ADMIN (el HOST ve sus tickets en la tienda).</summary>
    [RelayCommand]
    private async Task ViewInvoicesAsync(UserCardViewModel card)
    {
        if (!ActiveUser.IsAdmin) return;
        try
        {
            Invoices = new UserInvoicesViewModel(card.Name, await purchases.GetAsync(card.Name));
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.Error.Storage"), InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private void Rename(UserCardViewModel card) => OpenForm(card, isRename: true);

    [RelayCommand]
    private void ChangePassword(UserCardViewModel card) => OpenForm(card, isRename: false);

    [RelayCommand]
    private async Task ChangePhotoAsync(UserCardViewModel card)
    {
        if (!card.CanManage || await PhotoPicker.PickAsync() is not { } file) return;
        try
        {
            await ProfilePhotos.SaveAsync(card.Name, file);
            await card.LoadPhotoAsync();
            Notice.Show(StoreTexts.Format("Text.Users.PhotoSaved"), InfoBarSeverity.Success);
        }
        catch (Exception ex) when (ProfilePhotos.IsPhotoError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Users.PhotoFailed"), InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(UserCardViewModel card)
    {
        var texts = new ConfirmTexts(StoreTexts.Format("Text.Users.ConfirmDelete", card.Name),
            StoreTexts.Format("Text.Users.Delete"), StoreTexts.Format("Text.Admins.Cancel"));
        if (!card.CanDelete || !await confirm.ConfirmAsync(texts)) return;
        try
        {
            if (await accounts.DeleteAsync(card.Name) != CredentialError.None) return;
            Notice.Show(StoreTexts.Format("Text.Users.Deleted", card.Name), InfoBarSeverity.Success);
            await LoadAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Error.Storage"), InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private void CloseOverlay()
    {
        Stock = null;
        Form = null;
        Invoices = null;
    }

    [RelayCommand]
    private void Escape()
    {
        if (IsOverlayOpen) CloseOverlay();
        else Navigator.GoBack();
    }

    private void OpenForm(UserCardViewModel card, bool isRename)
    {
        if (!card.CanManage) return;
        var form = new AccountFormViewModel(accounts, card.Name, isRename);
        form.Done += async message =>
        {
            Form = null;
            Notice.Show(message, InfoBarSeverity.Success);
            await LoadAsync();
        };
        Form = form;
    }
}
