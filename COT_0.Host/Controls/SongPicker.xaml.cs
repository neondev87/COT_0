using System.Windows.Controls;
using System.Windows.Threading;

namespace COT_0.Host.Controls;

public partial class SongPicker : UserControl
{
    public SongPicker()
    {
        InitializeComponent();
        // Al abrir, el foco va al buscador; la lista sigue a la selección del teclado
        IsEnabledChanged += (_, e) =>
        {
            if ((bool)e.NewValue) Dispatcher.BeginInvoke(() => SearchBox.Focus(), DispatcherPriority.Input);
        };
        List.SelectionChanged += (_, _) =>
        {
            if (List.SelectedItem is not null) List.ScrollIntoView(List.SelectedItem);
        };
    }
}
