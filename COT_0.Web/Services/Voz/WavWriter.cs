using System.Runtime.InteropServices;

namespace COT_0.Web.Services.Voz;

// Arma un WAV PCM de 16 bits mono (suena en cualquier navegador, iPhone incluido)
public static class WavWriter
{
    private const short Channels = 1;
    private const short BitsPerSample = 16;

    public static byte[] Write(ReadOnlySpan<short> samples, int sampleRate)
    {
        var pcm = MemoryMarshal.AsBytes(samples);
        var blockAlign = (short)(Channels * BitsPerSample / 8);
        using var output = new MemoryStream(44 + pcm.Length);
        using var writer = new BinaryWriter(output);

        writer.Write("RIFF"u8);
        writer.Write(36 + pcm.Length);
        writer.Write("WAVEfmt "u8);
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(Channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * blockAlign);
        writer.Write(blockAlign);
        writer.Write(BitsPerSample);
        writer.Write("data"u8);
        writer.Write(pcm.Length);
        writer.Write(pcm);
        writer.Flush();
        return output.ToArray();
    }
}
