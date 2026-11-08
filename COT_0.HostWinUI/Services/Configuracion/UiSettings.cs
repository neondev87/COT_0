// Sección "Ui" de appsettings.json
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Cultura, formatos del reloj del dashboard, tiempo de la sala y cuánto dura el "COPIADO".</summary>
public record UiSettings(
    string Culture,
    string ClockFormat,
    string SecondsFormat,
    string DateFormat,
    string ElapsedFormat,
    int CopyFeedbackMilliseconds);
