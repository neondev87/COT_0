// Videos, sonidos y capturas del manual de Lain, fuera del repo (Manual:*, relativas a Paths:MediaRoot)
using COT_0.Core.Services;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Manual;

/// <summary>
/// Async: RefreshAsync revisa el disco una vez al abrir el manual
/// y cada ruta queda en null si su archivo no está (el manual sigue sin él y lo avisa).
/// </summary>
public sealed class ManualAssets
{
    public string? LainVideo { get; private set; }

    /// <summary>Lain con transparencia (GIF opcional); si existe, el video de Lain ya no es obligatorio.</summary>
    public string? LainCutout { get; private set; }

    public string? FarewellVideo { get; private set; }

    public string? NotificationSound { get; private set; }

    public string? StaticSound { get; private set; }

    /// <summary>Rutas que no se encontraron en la última revisión.</summary>
    public IReadOnlyList<string> Missing { get; private set; } = [];

    private static ManualSettings Settings => AppConfig.Settings.Manual;

    public Task RefreshAsync() => Task.Run(() =>
    {
        var missing = new List<string>();
        LainCutout = Find(Settings.LainCutout, []);
        LainVideo = Find(Settings.LainVideo, LainCutout is null ? missing : []);
        FarewellVideo = Find(Settings.FarewellVideo, missing);
        NotificationSound = Find(Settings.NotificationSound, missing);
        StaticSound = Find(Settings.StaticSound, missing);
        Missing = missing;
    });

    /// <summary>Captura opcional del paso (Manual:StagesFolder\pasoN.png); null si no hay.</summary>
    public Task<string?> FindStageAsync(int number) => Task.Run(() =>
    {
        var path = Path.Combine(Resolve(Settings.StagesFolder), $"paso{number}{Settings.StageExtension}");
        return File.Exists(path) ? path : null;
    });

    private static string? Find(string relative, List<string> missing)
    {
        var path = Resolve(relative);
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path)) return path;
        missing.Add(path);
        return null;
    }

    private static string Resolve(string relative) => MediaPath.Resolve(AppConfig.Settings.Paths.MediaRoot, relative);
}
