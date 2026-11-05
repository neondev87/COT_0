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

Referencias: `Host → Web, Core` y `Web → Core`. Toda la configuración está en `COT_0.Host/appsettings.json`.

## 3. Instalarlo en otra computadora

**Requisitos**

- Windows 10 u 11.
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
- cloudflared: `winget install Cloudflare.cloudflared` (o pon su ruta en `Tunnel:CloudflaredPath`).

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

Si falta alguna carpeta, la app funciona sin esa parte (sin música, sin fondo, manual sin sonido).

**`Paths:MediaRoot`**

Es la única ruta absoluta de la configuración. En `COT_0.Host/appsettings.json` cámbiala por la de tu `COT_0_P`:

```json
"Paths": { "MediaRoot": "C:\\Users\\TU_USUARIO\\Documents\\COT_0_P" }
```

Las demás rutas (`fondos`, `musica`, `gifs\\lain_tele_web.mp4`…) son relativas a ella.

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

Inicia sesión, toca **GENERAR QR** y comparte el código. Para probar solo la web, sin host: `dotnet run --project COT_0.Web` (la sala se abre sola en `http://localhost:5050`).

## 4. Funciones principales

- **Chat**: mensajes en vivo agrupados por autor, aviso de "escribiendo…", avisos de entrada y salida, y edición del perfil desde el chat. El host ve a los conectados, puede expulsar a alguien y cerrar la sala (todos salen).
- **Imágenes**: se reducen en el navegador antes de subirse, con vista previa. El servidor valida el formato y el tamaño, y hay un tope total por sala.
- **Notas de voz**: se graban desde el celular. Si llegan en WebM/Opus se convierten a WAV para que suenen también en iPhone.
- **Emotes de 7TV**: el host elige el set (ID, URL o `global`) en el panel EMOTES. Los invitados los usan desde el picker o escribiendo `:`, y pueden reportar uno; el host decide si lo quita, lo ignora o lo restaura.
- **Música y portadas**: suena la música de `musica\` en orden y en loop, al 25 % de volumen al iniciar. El selector de canciones (estilo rofi) muestra cada canción con su portada; si no tiene, usa otra disponible. Si la carpeta está vacía, "Subir música" copia ahí los archivos que elijas.
- **Fondos**: "Cambiar fondo" muestra las miniaturas de `fondos\`. El fondo elegido se ve en el dashboard y en la sala; los videos se pausan si minimizas la ventana.
- **Manual de Lain**: guía paso a paso en el Host, con Lain en video, avisos tipo notificación y estática de TV. Los invitados tienen su propio manual en `/manual` (enlace en `/join`).
