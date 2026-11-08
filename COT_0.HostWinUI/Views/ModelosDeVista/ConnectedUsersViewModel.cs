// Lista de conectados: se recarga cuando el chat avisa y refresca el "hace N min" cada segundo
using System.Collections.ObjectModel;
using COT_0.Core.Services;
using COT_0.HostWinUI.Services.Textos;
using COT_0.HostWinUI.Services.Tiempo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>La parte de RoomViewModel (WPF) que toca a los invitados.</summary>
public partial class ConnectedUsersViewModel : ObservableObject
{
    private readonly IChatService _chat;
    private readonly ClockService _clock;

    [ObservableProperty]
    private int _total;

    public ConnectedUsersViewModel(IChatService chat, ClockService clock)
    {
        _chat = chat;
        _clock = clock;

        clock.Tick += RefreshTimes;
        chat.UsersChanged += () => UiThread.Run(() => _ = ReloadAsync());
        _ = ReloadAsync();
    }

    public ObservableCollection<UserRowViewModel> Users { get; } = [];

    [RelayCommand]
    private Task KickAsync(UserRowViewModel user) => _chat.KickUserAsync(user.Id);

    private async Task ReloadAsync()
    {
        var sinceFormat = UiText.Get("Text.Room.Since");
        var users = await _chat.GetUsersAsync();

        Users.Clear();
        foreach (var user in users.OrderBy(u => u.JoinedAt))
            Users.Add(new UserRowViewModel(user, Users.Count, sinceFormat, _clock));
        Total = Users.Count;
    }

    private void RefreshTimes()
    {
        foreach (var user in Users) user.Refresh();
    }
}
