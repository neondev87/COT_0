// Límite de caracteres de los campos de texto (Inputs:MaxCaracteres): lo usan el XAML (MaxLength) y los validadores
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Validaciones;

/// <summary>Un solo lugar para el número: el XAML lo toma con x:Bind y el código para validar y armar el mensaje.</summary>
public static class InputLimits
{
    public static int MaxCharacters => AppConfig.Settings.Inputs.MaxCaracteres;

    public static int MaxEmoteSetCharacters => AppConfig.Settings.Inputs.MaxEmoteSetCaracteres;

    public static bool IsTooLong(string? text) => text?.Length > MaxCharacters;
}
