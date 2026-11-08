// Estado de los fondos para toda la app: catálogo, fondo elegido y último problema; guarda en fondos.json
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// Una sola instancia (la comparten la capa del fondo y FondosPage). Al releer la carpeta conserva lo que
/// elegiste por archivo (cuadro fijo o video). Changed se avisa en el hilo de quien llamó (la UI).
/// </summary>
public sealed class FondoService(FondoCatalog catalog, FondoStore store)
{
    private FondosGuardados _data = new();
    private Task? _loading;

    public event Action? Changed;

    public bool IsLoaded { get; private set; }

    public IReadOnlyList<Fondo> Items => _data.Items;

    public Fondo? Selected => Items.FirstOrDefault(f => f.FileName.Equals(SelectedName, StringComparison.OrdinalIgnoreCase));

    public FondoError Error { get; private set; }

    /// <summary>Carpeta, archivo o motivo del último problema.</summary>
    public string? ErrorDetail { get; private set; }

    public string PathOf(Fondo fondo) => catalog.PathOf(fondo);

    // Nunca elegiste: el de Background:DefaultFile
    private string SelectedName => _data.SelectedFile ?? AppConfig.Settings.Background.DefaultFile;

    /// <summary>Relee la carpeta y la mezcla con lo guardado; si ya se está leyendo, espera esa misma lectura.</summary>
    public Task LoadAsync() => _loading is { IsCompleted: false } ? _loading : _loading = ReadAsync();

    private async Task ReadAsync()
    {
        SetError(FondoError.None, null);
        var saved = await TryLoadSavedAsync();
        var scanned = await catalog.ScanAsync();

        _data = new FondosGuardados
        {
            SelectedFile = saved.SelectedFile,
            Items = scanned.Select(f => Merge(f, saved.Items)).ToList(),
        };
        IsLoaded = true;

        if (!catalog.FolderExists) SetError(FondoError.FolderMissing, catalog.Folder);
        else if (scanned.Count == 0) SetError(FondoError.Empty, catalog.Folder);
        await TrySaveAsync();
        Changed?.Invoke();
    }

    /// <summary>null = sin fondo.</summary>
    public async Task SelectAsync(Fondo? fondo)
    {
        _data.SelectedFile = fondo?.FileName ?? string.Empty;
        SetError(FondoError.None, null);
        await TrySaveAsync();
        Changed?.Invoke();
    }

    public async Task SetUseStillAsync(Fondo fondo, bool useStill)
    {
        var index = _data.Items.IndexOf(fondo);
        if (index < 0 || fondo.Kind != FondoKind.Video) return;
        _data.Items[index] = fondo with { UseStill = useStill };
        await TrySaveAsync();
        Changed?.Invoke();
    }

    /// <summary>El archivo no se pudo abrir (lo avisa la capa del fondo).</summary>
    public void ReportUnreadable(Fondo fondo)
    {
        SetError(FondoError.Unreadable, fondo.FileName);
        Changed?.Invoke();
    }

    // Lo guardado gana en UseStill; el tamaño y el tipo salen del disco
    private static Fondo Merge(Fondo scanned, IReadOnlyList<Fondo> saved) =>
        saved.FirstOrDefault(s => s.FileName.Equals(scanned.FileName, StringComparison.OrdinalIgnoreCase)) is { } old
            ? scanned with { UseStill = scanned.Kind == FondoKind.Video && old.UseStill }
            : scanned;

    // fondos.json dañado o sin permiso: se avisa y se empieza de cero
    private async Task<FondosGuardados> TryLoadSavedAsync()
    {
        try
        {
            return await store.LoadAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            SetError(FondoError.Storage, ex.Message);
            return new FondosGuardados();
        }
    }

    private async Task TrySaveAsync()
    {
        try
        {
            await store.SaveAsync(_data);
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            SetError(FondoError.Storage, ex.Message);
        }
    }

    private void SetError(FondoError error, string? detail)
    {
        Error = error;
        ErrorDetail = detail;
    }
}
