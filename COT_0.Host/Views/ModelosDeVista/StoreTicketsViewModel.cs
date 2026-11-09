// Sección TICKETS: los tickets del usuario que hizo ENTRAR (el más nuevo primero) y DESCARGAR PDF
using System.Collections.ObjectModel;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Usuarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Cada usuario ve solo los suyos. Sin compras: "Aún no has comprado nada." con IR A LA TIENDA.</summary>
public partial class StoreTicketsViewModel(PurchaseService purchases) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTickets))]
    private bool _isEmpty;

    public ObservableCollection<TicketViewModel> Tickets { get; } = [];

    public NoticeViewModel Notice { get; } = new();

    public bool HasTickets => !IsEmpty;

    /// <summary>IR A LA TIENDA: la pantalla cambia de sección.</summary>
    public event Action? ShopRequested;

    public async Task LoadAsync()
    {
        Tickets.Clear();
        try
        {
            if (ActiveUser.Name is { } name)
            {
                var user = await purchases.GetAsync(name);
                foreach (var ticket in user.Tickets.OrderByDescending(t => t.Number)) Tickets.Add(new TicketViewModel(ticket));
            }
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.Error.Storage"), InfoBarSeverity.Error);
        }
        IsEmpty = Tickets.Count == 0;
    }

    [RelayCommand]
    private void GoShop() => ShopRequested?.Invoke();

    [RelayCommand]
    private async Task DownloadAsync(TicketViewModel item)
    {
        try
        {
            Notice.Show(StoreTexts.Format("Text.Store.PdfSaved", await TicketDownloads.SaveAsync(item.Ticket)), InfoBarSeverity.Success);
        }
        catch (Exception ex) when (TicketDownloads.IsSaveError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.PdfFailed", TicketDownloads.Folder), InfoBarSeverity.Error);
        }
    }
}
