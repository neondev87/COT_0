# Cumplimiento de lo que pidió el profe

Revisión del 08/11 en la rama `host-winui`, con búsquedas en todo el repo y `dotnet build COT_0.sln` (0 errores, 0 warnings).

| Requisito | Dónde se cumple | Estado |
|---|---|---|
| Host en WinUI 3, no WPF | `COT_0.Host/COT_0.Host.csproj` (`UseWinUI`, Windows App SDK 1.8); el WPF se borró | ✅ |
| Datos en JSON, no XML (las vistas XAML sí) | `Services/Almacenamiento/JsonStore.cs` (`usuarios.json`, `visitas.json`, `sesiones.json`, `fondos.json`), `Services/Registro/MediaLog.cs` (`registro-medios.jsonl`), `appsettings.json`. Sin `XmlSerializer` ni `.xml` de datos | ✅ |
| Usuario y contraseña: solo letras y números, sin espacios | `Services/Validaciones/CredentialValidator.cs` (`char.IsLetterOrDigit`; acepta letras con acento y ñ) | ✅ |
| Contraseña y respuesta secreta hasheadas | `Services/Usuarios/SecretHasher.cs` (PBKDF2 + SHA-256, salt, 100 000 iteraciones, comparación `FixedTimeEquals`); `Models/HostUser.cs` guarda solo hash + salt | ✅ |
| Carpetas: primer nivel en inglés, subcarpetas en español | Host: `Views`, `Models`, `Services`, `Assets`. Web: `Views`, `Models`, `Services` (+ `wwwroot` y `Properties`, que pide ASP.NET). Core: `Models`, `Services`. Ver `docs/MAPA_CARPETAS.md` | ✅ (excepciones anotadas) |
| Una clase por archivo | Se separaron 8 archivos con varios tipos (`ImageModels`, `VoiceModels`, `SevenTvDtos`, `ManualStep`, `MessageToken`, `ManualTimeline`, `EmoteSetStatus`, `RoomEmote`) | ✅ |
| Comentario `///` en español en cada archivo | Todos los `.cs` tienen `/// <summary>` arriba del tipo (se agregó en 122). `.xaml` y `.razor` llevan `<!-- -->` / `@* *@` | ✅ |
| async/await en disco | `AppConfig.LoadAsync` (se espera en `App.OnLaunched` antes de armar los servicios; `IConfiguration` sale de los mismos bytes), `JsonStore`, `FondoStore`, `MediaLog`, `FrameFiles`, `MusicLibrary`, `CoverLibrary`, `FondoCatalog`, `ManualAssets`. Todo `FileStream` se abre con `useAsync: true` (`Services/Almacenamiento/AsyncFiles.cs`). Sin `File.ReadAll*`/`WriteAll*` síncronos, `.Result`, `.Wait()` ni `GetResult()` | ✅ (excepciones abajo) |
| Documentación en español | `README.md`, `docs/ESTRUCTURA.md`, `docs/CAMBIOS_VISUALES.md`, `docs/PRUEBAS_MANUALES.md`, `docs/MAPA_CARPETAS.md`, `docs/REPORTE_CIERRE.md` | ✅ |
| App ligera | Ver abajo | ✅ |

## Incumplimientos y cosas a revisar

- **Lo que queda síncrono a propósito (async/await en disco)**:
  - `WebApp.Create` (`WebApplication.CreateBuilder`) lee el `appsettings.json` de la web con el proveedor de ASP.NET, que no tiene versión async; corre dentro de `Task.Run` en `WebServerHost`, fuera del hilo de la UI.
  - `File.Exists`, `Directory.Exists`, `Directory.CreateDirectory` y `Directory.EnumerateFiles` (`JsonStore.Folder`, `CacheFolders`, `FondoCatalog`, `MusicLibrary`, `CoverLibrary`, `ManualAssets`, `FfmpegFrames`): .NET no tiene versión async; son consultas de metadatos baratas y las pesadas ya van en `Task.Run`.
  - `IAvatarStore.Save` e `IVoiceStore.Save` (Web) se llaman `Save` pero guardan en memoria, no en disco.
- **Archivos de más de 200 líneas**: `COT_0.Host/App.xaml` (621, paleta y estilos comunes) y `Views/Estilos/Textos.xaml` (258, todos los textos). Partir `App.xaml` tiene un límite de WinUI: un diccionario mezclado en `App.xaml` no ve las claves del propio `App.xaml`.
- **Servicios sin interfaz en Core**: `UserService`, `VisitService`, `SesionService`, `PasswordRecoveryService` (estáticos) y los que usan tipos de WinUI (`MusicService`, `QrService`, `ConfirmDialogService`, `ManualSound`). Ver `CONTRATO.md`.

## App ligera

Quitado en la migración (no se rescató): shader `LumaKey`, estática por píxeles (`NoiseLayer`), golpes de estática (`SignalLossLayer`), `BlurEffect` (niebla del login y del manual), `DropShadowEffect` (botones, QR, avisos, firma), scanlines (`DrawingBrush`), ceniza del login (`AshLayer`) y las 5 franjas de interferencia del manual. El velo negro reemplaza al desenfoque de FONDOS.

Efectos que siguen vivos (todos con `Storyboard` en el compositor, sin redibujar píxeles):

- Login: la imagen flota, la niebla va y viene y una franja de luz baja (`LoginPage.xaml`).
- Logo Neondev: brillo de vidrio que va y viene (`LogoNeondev.xaml`).
- Punto rojo con pulso de sala activa (`PuntoPulso.xaml`).
- Manual: barrido tenue (14 s) y halo de los resaltados (`ManualPage.xaml`, `ResaltadoManual.xaml`).
- Fondo en video (un solo `MediaPlayer` en bucle y en silencio). Lo más pesado que queda; los videos de más de `Background:HeavyVideoMegabytes` arrancan como cuadro fijo.
- Web: ceniza del fondo con CSS (`AshLayer.razor`, `Chat:AshCount`) y animaciones CSS del manual del invitado.
