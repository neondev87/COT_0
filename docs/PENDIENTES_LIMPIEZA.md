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

- **`Text.Pending`** ("Pantalla en migración a WinUI 3.") en `SalaPage` y `ManualPage`: quitarlo cuando se migren.
- **Tarjeta 02 (CAMBIAR FONDO) apagada** y su texto `Text.Dashboard.BackgroundSubtitle` ("Próximamente en WinUI"): cambiar al migrar el selector de fondos.
- **Tarjeta 01 solo navega**: al migrar `SalaPage`, que abra la sala (`RoomSession.OpenAsync`) como el WPF y conectar `SesionWatcher` (ver `docs/ESTRUCTURA.md`).
- **`SalaPage` y `ManualPage` no tienen botón de regresar** al dashboard: agregarlo al migrarlas.
- **Cuentas de prueba**: si probaste el registro, `%LocalAppData%\COT_0\usuarios.json` puede tener una cuenta sin pregunta (de antes de este cambio). La app te pide configurarla al entrar; si quieres empezar de cero, borra el archivo.

## Archivos temporales

- `bin/` y `obj/` de los tres proyectos: ya están en `.gitignore`.
- `diseno/inventario-winui.md` vive fuera del repo (en `COT_0_P/diseno`): no subirlo.
- Música, fondos, portadas, gifs y manual: fuera del repo y en `.gitignore` (`musica/`, `/fondos/`, `/portadas/`, `/gifs/`, `/manual/`, `*.mp4`, `*.mp3`, `*.wav`).
