# Estructura de COT_0.HostWinUI

Host de COT_0 en WinUI 3. Una sola ventana con un `Frame`; cada pantalla es una `Page`.
Las carpetas de primer nivel van en inglés; las de adentro, en español y con el nombre de lo que hacen.
Una clase por archivo.

```
COT_0.HostWinUI/
├── App.xaml                      Paleta, fuentes, medidas y estilos comunes (login, TextButton, SolidButton, VolumeSlider, filas de lista)
├── App.xaml.cs                   Arranque: arma los servicios, crea la ventana, arranca la música y levanta la web
├── MainWindow.xaml               Única ventana: solo un Frame (RootFrame)
├── appsettings.json              Ventana, archivos JSON, seguridad, validación, preguntas secretas, reloj, Audio (música y portadas), Links y la config de la web/túnel
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
│   └── ConfirmTexts.cs           Textos del diálogo sí/no
├── Services/
│   ├── Almacenamiento/           JsonStore: lee y guarda cualquier clase en %LocalAppData%\COT_0\*.json (async)
│   ├── Configuracion/            AppConfig + un record por sección de appsettings.json (Window, Storage, Security, Validation, Recovery, Ui, Paths, Audio, Links)
│   ├── Dialogos/                 ConfirmDialogService: abre el diálogo sí/no sobre la ventana
│   ├── Musica/                   MusicLibrary y CoverLibrary (copiadas del WPF, sin tipos de WPF), SongCatalog (duración y portada, async)
│   │                             y MusicService (Windows.Media.Playback.MediaPlayer: play/pausa, anterior/siguiente, volumen, silencio)
│   ├── Sala/                     WebServerHost (Kestrel con COT_0.Web), RoomSession (abrir y cerrar la sala) y GuestWatcher (base: detecta quién entra)
│   ├── Sesiones/                 SesionService (sesiones.json) y SesionWatcher (abre/cierra la sesión con la sala y anota invitados)
│   ├── Textos/                   UiText: busca un texto de Textos.xaml desde C#
│   ├── Tiempo/                   ClockService (tick de cada segundo) y UiThread (pasa eventos al hilo de la UI)
│   ├── Tunel/                    CloudflareTunnelService y KillOnCloseJob (cloudflared muere con la app)
│   ├── Usuarios/                 UserService (registro/login), PasswordRecoveryService (¿olvidaste tu contraseña?),
│   │                             HostAccounts (usuarios.json) y SecretHasher (PBKDF2 + SHA-256 con salt)
│   ├── Validaciones/             CredentialValidator (solo letras y números) y SecurityAnswerValidator (normaliza la respuesta)
│   ├── Ventana/                  WindowSetup (tamaño y barra de título) y Navigator (cambia de página: Login, Dashboard, Sala, Emotes, Música, Manual)
│   └── Visitas/                  VisitService (visitas.json) y VisitWatcher (anota a cada invitado que entra)
└── Views/
    ├── Componentes/              Piezas reutilizables
    │   ├── ConfirmacionDialogo   ContentDialog sí/no
    │   ├── ListaConectados       Invitados conectados (ListView)
    │   ├── FormularioRecuperacion  Pregunta secreta → contraseña nueva, dentro del login
    │   ├── TarjetaMenu.cs        Tarjeta inclinada del menú (Button con número, subtítulo e icono)
    │   ├── PuntoPulso            Punto rojo con pulso (sala activa)
    │   ├── EncabezadoPantalla    ← INICIO | TÍTULO de Sala, Emotes y Música (clic o Esc regresa al dashboard)
    │   ├── PanelEmotes           Set de 7TV, reportes y emotes quitados (+ EmoteStatusBrushes.cs: color del estado)
    │   ├── ControlVolumen        Nota (silencio), barra de volumen y número; lo usan la firma y MusicaPage
    │   ├── Firma                 Avatar + Neondev (abre el repo) y la música que suena, en el login
    │   ├── MusicGlyphs.cs        Iconos de play/pausa y nota/silencio según el estado
    │   └── AvatarTones.cs        Tono del avatar de cada fila
    ├── Estilos/
    │   ├── Textos.xaml           Todos los textos visibles, en español de México
    │   ├── Dashboard.xaml        Reloj, fecha, estado de la sala y botón sin aspecto (lo mezcla DashboardPage)
    │   └── TarjetaMenu.xaml      Plantilla de TarjetaMenu con sus estados (normal, mouse encima, apagada)
    ├── ModelosDeVista/           ViewModels: Login (+ LoginStep, FormViewModel), Recovery, SecurityQuestion,
    │                             Dashboard, ConnectedUsers, UserRow, Emotes + EmoteRow,
    │                             MusicControls (base) → Musica y Firma, SongRow
    └── Pantallas/                Páginas del Frame: Login, Dashboard, Sala, Emotes, Musica, Manual
```

