// TIENDA DE EMOTES (05): pestañas TIENDA | TICKETS (modo ADMIN: PUNTO DE VENTA | ADMINISTRADORES), EMOTES Y REPORTES encima y ESC
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Cada sección carga al mostrarse (así el cupo y los tickets siempre están al día).
/// ESC cierra el recuadro abierto (en COMPRA REALIZADA es SALIR); si no hay ninguno, regresa como ← INICIO.
/// </summary>
public partial class EmotesScreenViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsShopSection), nameof(IsTicketsSection), nameof(IsSalesSection), nameof(IsAdminsSection))]
    private StoreSection _section = ActiveUser.IsAdmin ? StoreSection.Sales : StoreSection.Shop;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverlayOpen))]
    private bool _isSetPanelOpen;

    public EmotesScreenViewModel(StoreShopViewModel shop, StoreTicketsViewModel tickets, SalesViewModel sales, AdminsViewModel admins)
    {
        Shop = shop;
        Tickets = tickets;
        Sales = sales;
        Admins = admins;
        tickets.ShopRequested += () => _ = ShowAsync(StoreSection.Shop);
        shop.Summary.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PurchaseSummaryViewModel.IsOpen)) OnPropertyChanged(nameof(IsOverlayOpen));
        };
    }

    public StoreShopViewModel Shop { get; }

    public StoreTicketsViewModel Tickets { get; }

    public SalesViewModel Sales { get; }

    public AdminsViewModel Admins { get; }

    /// <summary>Se lee al abrir la pantalla (se crea de nuevo en cada visita).</summary>
    public bool IsAdminMode { get; } = ActiveUser.IsAdmin;

    public bool IsHostMode => !IsAdminMode;

    // Las pestañas son RadioButton enlazados en dos sentidos: marcar una cambia de sección
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

    public bool IsSalesSection
    {
        get => Section == StoreSection.Sales;
        set { if (value && Section != StoreSection.Sales) _ = ShowAsync(StoreSection.Sales); }
    }

    public bool IsAdminsSection
    {
        get => Section == StoreSection.Admins;
        set { if (value && Section != StoreSection.Admins) _ = ShowAsync(StoreSection.Admins); }
    }

    /// <summary>Velo encendido: EMOTES Y REPORTES o el panel COMPRA REALIZADA.</summary>
    public bool IsOverlayOpen => IsSetPanelOpen || Shop.Summary.IsOpen;

    public Task LoadAsync() => ShowAsync(Section);

    /// <summary>Al salir de la pantalla: el temporizador de la tienda deja de correr.</summary>
    public void Unload() => Shop.Unload();

    [RelayCommand]
    private void OpenSetPanel() => IsSetPanelOpen = true;

    [RelayCommand]
    private void CloseOverlay()
    {
        if (Shop.Summary.IsOpen) Shop.Summary.ExitCommand.Execute(null);
        IsSetPanelOpen = false;
    }

    [RelayCommand]
    private void Escape()
    {
        // Durante el sello de compra ESC espera al panel (ahí es SALIR)
        if (Shop.Summary.IsSealVisible) return;
        if (IsOverlayOpen) CloseOverlay();
        else Navigator.GoBack();
    }

    private Task ShowAsync(StoreSection section)
    {
        Section = section;
        if (section != StoreSection.Shop) Shop.Unload();
        return section switch
        {
            StoreSection.Tickets => Tickets.LoadAsync(),
            StoreSection.Sales => Sales.LoadAsync(),
            StoreSection.Admins => Admins.LoadAsync(),
            _ => Shop.LoadAsync(),
        };
    }
}
