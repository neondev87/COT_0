# Estructura de COT_0

La solución (`COT_0.sln`) tiene tres proyectos. Referencias: `Host → Web, Core` y `Web → Core`; Core no depende de nadie.

Reglas de carpetas: en cada proyecto, el primer nivel va en inglés (`Views`, `Models`, `Services`, `Assets`); adentro, subcarpetas en español con el nombre de lo que hacen. Una clase por archivo y un comentario `///` arriba de cada tipo. El detalle de qué se movió y qué se quedó está en `MAPA_CARPETAS.md`.

```
COT_0/
├── COT_0.sln                     Solución: Core, Web y Host
├── CONTRATO.md                   Acuerdo entre agentes: dueños de carpetas, interfaces de Core y pedidos de cambio
├── README.md                     Qué es, requisitos, cómo compilar y correr
├── .gitignore                    Build, y medios locales (/musica/, /fondos/, /portadas/, /gifs/, /manual/, audio y video sueltos)
├── docs/                         Esta documentación
├── COT_0.Core/                   Lo compartido: modelos e interfaces (sin dependencias)
├── COT_0.Web/                    Lo que ven los invitados: Blazor Server + SignalR
└── COT_0.Host/                   La app del host: WinUI 3
```

## COT_0.Core

```
COT_0.Core/
├── Models/
│   ├── User.cs                   Invitado conectado (nombre, foto, escudo), solo en memoria
│   ├── ChatMessage.cs            Mensaje del chat (texto, voz, imagen o aviso del sistema)
│   ├── MessageKind.cs            Tipo de mensaje
│   ├── FlagOption.cs             Escudo de facultad que puede elegir el invitado
│   ├── RoomEmote.cs              Emote de 7TV activo en la sala
│   ├── EmoteSources.cs           URLs de las imágenes de un emote
│   ├── EmoteReport.cs            Reporte de un invitado sobre un emote
│   ├── EmoteSetStatus.cs         Estado del set de 7TV (apagado, cargando, listo…)
│   ├── EmoteSetError.cs          Por qué falló el set
│   ├── Fondo.cs                  Fondo del catálogo: archivo, tipo, tamaño y si se muestra como cuadro fijo
│   └── FondoKind.cs              Imagen o video
└── Services/
    ├── IChatService.cs           Usuarios e historial del chat (lo implementa Web)
    ├── IRoomService.cs           Abrir/cerrar la sala y su código (lo implementa Web)
    ├── IRoomEmotes.cs            Set de 7TV, reportes y emotes quitados (lo implementa Web)
    ├── IFlagService.cs           Lista de escudos de facultad (lo implementa Web)
    ├── ITunnelService.cs         Túnel de cloudflared (lo implementa el Host)
    ├── IBackgroundCatalog.cs     Carpeta de fondos (lo implementa el Host)
    └── MediaPath.cs              Une Paths:MediaRoot con las rutas relativas de los medios
```

## COT_0.Web

