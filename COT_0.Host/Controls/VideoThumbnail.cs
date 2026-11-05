using System.Windows;
using System.Windows.Controls;

namespace COT_0.Host.Controls;

// Miniatura de video: queda en pausa mostrando el fotograma de PreviewPosition.
// Solo abre el video mientras se ve; al ocultarse lo cierra y suelta el decodificador
public class VideoThumbnail : MediaElement
{
    public static readonly DependencyProperty VideoProperty =
        DependencyProperty.Register(nameof(Video), typeof(Uri), typeof(VideoThumbnail),
            new PropertyMetadata(null, (d, _) => ((VideoThumbnail)d).UpdateSource()));

    public static readonly DependencyProperty PreviewPositionProperty =
        DependencyProperty.Register(nameof(PreviewPosition), typeof(TimeSpan), typeof(VideoThumbnail));

    public Uri? Video
    {
        get => (Uri?)GetValue(VideoProperty);
        set => SetValue(VideoProperty, value);
    }

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
        IsVisibleChanged += (_, _) => UpdateSource();
        MediaOpened += (_, _) => Position = ClampToDuration(PreviewPosition);
    }

    private void UpdateSource()
    {
        Source = IsVisible ? Video : null;
        if (Source is not null) Pause();
    }

    // Videos más cortos que la vista previa: se usa la mitad
    private TimeSpan ClampToDuration(TimeSpan position) =>
        NaturalDuration.HasTimeSpan && position >= NaturalDuration.TimeSpan
            ? NaturalDuration.TimeSpan / 2
            : position;
}
