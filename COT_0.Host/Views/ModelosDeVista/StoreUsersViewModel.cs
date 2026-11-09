// Sección USUARIOS: todos los usuarios registrados como tarjetas; al tocar uno se abre su stock de emotes
using System.Collections.ObjectModel;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Usuarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>De usuarios.json solo se toma el nombre. No se marca quién hizo ENTRAR (dentro de la app nadie sabe cuál fue).</summary>
public partial class StoreUsersViewModel(PurchaseService purchases) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStockOpen))]
    private UserStockViewModel? _stock;

    [ObservableProperty]
    private bool _isEmpty;

    public ObservableCollection<UserCardViewModel> Users { get; } = [];

    public NoticeViewModel Notice { get; } = new();

    public bool IsStockOpen => Stock is not null;

    public async Task LoadAsync()
    {
        try
        {
            var accounts = await HostAccounts.LoadAsync();
            Users.Clear();
            for (var i = 0; i < accounts.Count; i++) Users.Add(new UserCardViewModel(accounts[i].UserName, i + 1));
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

    [RelayCommand]
    private void Close() => Stock = null;
}
