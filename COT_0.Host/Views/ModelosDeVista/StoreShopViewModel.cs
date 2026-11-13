// Sección TIENDA: todo 7TV por páginas con precio de muestra, buscador, cupo con su temporizador, switch CARRITO | UNO POR UNO y COMPRAR con ticket
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Usuarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Compra para ActiveUser sin cobro (precios de muestra). CARRITO: una compra y un ticket; UNO POR UNO: al instante. Cupo agotado: todo apagado y corre el temporizador. Bought dispara el efecto de la vista y Summary el panel COMPRA REALIZADA.</summary>
public partial class StoreShopViewModel(PurchaseService purchases, EmoteShopCatalog catalog) : ObservableObject
{
    private readonly HashSet<string> _owned = [];
    private QuotaInfo _quota = new(0, 0, 0, null);
    private string _query = string.Empty;
    private int _total;
    private bool _isBuying;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ShopItemsCollection? _items;
    [ObservableProperty] private string _quotaText = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private bool _isErrorVisible;
    [ObservableProperty] private string _errorText = string.Empty;
    [ObservableProperty] private bool _isBlocked;
    [ObservableProperty] private bool _canBuyCart;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCartMode), nameof(IsOneByOneMode))]
    private PurchaseMode _mode = PurchaseMode.Cart;

    public NoticeViewModel Notice { get; } = new();
    public ShopCartViewModel Cart { get; } = new();
    public QuotaTimerViewModel Timer { get; } = new();
    public PurchaseSummaryViewModel Summary { get; } = new();
    public string RulesText { get; } = StoreTexts.Rules();
    public string PriceNoteText { get; } = StoreTexts.Format("Text.Store.Shop.PriceNote");

    // Switch CARRITO | UNO POR UNO: dos RadioButton enlazados en dos sentidos
    public bool IsCartMode { get => Mode == PurchaseMode.Cart; set { if (value) Mode = PurchaseMode.Cart; } }
    public bool IsOneByOneMode { get => Mode == PurchaseMode.OneByOne; set { if (value) Mode = PurchaseMode.OneByOne; } }

    /// <summary>Compra lista: los emotes y si salieron del carrito (para el efecto de la vista).</summary>
    public event Action<IReadOnlyList<ShopEmoteViewModel>, bool>? Bought;

    /// <summary>Al abrir la tienda: relee el cupo de compras.json y carga la primera página una vez.</summary>
    public async Task LoadAsync()
    {
        Timer.Expired -= OnTimerExpired;
        Timer.Expired += OnTimerExpired;
        Summary.Finished -= OnSummaryFinished;
        Summary.Finished += OnSummaryFinished;
        await RefreshQuotaAsync();
        if (Items is null) StartSearch(string.Empty);
    }

    /// <summary>Al salir de la pantalla: el temporizador deja de correr.</summary>
    public void Unload() => Timer.Stop();

    partial void OnModeChanged(PurchaseMode value)
    {
        Cart.Clear();
        foreach (var item in AllItems) item.IsCartMode = value == PurchaseMode.Cart;
        Refresh();
    }

    [RelayCommand] private void Search() => StartSearch(SearchText);

    [RelayCommand]
    private async Task RetryAsync() { IsErrorVisible = false; if (Items is not null) await Items.ResumeAsync(); }

    /// <summary>AGREGAR (carrito) o COMPRAR (uno por uno) en una tarjeta.</summary>
    [RelayCommand]
    private async Task PickAsync(ShopEmoteViewModel item)
    {
        if (!item.IsBuyEnabled) return;
        if (!IsCartMode) await BuyAsync([item], fromCart: false);
        else if (Cart.Add(item)) Refresh();
    }

    [RelayCommand]
    private void RemoveFromCart(ShopEmoteViewModel item) { Cart.Remove(item); Refresh(); }

    [RelayCommand] private Task BuyCartAsync() => BuyAsync(Cart.Items.ToList(), fromCart: true);

    private async Task BuyAsync(IReadOnlyList<ShopEmoteViewModel> items, bool fromCart)
    {
        if (_isBuying || items.Count == 0 || _quota.IsBlocked) return;
        _isBuying = true;
        Refresh();
        try
        {
            var result = await purchases.BuyAsync(ActiveUser.Name, items.Select(i => i.Emote).ToList());
            if (result.Outcome != PurchaseOutcome.NoUser) ApplyQuota(result.Quota);
            await ShowOutcomeAsync(result, items, fromCart);
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.Error.Storage"), InfoBarSeverity.Error);
        }
        finally
        {
            _isBuying = false;
            Refresh();
        }
    }

    private async Task ShowOutcomeAsync(PurchaseResult result, IReadOnlyList<ShopEmoteViewModel> items, bool fromCart)
    {
        if (result.Outcome != PurchaseOutcome.Bought || result.Ticket is not { } ticket)
        {
            Notice.Show(StoreTexts.Format($"Text.Store.Outcome.{result.Outcome}", result.Quota.Remaining), InfoBarSeverity.Warning);
            return;
        }
        foreach (var item in items)
        {
            _owned.Add(item.Emote.Id);
            item.IsOwned = true;
        }
        if (fromCart) Cart.Clear();
        Bought?.Invoke(items, fromCart);
        await Summary.ShowAsync(ticket, items);
    }

    private void StartSearch(string query)
    {
        _query = query;
        _total = 0;
        IsEmpty = IsErrorVisible = false;
        Cart.Clear();
        // La cuadrícula pide la primera página sola al recibir la lista nueva
        Items = new ShopItemsCollection(LoadPageAsync);
    }

    private async Task<IReadOnlyList<ShopEmoteViewModel>?> LoadPageAsync(int page)
    {
        if (page > 1 && (page - 1) * EmoteShopCatalog.PageSize >= _total) return [];

        IsLoading = true;
        var result = await catalog.GetPageAsync(_query, page);
        IsLoading = false;
        if (!result.IsOk)
        {
            ErrorText = StoreTexts.CatalogError(result.Error, result.HttpCode);
            IsErrorVisible = true;
            return null;
        }

        _total = result.Total;
        IsEmpty = page == 1 && result.Emotes.Count == 0;
        return result.Emotes.Select(e => new ShopEmoteViewModel(e, _owned.Contains(e.Id), CanAdd, IsCartMode)).ToList();
    }

    // Siempre desde compras.json: si ya pasó la hora de reinicio, el cupo vuelve completo aquí
    private async Task RefreshQuotaAsync()
    {
        if (purchases.TakeRecreatedNotice()) Notice.Show(StoreTexts.Format("Text.Store.Recreated"), InfoBarSeverity.Warning);
        if (ActiveUser.Name is not { } name) return;
        try
        {
            var user = await purchases.GetAsync(name);
            _owned.Clear();
            _owned.UnionWith(user.Emotes.Select(e => e.Id));
            foreach (var item in AllItems) item.IsOwned = _owned.Contains(item.Emote.Id);
            ApplyQuota(QuotaCalculator.For(user, DateTime.UtcNow));
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.Error.Storage"), InfoBarSeverity.Error);
        }
    }

    private void ApplyQuota(QuotaInfo quota)
    {
        _quota = quota;
        QuotaText = StoreTexts.Quota(quota);
        IsBlocked = quota.IsBlocked;
        Cart.Capacity = quota.Remaining;
        Timer.Stop();
        if (quota is { IsBlocked: true, ResetsAt: { } resetsAt }) Timer.Start(resetsAt, quota.Limit);
        Refresh();
    }

    private async void OnTimerExpired() => await RefreshQuotaAsync();
    private void OnSummaryFinished(string message, InfoBarSeverity severity) => Notice.Show(message, severity);
    private bool CanAdd => !_quota.IsBlocked && !_isBuying && !(IsCartMode && Cart.IsFull);
    private IEnumerable<ShopEmoteViewModel> AllItems => Items ?? Enumerable.Empty<ShopEmoteViewModel>();

    // AGREGAR / COMPRAR se apagan con el cupo agotado, el carrito lleno o una compra guardándose
    private void Refresh()
    {
        var canAdd = CanAdd;
        foreach (var item in AllItems) item.CanBuy = canAdd;
        CanBuyCart = Cart.HasItems && !_quota.IsBlocked && !_isBuying;
    }
}
