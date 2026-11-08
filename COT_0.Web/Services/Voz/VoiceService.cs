using COT_0.Core.Models;
using COT_0.Core.Services;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Voz;

/// <summary>Implementa IVoiceService: valida, guarda y publica las notas de voz.</summary>
public sealed class VoiceService(IChatService chat, IVoiceStore store, IAudioTranscoder transcoder, IOptions<VoiceOptions> options)
    : IVoiceService
{
    private readonly VoiceOptions _options = options.Value;

    public async Task<OperationResult<VoiceClip>> SendAsync(string userId, Stream audio, int durationMs)
    {
        if (await chat.GetUserAsync(userId) is null) return OperationResult<VoiceClip>.Fail("Ya no estás en la sala.");

        var bytes = await ReadAsync(audio);
        if (bytes is null) return OperationResult<VoiceClip>.Fail(VoiceNotice.TooLarge);

        var id = Guid.NewGuid().ToString("N");
        var clip = AudioSniffer.Detect(bytes) switch
        {
            AudioFormat.Mp4Aac => new VoiceClip(id, bytes, "audio/mp4", ClampDuration(durationMs)),
            AudioFormat.WebmOpus => transcoder.WebmToWav(id, bytes),
            _ => null,
        };

        if (clip is null) return OperationResult<VoiceClip>.Fail(VoiceNotice.Invalid);
        if (clip.Duration.TotalMilliseconds < _options.MinMilliseconds) return OperationResult<VoiceClip>.Fail(VoiceNotice.TooShort);

        store.Save(clip);
        await chat.AddMediaAsync(userId, VoiceNote.ToText(clip.Id), MessageKind.Voice, UrlOf(clip.Id));
        return OperationResult<VoiceClip>.Ok(clip);
    }

    public VoicePlayback? Find(ChatMessage message)
    {
        if (VoiceNote.GetClipId(message.Text) is not { } id) return null;

        var url = UrlOf(id);
        return store.Get(id) is { } clip ? new VoicePlayback(url, clip.Duration, true) : new VoicePlayback(url, TimeSpan.Zero, false);
    }

    private string UrlOf(string clipId) => $"{_options.Path.Trim('/')}/{clipId}";

    // null si pasa Voice:MaxBytes (el stream de JS ya corta antes, esto es por las dudas)
    private async Task<byte[]?> ReadAsync(Stream audio)
    {
        using var buffer = new MemoryStream();
        await audio.CopyToAsync(buffer);
        return buffer.Length > _options.MaxBytes ? null : buffer.ToArray();
    }

    // La duración del MP4 la informa el navegador: se limita a lo permitido
    private TimeSpan ClampDuration(int durationMs) =>
        TimeSpan.FromMilliseconds(Math.Clamp(durationMs, 0, _options.MaxSeconds * 1000));
}
