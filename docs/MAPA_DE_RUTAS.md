# Mapa de rutas de COT_0

Guía para explicar el proyecto: dónde está cada cosa (A), qué direcciones web usan los invitados (B) y cómo se mueve el host entre pantallas (C). Revisado en la rama `host-winui`.

## Resumen en 5 líneas

1. **COT_0.Core** es el "idioma común": modelos (mensaje, usuario, emote, fondo) e interfaces que los otros dos proyectos comparten; no depende de nadie.
2. **COT_0.Web** es la página que ven los invitados en el celular (Blazor Server + SignalR): perfil en `/join`, chat en `/chat` y manual en `/manual`.
3. **COT_0.Host** es la ventana de escritorio (WinUI 3) del anfitrión: login, dashboard, sala con QR, emotes, música, fondos y manual de Lain.
4. Cuando el host abre la sala, el Host arranca la Web **dentro de su mismo proceso** (Kestrel en `localhost:5050`) y lanza `cloudflared`, que le da una URL pública `https://….trycloudflare.com`.
5. Ese enlace se convierte en QR; los invitados lo escanean, entran a `/join`, y Host y Web se hablan por las interfaces de Core (por ejemplo, `IRoomService` para abrir/cerrar la sala e `IChatService` para ver conectados o expulsar).

---

## A) Rutas de carpetas y archivos clave

### Raíz del repo — `COT_0_P\COT_0\`

Para qué sirve: junta los tres proyectos y la documentación.

