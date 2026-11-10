// PUNTO DE VENTA (modo ADMIN, solo lectura): usuarios, todos los tickets con su dueño, buscador, totales y DESCARGAR PDF
using System.Collections.ObjectModel;
using System.Globalization;
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Usuarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Lee compras.json completo con PurchaseService.GetAllAsync (vacío fuera del modo ADMIN). Al elegir un usuario
/// se ven solo sus tickets; el buscador filtra por nombre o por número de ticket. Aquí no se compra nada.
/// </summary>
public partial class SalesViewModel(PurchaseService purchases) : ObservableObject
{
    private List<TicketViewModel> _all = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private SalesUserViewModel? _selectedUser;

    [ObservableProperty]
    private string _totalsText = string.Empty;

    [ObservableProperty]
    private bool _isEmpty;

    public ObservableCollection<SalesUserViewModel> Users { get; } = [];

    public ObservableCollection<TicketViewModel> Tickets { get; } = [];

    public NoticeViewModel Notice { get; } = new();

    public async Task LoadAsync()
    {
        try
        {
            var data = await purchases.GetAllAsync();
            if (purchases.TakeRecreatedNotice()) Notice.Show(StoreTexts.Format("Text.Store.Recreated"), InfoBarSeverity.Warning);
            var accounts = await HostAccounts.LoadAsync();
            Fill(data, accounts.Select(a => a.UserName));
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            Notice.Show(StoreTexts.Format("Text.Store.Error.Storage"), InfoBarSeverity.Error);
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSelectedUserChanged(SalesUserViewModel? value) => ApplyFilter();

    [RelayCommand]
    private async Task DownloadAsync(TicketViewModel item)
    {
        var (message, severity) = await TicketPdfActions.DownloadAsync(item.Ticket);
        Notice.Show(message, severity);
    }

    // Usuarios registrados más los que tengan compras (por si el archivo trae alguno que ya no está en usuarios.json)
    private void Fill(PurchasesData data, IEnumerable<string> registered)
    {
        _all = data.Users
            .SelectMany(u => u.Tickets.Select(t => new TicketViewModel(t, u.UserName)))
            .OrderByDescending(t => t.Ticket.Number)
            .ToList();
        TotalsText = StoreTexts.Format("Text.Sales.Totals", _all.Count, _all.Sum(t => t.Ticket.Emotes.Count));

        Users.Clear();
        Users.Add(new SalesUserViewModel(null, StoreTexts.Format("Text.Sales.AllUsers"), TotalsText));
        var names = registered.Concat(data.Users.Select(u => u.UserName)).Distinct(StringComparer.OrdinalIgnoreCase);
        foreach (var name in names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
        {
            var tickets = _all.Where(t => Same(t.OwnerName, name)).ToList();
            Users.Add(new SalesUserViewModel(name, name, StoreTexts.Format("Text.Sales.UserLine", tickets.Count, tickets.Sum(t => t.Ticket.Emotes.Count))));
        }
        SelectedUser = Users[0];
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var text = SearchText.Trim();
        Tickets.Clear();
        foreach (var ticket in _all.Where(t => (SelectedUser?.Name is not { } user || Same(t.OwnerName, user)) && Matches(t, text)))
            Tickets.Add(ticket);
        IsEmpty = Tickets.Count == 0;
    }

    // Por nombre (contiene) o por número ("12" encuentra el #0012)
    private static bool Matches(TicketViewModel ticket, string text) =>
        text.Length == 0
        || ticket.OwnerName.Contains(text, StringComparison.OrdinalIgnoreCase)
        || (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && ticket.Ticket.Number == number);

    private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
