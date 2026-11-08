// Nombre limpio de un fondo para mostrar abajo de su miniatura
using System.Text;
using System.Text.RegularExpressions;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// Los videos bajados de YouTube traen el ID entre corchetes al final y signos de ancho completo
/// ("：", "＂", "｜") porque Windows no deja ":" ni "|" en un nombre. Aquí solo se limpian para leerlos.
/// </summary>
public static partial class FondoNames
{
    public static string Clean(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName).Normalize(NormalizationForm.FormKC).Replace('_', ' ');
        name = TrailingId().Replace(name, string.Empty);
        return Spaces().Replace(name, " ").Trim(' ', '|', '-');
    }

    [GeneratedRegex(@"\s*\[[^\]]*\]\s*$")]
    private static partial Regex TrailingId();

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex Spaces();
}
