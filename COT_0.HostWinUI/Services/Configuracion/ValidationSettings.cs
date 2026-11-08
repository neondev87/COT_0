// Sección "Validation" de appsettings.json
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Largos máximos de lo que escribe el host.</summary>
public record ValidationSettings(int MaxUserLength, int MaxPasswordLength, int MaxAnswerLength);
