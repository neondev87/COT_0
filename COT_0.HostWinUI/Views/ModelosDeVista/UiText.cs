// Busca un texto de Views/Estilos/Textos.xaml por su clave
using Microsoft.UI.Xaml;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Atajo para que los ViewModels usen los mismos textos que el XAML.</summary>
public static class UiText
{
    public static string Get(string key) =>
        Application.Current.Resources.TryGetValue(key, out var value) && value is string text ? text : key;
}
