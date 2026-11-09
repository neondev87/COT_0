# CONTRATO – COT_0

Acuerdo entre agentes. Si algo de acá tiene que cambiar, se anota en **Pedidos de cambio** (al final) y lo resuelve el Agente 0.

## Solución

```
COT_0.sln
COT_0.Core/   class library   → solo Agente 0
COT_0.Web/    Blazor Server   → Agente B (menos wwwroot) / Agente C (wwwroot)
COT_0.Host/   WinUI 3         → Agente A
```

Referencias: `Host → Web, Core` · `Web → Core`. Core no referencia a nadie.

## Dueños de carpetas

| Agente | Carpeta | Qué hace |
|---|---|---|
| 0 | `COT_0.Core/`, `CONTRATO.md`, `COT_0.sln` | Modelos, interfaces, este contrato |
| A | `COT_0.Host/` | Login del host, ventana principal, botón "Generar QR", Kestrel en segundo plano, cloudflared, QR |
| B | `COT_0.Web/` **excepto** `wwwroot/` | Páginas Razor, `ChatHub`, implementación de `IChatService`, `WebApp.cs`, `Program.cs` |
| C | `COT_0.Web/wwwroot/` | CSS (`css/`), imágenes y banderas (`img/`), emotes (`emotes/`). Estilo gótico/anime/dark |

