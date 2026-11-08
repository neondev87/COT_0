using System.Globalization;

namespace COT_0.Web.Services.Manual;

// Tiempos de los avisos de Lain y de los resaltados, como variables CSS (--in / --out)
public static class ManualTimeline
{
    public static IReadOnlyList<ManualCue> Cues(IReadOnlyList<string> texts, ManualOptions options) =>
        texts.Select((text, i) => new ManualCue(text, Style("--", i, texts.Count, options), i < texts.Count - 1)).ToList();

    // Claves resaltadas (data-focus) y sus tiempos: --name-in, --name-out…
    public static ManualFocus Focus(IReadOnlyList<ManualNotice> notices, ManualOptions options)
    {
        var focused = notices.Select((notice, i) => (notice.Focus, i)).Where(n => n.Focus is not null).ToList();
        var keys = string.Join(' ', focused.Select(n => n.Focus));
        var style = string.Join(';', focused.Select(n => Style($"--{n.Focus}-", n.i, notices.Count, options)));
        return new ManualFocus(keys, style);
    }

    // El último aviso no sale: se queda hasta cambiar de paso
    private static string Style(string prefix, int index, int count, ManualOptions options)
    {
        var start = options.NoticeDelaySeconds + index * options.NoticeIntervalSeconds;
        var style = $"{prefix}in:{Seconds(start)}";
        if (index == count - 1) return style;
        return $"{style};{prefix}out:{Seconds(start + options.NoticeIntervalSeconds - options.NoticeGapSeconds)}";
    }

    public static string Seconds(double value) => value.ToString("0.##", CultureInfo.InvariantCulture) + "s";
}

public sealed record ManualFocus(string Keys, string Style);