```
COT_0.Web/
├── COT_0.Web.csproj              Blazor Server; los namespaces de Services se importan aquí una sola vez (<Using>)
├── Program.cs                    Arranque solo (dotnet run --project COT_0.Web): la sala abre sola
├── WebApp.cs                     Arma la app web; la usan Program.cs y el Host
├── App.razor                     Componente raíz (HTML base, CSS y scripts)
├── _Imports.razor                Usings de todos los .razor (tiene que estar en la raíz)
├── Properties/launchSettings.json  Puerto 5050 para dotnet run (ruta que pide .NET)
├── Models/                       Clases simples de la web: formulario de entrada, borradores, imágenes, voz, manual, ceniza
├── Services/
│   ├── Configuracion/            ChatOptions, VoiceOptions, ManualOptions y ManualSteps (secciones Chat, Voice y GuestManual)
│   ├── Sala/                     ChatHub (/chathub), ChatBroadcaster, ChatEvents, InMemoryChatService (IChatService),
│   │                             RoomService (IRoomService), PresenceTracker, ChatSession, ChatBrowser (interop), SystemNotice
│   │                             y ChatRegistration (registro en DI)
│   ├── Mensajes/                 MessageFormatter (texto → piezas con emotes y enlaces), MessageGrouping, MessageAuthor
│   │                             y ComposerEditor (cuadro de escritura)
│   ├── Perfil/                   ProfileService, GuestStore (localStorage cifrado), InMemoryAvatarStore, FlagService (IFlagService) y FlagSearch
│   ├── Imagenes/                 ImageService, ImageUploader, ImageSniffer (valida el formato), InMemoryImageStore e ImageNote
│   ├── Voz/                      VoiceService, VoiceRecorder, AudioSniffer, OpusWavTranscoder + WebmOpusFrames + WavWriter
│   │                             (WebM/Opus → WAV con Concentus), InMemoryVoiceStore, VoiceFormat y VoiceNote
│   ├── Emotes/                   RoomEmotes (IRoomEmotes), SevenTvClient + sus DTO (SevenTv*), EmoteSearch, EmoteList,
│   │                             EmoteActions, EmoteSetId, EmoteSetResult y EmoteRegistration
│   └── Manual/                   Manual del invitado (/manual): ManualTour, ManualDemo, ManualFocus, ManualTimeline y ManualRegistration
├── Views/
│   ├── Pantallas/                Páginas con ruta: Join (/join), Chat (/chat), Manual (/manual) y Error
│   ├── Plantillas/               MainLayout y Routes (enrutador)
│   └── Componentes/              Piezas del chat y del perfil (MessageList, ChatComposer, EmotePicker, VoiceBubble, ImageViewer,
│       │                         ProfileForm, FacultySheet, UserPanel, AshLayer…)
│       └── Manual/               Piezas del manual del invitado (LainTv, ManualStory, ManualNotices, DemoChat, MuteButton…)
└── wwwroot/                      Archivos estáticos (nombre que pide ASP.NET; el Host los copia junto al .exe)
    ├── css/                      Estilos y animaciones por pantalla (join, chat, emotes, voz, manual…)
    ├── fonts/                    Fuentes woff2 (Barlow, Barlow Condensed, Cinzel)
    ├── img/                      Fondo ASCII y escudos de facultad (img/flags)
    └── js/                       Interop mínimo que C# no puede hacer: chat.js (scroll), composer.js (cuadro editable),
                                  voice.js (micrófono), manual.js (sonidos y atajos) y reconnect.js (reconexión)
```

## COT_0.Host

Host en WinUI 3. Una sola ventana con un `Frame`; cada pantalla es una `Page`.

