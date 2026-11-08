// Sección "Recovery" de appsettings.json
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Claves de las preguntas secretas que se ofrecen (su texto está en Textos.xaml).</summary>
public record RecoverySettings(IReadOnlyList<string> QuestionKeys);
