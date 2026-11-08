using System.Runtime.InteropServices;
using Concentus;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Voz;

// Decodifica con Concentus (Opus en C#) directo a mono, a la frecuencia de Voice:WavSampleRate
public sealed class OpusWavTranscoder(IOptions<VoiceOptions> options, ILogger<OpusWavTranscoder> logger) : IAudioTranscoder
{
    // Un paquete Opus dura como mucho 120 ms
    private const int MaxFrameMs = 120;

    public VoiceClip? WebmToWav(string id, byte[] webm)
    {
        var rate = options.Value.WavSampleRate;
        var decoder = OpusCodecFactory.CreateDecoder(rate, 1);
        var frameSize = rate * MaxFrameMs / 1000;
        var buffer = new short[frameSize];
        var samples = new List<short>(rate * options.Value.MaxSeconds);

        foreach (var frame in WebmOpusFrames.Read(webm))
        {
            try
            {
                var count = decoder.Decode(frame.Span, buffer, frameSize);
                samples.AddRange(buffer.AsSpan(0, count));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IndexOutOfRangeException)
            {
                logger.LogDebug(ex, "Paquete Opus inválido en la nota {Id}", id);
            }
        }

        // Por si el navegador mintió la duración: se corta en Voice:MaxSeconds
        var max = rate * options.Value.MaxSeconds;
        if (samples.Count > max) samples.RemoveRange(max, samples.Count - max);
        if (samples.Count == 0) return null;

        var wav = WavWriter.Write(CollectionsMarshal.AsSpan(samples), rate);
        return new VoiceClip(id, wav, "audio/wav", TimeSpan.FromSeconds((double)samples.Count / rate));
    }
}