Los diccionarios de `Views/Estilos` que usan pinceles o fuentes de `App.xaml` se mezclan en los recursos de la página, no en `App.xaml`: un diccionario mezclado dentro de `App.xaml` no puede ver las claves del propio `App.xaml`.

## Archivos que guarda la app

En `%LocalAppData%\COT_0\`:

| Archivo | Qué guarda |
|---|---|
| `usuarios.json` | Cuenta del host (una sola): usuario, hash + salt de la contraseña, clave de la pregunta secreta y hash + salt de la respuesta, fecha. Nunca la contraseña ni la respuesta. |
| `visitas.json` | Una línea por cada invitado que entra a la sala, con `EnteredAt`. Queda aunque se cierre el túnel. |
| `sesiones.json` | Una entrada por cada vez que se abre la sala: `Number` (1, 2, 3…), `StartedAt`, `EndedAt` y `Guests` (nombre + hora de entrada). El host no cuenta. |

## Login y recuperación

- `LoginStep` decide qué se ve: `SignIn` (ENTRAR), `Register` (REGISTRARME + pregunta secreta), `Recover` (¿Olvidaste tu contraseña?) y `SetupQuestion` (cuenta vieja sin pregunta: se pide después de ENTRAR).
- La respuesta se normaliza antes de hashear y al comparar: se recorta, se quitan todos los espacios y pasa a minúsculas (`SecurityAnswerValidator.Normalize`).
- Las preguntas salen de `Recovery:QuestionKeys` (claves) y su texto de `Textos.xaml`. En `usuarios.json` se guarda la clave, no el texto.

## Sesiones: cómo conectarlas al migrar SalaPage

`SesionWatcher` ya está registrado en `App.BuildServices`, pero nadie lo pide todavía, así que no se crea.
Al migrar `SalaPage` (abrir sala + QR), basta con pedirlo una vez después de levantar la web, junto a `VisitWatcher`:

```csharp
Services.GetRequiredService<SesionWatcher>();
```

Desde ahí: `IRoomService.Changed` con la sala abierta → `SesionService.StartAsync()` (número = máximo + 1);
cada invitado nuevo → `SesionService.AddGuestAsync(nombre)`; sala cerrada → `SesionService.EndAsync()`.
Si la app se cierra de golpe con la sala abierta, esa sesión se queda con `EndedAt = null`.

## Carpetas que vienen después

Se crean al migrar cada pantalla (ver `diseno/inventario-winui.md`, fuera del repo):

- `Views/Componentes/` · faltan el QR de la sala y el selector de fondos
- `Assets/Fondos/`, `Assets/Musica/` · solo si algún fondo o canción tiene que ir dentro del proyecto (hoy viven fuera del repo, en `Paths:MediaRoot`)

## Música, emotes y firma

- `MusicService` es una sola instancia (Singleton): la firma del login (`FirmaViewModel`) y `MusicaPage` (`MusicaViewModel`) heredan de `MusicControlsViewModel` y leen el mismo reproductor, así que van sincronizadas.
- La música vive fuera del repo: `Paths:MediaRoot` + `Audio:MusicFolder` (canciones) y `Audio:CoversFolder` (portadas). La app no crea la carpeta; si falta, `MusicaPage` lo avisa con un InfoBar.
- `EmotesViewModel` usa `IRoomEmotes` de Core tal cual, sacado del contenedor de la web (misma instancia que los chats de los invitados). En el WPF los emotes eran un bloque de la sala; en WinUI son su propia pantalla.