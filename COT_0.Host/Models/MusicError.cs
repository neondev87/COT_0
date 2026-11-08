// Problemas de la música que se muestran en un InfoBar
namespace COT_0.Host.Models;

/// <summary>Cada valor tiene su texto "Text.Music.Error.{valor}" en Textos.xaml.</summary>
public enum MusicError
{
    None,
    FolderMissing,
    NoTracks,
    Unreadable,
}
