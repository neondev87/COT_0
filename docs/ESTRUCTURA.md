# Estructura de COT_0.HostWinUI

Host de COT_0 en WinUI 3. Una sola ventana con un `Frame`; cada pantalla es una `Page`.
Las carpetas de primer nivel van en inglés; las de adentro, en español y con el nombre de lo que hacen.

```
COT_0.HostWinUI/
├── App.xaml                      Paleta, fuentes, medidas y estilos de toda la app
├── App.xaml.cs                   Arranque: arma los servicios, crea la ventana y levanta la web
├── MainWindow.xaml               Única ventana: solo un Frame (RootFrame)
├── appsettings.json              Ventana, archivos JSON, seguridad, validación y la config de la web/túnel
├── app.manifest                  Nitidez en pantallas con escala (DPI)
├── Assets/
│   ├── Fuentes/                  TTF del diseño (Cinzel, Barlow, Barlow Condensed, Inter)
│   └── Imagenes/                 Imágenes fijas de la interfaz (fondo ASCII del login)
├── Models/
│   └── Models.cs                 Clases simples: HostUser, Visit, CredentialError, ConfirmTexts
├── Services/
│   ├── Almacenamiento/           JsonStore: lee y guarda cualquier clase en %LocalAppData%\COT_0\*.json
│   ├── Configuracion/            AppConfig: carga appsettings.json una sola vez
│   ├── Dialogos/                 ConfirmDialogService: abre el diálogo sí/no sobre la ventana
│   ├── Sala/                     WebServerHost (Kestrel con COT_0.Web) y RoomSession (abrir y cerrar la sala)
│   ├── Textos/                   UiText: busca un texto de Textos.xaml desde C#
│   ├── Tiempo/                   ClockService (tick de cada segundo) y UiThread (pasa eventos al hilo de la UI)
│   ├── Tunel/                    CloudflareTunnelService y KillOnCloseJob (cloudflared muere con la app)
│   ├── Usuarios/                 UserService: registro e inicio de sesión del host (PBKDF2 + salt)
│   ├── Validaciones/             CredentialValidator: solo letras y números, sin espacios ni símbolos
│   ├── Ventana/                  WindowSetup (tamaño y barra de título) y Navigator (cambia de página)
│   └── Visitas/                  VisitService (visitas.json) y VisitWatcher (detecta quién entra a la sala)
└── Views/
    ├── Componentes/              Piezas reutilizables: ConfirmacionDialogo, ListaConectados, AvatarTones
    ├── Estilos/                  Textos.xaml: todos los textos visibles, en español de México
    ├── ModelosDeVista/           ViewModels: Login, ConnectedUsers y UserRow (fila de un invitado)
    └── Pantallas/                Páginas del Frame: Login, Dashboard, Sala, Manual
```

## Archivos que guarda la app

En `%LocalAppData%\COT_0\`:

| Archivo | Qué guarda |
|---|---|
| `usuarios.json` | Cuenta del host: usuario, hash de la contraseña, salt y fecha. Nunca la contraseña. |
| `visitas.json` | Una línea por cada invitado que entra a la sala, con `EnteredAt`. Queda aunque se cierre el túnel. |

## Carpetas que vienen después

Se crean al migrar cada pantalla (ver `diseno/inventario-winui.md`, fuera del repo):

- `Views/Componentes/` · faltan el panel de emotes, el selector de canciones y el selector de fondos
- `Assets/Fondos/`, `Assets/Musica/` · solo si algún fondo o canción tiene que ir dentro del proyecto (hoy viven fuera del repo, en `Paths:MediaRoot`)
