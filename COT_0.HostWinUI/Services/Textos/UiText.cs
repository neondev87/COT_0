// Busca un texto de Views/Estilos/Textos.xaml por su clave
using Microsoft.UI.Xaml;

namespace COT_0.HostWinUI.Services.Textos;

/// <summary>Atajo para que los ViewModels usen los mismos textos que el XAML.</summary>
public static class UiText
{
    public static string Get(string key) => Find(key) ?? key;

    /// <summary>null si la clave no existe (para textos opcionales, como el título del primer paso del manual).</summary>
    public static string? Find(string key) =>
        Application.Current.Resources.TryGetValue(key, out var value) && value is string text ? text : null;
}
