# COT_0

Chat grupal para el salón, hecho en C# (.NET 8). Proyecto de la materia de C#.

## 1. Qué es y cómo funciona

El host abre la app en su PC, inicia sesión y toca **GENERAR QR**. En ese momento:

1. La app ya tiene corriendo un servidor web interno (Blazor Server + SignalR) en `http://localhost:5050`.
2. Abre un túnel de Cloudflare (`cloudflared`) que le da una URL pública `https://xxxx.trycloudflare.com`.
3. Abre la sala con un código corto y muestra un QR que apunta a `{URL pública}/join`.

Los invitados escanean el QR con el celular, eligen nombre, foto y escudo de facultad, y entran al chat en tiempo real. Todo vive en memoria: al cerrar la sala se borran usuarios, mensajes, notas de voz e imágenes. No hay base de datos; lo único que se guarda en disco son archivos JSON del host (usuarios, visitas, sesiones y fondo elegido).

## 2. Proyectos

| Proyecto | Qué hace |
|---|---|
| `COT_0.Core` | Modelos (`User`, `ChatMessage`, `RoomEmote`, `Fondo`…) e interfaces compartidas (`IChatService`, `IRoomService`, `IRoomEmotes`, `IFlagService`, `ITunnelService`, `IBackgroundCatalog`). No depende de nadie. |
| `COT_0.Web` | Blazor Server + SignalR: páginas `/join`, `/chat` y `/manual`, el hub `/chathub`, el chat en memoria, voz, imágenes y emotes de 7TV. También corre sola para pruebas. |
| `COT_0.Host` | App de escritorio **WinUI 3**: login (hasta 20 usuarios), dashboard, sala (QR, conectados), emotes, música, fondos y manual de Lain. Levanta la web dentro de su propio proceso y maneja `cloudflared`. |

Referencias: `Host → Web, Core` y `Web → Core`. El árbol completo está en `docs/ESTRUCTURA.md`.

La configuración está en `COT_0.Host/appsettings.json` (se copia junto al `.exe` al compilar). `COT_0.Web` no tiene `appsettings.json` propio: sola, usa los valores por defecto de sus clases de opciones y el puerto de `Properties/launchSettings.json`.

Paquetes NuGet (se bajan solos con `dotnet build`): Microsoft.WindowsAppSDK, CommunityToolkit.Mvvm y QRCoder en el Host; Concentus y SignalR.Client en Web.

## 3. Requisitos

- Windows 10 (1809 o más nuevo) u 11, de 64 bits. El Host es WinUI 3 (`net8.0-windows10.0.19041.0`, x64).
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). Un SDK más nuevo también compila, pero hace falta el runtime de .NET 8 (viene con el SDK 8). No hay `global.json`.
- No hay que instalar el Windows App SDK aparte: el proyecto lo lleva dentro de la carpeta de salida (`WindowsAppSDKSelfContained`, sin MSIX).
- cloudflared: `winget install Cloudflare.cloudflared` (o pon su ruta en `Tunnel:CloudflaredPath`).
- ffmpeg **no hace falta**. Si está en el `PATH`, la pantalla FONDOS lo usa para sacar el cuadro fijo de los videos; si no, usa los métodos de Windows.

## 4. Compilar y correr

Desde la carpeta del repo:

```
dotnet build COT_0.sln
dotnet run --project COT_0.Host
```

La app siempre abre en **ACCESO DEL HOST** (ENTRAR). La primera vez no hay usuarios: usa el enlace **Registrar usuario** (junto a "¿Olvidaste tu contraseña?") y escribe usuario y contraseña dos veces (usuario solo letras con la primera en mayúscula; contraseña de 8 a 20 con al menos un número y un carácter especial; todo hasta `Inputs:MaxCaracteres` = 20). Caben hasta `Usuarios:Maximo` = 20 usuarios en `%LocalAppData%\COT_0\usuarios.json`, con la contraseña y la respuesta secreta hasheadas (PBKDF2 + SHA-256 con salt). La primera vez que cada usuario entra configura su pregunta secreta; si olvida la contraseña, la recupera escribiendo su usuario y contestando esa pregunta. Todos los usuarios entran a lo mismo.

Si ya tenías la cuenta única de antes, se lee sola como el primer usuario: no hay que hacer nada.

Para probar solo la web, sin host: `dotnet run --project COT_0.Web` (la sala se abre sola en `http://localhost:5050`).

## 5. Carpetas de multimedia (fuera del repo)

Música, fondos, portadas, videos de Lain y sonidos del manual **no van en el repo** (están en `.gitignore`). Crea una carpeta `COT_0_P` con esta estructura (el repo puede quedar adentro, en `COT_0_P\COT_0`):

```
COT_0_P\
├─ fondos\     videos o imágenes de fondo (el inicial es Background:DefaultFile)
├─ musica\     canciones (.mp3, .wav, .m4a, .aac, .wma); empieza por Audio:DefaultFile
├─ portadas\   portadas con el mismo nombre que la canción (.jpg, .png, .webp)
├─ gifs\       lain_tele_web.mp4 y lain_despedida.mp4 (manual de Lain del host)
└─ manual\     notificacion.mp3 y estatica.wav (manual del host); lain.mp4 y lain-poster.jpg (manual de invitados);
               opcional: pasos\paso1.png…paso6.png (capturas del manual del host)
```

