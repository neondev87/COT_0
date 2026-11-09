// Filtro al escribir: quita de lo que se teclea o se pega lo que no cumple la regla del campo; las reglas viven en InputRules
using COT_0.Host.Services.Validaciones;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>
/// Propiedades adjuntas: los estilos LoginTextBox, UserNameTextBox y LoginPasswordBox encienden el filtro.
/// IsUserName cambia la regla del TextBox a la de usuario; un PasswordBox siempre usa la de contraseña.
/// </summary>
public static class FiltroCampo
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(FiltroCampo), new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty IsUserNameProperty = DependencyProperty.RegisterAttached(
        "IsUserName", typeof(bool), typeof(FiltroCampo), new PropertyMetadata(false));

    public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);

    public static bool GetIsUserName(DependencyObject element) => (bool)element.GetValue(IsUserNameProperty);

    public static void SetIsUserName(DependencyObject element, bool value) => element.SetValue(IsUserNameProperty, value);

    private static void OnIsEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
    {
        var enabled = (bool)e.NewValue;
        if (element is TextBox textBox)
        {
            textBox.TextChanging -= OnTextChanging;
            if (enabled) textBox.TextChanging += OnTextChanging;
        }
        else if (element is PasswordBox passwordBox)
        {
            passwordBox.PasswordChanged -= OnPasswordChanged;
            if (enabled) passwordBox.PasswordChanged += OnPasswordChanged;
        }
    }

    // Conserva el cursor donde estaba, menos los caracteres quitados
    private static void OnTextChanging(TextBox box, TextBoxTextChangingEventArgs args)
    {
        var kind = GetIsUserName(box) ? FieldKind.UserName : FieldKind.Text;
        var text = box.Text;
        if (InputRules.IsClean(text, kind)) return;

        var caret = InputRules.Clean(text[..Math.Min(box.SelectionStart, text.Length)], kind).Length;
        box.Text = InputRules.Clean(text, kind);
        box.SelectionStart = Math.Min(caret, box.Text.Length);
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        var box = (PasswordBox)sender;
        if (!InputRules.IsClean(box.Password, FieldKind.Password)) box.Password = InputRules.Clean(box.Password, FieldKind.Password);
    }
}
