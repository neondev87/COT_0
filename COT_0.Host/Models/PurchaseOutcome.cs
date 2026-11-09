// Resultado de presionar COMPRAR
namespace COT_0.Host.Models;

/// <summary>Cada valor distinto de Bought tiene su texto "Text.Store.Outcome.{valor}" en Textos.xaml.</summary>
public enum PurchaseOutcome
{
    Bought,
    LimitReached,
    AlreadyOwned,
    NoUser,
}
