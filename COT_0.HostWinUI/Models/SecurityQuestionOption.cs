// Una opción de la lista de preguntas secretas
namespace COT_0.HostWinUI.Models;

/// <summary>Key se guarda en usuarios.json; Text es lo que ves en la lista.</summary>
public record SecurityQuestionOption(string Key, string Text)
{
    public override string ToString() => Text;
}
