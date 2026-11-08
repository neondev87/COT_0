// Problemas de los fondos que se muestran en un InfoBar
namespace COT_0.HostWinUI.Models;

/// <summary>Cada valor (menos None) tiene su texto "Text.Backgrounds.Error.{valor}" en Textos.xaml.</summary>
public enum FondoError
{
    None,
    FolderMissing,
    Empty,
    Unreadable,
    Storage,
}
