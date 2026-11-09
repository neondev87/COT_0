// Reglas de contraseña que no se cumplen (pueden ser varias a la vez)
namespace COT_0.Host.Services.Validaciones;

/// <summary>Cada bandera tiene su texto "Text.Password.{bandera}" en Textos.xaml.</summary>
[Flags]
public enum PasswordProblems
{
    None = 0,
    TooShort = 1,
    TooLong = 2,
    NoDigit = 4,
    NoSymbol = 8,
    HasSpaces = 16,
    InvalidChars = 32,
}
