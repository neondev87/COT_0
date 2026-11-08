namespace COT_0.Core.Services;

/// <summary>Archivos fuera del repo (fondos, música, manual): en la configuración van relativos a Paths:MediaRoot, así en otra PC solo cambia esa ruta. Una ruta absoluta se respeta tal cual</summary>
public static class MediaPath
{
    public const string RootKey = "Paths:MediaRoot";

    public static string Resolve(string? root, string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;
        return Path.IsPathRooted(path) || string.IsNullOrWhiteSpace(root) ? path : Path.Combine(root, path);
    }
}
