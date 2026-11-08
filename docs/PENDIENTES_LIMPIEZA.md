# Pendientes de limpieza

Solo lo que sigue pendiente después del cierre de la migración (08/11, rama `host-winui`). Lo que ya se borró o se aplicó está en `REPORTE_CIERRE.md`.

## Por probar

- **Prueba a mano completa** de `PRUEBAS_MANUALES.md` en la app final (`dotnet run --project COT_0.Host`): túnel real, QR escaneado desde un celular, música, videos de fondo y manual con sonido. En el cierre solo se verificó build, arranque y código.

## En esta PC (fuera del repo)

- **user-secrets del WPF viejo** (`Host:User` / `Host:Password`, `UserSecretsId cot0-host-7fc7bba9-727e-4d80-9300-10219506c924`): la app final no los lee. Se dejaron porque el respaldo (`main`, `winui-migration`, tag `pre-cierre-winui`) todavía los necesita para entrar. Si ya no vas a usar el respaldo: `dotnet user-secrets clear --id cot0-host-7fc7bba9-727e-4d80-9300-10219506c924`.
- **Cuentas de prueba**: `%LocalAppData%\COT_0\usuarios.json` puede tener una cuenta sin pregunta secreta (de antes de esa pantalla). La app te pide configurarla al entrar; si quieres empezar de cero, borra el archivo.
- **`%LocalAppData%\COT_0\cache\fondos\*.jpg`**: un cuadro por video (≈ 12–90 KB cada uno). Si cambias o borras un video, su JPEG viejo **se queda** (no hay limpieza automática). Se puede borrar la carpeta completa: se regenera al abrir FONDOS. Puede quedar algún `*.part.jpg` si la app se cerró a medio generar.
- **`%LocalAppData%\COT_0\registro-medios.jsonl`**: crece sin tope (una línea por falla). Si estorba, borrarlo o rotarlo.
- **`COT_0_P/diseno/`** (maquetas e `inventario-winui.md`): vive fuera del repo; no subirlo.

## En el código (no se tocó en el cierre: cambia lógica)

- **SUBIR MÚSICA** no existe en WinUI (necesita `FileOpenPicker` con el HWND de la ventana). Si se quiere, va en `Views/Pantallas/MusicaPage` + `Services/Musica`.
- **`FondoImages` también lo usa el manual** (`ManualViewModel`, para las capturas `pasoN.png`): si crece, renombrarlo a algo general (`ImageDecoder`).
- **Kestrel queda arriba con la sala cerrada** (se apaga al cerrar la ventana). Para apagarlo también al cerrar la sala habría que pedir `IChatService`/`IRoomService` a la web nueva en cada ViewModel (hoy se guardan una vez como Singleton).
- **`ConnectedUsersViewModel` y `SalaViewModel`** se suscriben a eventos de la web y del reloj sin soltarlos: es intencional (viven lo que la app), pero si se cambian a Transient habría que desuscribir.
- **`ChatMessage.Kind`**: el render y el hub todavía reconocen voz e imagen por el texto reservado (`VoiceNote` / `ImageNote`); pasar a `Kind` quedó para después (ver `CONTRATO.md`).
- Incumplimientos menores anotados en `CUMPLIMIENTO.md`: `App.xaml` y `Textos.xaml` pasan de 200 líneas, y varios servicios del Host no tienen interfaz en Core. El acceso a disco ya es async (`AppConfig.LoadAsync`, `AsyncFiles`); lo que queda síncrono a propósito está en `CUMPLIMIENTO.md`.

## Opcional

- **Capturas del manual** (`manual\pasos\paso1.png`…`paso6.png`): no existen; sin ellas cada paso muestra su vista de ejemplo con resaltados. Si se hacen, van fuera del repo (en `Paths:MediaRoot`).
