# Reporte de cierre de la migración a WinUI 3

Rama `host-winui`, creada desde `winui-migration` (tag de respaldo `pre-cierre-winui` en `1d8ebc2`). Proyecto final: `COT_0.Core`, `COT_0.Web` y `COT_0.Host` (WinUI 3). Ya no queda nada del WPF.

| Paso | Commit |
|---|---|
| 0. Rama `host-winui` y tag `pre-cierre-winui` | — |
| 1. Comparación WPF → WinUI (anexo al final) | `4d8d387` |
| 2. Borrado del WPF y código muerto | `394274f` |
| 3. `COT_0.HostWinUI` → `COT_0.Host` y agregado a `COT_0.sln` | `44a8209` |
| 4. Carpetas (`Views`/`Services` con subcarpetas en español), `MAPA_CARPETAS.md` | `06a1655` |
| 5. Pedidos de `CONTRATO.md`, `IBackgroundCatalog` en Core y `.gitignore` de medios (`/musica/`…) | `8ded1d1` |
| 6. Una clase por archivo, comentarios `///` y `CUMPLIMIENTO.md` | `c0d3fe2` |
| 7. Documentación final (este archivo, `ESTRUCTURA.md`, `README.md`, `PENDIENTES_LIMPIEZA.md`) | último commit de la rama |

## DÓNDE QUEDÓ CADA COSA

Rutas relativas a `COT_0.Host/` salvo que digan otro proyecto. Las pantallas `X` son `X.xaml` + `X.xaml.cs`.