| Archivo / carpeta | Qué es |
|---|---|
| `COT_0.sln` | La solución: abre los tres proyectos juntos en Visual Studio. |
| `README.md` | Qué es la app, cómo funciona y cómo se corre. |
| `CONTRATO.md` | Acuerdo entre los agentes: quién toca qué carpeta y pedidos de cambio a Core. |
| `.gitignore` | Lo que Git no sube (`bin`, `obj`, `.vs`, etc.). |
| `docs\` | Documentación del proyecto (ver abajo). |
| `COT_0.Core\`, `COT_0.Web\`, `COT_0.Host\` | Los tres proyectos. |

Referencias entre proyectos: **Host → Web y Core**, **Web → Core**. Core no conoce a nadie.

---

### COT_0.Core — lo compartido

#### `COT_0.Core\Models\` — los "datos" que viajan entre Web y Host

| Archivo | Qué hace |
|---|---|
| `ChatMessage.cs` | Un mensaje del chat (texto, autor, hora, tipo). |
| `MessageKind.cs` | Tipo de mensaje: de invitado, aviso del sistema, nota de voz o imagen. |
| `User.cs` | Un invitado conectado (solo en memoria). |
| `FlagOption.cs` | Una facultad/escudo que el invitado puede elegir. |
| `RoomEmote.cs` | Un emote de 7TV de la sala (las imágenes vienen del CDN de 7TV). |
| `EmoteSources.cs` | Las URLs de un emote (animado, cuadro fijo, gif). |
| `EmoteSetStatus.cs` / `EmoteSetError.cs` | Estado del set de 7TV y por qué falló si falló. |
| `EmoteReport.cs` | Reporte que hace un invitado sobre un emote; lo revisa el host. |
| `Fondo.cs` / `FondoKind.cs` | Un fondo (imagen o video) de la carpeta de fondos y su tipo. |

#### `COT_0.Core\Services\` — los "contratos" (interfaces)

| Archivo | Qué hace |
|---|---|
| `IRoomService.cs` | Estado de la sala: abrir, cerrar, código. La abre el host, la ven los invitados. |
| `IChatService.cs` | Conectados e historial de mensajes en memoria (y expulsar). |
| `IFlagService.cs` | Lista de facultades con su escudo. |
| `IRoomEmotes.cs` | Emotes de la sala: set elegido, emotes quitados y reportes. |
| `ITunnelService.cs` | El túnel público de cloudflared. |
| `IBackgroundCatalog.cs` | Catálogo de la carpeta de fondos (lo implementa el Host). |
| `MediaPath.cs` | Arma rutas de archivos fuera del repo a partir de `Paths:MediaRoot`. |

---

### COT_0.Web — la página de los invitados

#### Raíz `COT_0.Web\`

| Archivo | Qué hace |
|---|---|
| `WebApp.cs` | **Arma toda la web**: servicios, archivos estáticos, hub y páginas. Lo usan el Host y `Program.cs`. |
| `Program.cs` | Arranque "suelto" para probar la web sin el Host (la sala nace abierta). |
| `App.razor` | El HTML base: carga los CSS, los JS y el aviso de "RECONECTANDO…". |
| `_Imports.razor` | Los `using` comunes de todos los `.razor`. |
| `Properties\launchSettings.json` | Puerto 5050 cuando se corre la web sola con `dotnet run`. |

> La web **no tiene appsettings.json propio**: cuando corre dentro del Host lee el `appsettings.json` del Host (secciones `Web`, `Chat`, `GuestManual`, `Paths`).

#### `COT_0.Web\Views\Pantallas\` — las páginas con dirección (URL)

| Archivo | Qué hace |
|---|---|
| `Join.razor` | `/join`: perfil del invitado (nombre, foto, facultad). |
| `Chat.razor` | `/chat`: el chat grupal. |
| `Manual.razor` | `/manual`: tutorial animado para el invitado, con Lain. |
| `Error.razor` | `/Error`: página de error genérica. |

#### `COT_0.Web\Views\Plantillas\` — el marco de las páginas

| Archivo | Qué hace |
|---|---|
| `Routes.razor` | El enrutador: según la URL elige qué página mostrar. |
| `MainLayout.razor` | El molde común donde se pinta cada página. |

#### `COT_0.Web\Views\Componentes\` — piezas reutilizables

| Grupo | Archivos | Qué hacen |
|---|---|---|
| Fondo y marca | `Backdrop`, `AshLayer`, `CotLogo`, `Icon` | Fondo ASCII con niebla y ceniza, logo, íconos. |
| Perfil | `ProfileForm`, `ProfileEditor`, `PhotoUpload`, `UserAvatar` | Formulario de perfil (en `/join` y en "editar perfil"), foto. |
| Facultad | `FacultyField`, `FacultySheet`, `FacultyShield`, `FlagIcon` | Campo, panel con buscador y escudos. |
| Chat | `ChatHeader`, `MessageList`, `MessageBlock`, `MessageText`, `ChatComposer`, `UserPanel` | Barra de arriba, mensajes, cuadro de escribir, lista de conectados. |
| Emotes | `EmotePicker`, `EmoteImage`, `EmoteMenu` | Elegir emotes, mostrarlos, reportarlos. |
| Imágenes | `ImageDraft`, `ImageBubble`, `ImageViewer` | Vista previa, miniatura en el chat, imagen en grande. |
| Voz | `VoiceRecordingBar`, `VoiceBubble` | Grabar y reproducir notas de voz. |

`Views\Componentes\Manual\` — piezas solo del manual del invitado: `ManualWelcome` (bienvenida), `ManualStory` (pasos tipo historias), `ManualFinish` (final), `ManualNav` (ANTERIOR/SIGUIENTE/SALIR), `ManualNotices` (avisos de Lain), `LainTv` (video de Lain), `DemoChat` y `ManualDemoView` (vistas reales con datos de mentira), `ManualFx`, `ManualSignal`, `MuteButton` (efectos y silencio).

#### `COT_0.Web\Models\` — datos internos de la web

Clases chicas sin lógica: `JoinForm` (formulario de perfil), `LeaveReason` (por qué te sacaron: expulsado o sala cerrada), `StoredGuest` (perfil guardado en el navegador), `MessageGroup`/`MessageToken`/`FormattedMessage` (mensajes ya partidos para pintar), `VoiceClip`/`VoiceState`/… (notas de voz), `ImageClip`/`ImageView`/… (imágenes), `ManualStep`/`ManualNotice`/… (pasos del manual), `OperationResult` (resultado o error).

#### `COT_0.Web\Services\` — la lógica de la web, por tema

| Carpeta | Para qué sirve | Archivos importantes |
|---|---|---|
| `Sala\` | Tiempo real, presencia y estado de la sala | `ChatHub.cs` (el hub de SignalR), `ChatBroadcaster.cs` (avisa a todos los celulares), `InMemoryChatService.cs` (chat en memoria), `RoomService.cs` (sala abierta/cerrada), `ChatSession.cs` (conexión de cada pestaña), `PresenceTracker.cs` (saca a quien se desconectó y no volvió), `ChatRegistration.cs` (registra servicios y endpoints). |
| `Perfil\` | Perfil del invitado | `ProfileService.cs` (crear/editar perfil), `GuestStore.cs` (recordar el perfil en el navegador), `InMemoryAvatarStore.cs` (fotos en memoria), `FlagService.cs` (facultades y escudos). |
| `Mensajes\` | Formato del texto | `MessageFormatter.cs` (parte texto, links y emotes), `MessageGrouping.cs` (junta mensajes seguidos), `ComposerEditor.cs` (puente con `composer.js`). |
| `Emotes\` | Emotes de 7TV | `SevenTvClient.cs` (pide el set a la API de 7TV), `RoomEmotes.cs` (caché y reportes), `EmoteActions.cs` (menú de reportar), `EmoteSearch.cs`. |
| `Imagenes\` | Fotos dentro del chat | `ImageService.cs` (valida y publica), `ImageSniffer.cs` (revisa que sí sea imagen), `InMemoryImageStore.cs`. |
| `Voz\` | Notas de voz | `VoiceService.cs`, `VoiceRecorder.cs` (puente con `voice.js`), `OpusWavTranscoder.cs` + `WebmOpusFrames.cs` + `WavWriter.cs` (convierten a WAV para iPhone). |
| `Manual\` | Manual del invitado | `ManualTour.cs` (en qué paso vas), `ManualDemo.cs` (datos de muestra), `ManualRegistration.cs` (sirve los audios del manual). |
| `Configuracion\` | Valores por defecto | `ChatOptions.cs`, `VoiceOptions.cs`, `ManualOptions.cs`, `ManualSteps.cs` (textos de los 5 pasos). |

#### `COT_0.Web\wwwroot\` — archivos que el celular descarga tal cual

| Carpeta | Qué hay |
|---|---|
| `css\` | Estilos por pantalla: `join.css`, `chat.css`, `messages.css`, `composer.css`, `voice.css`, `images.css`, `emotes.css`, `manual*.css`, `reconnect.css`, `fonts.css`, `cot.css`, `app.css`. |
| `js\` | Lo mínimo de JavaScript que el navegador exige: `chat.js` (scroll), `composer.js` (cuadro de texto con emotes), `voice.js` (micrófono), `manual.js` (sonidos/atajos del manual), `reconnect.js` (reintento). |
| `fonts\` | Tipografías Barlow y Cinzel. |
| `img\` | `bg-ascii.png` (fondo) y `flags\` (26 escudos de facultad). |

---

### COT_0.Host — la ventana del anfitrión

#### Raíz `COT_0.Host\`

| Archivo | Qué hace |
|---|---|
| `App.xaml` / `App.xaml.cs` | Punto de entrada: arma los servicios, abre la ventana y prepara la web en segundo plano. |
| `MainWindow.xaml` / `.cs` | La única ventana: fondo + un "marco" (Frame) que muestra una pantalla a la vez; abre el Login. |
| `appsettings.json` | **Toda la configuración** (carpetas, puerto, textos de formato, volumen, QR, túnel, chat…). |
| `app.manifest` | Ajuste de Windows para la nitidez (DPI). |
| `COT_0.Host.csproj` | Proyecto; además copia `wwwroot` de la web junto al `.exe`. |

#### `COT_0.Host\Views\Pantallas\` — las pantallas

`LoginPage`, `DashboardPage`, `SalaPage`, `EmotesPage`, `MusicaPage`, `FondosPage`, `ManualPage` (cada una `.xaml` + `.xaml.cs`). Detalle en la sección C.

#### `COT_0.Host\Views\ModelosDeVista\` — el "cerebro" de cada pantalla

| Archivo | Qué hace |
|---|---|
| `LoginViewModel.cs` + `LoginStep.cs` | Login con sus pasos: entrar, registrar, recuperar, pregunta secreta. |
| `RegisterViewModel.cs` / `RecoveryViewModel.cs` / `SecurityQuestionViewModel.cs` | Registro, "¿olvidaste tu contraseña?" y pregunta secreta. |
| `DashboardViewModel.cs` | Reloj, estado de la sala y botones del menú. |
| `SalaViewModel.cs` | Abre la sala, muestra QR, enlace, código y tiempo; la cierra. |
| `ConnectedUsersViewModel.cs` / `UserRowViewModel.cs` | Lista de conectados (y expulsar). |
| `EmotesViewModel.cs` / `EmoteRowViewModel.cs` | Set de 7TV, reportes y quitados. |
| `MusicaViewModel.cs` / `MusicControlsViewModel.cs` / `SongRowViewModel.cs` / `FirmaViewModel.cs` | Música y volumen. |
| `FondosViewModel.cs` / `FondoRowViewModel.cs` / `CapaFondoViewModel.cs` | Selector de fondos y el fondo vivo detrás de las pantallas. |
| `ManualViewModel.cs` / `ManualStepViewModel.cs` / `ManualNoticeViewModel.cs` / `ManualDemoViewModel.cs` | Manual de Lain: pasos, avisos, datos de muestra, despedida. |
| `FormViewModel.cs` / `LogoNeondevViewModel.cs` | Base de formularios; logo que abre el repo. |

#### `COT_0.Host\Views\Componentes\` — piezas reutilizables

| Archivo | Qué hace |
|---|---|
| `EncabezadoPantalla` | Barra "← INICIO / ← SALA \| TÍTULO" de las pantallas internas (regresa con clic o Esc). |
| `FormularioRegistro` / `FormularioRecuperacion` | Formularios dentro del login. |
| `ListaConectados` | Lista de invitados en la sala. |
| `PanelEmotes` | Bloque de emotes (se usa en EmotesPage). |
| `CapaFondo` | El fondo (imagen o video) que va detrás de todo. |
| `ConfirmacionDialogo` | Ventanita de "¿seguro?" (sí/no). |
| `Firma`, `LogoNeondev`, `ControlVolumen`, `BotonBocina` | Firma Neondev con volumen. |
| `TarjetaMenu`, `PuntoPulso` | Botones inclinados del dashboard y punto rojo de "en vivo". |
| `FiltroCampo` | Quita símbolos y espacios mientras escribes. |
| `Manual\` | Piezas solo del manual de Lain: `VistaEjemplo*` (réplicas de Login, Inicio, Sala, Emotes, Música), `AvisoLain` (notificación), `ResaltadoManual`, `EncabezadoEjemplo`, `BotonManual`. |

#### Otras carpetas de `Views\`

| Carpeta | Para qué sirve |
|---|---|
| `Views\Estilos\` | Estilos XAML por pantalla (`Dashboard`, `Sala`, `Manual`, `TarjetaMenu`, `Volumen`) y **`Textos.xaml`: todos los textos visibles de la app**. |
| `Views\Convertidores\` | Pequeñas traducciones para la vista: color del avatar, color del estado de emotes, íconos de música. |

#### `COT_0.Host\Models\` — datos del Host

`HostUser` (usuario del login en `usuarios.json`), `HashedSecret` (contraseña hasheada), `Visit` (entrada a la sala), `SesionInvitados` (una sesión por cada vez que se abre la sala), `Song`/`CoverArt` (canción y portada), `FondosGuardados` (contenido de `fondos.json`), `MediaLogEntry` (línea del registro de fallas), y los errores para mostrar en pantalla: `CredentialError`, `RoomError`, `MusicError`, `FondoError`; además `ConfirmTexts`, `SecurityQuestionOption`, `ManualDemoRow`.

#### `COT_0.Host\Services\` — la lógica del Host, por tema

| Carpeta | Para qué sirve | Archivos importantes |
|---|---|---|
| `Sala\` | Abrir y cerrar la sala | `RoomSession.cs` (servidor → túnel → sala, y al revés), `WebServerHost.cs` (arranca la web dentro del Host), `QrService.cs` (dibuja el QR), `PortProbe.cs` (revisa que el 5050 esté libre), `RoomOpenErrors.cs` (traduce errores), `GuestWatcher.cs` (detecta invitados nuevos). |
| `Tunel\` | Internet público | `CloudflareTunnelService.cs` (lanza `cloudflared` y lee la URL), `KillOnCloseJob.cs` (si el Host muere, mata a cloudflared). |
| `Usuarios\` | Cuentas del host | `UserService.cs` (registro y login, máximo 20), `PasswordRecoveryService.cs`, `SecretHasher.cs` (hash PBKDF2), `HostAccounts.cs` (lee/escribe `usuarios.json`). |
| `Validaciones\` | Reglas de los campos | `InputRules.cs` (solo letras y números), `InputLimits.cs` (15 caracteres), `CredentialValidator.cs`, `SecurityAnswerValidator.cs`. |
| `Ventana\` | La ventana y la navegación | `Navigator.cs` (**cambia de pantalla**), `CloseGuard.cs` (pregunta antes de cerrar), `WindowSetup.cs` (pantalla completa), `ShortcutHints.cs`. |
| `Almacenamiento\` | Guardar en disco | `JsonStore.cs` (lee/guarda JSON en `%LocalAppData%\COT_0`), `AsyncFiles.cs`. |
| `Cache\` | Caché | `CacheFolders.cs` (carpetas en `%LocalAppData%\COT_0\cache`). |
| `Configuracion\` | Leer `appsettings.json` | `AppConfig.cs` (lo lee una vez) y una clase por sección: `WebSettings`, `TunnelSettings`, `QrSettings`, `AudioSettings`, `BackgroundSettings`, `ManualSettings`, `PathsSettings`, `StorageSettings`, etc. |
| `Musica\` | Música de fondo | `MusicService.cs` (reproduce en bucle), `MusicLibrary.cs`, `SongCatalog.cs`, `CoverLibrary.cs`/`CoverImages.cs` (portadas). |
| `Fondos\` | Fondos de pantalla | `FondoService.cs` (estado general), `FondoCatalog.cs` (lee la carpeta), `FondoPlayer.cs` (video en bucle), `FondoFrameCache.cs` (cuadro fijo de cada video) y tres formas de sacar ese cuadro: `FfmpegFrames`, `ShellThumbnailFrames`, `CompositionFrames`. |
| `Manual\` | Manual de Lain | `ManualVideo.cs`, `ManualSound.cs`, `ManualTimeline.cs`, `ManualAssets.cs` (rutas de videos y sonidos). |
| `Visitas\` / `Sesiones\` | Bitácoras | `VisitService`/`VisitWatcher` (`visitas.json`), `SesionService`/`SesionWatcher` (`sesiones.json`). |
| `Registro\` | Log de fallas | `MediaLog.cs` (`registro-medios.jsonl`). |
| `Dialogos\`, `Textos\`, `Tiempo\` | Utilidades | `ConfirmDialogService` (abre el "¿seguro?"), `UiText` (busca un texto de `Textos.xaml`), `ClockService` (reloj), `UiThread` (pasa avisos de la web a la ventana). |

#### `COT_0.Host\Assets\`

| Carpeta | Qué hay |
|---|---|
| `Fuentes\` | Tipografías Barlow, Cinzel, Inter y Redaction. |
| `Imagenes\` | `ascii-bg.png` (fondo del login) y `neondev-avatar.png` (logo de la firma). |

---

### `docs\` — documentación

| Archivo | Qué es |
|---|---|
| `ESTRUCTURA.md` | Estructura de la solución y reglas de carpetas. |
| `MAPA_CARPETAS.md` | Qué carpetas se movieron/renombraron y por qué. |
| `MAPA_DE_RUTAS.md` | Este documento. |
| `CUMPLIMIENTO.md` | Cómo se cumple cada cosa que pidió el profe. |
| `PRUEBAS_MANUALES.md` | Lista de pruebas a mano del Host. |
| `REPORTE_CIERRE.md` | Cierre de la migración de WPF a WinUI 3. |
| `CAMBIOS_VISUALES.md` | Qué se simplificó visualmente al pasar a WinUI 3. |
| `PENDIENTES_LIMPIEZA.md` | Lo que falta revisar o limpiar. |
| `PREPARAR_LAIN.md` | Cómo preparar el video de Lain con transparencia. |
| `qa\REPORTE_QA.md`, `qa\PRUEBAS_NO_REALIZADAS.md`, `qa\CAPAS_MANUAL.md`, `qa\capturas\` | Reporte de pruebas, lo que no se probó, capas del manual y 3 capturas de errores encontrados. |

---

### Rutas fuera del repo que usa la app

**1. Datos guardados — `%LocalAppData%\COT_0\`** (en esta PC: `C:\Users\neonpc\AppData\Local\COT_0\`). Lo maneja `JsonStore.cs`.

| Archivo | Qué guarda |
|---|---|
| `usuarios.json` | Usuarios del host (contraseña y respuesta secreta hasheadas). |
| `visitas.json` | Bitácora de cada invitado que entró. |
| `sesiones.json` | Una sesión por cada vez que se abrió la sala. |
| `fondos.json` | Catálogo de fondos y el fondo elegido. |
| `registro-medios.jsonl` | Fallas al reproducir música/fondos/videos (se crea cuando hay una). |
| `cache\fondos\<hash>.jpg` | **Caché**: cuadro fijo de cada video de fondo, para no generarlo cada vez. |

> Los mensajes del chat, fotos, imágenes y notas de voz **no** se guardan en disco: viven en memoria y se borran al cerrar la sala.

**2. Medios — `Paths:MediaRoot`** = `C:\Users\neonpc\Documents\COT_0_P\` (en otra PC solo se cambia esa línea de `appsettings.json`).

| Carpeta | Qué hay | Quién la usa |
|---|---|---|
| `musica\` | Canciones (`Audio_01.mp3` … `Audio_05.mp3`); arranca con `Audio_01` al 25 %. | Host, pantalla Música. |
| `portadas\` | Portada de cada canción, con el mismo nombre (`Audio_01.png`). | Host, pantalla Música. |
| `fondos\` | Videos/imágenes de fondo (por defecto `Batman.mp4`). | Host, pantalla Fondos y fondo vivo. |
| `gifs\` | Videos de Lain: `lain_tele_web.mp4` (manual del host) y `lain_despedida.mp4` (despedida). | Host, Manual de Lain. |
| `manual\` | `notificacion.mp3`, `estatica.wav` (host); `lain.mp4`, `lain-poster.jpg` y `notificacion.mp3` (manual web, servidos en `/manual-assets/`). | Host y Web. |

**3. Programas externos**: `cloudflared` y (opcional) `ffmpeg`, tomados del PATH o junto al `.exe`.

---

## B) Rutas web (lo que usan los invitados en el celular)

Todas viven en `http://localhost:5050` y, hacia afuera, en la URL de Cloudflare (`https://algo.trycloudflare.com`). Se registran en `COT_0.Web\WebApp.cs`.

