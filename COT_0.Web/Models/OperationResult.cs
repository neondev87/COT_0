namespace COT_0.Web.Models;

// Resultado simple: valor o mensaje de error para mostrar al usuario
public record OperationResult<T>(T? Value, string? Error)
{
    public static OperationResult<T> Ok(T value) => new(value, null);
    public static OperationResult<T> Fail(string error) => new(default, error);
}
