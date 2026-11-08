// Sección "Ui" de appsettings.json
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Cultura y formatos del reloj del dashboard.</summary>
public record UiSettings(string Culture, string ClockFormat, string SecondsFormat, string DateFormat);