Nadie toca carpetas ajenas. Ver `../CLAUDE.md` para las reglas generales (C#, sin Node/npm, sin base de datos).

## Red y rutas

- Servidor: **http://localhost:5050** (`Web:Url` en `appsettings.json`; en standalone sale del perfil de `launchSettings.json`). Solo HTTP; el HTTPS público lo da cloudflared.
- `COT_0.Host` arranca la web dentro de su propio proceso (`Services/Sala/WebServerHost.cs` → `WebApp.Create(...)`) al iniciar la app. Usa `FrameworkReference Microsoft.AspNetCore.App`.
- `COT_0.Web` también corre solo con `dotnet run` (mismo puerto) para probar sin el Host.

| Ruta | Qué es |
|---|---|
| `/` | Redirige a `/join` |
| `/join` | `Views/Pantallas/Join.razor`: el invitado carga nombre, foto y bandera de facultad |
| `/chat` | `Views/Pantallas/Chat.razor`: chat grupal en tiempo real |
| `/chathub` | `Services/Sala/ChatHub.cs`: hub de SignalR |

El QR apunta a `{PublicUrl}/join`.

## Modelos (`COT_0.Core.Models`)

### `User`
Invitado conectado, solo en memoria.

| Propiedad | Tipo | Notas |
|---|---|---|
| `Id` | `string` | GUID sin guiones, se genera solo |
| `Name` | `string` | Nombre visible |
| `PhotoUrl` | `string?` | URL o data URI (base64) de la foto |
| `FacultyFlag` | `string?` | Clave de la bandera, ej. `"ingenieria"` → `wwwroot/img/flags/ingenieria.png` |
| `ConnectionId` | `string?` | Conexión SignalR actual |
| `JoinedAt` | `DateTime` | UTC |

### `ChatMessage`
| Propiedad | Tipo | Notas |
|---|---|---|
| `Id` | `string` | GUID sin guiones |
| `UserId` | `string` | Autor |
| `UserName`, `UserPhotoUrl`, `UserFacultyFlag` | `string` / `string?` | Copia del autor al enviar (el mensaje queda bien aunque el usuario se vaya) |
| `Kind` | `MessageKind` | `Text` / `Joined` / `Left` / `Kicked` / `Voice` / `Image` |
| `Text` | `string` | Contenido (en voz e imagen sigue siendo el texto reservado de Web, ver Pedidos) |
| `AudioUrl` | `string?` | Solo `Kind = Voice`: ruta relativa del audio (`{Voice:Path}/{id}`) |
| `ImageUrl` | `string?` | Solo `Kind = Image`: ruta relativa de la imagen (`{Chat:ImagesPath}/{id}`) |
| `SentAt` | `DateTime` | UTC |

## Interfaces (`COT_0.Core.Services`)

### `IChatService` – implementa Agente B (en Web, registrar como **Singleton**)
Estado del chat en memoria.

- `Task<User> AddUserAsync(User user)` – agrega un invitado y lo devuelve.
- `Task RemoveUserAsync(string userId)` – lo saca (desconexión).
- `Task<User?> GetUserAsync(string userId)`
- `Task<User?> UpdateUserAsync(string userId, string name, string? photoUrl, string? facultyFlag)` – edita el perfil y la copia del autor en sus mensajes; dispara `UserUpdated` y `UsersChanged`.
- `Task<IReadOnlyList<User>> GetUsersAsync()` – conectados.
- `Task<ChatMessage> AddMessageAsync(string userId, string text)` – crea el mensaje con los datos del autor.
- `Task<ChatMessage> AddMediaAsync(string userId, string text, MessageKind kind, string url)` – nota de voz (`AudioUrl`) o imagen (`ImageUrl`).
- `Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int max = 100)` – últimos mensajes, del más viejo al más nuevo.
- `Task KickUserAsync(string userId)` – lo saca y dispara `UserKicked`.
- `Task ClearAsync()` – borra usuarios e historial (al cerrar la sala).
- `event Action? UsersChanged` – entra/sale/expulsado/vaciado. El Host lo escucha con `Dispatcher.Invoke`.
- `event Action<User>? UserKicked` – Web le avisa al expulsado por el hub (evento `"Kicked"`).
- `event Action<User>? UserUpdated` – perfil editado; Web lo reenvía por el hub (evento `"UserUpdated"`).

### `IRoomService` – implementa Web (`Services/Sala/RoomService.cs`, **Singleton**)
Estado de la sala. El Host la abre/cierra; Web muestra el código y bloquea `/join` si está cerrada.
- `string? Code` – código corto ("7F3K"), `null` si cerrada. Largo y caracteres en `Chat:RoomCodeLength` / `Chat:RoomCodeChars`.
- `DateTime? OpenedAt` (UTC) · `bool IsOpen`
- `void Open()` – genera código nuevo y marca la hora.
- `Task CloseAsync()` – cierra y llama a `IChatService.ClearAsync()`.
- `event Action? Changed`

El Host obtiene `IChatService` e `IRoomService` del contenedor de la app web que levanta `WebServerHost` (son la misma instancia que usa el hub).

### `IFlagService` – implementa Web
Con su modelo `FlagOption` (`COT_0.Core.Models`).

### `IRoomEmotes` – implementa Web (`Services/Emotes/RoomEmotes.cs`, **Singleton**)
Emotes de la sala desde un set de 7TV. Modelos en `COT_0.Core.Models`: `RoomEmote`, `EmoteSources`, `EmoteReport`, `EmoteSetStatus`, `EmoteSetError`. El Host la saca del contenedor de la web (misma instancia que usan los chats). Ver el pedido "emotes 7TV" para el detalle de los miembros.

### `ITunnelService` – implementa Agente A (`Services/Tunel/CloudflareTunnelService.cs`)
- `string? PublicUrl` – `https://xxxx.trycloudflare.com` o `null`.
- `bool IsRunning`
- `Task<string> StartAsync(string localUrl, CancellationToken ct = default)` – lanza `cloudflared tunnel --url {localUrl}` y devuelve la URL pública.
- `Task StopAsync()` – cierra cloudflared.

### `IBackgroundCatalog` – implementa Host (`Services/Fondos/FondoCatalog.cs`, **Singleton**)
Carpeta de fondos sin tipos de vista. Modelos `Fondo` (`FileName`, `Kind`, `SizeBytes`, `UseStill`) y `FondoKind` (`Image` / `Video`) en `COT_0.Core.Models`.
- `string Folder` · `bool FolderExists`
- `string PathOf(Fondo fondo)`
- `Task<IReadOnlyList<Fondo>> ScanAsync()` – ordenados por nombre; vacío si no existe la carpeta.

## Clases principales

- Host: `App`, `MainWindow` (un `Frame` con las páginas de `Views/Pantallas`), `Services/Sala/WebServerHost`, `Services/Tunel/CloudflareTunnelService`, `Services/Sala/QrService` (QRCoder). Ver `docs/ESTRUCTURA.md`.
- Web: `WebApp` (arma la app), `Services/Sala/ChatHub`, `Views/Pantallas/Join`, `Views/Pantallas/Chat`, `Views/Plantillas/MainLayout`.

## Configuración

Un solo `appsettings.json`, en `COT_0.Host/` (se copia a la salida y Web lo lee al estar hospedado):

- La cuenta del host no va en `appsettings.json`: se registra en la app y se guarda en `%LocalAppData%\COT_0\usuarios.json` (contraseña y respuesta secreta con PBKDF2).
- `Paths:MediaRoot`: única ruta absoluta de la PC (carpeta con `fondos`, `musica`, `portadas`, `gifs`, `manual`). Las rutas de `Background:Folder`, `Audio:MusicFolder`, `Audio:CoversFolder`, `Manual:*` y `GuestManual:AssetsFolder` van relativas a ella (`COT_0.Core/Services/MediaPath`).
- `Web:Url`: URL local del servidor.

`COT_0.Host` copia `COT_0.Web/wwwroot` a su carpeta de salida; los archivos que agregue el Agente C se sirven solos.

## Pedidos de cambio

_(Anotar acá: agente, qué se necesita en Core y por qué.)_

- **Agente A**: `IQrService` en Core (ej. `byte[] GeneratePng(string text)`), para cumplir "todo servicio con interfaz en Core". Hoy `QrService` se registra como clase concreta porque devuelve `BitmapImage` (WPF). Config nueva en `appsettings.json`: `Tunnel:CloudflaredPath`, `Tunnel:StartTimeoutSeconds`, `Qr:PixelsPerModule`.
- **Agente A** (rediseño WPF): servicios del Host que hoy se registran como clases concretas por depender de WPF o ser solo de UI: `QrService`, `MusicService` (MediaPlayer), `DialogService`, `BackgroundCatalog`, `WindowNavigator`, `ScreenNavigator`. Si se quiere cumplir "interfaz en Core", propongo en Core: `IQrService` (`byte[] GeneratePng(string text)`), `IMusicService` (`bool IsAvailable`, `double Volume`, `void Start()`), `IDialogService` (`ShowManual()`, `ShowError(title, message)`). Los navegadores y el catálogo de fondos usan tipos WPF y quedarían en Host. Config nueva en `appsettings.json`: `Web:JoinPath`, `Background:*`, `Audio:*`, `Links:GitHub`, `Ui:*`, `Texts:*`, `Qr:DarkColor`/`LightColor`.
- **Agente B → Agente 0** (hecho): `IEmoteService`, `IFlagService` y sus modelos pasaron a Core. `IAvatarStore`/`StoredAvatar` quedan en Web por ahora. `IProfileService` e `IChatSession` quedan en Web (usan `IBrowserFile` / SignalR client).
- **Agente B → Agente A** (hecho en el merge): sección `"Chat"` en `COT_0.Host/appsettings.json` (todo tiene default, ver `COT_0.Web/Services/Configuracion/ChatOptions.cs`): `MaxNameLength`, `MaxMessageLength`, `HistorySize`, `MessagesOnJoin`, `TimeFormat`, `PhotoSize`, `MaxPhotoBytes`, etc.
- **Agente B → Agente C**: banderas en `wwwroot/img/flags/{clave}.png`, emotes en `wwwroot/emotes/{codigo}.gif|png|webp` (el código `:rana:` sale del nombre del archivo; se leen al arrancar). Clases extra además de las pedidas: `.chat-page`, `.chat-main`, `.msg-list`, `.msg-body`, `.msg-header`, `.msg-author`, `.msg-time`, `.msg-text`, `.avatar-initial`, `.user-item`, `.user-name`, `.chat-input`, `.emote-picker`, `.emote-toggle`, `.emote-panel`, `.emote-option`, `.join-page`, `.join-form`, `.join-photo`, `.join-field`, `.flag-picker`, `.flag-option`, `.join-error`, `.join-submit`. Para que el chat quede scrolleado abajo sin JS: `.msg-list { display:flex; flex-direction:column; overflow-y:auto }` + `margin-top:auto` en el primer `.msg`, o pedime invertir el orden para usar `column-reverse`.
- **Agente Web** (diseño 04/05): claves nuevas en `Chat` (todas con default en `ChatOptions`): `FlagNames` (diccionario clave de escudo → nombre visible), `AshSeed`, `AshCount` (ceniza del fondo). Hub: eventos nuevos `"Kicked"` (solo al expulsado) y `"RoomClosed"` (a todos); los manda `Services/Sala/ChatBroadcaster` escuchando `IChatService.UserKicked/UsersChanged` e `IRoomService.Changed`, así que el Host solo llama a `KickUserAsync` / `CloseAsync`. El expulsado vuelve a `/join?reason=kicked`.
- **Agente Web → Agente A**: el Host tiene que llamar a `IRoomService.Open()` al generar el QR; con la sala cerrada `/join` muestra un aviso y no deja entrar. En standalone (`dotnet run` de Web) `Program.cs` abre la sala sola.
- **Agente Web → Agente 0**: `IProfileService` e `IChatSession` siguen en Web (usan `IBrowserFile` y el cliente de SignalR); si se quieren en Core habría que sacar esos tipos de la firma.
- **Agente Web → Agente 0**: editar perfil desde el chat. Hoy vive en Web como `Services/IUserEditor` (`Task<User?> UpdateUserAsync(userId, name, photoUrl, facultyFlag)`), implementada por `InMemoryChatService` (misma instancia que `IChatService`) y dispara `UsersChanged`, así que el Host y los clientes del hub se actualizan solos. Propuesta: sumar ese método a `IChatService` en Core y borrar `IUserEditor`.
- **Agente Web → Agente 0**: al editar el perfil, `IUserEditor.UpdateUserAsync` también reescribe `UserName`/`UserPhotoUrl`/`UserFacultyFlag` en los mensajes guardados de ese `UserId`, y dispara `event Action<User>? UserUpdated` (en `IUserEditor`, Web). `ChatBroadcaster` lo manda a todos por el hub como `"UserUpdated"` (con el `User`) y cada cliente reemplaza los datos en los mensajes ya pintados. Propuesta: sumar `UpdateUserAsync` y `UserUpdated` a `IChatService` en Core y borrar `IUserEditor`.
- **Agente Web → Agente 0** (chat sólido): los avisos del sistema (entró / salió / expulsado) se guardan como `ChatMessage` con `UserId = "system"` (`Web/Services/SystemNotice`). Propuesta: `ChatMessage.Kind` (`Text` / `Joined` / `Left` / `Kicked`) en Core. Los mensajes nuevos los avisa `IChatFeed.MessageAdded` (Web, misma instancia que `IChatService`) y `ChatBroadcaster` los manda en orden; el hub ya no hace broadcast directo. Al desconectarse, `IPresenceTracker` espera `Chat:LeaveGraceSeconds` antes de sacar al usuario (si se reconecta, no se duplica ni sale/entra). Hub: método nuevo `Typing()` y evento `"UserTyping"` (userId, nombre). `IMessageFormatter` e `IChatBrowser` quedan en Web (usan `ElementReference`/`IJSRuntime`).
- **Agente Web → Agente A**: claves nuevas en `Chat` (todas con default en `ChatOptions`, no hace falta tocar el appsettings del Host): `GroupWindowMinutes`, `TypingThrottleSeconds`, `TypingTimeoutSeconds`, `LeaveGraceSeconds`, `CircuitRetentionMinutes`, `ReconnectRetries`, `ReconnectIntervalMs`, `JoinedNotice`, `LeftNotice`, `KickedNotice`.
- **Agente Web → Agente C**: JS mínimo en `wwwroot/js/` (`chat.js`: Enter envía, scroll y aviso de nuevos; `reconnect.js`: reconexión de Blazor). CSS nuevo: `messages.css`, `composer.css`, `reconnect.css`.
- **Agente Web → Agente 0** (notas de voz): una nota viaja como `ChatMessage` con texto reservado `"\u0001voice:{clipId}"` (`Web/Services/VoiceNote`; el hub rechaza ese texto si lo escribe un invitado). Propuesta: `ChatMessage.Kind = Voice` + `AudioUrl` / `AudioDuration` en Core. Servicios en Web: `IVoiceService` (valida por magic bytes, guarda y publica), `IVoiceStore` (memoria, se vacía en `RoomService.CloseAsync`), `IAudioTranscoder` (WebM/Opus → WAV con el paquete NuGet Concentus, para que suene en iPhone), `IVoiceRecorder` (Scoped, interop con `wwwroot/js/voice.js`). Endpoint `GET {Voice:Path}/{id}` con Range.
- **Agente Web → Agente A**: sección nueva `Voice` (todo con default en `Web/Services/Configuracion/VoiceOptions.cs`): `Path`, `MimeCandidates`, `MaxSeconds`, `MinMilliseconds`, `MaxBytes`, `MaxClips`, `WavSampleRate`.
- **Agente Web → Agente 0** (imágenes): una imagen viaja como `ChatMessage` con texto reservado `"\u0001image:{imageId}"` (`Web/Services/ImageNote`, mismo mecanismo que `VoiceNote`; el hub rechaza ese texto si lo escribe un invitado). Propuesta: `ChatMessage.Kind = Image` + `ImageUrl` / `ImageWidth` / `ImageHeight` en Core. Servicios en Web: `IImageService` (valida por magic bytes JPEG/PNG/WebP/GIF y tamaño, guarda y publica), `IImageStore` (memoria con tope total por sala, se vacía en `RoomService.CloseAsync`), `IImageUploader` (Scoped: achica en el navegador con `RequestImageFileAsync`, vista previa y subida). Endpoint `GET {Chat:ImagesPath}/{id}` con `nosniff` y cache.
- **Agente Web → Agente A**: claves nuevas en `Chat` (todas con default en `ChatOptions`): `ImagesPath`, `ImageFormat`, `ImageMaxSide`, `ImagePreviewSide`, `MaxImageBytes`, `ImageDecodeTimeoutSeconds`, `MaxRoomImageBytes`, `ImageThumbWidth`, `ImageThumbMaxHeight`, `ImageCacheSeconds`.
- **Agente Web → Agente 0** (emotes 7TV): los emotes ya no salen de `wwwroot/emotes` sino de un set de 7TV (API pública v3, `GET https://7tv.io/v3/emote-sets/{id}`; imágenes directo de `cdn.7tv.app`, nada en disco). Web ya no usa `IEmoteService`, `Emote` ni `MessagePart` de Core (se borró `Web/Services/EmoteService.cs`). Propuesta: reemplazarlos en Core por `IRoomEmotes` + `RoomEmote` / `EmoteSources` / `EmoteReport` / `EmoteSetStatus`, hoy en `Web/Services/Emotes` (aislado del resto por si se cobra aparte): `SetId`, `SetName`, `Status`, `Version`, `Active`, `Removed`, `Reports`, `event Changed`, `event Reported(EmoteReport)`, `Task<EmoteSetStatus> UseSetAsync(string? idOrUrl)`, `Find(name)`, `Search(query, max)`, `bool Report(emoteId, userId, userName)`, `Remove(emoteId)`, `Restore(emoteId)`, `ClearReports()`. También `ISevenTvClient` (cliente tipado) e `IEmoteActions` (Scoped, menú del emote en el chat).
- **Agente Web → Agente A** (emotes 7TV): el Host saca `IRoomEmotes` del contenedor de la web (Singleton, misma instancia que usan los chats). `UseSetAsync(idOrUrl)` cambia el set (acepta `https://7tv.app/emote-sets/{id}`, el ID o `"global"`; `null` apaga los emotes). `Reported` llega desde otro hilo: usar `Dispatcher.Invoke`. `Remove(emoteId)` lo saca del picker y todos los mensajes (viejos y nuevos) lo muestran como texto; `Restore` lo devuelve. Claves nuevas en `Chat` (con default en `ChatOptions`): `SevenTvApiUrl`, `SevenTvEmoteSetPath`, `SevenTvCdnHost`, `SevenTvUserAgent`, `EmoteSetId`, `EmoteCacheMinutes`, `EmoteRetrySeconds`, `EmoteRequestTimeoutSeconds`, `EmoteScale`, `EmoteLargeScale`, `EmoteFormat`, `EmoteFallbackFormat`, `EmotePickerLimit`, `EmoteHoldMs`. `Chat:EmotesFolder` ya no se usa (se puede borrar del appsettings del Host).
- **Agente Web → Agente C**: `wwwroot/emotes/` ya no se usa. CSS nuevo `css/emotes.css` (emote inline, picker, menú REPORTAR); `chat.js` suma mantener presionado un emote (abre el menú) y Tab para completar la sugerencia de `:`.
- **Agente A** (manual de Lain, P10b): `ManualAssets`, `ManualSound` (MediaPlayer) y `ManualDemoFactory` se registran como clases concretas en el Host por usar tipos de WPF. Si se quiere interfaz en Core: `IManualSound` (`bool IsMuted`, `PlayNotice()`, `StartStatic()`, `Stop()`). Config nueva en `appsettings.json`: sección `Manual` (rutas de videos y sonidos fuera del repo, volúmenes, `CueSeconds`, `StepChangeMilliseconds`, `Demo`) y `Texts:Manual` (textos y pasos). Se quitó `Texts:Manual:Text` y `DialogService.ShowManual`.
- **Agente B → Agente 0 / Agente A** (manual del invitado): no hace falta nada en Core. Rutas nuevas en Web: `/manual` (`Views/Pantallas/Manual.razor`, enlace discreto desde `/join`) y `/manual-assets/*` (audios desde una carpeta local fuera del repo). Sección opcional `"GuestManual"` (defaults en `COT_0.Web/Services/Configuracion/ManualOptions.cs`); el Host puede pisar `GuestManual:AssetsFolder` en su `appsettings.json` como hace con `Background:Folder`. `IManualTour`/`IManualDemo` quedan en Web (dependen de modelos de Web).
- **Agente Web → Agente 0** (emotes en el cuadro de escritura): el cuadro del chat ya no es `<textarea>` sino un `contenteditable` manejado por `wwwroot/js/composer.js`; los emotes se ven como imagen y C# recibe el texto con las claves (el mensaje enviado no cambia). Servicio nuevo en Web: `IComposerEditor` (Scoped, usa `ElementReference`/`IJSRuntime`, queda en Web como `IChatBrowser`). Core no cambia.
- **Agente A (WinUI, rama `winui-migration`)**: no hizo falta tocar Core. Pedidos para cuando termine la migración:
  - `IAuthService` ya no lo usa el Host WinUI (login con `UserService` + `usuarios.json`, contraseña y respuesta secreta con PBKDF2). Propuesta: borrarlo de Core junto con el WPF, o cambiarlo por `IHostAccountService` (`HasUsersAsync`, `RegisterAsync(user, password, questionKey, answer)`, `LoginAsync`) si se quiere cumplir "interfaz en Core".
  - Los servicios del Host WinUI (`UserService`, `PasswordRecoveryService`, `VisitService`, `SesionService`) son clases estáticas sin interfaz en Core. Si se pide interfaz: `IVisitLog` (`RecordAsync(name)`, `GetAllAsync()`) e `IGuestSessions` (`StartAsync()`, `AddGuestAsync(name)`, `EndAsync()`, `GetAllAsync()`); los modelos `Visit`/`SesionInvitados` pasarían a Core.
  - Música (WinUI): `MusicLibrary` y `CoverLibrary` viven en el Host WPF, no en Core, y WinUI no puede referenciar un proyecto WPF; quedaron copiadas en `COT_0.Host/Services/Musica`. Si se quieren compartidas: moverlas a Core (no dependen de WPF; solo de `MediaPath` y de la config `Audio:*`) e `IMusicPlayer` (`StartAsync`, `PlayAsync(path)`, `TogglePlay`, `NextAsync`, `PreviousAsync`, `Volume`, `IsMuted`, `Current`, `event StateChanged`).
  - Emotes (WinUI): se usa `IRoomEmotes` tal cual, sin cambios.
  - Fondos (WinUI): `BackgroundCatalog` vive en el Host WPF y WinUI no puede referenciarlo; quedó copiado y adaptado como `COT_0.Host/Services/Fondos/FondoCatalog` (async, sin `BackgroundOption` de WPF), más `FondoStore` (`fondos.json`), `FondoService`, `FondoImages` y `FondoPlayer`. Son clases concretas porque usan tipos de WinUI (`BitmapImage`, `MediaPlayer`). Si se quiere interfaz en Core: `IBackgroundCatalog` (`Task<IReadOnlyList<...>> ScanAsync()`) con un modelo de fondo sin tipos de vista. Config nueva en el `appsettings.json` del Host WinUI: `Background:HeavyVideoMegabytes`, `ThumbnailSize`, `StillSize`, `VeilOpacity`, `PickerVeilOpacity` y `Storage:BackgroundsFile`.
  - Manual (WinUI): `ManualAssets` vive en el Host WPF; quedó copiado y adaptado (async) en `COT_0.Host/Services/Manual`, y `ManualSound` reescrito con `Windows.Media.Playback`. `ManualDemoFactory` no se migró (el manual de WinUI ya no usa vistas de muestra). Config nueva: `Manual:StagesFolder`, `StageExtension`, `StageDecodeSize`, `NoticeCounts`; los textos pasaron a `Textos.xaml`.
  - `.gitignore` de la raíz (no es carpeta de ningún agente): la regla `musica/` tapa `COT_0.Host/Services/Musica/`. Pedido: cambiarla a `/musica/`. Mientras, `COT_0.Host/.gitignore` la vuelve a incluir.
  - Cierre de la migración (Agente 0): borrar el Host WPF, renombrar el proyecto WinUI a `COT_0.Host` y agregarlo a `COT_0.sln`. Lista completa en `docs/PENDIENTES_LIMPIEZA.md`.
  - Sesiones de invitados: `SesionWatcher` se entera de quién entra comparando `GetUsersAsync()` en cada `UsersChanged` (igual que `VisitWatcher`). Un evento `event Action<User>? UserJoined` en `IChatService` lo haría directo.
- **Agente Host → Agente 0 (09/11, tienda de emotes)**: para que la tienda del Host meta al chat los emotes comprados se agregó en Web `Services/Emotes/IPurchasedEmotes.cs` (`Purchased`, `SetPurchased`), implementada por la misma instancia de `RoomEmotes` que `IRoomEmotes` (`EmoteRegistration`). Pedido: mover esa interfaz a `COT_0.Core/Services` (o sumar `SetPurchased` a `IRoomEmotes`) para cumplir "todo servicio tiene interfaz en Core". También se sumó a `ISevenTvClient` (Web) `SearchEmotesAsync` (GraphQL v3 de 7TV, `Chat:SevenTvGqlPath`) y `EmoteCatalogPage`. No se tocó Core.
- **Agente Host → Agente Web (09/11, picker)**: `EmoteSearch.Find` ya no corta la lista completa a `Chat:EmotePickerLimit` (el tope solo aplica a búsquedas) y `EmotePicker.razor` muestra la cuadrícula si hay emotes activos aunque el set esté apagado (comprados). No se tocó `wwwroot`.

### Resueltos por el Agente 0 (cierre 08/11, rama `host-winui`)

| Pedido | Estado | Razón |
|---|---|---|
| `IBackgroundCatalog` en Core | **Aplicado** | `COT_0.Core/Services/IBackgroundCatalog.cs`; `Fondo` y `FondoKind` pasaron a `COT_0.Core/Models`. `FondoCatalog` (Host) la implementa y `FondoService` la recibe por DI. Sin cambio de lógica |
| Config `Background:*` (`HeavyVideoMegabytes`, `ThumbnailSize`, `StillSize`, `VeilOpacity`, `PickerVeilOpacity`) y `Storage:BackgroundsFile` | **Aplicado** | Ya estaban en `COT_0.Host/appsettings.json` |
| Config `Manual:*` (`StagesFolder`, `StageExtension`, `StageDecodeSize`, `NoticeCounts`) | **Aplicado** | Ya estaban en `COT_0.Host/appsettings.json` |
| Config `Tunnel:*`, `Qr:*`, `Web:JoinPath`, `Audio:*`, `Ui:*` | **Aplicado** | Ya estaban; `Links:GitHub` es `Branding:RepoUrl` y `Texts:*` vive en `Views/Estilos/Textos.xaml` |
| Borrar `IAuthService` | **Aplicado** | Nadie lo usaba fuera del WPF |
| `.gitignore`: `musica/` → `/musica/` y borrar `COT_0.Host/.gitignore` | **Aplicado** | Reglas de medios ancladas en la raíz; `git check-ignore` confirma que el código no se ignora |
| Borrar el WPF, renombrar el proyecto WinUI → `COT_0.Host` y agregarlo a `COT_0.sln` | **Aplicado** | Pasos 2 y 3 de `docs/REPORTE_CIERRE.md` |
| `event Action<User>? UserJoined` en `IChatService` | **Descartado** | Cambia cuándo cuenta una entrada: hoy `GuestWatcher` compara la lista y cuenta como nueva a quien sale y vuelve; con un evento en `AddUserAsync` habría que revisar reconexiones (`PresenceTracker`). No es solo mover código |
| `IHostAccountService`, `IVisitLog`, `IGuestSessions` | **Descartado** | `UserService`, `VisitService` y `SesionService` son estáticos; pasarlos a interfaz cambia cómo se llaman en todos los ViewModels. Queda como mejora en `docs/REPORTE_CIERRE.md` |
| `MusicLibrary`/`CoverLibrary` a Core e `IMusicPlayer` | **Descartado** | El duplicado se fue con el WPF; leen `AppConfig` del Host y `MusicService` usa `Windows.Media.Playback` |
| `IQrService`, `IMusicService`, `IDialogService`, `IManualSound` | **Descartado** | Devuelven o usan tipos de WinUI (`BitmapImage`, `MediaPlayer`, `ContentDialog`); separar la parte sin vista es cambio de lógica |

### Resueltos por el Agente 0 (auditoría 05/11)

- `IUserEditor` (Web) → `IChatService.UpdateUserAsync` + `event UserUpdated` en Core. `IUserEditor` se borró.
- `IRoomEmotes` y sus modelos pasaron de `Web/Services/Emotes` a Core. `EmoteSetResult`, `ISevenTvClient` e `IEmoteActions` quedan en Web (son del cliente de 7TV y de la vista).
- `IEmoteService`, `Emote` y `MessagePart` se borraron de Core (nadie los usaba desde los emotes de 7TV).
- `ChatMessage.Kind` (`MessageKind`), `AudioUrl` e `ImageUrl` en Core; los avisos del sistema y las notas de voz e imágenes ya los completan (`IChatService.AddMediaAsync`). **Pendiente**: el render y el hub todavía reconocen voz e imagen por el texto reservado (`VoiceNote` / `ImageNote`); pasar a `Kind` queda para después. No se agregaron `AudioDuration` ni `ImageWidth`/`ImageHeight`: la duración y el tamaño salen de `IVoiceStore` / `IImageStore`.

## Supuesto de una sola sala (para el futuro, sin refactorizar)

Hoy todo asume **una sala por proceso**. Si algún día hay varias, esto es lo que habría que separar por sala:

- **Core**: `IRoomService` tiene un solo `Code` / `OpenedAt`; `IChatService` guarda una sola lista de usuarios e historial; `IRoomEmotes` un solo set, quitados y reportes. Ninguna firma recibe un id de sala.
- **Web, servicios Singleton globales**: `InMemoryChatService`, `RoomService`, `RoomEmotes`, `PresenceTracker`, `InMemoryVoiceStore`, `InMemoryImageStore` (tope `MaxRoomImageBytes` para todo el proceso) e `InMemoryAvatarStore`. `RoomService.CloseAsync` vacía chat, voz e imágenes de todos.
- **Hub**: `ChatBroadcaster` manda todo con `Clients.All` (sin grupos de SignalR por sala); `RoomClosed` le llega a todos los conectados.
- **Rutas**: `/join`, `/chat`, `/chathub`, `{Voice:Path}/{id}`, `{Chat:ImagesPath}/{id}` y `/avatars/{id}` no llevan sala; el código de sala solo se muestra, no se usa para entrar.
- **Navegador**: el invitado se guarda en `localStorage` con una sola clave (`Chat:GuestStorageKey`).
- **Host**: `RoomSession`, `SalaViewModel`, `DashboardViewModel` y `EmotesViewModel` manejan una sala; un solo túnel de cloudflared (`ITunnelService.PublicUrl`) y un solo QR a `{PublicUrl}{Web:JoinPath}`.

## Registro

- **Fix de emotes (commit `e219f9e`, 04/11, rama `host`)**: el agente del Host tocó archivos de Web fuera de su carpeta para arreglar el timeout de 7TV: `Web/Services/Configuracion/ChatOptions.cs`, `Web/Services/Emotes/EmoteSetStatus.cs`, `IRoomEmotes.cs`, `RoomEmotes.cs` y `SevenTvClient.cs` (además de `Host/Views/ModelosDeVista/EmotesViewModel.cs` y `Host/appsettings.json`). Queda anotado como excepción a "cada agente solo en su carpeta".
- **QA 08/11 (rama `host-winui`, F14)**: el agente del Host tocó archivos de Web fuera de su carpeta para dejar los textos visibles en español: `Web/Views/Plantillas/MainLayout.razor` (aviso de error de Blazor) y `Web/Views/Pantallas/Error.razor` (página `/Error`). Solo textos; sin cambios de lógica ni de Core.
- **Agente Host → Agente Web (09/11, validación estricta)**: el Host ya solo deja escribir letras y números en el set de 7TV (sin URL) y lo revisa en `EmotesViewModel.ApplyAsync` antes de llamar `UseSetAsync`. Pedido opcional: que `RoomEmotes.UseSetAsync` rechace también lo que no sea letra o número, para no confiar solo en el Host. No se tocó Web.
- **Pruebas**: la solución no tiene proyectos de prueba (`COT_0.sln` solo incluye Core, Web y Host). Las verificaciones se hacen compilando (0 errores, 0 warnings) y probando la app a mano.
- **Auditoría 05/11 (Agente 0, sola)**: arranque del Host medido y optimizado (`Diagnostics:StartupTrace` deja el log de tiempos, apagado por defecto), pedidos de Core aplicados, rutas relativas a `Paths:MediaRoot`, credenciales en user-secrets, cultura `es-MX` y textos sin voseo.
