# Pruebas manuales (Host WinUI)

No hay proyectos de prueba: esto se revisa a mano con `dotnet run --project COT_0.HostWinUI`.
Antes: `dotnet build COT_0.HostWinUI` en 0 errores y 0 warnings.

## Preparación

- `Paths:MediaRoot` en `COT_0.HostWinUI/appsettings.json` apunta a tu carpeta `COT_0_P` (fuera del repo), con `musica\` (2 o 3 canciones), `portadas\`,
  `fondos\` (al menos una imagen y un video), `gifs\` (videos de Lain) y `manual\` (sonidos del manual).
- `cloudflared` instalado (`winget install Cloudflare.cloudflared`) o su ruta en `Tunnel:CloudflaredPath`.
- Para empezar de cero, borra `%LocalAppData%\COT_0\` (ahí viven `usuarios.json`, `visitas.json`, `sesiones.json` y `fondos.json`).
- Cierra el Host WPF y cualquier `dotnet run` de la web: usan el mismo puerto 5050.

## 1. Registro con pregunta secreta

1. Sin `usuarios.json`, la app abre en **CREA TU CUENTA DE HOST**.
2. REGISTRARME vacío → InfoBar "Escribe tu usuario.". Usuario con espacio o símbolo → "solo puede tener letras y números".
3. Sin elegir pregunta → "Elige una pregunta secreta."; sin respuesta → "Escribe tu respuesta.".
4. Llena todo y REGISTRARME → entra al dashboard. En `usuarios.json` hay hash y salt, nunca la contraseña ni la respuesta.
5. Cierra y abre la app: ahora sale **ACCESO DEL HOST** (ENTRAR).

## 2. Recuperación de contraseña

1. En ACCESO DEL HOST, clic en **¿Olvidaste tu contraseña?** → RECUPERA TU CONTRASEÑA con tu pregunta guardada.
2. Respuesta mal + VERIFICAR → "La respuesta no coincide.". Respuesta bien pero con mayúsculas y espacios de más → también pasa.
3. Ya verificada, la respuesta queda fija y aparecen CONTRASEÑA NUEVA y ESCRÍBELA OTRA VEZ.
4. Distintas en los dos campos → "Las dos contraseñas no coinciden.".
5. CAMBIAR CONTRASEÑA → InfoBar verde "Tu contraseña cambió…". La vieja ya no entra; la nueva sí.
6. "Volver al inicio de sesión" regresa sin cambiar nada.

## 3. ENTRAR y SALIR

1. Usuario o contraseña mal → "Usuario o contraseña incorrectos.".
2. Bien → dashboard con reloj, fecha, estado de la sala y las 6 tarjetas.
3. **06 SALIR** sin sala abierta → regresa al login sin preguntar.
4. Con la sala abierta (ver 4), SALIR → diálogo "Hay una sala activa. ¿Cerrar la sala y salir?":
   - CANCELAR (o Esc) → te quedas en el dashboard y la sala sigue abierta.
   - CERRAR SALA Y SALIR → cierra la sala y el túnel y vuelve al login.
5. Lo mismo con la ✕ de la ventana: con sala abierta pregunta; sin sala cierra directo.

## 4. Sala

1. **01 GENERAR QR** → SalaPage con InfoBar "Abriendo la sala" y barra de progreso; luego QR, enlace `https://….trycloudflare.com/join`, código `#XXXX`, punto rojo **EN VIVO** y el tiempo corriendo.
2. **COPIAR** → cambia a COPIADO un momento; pega en el navegador y abre `/join`.
3. **Escanear**: apunta el celular al QR → se abre `/join`. Pon nombre, foto y escudo y entra al chat.
4. **Invitado conectado**: el contador pasa a `01` y aparece en la lista. En el dashboard dice "· 1 conectados".
5. **Expulsar**: ✕ de su fila → sale de la lista y el celular vuelve a `/join` con aviso de expulsado.
6. **EMOTES** y **MÚSICA** (abajo a la izquierda) abren su pantalla; el encabezado dice **← SALA** y regresa con la sala abierta (mismo código, el tiempo sigue).
7. **← INICIO** (o Esc) vuelve al dashboard con la sala abierta: la tarjeta 01 dice SALA "Volver a la sala activa".
8. **CERRAR SALA** → el celular ve que la sala se cerró, regresas al dashboard y la tarjeta vuelve a GENERAR QR. Abrir otra vez da código nuevo.

