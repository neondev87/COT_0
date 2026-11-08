// Notificación de Lain: solo recibe su ManualNoticeViewModel (la vista se enlaza a Opacity y Text)
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class AvisoLain : UserControl
{
    public static readonly DependencyProperty NoticeProperty = DependencyProperty.Register(
        nameof(Notice), typeof(ManualNoticeViewModel), typeof(AvisoLain), new PropertyMetadata(null));

    public AvisoLain() => InitializeComponent();

    public ManualNoticeViewModel? Notice
    {
        get => (ManualNoticeViewModel?)GetValue(NoticeProperty);
        set => SetValue(NoticeProperty, value);
    }
}
