# Estructura de COT_0.HostWinUI

Host de COT_0 en WinUI 3. Una sola ventana con un `Frame`; cada pantalla es una `Page`.
Las carpetas de primer nivel van en inglés; las de adentro, en español y con el nombre de lo que hacen.

```
COT_0.HostWinUI/
├── App.xaml                      Paleta, fuentes, medidas y estilos de toda la app
├── App.xaml.cs                   Arranque: crea la ventana principal
├── MainWindow.xaml               Única ventana: solo un Frame (RootFrame)
├── appsettings.json              Tamaño de ventana, archivos JSON, seguridad y límites de validación
├── app.manifest                  Nitidez en pantallas con escala (DPI)
├── Assets/
│   ├── Fuentes/                  TTF del diseño (Cinzel, Barlow, Barlow Condensed)
│   └── Imagenes/                 Imágenes fijas de la interfaz (fondo ASCII del login)
├── Models/
│   └── Models.cs                 Clases simples que se guardan en JSON: HostUser, Visit, CredentialError
├── Services/
│   ├── Almacenamiento/           JsonStore: lee y guarda cualquier clase en %LocalAppData%\COT_0\*.json
│   ├── Configuracion/            AppConfig: carga appsettings.json una sola vez
│   ├── Usuarios/                 UserService: registro e inicio de sesión del host (PBKDF2 + salt)
│   ├── Validaciones/             CredentialValidator: solo letras y números, sin espacios ni símbolos
│   ├── Ventana/                  WindowSetup (tamaño y barra de título) y Navigator (cambia de página)
│   └── Visitas/                  VisitService (visitas.json) y VisitWatcher (detecta quién entra a la sala)
└── Views/
    ├── Estilos/                  Textos.xaml: todos los textos visibles, en español de México
    ├── ModelosDeVista/           ViewModels: estado y acciones de cada pantalla (LoginViewModel)
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

- `Views/Componentes/` · piezas reutilizables: lista de conectados, panel de emotes, selector de canciones, selector de fondos
- `Assets/Fondos/`, `Assets/Musica/` · solo si algún fondo o canción tiene que ir dentro del proyecto (hoy viven fuera del repo, en `Paths:MediaRoot`)