### Errores (InfoBar rojo con REINTENTAR)

- **cloudflared no encontrado**: pon `Tunnel:CloudflaredPath` en `noexiste.exe` → "No se encontró cloudflared…".
- **Túnel que no arranca**: desconecta el internet y genera el QR → "no entregó el enlace en N segundos" (o "se cerró antes de dar el enlace").
- **Puerto ocupado**: corre `dotnet run --project COT_0.Web` antes de abrir la app → "El puerto de http://localhost:5050 ya está ocupado…".
- Arregla la causa y REINTENTAR abre la sala sin salir de la pantalla.

## 5. Archivos en `%LocalAppData%\COT_0`

1. `visitas.json`: una línea por cada invitado que entró (`Name`, `EnteredAt`). Si sale y vuelve a entrar, cuenta otra vez.
2. `sesiones.json`: una entrada por cada vez que abriste la sala, con `Number` 1, 2, 3…, `StartedAt`, `Guests` y `EndedAt`.
3. Después de CERRAR SALA (o SALIR / ✕ con confirmación) la última sesión tiene `EndedAt`. Si matas la app desde el Administrador de tareas, queda en `null` (esperado).
4. Abre una sala sin invitados y ciérrala: sesión nueva con `Guests` vacío.

## 6. Emotes

1. **04 EMOTES** (o EMOTES desde la sala). Escribe un ID o URL de set de 7TV y APLICAR → CARGANDO… y luego **OK** con el número de emotes.
2. ID inventado → estado en rojo con el motivo. Sin internet → "SIN CONEXIÓN CON 7TV". Campo vacío → SIN EMOTES.
3. Con la sala abierta, un invitado mantiene presionado un emote → REPORTAR: aparece en REPORTES.
4. QUITAR → pasa a QUITADOS y en el chat se ve como texto; RESTAURAR lo regresa. IGNORAR lo saca de REPORTES.

## 7. Música

1. **05 MÚSICA**: lista con portada, título y duración; la que suena tiene barra roja y su portada grande.
2. Clic en otra canción la cambia. ▶‖ pausa/reanuda; ◀ ▶ anterior/siguiente (en la última, ▶ vuelve a la primera).
3. Errores con InfoBar amarillo: carpeta que no existe, carpeta vacía, archivo que no es audio renombrado a `.mp3` (salta a la siguiente).
4. ← INICIO no corta la música.

## 8. Firma del login

1. Arriba a la derecha: avatar, **Neondev**, nota, barra de volumen y la canción que suena. La música empieza sola.
2. Mouse encima: el nombre pasa a rojo y el borde del avatar se aclara (sin brillo).
3. Clic en el nombre → abre el repo de GitHub.
4. Clic en la nota → silencio (icono tachado); mover la barra quita el silencio.
5. El volumen y la canción son los mismos que en MÚSICA (cambia uno y revisa el otro).
6. Con `Audio:MusicFolder` en una carpeta que no existe → la firma dice **Sin música**.

## 9. Fondos

### Imagen

