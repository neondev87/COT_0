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
