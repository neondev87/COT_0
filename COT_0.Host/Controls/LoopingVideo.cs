using System.Windows.Controls;

namespace COT_0.Host.Controls;

// Video de fondo en loop y sin sonido
public class LoopingVideo : MediaElement
{
    public LoopingVideo()
    {
        LoadedBehavior = MediaState.Manual;
        UnloadedBehavior = MediaState.Close;
        IsMuted = true;
        Loaded += (_, _) => Play();
        MediaEnded += (_, _) =>
        {
            Position = TimeSpan.Zero;
            Play();
        };
    }
}
