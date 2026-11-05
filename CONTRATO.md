# CONTRATO – COT_0

Acuerdo entre agentes. Si algo de acá tiene que cambiar, se anota en **Pedidos de cambio** (al final) y lo resuelve el Agente 0.

## Solución

```
COT_0.sln
COT_0.Core/   class library   → solo Agente 0
COT_0.Web/    Blazor Server   → Agente B (menos wwwroot) / Agente C (wwwroot)
COT_0.Host/   WPF             → Agente A
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
- `COT_0.Host` arranca la web dentro de su propio proceso (`Services/WebServerHost.cs` → `WebApp.Create(...)`) al iniciar la app. Usa `FrameworkReference Microsoft.AspNetCore.App`.
- `COT_0.Web` también corre solo con `dotnet run` (mismo puerto) para probar sin el Host.

| Ruta | Qué es |
|---|---|
| `/` | Redirige a `/join` |
| `/join` | `Pages/Join.razor`: el invitado carga nombre, foto y bandera de facultad |
| `/chat` | `Pages/Chat.razor`: chat grupal en tiempo real |
| `/chathub` | `Hubs/ChatHub.cs`: hub de SignalR |

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

### `IRoomService` – implementa Web (`Services/RoomService.cs`, **Singleton**)
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

### `ITunnelService` – implementa Agente A (`Services/CloudflareTunnelService.cs`)
- `string? PublicUrl` – `https://xxxx.trycloudflare.com` o `null`.
- `bool IsRunning`
- `Task<string> StartAsync(string localUrl, CancellationToken ct = default)` – lanza `cloudflared tunnel --url {localUrl}` y devuelve la URL pública.
- `Task StopAsync()` – cierra cloudflared.

### `IAuthService` – implementa Agente A (en Host)
Login del host contra `appsettings.json` → `Host:User` / `Host:Password`.
- `bool IsAuthenticated`
- `Task<bool> LoginAsync(string user, string password)`
- `void Logout()`

## Clases existentes (stubs)

- Host: `LoginWindow`, `MainWindow`, `Services/WebServerHost` (funciona), `Services/CloudflareTunnelService`, `Services/QrService` (QRCoder ya instalado).
- Web: `WebApp` (arma la app), `Hubs/ChatHub`, `Pages/Join`, `Pages/Chat`, `Layout/MainLayout`.

## Configuración

Un solo `appsettings.json`, en `COT_0.Host/` (se copia a la salida y Web lo lee al estar hospedado):

- `Host:User` / `Host:Password` **no** van en `appsettings.json` (quedan vacíos): se cargan con `dotnet user-secrets` del Host:
  `dotnet user-secrets set "Host:User" "..." --project COT_0.Host` (y lo mismo con `Host:Password`).
- `Paths:MediaRoot`: única ruta absoluta de la PC (carpeta con `fondos`, `musica`, `portadas`, `gifs`, `manual`). Las rutas de `Background:Folder`, `Audio:MusicFolder`, `Audio:CoversFolder`, `Manual:*` y `GuestManual:AssetsFolder` van relativas a ella (`COT_0.Core/Services/MediaPath`).
- `Web:Url`: URL local del servidor.

`COT_0.Host` copia `COT_0.Web/wwwroot` a su carpeta de salida; los archivos que agregue el Agente C se sirven solos.

## Pedidos de cambio

_(Anotar acá: agente, qué se necesita en Core y por qué.)_