```
COT_0.Host/
├── COT_0.Host.csproj             WinUI 3 sin MSIX (WindowsAppSDKSelfContained); copia Assets, appsettings.json y wwwroot de la web
├── App.xaml                      Paleta, fuentes, medidas y estilos comunes (login, TextButton, SolidButton, VolumeSlider, filas de lista)
├── App.xaml.cs                   Arranque: arma los servicios, crea la ventana, arranca la música y levanta la web
├── MainWindow.xaml               Única ventana (pantalla completa): CapaFondo (fondo elegido), encima el Frame (RootFrame) y la ✕ arriba a la derecha
├── appsettings.json              Toda la configuración (ventana, archivos JSON, seguridad, validación, preguntas secretas, Ui, Paths,
│                                 Audio, Background, Manual + Manual:Demo, Branding, Tunnel, Qr, Web, Chat, GuestManual)
├── app.manifest                  Nitidez en pantallas con escala (DPI)
├── Assets/
│   ├── Fuentes/                  TTF del diseño (Cinzel, Barlow, Barlow Condensed 200/300/500/700/800 itálica, Inter, Redaction 50 de la firma)
│   └── Imagenes/                 Imágenes fijas de la interfaz (fondo ASCII del login, avatar de la firma)
├── Models/                       Clases simples que se guardan en JSON o viajan entre capas (sin lógica)
│   ├── HostUser.cs               Un usuario del login (hasta 5): hash + salt de la contraseña y de la respuesta secreta
│   ├── HashedSecret.cs           Hash y salt en Base64
│   ├── SecurityQuestionOption.cs Clave y texto de una pregunta secreta
│   ├── CredentialError.cs        Por qué se rechazó lo que escribiste, con nombre repetido y máximo de usuarios (cada valor tiene su Text.Error.*)
│   ├── Visit.cs                  Nombre y hora de entrada de un invitado (visitas y sesiones)
│   ├── SesionInvitados.cs        Sesión numerada: inicio, fin e invitados que entraron
│   ├── RoomError.cs              Por qué no abrió la sala: cloudflared no encontrado, túnel lento o caído, puerto ocupado, servidor
│   ├── Song.cs                   Canción de la biblioteca: ruta, título, duración y portada
│   ├── CoverArt.cs               Portada grande decodificada y su tamaño en pantalla
│   ├── MusicError.cs             Carpeta sin encontrar, sin canciones o archivo ilegible
│   ├── FondosGuardados.cs        Contenido de fondos.json: catálogo y fondo elegido
│   ├── FondoError.cs             Carpeta vacía o sin encontrar, imagen ilegible, fondos.json dañado, archivo bloqueado o que ya no está,
│   │                             video sin códec, que no decodifica o sin cuadro fijo
│   ├── MediaLogEntry.cs          Una línea de registro-medios.jsonl: hora, origen, archivo, ErrorCode (HRESULT) y detalle
│   ├── ManualDemoRow.cs          Fila ficticia de las vistas de ejemplo del manual (emote o canción)
│   └── ConfirmTexts.cs           Textos del diálogo sí/no
├── Services/
│   ├── Almacenamiento/           JsonStore: lee y guarda cualquier clase en %LocalAppData%\COT_0\*.json (async); AsyncFiles: FileStream con useAsync
│   ├── Cache/                    CacheFolders: %LocalAppData%\COT_0\cache\<nombre> y la llave (hash de ruta + tamaño + fecha)
│   ├── Registro/                 MediaLog: fallas de video, miniaturas y portadas en registro-medios.jsonl
│   ├── Configuracion/            AppConfig (LoadAsync de appsettings.json al arrancar) + AppSettings y un record por sección (Window, Storage, Security,
│   │                             Inputs, Usuarios, Recovery, Ui, Paths, Audio, Background, Manual, ManualDemo*, Branding, Tunnel, Qr, Web)
│   ├── Dialogos/                 ConfirmDialogService: abre el diálogo sí/no sobre la ventana
│   ├── Fondos/                   FondoCatalog (IBackgroundCatalog: lee la carpeta), FondoStore (fondos.json), FondoService (catálogo,
│   │                             elegido y último error), FondoImages (imagen o cuadro fijo decodificado), FondoPlayer (único MediaPlayer),
│   │                             FondoFrameCache (cuadro fijo en caché) con sus intentos FfmpegFrames → ShellThumbnailFrames →
│   │                             CompositionFrames (+ FrameFiles: guarda el JPEG), FondoFailures (HRESULT → FondoError) y FondoNames
│   ├── Manual/                   ManualAssets (qué archivos de Lain existen, async), ManualSound (aviso + estática),
│   │                             ManualVideo (un MediaPlayer para Lain y la despedida) y ManualTimeline (fases con Manual:CueSeconds)
│   ├── Musica/                   MusicLibrary (canciones de la carpeta), CoverLibrary (portadas), SongCatalog (duración y portada, async),
│   │                             MusicService (Windows.Media.Playback: play/pausa, anterior/siguiente, volumen, silencio
│   │                             del usuario y silencio temporal del Manual, IsHushed) y CoverImages
│   ├── Sala/                     WebServerHost (Kestrel con COT_0.Web), RoomSession (abrir/cerrar sala, vigilantes y apagado),
│   │                             QrService (PNG de QRCoder → BitmapImage), RoomOpenErrors (error → RoomError + texto) y GuestWatcher (base)
│   ├── Sesiones/                 SesionService (sesiones.json) y SesionWatcher (abre/cierra la sesión con la sala y anota invitados)
│   ├── Textos/                   UiText: busca un texto de Textos.xaml desde C#
│   ├── Tiempo/                   ClockService (tick de cada segundo) y UiThread (pasa eventos al hilo de la UI)
│   ├── Tunel/                    CloudflareTunnelService (ITunnelService) y KillOnCloseJob (cloudflared muere con la app)
│   ├── Usuarios/                 UserService (registrar hasta Usuarios:Maximo, ENTRAR, pregunta al entrar), PasswordRecoveryService
│   │                             (recuperación por usuario), HostAccounts (usuarios.json como lista; también lee la cuenta suelta) y SecretHasher (PBKDF2 + SHA-256 con salt)
│   ├── Validaciones/             InputRules (regla única: solo letras y números), InputLimits (Inputs:MaxCaracteres), CredentialValidator y SecurityAnswerValidator (normaliza la respuesta)
│   ├── Ventana/                  WindowSetup (pantalla completa), Navigator (cambia de página), CloseGuard (✕ y Alt+F4: pregunta antes de cerrar),
│   │                             AppShutdown (apagado en orden: reloj y reproductores → sala, túnel y Kestrel → ventana)
│   │                             y ShortcutHints (atajos de teclado sin letrero flotante)
│   └── Visitas/                  VisitService (visitas.json) y VisitWatcher (anota a cada invitado que entra)
└── Views/
    ├── Pantallas/                Páginas del Frame: LoginPage, DashboardPage, SalaPage, EmotesPage, MusicaPage, FondosPage, ManualPage
    ├── ModelosDeVista/           ViewModels: Login (+ LoginStep, FormViewModel), Register, Recovery, SecurityQuestion, Dashboard, Sala,
    │                             ConnectedUsers + UserRow, Emotes + EmoteRow, MusicControls (base) → Musica y Firma, SongRow,
    │                             CapaFondo, Fondos + FondoRow, LogoNeondev, Manual + ManualStep + ManualNotice + ManualDemo
    ├── Componentes/              Piezas reutilizables
    │   ├── CapaFondo             Fondo detrás del Frame: degradado, imagen/cuadro fijo o video, velo y viñeta (Dashboard, Sala, Fondos y Manual)
    │   ├── ConfirmacionDialogo   ContentDialog sí/no
    │   ├── BotonBocina.cs        Bocina de la firma: Button con cursor de mano
    │   ├── FiltroCampo.cs        Propiedad adjunta de LoginTextBox/LoginPasswordBox: quita espacios y símbolos al escribir o pegar
    │   ├── ControlVolumen        Nota (silencio), barra de volumen y número (MÚSICA)
    │   ├── EncabezadoPantalla    ← INICIO | TÍTULO de cada pantalla (clic o Esc regresa; ← SALA si viniste de la sala)
    │   ├── Firma                 LogoNeondev y debajo bocina + línea de volumen (login, dashboard y sala)
    │   ├── LogoNeondev           Avatar + "Neondev" con brillo de vidrio; clic abre Branding:RepoUrl
    │   ├── FormularioRecuperacion  Usuario → su pregunta secreta → contraseña nueva, dentro del login
    │   ├── FormularioRegistro    Registrar usuario: nombre, contraseña y repetirla, dentro del login
    │   ├── ListaConectados       Invitados conectados (ListView)
    │   ├── PanelEmotes           Set de 7TV, reportes y emotes quitados
    │   ├── PuntoPulso            Punto rojo con pulso (sala activa)
    │   ├── TarjetaMenu.cs        Tarjeta inclinada del menú del dashboard
    │   └── Manual/               Piezas del manual de Lain
    │       ├── AvisoLain         Notificación de Lain estilo celular
    │       ├── BotonManual.cs    Botón del manual con tecla dibujada (ENTER, →, ←, ESC)
    │       ├── ResaltadoManual   Borde rojo + halo que late sobre la vista de ejemplo
    │       ├── FondoManual, EstaticaManual  Fondo azul con barrido; estática y viñeta (solo XAML)
    │       ├── EncabezadoEjemplo ← INICIO | TÍTULO dibujado, sin botón
    │       ├── VistaEjemplo.cs   Base de las vistas de ejemplo (Step, Phase, Demo)
    │       ├── VistaEjemploManual  Escenario: crea la vista del paso a 1280×720 dentro de un Viewbox
    │       └── VistaEjemploLogin, VistaEjemploInicio, VistaEjemploSala, VistaEjemploEmotes, VistaEjemploMusica  Réplicas quietas por paso
    ├── Convertidores/            AvatarTones (tono del avatar), EmoteStatusBrushes (color del estado), MusicGlyphs (iconos de música)
    └── Estilos/
        ├── Textos.xaml           Todos los textos visibles, en español de México
        ├── Dashboard.xaml        Reloj, fecha, estado de la sala (lo mezcla DashboardPage)
        ├── Sala.xaml             Medidas del QR, fuente mono, contador, EN VIVO (lo mezcla SalaPage)
        ├── Manual.xaml           Paneles de vidrio, medidas y estilos de BotonManual (lo mezcla ManualPage)
        ├── ManualFondo.xaml      Azul del Manual, barrido, estática, sombra y orillas de Lain y de la despedida (ManualPage, FondoManual, EstaticaManual)
        ├── Volumen.xaml          Volumen minimalista: plantilla del Slider, punto y bocina (lo mezcla Firma)
        └── TarjetaMenu.xaml      Plantilla de TarjetaMenu y trazos de sus iconos
```

