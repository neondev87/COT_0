// Tarjeta de un usuario en USUARIOS (04): foto de perfil (o inicial con color), nombre y qué acciones puedes usar
using COT_0.Host.Services.Perfiles;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Views.Convertidores;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Nunca trae contraseña, hash ni pregunta secreta. Tone sale del nombre (UserTones), igual que en la firma.
/// CanManage: es tu cuenta (nombre, contraseña y foto). CanDelete: tu cuenta, o cualquiera en modo ADMIN.
/// </summary>
public partial class UserCardViewModel(string name) : ObservableObject
{
    private const string UnknownInitial = "?";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPhoto), nameof(HasInitial))]
    private BitmapImage? _photo;

    public string Name { get; } = name;

    public string Initial { get; } = name.Length > 0 ? char.ToUpperInvariant(name[0]).ToString() : UnknownInitial;

    public int Tone { get; } = UserTones.For(name);

    public bool HasPhoto => Photo is not null;

    public bool HasInitial => Photo is null;

    public bool CanManage { get; } = AccountService.CanManage(name);

    public bool CanDelete { get; } = AccountService.CanDelete(name);

    public async Task LoadPhotoAsync() => Photo = await ProfileImages.LoadAsync(Name);
}