1. Copia un `.jpg` o `.png` a `fondos\`. En el dashboard, **02 CAMBIAR FONDO** → pantalla FONDOS con miniaturas (nombre, IMAGEN · N MB).
2. Detrás se ve el fondo actual con un velo oscuro (no hay desenfoque).
3. Clic en la imagen → borde rojo en su miniatura y el fondo cambia detrás. ← INICIO (o Esc) → el dashboard tiene esa imagen con un velo más suave.
4. **01 GENERAR QR**: la sala tiene el mismo fondo. En la sala, **CAMBIAR FONDO** (abajo a la izquierda) abre FONDOS y el encabezado dice **← SALA**.
5. Cierra y abre la app: el fondo elegido sigue (está en `%LocalAppData%\COT_0\fondos.json`, `SelectedFile`).
6. **SIN FONDO** → vuelve el degradado gris del login; en `fondos.json`, `SelectedFile` queda en `""`.

### Video

1. Elige un video ligero (menos de 120 MB): se reproduce detrás en bucle y **sin sonido**; la música sigue sonando igual.
2. Elige un video de más de 120 MB: su miniatura dice **PESADO** y arranca como **cuadro fijo**. "Usar cuadro fijo (más ligero)" está marcado.
3. Desmarca "Usar cuadro fijo" → el video se reproduce. Márcalo → vuelve el cuadro fijo. Se guarda por video en `fondos.json` (`UseStill`).
4. Con un video de fondo, abre 05 MÚSICA o 03 MANUAL: el fondo no se ve ahí (y se pausa). Regresa al dashboard: sigue donde iba.
5. En el Administrador de tareas, con el video corriendo en el dashboard, el uso de CPU no debería trabar la ventana (el reloj avanza cada segundo).

### Errores (InfoBar amarillo en FONDOS)

- **Carpeta faltante**: cambia `Background:Folder` a `noexiste` → "No se encontró la carpeta de fondos: …"; el dashboard queda con el degradado.
- **Carpeta vacía**: apunta `Background:Folder` a una carpeta sin imágenes ni videos → "La carpeta … no tiene imágenes ni videos."
- **Archivo ilegible**: renombra un `.txt` a `roto.mp4` (o `roto.png`) en `fondos\` y elígelo → el fondo cae a su cuadro fijo o al degradado y, al volver a FONDOS, "No se pudo abrir «roto.mp4»…".
- **fondos.json dañado**: escribe basura en `fondos.json` y abre FONDOS → "No se pudo leer o guardar fondos.json…"; la lista sale igual.
- **RECARGAR** después de arreglar la carpeta: el aviso se quita y aparecen las miniaturas.

## 10. Manual de Lain

1. **03 MANUAL** → fondo azul noche, ← INICIO | MANUAL arriba a la izquierda, atajos ENTER / → / ← / ESC arriba a la derecha y "Bienvenido a COT_0".
2. Abajo a la izquierda, el video de Lain en bucle (sin quitar el negro: se ve sobre el azul con el borde de arriba fundido).
3. Arriba a la izquierda salen las notificaciones de Lain una por una con fundido y el sonido de aviso; la última se queda. En el recuadro del paso, los avisos que ya salieron se encienden.
4. ENTER o → → paso siguiente con un destello corto; ← regresa. En el paso 1, ← está apagado; en el 6, → también.
5. Bocina (abajo a la derecha) → silencio (icono tachado): ya no suena el aviso.
6. "Gracias, pero no necesito ayuda" o ESC → despedida: el video cambia al de despedida, al centro y más tenue, con estática de fondo y "Gracias por estar aquí. Vuelve pronto."
7. **Cerrar**, ENTER o ESC en la despedida → regresa al dashboard. ← INICIO en cualquier momento también regresa y corta video y sonidos.
8. Vuelve a abrir el manual: empieza otra vez en el paso 1 y sin silencio.
9. Capturas opcionales: pon `paso2.png` en `manual\pasos\` → el recuadro del paso 2 muestra la imagen en lugar de la lista.

### Errores (InfoBar amarillo abajo a la derecha)

- **Archivos que faltan**: renombra `gifs\lain_tele_web.mp4` → "Faltan archivos del manual: lain_tele_web.mp4…"; el manual sigue sin video.
- **Archivo ilegible**: reemplaza `lain_despedida.mp4` por un `.txt` renombrado y llega a la despedida → "No se pudo abrir «lain_despedida.mp4»…".

## 11. Correcciones visuales (08/11)

### Fondos: video de Gojo y miniaturas

1. Borra `%LocalAppData%\COT_0\cache\fondos\` (si existe) y abre **02 CAMBIAR FONDO**.
2. Las 3 miniaturas (Batman, Jujutsu Kaisen…, "CHILL" NIGHT RIDE…) muestran un cuadro del video (no el icono vacío). Abajo, el nombre limpio: sin `[3-WVRBufuVw]` y con `:` / `"` / `|` normales. Pasa el mouse: el tooltip da el nombre completo del archivo.
3. En `cache\fondos\` hay 3 `.jpg` (uno por video). Cierra y vuelve a abrir FONDOS: salen al instante y no se generan otros.
4. Elige **Gojo**: dice PESADO y "Usar cuadro fijo" está marcado → el fondo es su cuadro fijo y **no** sale el InfoBar.
5. Desmarca "Usar cuadro fijo" → el video de Gojo se reproduce. Si tu PC no pudiera, cae solo al cuadro fijo y el InfoBar dice la causa (sin códec, no decodifica…) con el código; el detalle queda en `%LocalAppData%\COT_0\registro-medios.jsonl`.
6. El CheckBox marcado es **rojo**, no azul.
7. Sin ffmpeg: renombra temporalmente `Background:FfmpegPath` a `noexiste` y borra la caché → las miniaturas salen igual (miniatura de Windows o `MediaComposition`, un poco más lento).
8. Cuadro que no sale: renombra un `.txt` a `roto.mp4` en `fondos\` → su miniatura queda con el icono, y al elegirlo con cuadro fijo el InfoBar dice "No se pudo sacar un cuadro de «roto.mp4»…"; sin cuadro fijo, la causa de Windows y su código.

### Sala: barra de abajo

1. **01 GENERAR QR**. Abajo: EMOTES, MÚSICA y CAMBIAR FONDO a la izquierda; "Al cerrar, todos / salen de la sala" en dos líneas en medio; CERRAR SALA a la derecha, completo y con margen. Nada encimado.
2. Pasa el mouse por CERRAR SALA: se corre a la izquierda y no se corta.

### Música

1. **05 MÚSICA**: la portada grande se ve nítida, sin marco de sobra y sin recortes. `Audio_04` (vertical) sale más angosta que las cuadradas.
2. Las miniaturas de la lista se ven completas (las verticales con fondo a los lados).
3. La barra de volumen y su bolita son rojas.

### Emotes (7TV)

1. **04 EMOTES**: arriba "SET DE 7TV" a la izquierda y "N emotes cargados" a la derecha, sin encimarse.
2. El campo y APLICAR tienen la misma altura; APLICAR con el texto centrado.
3. Abajo, el estado ("OK" o el error) alineado a la izquierda. Escribe un ID inválido y APLICAR: el error largo hace wrap.

### Login

1. Entra con una cuenta vieja sin pregunta (o fuerza el paso): el subtítulo "CONFIGURA TU PREGUNTA SECRETA" hace wrap entre sus dos líneas rojas; ninguna se sale del formulario.

### Logo Neondev

1. En el **login** (arriba a la derecha), el **dashboard** (abajo del estado de la sala) y la **sala** (abajo de EN VIVO): avatar de 64 px con esquinas redondeadas y "Neondev".
2. Un brillo diagonal cruza el avatar de ida y vuelta (3 s por lado), sin trabar nada.
3. Mouse encima: cursor de mano, tooltip "Abrir el repositorio del proyecto", el nombre pasa a rojo y el borde del avatar se aclara.
4. Clic → abre https://github.com/neondev87/COT_0 en el navegador (cámbialo en `Branding:RepoUrl`).
