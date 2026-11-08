# Pendientes de limpieza

Cosas que sobran o que hay que borrar/cambiar cuando termine la migración a WinUI 3. Nada de esto se borró todavía.

## Al terminar la migración

- **`COT_0.Host/` completo (WPF)**: se queda solo como referencia. Al migrar Sala, Música, Emotes, Fondos y Manual: quitarlo de `COT_0.sln` y borrar la carpeta.
- **`COT_0.Host/Assets/Shaders/LumaKey.hlsl`** y los controles `LumaKeyEffect`, `ShaderCompiler`, `NoiseLayer`, `SignalLossLayer`: no tienen equivalente en WinUI 3; se van con el WPF.
- **Fuentes del WPF que WinUI no usa**: `Barlow-600`, `BarlowCondensed-700Italic`, `Cinzel-400`, `CormorantGaramond-*`, `JetBrainsMono-*`, `Redaction50-*` (en `COT_0.Host/Assets/Fonts`). Si una pantalla nueva las necesita, copiarla a `COT_0.HostWinUI/Assets/Fuentes` antes de borrar.
- **`IAuthService` (Core) y `ConfigAuthService` (WPF)**: el login WinUI usa `UserService` + `usuarios.json`; ver pedido en `CONTRATO.md`.
- **user-secrets `Host:User` / `Host:Password`**: solo los lee el WPF. Borrarlos con `dotnet user-secrets clear --project COT_0.Host` cuando se quite el WPF.
- **Sección `Texts` del `appsettings.json` del WPF**: en WinUI los textos están en `Views/Estilos/Textos.xaml`.
- **`docs/DOCUMENTACION.md`**: todavía describe el Host como WPF (requisitos, estructura). Actualizar al final.

## En COT_0.HostWinUI

