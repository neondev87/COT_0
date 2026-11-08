
namespace COT_0.Web.Services.Voz;

// Reconoce el audio por sus magic bytes (no por el tipo que dice el navegador)
public static class AudioSniffer
{
    private static ReadOnlySpan<byte> WebmMagic => [0x1A, 0x45, 0xDF, 0xA3];

    public static AudioFormat Detect(ReadOnlySpan<byte> data)
    {
        if (data.StartsWith(WebmMagic))
            return data.IndexOf("A_OPUS"u8) >= 0 ? AudioFormat.WebmOpus : AudioFormat.Unknown;

        // MP4: caja "ftyp" en el byte 4; solo con AAC (mp4a), que es lo que suena en todos lados
        if (data.Length > 12 && data[4..8].SequenceEqual("ftyp"u8))
            return data.IndexOf("mp4a"u8) >= 0 ? AudioFormat.Mp4Aac : AudioFormat.Unknown;

        return AudioFormat.Unknown;
    }
}
