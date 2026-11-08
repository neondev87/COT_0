using System.Numerics;

namespace COT_0.Web.Services.Voz;

/// <summary>Lee los paquetes Opus de un WebM de MediaRecorder (EBML: Segment → Cluster → SimpleBlock)</summary>
public static class WebmOpusFrames
{
    private const uint Segment = 0x18538067;
    private const uint Cluster = 0x1F43B675;
    private const uint BlockGroup = 0xA0;
    private const uint Block = 0xA1;
    private const uint SimpleBlock = 0xA3;

    public static List<ReadOnlyMemory<byte>> Read(byte[] data)
    {
        var frames = new List<ReadOnlyMemory<byte>>();
        var pos = 0;

        while (TryReadId(data, ref pos, out var id) && TryReadSize(data, ref pos, out var size))
        {
            // Contenedores: se entra a sus hijos (MediaRecorder los deja con tamaño desconocido)
            if (id is Segment or Cluster or BlockGroup) continue;
            if (size is null) break;

            var end = (int)Math.Min(data.Length, pos + size.Value);
            if (id is SimpleBlock or Block) AddFrame(data, pos, end, frames);
            pos = end;
        }

        return frames;
    }

    // Bloque: pista (vint) + timecode (2 bytes) + flags (1 byte) + paquete
    private static void AddFrame(byte[] data, int pos, int end, List<ReadOnlyMemory<byte>> frames)
    {
        if (!TryReadSize(data, ref pos, out _) || pos + 3 > end) return;

        // Con lacing (varios paquetes juntos): MediaRecorder no lo usa
        if ((data[pos + 2] & 0x06) != 0) return;

        pos += 3;
        if (end > pos) frames.Add(data.AsMemory(pos, end - pos));
    }

    // El ID conserva el bit marcador; largo de 1 a 4 bytes según los ceros iniciales
    private static bool TryReadId(byte[] data, ref int pos, out uint id)
    {
        id = 0;
        var length = VintLength(data, pos);
        if (length is 0 or > 4) return false;

        for (var i = 0; i < length; i++) id = (id << 8) | data[pos + i];
        pos += length;
        return true;
    }

    // Tamaño vint de 1 a 8 bytes; todos los bits de valor en 1 = desconocido (null)
    private static bool TryReadSize(byte[] data, ref int pos, out long? size)
    {
        size = null;
        var length = VintLength(data, pos);
        if (length == 0) return false;

        long value = data[pos] & (0xFF >> length);
        var unknown = value == 0xFF >> length;
        for (var i = 1; i < length; i++)
        {
            value = (value << 8) | data[pos + i];
            unknown &= data[pos + i] == 0xFF;
        }

        pos += length;
        if (!unknown) size = value;
        return true;
    }

    // 0 si no hay un vint válido completo en pos
    private static int VintLength(byte[] data, int pos)
    {
        if (pos >= data.Length || data[pos] == 0) return 0;

        var length = BitOperations.LeadingZeroCount((uint)data[pos]) - 23;
        return pos + length <= data.Length ? length : 0;
    }
}
