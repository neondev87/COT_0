# Reporte de cierre de la migración a WinUI 3

Rama `host-winui`, creada desde `winui-migration` (tag de respaldo `pre-cierre-winui` en `1d8ebc2`). Proyecto final: `COT_0.Core`, `COT_0.Web` y `COT_0.Host` (WinUI 3). Ya no queda nada del WPF.

| Paso | Commit |
|---|---|
| 0. Rama `host-winui` y tag `pre-cierre-winui` | — |
| 1. Comparación WPF → WinUI (anexo al final) | `4d8d387` |
| 2. Borrado del WPF y código muerto | `394274f` |
| 3. Proyecto WinUI renombrado a `COT_0.Host` y agregado a `COT_0.sln` | `44a8209` |
| 4. Carpetas (`Views`/`Services` con subcarpetas en español), `MAPA_CARPETAS.md` | `06a1655` |
| 5. Pedidos de `CONTRATO.md`, `IBackgroundCatalog` en Core y `.gitignore` de medios (`/musica/`…) | `8ded1d1` |
| 6. Una clase por archivo, comentarios `///` y `CUMPLIMIENTO.md` | `c0d3fe2` |
| 7. Documentación final (este archivo, `ESTRUCTURA.md`, `README.md`, `PENDIENTES_LIMPIEZA.md`) | último commit de la rama |

## DÓNDE QUEDÓ CADA COSA

Rutas relativas a `COT_0.Host/` salvo que digan otro proyecto. Las pantallas `X` son `X.xaml` + `X.xaml.cs`.

