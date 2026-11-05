# COT_0

## 1. Qué es y cómo funciona

COT_0 es un chat grupal para el salón. El host abre la app en su PC, inicia sesión y toca **GENERAR QR**. En ese momento:

1. La app ya tiene corriendo un servidor web interno (Blazor + SignalR) en `http://localhost:5050`.
2. Abre un túnel de Cloudflare (`cloudflared`) que le da una URL pública `https://xxxx.trycloudflare.com`.
3. Abre la sala con un código corto y muestra un QR que apunta a `{URL pública}/join`.

Los invitados escanean el QR con el celular, eligen nombre, foto y escudo de facultad, y entran al chat en tiempo real. Todo vive en memoria: al cerrar la sala se borran usuarios, mensajes, notas de voz e imágenes. No hay base de datos.

## 2. Estructura de la solución

| Proyecto | Qué hace |
|---|---|
| `COT_0.Core` | Modelos (`User`, `ChatMessage`, `RoomEmote`…) e interfaces compartidas (`IChatService`, `IRoomService`, `IRoomEmotes`, `ITunnelService`, `IAuthService`). No depende de nadie. |
| `COT_0.Web` | Blazor Server + SignalR: páginas `/join`, `/chat` y `/manual`, el hub `/chathub`, el chat en memoria, voz, imágenes y emotes de 7TV. También corre sola con `dotnet run` para pruebas. |
| `COT_0.Host` | App de escritorio WPF: login del host, dashboard, sala (QR, conectados, emotes), música, fondos y manual de Lain. Levanta la web dentro de su propio proceso y maneja `cloudflared`. |

Referencias: `Host → Web, Core` y `Web → Core`. La configuración está en `COT_0.Host/appsettings.json` (se copia junto al exe al compilar). `COT_0.Web` no tiene `appsettings.json` propio: sola, usa los valores por defecto de sus clases de opciones y el puerto de `Properties/launchSettings.json`.

Paquetes NuGet (se bajan solos con `dotnet build`): QRCoder, CommunityToolkit.Mvvm y TagLibSharp en el Host; Concentus y SignalR.Client en Web.

## 3. Instalarlo en otra computadora

**Requisitos**

- Windows 10 u 11 (el Host es WPF, `net8.0-windows`).
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0): todos los proyectos apuntan a .NET 8. Un SDK más nuevo (9 o 10) también compila, pero hace falta el runtime de .NET 8 (viene con el SDK 8). No hay `global.json`.
- cloudflared: `winget install Cloudflare.cloudflared` (o pon su ruta en `Tunnel:CloudflaredPath`).
- **ffmpeg no hace falta**: las notas de voz se decodifican en C# con Concentus.

**Carpetas de multimedia (fuera del repo)**

Crea una carpeta `COT_0_P` con esta estructura (el repo puede quedar adentro, en `COT_0_P\COT_0`):

```
COT_0_P\
├─ fondos\     videos o imágenes de fondo (el inicial es Background:DefaultFile)
├─ musica\     canciones (.mp3, .wav, .m4a…)
├─ portadas\   portadas con el mismo nombre que la canción (.jpg, .png, .webp)
├─ gifs\       lain_tele_web.mp4 y lain_despedida.mp4 (manual de Lain)
└─ manual\     notificacion.mp3 y estatica.wav (sonidos del manual)
```

Si falta alguna carpeta, la app funciona sin esa parte (sin música, sin fondo, manual sin sonido). Los sonidos del manual de invitados (`/manual`) salen de la misma carpeta `manual\` (`GuestManual:AssetsFolder`).

**`Paths:MediaRoot`**

Es la única ruta absoluta de la configuración. En `COT_0.Host/appsettings.json` cámbiala por la de tu `COT_0_P`:

```json
"Paths": { "MediaRoot": "C:\\Users\\TU_USUARIO\\Documents\\COT_0_P" }
```

Las demás rutas (`fondos`, `musica`, `gifs\lain_tele_web.mp4`…) son relativas a ella.

**Usuario y contraseña del host (user-secrets)**

No van en `appsettings.json`. Desde la carpeta del repo:

```
dotnet user-secrets set "Host:User" "tu_usuario" --project COT_0.Host
dotnet user-secrets set "Host:Password" "tu_contraseña" --project COT_0.Host
```

**Arrancar**

```
dotnet run --project COT_0.Host
```

Inicia sesión, toca **GENERAR QR** y comparte el código. Para probar solo la web, sin host: `dotnet run --project COT_0.Web` (la sala se abre sola en `http://localhost:5050`; sin `Paths:MediaRoot`, el manual de invitados queda sin sonido).

