using System.Windows;
using System.Windows.Controls;

namespace COT_0.Host.Controls;

// Permite enlazar PasswordBox.Password y saber si está vacío (para el placeholder).
// HasValue también lo usa el TextBox del login (vía estilo) para el placeholder.
// El default de BoundPassword es null para que el primer binding ("") enganche el evento.
public static class PasswordAssist
{
    public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
        "BoundPassword", typeof(string), typeof(PasswordAssist),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

    public static readonly DependencyProperty HasValueProperty = DependencyProperty.RegisterAttached(
        "HasValue", typeof(bool), typeof(PasswordAssist), new PropertyMetadata(false));

    private static readonly DependencyProperty IsHookedProperty = DependencyProperty.RegisterAttached(
        "IsHooked", typeof(bool), typeof(PasswordAssist), new PropertyMetadata(false));

    public static string? GetBoundPassword(DependencyObject d) => (string?)d.GetValue(BoundPasswordProperty);
    public static void SetBoundPassword(DependencyObject d, string? value) => d.SetValue(BoundPasswordProperty, value);

    public static bool GetHasValue(DependencyObject d) => (bool)d.GetValue(HasValueProperty);
    public static void SetHasValue(DependencyObject d, bool value) => d.SetValue(HasValueProperty, value);

    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox box) return;

        if (!(bool)box.GetValue(IsHookedProperty))
        {
            box.SetValue(IsHookedProperty, true);
            box.PasswordChanged += OnPasswordChanged;
        }

        var value = e.NewValue as string ?? string.Empty;
        if (box.Password != value) box.Password = value;
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        var box = (PasswordBox)sender;
        SetHasValue(box, box.Password.Length > 0);
        SetBoundPassword(box, box.Password);
    }
}
