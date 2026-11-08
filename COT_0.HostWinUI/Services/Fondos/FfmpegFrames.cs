// Saca un cuadro de un video con ffmpeg (si ya está junto a la app o en el PATH; no se instala ni se descarga nada)
using System.Diagnostics;
using System.Globalization;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Registro;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// Primer intento del cuadro fijo: es el más rápido y lee cualquier códec que traiga ffmpeg.
/// Cuadro cerca de Background:FrameSeconds (si el video es más corto, el primero), máximo FrameMaxWidth de ancho.
/// Los argumentos van por ArgumentList: los nombres con "：", "＂", corchetes o espacios llegan tal cual.
/// </summary>
public class FfmpegFrames
{
    private readonly Lazy<Task<string?>> _exe = new(() => Task.Run(Locate));

    private static BackgroundSettings Settings => AppConfig.Settings.Background;

    public async Task<bool> TryExtractAsync(string video, string output)
    {
        if (await _exe.Value is not { } exe) return false;
        return await RunAsync(exe, video, output, Settings.FrameSeconds) || await RunAsync(exe, video, output, 0);
    }

    private static async Task<bool> RunAsync(string exe, string video, string output, double seconds)
    {
        var info = new ProcessStartInfo(exe) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true };
        foreach (var arg in Arguments(video, output, seconds)) info.ArgumentList.Add(arg);

        using var process = Process.Start(info);
        if (process is null) return false;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(Settings.FfmpegTimeoutSeconds));
        var errors = process.StandardError.ReadToEndAsync(CancellationToken.None);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            await MediaLog.WriteAsync("ffmpeg", video, null, $"tardó más de {Settings.FfmpegTimeoutSeconds} s");
            return false;
        }

        if (process.ExitCode == 0 && await Task.Run(() => File.Exists(output) && new FileInfo(output).Length > 0)) return true;
        await MediaLog.WriteAsync("ffmpeg", video, $"exit {process.ExitCode}", (await errors).Trim());
        return false;
    }

    private static IEnumerable<string> Arguments(string video, string output, double seconds) =>
    [
        "-hide_banner", "-loglevel", "error",
        "-ss", seconds.ToString(CultureInfo.InvariantCulture),
        "-i", video,
        "-frames:v", "1",
        "-vf", $"scale='min({Settings.FrameMaxWidth},iw)':-2:flags=lanczos",
        "-q:v", "3", "-y", output,
    ];

    // Ruta fija, junto al .exe o en alguna carpeta del PATH; null si no está
    private static string? Locate()
    {
        var configured = Settings.FfmpegPath;
        if (Path.IsPathRooted(configured)) return File.Exists(configured) ? configured : null;

        var name = Path.HasExtension(configured) ? configured : configured + ".exe";
        var folders = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        return folders.Prepend(AppContext.BaseDirectory)
            .Select(folder => Path.Combine(folder.Trim('"'), name))
            .FirstOrDefault(File.Exists);
    }
}
