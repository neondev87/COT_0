// Pantalla Emotes: pestañas USUARIOS | TIENDA | TICKETS, recuadros encima (stock de un usuario, set y reportes) y ESC
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Cada sección carga al mostrarse (así el cupo y los tickets siempre están al día).
/// ESC cierra el recuadro abierto; si no hay ninguno, regresa como ← INICIO.
/// </summary>
public partial class EmotesScreenViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUsersSection), nameof(IsShopSection), nameof(IsTicketsSection))]
    private StoreSection _section = StoreSection.Shop;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverlayOpen))]
    private bool _isSetPanelOpen;

    public EmotesScreenViewModel(StoreUsersViewModel users, StoreShopViewModel shop, StoreTicketsViewModel tickets)
    {
        Users = users;
        Shop = shop;
        Tickets = tickets;
        tickets.ShopRequested += () => _ = ShowAsync(StoreSection.Shop);
        users.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(StoreUsersViewModel.IsStockOpen)) OnPropertyChanged(nameof(IsOverlayOpen));
        };
    }

    public StoreUsersViewModel Users { get; }

    public StoreShopViewModel Shop { get; }

    public StoreTicketsViewModel Tickets { get; }

    // Las pestañas son RadioButton enlazados en dos sentidos: marcar una cambia de sección
    public bool IsUsersSection
    {
        get => Section == StoreSection.Users;
        set { if (value && Section != StoreSection.Users) _ = ShowAsync(StoreSection.Users); }
    }

    public bool IsShopSection
    {
        get => Section == StoreSection.Shop;
        set { if (value && Section != StoreSection.Shop) _ = ShowAsync(StoreSection.Shop); }
    }

    public bool IsTicketsSection
    {
        get => Section == StoreSection.Tickets;
        set { if (value && Section != StoreSection.Tickets) _ = ShowAsync(StoreSection.Tickets); }
    }

    public bool IsOverlayOpen => Users.IsStockOpen || IsSetPanelOpen;

    public Task LoadAsync() => ShowAsync(Section);

    [RelayCommand]
    private void OpenSetPanel() => IsSetPanelOpen = true;

    [RelayCommand]
    private void CloseOverlay()
    {
        IsSetPanelOpen = false;
        Users.CloseCommand.Execute(null);
    }

    [RelayCommand]
    private void Escape()
    {
        if (IsOverlayOpen) CloseOverlay();
        else Navigator.GoBack();
    }

    private Task ShowAsync(StoreSection section)
    {
        Section = section;
        return section switch
        {
            StoreSection.Users => Users.LoadAsync(),
            StoreSection.Tickets => Tickets.LoadAsync(),
            _ => Shop.LoadAsync(),
        };
    }
}
