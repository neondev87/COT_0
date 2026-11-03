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

    // Nombre visible de cada escudo (clave = nombre del archivo sin extensión)
    public Dictionary<string, string> FlagNames { get; set; } = new()
    {
        ["arquitectura"] = "Arquitectura",
        ["artes-escenicas"] = "Artes Escénicas",
        ["artes-visuales"] = "Artes Visuales",
        ["ciencias-biologicas"] = "Ciencias Biológicas",
        ["ciencias-de-la-comunicacion"] = "Ciencias de la Comunicación",
        ["ciencias-fisico-matematicas"] = "Ciencias Físico Matemáticas",
        ["ciencias-politicas-y-rri"] = "Ciencias Políticas y RI",
        ["ciencias-quimicas"] = "Ciencias Químicas",
        ["derecho-y-criminologia"] = "Derecho y Criminología",
        ["economia"] = "Economía",
        ["facpya"] = "FACPYA",
        ["filosofia-y-letras"] = "Filosofía y Letras",
        ["fime"] = "FIME",
        ["ingenieria-civil"] = "Ingeniería Civil",
        ["musica"] = "Música",
        ["organizacion-deportiva"] = "Organización Deportiva",
        ["trabajo-social"] = "Trabajo Social",
    };

    // Partículas de ceniza del fondo (semilla fija: siempre el mismo dibujo)
    public int AshSeed { get; set; } = 31;
    public int AshCount { get; set; } = 20;

    public int RoomCodeLength { get; set; } = 4;
    public string RoomCodeChars { get; set; } = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

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
