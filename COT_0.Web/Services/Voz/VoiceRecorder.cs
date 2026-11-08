using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace COT_0.Web.Services.Voz;

public sealed class VoiceRecorder(IJSRuntime js, IVoiceService voice, IOptions<VoiceOptions> options, ILogger<VoiceRecorder> logger)
    : IVoiceRecorder
{
    private readonly VoiceOptions _options = options.Value;
    private DotNetObjectReference<VoiceRecorder>? _self;
    private ElementReference _composer;
    private string? _userId;
    private DateTime _startedAt;
    private Timer? _clock;

    public VoiceState State { get; private set; }
    public TimeSpan Elapsed => State == VoiceState.Recording ? DateTime.UtcNow - _startedAt : TimeSpan.Zero;
    public string? Notice { get; private set; }

    public event Action? Changed;

    public async Task AttachAsync(ElementReference composer, string userId)
    {
        _composer = composer;
        _userId = userId;
        _self ??= DotNetObjectReference.Create(this);

        var config = new
        {
            _options.MimeCandidates,
            MaxMs = _options.MaxSeconds * 1000,
            MinMs = _options.MinMilliseconds,
        };
        await js.InvokeVoidAsync("cotVoice.attach", composer, _self, config);
    }

    public void DismissNotice() => Update(State, null);

    [JSInvokable]
    public void OnStarted()
    {
        _startedAt = DateTime.UtcNow;
        _clock ??= new Timer(_ => Changed?.Invoke(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        Update(VoiceState.Recording, null);
    }

    [JSInvokable]
    public void OnFailed(string code) => Update(VoiceState.Idle, VoiceNotice.ForError(code));

    [JSInvokable]
    public void OnCancelled(bool tooShort) => Update(VoiceState.Idle, tooShort ? VoiceNotice.TooShort : null);

    // Llega como stream: no choca con el límite de 32 KB por mensaje de SignalR
    [JSInvokable]
    public async Task OnRecorded(IJSStreamReference audio, int durationMs)
    {
        // Seguro contra doble envío: una sola nota subiendo a la vez
        if (State == VoiceState.Sending)
        {
            await audio.DisposeAsync();
            return;
        }

        Update(VoiceState.Sending, null);
        string? notice;

        try
        {
            await using var stream = await audio.OpenReadStreamAsync(_options.MaxBytes);
            notice = (await voice.SendAsync(_userId!, stream, durationMs)).Error;
        }
        catch (Exception ex)
        {
            // Más grande que Voice:MaxBytes, se cortó la conexión o falló el navegador
            logger.LogWarning(ex, "No se pudo recibir la nota de voz");
            notice = VoiceNotice.SendFailed;
        }
        finally
        {
            await audio.DisposeAsync();
        }

        Update(VoiceState.Idle, notice);
    }

    public async Task DetachAsync()
    {
        StopClock();
        State = VoiceState.Idle;
        Notice = null;
        if (_self is null) return;

        try
        {
            // Apaga el micrófono si se va de la página grabando
            await js.InvokeVoidAsync("cotVoice.detach", _composer);
        }
        catch (Exception ex) when (ex is JSDisconnectedException or JSException or TaskCanceledException)
        {
            logger.LogDebug(ex, "El navegador ya no estaba para soltar el micrófono.");
        }

        _self.Dispose();
        _self = null;
    }

    public async ValueTask DisposeAsync() => await DetachAsync();

    private void Update(VoiceState state, string? notice)
    {
        State = state;
        Notice = notice;
        if (state != VoiceState.Recording) StopClock();
        Changed?.Invoke();
    }

    private void StopClock()
    {
        _clock?.Dispose();
        _clock = null;
    }
}
