# Estructura de COT_0.HostWinUI

Host de COT_0 en WinUI 3. Una sola ventana con un `Frame`; cada pantalla es una `Page`.
Las carpetas de primer nivel van en inglés; las de adentro, en español y con el nombre de lo que hacen.
Una clase por archivo.

```
COT_0.HostWinUI/
├── App.xaml                      Paleta, fuentes, medidas y estilos comunes (login, TextButton, SolidButton, VolumeSlider, filas de lista)
├── App.xaml.cs                   Arranque: arma los servicios, crea la ventana, arranca la música y levanta la web
├── MainWindow.xaml               Única ventana: CapaFondo (fondo elegido) y encima el Frame (RootFrame)
├── appsettings.json              Ventana, archivos JSON, seguridad, validación, preguntas secretas, reloj, Audio (música y portadas),
│                                 Background (fondos), Manual (videos, sonidos y tiempos de Lain), Links y la config de la web/túnel
├── .gitignore                    Vuelve a incluir Services/Musica (la regla musica/ de la raíz la tapaba) e ignora fondos/gifs/manual sueltos
├── app.manifest                  Nitidez en pantallas con escala (DPI)
├── Assets/
│   ├── Fuentes/                  TTF del diseño (Cinzel, Barlow, Barlow Condensed 200/300/500/700/800 itálica, Inter, Redaction 50 de la firma)
│   └── Imagenes/                 Imágenes fijas de la interfaz (fondo ASCII del login, avatar de la firma)
├── Models/                       Clases simples que se guardan en JSON o viajan entre capas (sin lógica)
│   ├── HostUser.cs               Cuenta del host: hash + salt de la contraseña y de la respuesta secreta
│   ├── Visit.cs                  Nombre y hora de entrada de un invitado (visitas y sesiones)
│   ├── SesionInvitados.cs        Sesión numerada: inicio, fin e invitados que entraron
│   ├── HashedSecret.cs           Hash y salt en Base64
│   ├── SecurityQuestionOption.cs Clave y texto de una pregunta secreta
│   ├── CredentialError.cs        Por qué se rechazó lo que escribiste (cada valor tiene su Text.Error.*)
│   ├── Song.cs                   Canción de la biblioteca: ruta, título, duración y portada
│   ├── MusicError.cs             Carpeta sin encontrar, sin canciones o archivo ilegible (Text.Music.Error.*)
│   ├── RoomError.cs              Por qué no abrió la sala: cloudflared no encontrado, túnel lento o caído, puerto ocupado, servidor (Text.Room.Error.*)
│   ├── Fondo.cs                  Fondo del catálogo: archivo, tipo, tamaño y si se muestra como cuadro fijo
│   ├── FondoKind.cs              Imagen o video
│   ├── FondosGuardados.cs        Contenido de fondos.json: catálogo y fondo elegido
│   ├── FondoError.cs             Carpeta sin encontrar, vacía, archivo ilegible o fondos.json dañado (Text.Backgrounds.Error.*)
│   └── ConfirmTexts.cs           Textos del diálogo sí/no
├── Services/
│   ├── Almacenamiento/           JsonStore: lee y guarda cualquier clase en %LocalAppData%\COT_0\*.json (async)
│   ├── Configuracion/            AppConfig + un record por sección de appsettings.json (Window, Storage, Security, Validation, Recovery, Ui, Paths,
│   │                             Audio, Background, Manual, Links, Web, Tunnel, Qr)
│   ├── Dialogos/                 ConfirmDialogService: abre el diálogo sí/no sobre la ventana
│   ├── Fondos/                   FondoCatalog (lee la carpeta; antes BackgroundCatalog del WPF), FondoStore (fondos.json), FondoService (catálogo,
│   │                             elegido y último error), FondoImages (imagen o cuadro de video decodificado chico) y FondoPlayer (único MediaPlayer)
│   ├── Manual/                   ManualAssets (rutas de Lain, async; copiado del WPF), ManualSound (aviso + estática), ManualVideo (un MediaPlayer
│   │                             para Lain y la despedida) y ManualTimeline (fases de cada paso con Manual:CueSeconds)
│   ├── Musica/                   MusicLibrary y CoverLibrary (copiadas del WPF, sin tipos de WPF), SongCatalog (duración y portada, async)
│   │                             y MusicService (Windows.Media.Playback.MediaPlayer: play/pausa, anterior/siguiente, volumen, silencio)
│   ├── Sala/                     WebServerHost (Kestrel con COT_0.Web), RoomSession (abrir/cerrar sala, vigilantes y apagado),
│   │                             QrService (PNG de QRCoder → BitmapImage), RoomOpenErrors (error → RoomError + texto) y GuestWatcher (base: detecta quién entra)
│   ├── Sesiones/                 SesionService (sesiones.json) y SesionWatcher (abre/cierra la sesión con la sala y anota invitados)
│   ├── Textos/                   UiText: busca un texto de Textos.xaml desde C# (Get, o Find si es opcional)
│   ├── Tiempo/                   ClockService (tick de cada segundo) y UiThread (pasa eventos al hilo de la UI)
│   ├── Tunel/                    CloudflareTunnelService y KillOnCloseJob (cloudflared muere con la app)
│   ├── Usuarios/                 UserService (registro/login), PasswordRecoveryService (¿olvidaste tu contraseña?),
│   │                             HostAccounts (usuarios.json) y SecretHasher (PBKDF2 + SHA-256 con salt)
│   ├── Validaciones/             CredentialValidator (solo letras y números) y SecurityAnswerValidator (normaliza la respuesta)
│   ├── Ventana/                  WindowSetup (tamaño y barra de título), Navigator (cambia de página, avisa PageChanged y regresa a sala o dashboard)
│   │                             y CloseGuard (cerrar la ventana con sala activa pregunta; luego apaga sala, túnel y servidor)
│   └── Visitas/                  VisitService (visitas.json) y VisitWatcher (anota a cada invitado que entra)
└── Views/
    ├── Componentes/              Piezas reutilizables
    │   ├── ConfirmacionDialogo   ContentDialog sí/no
    │   ├── ListaConectados       Invitados conectados (ListView)
    │   ├── FormularioRecuperacion  Pregunta secreta → contraseña nueva, dentro del login
    │   ├── TarjetaMenu.cs        Tarjeta inclinada del menú (Button con número, subtítulo e icono)
    │   ├── PuntoPulso            Punto rojo con pulso (sala activa)
    │   ├── EncabezadoPantalla    ← INICIO | TÍTULO de Sala, Emotes, Música, Fondos y Manual (clic o Esc regresa; ← SALA si viniste de la sala;
    │   │                         en el manual HandlesEscape="False" porque ahí Esc abre la despedida)
    │   ├── CapaFondo             Fondo detrás del Frame: degradado, imagen/cuadro fijo o video, velo y viñeta (solo en Dashboard, Sala y Fondos)
    │   ├── AvisoLain             Notificación de Lain estilo celular, con transición de opacidad
    │   ├── BotonManual.cs        Botón del manual: atajo con tecla dibujada (ENTER, →, ←, ESC) o botón redondeado
    │   ├── PanelEmotes           Set de 7TV, reportes y emotes quitados (+ EmoteStatusBrushes.cs: color del estado)
    │   ├── ControlVolumen        Nota (silencio), barra de volumen y número; lo usan la firma y MusicaPage
    │   ├── Firma                 Avatar + Neondev (abre el repo) y la música que suena, en el login
    │   ├── MusicGlyphs.cs        Iconos de play/pausa, nota/silencio y la bocina del manual según el estado
    │   └── AvatarTones.cs        Tono del avatar de cada fila
    ├── Estilos/
    │   ├── Textos.xaml           Todos los textos visibles, en español de México
    │   ├── Dashboard.xaml        Reloj, fecha, estado de la sala y botón sin aspecto (lo mezcla DashboardPage)
    │   ├── Sala.xaml             Medidas del QR, fuente mono, textos del contador, EN VIVO y la nota de CERRAR SALA (lo mezcla SalaPage)
    │   ├── Manual.xaml           Paleta de Lain, fondo azul noche, barrido, textos y estilos de BotonManual (lo mezcla ManualPage)
    │   └── TarjetaMenu.xaml      Plantilla de TarjetaMenu con sus estados (normal, mouse encima, apagada)
    ├── ModelosDeVista/           ViewModels: Login (+ LoginStep, FormViewModel), Recovery, SecurityQuestion,
    │                             Dashboard, Sala, ConnectedUsers, UserRow, Emotes + EmoteRow,
    │                             MusicControls (base) → Musica y Firma, SongRow, CapaFondo, Fondos + FondoRow,
    │                             Manual + ManualStep + ManualNotice
    └── Pantallas/                Páginas del Frame: Login, Dashboard, Sala, Emotes, Musica, Fondos, Manual
```

