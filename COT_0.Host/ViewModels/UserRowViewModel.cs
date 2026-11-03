using COT_0.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.ViewModels;

// Fila de un invitado en la lista de conectados
public partial class UserRowViewModel : ObservableObject
{
    private const int InitialsLength = 2;
    private const string UnknownInitials = "?";

    private readonly DateTime _joinedAt;
    private readonly string _sinceFormat;

    [ObservableProperty]
    private string _since = string.Empty;

    public UserRowViewModel(User user, string sinceFormat)
    {
        Id = user.Id;
        Name = user.Name;
        Initials = BuildInitials(user.Name);
        _joinedAt = user.JoinedAt;
        _sinceFormat = sinceFormat;
        Refresh();
    }

    public string Id { get; }
    public string Name { get; }
    public string Initials { get; }

    public void Refresh() =>
        Since = string.Format(_sinceFormat, (int)(DateTime.UtcNow - _joinedAt).TotalMinutes);

    private static string BuildInitials(string name)
    {
        var letters = new string(name.Where(char.IsLetter).Take(InitialsLength).ToArray());
        return letters.Length > 0 ? letters.ToUpperInvariant() : UnknownInitials;
    }
}
