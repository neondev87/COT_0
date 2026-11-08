using System.Globalization;

namespace COT_0.Web.Services.Voz;

// Formato de duraciones de las notas de voz
public static class VoiceFormat
{
    // m:ss (0:00 si no es válida)
    public static string Duration(TimeSpan duration)
    {
        var seconds = Math.Max(0, (int)Math.Round(duration.TotalSeconds));
        return $"{seconds / 60}:{seconds % 60:00}";
    }

    // Segundos con punto, para data-duration (lo lee voice.js)
    public static string Seconds(TimeSpan duration) => duration.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
}
