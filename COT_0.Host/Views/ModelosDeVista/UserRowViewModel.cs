// Fila de un invitado en la lista de conectados: nombre, iniciales, tono del avatar y "hace N min"
using COT_0.Core.Models;
using COT_0.Host.Services.Tiempo;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

public partial class UserRowViewModel : ObservableObject
{
    private const int InitialsLength = 2;
    private const int ToneCount = 5;
    private const string UnknownInitials = "?";

    private readonly DateTime _joinedAt;
    private readonly string _sinceFormat;
    private readonly ClockService _clock;

    [ObservableProperty]
    private string _since = string.Empty;

    /// <param name="position">Lugar en la lista: los tonos van 1,2,3,4,0 porque el 0 es del host.</param>
    public UserRowViewModel(User user, int position, string sinceFormat, ClockService clock)
    {
        Id = user.Id;
        Name = user.Name;
        Initials = BuildInitials(user.Name);
        Tone = (position + 1) % ToneCount;
        _joinedAt = user.JoinedAt;
        _sinceFormat = sinceFormat;
        _clock = clock;
        Refresh();
    }

    public string Id { get; }
    public string Name { get; }
    public string Initials { get; }
    public int Tone { get; }

    public void Refresh() =>
        Since = string.Format(_sinceFormat, (int)_clock.Since(_joinedAt).TotalMinutes);

    private static string BuildInitials(string name)
    {
        var letters = new string(name.Where(char.IsLetter).Take(InitialsLength).ToArray());
        return letters.Length > 0 ? letters.ToUpperInvariant() : UnknownInitials;
    }
}
