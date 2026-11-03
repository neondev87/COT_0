using System.Globalization;

namespace COT_0.Web.Models;

// Partícula de ceniza del fondo; Style lleva las variables CSS que usa .cot-ash
public record AshParticle(string Style)
{
    // Mismo generador pseudoaleatorio que el diseño: con la misma semilla sale el mismo dibujo
    public static IReadOnlyList<AshParticle> Generate(int seed, int count)
    {
        var state = seed;
        double Next()
        {
            state = (state * 9301 + 49297) % 233280;
            return state / 233280.0;
        }

        var inv = CultureInfo.InvariantCulture;
        var list = new List<AshParticle>(count);
        for (var i = 0; i < count; i++)
        {
            var size = (1 + Next() * 2.2).ToString("0.0", inv);
            var left = (Next() * 100).ToString("0.0", inv);
            var duration = (18 + Next() * 24).ToString("0.0", inv);
            var delay = (Next() * 30).ToString("0.0", inv);
            var dx = (int)Math.Round(-50 + Next() * 110);
            var opacity = (0.12 + Next() * 0.25).ToString("0.00", inv);
            list.Add(new AshParticle(
                $"--x:{left}%;--s:{size}px;--dur:{duration}s;--delay:-{delay}s;--dx:{dx}px;--o:{opacity}"));
        }

        return list;
    }
}
