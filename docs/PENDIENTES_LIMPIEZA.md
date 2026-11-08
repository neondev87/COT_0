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

- **`Text.Pending`** ("Pantalla en migración a WinUI 3."): ya solo lo usa `ManualPage`; borrar el texto de `Textos.xaml` al migrarla.
- **Tarjeta 02 (CAMBIAR FONDO) apagada** y su texto `Text.Dashboard.BackgroundSubtitle` ("Próximamente en WinUI"): cambiar al migrar el selector de fondos.
- **`ManualPage` no tiene botón de regresar**: usar `EncabezadoPantalla` al migrarla (Sala, Emotes y Música ya lo tienen).
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