Los diccionarios de `Views/Estilos` que usan pinceles o fuentes de `App.xaml` se mezclan en los recursos de la página, no en `App.xaml`: un diccionario mezclado dentro de `App.xaml` no puede ver las claves del propio `App.xaml`.

## Archivos que guarda la app

En `%LocalAppData%\COT_0\`:

| Archivo | Qué guarda |
|---|---|
| `usuarios.json` | Cuenta del host (una sola): usuario, hash + salt de la contraseña, clave de la pregunta secreta y hash + salt de la respuesta, fecha. Nunca la contraseña ni la respuesta. |
| `visitas.json` | Una línea por cada invitado que entra a la sala, con `EnteredAt`. Queda aunque se cierre el túnel. |
| `fondos.json` | Catálogo de la última lectura de la carpeta de fondos (`FileName`, `Kind`, `SizeBytes`, `UseStill`) y `SelectedFile`: `null` = nunca elegiste (usa `Background:DefaultFile`), `""` = sin fondo. |
| `sesiones.json` | Una entrada por cada vez que se abre la sala: `Number` (1, 2, 3…), `StartedAt`, `EndedAt` y `Guests` (nombre + hora de entrada). El host no cuenta. |

## Login y recuperación

- `LoginStep` decide qué se ve: `SignIn` (ENTRAR), `Register` (REGISTRARME + pregunta secreta), `Recover` (¿Olvidaste tu contraseña?) y `SetupQuestion` (cuenta vieja sin pregunta: se pide después de ENTRAR).
- La respuesta se normaliza antes de hashear y al comparar: se recorta, se quitan todos los espacios y pasa a minúsculas (`SecurityAnswerValidator.Normalize`).
- Las preguntas salen de `Recovery:QuestionKeys` (claves) y su texto de `Textos.xaml`. En `usuarios.json` se guarda la clave, no el texto.

## Sala: abrir y cerrar

`SalaPage` llama a `SalaViewModel.EnsureOpenAsync()` al cargar; si la sala ya está abierta (por ejemplo, al regresar de Emotes o Música) no hace nada.

| Paso | Abrir (`RoomSession.OpenAsync`) | Cerrar (`RoomSession.CloseAsync`) |
|---|---|---|
| 1 | `WebServerHost.StartAsync()` (reusa el arranque de `App`) | `IRoomService.CloseAsync()`: saca a todos y vacía el chat |
| 2 | Crea `VisitWatcher` y `SesionWatcher` | `SesionService.EndAsync()`: hora de fin en `sesiones.json` |
| 3 | `cloudflared tunnel --url` y espera la URL pública | Suelta (`Dispose`) los dos vigilantes |
| 4 | `IRoomService.Open()` → `SesionWatcher` abre la sesión N+1 | Detiene cloudflared |

- Si abrir falla, se sueltan los vigilantes y el error sale en un `InfoBar` con REINTENTAR (`RoomOpenErrors`: cloudflared no encontrado, túnel que no da URL a tiempo, túnel caído, puerto ocupado, servidor).
- Kestrel no se apaga al cerrar la sala: `IChatService`/`IRoomService` salen del contenedor de la web y los ViewModels (Singleton) guardan esas instancias. Se apaga al cerrar la ventana (`CloseGuard` → `RoomSession.ShutdownAsync`). Con la sala cerrada `/join` no deja entrar y sin túnel nadie de fuera llega.
- SALIR del dashboard y cerrar la ventana con sala activa usan `RoomSession.ConfirmLeaveAsync()`. CERRAR SALA no pregunta (es el botón explícito).
- Si la app muere de golpe con la sala abierta, esa sesión se queda con `EndedAt = null`.

## Música, emotes y firma

- `MusicService` es una sola instancia (Singleton): la firma del login (`FirmaViewModel`) y `MusicaPage` (`MusicaViewModel`) heredan de `MusicControlsViewModel` y leen el mismo reproductor, así que van sincronizadas.
- La música vive fuera del repo: `Paths:MediaRoot` + `Audio:MusicFolder` (canciones) y `Audio:CoversFolder` (portadas). La app no crea la carpeta; si falta, `MusicaPage` lo avisa con un InfoBar.
- `EmotesViewModel` usa `IRoomEmotes` de Core tal cual, sacado del contenedor de la web (misma instancia que los chats de los invitados). En el WPF los emotes eran un bloque de la sala; en WinUI son su propia pantalla.

## Fondos

- Carpeta fuera del repo: `Paths:MediaRoot` + `Background:Folder` (imágenes `Background:ImageExtensions`, videos `Background:VideoExtensions`). La app no la crea.
- `CapaFondo` vive en `MainWindow`, detrás del `Frame`, y es la única: Dashboard, Sala y Fondos son páginas transparentes. En las demás páginas se oculta y el video se pausa. La carpeta se lee la primera vez que entras al dashboard (el login no la toca).
- Un solo `MediaPlayer` (`FondoPlayer`) en bucle y en silencio; cambiar de fondo solo cambia su `Source`.
- Videos de más de `Background:HeavyVideoMegabytes` (120 MB) arrancan como **cuadro fijo** (miniatura de Windows a `Background:StillSize`). En FONDOS, "Usar cuadro fijo" lo cambia por video y se guarda en `fondos.json`.
- Si un video no abre, cae solo a su cuadro fijo; el aviso queda en `FondoService` y lo muestra el InfoBar de FONDOS. Abrir FONDOS vuelve a intentar los archivos que fallaron.
- Velo negro: `Background:VeilOpacity` con fondo elegido, `Background:PickerVeilOpacity` en FONDOS (reemplaza el desenfoque).

## Manual de Lain

- Archivos fuera del repo (`Manual:*`, relativos a `Paths:MediaRoot`): `gifs\lain_tele_web.mp4`, `gifs\lain_despedida.mp4`, `manual\notificacion.mp3`, `manual\estatica.wav` y, opcional, una captura por paso en `manual\pasos\paso1.png`…`paso6.png`.
- Al abrir: `ManualAssets.RefreshAsync()` revisa qué existe; lo que falte sale en un InfoBar y el manual sigue sin eso.
- Pasos: textos en `Textos.xaml` (`Text.Manual.StepN.Title/Text/NoticeM`); cuántos avisos tiene cada uno, en `Manual:NoticeCounts`. El paso 1 no tiene título (`UiText.Find` regresa `null`).
- Fases (igual que el WPF): la fase 2i+1 muestra el aviso i y la 2i+2 lo quita; la última se queda. Los tiempos salen de `Manual:CueSeconds`.
- Atajos: ENTER siguiente (después del último, despedida; en la despedida, cerrar), → / ← cambian de paso, ESC despedida y luego cerrar. ← INICIO regresa al dashboard en cualquier momento.
- La despedida usa el mismo video y el mismo reproductor: un `StateTrigger` lo mueve al centro con menos opacidad.
