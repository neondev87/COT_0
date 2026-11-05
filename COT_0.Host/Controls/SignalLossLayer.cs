using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace COT_0.Host.Controls;

// Pérdida de señal de TV: ráfagas al azar con franjas corridas, temblor, destello gris y más estática.
// Solo corre con la ventana a la vista y con las animaciones del sistema activas
public class SignalLossLayer : Grid
{
    private const int BandCount = 7;
    private const double FramesPerSecond = 30;
    private const double FlashMilliseconds = 70;

    public static readonly DependencyProperty ChannelProperty = DependencyProperty.Register(
        nameof(Channel), typeof(object), typeof(SignalLossLayer), new PropertyMetadata(null, OnChannelChanged));

    public static readonly DependencyProperty BurstCommandProperty = DependencyProperty.Register(
        nameof(BurstCommand), typeof(ICommand), typeof(SignalLossLayer));

    public static readonly DependencyProperty ShiftProperty = DependencyProperty.Register(
        nameof(Shift), typeof(double), typeof(SignalLossLayer));

    public static readonly DependencyProperty TremorProperty = DependencyProperty.Register(
        nameof(Tremor), typeof(double), typeof(SignalLossLayer));

    private static readonly Brush[] Grays = Enumerable.Range(0, 6).Select(Gray).ToArray();

    private readonly DispatcherTimer _schedule = new();
    private readonly DispatcherTimer _frame = new(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(1 / FramesPerSecond) };
    private readonly NoiseLayer _noise = new() { Visibility = Visibility.Collapsed };
    private readonly Canvas _bands = new() { Visibility = Visibility.Collapsed };
    private readonly Rectangle _flash = new() { Opacity = 0 };
    private Window? _window;
    private long _burstStart;
    private long _burstEnd;

    public SignalLossLayer()
    {
        IsHitTestVisible = false;
        ClipToBounds = true;
        for (var i = 0; i < BandCount; i++) _bands.Children.Add(new Rectangle());
        Children.Add(_noise);
        Children.Add(_bands);
        Children.Add(_flash);
        _schedule.Tick += (_, _) => Burst(Between(MinBurst, MaxBurst));
        _frame.Tick += (_, _) => NextFrame();
        Loaded += (_, _) => Attach();
        Unloaded += (_, _) => Detach();
        IsVisibleChanged += (_, _) => UpdateRunning();
    }

    // Cambia al pasar de paso o a la despedida: dispara una ráfaga larga
    public object? Channel { get => GetValue(ChannelProperty); set => SetValue(ChannelProperty, value); }

    // Se ejecuta con la duración (TimeSpan) de cada ráfaga, para el sonido
    public ICommand? BurstCommand { get => (ICommand?)GetValue(BurstCommandProperty); set => SetValue(BurstCommandProperty, value); }

    // Desplazamiento horizontal y vertical del momento, para que otros elementos sigan a las franjas
    public double Shift { get => (double)GetValue(ShiftProperty); private set => SetValue(ShiftProperty, value); }

    public double Tremor { get => (double)GetValue(TremorProperty); private set => SetValue(TremorProperty, value); }

    public TimeSpan MinInterval { get; set; }
    public TimeSpan MaxInterval { get; set; }
    public TimeSpan MinBurst { get; set; }
    public TimeSpan MaxBurst { get; set; }
    public TimeSpan LongBurst { get; set; }
    public double BurstNoiseOpacity { get; set; }
    public double MaxShift { get; set; }
    public double MaxTremor { get; set; }
    public Brush? FlashBrush { get; set; }

    private bool CanRun => IsVisible && SystemParameters.ClientAreaAnimation && _window?.WindowState != WindowState.Minimized;

    private static void OnChannelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var layer = (SignalLossLayer)d;
        if (layer.IsLoaded && layer.CanRun) layer.Burst(layer.LongBurst);
    }

    private void Attach()
    {
        _noise.Opacity = BurstNoiseOpacity;
        _flash.Fill = FlashBrush;
        _window = Window.GetWindow(this);
        if (_window is not null) _window.StateChanged += OnWindowStateChanged;
        SystemParameters.StaticPropertyChanged += OnSystemParameterChanged;
        UpdateRunning();
    }

    private void Detach()
    {
        if (_window is not null) _window.StateChanged -= OnWindowStateChanged;
        SystemParameters.StaticPropertyChanged -= OnSystemParameterChanged;
        _window = null;
        Pause();
    }

    private void OnWindowStateChanged(object? sender, EventArgs e) => UpdateRunning();

    private void OnSystemParameterChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SystemParameters.ClientAreaAnimation)) UpdateRunning();
    }

    private void UpdateRunning()
    {
        if (!CanRun) Pause();
        else if (!_frame.IsEnabled && !_schedule.IsEnabled) ScheduleNext();
    }

    private void Pause()
    {
        EndBurst();
        _schedule.Stop();
    }

    private void ScheduleNext()
    {
        _schedule.Interval = Between(MinInterval, MaxInterval);
        _schedule.Start();
    }

    private void Burst(TimeSpan duration)
    {
        _schedule.Stop();
        _burstStart = Environment.TickCount64;
        _burstEnd = Math.Max(_burstEnd, _burstStart + (long)duration.TotalMilliseconds);
        _noise.Visibility = _bands.Visibility = Visibility.Visible;
        _frame.Start();
        NextFrame();
        if (BurstCommand?.CanExecute(duration) == true) BurstCommand.Execute(duration);
    }

    private void NextFrame()
    {
        var now = Environment.TickCount64;
        if (now >= _burstEnd)
        {
            EndBurst();
            ScheduleNext();
            return;
        }
        _flash.Opacity = now - _burstStart < FlashMilliseconds ? 1 : 0;
        Shift = Signed(MaxShift);
        Tremor = Signed(MaxTremor);
        foreach (Rectangle band in _bands.Children) Scramble(band);
    }

    // Cada franja: altura, posición, ancho, corrimiento y gris al azar; algunas se apagan en el cuadro
    private void Scramble(Rectangle band)
    {
        var random = Random.Shared;
        band.Visibility = random.NextDouble() < 0.75 ? Visibility.Visible : Visibility.Hidden;
        band.Height = 2 + random.NextDouble() * random.NextDouble() * 46;
        band.Width = ActualWidth * (0.4 + random.NextDouble() * 0.8);
        band.Fill = Grays[random.Next(Grays.Length)];
        Canvas.SetTop(band, random.NextDouble() * ActualHeight);
        Canvas.SetLeft(band, random.NextDouble() * (ActualWidth - band.Width * 0.6) + Signed(MaxShift * 6));
    }

    private void EndBurst()
    {
        _frame.Stop();
        _burstEnd = 0;
        _noise.Visibility = _bands.Visibility = Visibility.Collapsed;
        _flash.Opacity = 0;
        Shift = Tremor = 0;
    }

    private static TimeSpan Between(TimeSpan min, TimeSpan max) => min + (max - min) * Random.Shared.NextDouble();

    private static double Signed(double max) => (Random.Shared.NextDouble() * 2 - 1) * max;

    // Grises cada vez más claros y opacos, congelados para reusarlos en cada cuadro
    private static Brush Gray(int step)
    {
        var level = (byte)(60 + step * 36);
        var brush = new SolidColorBrush(Color.FromArgb((byte)(24 + step * 14), level, level, level));
        brush.Freeze();
        return brush;
    }
}
