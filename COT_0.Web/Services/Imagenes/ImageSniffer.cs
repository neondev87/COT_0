using System.Buffers.Binary;

namespace COT_0.Web.Services.Imagenes;

/// <summary>Reconoce la imagen por sus magic bytes (no por la extensión ni el content-type) y lee su tamaño</summary>
public static class ImageSniffer
{
    private static ReadOnlySpan<byte> PngMagic => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    public static ImageInfo Detect(ReadOnlySpan<byte> data)
    {
        if (data.Length < 16) return ImageInfo.Unknown;
        if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF) return Jpeg(data);
        if (data.StartsWith(PngMagic) && data.Length >= 24)
            return new(ImageFormat.Png, BinaryPrimitives.ReadInt32BigEndian(data[16..]), BinaryPrimitives.ReadInt32BigEndian(data[20..]));
        if (data.StartsWith("GIF87a"u8) || data.StartsWith("GIF89a"u8))
            return new(ImageFormat.Gif, BinaryPrimitives.ReadUInt16LittleEndian(data[6..]), BinaryPrimitives.ReadUInt16LittleEndian(data[8..]));
        if (data.StartsWith("RIFF"u8) && data[8..12].SequenceEqual("WEBP"u8)) return WebP(data);
        return ImageInfo.Unknown;
    }

    // Recorre los segmentos hasta el SOF (Start Of Frame), que tiene alto y ancho
    private static ImageInfo Jpeg(ReadOnlySpan<byte> data)
    {
        var i = 2;
        while (i + 9 < data.Length)
        {
            if (data[i] != 0xFF) return ImageInfo.Unknown;
            var marker = data[i + 1];
            if (marker == 0xFF) { i++; continue; }

            var length = BinaryPrimitives.ReadUInt16BigEndian(data[(i + 2)..]);
            var isFrame = marker is >= 0xC0 and <= 0xCF and not 0xC4 and not 0xC8 and not 0xCC;
            if (isFrame)
                return new(ImageFormat.Jpeg, BinaryPrimitives.ReadUInt16BigEndian(data[(i + 7)..]), BinaryPrimitives.ReadUInt16BigEndian(data[(i + 5)..]));
            i += 2 + length;
        }
        return ImageInfo.Unknown;
    }

    private static ImageInfo WebP(ReadOnlySpan<byte> data)
    {
        if (data.Length < 30) return ImageInfo.Unknown;
        var chunk = data[12..16];

        if (chunk.SequenceEqual("VP8 "u8))
            return new(ImageFormat.WebP, BinaryPrimitives.ReadUInt16LittleEndian(data[26..]) & 0x3FFF, BinaryPrimitives.ReadUInt16LittleEndian(data[28..]) & 0x3FFF);
        if (chunk.SequenceEqual("VP8L"u8))
        {
            var bits = BinaryPrimitives.ReadUInt32LittleEndian(data[21..]);
            return new(ImageFormat.WebP, (int)(bits & 0x3FFF) + 1, (int)((bits >> 14) & 0x3FFF) + 1);
        }
        if (chunk.SequenceEqual("VP8X"u8))
            return new(ImageFormat.WebP, ReadUInt24(data[24..]) + 1, ReadUInt24(data[27..]) + 1);
        return ImageInfo.Unknown;
    }

    private static int ReadUInt24(ReadOnlySpan<byte> data) => data[0] | (data[1] << 8) | (data[2] << 16);
}
