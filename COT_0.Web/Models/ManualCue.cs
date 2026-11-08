namespace COT_0.Web.Models;

/// <summary>Aviso de Lain listo para pintar: Style trae --in y --out (cuándo entra y sale)</summary>
public sealed record ManualCue(string Text, string Style, bool Leaves);
