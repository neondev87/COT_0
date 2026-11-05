using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace COT_0.Host.Controls;

// Funde el negro de un video con lo que hay detrás (en la GPU): el brillo se vuelve opacidad.
// Low/High: desde qué brillo empieza a verse y desde cuál es opaco. Strength: opacidad total (menos de 1 = fantasma)
public class LumaKeyEffect : ShaderEffect
{
    private static readonly PixelShader? Shader = ShaderCompiler.Compile("LumaKey.hlsl");

    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(nameof(Input), typeof(LumaKeyEffect), 0);

    public static readonly DependencyProperty LowProperty = DependencyProperty.Register(
        nameof(Low), typeof(double), typeof(LumaKeyEffect), new UIPropertyMetadata(0.05, PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty HighProperty = DependencyProperty.Register(
        nameof(High), typeof(double), typeof(LumaKeyEffect), new UIPropertyMetadata(0.5, PixelShaderConstantCallback(1)));

    public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
        nameof(Strength), typeof(double), typeof(LumaKeyEffect), new UIPropertyMetadata(1.0, PixelShaderConstantCallback(2)));

    public LumaKeyEffect()
    {
        PixelShader = Shader;
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(LowProperty);
        UpdateShaderValue(HighProperty);
        UpdateShaderValue(StrengthProperty);
    }

    public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }
    public double Low { get => (double)GetValue(LowProperty); set => SetValue(LowProperty, value); }
    public double High { get => (double)GetValue(HighProperty); set => SetValue(HighProperty, value); }
    public double Strength { get => (double)GetValue(StrengthProperty); set => SetValue(StrengthProperty, value); }
}
