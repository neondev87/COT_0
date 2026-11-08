// Problemas de los fondos que se muestran en un InfoBar
namespace COT_0.Host.Models;

/// <summary>
/// Cada valor (menos None) tiene su texto "Text.Backgrounds.Error.{valor}" en Textos.xaml
/// ({0} = carpeta o archivo, {1} = código de Windows cuando hay).
/// </summary>
public enum FondoError
{
    None,
    FolderMissing,
    Empty,
    Unreadable,
    Storage,
    FileMissing,
    FileLocked,
    VideoCodec,
    VideoDecode,
    NoFrame,
}