Los diccionarios de `Views/Estilos` que usan pinceles o fuentes de `App.xaml` se mezclan en los recursos de la página, no en `App.xaml`: un diccionario mezclado dentro de `App.xaml` no puede ver las claves del propio `App.xaml`.

## Archivos que guarda la app

En `%LocalAppData%\COT_0\`:

| Archivo | Qué guarda |
|---|---|
| `usuarios.json` | Lista de usuarios del login (hasta `Usuarios:Maximo` = 20). Por cada uno: usuario, hash + salt de la contraseña, clave de la pregunta secreta y hash + salt de la respuesta, fecha. Nunca la contraseña ni la respuesta. |
| `visitas.json` | Una línea por cada invitado que entra a la sala, con `EnteredAt`. Queda aunque se cierre el túnel. |
| `fondos.json` | Catálogo de la última lectura de la carpeta de fondos (`FileName`, `Kind`, `SizeBytes`, `UseStill`) y `SelectedFile`: `null` = nunca elegiste (usa `Background:DefaultFile`), `""` = sin fondo. |
| `sesiones.json` | Una entrada por cada vez que se abre la sala: `Number` (1, 2, 3…), `StartedAt`, `EndedAt` y `Guests` (nombre + hora de entrada). El host no cuenta. |
| `registro-medios.jsonl` | Una línea JSON por falla de medios: video de fondo (`Error` + `ExtendedErrorCode` de `MediaFailed`), miniatura de Windows, ffmpeg, MediaComposition, imagen o portada. Crece sin tope (ver `PENDIENTES_LIMPIEZA.md`). |
| `cache\fondos\<hash>.jpg` | Cuadro fijo de cada video (miniatura de FONDOS y fondo en modo cuadro fijo), máx. `Background:FrameMaxWidth` de ancho. El hash sale de ruta + tamaño + fecha: si el video cambia, se genera otro. |

## Login y recuperación

- `LoginStep` decide qué se ve: `SignIn` (ENTRAR, siempre el paso inicial), `Register` (Registrar usuario: nombre y contraseña dos veces), `Recover` (¿Olvidaste tu contraseña?: usuario → su pregunta → contraseña nueva) y `SetupQuestion` (usuario sin pregunta: se pide después de ENTRAR por primera vez).
- Hasta `Usuarios:Maximo` (20) usuarios, con nombre único sin distinguir mayúsculas y minúsculas. Todos entran a lo mismo: Dashboard, Sala, Emotes, Música, Fondos, Manual, sesiones y visitas no saben qué usuario entró.
- Migración: `HostAccounts.LoadAsync` lee `usuarios.json` como `JsonElement`; si es una lista la usa tal cual y si es una cuenta suelta la convierte en el primer usuario. No valida ni recorta nada al leer, así que ninguna cuenta guardada se pierde (aunque su nombre pase del límite).
- Recuperación: si el usuario no existe o no tiene pregunta, el mensaje es el mismo ("No se pudo recuperar la contraseña con esos datos.") para no revelar qué usuarios hay.
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
- Kestrel no se apaga al cerrar la sala: `IChatService`/`IRoomService` salen del contenedor de la web y los ViewModels (Singleton) guardan esas instancias. Se apaga al cerrar la ventana (`CloseGuard` → `AppShutdown.StopAsync` → `RoomSession.ShutdownAsync`). Los `MediaPlayer` se detienen pero no se liberan con `Dispose`: sus `MediaPlayerElement` y el `Unloaded` de `ManualPage` los siguen tocando después de cerrar la ventana. Con la sala cerrada `/join` no deja entrar y sin túnel nadie de fuera llega.
- SALIR del dashboard y cerrar la ventana con sala activa usan `RoomSession.ConfirmLeaveAsync()`. CERRAR SALA no pregunta (es el botón explícito).
- Si la app muere de golpe con la sala abierta, esa sesión se queda con `EndedAt = null`.

## Música, emotes y firma

- `MusicService` es una sola instancia (Singleton): la firma del login (`FirmaViewModel`) y `MusicaPage` (`MusicaViewModel`) heredan de `MusicControlsViewModel` y leen el mismo reproductor, así que van sincronizadas.
- La música vive fuera del repo: `Paths:MediaRoot` + `Audio:MusicFolder` (canciones) y `Audio:CoversFolder` (portadas). La app no crea la carpeta; si falta, `MusicaPage` lo avisa con un InfoBar.
- `EmotesViewModel` usa `IRoomEmotes` de Core tal cual, sacado del contenedor de la web (misma instancia que los chats de los invitados). Los emotes tienen su propia pantalla (EMOTES), no son un bloque de la sala.

## Fondos

- Carpeta fuera del repo: `Paths:MediaRoot` + `Background:Folder` (imágenes `Background:ImageExtensions`, videos `Background:VideoExtensions`). La app no la crea.
- `CapaFondo` vive en `MainWindow`, detrás del `Frame`, y es la única: Dashboard, Sala, Fondos y Manual son páginas transparentes (en el Manual un fondo de video sigue en bucle pero mudo, `FondoPlayer.SetSilenced`; al salir recupera su sonido normal, `Background:VideoMuted`, sin reiniciarse). En las demás páginas se oculta y el video se pausa. La carpeta se lee la primera vez que entras al dashboard (el login no la toca).
- Un solo `MediaPlayer` (`FondoPlayer`) en bucle y en silencio; cambiar de fondo solo cambia su `Source`.
- Videos de más de `Background:HeavyVideoMegabytes` (120 MB) arrancan como **cuadro fijo**. En FONDOS, "Usar cuadro fijo" lo cambia por video y se guarda en `fondos.json`.
- **Cuadro fijo** (`FondoFrameCache`): una sola vez por video, async, en `%LocalAppData%\COT_0\cache\fondos\<hash>.jpg`. Intentos en orden:
  1. `ffmpeg` si ya está junto al `.exe` o en el `PATH` (`Background:FfmpegPath`; no se instala nada): cuadro en `Background:FrameSeconds` (3 s), máx. `FrameMaxWidth` (1280) con `lanczos`.
  2. Miniatura de Windows con `ThumbnailMode.VideosView` (con `SingleItem` Windows regresaba el **icono** del archivo, no un cuadro).
  3. `MediaComposition.GetThumbnailAsync` (Media Foundation; en lugar del frame server de `MediaPlayer`, que pide una superficie Direct3D).
- La miniatura de FONDOS y el cuadro fijo del fondo usan ese mismo JPEG (decodificado a `ThumbnailSize` o `StillSize`).
- El video se abre con `StorageFile.GetFileFromPathAsync` + `MediaSource.CreateFromStorageFile` (no con `Uri`), así los nombres con `：`, `＂`, `｜`, corchetes o espacios no pasan por el escape de URL.
- Si un video no abre, cae solo a su cuadro fijo; `FondoFailures` traduce el `ExtendedErrorCode` (archivo que ya no está, bloqueado, sin códec, no decodifica) y el InfoBar de FONDOS dice la causa con el código. Todo queda en `registro-medios.jsonl`. Abrir FONDOS vuelve a intentar los archivos que fallaron.
- Velo negro: `Background:VeilOpacity` con fondo elegido, `Background:PickerVeilOpacity` en FONDOS (reemplaza el desenfoque).

## Manual de Lain

- Archivos fuera del repo (`Manual:*`, relativos a `Paths:MediaRoot`): `gifs\lain_tele_web.mp4`, `gifs\lain_despedida.mp4`, `manual\notificacion.mp3`, `manual\estatica.wav` y, opcional, una captura por paso en `manual\pasos\paso1.png`…`paso6.png`.
- Fondo: `FondoManual` (azul noche opaco y barrido) tapa el fondo elegido (`CapaFondo`), que sigue corriendo debajo, mudo y sin verse. Lain va en una sombra negra (`LainShade`) que empata con el negro de su cuadro; encima, `EstaticaManual` (textura fija `Assets/Imagenes/Manual/estatica.png` que salta de lugar) y la viñeta. La despedida funde sus orillas al azul de su video.
- Música: `ManualViewModel` pone `MusicService.IsHushed` en `true` al entrar y en `false` en `Leave`: la canción sigue avanzando muda y al salir se oye donde iba. No toca `IsMuted` (el silencio del usuario con la bocina), así que ese se conserva.
- Al abrir: `ManualAssets.RefreshAsync()` revisa qué existe; lo que falte sale en un InfoBar y el manual sigue sin eso.
- Pasos: textos en `Textos.xaml` (`Text.Manual.StepN.Title/Text/NoticeM`); cuántos avisos tiene cada uno, en `Manual:NoticeCounts` (3 por paso). El aviso N explica el resaltado N.
- Vistas de ejemplo: `VistaEjemploManual` elige la del paso (1 login, 2 inicio, 3 y 4 sala, 5 emotes, 6 música). Los datos ficticios salen de `Manual:Demo`
  (usuario, código, enlace, invitados, set y reportes, canciones, volumen) y los arma `ManualDemoViewModel`; la hora es la del reloj. No tocan la sala, el túnel, 7TV ni la música.
- Resaltados: `ManualViewModel.Phase` llega a la vista; cada `ResaltadoManual` dice en qué fases se ve (aviso 1 → fases 1-2, aviso 2 → 3-4, aviso 3 → 5 en adelante).
- Si hay captura `manual\pasos\pasoN.png`, se muestra en lugar de la vista de ejemplo de ese paso.
- Fases: la fase 2i+1 muestra el aviso i y la 2i+2 lo quita; la última se queda. Los tiempos salen de `Manual:CueSeconds`.
- Atajos: ENTER siguiente (después del último, despedida; en la despedida, cerrar), → / ← cambian de paso, ESC despedida y luego cerrar. ← INICIO regresa al dashboard en cualquier momento.
- La despedida usa el mismo video y el mismo reproductor: un `StateTrigger` centra en toda la pantalla el grupo video + texto (`VideoLayer`, un `Viewbox` que solo reduce) con menos opacidad.

## Atajos de teclado sin letrero

WinUI muestra un tooltip con la tecla ("Enter", "Esc", flechas) al pasar el mouse por cualquier elemento con `KeyboardAccelerators`, salvo que tenga `KeyboardAcceleratorPlacementMode="Hidden"`. En COT_0 ningún atajo muestra letrero:

- `Services/Ventana/ShortcutHints.Hide(elemento)`: `Navigator` lo aplica a **toda página** al navegar.
- Estilos con `Hidden` por defecto: el estilo implícito de `Button` y `TextButton` / `SolidButton` (`App.xaml`), y `BotonManualStyle` (`Views/Estilos/Manual.xaml`).
- Además, explícito en cada dueño de atajos: `ManualPage` (ENTER, →, ←, ESC), el botón ← INICIO de `EncabezadoPantalla` (Esc) y APLICAR de `PanelEmotes` (Enter).
- Atajo nuevo en otro tipo de elemento: poner `KeyboardAcceleratorPlacementMode="Hidden"` en el XAML o llamar a `ShortcutHints.Hide`.
- Tooltips que sí se quedan (intencionales): logo Neondev ("Abrir el repositorio del proyecto") y las bocinas (firma, música y manual).
