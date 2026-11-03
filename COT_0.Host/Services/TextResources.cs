using System.Windows;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Pasa la sección "Texts" de appsettings.json a recursos: Texts:Login:User → "Text.Login.User"
public static class TextResources
{
    public const string Prefix = "Text.";

    public static void Load(IConfiguration configuration, ResourceDictionary resources)
    {
        foreach (var (key, value) in configuration.GetSection("Texts").AsEnumerable(makePathsRelative: true))
        {
            if (value is null) continue;
            resources[Prefix + key.Replace(':', '.')] = value;
        }
    }
}
