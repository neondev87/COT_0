# Pendientes de limpieza

Solo lo que sigue pendiente después del cierre (08/11, rama `host-winui`; ya con varios usuarios, async en disco y nombres naturales). Lo que ya se borró o se aplicó está en `REPORTE_CIERRE.md`.

## Por probar

- **Prueba a mano completa** de `PRUEBAS_MANUALES.md` en la app final (`dotnet run --project COT_0.Host`): túnel real, QR escaneado desde un celular, música, videos de fondo, manual con sonido y el login con varios usuarios (sección 1). En el cierre solo se verificó build, arranque y código.

## En esta PC (fuera del repo)

- **user-secrets del WPF viejo** (`Host:User` / `Host:Password`, `UserSecretsId cot0-host-7fc7bba9-727e-4d80-9300-10219506c924`): la app final no los lee. Se dejaron porque el respaldo (`main`, `winui-migration`, tag `pre-cierre-winui`) todavía los necesita para entrar. Si ya no vas a usar el respaldo: `dotnet user-secrets clear --id cot0-host-7fc7bba9-727e-4d80-9300-10219506c924`.
- **Usuarios de prueba**: si registraste usuarios para probar el máximo de 20, quedan en `%LocalAppData%\COT_0\usuarios.json`. Un ADMIN los puede ELIMINAR desde 04 USUARIOS (también borra sus compras, tickets y foto).
- **Admins de prueba**: `%LocalAppData%\COT_0\admin.json`. Si el único admin pierde su contraseña, cierra la app y borra `admin.json`: al elegir ADMIN se crea el primero otra vez (no hay puerta trasera).
- **`*.json.tmp`**: si la app se cerrara a la mitad de un guardado podría quedar un `.tmp` junto al JSON; el original sigue intacto y el `.tmp` se puede borrar.
- **Temporales de la sesión del 09/11 (noche)**: solo `docs_final.py` (script que actualizó estos docs) en el scratchpad de la sesión, fuera del repo; se puede borrar.
- **`%LocalAppData%\COT_0\cache\fondos\*.jpg`**: un cuadro por video (≈ 12–90 KB cada uno). Si cambias o borras un video, su JPEG viejo **se queda** (no hay limpieza automática). Se puede borrar la carpeta completa: se regenera al abrir FONDOS. Puede quedar algún `*.part.jpg` si la app se cerró a medio generar.
- **`%LocalAppData%\COT_0\registro-medios.jsonl`**: crece sin tope (una línea por falla). Si estorba, borrarlo o rotarlo.
- **`COT_0_P/portadas/respaldo_4y5_2026-11-09/`** (09/11): originales de `Audio_04.png` y `Audio_05.png` con su fondo blanco (≈ 42 y 65 KB) y `portadas45.py`, el script que les quitó el fondo (mismo proceso que `procesar.py` + máscara encogida 2 px). Si las portadas nuevas se ven bien, se puede borrar la carpeta.
- **`COT_0_P/diseno/`** (maquetas e `inventario-winui.md`): vive fuera del repo; no subirlo.
- **`COT_0_P/gifs/lain-transparente.gif`** (≈ 2.1 MB): Lain con transparencia, generado con ffmpeg a partir de `lain_tele_web.mp4` (`docs/PREPARAR_LAIN.md`). Fuera del repo e ignorado (`/gifs/`, `lain-transparente*.gif`). Si se borra, el Manual usa el video con su penumbra; se regenera con el mismo comando.

- **Archivos temporales de la corrección del 09/11** (video en el Manual, audio, cierre y despedida): ninguno en el repo ni en `COT_0_P`. Los scripts de prueba (`close-test.ps1`, `farewell-test.ps1`), el build de prueba y un cuadro de `lain_despedida.mp4` quedaron en la carpeta temporal de la sesión (`%LocalAppData%\Temp\claude\…\scratchpad`); se pueden borrar.

