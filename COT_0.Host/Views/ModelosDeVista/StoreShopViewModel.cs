// Sección TIENDA: cuadrícula de todo 7TV por páginas, buscador, contador de cupo y COMPRAR con su ticket
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Dialogos;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Usuarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Compra para el usuario que hizo ENTRAR (ActiveUser). Si 7TV falla, aviso con REINTENTAR y la app sigue.
/// Al comprar pregunta si quieres el PDF; con o sin él, el ticket queda en TICKETS.
/// </summary>
public partial class StoreShopViewModel(PurchaseService purchases, EmoteShopCatalog catalog, ConfirmDialogService dialogs) : ObservableObject
{
    private readonly HashSet<string> _owned = [];
    private QuotaInfo _quota = new(0, 0, 0, null);
    private string _query = string.Empty;
    private int _total;
    private bool _isBuying;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ShopItemsCollection? _items;

    [ObservableProperty]
    private string _quotaText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isErrorVisible;

    [ObservableProperty]
    private string _errorText = string.Empty;

    public NoticeViewModel Notice { get; } = new();

    /// <summary>Al abrir la tienda: relee el cupo (se reinicia solo si ya pasaron las horas) y carga la primera página una vez.</summary>
    public async Task LoadAsync()
    {
        await RefreshQuotaAsync();
        if (Items is null) StartSearch(string.Empty);
    }

    [RelayCommand]
    private void Search() => StartSearch(SearchText);

    [RelayCommand]
    private async Task RetryAsync()
    {
        IsErrorVisible = false;
        if (Items is not null) await Items.ResumeAsync();
    }

    [RelayCommand]
    private async Task BuyAsync(ShopEmoteViewModel item)
    {
        if (_isBuying || !item.IsBuyEnabled) return;

        SetBuying(true);
        try
        {
            var result = await purchases.BuyAsync(ActiveUser.Name, item.Emote);
            if (result.Outcome != PurchaseOutcome.NoUser) ApplyQuota(result.Quota);
            await ShowOutcomeAsync(result, item);
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.Error.Storage"), InfoBarSeverity.Error);
        }
        finally
        {
            SetBuying(false);
        }
    }

    private async Task ShowOutcomeAsync(PurchaseResult result, ShopEmoteViewModel item)
    {
        switch (result.Outcome)
        {
            case PurchaseOutcome.Bought when result.Ticket is { } ticket:
                _owned.Add(item.Emote.Id);
                item.IsOwned = true;
                var (message, severity) = await OfferPdfAsync(ticket, item.Name);
                // La décima compra también avisa que el cupo se acabó y cuándo vuelve
                if (_quota.IsBlocked) (message, severity) = ($"{message} {StoreTexts.Blocked(_quota)}", InfoBarSeverity.Warning);
                Notice.Show(message, severity);
                break;
            case PurchaseOutcome.LimitReached:
                Notice.Show(StoreTexts.Blocked(result.Quota), InfoBarSeverity.Warning);
                break;
            default:
                Notice.Show(StoreTexts.Format($"Text.Store.Outcome.{result.Outcome}"), InfoBarSeverity.Informational);
                break;
        }
    }

    // "¿Quieres descargar tu ticket en PDF?" Sí: lo guarda en el Escritorio. No: queda solo en TICKETS
    private async Task<(string Message, InfoBarSeverity Severity)> OfferPdfAsync(PurchaseTicket ticket, string emoteName)
    {
        var texts = new ConfirmTexts(StoreTexts.Format("Text.Store.PdfDialog.Message"),
            StoreTexts.Format("Text.Store.PdfDialog.Confirm"), StoreTexts.Format("Text.Store.PdfDialog.Cancel"));
        if (!await dialogs.ConfirmAsync(texts)) return (StoreTexts.Format("Text.Store.Shop.Bought", emoteName), InfoBarSeverity.Success);

        try
        {
            return (StoreTexts.Format("Text.Store.PdfSaved", await TicketDownloads.SaveAsync(ticket)), InfoBarSeverity.Success);
        }
        catch (Exception ex) when (TicketDownloads.IsSaveError(ex))
        {
            return (StoreTexts.Format("Text.Store.PdfFailed", TicketDownloads.Folder), InfoBarSeverity.Error);
        }
    }

    private void StartSearch(string query)
    {
        _query = query;
        _total = 0;
        IsEmpty = IsErrorVisible = false;
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
        return result.Emotes.Select(e => new ShopEmoteViewModel(e, _owned.Contains(e.Id), !_quota.IsBlocked && !_isBuying)).ToList();
    }

    private async Task RefreshQuotaAsync()
    {
        if (purchases.TakeRecreatedNotice()) Notice.Show(StoreTexts.Format("Text.Store.Recreated"), InfoBarSeverity.Warning);
        if (ActiveUser.Name is not { } name) return;
        try
        {
            var user = await purchases.GetAsync(name);
            _owned.Clear();
            _owned.UnionWith(user.Emotes.Select(e => e.Id));
            ApplyQuota(QuotaCalculator.For(user, DateTime.UtcNow));
            foreach (var item in Items ?? Enumerable.Empty<ShopEmoteViewModel>()) item.IsOwned = _owned.Contains(item.Emote.Id);
            if (_quota.IsBlocked) Notice.Show(StoreTexts.Blocked(_quota), InfoBarSeverity.Warning);
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
        SetBuying(_isBuying);
    }

    // Con el cupo agotado o mientras se guarda una compra, COMPRAR queda apagado en todas las tarjetas
    private void SetBuying(bool isBuying)
    {
        _isBuying = isBuying;
        foreach (var item in Items ?? Enumerable.Empty<ShopEmoteViewModel>()) item.CanBuy = !_quota.IsBlocked && !isBuying;
    }
}