### Páginas

| URL | Qué ve o hace el invitado | Archivo |
|---|---|---|
| `/` | Nada: lo manda directo a `/join`. | `WebApp.cs` |
| `/join` | **Lo que abre el QR.** Llena nombre, foto y facultad y toca "ENTRAR AL CHAT". Si la sala está cerrada ve un aviso. Si ya había entrado antes en esa sala, lo reconoce y lo pasa solo al chat. Tiene el enlace "¿Primera vez? Mira cómo funciona" → `/manual`. | `Views\Pantallas\Join.razor` |
| `/join?reason=kicked` | Igual que `/join` con el aviso "El anfitrión te expulsó de la sala". | `Join.razor` + `Models\LeaveReason.cs` |
| `/join?reason=closed` | Igual que `/join` con el aviso "La sala se cerró". | `Join.razor` + `LeaveReason.cs` |
| `/chat?u={id}` | El chat grupal: mensajes, emotes, fotos, notas de voz, lista de conectados y editar perfil. Si no tiene perfil válido, regresa a `/join`. | `Views\Pantallas\Chat.razor` |
| `/manual` | Tutorial con Lain: bienvenida, 5 pasos tipo historias y final. "SALIR" o "entrar" lo regresa a `/join`. | `Views\Pantallas\Manual.razor` |
| `/Error` | Página de error: "Recarga o vuelve a escanear el QR". | `Views\Pantallas\Error.razor` |

