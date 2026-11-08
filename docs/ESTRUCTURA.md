# Estructura de COT_0.HostWinUI

Host de COT_0 en WinUI 3. Una sola ventana con un `Frame`; cada pantalla es una `Page`.
Las carpetas de primer nivel van en inglés; las de adentro, en español y con el nombre de lo que hacen.
Una clase por archivo.

```
COT_0.HostWinUI/
├── App.xaml                      Paleta, fuentes, medidas y estilos comunes (login, botones, lista de conectados)
├── App.xaml.cs                   Arranque: arma los servicios, crea la ventana y levanta la web
├── MainWindow.xaml               Única ventana: solo un Frame (RootFrame)
├── appsettings.json              Ventana, archivos JSON, seguridad, validación, preguntas secretas, reloj y la config de la web/túnel
├── app.manifest                  Nitidez en pantallas con escala (DPI)
├── Assets/
│   ├── Fuentes/                  TTF del diseño (Cinzel, Barlow, Barlow Condensed 200/300/500/700/800 itálica, Inter)
│   └── Imagenes/                 Imágenes fijas de la interfaz (fondo ASCII del login)
├── Models/                       Clases simples que se guardan en JSON o viajan entre capas (sin lógica)
│   ├── HostUser.cs               Cuenta del host: hash + salt de la contraseña y de la respuesta secreta
│   ├── Visit.cs                  Nombre y hora de entrada de un invitado (visitas y sesiones)
│   ├── SesionInvitados.cs        Sesión numerada: inicio, fin e invitados que entraron
│   ├── HashedSecret.cs           Hash y salt en Base64
│   ├── SecurityQuestionOption.cs Clave y texto de una pregunta secreta
│   ├── CredentialError.cs        Por qué se rechazó lo que escribiste (cada valor tiene su Text.Error.*)
│   └── ConfirmTexts.cs           Textos del diálogo sí/no
├── Services/
│   ├── Almacenamiento/           JsonStore: lee y guarda cualquier clase en %LocalAppData%\COT_0\*.json (async)
│   ├── Configuracion/            AppConfig + un record por sección de appsettings.json (Window, Storage, Security, Validation, Recovery, Ui)
│   ├── Dialogos/                 ConfirmDialogService: abre el diálogo sí/no sobre la ventana
│   ├── Sala/                     WebServerHost (Kestrel con COT_0.Web), RoomSession (abrir y cerrar la sala) y GuestWatcher (base: detecta quién entra)
│   ├── Sesiones/                 SesionService (sesiones.json) y SesionWatcher (abre/cierra la sesión con la sala y anota invitados)
│   ├── Textos/                   UiText: busca un texto de Textos.xaml desde C#
│   ├── Tiempo/                   ClockService (tick de cada segundo) y UiThread (pasa eventos al hilo de la UI)
│   ├── Tunel/                    CloudflareTunnelService y KillOnCloseJob (cloudflared muere con la app)
│   ├── Usuarios/                 UserService (registro/login), PasswordRecoveryService (¿olvidaste tu contraseña?),
│   │                             HostAccounts (usuarios.json) y SecretHasher (PBKDF2 + SHA-256 con salt)
│   ├── Validaciones/             CredentialValidator (solo letras y números) y SecurityAnswerValidator (normaliza la respuesta)
│   ├── Ventana/                  WindowSetup (tamaño y barra de título) y Navigator (cambia de página)
│   └── Visitas/                  VisitService (visitas.json) y VisitWatcher (anota a cada invitado que entra)
└── Views/
    ├── Componentes/              Piezas reutilizables
    │   ├── ConfirmacionDialogo   ContentDialog sí/no
    │   ├── ListaConectados       Invitados conectados (ListView)
    │   ├── FormularioRecuperacion  Pregunta secreta → contraseña nueva, dentro del login
    │   ├── TarjetaMenu.cs        Tarjeta inclinada del menú (Button con número, subtítulo e icono)
    │   ├── PuntoPulso            Punto rojo con pulso (sala activa)
    │   └── AvatarTones.cs        Tono del avatar de cada fila
    ├── Estilos/
    │   ├── Textos.xaml           Todos los textos visibles, en español de México
    │   ├── Dashboard.xaml        Reloj, fecha, estado de la sala y botón sin aspecto (lo mezcla DashboardPage)
    │   └── TarjetaMenu.xaml      Plantilla de TarjetaMenu con sus estados (normal, mouse encima, apagada)
    ├── ModelosDeVista/           ViewModels: Login (+ LoginStep, FormViewModel), Recovery, SecurityQuestion,
    │                             Dashboard, ConnectedUsers y UserRow
    └── Pantallas/                Páginas del Frame: Login, Dashboard, Sala, Manual
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

- `Views/Componentes/` · faltan el QR de la sala, el panel de emotes, el selector de canciones, la firma y el selector de fondos
- `Assets/Fondos/`, `Assets/Musica/` · solo si algún fondo o canción tiene que ir dentro del proyecto (hoy viven fuera del repo, en `Paths:MediaRoot`)
