namespace COT_0.Web.Options;

// Configuración del chat (sección "Chat" de appsettings.json); los valores son defaults
public class ChatOptions
{
    public const string Section = "Chat";

    public string HubPath { get; set; } = "/chathub";
    public string AvatarsPath { get; set; } = "/avatars";
    public string EmotesFolder { get; set; } = "emotes";
    public string[] EmoteExtensions { get; set; } = [".gif", ".png", ".webp", ".jpg"];
    public string FlagsFolder { get; set; } = "img/flags";
    public string FlagExtension { get; set; } = ".png";

    public int MaxNameLength { get; set; } = 24;
    public int MaxMessageLength { get; set; } = 500;
    public int HistorySize { get; set; } = 200;
    public int MessagesOnJoin { get; set; } = 100;
    public string TimeFormat { get; set; } = "HH:mm";

    public int PhotoSize { get; set; } = 256;
    public string PhotoFormat { get; set; } = "image/jpeg";
    public long MaxPhotoBytes { get; set; } = 1024 * 1024;

    // URL local de Kestrel; la completa WebApp (el chat se conecta al hub por acá)
    public string ServerUrl { get; set; } = string.Empty;

    public string HubUrl => ServerUrl.TrimEnd('/') + HubPath;
}