### Tiempo real y archivos del chat

| URL | Qué hace | Archivo |
|---|---|---|
| `/chathub` | **El hub de SignalR**: canal en vivo por donde llegan y salen mensajes, "escribiendo…", conectados y expulsiones. El invitado no lo ve; lo usa la página `/chat`. | `Services\Sala\ChatHub.cs` (y `ChatBroadcaster.cs` para avisar a todos) |
| `/avatars/{id}` | Devuelve la foto de perfil de un invitado. | `Services\Sala\ChatRegistration.cs` |
| `/images/{id}` | Devuelve una imagen enviada en el chat. | `ChatRegistration.cs` |
| `/voice/{id}` | Devuelve una nota de voz (con soporte para iPhone). | `ChatRegistration.cs` |
| `/manual-assets/{archivo}` | Video, póster y sonido del manual web (salen de la carpeta `manual\` fuera del repo). | `Services\Manual\ManualRegistration.cs` |
| `/css/…`, `/js/…`, `/fonts/…`, `/img/…`, `/img/flags/{facultad}.png` | Estilos, scripts, letras, fondo y escudos. | Carpeta `wwwroot\` |
| `/_framework/blazor.web.js` | Script propio de Blazor que mantiene viva la conexión. | Lo pone .NET |

Hacia afuera, la web solo llama a la **API de 7TV** (`https://7tv.io/v3/emote-sets/{id}`) para traer los emotes (`Services\Emotes\SevenTvClient.cs`).

### Recorrido típico del invitado

```
Escanea QR → /join → (opcional /manual → /join) → /chat?u=… 
                                  ↑                      │
                                  └── expulsado o sala cerrada (/join?reason=…)
```

---

## C) Rutas de navegación del Host (escritorio)

Todo pasa en **una sola ventana** a pantalla completa. Quien cambia de pantalla es `Services\Ventana\Navigator.cs`. Las pantallas internas tienen arriba `EncabezadoPantalla` con "← INICIO" (o "← SALA") y la tecla **Esc** para regresar.

```
Login ─(Registrar / ¿Olvidaste?)─ se quedan dentro del Login
  │ ENTRAR
  ▼
Dashboard ──► Sala ──► Emotes / Música / Fondos (regresan a Sala)
  │  ├──► Emotes / Música / Fondos (regresan a Dashboard)
  │  ├──► Manual de Lain ──► Despedida ──► Dashboard
  │  └── SALIR ──► Login
  └── ✕ o Alt+F4 en cualquier momento ──► "¿Salir de COT_0?" ──► cierra todo
```

| Pantalla | Cómo se llega | A dónde puede ir | Archivos |
|---|---|---|---|
| **Login** | Es la primera al abrir la app (`MainWindow`), o al tocar SALIR en el Dashboard. | ENTRAR → Dashboard. "Registrar usuario" y "¿Olvidaste tu contraseña?" cambian el formulario sin salir. | `Views\Pantallas\LoginPage.xaml(.cs)`, `ModelosDeVista\LoginViewModel.cs` |
| **Registro** | Desde Login, enlace "Registrar usuario" (hasta 5 usuarios). | Al terminar o cancelar, vuelve al paso ENTRAR del Login. | `Componentes\FormularioRegistro.xaml`, `ModelosDeVista\RegisterViewModel.cs` |
| **Recuperación** | Desde Login, enlace "¿Olvidaste tu contraseña?". Pide usuario, respuesta secreta y contraseña nueva. | Al terminar o cancelar, vuelve a ENTRAR. | `Componentes\FormularioRecuperacion.xaml`, `ModelosDeVista\RecoveryViewModel.cs`, `Services\Usuarios\PasswordRecoveryService.cs` |
| *(Pregunta secreta)* | Paso extra del Login: aparece después de ENTRAR si la cuenta todavía no tiene pregunta. | GUARDAR Y ENTRAR → Dashboard. | `LoginViewModel.cs` (paso `SetupQuestion`), `SecurityQuestionViewModel.cs` |
| **Dashboard** (Inicio) | Después de ENTRAR; o al regresar de cualquier pantalla interna; o al cerrar la sala o terminar el manual. | Tarjetas: 01 GENERAR QR / SALA → Sala · Fondo → Fondos · Manual → Manual · Emotes · Música · SALIR → Login (si hay sala activa, pregunta y la cierra). | `Pantallas\DashboardPage.xaml(.cs)`, `ModelosDeVista\DashboardViewModel.cs` |
| **Sala** | Dashboard → "GENERAR QR" (o "SALA" si ya está abierta). Al entrar abre servidor + túnel y muestra QR, enlace, código, tiempo y conectados. | Emotes, Música, Fondos · "← INICIO" → Dashboard (la sala sigue abierta) · CERRAR SALA → pregunta, saca a todos y vuelve al Dashboard. | `Pantallas\SalaPage.xaml(.cs)`, `ModelosDeVista\SalaViewModel.cs`, `Services\Sala\RoomSession.cs` |
| **Emotes** | Desde Dashboard o desde Sala. Elegir set de 7TV, ver reportes, quitar/restaurar emotes. | "← INICIO" o "← SALA" (según de dónde vino) / Esc. | `Pantallas\EmotesPage.xaml(.cs)`, `Componentes\PanelEmotes.xaml`, `ModelosDeVista\EmotesViewModel.cs` |
| **Música** | Desde Dashboard o desde Sala. Biblioteca con portadas, pausa, siguiente, volumen. | Regresa a donde vino / Esc. | `Pantallas\MusicaPage.xaml(.cs)`, `ModelosDeVista\MusicaViewModel.cs`, `Services\Musica\MusicService.cs` |
| **Fondos** | Desde Dashboard o desde Sala. Miniaturas para elegir el fondo de la app. | Regresa a donde vino / Esc. | `Pantallas\FondosPage.xaml(.cs)`, `ModelosDeVista\FondosViewModel.cs`, `Services\Fondos\FondoService.cs` |
| **Manual de Lain** | Desde Dashboard. Seis pasos con réplicas de las pantallas reales y notificaciones de Lain. Enter/flechas avanzan. | Esc o Enter en el último paso → Despedida. | `Pantallas\ManualPage.xaml(.cs)`, `ModelosDeVista\ManualViewModel.cs`, `Componentes\Manual\` |
| **Despedida** | Parte final del Manual: video `lain_despedida.mp4` con estática. | Enter/Esc → Dashboard. | `ManualViewModel.cs` (`Farewell` y `Close`), `Services\Manual\ManualVideo.cs` |
| *(Cerrar la app)* | ✕ propia de la ventana o Alt+F4, desde cualquier pantalla. | Pregunta "¿Salir de COT_0?" (o "¿Cerrar la sala y salir?"); si aceptas, cierra sala, túnel y servidor. | `MainWindow.xaml.cs`, `Services\Ventana\CloseGuard.cs` |

Regla del botón "regresar" (`Navigator.GoBack`): vuelve a la pantalla anterior (Sala o Dashboard), **nunca al Login**.

---

## Cómo explicarlo en 3 minutos

> "COT_0 es un chat grupal para el salón, hecho todo en C#. Está dividido en tres proyectos.
>
> **Core** es lo que comparten los otros dos: los modelos, como el mensaje o el usuario, y las interfaces, por ejemplo `IRoomService` para saber si la sala está abierta. No depende de nadie.
>
> **Host** es la ventana de escritorio, hecha en WinUI 3. El anfitrión entra en el **Login** —ahí mismo puede registrarse o recuperar su contraseña con una pregunta secreta—. Luego llega al **Dashboard**, que es el menú. Si toca **Generar QR** pasa a la **Sala**: en ese momento el Host levanta un servidor web dentro de sí mismo en el puerto 5050 y lanza cloudflared, que nos da una URL pública. Esa URL se convierte en el QR. Desde el Dashboard o la Sala también se puede ir a **Emotes**, **Música** y **Fondos**, y siempre se regresa con Esc. Además está el **Manual de Lain**, un tutorial animado que termina con una despedida y vuelve al inicio. Los usuarios del host se guardan en `AppData\Local\COT_0`, con las contraseñas hasheadas; la música, fondos y videos están en una carpeta aparte que se configura en `appsettings.json`.
>
> **Web** es lo que ven los invitados en el celular, con Blazor Server y SignalR. El QR los lleva a **/join**, donde ponen nombre, foto y facultad. Después pasan a **/chat**, que se mantiene en tiempo real gracias al hub **/chathub**. Ahí pueden mandar texto, emotes de 7TV, fotos y notas de voz. Si el host los expulsa o cierra la sala, regresan a /join con un aviso. Y si es su primera vez, tienen **/manual**, un tutorial para el celular.
>
> No usamos base de datos: el chat vive en memoria y se borra al cerrar la sala. Todo se configura en `appsettings.json` y cada servicio tiene su interfaz y se registra con inyección de dependencias. En resumen: **Core define, Web atiende a los invitados y Host controla todo desde el escritorio.**"