- **Agente A**: `IQrService` en Core (ej. `byte[] GeneratePng(string text)`), para cumplir "todo servicio con interfaz en Core". Hoy `QrService` se registra como clase concreta porque devuelve `BitmapImage` (WPF). Config nueva en `appsettings.json`: `Tunnel:CloudflaredPath`, `Tunnel:StartTimeoutSeconds`, `Qr:PixelsPerModule`.
- **Agente A** (rediseño WPF): servicios del Host que hoy se registran como clases concretas por depender de WPF o ser solo de UI: `QrService`, `MusicService` (MediaPlayer), `DialogService`, `BackgroundCatalog`, `WindowNavigator`, `ScreenNavigator`. Si se quiere cumplir "interfaz en Core", propongo en Core: `IQrService` (`byte[] GeneratePng(string text)`), `IMusicService` (`bool IsAvailable`, `double Volume`, `void Start()`), `IDialogService` (`ShowManual()`, `ShowError(title, message)`). Los navegadores y el catálogo de fondos usan tipos WPF y quedarían en Host. Config nueva en `appsettings.json`: `Web:JoinPath`, `Background:*`, `Audio:*`, `Links:GitHub`, `Ui:*`, `Texts:*`, `Qr:DarkColor`/`LightColor`.
- **Agente B → Agente 0** (hecho): `IEmoteService`, `IFlagService` y sus modelos pasaron a Core. `IAvatarStore`/`StoredAvatar` quedan en Web por ahora. `IProfileService` e `IChatSession` quedan en Web (usan `IBrowserFile` / SignalR client).
- **Agente B → Agente A** (hecho en el merge): sección `"Chat"` en `COT_0.Host/appsettings.json` (todo tiene default, ver `COT_0.Web/Options/ChatOptions.cs`): `MaxNameLength`, `MaxMessageLength`, `HistorySize`, `MessagesOnJoin`, `TimeFormat`, `PhotoSize`, `MaxPhotoBytes`, etc.
- **Agente B → Agente C**: banderas en `wwwroot/img/flags/{clave}.png`, emotes en `wwwroot/emotes/{codigo}.gif|png|webp` (el código `:rana:` sale del nombre del archivo; se leen al arrancar). Clases extra además de las pedidas: `.chat-page`, `.chat-main`, `.msg-list`, `.msg-body`, `.msg-header`, `.msg-author`, `.msg-time`, `.msg-text`, `.avatar-initial`, `.user-item`, `.user-name`, `.chat-input`, `.emote-picker`, `.emote-toggle`, `.emote-panel`, `.emote-option`, `.join-page`, `.join-form`, `.join-photo`, `.join-field`, `.flag-picker`, `.flag-option`, `.join-error`, `.join-submit`. Para que el chat quede scrolleado abajo sin JS: `.msg-list { display:flex; flex-direction:column; overflow-y:auto }` + `margin-top:auto` en el primer `.msg`, o pedime invertir el orden para usar `column-reverse`.
- **Agente Web** (diseño 04/05): claves nuevas en `Chat` (todas con default en `ChatOptions`): `FlagNames` (diccionario clave de escudo → nombre visible), `AshSeed`, `AshCount` (ceniza del fondo). Hub: eventos nuevos `"Kicked"` (solo al expulsado) y `"RoomClosed"` (a todos); los manda `Hubs/ChatBroadcaster` escuchando `IChatService.UserKicked/UsersChanged` e `IRoomService.Changed`, así que el Host solo llama a `KickUserAsync` / `CloseAsync`. El expulsado vuelve a `/join?reason=kicked`.
- **Agente Web → Agente A**: el Host tiene que llamar a `IRoomService.Open()` al generar el QR; con la sala cerrada `/join` muestra un aviso y no deja entrar. En standalone (`dotnet run` de Web) `Program.cs` abre la sala sola.
- **Agente Web → Agente 0**: `IProfileService` e `IChatSession` siguen en Web (usan `IBrowserFile` y el cliente de SignalR); si se quieren en Core habría que sacar esos tipos de la firma.
- **Agente Web → Agente 0**: editar perfil desde el chat. Hoy vive en Web como `Services/IUserEditor` (`Task<User?> UpdateUserAsync(userId, name, photoUrl, facultyFlag)`), implementada por `InMemoryChatService` (misma instancia que `IChatService`) y dispara `UsersChanged`, así que el Host y los clientes del hub se actualizan solos. Propuesta: sumar ese método a `IChatService` en Core y borrar `IUserEditor`.
- **Agente Web → Agente 0**: al editar el perfil, `IUserEditor.UpdateUserAsync` también reescribe `UserName`/`UserPhotoUrl`/`UserFacultyFlag` en los mensajes guardados de ese `UserId`, y dispara `event Action<User>? UserUpdated` (en `IUserEditor`, Web). `ChatBroadcaster` lo manda a todos por el hub como `"UserUpdated"` (con el `User`) y cada cliente reemplaza los datos en los mensajes ya pintados. Propuesta: sumar `UpdateUserAsync` y `UserUpdated` a `IChatService` en Core y borrar `IUserEditor`.
- **Agente Web → Agente 0** (chat sólido): los avisos del sistema (entró / salió / expulsado) se guardan como `ChatMessage` con `UserId = "system"` (`Web/Services/SystemNotice`). Propuesta: `ChatMessage.Kind` (`Text` / `Joined` / `Left` / `Kicked`) en Core. Los mensajes nuevos los avisa `IChatFeed.MessageAdded` (Web, misma instancia que `IChatService`) y `ChatBroadcaster` los manda en orden; el hub ya no hace broadcast directo. Al desconectarse, `IPresenceTracker` espera `Chat:LeaveGraceSeconds` antes de sacar al usuario (si se reconecta, no se duplica ni sale/entra). Hub: método nuevo `Typing()` y evento `"UserTyping"` (userId, nombre). `IMessageFormatter` e `IChatBrowser` quedan en Web (usan `ElementReference`/`IJSRuntime`).
- **Agente Web → Agente A**: claves nuevas en `Chat` (todas con default en `ChatOptions`, no hace falta tocar el appsettings del Host): `GroupWindowMinutes`, `TypingThrottleSeconds`, `TypingTimeoutSeconds`, `LeaveGraceSeconds`, `CircuitRetentionMinutes`, `ReconnectRetries`, `ReconnectIntervalMs`, `JoinedNotice`, `LeftNotice`, `KickedNotice`.
- **Agente Web → Agente C**: JS mínimo en `wwwroot/js/` (`chat.js`: Enter envía, scroll y aviso de nuevos; `reconnect.js`: reconexión de Blazor). CSS nuevo: `messages.css`, `composer.css`, `reconnect.css`.
- **Agente Web → Agente 0** (notas de voz): una nota viaja como `ChatMessage` con texto reservado `"\u0001voice:{clipId}"` (`Web/Services/VoiceNote`; el hub rechaza ese texto si lo escribe un invitado). Propuesta: `ChatMessage.Kind = Voice` + `AudioUrl` / `AudioDuration` en Core. Servicios en Web: `IVoiceService` (valida por magic bytes, guarda y publica), `IVoiceStore` (memoria, se vacía en `RoomService.CloseAsync`), `IAudioTranscoder` (WebM/Opus → WAV con el paquete NuGet Concentus, para que suene en iPhone), `IVoiceRecorder` (Scoped, interop con `wwwroot/js/voice.js`). Endpoint `GET {Voice:Path}/{id}` con Range.
- **Agente Web → Agente A**: sección nueva `Voice` (todo con default en `Web/Options/VoiceOptions.cs`): `Path`, `MimeCandidates`, `MaxSeconds`, `MinMilliseconds`, `MaxBytes`, `MaxClips`, `WavSampleRate`.
- **Agente Web → Agente 0** (imágenes): una imagen viaja como `ChatMessage` con texto reservado `"\u0001image:{imageId}"` (`Web/Services/ImageNote`, mismo mecanismo que `VoiceNote`; el hub rechaza ese texto si lo escribe un invitado). Propuesta: `ChatMessage.Kind = Image` + `ImageUrl` / `ImageWidth` / `ImageHeight` en Core. Servicios en Web: `IImageService` (valida por magic bytes JPEG/PNG/WebP/GIF y tamaño, guarda y publica), `IImageStore` (memoria con tope total por sala, se vacía en `RoomService.CloseAsync`), `IImageUploader` (Scoped: achica en el navegador con `RequestImageFileAsync`, vista previa y subida). Endpoint `GET {Chat:ImagesPath}/{id}` con `nosniff` y cache.
- **Agente Web → Agente A**: claves nuevas en `Chat` (todas con default en `ChatOptions`): `ImagesPath`, `ImageFormat`, `ImageMaxSide`, `ImagePreviewSide`, `MaxImageBytes`, `ImageDecodeTimeoutSeconds`, `MaxRoomImageBytes`, `ImageThumbWidth`, `ImageThumbMaxHeight`, `ImageCacheSeconds`.
- **Agente Web → Agente 0** (emotes 7TV): los emotes ya no salen de `wwwroot/emotes` sino de un set de 7TV (API pública v3, `GET https://7tv.io/v3/emote-sets/{id}`; imágenes directo de `cdn.7tv.app`, nada en disco). Web ya no usa `IEmoteService`, `Emote` ni `MessagePart` de Core (se borró `Web/Services/EmoteService.cs`). Propuesta: reemplazarlos en Core por `IRoomEmotes` + `RoomEmote` / `EmoteSources` / `EmoteReport` / `EmoteSetStatus`, hoy en `Web/Services/Emotes` (aislado del resto por si se cobra aparte): `SetId`, `SetName`, `Status`, `Version`, `Active`, `Removed`, `Reports`, `event Changed`, `event Reported(EmoteReport)`, `Task<EmoteSetStatus> UseSetAsync(string? idOrUrl)`, `Find(name)`, `Search(query, max)`, `bool Report(emoteId, userId, userName)`, `Remove(emoteId)`, `Restore(emoteId)`, `ClearReports()`. También `ISevenTvClient` (cliente tipado) e `IEmoteActions` (Scoped, menú del emote en el chat).
- **Agente Web → Agente A** (emotes 7TV): el Host saca `IRoomEmotes` del contenedor de la web (Singleton, misma instancia que usan los chats). `UseSetAsync(idOrUrl)` cambia el set (acepta `https://7tv.app/emote-sets/{id}`, el ID o `"global"`; `null` apaga los emotes). `Reported` llega desde otro hilo: usar `Dispatcher.Invoke`. `Remove(emoteId)` lo saca del picker y todos los mensajes (viejos y nuevos) lo muestran como texto; `Restore` lo devuelve. Claves nuevas en `Chat` (con default en `ChatOptions`): `SevenTvApiUrl`, `SevenTvEmoteSetPath`, `SevenTvCdnHost`, `SevenTvUserAgent`, `EmoteSetId`, `EmoteCacheMinutes`, `EmoteRetrySeconds`, `EmoteRequestTimeoutSeconds`, `EmoteScale`, `EmoteLargeScale`, `EmoteFormat`, `EmoteFallbackFormat`, `EmotePickerLimit`, `EmoteHoldMs`. `Chat:EmotesFolder` ya no se usa (se puede borrar del appsettings del Host).
- **Agente Web → Agente C**: `wwwroot/emotes/` ya no se usa. CSS nuevo `css/emotes.css` (emote inline, picker, menú REPORTAR); `chat.js` suma mantener presionado un emote (abre el menú) y Tab para completar la sugerencia de `:`.
- **Agente A** (manual de Lain, P10b): `ManualAssets`, `ManualSound` (MediaPlayer) y `ManualDemoFactory` se registran como clases concretas en el Host por usar tipos de WPF. Si se quiere interfaz en Core: `IManualSound` (`bool IsMuted`, `PlayNotice()`, `StartStatic()`, `Stop()`). Config nueva en `appsettings.json`: sección `Manual` (rutas de videos y sonidos fuera del repo, volúmenes, `CueSeconds`, `StepChangeMilliseconds`, `Demo`) y `Texts:Manual` (textos y pasos). Se quitó `Texts:Manual:Text` y `DialogService.ShowManual`.
- **Agente B → Agente 0 / Agente A** (manual del invitado): no hace falta nada en Core. Rutas nuevas en Web: `/manual` (`Pages/Manual.razor`, enlace discreto desde `/join`) y `/manual-assets/*` (audios desde una carpeta local fuera del repo). Sección opcional `"GuestManual"` (defaults en `COT_0.Web/Options/ManualOptions.cs`); el Host puede pisar `GuestManual:AssetsFolder` en su `appsettings.json` como hace con `Background:Folder`. `IManualTour`/`IManualDemo` quedan en Web (dependen de modelos de Web).
- **Agente Web → Agente 0** (emotes en el cuadro de escritura): el cuadro del chat ya no es `<textarea>` sino un `contenteditable` manejado por `wwwroot/js/composer.js`; los emotes se ven como imagen y C# recibe el texto con las claves (el mensaje enviado no cambia). Servicio nuevo en Web: `IComposerEditor` (Scoped, usa `ElementReference`/`IJSRuntime`, queda en Web como `IChatBrowser`). Core no cambia.

### Resueltos por el Agente 0 (auditoría 05/11)

- `IUserEditor` (Web) → `IChatService.UpdateUserAsync` + `event UserUpdated` en Core. `IUserEditor` se borró.
- `IRoomEmotes` y sus modelos pasaron de `Web/Services/Emotes` a Core. `EmoteSetResult`, `ISevenTvClient` e `IEmoteActions` quedan en Web (son del cliente de 7TV y de la vista).
- `IEmoteService`, `Emote` y `MessagePart` se borraron de Core (nadie los usaba desde los emotes de 7TV).
- `ChatMessage.Kind` (`MessageKind`), `AudioUrl` e `ImageUrl` en Core; los avisos del sistema y las notas de voz e imágenes ya los completan (`IChatService.AddMediaAsync`). **Pendiente**: el render y el hub todavía reconocen voz e imagen por el texto reservado (`VoiceNote` / `ImageNote`); pasar a `Kind` queda para después. No se agregaron `AudioDuration` ni `ImageWidth`/`ImageHeight`: la duración y el tamaño salen de `IVoiceStore` / `IImageStore`.