## 4. Funciones principales

- **Chat**: mensajes en vivo agrupados por autor, aviso de "escribiendo…", avisos de entrada y salida, y edición del perfil desde el chat. El host ve a los conectados, puede expulsar a alguien y cerrar la sala (todos salen).
- **Imágenes**: se reducen en el navegador antes de subirse, con vista previa. El servidor valida el formato y el tamaño, y hay un tope total por sala.
- **Notas de voz**: se graban desde el celular. Si llegan en WebM/Opus se convierten a WAV en el servidor (Concentus, sin ffmpeg) para que suenen también en iPhone. Se sirven con soporte de `Range`, que Safari exige.
- **Emotes de 7TV**: el host elige el set (ID, URL o `global`) en el panel EMOTES. Los invitados los usan desde el picker o escribiendo `:`, y pueden reportar uno; el host decide si lo quita, lo ignora o lo restaura.
- **Música y portadas**: suena la música de `musica\` en orden y en loop, al 25 % de volumen al iniciar. El selector de canciones (estilo rofi) muestra cada canción con su portada; si no tiene, usa otra disponible. Si la carpeta está vacía, "Subir música" copia ahí los archivos que elijas.
- **Fondos**: "Cambiar fondo" muestra las miniaturas de `fondos\`. El fondo elegido se ve en el dashboard y en la sala; los videos se pausan si minimizas la ventana.
- **Manual de Lain**: guía paso a paso en el Host, con Lain en video, avisos tipo notificación y estática de TV. Los invitados tienen su propio manual en `/manual` (enlace en `/join`).

## 5. Problemas comunes

- **El túnel no levanta.** "No se encontró cloudflared": instálalo con `winget install Cloudflare.cloudflared`, abre una terminal nueva para que tome el PATH, o pon la ruta completa del `.exe` en `Tunnel:CloudflaredPath`. "No entregó la URL en N segundos": revisa el internet o que un firewall/antivirus no bloquee `cloudflared`; puedes subir `Tunnel:StartTimeoutSeconds`. Los quick tunnels cambian de URL cada vez: genera un QR nuevo en cada sesión.
- **Rutas que no se encuentran (sin música, sin fondo, manual mudo).** `Paths:MediaRoot` apunta a otra PC: cámbiala a tu `COT_0_P`. Los nombres de carpeta y archivo deben coincidir con `appsettings.json` (`fondos`, `musica`, `gifs\lain_tele_web.mp4`, `Background:DefaultFile`…). Arranca con `dotnet run --project COT_0.Host` o desde `bin\`: `appsettings.json` y `wwwroot` tienen que estar junto al exe.
- **"Falta Web:Url" o el puerto 5050 ocupado.** Cierra otra instancia de COT_0 o de `COT_0.Web` (`dotnet run`) que siga abierta, o cambia `Web:Url`.
- **7TV lento.** La API de 7TV a veces tarda; el panel muestra "CARGANDO…" y espera hasta `Chat:EmoteRequestTimeoutSeconds` (90 s). Si da "TARDÓ DEMASIADO", vuelve a aplicar el set. Con `global` carga el set global de 7TV.
- **Audio en iPhone.** Safari solo deja sonar audio después de un toque: el manual de invitados suena al tocar EMPEZAR y las notas de voz al tocar reproducir. El micrófono pide permiso al primer toque y solo funciona por `https` (la URL del túnel), no por `http://IP-local`.
- **Celular que se desconecta.** Al bloquear la pantalla el celular corta la conexión; al volver, la página reconecta o recarga sola y conserva el perfil.