| Qué | Vista | Lógica |
|---|---|---|
| **Login** (registro, entrar, pregunta secreta, recuperación) | `Views/Pantallas/LoginPage`, `Views/Componentes/FormularioRecuperacion` | `Views/ModelosDeVista/LoginViewModel.cs`, `LoginStep.cs`, `FormViewModel.cs`, `RecoveryViewModel.cs`, `SecurityQuestionViewModel.cs`; `Services/Usuarios/UserService.cs`, `PasswordRecoveryService.cs`, `HostAccounts.cs`, `SecretHasher.cs`; `Services/Validaciones/CredentialValidator.cs`, `SecurityAnswerValidator.cs`; `Models/HostUser.cs`, `HashedSecret.cs`, `SecurityQuestionOption.cs`, `CredentialError.cs` |
| **Dashboard** | `Views/Pantallas/DashboardPage`, `Views/Componentes/TarjetaMenu.cs`, `Views/Estilos/Dashboard.xaml`, `Views/Estilos/TarjetaMenu.xaml` | `Views/ModelosDeVista/DashboardViewModel.cs`, `Services/Tiempo/ClockService.cs` |
| **Sala** (QR, túnel, conectados, cerrar) | `Views/Pantallas/SalaPage`, `Views/Componentes/ListaConectados`, `Views/Componentes/PuntoPulso`, `Views/Estilos/Sala.xaml` | `Views/ModelosDeVista/SalaViewModel.cs`, `ConnectedUsersViewModel.cs`, `UserRowViewModel.cs`; `Services/Sala/RoomSession.cs`, `WebServerHost.cs`, `QrService.cs`, `RoomOpenErrors.cs`, `GuestWatcher.cs`; `Services/Tunel/CloudflareTunnelService.cs`, `KillOnCloseJob.cs`; `Services/Ventana/CloseGuard.cs`; `Models/RoomError.cs`. Del lado web: `COT_0.Web/Services/Sala/` (`RoomService`, `InMemoryChatService`, `ChatHub`) |
| **Emotes** | `Views/Pantallas/EmotesPage`, `Views/Componentes/PanelEmotes`, `Views/Convertidores/EmoteStatusBrushes.cs` | `Views/ModelosDeVista/EmotesViewModel.cs`, `EmoteRowViewModel.cs`; `COT_0.Core/Services/IRoomEmotes.cs`; `COT_0.Web/Services/Emotes/RoomEmotes.cs`, `SevenTvClient.cs` |
| **Música** | `Views/Pantallas/MusicaPage`, `Views/Componentes/ControlVolumen`, `Views/Convertidores/MusicGlyphs.cs` | `Views/ModelosDeVista/MusicaViewModel.cs`, `MusicControlsViewModel.cs`, `SongRowViewModel.cs`; `Services/Musica/MusicService.cs`, `MusicLibrary.cs`, `CoverLibrary.cs`, `SongCatalog.cs`, `CoverImages.cs`; `Models/Song.cs`, `CoverArt.cs`, `MusicError.cs` |
| **Fondos** | `Views/Pantallas/FondosPage`, `Views/Componentes/CapaFondo` (en `MainWindow.xaml`) | `Views/ModelosDeVista/FondosViewModel.cs`, `FondoRowViewModel.cs`, `CapaFondoViewModel.cs`; `Services/Fondos/` (`FondoCatalog`, `FondoStore`, `FondoService`, `FondoImages`, `FondoPlayer`, `FondoFrameCache`, `FfmpegFrames`, `ShellThumbnailFrames`, `CompositionFrames`, `FrameFiles`, `FondoFailures`, `FondoNames`); `Services/Cache/CacheFolders.cs`; `Models/FondosGuardados.cs`, `FondoError.cs`; `COT_0.Core/Models/Fondo.cs`, `FondoKind.cs`, `COT_0.Core/Services/IBackgroundCatalog.cs` |
| **Manual de Lain** (host) | `Views/Pantallas/ManualPage`, `Views/Componentes/Manual/` (`AvisoLain`, `BotonManual`, `ResaltadoManual`, `EncabezadoEjemplo`, `VistaEjemplo*`), `Views/Estilos/Manual.xaml` | `Views/ModelosDeVista/ManualViewModel.cs`, `ManualStepViewModel.cs`, `ManualNoticeViewModel.cs`, `ManualDemoViewModel.cs`; `Services/Manual/ManualAssets.cs`, `ManualSound.cs`, `ManualVideo.cs`, `ManualTimeline.cs`; `Models/ManualDemoRow.cs`; `Services/Configuracion/ManualSettings.cs`, `ManualDemo*.cs` |
| Manual del invitado (`/manual`) | `COT_0.Web/Views/Pantallas/Manual.razor`, `COT_0.Web/Views/Componentes/Manual/` | `COT_0.Web/Services/Manual/` |
| **Firma de Neondev** (logo + volumen) | `Views/Componentes/Firma`, `Views/Componentes/LogoNeondev`, `Views/Componentes/ControlVolumen`; `Assets/Imagenes/neondev-avatar.png`, `Assets/Fuentes/Redaction50-400.ttf` | `Views/ModelosDeVista/FirmaViewModel.cs`, `LogoNeondevViewModel.cs`; `Services/Configuracion/BrandingSettings.cs` (`Branding:RepoUrl`) |
| **Sesiones** (`sesiones.json`) | — | `Services/Sesiones/SesionService.cs`, `SesionWatcher.cs`; `Models/SesionInvitados.cs` |
| **Visitas** (`visitas.json`) | — | `Services/Visitas/VisitService.cs`, `VisitWatcher.cs`; `Models/Visit.cs` |
| **JSON** (todo lo que se guarda) | — | `Services/Almacenamiento/JsonStore.cs` (lee/guarda en `%LocalAppData%\COT_0\`), `Services/Configuracion/StorageSettings.cs` (nombres de archivo), `Services/Fondos/FondoStore.cs` (`fondos.json`), `Services/Usuarios/HostAccounts.cs` (`usuarios.json`), `Services/Registro/MediaLog.cs` (`registro-medios.jsonl`) |
| Configuración y textos | `Views/Estilos/Textos.xaml` | `appsettings.json`, `Services/Configuracion/AppConfig.cs` + un record por sección, `Services/Textos/UiText.cs` |
| Ventana y navegación | `MainWindow.xaml`, `Views/Componentes/EncabezadoPantalla`, `Views/Componentes/ConfirmacionDialogo` | `App.xaml.cs`, `Services/Ventana/WindowSetup.cs`, `Navigator.cs`, `Services/Dialogos/ConfirmDialogService.cs` |

## QUÉ SE BORRÓ

- **Host WPF completo** (`COT_0.Host/` viejo, 142 archivos, ≈ 7 600 líneas) y su entrada en `COT_0.sln`: `LoginWindow`, `MainWindow`, `ConfirmWindow`, `ManualWindow`, `Views/` (9), `Controls/` (38: `LumaKeyEffect`, `ShaderCompiler`, `NoiseLayer`, `SignalLossLayer`, `SignatureBadge`, `SongPicker`, `EmotePanel`, `QrPanel`, `UserList`, `BackgroundPicker`, `BackgroundLayer`, `ManualStepView`…), `Themes/` (18), `ViewModels/` (16), `Services/` (25: `ConfigAuthService`, `DialogService`, `WindowNavigator`, `ScreenNavigator`, `ShutdownCoordinator`, `StartupTrace`, `TextResources`, `CoverDecoder`…), `Assets/` (23: fuentes sin uso y `Shaders/LumaKey.hlsl`), su `appsettings.json` (sección `Texts`, `Links:GitHub`, `Diagnostics`…) y el paquete TagLibSharp.
- **Duplicados** que el WPF obligaba a tener: `MusicLibrary`/`CoverLibrary` y `BackgroundCatalog`/`ManualAssets` del WPF (las copias de WinUI son las que quedan).
- **`COT_0.Core/Services/IAuthService.cs`**: nadie lo usaba fuera del WPF.
- **`COT_0.Web/wwwroot/emotes/.gitkeep`** y **`wwwroot/img/.gitkeep`**: carpeta sin uso desde 7TV y carpeta que ya tiene archivos.
- **`COT_0.Host/.gitignore`**: su excepción `!Services/Musica/` ya no hace falta con las reglas ancladas (`/musica/`) del `.gitignore` de la raíz.
- **Archivos con varios tipos**, separados (no borrados): `ImageModels.cs`, `VoiceModels.cs` y `SevenTvDtos.cs` se partieron en un archivo por clase.
- **`docs/DOCUMENTACION.md`**: se movió a `README.md` (en la raíz) y se actualizó; describía el Host como WPF.

## QUÉ FALTÓ O ESTÁ INCOMPLETO

Lo que el WPF hacía y ahora no:

- **SUBIR MÚSICA** (copiar canciones a la carpeta desde la firma): no existe; necesita `FileOpenPicker` con el HWND.
- **`StartupTrace`** (`Diagnostics:*`, log de tiempos de arranque, apagado por defecto): no se migró.
- **Login por user-secrets** (`Host:User` / `Host:Password`): reemplazado por registro con `usuarios.json` (es un cambio, no una falta).

Simplificado o desactivado a propósito (detalle en `CAMBIOS_VISUALES.md`):

- Efectos pesados quitados: shader `LumaKey`, estática por píxeles (`NoiseLayer`), golpes de estática (`SignalLossLayer`), `BlurEffect` (niebla del login y del manual), `DropShadowEffect` (brillo de botones, QR, avisos, firma), scanlines (`DrawingBrush`), ceniza del login (`AshLayer`) y las 5 franjas de interferencia del manual.
- Emotes: ya no son un bloque de la sala, sino su propia pantalla.
- Duración de canciones sin TagLibSharp (`GetMusicPropertiesAsync`).
- Réplicas del manual sin animaciones de entrada ni firma.
- Kestrel no se apaga al cerrar la sala, solo al cerrar la ventana (con la sala cerrada `/join` no deja entrar).

No se pudo verificar en el cierre (solo build, arranque y lectura de código):

- Túnel real de cloudflared y QR escaneado desde un celular.
- Que la música, los videos de fondo y los sonidos del manual suenen y se vean (dependen de los archivos de `Paths:MediaRoot`).
- El aspecto visual de cada pantalla, escalas de DPI y tamaños de ventana.
- El flujo completo de `PRUEBAS_MANUALES.md`.

## QUÉ SE DEBE AGREGAR

Por importancia:

1. **Correr completo `PRUEBAS_MANUALES.md`** en la app final antes de entregar (túnel, QR, chat desde el celular, música, fondos, manual).
2. **Interfaces en Core para los servicios del Host** que hoy son estáticos o concretos (`UserService`, `VisitService`, `SesionService`, `PasswordRecoveryService`, y la parte sin vista de `QrService`/`MusicService`), para cumplir "todo servicio tiene interfaz en Core" (propuestas en `CONTRATO.md`).
3. **`AppConfig` async** (hoy lee `appsettings.json` con `File.ReadAllText` una vez al arrancar) y `FileStream` con `useAsync: true` en `JsonStore`/`FrameFiles`.
4. **Partir `App.xaml` (621 líneas) y `Textos.xaml` (258)** en diccionarios por pantalla, respetando el límite de WinUI (un diccionario mezclado en `App.xaml` no ve las claves de `App.xaml`).
5. **SUBIR MÚSICA** en `MusicaPage` con `FileOpenPicker`.
6. **Pasar voz e imagen a `ChatMessage.Kind`** en el render y el hub (hoy se reconocen por texto reservado).
7. **Limpieza automática** de `cache\fondos` (JPEG de videos que ya no existen) y tope o rotación de `registro-medios.jsonl`.
8. Opcional: apagar Kestrel también al cerrar la sala; capturas reales del manual (`manual\pasos\pasoN.png`).

## CÓMO CORRERLO Y VOLVER ATRÁS

Correr la app final:

```
dotnet build COT_0.sln
dotnet run --project COT_0.Host
```

(o el `.exe` en `COT_0.Host/bin/x64/Debug/net8.0-windows10.0.19041.0/win-x64/COT_0.Host.exe`). Requisitos y carpetas de medios en `README.md`.

Volver al estado anterior al cierre (con el WPF todavía en el repo):

```
git checkout pre-cierre-winui          # tag, estado exacto antes del cierre (modo "detached")
git checkout winui-migration           # o la rama de la migración
git checkout -b respaldo pre-cierre-winui   # si quieres trabajar sobre ese estado
```

En ese estado el WPF es `COT_0.Host` y el WinUI es `COT_0.HostWinUI`; el login del WPF usa los user-secrets `Host:User` / `Host:Password`, que siguen en esta PC.

## Anexo: comparación WPF → WinUI antes de borrar (histórico)

Se comparó `COT_0.Host` (WPF) contra el Host WinUI (entonces en otra carpeta) en pantallas, servicios, assets y configuración. **No hizo falta copiar nada**: todo lo que el WPF usaba y sigue siendo necesario ya existe en WinUI.

### Pantallas

| WPF | WinUI |
|---|---|
| `LoginWindow` | `Views/Pantallas/LoginPage` |
| `MainWindow` + `DashboardView` | `MainWindow` (Frame) + `Views/Pantallas/DashboardPage` |
| `RoomView`, `QrPanel`, `UserList` | `SalaPage`, `ListaConectados` |
| `EmotePanel` (bloque de la sala) | `EmotesPage` + `PanelEmotes` (pantalla propia) |
| `SongPicker`, `SignatureBadge` | `MusicaPage`, `Firma` + `LogoNeondev` + `ControlVolumen` |
| `BackgroundPicker`, `BackgroundLayer` | `FondosPage`, `CapaFondo` |
| `ManualWindow`, `ManualStepView`, `ManualFarewellView`, `ManualStages` | `ManualPage` + `VistaEjemplo*` + `ResaltadoManual` |
| `ConfirmWindow`, `ConfirmPanel` | `ConfirmacionDialogo` (`ContentDialog`) |

### Servicios

| WPF | WinUI |
|---|---|
| `ConfigAuthService` (`IAuthService`, user-secrets) | `UserService` + `HostAccounts` + `SecretHasher` (`usuarios.json`, PBKDF2) |
| `CloudflareTunnelService`, `KillOnCloseJob`, `QrService`, `WebServerHost`, `RoomSession` | Mismos nombres en `Services/Tunel` y `Services/Sala` |
| `MusicService`, `MusicLibrary`, `CoverLibrary`, `SongCatalog`, `CoverDecoder` (TagLibSharp) | `Services/Musica` (`SongCatalog` lee duración y portada sin TagLibSharp) |
| `BackgroundCatalog` | `Services/Fondos/FondoCatalog` (+ `FondoStore`, `FondoService`, …) |
| `ManualAssets`, `ManualSound`, `ManualDemoFactory` | `Services/Manual` + `ManualDemoViewModel` (datos de `Manual:Demo`) |
| `DialogService` | `Services/Dialogos/ConfirmDialogService` |
| `WindowNavigator`, `ScreenNavigator` | `Services/Ventana/Navigator` |
| `ShutdownCoordinator`, `IShutdownCoordinator` | `Services/Ventana/CloseGuard` + `RoomSession.ShutdownAsync` |
| `TextResources`, `ConfigurationExtensions.GetText` (sección `Texts`) | `Services/Textos/UiText` + `Views/Estilos/Textos.xaml` |
| `ConfigurationExtensions.GetMediaPath` / `GetUiCulture` | `Services/Configuracion/AppConfig` |
| `ClockService`, `UiThread` | `Services/Tiempo` |
| `StartupTrace` (`Diagnostics:*`) | **No migrado** (diagnóstico de arranque, apagado por defecto) |

### Assets

- Fuentes: todas las que referencia el XAML de WinUI existen en `Assets/Fuentes`. Las del WPF que WinUI no usa (`Barlow-600`, `BarlowCondensed-700Italic`, `Cinzel-400`, `CormorantGaramond-*`, `JetBrainsMono-*`, `Redaction50-700`) no se copian: ninguna vista las pide (la sala usa `Consolas` como fuente mono).
- Imágenes: `ascii-bg.png` y `neondev-avatar.png` ya están en `Assets/Imagenes`.
- Sonidos y videos: nunca estuvieron en el repo (viven en `Paths:MediaRoot`).
- `Assets/Shaders/LumaKey.hlsl`: **no se rescata** (efecto pesado quitado a propósito, igual que la estática por píxeles y el desenfoque).

### Configuración (claves que solo tiene el WPF)

| Clave | Motivo para no copiarla |
|---|---|
| `Host:User`, `Host:Password` | Login viejo por user-secrets; WinUI usa `usuarios.json` |
| `Links:GitHub` | Reemplazada por `Branding:RepoUrl` |
| `Background:PreviewSeconds` | Reemplazada por `Background:FrameSeconds` |
| `Manual:StaticHitVolume` | Golpes de estática quitados con `SignalLossLayer` |
| `Diagnostics:*` | `StartupTrace` no se migró |
| `Texts:*` (110 claves) | Los textos viven en `Textos.xaml` |

