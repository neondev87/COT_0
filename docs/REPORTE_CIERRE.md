# Reporte de cierre de la migración a WinUI 3

Rama `host-winui`, creada desde `winui-migration` (tag de respaldo `pre-cierre-winui` en `1d8ebc2`).

## Paso 1: comparación WPF → WinUI antes de borrar

Se comparó `COT_0.Host` (WPF) contra `COT_0.HostWinUI` en pantallas, servicios, assets y configuración. **No hizo falta copiar nada**: todo lo que el WPF usaba y sigue siendo necesario ya existe en WinUI.

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
