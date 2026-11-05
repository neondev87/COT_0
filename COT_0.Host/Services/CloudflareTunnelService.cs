using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using COT_0.Core.Services;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Quick tunnel de cloudflared hacia localhost:5050
public partial class CloudflareTunnelService(IConfiguration configuration) : ITunnelService, IDisposable
{
    // cloudflared queda atado a la vida de COT_0, incluso si se cierra de golpe
    private readonly KillOnCloseJob _job = new();
    private Process? _process;

    public string? PublicUrl { get; private set; }
    public bool IsRunning => _process is { HasExited: false };

    public async Task<string> StartAsync(string localUrl, CancellationToken cancellationToken = default)
    {
        if (IsRunning && PublicUrl is not null) return PublicUrl;
        await StopAsync();

        var executable = configuration["Tunnel:CloudflaredPath"] ?? "cloudflared";
        var timeoutSeconds = configuration.GetValue("Tunnel:StartTimeoutSeconds", 30);
        var urlFound = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        var process = CreateProcess(executable, localUrl);
        // cloudflared escribe la URL en stderr; se revisan ambas salidas
        process.OutputDataReceived += (_, e) => TryExtractUrl(e.Data, urlFound);
        process.ErrorDataReceived += (_, e) => TryExtractUrl(e.Data, urlFound);
        process.Exited += (_, _) => urlFound.TrySetException(
            new InvalidOperationException("cloudflared se cerró antes de entregar la URL pública."));

        try
        {
            process.Start();
        }
        catch (Win32Exception)
        {
            process.Dispose();
            throw new InvalidOperationException(
                $"No se encontró cloudflared (\"{executable}\").\n\n" +
                "Instálalo con: winget install Cloudflare.cloudflared\n" +
                "o configura la ruta en appsettings.json → Tunnel:CloudflaredPath.");
        }

        _process = process;
        _job.Add(process);
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            PublicUrl = await urlFound.Task.WaitAsync(TimeSpan.FromSeconds(timeoutSeconds), cancellationToken);
            return PublicUrl;
        }
        catch (TimeoutException)
        {
            await StopAsync();
            throw new TimeoutException($"cloudflared no entregó la URL en {timeoutSeconds} segundos. Revisa tu conexión.");
        }
        catch
        {
            await StopAsync();
            throw;
        }
    }

    public async Task StopAsync()
    {
        PublicUrl = null;
        if (_process is null) return;

        var process = _process;
        _process = null;
        if (KillProcess(process)) await process.WaitForExitAsync();
        process.Dispose();
    }

    public void Dispose()
    {
        PublicUrl = null;
        if (_process is not null) KillProcess(_process);
        _process?.Dispose();
        _process = null;
        _job.Dispose();
        GC.SuppressFinalize(this);
    }

    private static Process CreateProcess(string executable, string localUrl) => new()
    {
        EnableRaisingEvents = true,
        StartInfo = new ProcessStartInfo(executable, $"tunnel --url {localUrl}")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        },
    };

    private static void TryExtractUrl(string? line, TaskCompletionSource<string> urlFound)
    {
        if (line is null) return;
        var match = PublicUrlRegex().Match(line);
        if (match.Success) urlFound.TrySetResult(match.Value);
    }

    // Devuelve true si el proceso seguía vivo y se mató
    private static bool KillProcess(Process process)
    {
        try
        {
            if (process.HasExited) return false;
            process.Kill(entireProcessTree: true);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    [GeneratedRegex(@"https://(?!api\.)[a-z0-9-]+\.trycloudflare\.com", RegexOptions.IgnoreCase)]
    private static partial Regex PublicUrlRegex();
}
