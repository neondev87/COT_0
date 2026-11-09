// Filtro al escribir: quita espacios y símbolos de lo que se teclea o se pega (TextBox y PasswordBox); la regla vive en InputRules
using COT_0.Host.Services.Validaciones;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>Propiedad adjunta: los estilos LoginTextBox y LoginPasswordBox la encienden, así aplica a todos los campos.</summary>
public static class FiltroCampo
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(FiltroCampo), new PropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);

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
        var text = box.Text;
        if (InputRules.IsAllowed(text)) return;

        var caret = InputRules.Clean(text[..Math.Min(box.SelectionStart, text.Length)]).Length;
        box.Text = InputRules.Clean(text);
        box.SelectionStart = caret;
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        var box = (PasswordBox)sender;
        if (!InputRules.IsAllowed(box.Password)) box.Password = InputRules.Clean(box.Password);
    }
}