Si falta alguna carpeta, la app funciona sin esa parte (sin música, sin fondo, manual sin sonido) y lo avisa en la pantalla correspondiente.

`Paths:MediaRoot` es la única ruta absoluta de la configuración. En `COT_0.Host/appsettings.json` cámbiala por la de tu `COT_0_P`:

```json
"Paths": { "MediaRoot": "C:\\Users\\TU_USUARIO\\Documents\\COT_0_P" }
```

Las demás rutas (`fondos`, `musica`, `gifs\lain_tele_web.mp4`…) son relativas a ella.

`lain.mp4` del manual de invitados es `gifs\lain.gif` convertido una sola vez (H.264, sin audio): `ffmpeg -i gifs\lain.gif -an -c:v libx264 -pix_fmt yuv420p -crf 26 -movflags +faststart -vf "scale=trunc(iw/2)*2:trunc(ih/2)*2" manual\lain.mp4` y el poster con `ffmpeg -i gifs\lain.gif -frames:v 1 manual\lain-poster.jpg`.

## 6. Funciones principales

- **Chat**: mensajes en vivo agrupados por autor, aviso de "escribiendo…", avisos de entrada y salida, y edición del perfil desde el chat. El host ve a los conectados, puede expulsar a alguien y cerrar la sala (todos salen).
- **Imágenes**: se reducen en el navegador antes de subirse, con vista previa. El servidor valida el formato y el tamaño, y hay un tope total por sala.
- **Notas de voz**: se graban desde el celular. Si llegan en WebM/Opus se convierten a WAV en el servidor (Concentus, sin ffmpeg) para que suenen también en iPhone.
- **Tienda de emotes** (pantalla EMOTES, 09/11): pestañas USUARIOS | TIENDA | TICKETS. La TIENDA explora todo 7TV por páginas con buscador; cada usuario del login compra (gratis) hasta `Store:DailyLimit` = 10 emotes cada `Store:CooldownHours` = 24 horas, guardados en `%LocalAppData%\COT_0\compras.json` con su ticket (PDF opcional en el Escritorio, carpeta `COT_0_tickets`). Al abrir la sala, los comprados por quien hizo ENTRAR entran solos al chat.
- **Emotes de 7TV**: el host elige el set (ID, URL o `global`) en la pantalla EMOTES. Los invitados los usan desde el picker o escribiendo `:`, y pueden reportar uno; el host decide si lo quita, lo ignora o lo restaura.
- **Música y portadas**: suena la música de `musica\` en orden y en loop; la app abre en silencio (volumen 0) y suena al subir el volumen. La pantalla MÚSICA muestra cada canción con su portada. La firma de Neondev (login, dashboard y sala) trae la barra de volumen.
- **Fondos**: la pantalla FONDOS muestra las miniaturas de `fondos\`. El fondo elegido se ve en el dashboard y en la sala; los videos pesados arrancan como cuadro fijo.
- **Manual de Lain**: guía paso a paso en el Host, con Lain en video, avisos tipo notificación y vistas de ejemplo con resaltados. Los invitados tienen su propio manual en `/manual` (enlace en `/join`).
- **Visitas y sesiones**: cada invitado que entra queda en `visitas.json`, y cada vez que se abre la sala se numera una sesión en `sesiones.json`.

## 7. Problemas comunes

- **El túnel no levanta.** "No se encontró cloudflared": instálalo con `winget install Cloudflare.cloudflared`, abre una terminal nueva para que tome el PATH, o pon la ruta completa del `.exe` en `Tunnel:CloudflaredPath`. Si tarda: revisa el internet o que un firewall/antivirus no bloquee `cloudflared`; puedes subir `Tunnel:StartTimeoutSeconds`. Los quick tunnels cambian de URL cada vez: el QR se genera de nuevo en cada sesión.
- **Rutas que no se encuentran (sin música, sin fondo, manual mudo).** `Paths:MediaRoot` apunta a otra PC: cámbiala a tu `COT_0_P`. Los nombres de carpeta y archivo deben coincidir con `appsettings.json`. Arranca con `dotnet run --project COT_0.Host` o desde `bin\`: `appsettings.json` y `wwwroot` tienen que estar junto al `.exe`.
- **Puerto 5050 ocupado.** Cierra otra instancia de COT_0 o de `COT_0.Web` (`dotnet run`) que siga abierta, o cambia `Web:Url`.
- **7TV lento.** La API de 7TV a veces tarda; el panel muestra "CARGANDO…" y espera hasta `Chat:EmoteRequestTimeoutSeconds`. Si da "TARDÓ DEMASIADO", vuelve a aplicar el set.
- **Audio en iPhone.** Safari solo deja sonar audio después de un toque. El micrófono solo funciona por `https` (la URL del túnel), no por `http://IP-local`.
- **Celular que se desconecta.** Al bloquear la pantalla el celular corta la conexión; al volver, la página reconecta o recarga sola y conserva el perfil.
- **Empezar de cero.** Borra `%LocalAppData%\COT_0\` (usuarios, visitas, sesiones, fondo elegido y caché de cuadros).

## 8. Documentación

En `docs/`: `ESTRUCTURA.md` (árbol comentado), `MAPA_CARPETAS.md` (convención de carpetas), `CAMBIOS_VISUALES.md`, `PRUEBAS_MANUALES.md`, `CUMPLIMIENTO.md`, `PENDIENTES_LIMPIEZA.md` y `REPORTE_CIERRE.md`.
