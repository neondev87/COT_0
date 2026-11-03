using System.Windows;
using System.Windows.Controls;

namespace COT_0.Host.Controls;

// Miniatura de video: queda en pausa mostrando el fotograma de PreviewPosition
public class VideoThumbnail : MediaElement
{
    public static readonly DependencyProperty PreviewPositionProperty =
        DependencyProperty.Register(nameof(PreviewPosition), typeof(TimeSpan), typeof(VideoThumbnail));

    public TimeSpan PreviewPosition
    {
        get => (TimeSpan)GetValue(PreviewPositionProperty);
        set => SetValue(PreviewPositionProperty, value);
    }

    public VideoThumbnail()
    {
        LoadedBehavior = MediaState.Manual;
        UnloadedBehavior = MediaState.Close;
        ScrubbingEnabled = true;
        IsMuted = true;
        Loaded += (_, _) => Pause();
        MediaOpened += (_, _) => Position = ClampToDuration(PreviewPosition);
    }

    // Videos más cortos que la vista previa: se usa la mitad
    private TimeSpan ClampToDuration(TimeSpan position) =>
        NaturalDuration.HasTimeSpan && position >= NaturalDuration.TimeSpan
            ? NaturalDuration.TimeSpan / 2
            : position;
}
