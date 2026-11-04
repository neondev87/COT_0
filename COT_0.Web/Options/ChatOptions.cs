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

    // Las 26 facultades de la UANL: clave (= archivo del escudo, sin extensión) → nombre visible
    public Dictionary<string, string> FlagNames { get; set; } = new()
    {
        ["agronomia"] = "Agronomía",
        ["arquitectura"] = "Arquitectura",
        ["artes-escenicas"] = "Artes Escénicas",
        ["artes-visuales"] = "Artes Visuales",
        ["ciencias-biologicas"] = "Ciencias Biológicas",
        ["ciencias-de-la-comunicacion"] = "Ciencias de la Comunicación",
        ["ciencias-de-la-tierra"] = "Ciencias de la Tierra",
        ["ciencias-fisico-matematicas"] = "Ciencias Físico Matemáticas",
        ["ciencias-forestales"] = "Ciencias Forestales",
        ["ciencias-politicas-y-rri"] = "Ciencias Políticas y RI",
        ["ciencias-quimicas"] = "Ciencias Químicas",
        ["derecho-y-criminologia"] = "Derecho y Criminología",
        ["economia"] = "Economía",
        ["enfermeria"] = "Enfermería",
        ["facpya"] = "FACPYA",
        ["filosofia-y-letras"] = "Filosofía y Letras",
        ["fime"] = "FIME",
        ["ingenieria-civil"] = "Ingeniería Civil",
        ["medicina"] = "Medicina",
        ["medicina-veterinaria-y-zootecnia"] = "Medicina Veterinaria y Zootecnia",
        ["musica"] = "Música",
        ["odontologia"] = "Odontología",
        ["organizacion-deportiva"] = "Organización Deportiva",
        ["psicologia"] = "Psicología",
        ["salud-publica-y-nutricion"] = "Salud Pública y Nutrición",
        ["trabajo-social"] = "Trabajo Social y Desarrollo Humano",
    };

    // Facultades que van primero en el selector, en este orden; el resto va alfabético.
    // Default en código como FlagNames: un array en appsettings se sumaría a este en vez de reemplazarlo
    public string[] FlagPriority { get; set; } = ["facpya", "fime", "ciencias-politicas-y-rri"];

    // Clave de localStorage con el perfil del invitado (para volver a entrar sin el formulario)
    public string GuestStorageKey { get; set; } = "cot0.guest";

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

    // Mensajes seguidos del mismo autor dentro de esta ventana van en un solo grupo
    public int GroupWindowMinutes { get; set; } = 5;

    // "Escribiendo...": cada cuánto se avisa como máximo y cuánto dura el aviso
    public int TypingThrottleSeconds { get; set; } = 2;
    public int TypingTimeoutSeconds { get; set; } = 5;

    // Segundos de espera antes de sacar a alguien que perdió la conexión (puede volver)
    public int LeaveGraceSeconds { get; set; } = 20;

    // Reconexión del circuito de Blazor (celular con la pantalla bloqueada)
    public int CircuitRetentionMinutes { get; set; } = 5;
    public int ReconnectRetries { get; set; } = 60;
    public int ReconnectIntervalMs { get; set; } = 2000;

    // Avisos del sistema en el chat ({0} = nombre)
    public string JoinedNotice { get; set; } = "{0} entró a la sala";
    public string LeftNotice { get; set; } = "{0} salió de la sala";
    public string KickedNotice { get; set; } = "{0} fue expulsado";

    public int PhotoSize { get; set; } = 256;
    public string PhotoFormat { get; set; } = "image/jpeg";
    public long MaxPhotoBytes { get; set; } = 1024 * 1024;

    // Imágenes del chat: el navegador las achica a ImageMaxSide y las pasa a ImageFormat antes de subir
    public string ImagesPath { get; set; } = "/images";
    public string ImageFormat { get; set; } = "image/jpeg";
    public int ImageMaxSide { get; set; } = 1600;
    public int ImagePreviewSide { get; set; } = 720;
    public long MaxImageBytes { get; set; } = 5 * 1024 * 1024;
    public int ImageDecodeTimeoutSeconds { get; set; } = 30;

    // Tope de memoria para todas las imágenes de la sala (se vacía al cerrarla)
    public long MaxRoomImageBytes { get; set; } = 200L * 1024 * 1024;

    // Miniatura en la burbuja (px) y cuánto cachea el navegador cada imagen
    public int ImageThumbWidth { get; set; } = 240;
    public int ImageThumbMaxHeight { get; set; } = 300;
    public int ImageCacheSeconds { get; set; } = 86400;

    // URL local de Kestrel; la completa WebApp (el chat se conecta al hub por acá)
    public string ServerUrl { get; set; } = string.Empty;

    public string HubUrl => ServerUrl.TrimEnd('/') + HubPath;
}
