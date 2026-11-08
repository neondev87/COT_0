// Abre archivos en modo asíncrono (FileStream con useAsync) para no trabar la interfaz
namespace COT_0.Host.Services.Almacenamiento;

/// <summary>Reemplazo de File.OpenRead/File.Create con E/S asíncrona real.</summary>
public static class AsyncFiles
{
    private const int BufferSize = 4096;

    /// <summary>Abre para leer (el archivo debe existir).</summary>
    public static FileStream OpenRead(string path) =>
        new(path, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, useAsync: true);

    /// <summary>Crea o sobrescribe el archivo para escribir.</summary>
    public static FileStream Create(string path) =>
        new(path, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, useAsync: true);
}
