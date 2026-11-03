using System.Windows.Media;

namespace COT_0.Host.ViewModels;

// Fondo elegible del dashboard y la sala
public abstract record BackgroundOption(string Label);

public record GradientBackground(string Label, Brush Brush) : BackgroundOption(Label);

public record VideoBackground(string Label, Uri Source) : BackgroundOption(Label);
