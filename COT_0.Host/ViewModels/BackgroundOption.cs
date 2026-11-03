namespace COT_0.Host.ViewModels;

// Fondo elegible del dashboard y la sala: un archivo de la carpeta de fondos
public abstract record BackgroundOption(Uri Source);

public record ImageBackground(Uri Source) : BackgroundOption(Source);

// Preview: momento del video que se muestra como miniatura
public record VideoBackground(Uri Source, TimeSpan Preview) : BackgroundOption(Source);
