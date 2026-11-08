// Bocina de la firma: botón con cursor de mano (ProtectedCursor solo se pone desde la clase); estilo en Views/Estilos/Volumen.xaml
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>Es un Button normal: el glifo va en Content y silenciar en Command.</summary>
public sealed partial class BotonBocina : Button
{
    public BotonBocina() => ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
}
