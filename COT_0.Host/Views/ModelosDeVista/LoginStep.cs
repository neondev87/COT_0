// Qué parte del login se está mostrando
namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>El login es una sola pantalla que cambia según el paso.</summary>
public enum LoginStep
{
    /// <summary>ENTRAR: ya hay host registrado.</summary>
    SignIn,

    /// <summary>REGISTRARME: no hay host; pide también la pregunta secreta.</summary>
    Register,

    /// <summary>"¿Olvidaste tu contraseña?": pregunta secreta y contraseña nueva.</summary>
    Recover,

    /// <summary>Cuenta anterior sin pregunta: se configura después de ENTRAR.</summary>
    SetupQuestion,
}
