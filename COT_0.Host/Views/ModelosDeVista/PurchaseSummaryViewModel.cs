// Después de comprar: sello COMPRA REALIZADA y luego el panel con el resumen y DESCARGAR PDF, SOLO VER PDF o SALIR
using System.Collections.ObjectModel;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Tienda;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// El PDF del ticket se genera siempre al comprar (TicketFiles), aunque elijas SALIR. ESC o clic en el velo = SALIR.
/// Al cerrar avisa con Finished el mensaje para el InfoBar de la tienda.
/// </summary>
public partial class PurchaseSummaryViewModel : ObservableObject
{
    private PurchaseTicket? _ticket;
    private Task<string?> _generating = Task.FromResult<string?>(null);

    [ObservableProperty]
    private bool _isSealVisible;

    [ObservableProperty]
    private bool _isOpen;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _titleText = string.Empty;

    [ObservableProperty]
    private string _totalText = string.Empty;

    public ObservableCollection<PurchaseSummaryLineViewModel> Lines { get; } = [];

    /// <summary>Se cerró el panel: mensaje y tipo para el aviso de la tienda.</summary>
    public event Action<string, InfoBarSeverity>? Finished;

    /// <summary>Sello (Store:PurchaseEffectMs) y luego el panel; mientras tanto el PDF se va generando.</summary>
    public async Task ShowAsync(PurchaseTicket ticket, IReadOnlyList<ShopEmoteViewModel> items)
    {
        _ticket = ticket;
        _generating = GenerateAsync(ticket);

        var bought = items.Where(i => ticket.Emotes.Any(e => e.Id == i.Emote.Id)).ToList();
        Lines.Clear();
        foreach (var item in bought) Lines.Add(new PurchaseSummaryLineViewModel(item.Name, item.PriceText, item.Thumbnail));
        TitleText = StoreTexts.Format("Text.Store.Summary.Title", TicketLines.Number(ticket.Number));
        TotalText = StoreTexts.Format("Text.Store.Summary.Total", EmotePricing.Format(bought.Sum(i => i.Price)));

        IsSealVisible = true;
        await Task.Delay(AppConfig.Settings.Store.PurchaseEffectMs);
        IsSealVisible = false;
        IsOpen = true;
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
        if (_ticket is not { } ticket || !Begin()) return;
        var (message, severity) = await TicketPdfActions.DownloadAsync(ticket);
        Close(message, severity);
    }

    [RelayCommand]
    private async Task ViewAsync()
    {
        if (_ticket is not { } ticket || !Begin()) return;
        var opened = await _generating is { } path && await TicketViewer.OpenAsync(path);
        Close(StoreTexts.Format(opened ? "Text.Store.Summary.Viewed" : "Text.Store.Summary.ViewFailed", TicketLines.Number(ticket.Number)),
            opened ? InfoBarSeverity.Success : InfoBarSeverity.Warning);
    }

    /// <summary>SALIR, ESC o clic en el velo: no hace nada más (el PDF ya se generó).</summary>
    [RelayCommand]
    private void Exit()
    {
        if (!IsOpen || IsBusy) return;
        Close(StoreTexts.Format("Text.Store.Shop.Bought", _ticket?.Emotes.Count ?? 0), InfoBarSeverity.Success);
    }

    private bool Begin()
    {
        if (!IsOpen || IsBusy) return false;
        IsBusy = true;
        return true;
    }

    private void Close(string message, InfoBarSeverity severity)
    {
        IsBusy = false;
        IsOpen = false;
        Lines.Clear();
        Finished?.Invoke(message, severity);
    }

    // Un fallo al generar no detiene la compra: SOLO VER avisa y DESCARGAR lo intenta otra vez
    private static async Task<string?> GenerateAsync(PurchaseTicket ticket)
    {
        try
        {
            return await TicketFiles.EnsureAsync(ticket);
        }
        catch (Exception ex) when (TicketDownloads.IsSaveError(ex))
        {
            return null;
        }
    }
}