- **`.gitignore` de la raíz: la regla `musica/`** (sin `/` al inicio) también ignoraba `COT_0.HostWinUI/Services/Musica/`: `MusicLibrary`, `CoverLibrary`, `SongCatalog` y `MusicService` nunca se habían subido y un clon limpio no compilaba. Se arregló con `COT_0.HostWinUI/.gitignore` (`!Services/Musica/`). Lo correcto es cambiar la regla de la raíz a `/musica/` y borrar esa excepción (la raíz no es carpeta de este agente).
- **`FondoImages` lo usa también el manual** (para las capturas `pasoN.png`): si crece, renombrarlo a algo general (`ImageDecoder`) en `Services/`.
- **`ManualDemoFactory`, `ManualDemo`, `ManualHighlight`, `ManualCue`, `ManualStages.xaml`, `ManualStepView`, `ManualFarewellView`, `ManualBackdrop`, `ManualWindow`, `Themes/Manual*.xaml`** del WPF: no se migraron (ver `CAMBIOS_VISUALES.md`, "Manual de Lain"); se van con el WPF.
- **`BackgroundPicker`, `BackgroundLayer`, `BackgroundPickerViewModel`, `BackgroundOption`, `ThumbnailImage`, `VideoThumbnail`, `LoopingVideo`** del WPF: ya tienen reemplazo (`FondosPage`, `CapaFondo`, `FondosViewModel`, `Fondo`, `FondoImages`, `FondoPlayer`); se van con el WPF.
- **Secciones `Background:PreviewSeconds`, `Manual:StaticHitVolume`, `Manual:Demo` y `Texts:Manual`** del `appsettings.json` del WPF: WinUI no las usa.
- **Duplicados con el WPF (fondos y manual)**: `FondoCatalog` (antes `BackgroundCatalog`) y `ManualAssets` quedaron copiados y adaptados en `COT_0.HostWinUI/Services`. Al borrar el WPF, desaparece el duplicado.
- **Capturas del manual (`manual\pasos\paso1.png`…)**: no existen todavía; sin ellas cada paso muestra su lista de avisos. Si se hacen, van fuera del repo (en `Paths:MediaRoot`).
- **SUBIR MÚSICA** de la firma del WPF no se migró (necesita `FileOpenPicker` con el HWND de la ventana). Si se quiere, va en `MusicaPage`.
- **`.gitignore` de la raíz** solo ignora `*.mp3` y `*.wav`: si alguien copia música dentro del repo, `.m4a`, `.aac` y `.wma` (extensiones de `Audio:Extensions`) y las portadas `.jpg/.png/.webp` sueltas no quedan ignoradas. Agregar esas reglas (está fuera de las carpetas de este agente).
- **Duplicados con el WPF**: `MusicLibrary` y `CoverLibrary` quedaron copiados en `COT_0.HostWinUI/Services/Musica` (WinUI no puede referenciar el proyecto WPF). Al borrar el WPF, desaparece el duplicado.
- **TagLibSharp, `CoverDecoder`, `SongPicker`, `SignatureBadge`, `EmotePanel`, `Themes/Emotes.xaml`, `Themes/SongPicker.xaml`** del WPF: ya tienen reemplazo en WinUI; se van con el WPF.
- **Sección `Texts:Emotes`, `Texts:Music`, `Texts:Songs`, `Texts:Signature`** del `appsettings.json` del WPF: en WinUI esos textos están en `Textos.xaml`.
- **Cuentas de prueba**: si probaste el registro, `%LocalAppData%\COT_0\usuarios.json` puede tener una cuenta sin pregunta (de antes de este cambio). La app te pide configurarla al entrar; si quieres empezar de cero, borra el archivo.
- **`QrPanel`, `UserList`, `RoomView`, `Themes/Room.xaml`, `RoomViewModel`** del WPF: ya tienen reemplazo (`SalaPage`, `ListaConectados`, `Sala.xaml`, `SalaViewModel`); se van con el WPF.
- **Kestrel queda arriba con la sala cerrada** (se apaga al cerrar la ventana). Si se quiere apagar también al cerrar la sala, hay que volver a pedir `IChatService`/`IRoomService` a la web nueva en cada ViewModel (hoy se guardan una vez como Singleton).
- **`ConnectedUsersViewModel` y `SalaViewModel`** se suscriben a eventos de la web y del reloj sin soltarlos: es intencional (viven lo que la app), pero si se cambian a Transient habría que desuscribir.

## Archivos temporales

- `bin/` y `obj/` de los tres proyectos: ya están en `.gitignore`.
- `diseno/inventario-winui.md` vive fuera del repo (en `COT_0_P/diseno`): no subirlo.
- Música, fondos, portadas, gifs y manual: fuera del repo y en `.gitignore` (`musica/`, `/fondos/`, `/portadas/`, `/gifs/`, `/manual/`, `*.mp4`, `*.mp3`, `*.wav`).

## Cierre de la migración (le toca al Agente 0, según `CONTRATO.md`)

Fondos y Manual ya están migrados: el Host WinUI tiene todas las pantallas del WPF. No se hizo nada de esto todavía:

1. Probar a mano todo `docs/PRUEBAS_MANUALES.md` en WinUI.
2. Borrar `COT_0.Host/` (WPF) y quitarlo de `COT_0.sln`.
3. Renombrar `COT_0.HostWinUI/` a `COT_0.Host/` (carpeta, `.csproj`, `RootNamespace` y los `namespace COT_0.HostWinUI…` / `using:COT_0.HostWinUI…` de C# y XAML).
4. Agregar el proyecto renombrado a `COT_0.sln`.
5. Actualizar `CONTRATO.md` (la tabla de la solución dice `COT_0.Host/ WPF`), `docs/DOCUMENTACION.md` y la nota de `CLAUDE.md` ("COT_0.Host: WPF").
6. Borrar los user-secrets del WPF (`dotnet user-secrets clear --project COT_0.Host`) antes de renombrar.
7. Cambiar `musica/` por `/musica/` en el `.gitignore` de la raíz y borrar `COT_0.HostWinUI/.gitignore` (ver arriba).
