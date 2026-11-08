// Qué parte del login se está mostrando
namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>El login es una sola pantalla que cambia según el paso.</summary>
public enum LoginStep
{
    /// <summary>ENTRAR con usuario y contraseña (siempre es el paso inicial).</summary>
    SignIn,

    /// <summary>"Registrar usuario": nombre y contraseña dos veces.</summary>
    Register,

    /// <summary>"¿Olvidaste tu contraseña?": usuario, su pregunta secreta y contraseña nueva.</summary>
    Recover,

    /// <summary>Usuario sin pregunta: se configura después de ENTRAR por primera vez.</summary>
    SetupQuestion,
}
