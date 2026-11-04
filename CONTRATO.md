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

- Servidor: **http://localhost:5050** (constante `WebApp.DefaultUrl`). Solo HTTP; el HTTPS público lo da cloudflared.
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
| `Text` | `string` | Contenido |
| `SentAt` | `DateTime` | UTC |

## Interfaces (`COT_0.Core.Services`)

### `IChatService` – implementa Agente B (en Web, registrar como **Singleton**)
Estado del chat en memoria.

- `Task<User> AddUserAsync(User user)` – agrega un invitado y lo devuelve.
- `Task RemoveUserAsync(string userId)` – lo saca (desconexión).
- `Task<User?> GetUserAsync(string userId)`
- `Task<IReadOnlyList<User>> GetUsersAsync()` – conectados.
- `Task<ChatMessage> AddMessageAsync(string userId, string text)` – crea el mensaje con los datos del autor.
- `Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int max = 100)` – últimos mensajes, del más viejo al más nuevo.
- `Task KickUserAsync(string userId)` – lo saca y dispara `UserKicked`.
- `Task ClearAsync()` – borra usuarios e historial (al cerrar la sala).
- `event Action? UsersChanged` – entra/sale/expulsado/vaciado. El Host lo escucha con `Dispatcher.Invoke`.
- `event Action<User>? UserKicked` – Web le avisa al expulsado por el hub (evento `"Kicked"`).

### `IRoomService` – implementa Web (`Services/RoomService.cs`, **Singleton**)
Estado de la sala. El Host la abre/cierra; Web muestra el código y bloquea `/join` si está cerrada.
- `string? Code` – código corto ("7F3K"), `null` si cerrada. Largo y caracteres en `Chat:RoomCodeLength` / `Chat:RoomCodeChars`.
- `DateTime? OpenedAt` (UTC) · `bool IsOpen`
- `void Open()` – genera código nuevo y marca la hora.
- `Task CloseAsync()` – cierra y llama a `IChatService.ClearAsync()`.
- `event Action? Changed`

El Host obtiene `IChatService` e `IRoomService` del contenedor de la app web que levanta `WebServerHost` (son la misma instancia que usa el hub).

### `IFlagService` / `IEmoteService` – implementa Web
Movidas a Core junto con sus modelos `FlagOption`, `Emote`, `MessagePart` (`COT_0.Core.Models`).

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

```json
{ "Host": { "User": "admin", "Password": "cambiame123" } }
```

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
- **Agente Web → Agente A**: sección nueva `Voice` (todo con default en `Web/Options/VoiceOptions.cs`): `Path`, `MimeCandidates`, `MaxSeconds`, `MinMilliseconds`, `MaxBytes`, `MaxClips`, `WavSampleRate`, `CancelSlidePx`.
- **Agente Web → Agente 0** (imágenes): una imagen viaja como `ChatMessage` con texto reservado `"\u0001image:{imageId}"` (`Web/Services/ImageNote`, mismo mecanismo que `VoiceNote`; el hub rechaza ese texto si lo escribe un invitado). Propuesta: `ChatMessage.Kind = Image` + `ImageUrl` / `ImageWidth` / `ImageHeight` en Core. Servicios en Web: `IImageService` (valida por magic bytes JPEG/PNG/WebP/GIF y tamaño, guarda y publica), `IImageStore` (memoria con tope total por sala, se vacía en `RoomService.CloseAsync`), `IImageUploader` (Scoped: achica en el navegador con `RequestImageFileAsync`, vista previa y subida). Endpoint `GET {Chat:ImagesPath}/{id}` con `nosniff` y cache.
- **Agente Web → Agente A**: claves nuevas en `Chat` (todas con default en `ChatOptions`): `ImagesPath`, `ImageFormat`, `ImageMaxSide`, `ImagePreviewSide`, `MaxImageBytes`, `ImageDecodeTimeoutSeconds`, `MaxRoomImageBytes`, `ImageThumbWidth`, `ImageThumbMaxHeight`, `ImageCacheSeconds`.