- **Build de prueba de la música en el Manual (09/11)**: la salida de `dotnet build` quedó en `%LocalAppData%\Temp\claude\…\scratchpad\build\` (la app abierta bloqueaba `bin\`). Fuera del repo; se puede borrar.

- **Tienda de emotes (09/11)**: al probar quedan `%LocalAppData%\COT_0\compras.json` (compras y tickets de prueba) y la carpeta `Escritorio\COT_0_tickets\` con los PDF. Borrarlos si estorban; la app recrea lo que necesite.
- **Archivos temporales de la tienda (09/11)**: prueba de PDFsharp en el scratchpad de la sesión (`…\scratchpad\pdftest\`, `t.pdf`, `ticket.pdf`), fuera del repo y de `COT_0_P`. Nada temporal en el repo.
- **Sesión del 12/11 (admins, emotes, panel de compra)**: nada temporal en el repo ni en `COT_0_P`. Se usaron archivos de paso en `%TEMP%` (`global.json` con el set global de 7TV para revisar nombres repetidos, `p.xaml`/`v.xaml`) y ya se borraron o movieron. Al probar quedan los PDF generados en `%LocalAppData%\COT_0\tickets\` (copia de la app de cada ticket); se pueden borrar: SOLO VER y DESCARGAR los vuelven a generar.

- **Sesión del 12/11 (segunda: punto de venta por usuario y precio guardado)**: nada temporal en el repo ni en `COT_0_P`. En el scratchpad de la sesión (`%LocalAppData%\Temp\claude\…\scratchpad\`) quedaron `admintest\` (consola que probó `AdminService` contra una carpeta `%LocalAppData%\COT_0_admintest` que se borra sola al terminar) y los scripts `ed.py`, `ventas.py`, `usuarios.py` y `t2.py`; se pueden borrar.

## En el código (no se tocó en el cierre: cambia lógica)

- **`README.md`** (fuera de `COT_0.Host` y `docs`) todavía dice que la música "empieza por `Audio:DefaultFile`": ahora es `Audio:InitialTrack` (Audio_02).

- **SUBIR MÚSICA** no existe en WinUI (necesita `FileOpenPicker` con el HWND de la ventana). Si se quiere, va en `Views/Pantallas/MusicaPage` + `Services/Musica`.
- **Vista de ejemplo del paso 5 del Manual** (`VistaEjemploEmotes`): ya sin set (12/11); revisar a mano que el resaltado siga cayendo sobre el bloque de arriba.
- **`IPurchasedEmotes`** vive en Web y `IRoomEmotes` todavía trae miembros del set (`SetId`, `UseSetAsync`…) que ya no se usan; pedido a Core anotado en `CONTRATO.md` (12/11).
- **`FondoImages` también lo usa el manual** (`ManualViewModel`, para las capturas `pasoN.png`): si crece, renombrarlo a algo general (`ImageDecoder`).
- **Kestrel queda arriba con la sala cerrada** (se apaga al cerrar la ventana). Para apagarlo también al cerrar la sala habría que pedir `IChatService`/`IRoomService` a la web nueva en cada ViewModel (hoy se guardan una vez como Singleton).
- **`ConnectedUsersViewModel` y `SalaViewModel`** se suscriben a eventos de la web y del reloj sin soltarlos: es intencional (viven lo que la app), pero si se cambian a Transient habría que desuscribir.
- **`ChatMessage.Kind`**: el render y el hub todavía reconocen voz e imagen por el texto reservado (`VoiceNote` / `ImageNote`); pasar a `Kind` quedó para después (ver `CONTRATO.md`).
- Incumplimientos menores anotados en `CUMPLIMIENTO.md`: `App.xaml` y `Textos.xaml` pasan de 200 líneas, y varios servicios del Host no tienen interfaz en Core.

## Opcional

- **Capturas del manual** (`manual\pasos\paso1.png`…`paso6.png`): no existen; sin ellas cada paso muestra su vista de ejemplo con resaltados. Si se hacen, van fuera del repo (en `Paths:MediaRoot`).