| Qué | Vista | Lógica |
|---|---|---|
| **Login** (hasta 20 usuarios: entrar, registrar usuario, pregunta secreta, recuperación por usuario) | `Views/Pantallas/LoginPage`, `Views/Componentes/FormularioRegistro`, `Views/Componentes/FormularioRecuperacion` | `Views/ModelosDeVista/LoginViewModel.cs`, `LoginStep.cs`, `FormViewModel.cs`, `RegisterViewModel.cs`, `RecoveryViewModel.cs`, `SecurityQuestionViewModel.cs`; `Services/Usuarios/UserService.cs`, `PasswordRecoveryService.cs`, `HostAccounts.cs`, `SecretHasher.cs`; `Services/Validaciones/CredentialValidator.cs`, `SecurityAnswerValidator.cs`; `Models/HostUser.cs`, `HashedSecret.cs`, `SecurityQuestionOption.cs`, `CredentialError.cs`; `Services/Configuracion/UsuariosSettings.cs` (`Usuarios:Maximo`) |
| **Dashboard** | `Views/Pantallas/DashboardPage`, `Views/Componentes/TarjetaMenu.cs`, `Views/Estilos/Dashboard.xaml`, `Views/Estilos/TarjetaMenu.xaml` | `Views/ModelosDeVista/DashboardViewModel.cs`, `Services/Tiempo/ClockService.cs` |
| **Sala** (QR, túnel, conectados, cerrar) | `Views/Pantallas/SalaPage`, `Views/Componentes/ListaConectados`, `Views/Componentes/PuntoPulso`, `Views/Estilos/Sala.xaml` | `Views/ModelosDeVista/SalaViewModel.cs`, `ConnectedUsersViewModel.cs`, `UserRowViewModel.cs`; `Services/Sala/RoomSession.cs`, `WebServerHost.cs`, `QrService.cs`, `RoomOpenErrors.cs`, `GuestWatcher.cs`; `Services/Tunel/CloudflareTunnelService.cs`, `KillOnCloseJob.cs`; `Services/Ventana/CloseGuard.cs`; `Models/RoomError.cs`. Del lado web: `COT_0.Web/Services/Sala/` (`RoomService`, `InMemoryChatService`, `ChatHub`) |
| **Emotes** | `Views/Pantallas/EmotesPage`, `Views/Componentes/PanelEmotes`, `Views/Convertidores/EmoteStatusBrushes.cs` | `Views/ModelosDeVista/EmotesViewModel.cs`, `EmoteRowViewModel.cs`; `COT_0.Core/Services/IRoomEmotes.cs`; `COT_0.Web/Services/Emotes/RoomEmotes.cs`, `SevenTvClient.cs` |
| **Música** | `Views/Pantallas/MusicaPage`, `Views/Componentes/ControlVolumen`, `Views/Convertidores/MusicGlyphs.cs` | `Views/ModelosDeVista/MusicaViewModel.cs`, `MusicControlsViewModel.cs`, `SongRowViewModel.cs`; `Services/Musica/MusicService.cs`, `MusicLibrary.cs`, `CoverLibrary.cs`, `SongCatalog.cs`, `CoverImages.cs`; `Models/Song.cs`, `CoverArt.cs`, `MusicError.cs` |
| **Fondos** | `Views/Pantallas/FondosPage`, `Views/Componentes/CapaFondo` (en `MainWindow.xaml`) | `Views/ModelosDeVista/FondosViewModel.cs`, `FondoRowViewModel.cs`, `CapaFondoViewModel.cs`; `Services/Fondos/` (`FondoCatalog`, `FondoStore`, `FondoService`, `FondoImages`, `FondoPlayer`, `FondoFrameCache`, `FfmpegFrames`, `ShellThumbnailFrames`, `CompositionFrames`, `FrameFiles`, `FondoFailures`, `FondoNames`); `Services/Cache/CacheFolders.cs`; `Models/FondosGuardados.cs`, `FondoError.cs`; `COT_0.Core/Models/Fondo.cs`, `FondoKind.cs`, `COT_0.Core/Services/IBackgroundCatalog.cs` |
| **Manual de Lain** (host) | `Views/Pantallas/ManualPage`, `Views/Componentes/Manual/` (`AvisoLain`, `BotonManual`, `ResaltadoManual`, `EncabezadoEjemplo`, `VistaEjemplo*`), `Views/Estilos/Manual.xaml` | `Views/ModelosDeVista/ManualViewModel.cs`, `ManualStepViewModel.cs`, `ManualNoticeViewModel.cs`, `ManualDemoViewModel.cs`; `Services/Manual/ManualAssets.cs`, `ManualSound.cs`, `ManualVideo.cs`, `ManualTimeline.cs`; `Models/ManualDemoRow.cs`; `Services/Configuracion/ManualSettings.cs`, `ManualDemo*.cs` |
| Manual del invitado (`/manual`) | `COT_0.Web/Views/Pantallas/Manual.razor`, `COT_0.Web/Views/Componentes/Manual/` | `COT_0.Web/Services/Manual/` |
| **Firma de Neondev** (logo + volumen) | `Views/Componentes/Firma`, `Views/Componentes/LogoNeondev`, `Views/Componentes/BotonBocina.cs`, `Views/Estilos/Volumen.xaml`; `Assets/Imagenes/neondev-avatar.png`, `Assets/Fuentes/Redaction50-400.ttf` | `Views/ModelosDeVista/FirmaViewModel.cs`, `MusicControlsViewModel.cs` (volumen y silencio), `LogoNeondevViewModel.cs`; `Views/Convertidores/MusicGlyphs.cs` (bocina por rangos); `Services/Configuracion/BrandingSettings.cs` (`Branding:RepoUrl`) |
| **Sesiones** (`sesiones.json`) | — | `Services/Sesiones/SesionService.cs`, `SesionWatcher.cs`; `Models/SesionInvitados.cs` |
| **Visitas** (`visitas.json`) | — | `Services/Visitas/VisitService.cs`, `VisitWatcher.cs`; `Models/Visit.cs` |
| **JSON** (todo lo que se guarda) | — | `Services/Almacenamiento/JsonStore.cs` (lee/guarda en `%LocalAppData%\COT_0\`), `Services/Configuracion/StorageSettings.cs` (nombres de archivo), `Services/Fondos/FondoStore.cs` (`fondos.json`), `Services/Usuarios/HostAccounts.cs` (`usuarios.json`), `Services/Registro/MediaLog.cs` (`registro-medios.jsonl`) |
| Configuración y textos | `Views/Estilos/Textos.xaml` | `appsettings.json`, `Services/Configuracion/AppConfig.cs` + un record por sección, `Services/Textos/UiText.cs` |
| Límite de caracteres (`Inputs`) | `MaxLength` en `LoginPage`, `FormularioRegistro`, `FormularioRecuperacion` y `PanelEmotes` | `Services/Configuracion/InputsSettings.cs` (reemplaza a `ValidationSettings.cs`), `Services/Validaciones/InputLimits.cs`, `CredentialValidator.cs`, `SecurityAnswerValidator.cs` |
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
3. **Partir `App.xaml` (621 líneas) y `Textos.xaml` (258)** en diccionarios por pantalla, respetando el límite de WinUI (un diccionario mezclado en `App.xaml` no ve las claves de `App.xaml`).
4. **SUBIR MÚSICA** en `MusicaPage` con `FileOpenPicker`.
5. **Pasar voz e imagen a `ChatMessage.Kind`** en el render y el hub (hoy se reconocen por texto reservado).
6. **Limpieza automática** de `cache\fondos` (JPEG de videos que ya no existen) y tope o rotación de `registro-medios.jsonl`.
7. Opcional: apagar Kestrel también al cerrar la sala; capturas reales del manual (`manual\pasos\pasoN.png`).

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

En ese estado el WPF es `COT_0.Host` y el WinUI está en su propia carpeta, con su nombre de entonces; el login del WPF usa los user-secrets `Host:User` / `Host:Password`, que siguen en esta PC.

## Verificación final (08/11)

- `dotnet build COT_0.sln --no-incremental`: **0 errores, 0 warnings**.
- `COT_0.Host.exe` lanzado 8 s: proceso vivo, ventana "COT_0" abierta y Kestrel escuchando en el puerto 5050. La ventana arranca en `LoginPage` (`MainWindow.xaml.cs` → `Navigator.ShowLogin()`); no se tomó captura, así que el aspecto no se revisó.
- Búsqueda en todo el repo: cero "HostWinUI"; "WPF" solo en notas históricas de `docs/` y `CONTRATO.md` (y la regla `*_wpftmp.csproj` estándar del `.gitignore`); rutas viejas (`Pages/`, `Hubs/`, `Options/`, `Controls/`…) solo en la tabla de movimientos de `MAPA_CARPETAS.md` y en el registro histórico.
- `git check-ignore`: ignorados `musica/`, `fondos/`, `portadas/`, `gifs/`, `manual/` (y `manual/pasos/`), sus copias dentro de `COT_0.Host/`, `COT_0.Host/cache/` y `*.jsonl`. **No** ignorados: `COT_0.Host/Services/Musica`, `Services/Fondos`, `Services/Manual`, `Views/Componentes/Manual`, `COT_0.Web/Services/Manual` y `wwwroot/img/flags/musica.png`.

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

## Retoques finales (08/11): manual, volumen y límite de caracteres

- Archivos nuevos: `Views/Componentes/BotonBocina.cs`, `Views/Estilos/Volumen.xaml`, `Services/Validaciones/InputLimits.cs`. `Services/Configuracion/ValidationSettings.cs` pasó a `InputsSettings.cs` (sección `Validation` → `Inputs`).
- `dotnet build COT_0.sln`: 0 errores, 0 warnings. El `.exe` se lanzó unos segundos para confirmar que abre; sin capturas, así que el aspecto del manual y de la barra no se revisó a ojo (ver `PRUEBAS_MANUALES.md`, sección 12).

## Cierre final (08/11): varios usuarios, async en disco y nombres naturales

| Tarea | Commit | Archivos |
|---|---|---|
| A. Login con hasta 5 usuarios | `3def19d` | Nuevos: `Services/Configuracion/UsuariosSettings.cs`, `Views/ModelosDeVista/RegisterViewModel.cs`, `Views/Componentes/FormularioRegistro.xaml(.cs)`. Cambiados: `appsettings.json` (`Usuarios:Maximo`), `AppSettings.cs`, `Models/HostUser.cs`, `Models/CredentialError.cs` (`UserTaken`, `UsersFull`), `Services/Usuarios/HostAccounts.cs`, `UserService.cs`, `PasswordRecoveryService.cs`, `Services/Validaciones/CredentialValidator.cs`, `Views/ModelosDeVista/LoginViewModel.cs`, `LoginStep.cs`, `FormViewModel.cs`, `RecoveryViewModel.cs`, `Views/Pantallas/LoginPage.xaml`, `Views/Componentes/FormularioRecuperacion.xaml`, `Views/Estilos/Textos.xaml`, `App.xaml` (`LoginLinkSeparator`) |
| B. Async en todo acceso a disco | `72e6319` | Sin cambios de código: ya estaba resuelto en `b5b5824` (`AppConfig.LoadAsync`, `AsyncFiles`). Se reverificó con búsquedas y se anotó en `CUMPLIMIENTO.md` |
| C. Nombres naturales | `992830b` | Tag `pre-nombres-naturales`. El proyecto ya era `COT_0.Host`; no había carpetas, archivos, clases ni recursos con sufijos de migración. Se limpiaron los comentarios de código que decían "WinUI" (12 archivos) y se agregó "Renombres de cierre" en `MAPA_CARPETAS.md` |
| Documentación | último commit | `CUMPLIMIENTO.md`, `ESTRUCTURA.md`, `MAPA_CARPETAS.md`, este archivo, `README.md`, `CAMBIOS_VISUALES.md`, `PRUEBAS_MANUALES.md`, `PENDIENTES_LIMPIEZA.md` |

- Lo único fuera del login que suponía una sola cuenta eran textos: el README ("CREA TU CUENTA DE HOST") y la descripción de `usuarios.json` en `ESTRUCTURA.md`; se actualizaron. Ningún servicio de adentro (Dashboard, Sala, Emotes, Música, Fondos, Manual, firma, `SesionService`, `VisitService`) leía la cuenta.
- Volver atrás: `git checkout pre-nombres-naturales` (antes de la Tarea C) o `git checkout 648df7a` (antes de las tres tareas).
- Verificación: `dotnet build COT_0.sln` en 0 errores y 0 warnings; `COT_0.Host.exe` lanzado 8 s: ventana "COT_0" abierta, respondiendo y Kestrel en el puerto 5050. Sin capturas: el aspecto de los enlaces y del formulario de registro no se revisó a ojo (`PRUEBAS_MANUALES.md`, secciones 1, 2 y 2b).


## Manual limpio y sin letreros (08/11, noche)

| Tarea | Commit | Archivos |
|---|---|---|
| 1. Diagnóstico de capas | `c53a3bd` | Nuevo: `docs/qa/CAPAS_MANUAL.md` |
| 2. Fondo con tres capas | `86a72ef` | `Views/ModelosDeVista/CapaFondoViewModel.cs` (`IsDecorVisible`, sin velo propio en el Manual), `Views/Componentes/CapaFondo.xaml`, `Views/Estilos/Manual.xaml` (`ManualVeilBrush` plano al 60 %; fuera `ManualSweepBrush` y `ManualFloorBrush`), `Views/Pantallas/ManualPage.xaml(.cs)` (fuera barrido y piso) |
| 3. Lain transparente (opción A + respaldo B) | `50da859` | Nuevo: `docs/PREPARAR_LAIN.md`. Cambiados: `appsettings.json` (`Manual:LainCutout`), `Services/Configuracion/ManualSettings.cs`, `Services/Manual/ManualAssets.cs`, `Services/Fondos/FondoImages.cs` (ancho opcional), `Views/ModelosDeVista/ManualViewModel.cs`, `ManualPage.xaml`, `Manual.xaml` (brochas `Edge*`), `.gitignore` |
| 4. Despedida sin rectángulo duro | `824db43` | `ManualPage.xaml` (`FarewellFades` y franjas), `Manual.xaml` |
| 5. Sin letreros de atajos | `e3210d2` | Nuevo: `Services/Ventana/ShortcutHints.cs`. Cambiados: `Services/Ventana/Navigator.cs`, `App.xaml` (estilo implícito de `Button`, `TextButton`, `SolidButton`), `Manual.xaml` (`BotonManualStyle`), `ManualPage.xaml`, `Views/Componentes/EncabezadoPantalla.xaml`, `ListaConectados.xaml`, `Views/Pantallas/FondosPage.xaml`, `MusicaPage.xaml` |
| 6. Verificación y documentación | último commit | `PRUEBAS_MANUALES.md` (10b), este archivo, `PENDIENTES_LIMPIEZA.md` |

- Verificación: `dotnet build COT_0.sln` en 0 errores y 0 warnings; `COT_0.Host.exe` lanzado 8 s: ventana "COT_0" abierta y respondiendo. Sin capturas de pantalla: el GIF se revisó extrayendo cuadros con ffmpeg (alfa en los 93 cuadros, sin halo sobre fondo rojo) y la composición del Manual (velo + Lain, velo + despedida fundida) se simuló con ffmpeg sobre un cuadro de `Batman.mp4`. El aspecto en la app real queda para `PRUEBAS_MANUALES.md`, sección 10b.
- Pendiente de confirmar a ojo: que `BitmapImage.SetSourceAsync` anime el GIF (WinUI lo anima con `AutoPlay`, que viene activo) y que la penumbra de la despedida no se note sobre fondos muy claros.

## Validación estricta, portadas, volumen y pantalla completa (09/11)

| Tarea | Commit | Archivos |
|---|---|---|
| 1. Solo letras y números | `d9c0056` | Nuevos: `Services/Validaciones/InputRules.cs` (regla única), `Views/Componentes/FiltroCampo.cs` (al escribir y pegar; encendido en `LoginFieldStyle` de `App.xaml`). Cambiados: `CredentialValidator.cs`, `SecurityAnswerValidator.cs` (`AnswerInvalid`), `Models/CredentialError.cs`, `UserService.cs` y `PasswordRecoveryService.cs` (validan otra vez antes de guardar o comparar), `LoginViewModel.cs`, `EmotesViewModel.cs` (sin `Trim`, rechaza símbolos), `Textos.xaml`, `VistaEjemploLogin.xaml` (muestra de puntos sin filtro), `appsettings.json` (`hostdemo`). Pedido a Web anotado en `CONTRATO.md` |
| 2. Portadas 4 y 5 | sin commit (fuera del repo) | `COT_0_P/portadas/Audio_04.png`, `Audio_05.png`; respaldo en `portadas/respaldo_4y5_2026-11-09/` |
| 3. Volumen 25 % | `96cfb5c` | `appsettings.json` (`Audio:DefaultVolume` = 25, escala 0–100 como ya estaba), `MusicService.cs` (al arrancar solo carga, no reproduce), `AudioSettings.cs` |
| 4. Pantalla completa | `51c8ae4` | `WindowSetup.cs`, `MainWindow.xaml(.cs)` (✕ propia), `CloseGuard.cs` (`RequestCloseAsync`), `WindowSettings.cs` (ahora solo tamaño del diseño para el Manual), `Textos.xaml` |

- El Host no tiene cuadro de mensajes de chat (el chat vive en la web), así que no hubo campo que dejar libre.
- `MusicService.cs` tenía un cambio sin commit (`_failures =25` en `MediaOpened`); se regresó a `0` en el commit del volumen porque rompía el salto de canciones ilegibles.
- Verificación: `dotnet build COT_0.sln` en 0 errores y 0 warnings. La app no se abrió (sin capturas): filtro, ✕, pantalla completa y volumen quedan para `PRUEBAS_MANUALES.md`, sección 13. Las portadas sí se revisaron a ojo como imagen (completas y con acercamiento a los bordes).

## Video en el Manual, audio inicial, cierre limpio y despedida (09/11)

| Tarea | Commit | Archivos |
|---|---|---|
| 1. Video de fondo en el Manual | `e94a995` | `CapaFondoViewModel.cs` (el Manual ya no fuerza cuadro fijo; `_isManual`), `FondoPlayer.cs` (`SetSilenced`), `BackgroundSettings.cs` y `appsettings.json` (`Background:VideoMuted`), comentarios de `CapaFondo.xaml`, `Manual.xaml`, `ManualPage.xaml` |
| 2. Audio inicial | `a487668` | `MusicService.cs` (abre y reproduce), `AudioSettings.cs` y `appsettings.json` (`Audio:InitialTrack` = 2, `Audio:TrackNameFormat`; se quitó `Audio:DefaultFile`) |
| 3. Cierre limpio | `951c94e` | Nuevo: `Services/Ventana/AppShutdown.cs`. Cambiados: `CloseGuard.cs`, `App.xaml.cs` (ya no `Services.Dispose()` en `Window.Closed`), `MusicService.cs` (`Stop`, handlers con nombre), `ClockService.cs` (`Stop`), `UiThread.cs` (`Detach`), `FondoPlayer.cs`, `ManualVideo.cs`, `ManualSound.cs` (sin `IDisposable`) |
| 4. Despedida centrada | `eb8ee65` | `ManualPage.xaml` (`VideoLayer` + panel debajo del video), `Manual.xaml` (tamaños, esquinas de abajo, `ManualFarewellText`) |

- **Causa del crash al cerrar**: `App` hacía `Services.Dispose()` en `Window.Closed`, que liberaba los `MediaPlayer` de los fondos, la música y el Manual mientras la ventana seguía montada. (a) Con video de fondo (Dashboard/Sala) `Window.Close()` se quedaba colgado en el desmontaje; (b) desde el Manual, WinUI dispara `ManualPage.Unloaded` **después** de cerrar → `ManualViewModel.Leave()` → `ManualSound.Stop()` sobre un `MediaPlayer` ya liberado → `COMException 0x80004004` sin manejar → 0xC000027B (el mismo código y módulo que el visor de eventos tenía de las pruebas del 08/11). Confirmado con trazas temporales (no se subieron).
- **Arreglo**: `AppShutdown.StopAsync` (detiene reloj y reproductores, apaga sala, vigilantes, túnel y Kestrel con async, y suelta `UiThread`), luego `Window.Close()` y al final `Release` (job de cloudflared). Los `MediaPlayer` ya no se liberan con `Dispose`: quedan detenidos y sin archivo, y Windows los suelta al terminar el proceso.
- Verificación: build de `COT_0.Host` en 0 errores y 0 warnings (en una carpeta aparte: la app abierta desde Visual Studio bloqueaba `bin\`). Cierre probado con UI Automation (sin capturas) en una segunda instancia: Login, Dashboard y Manual × (✕ + SALIR dos veces, Alt+F4) = 9 de 9 con código de salida 0 y sin eventos de error; antes, el Manual daba 0xC000027B y el Dashboard se quedaba colgado. Despedida medida con UI Automation a 1920 × 1080: Cerrar en x 927–996 (centro 961) y 16 px libres arriba y abajo del grupo.
- No verificado: cierre con la sala abierta (túnel y Kestrel reales; la segunda instancia no podía usar el puerto 5050), la despedida a 1280 × 720 (se calculó: el `Viewbox` la reduce a ~68 %), y a ojo el video del Manual, el sonido de Audio_02 y las orillas de la despedida.

## Música muteada en el Manual (09/11)

| Tarea | Commit | Archivos |
|---|---|---|
| Música muda (sin pausa) dentro del Manual | commit "Manual: la música de fondo queda muteada sin pausa…" | `Services/Musica/MusicService.cs` (`IsHushed`, `ApplyMute`; `IsMuted` ahora guarda el silencio del usuario en su propio campo), `Views/ModelosDeVista/ManualViewModel.cs` (recibe `MusicService`; `IsHushed` en `EnterAsync` y `Leave`) |

- Diseño: el reproductor queda mudo si `IsMuted` (bocina del usuario) **o** `IsHushed` (Manual). Así no hace falta guardar y restaurar el estado previo: el del usuario nunca se toca y la bocina no cambia de icono. `IsHushed` no avisa a la interfaz.
- Cierre: si la app se cierra dentro del Manual, `AppShutdown` hace `music.Stop()` y después llega `ManualPage.Unloaded` → `Leave` → `IsHushed = false`: solo pone `IsMuted` en un `MediaPlayer` detenido y **no liberado** (no hay `Dispose`), sin `StateChanged`. No hay error.
- Sin claves nuevas en `appsettings.json`. Video de fondo sin cambios.
- Verificación: `dotnet build COT_0.sln` en 0 errores y 0 warnings, en una carpeta aparte (`-p:BaseOutputPath` en el scratchpad de la sesión): la app abierta bloqueaba `COT_0.Host\bin\` y no se cerró. No se abrió la app: lo de oído queda para `PRUEBAS_MANUALES.md`, sección 15.

## Fondo de color en el Manual (09/11)

| Tarea | Commit | Archivos |
|---|---|---|
| Fondo de color y Lain sin recuadro | commit "Manual: fondo de color opaco…" | `Views/Estilos/Manual.xaml` (`ManualBackdropBrush` en lugar de `ManualVeilBrush`, `LainCutoutFade`), `Views/Pantallas/ManualPage.xaml` (fondo nuevo, `LainCutoutHost` con orillas), comentarios de `CapaFondo.xaml` y `CapaFondoViewModel.cs` |

- Sin cambios de lógica: `CapaFondoViewModel` ya dejaba el video corriendo y mudo en el Manual; ahora solo queda tapado por un fondo opaco. Sin claves nuevas en `appsettings.json` (son colores y tamaños: van en `Views\Estilos`).
- Verificación: `dotnet build COT_0.sln` en 0 errores y 0 warnings, en la carpeta aparte del scratchpad (la app abierta bloqueaba `bin\`). No se abrió la app: el aspecto queda para `PRUEBAS_MANUALES.md`, sección 16.

## Reglas nuevas, 20 usuarios y tienda de emotes (09/11)

| Tarea | Commit | Archivos |
|---|---|---|
| A. Validaciones | `d3f5f77` | Nuevos: `Services/Validaciones/FieldKind.cs`, `PasswordValidator.cs`, `PasswordProblems.cs`, `PasswordMessages.cs`. Cambiados: `InputRules.cs`, `CredentialValidator.cs`, `InputsSettings.cs`, `Models/CredentialError.cs` (`PasswordRules`, `AccountOutdated`, `WrongCredentials`), `Models/HostUser.cs` (`RulesVersion`), `HostAccounts.cs`, `UserService.cs`, `FiltroCampo.cs` (`IsUserName`), `App.xaml` (`UserNameTextBox`), `LoginPage.xaml`, `FormularioRegistro.xaml`, `FormularioRecuperacion.xaml`, ViewModels del login, `Textos.xaml`, `appsettings.json` (`Inputs:*`, `Hostdemo`) |
| B. 20 usuarios | `a00e6da` | `appsettings.json` (`Usuarios:Maximo` = 20) y docs |
| C. Tienda de emotes | commit "Emotes: mini tienda…" | Web: `EmoteSearch.cs` (bug del picker), `EmotePicker.razor`, `ISevenTvClient`/`SevenTvClient` (`SearchEmotesAsync`), `SevenTvSearch*`, `EmoteCatalogPage.cs`, `IPurchasedEmotes.cs`, `RoomEmotes.cs`, `EmoteRegistration.cs`, `ChatOptions.cs` (`SevenTvGqlPath`). Host: `Services/Tienda/*`, `Services/Usuarios/ActiveUser.cs`, `StoreSettings.cs`, modelos de compras y tickets, `Views/Componentes/Tienda/*`, `Views/Estilos/Tienda.xaml` y `TiendaControles.xaml`, ViewModels de la tienda, `EmotesPage`, `CapaFondoViewModel.cs`, `RoomSession.cs`, `LoginViewModel.cs`, `App.xaml.cs`, `COT_0.Host.csproj` (PDFsharp 6.2.4) |

- **Bug del picker**: `EmoteSearch.Find` hacía `emotes.Take(max)` también sin texto, con `Chat:EmotePickerLimit` = 120: el picker (celular y PC) solo pintaba los primeros 120 del set y el resto solo salía buscando. No era 7TV ni el CSS (`.emote-grid` ya tenía scroll). Ahora sin texto regresa todos; el tope queda solo para búsquedas. Las imágenes ya cargaban diferidas (`loading="lazy"`).
- **Cuentas viejas**: no se migra ni se borra nada al abrir. Al ENTRAR con la contraseña correcta, si la cuenta no tiene `RulesVersion` 2 (todas las de antes), su nombre ya no cumple o su contraseña no tiene número y símbolo, sale "Esta cuenta ya no cumple las reglas nuevas. Regístrala de nuevo." y no entra. Registrar ese mismo nombre reemplaza la entrada vieja (no suma al máximo); cambiar la contraseña con ¿Olvidaste tu contraseña? también la deja vigente si el nombre cumple. Una cuenta con números en el nombre no se puede escribir (el filtro los quita) y se queda en el archivo tal cual.
- **7TV**: búsqueda por GraphQL v3 (`POST https://7tv.io/v3/gql`, `emotes(query, page, limit)`), probada el 09/11 con y sin texto y en la página 3; la v4 también respondía pero la v3 trae el mismo `host.files` que ya traduce `SevenTvMapper`.
- **PDF**: PDFsharp 6.2.4 (MIT, sin binarios nativos). Su resolvedor de fuentes de Windows no encuentra Consolas; con Courier New sí (probado generando un ticket en el scratchpad).
- **Decisiones**: la pantalla abre en TIENDA; el set de 7TV, los reportes y los quitados pasaron al recuadro SET Y REPORTES (para dejar solo 3 pestañas); el ticket no lleva el nombre del usuario; el número de ticket es global (para que los PDF de varios usuarios no se pisen).
- Verificación: `dotnet build COT_0.sln` en 0 errores y 0 warnings. La app no se abrió (sin capturas ni computer use).

## Tienda en modo HOST: carrito, uno por uno y temporizador (09/11)

- CARRITO (por defecto) junta hasta lo que te queda y hace **una compra con un ticket** (`PurchaseService.BuyAsync` recibe la lista; si no cabe en el cupo no compra ninguno). UNO POR UNO compra al instante con un ticket por emote.
- El cupo se calcula siempre desde `compras.json`: `LimitReachedAt` (UTC) se guarda al llegar a `Store:DailyLimit` y el reinicio es `LimitReachedAt + Store:CooldownHours`. `QuotaTimerViewModel` solo resta la hora actual a esa fecha cada segundo; al llegar a cero vuelve a leer el JSON. Archivos viejos sin `LimitReachedAt` lo deducen de la décima compra.
- Efecto de compra en `SeccionTienda` (Storyboard en XAML) y `Store:PurchaseEffectMs` (900 ms) antes del diálogo del PDF. `TicketPdfActions` junta la descarga del PDF que repetían TIENDA, TICKETS y PUNTO DE VENTA.
- Verificación: `dotnet build COT_0.sln` en 0 errores y 0 warnings. Sin abrir la app: el aspecto queda en `PRUEBAS_MANUALES.md`, sección 20.
