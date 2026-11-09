# Pruebas manuales (COT_0.Host)

No hay proyectos de prueba: esto se revisa a mano con `dotnet run --project COT_0.Host`.
Antes: `dotnet build COT_0.sln` en 0 errores y 0 warnings.

## Preparación

- `Paths:MediaRoot` en `COT_0.Host/appsettings.json` apunta a tu carpeta `COT_0_P` (fuera del repo), con `musica\` (2 o 3 canciones), `portadas\`,
  `fondos\` (al menos una imagen y un video), `gifs\` (videos de Lain) y `manual\` (sonidos del manual).
- `cloudflared` instalado (`winget install Cloudflare.cloudflared`) o su ruta en `Tunnel:CloudflaredPath`.
- Para empezar de cero, borra `%LocalAppData%\COT_0\` (ahí viven `usuarios.json`, `visitas.json`, `sesiones.json` y `fondos.json`).
- Cierra otra instancia del Host y cualquier `dotnet run` de la web: usan el mismo puerto 5050.

## 1. Varios usuarios: registrar y entrar

1. Sin `usuarios.json`, la app abre en **ACCESO DEL HOST** (botón ENTRAR) con el aviso "Todavía no hay usuarios. Usa «Registrar usuario» para crear el primero.".
2. Abajo de la contraseña, a la derecha: **¿Olvidaste tu contraseña?** | **Registrar usuario**, en una línea, separados por una barra tenue, sin encimarse ni cortarse.
3. Clic en **Registrar usuario** → REGISTRA UN USUARIO con USUARIO, CONTRASEÑA y ESCRÍBELA OTRA VEZ (no pide pregunta secreta).
4. REGISTRAR vacío → "Escribe tu usuario.". Usuario con espacio o símbolo → "solo puede tener letras y números".
5. Contraseñas distintas → "Las dos contraseñas no coinciden.".
6. Escribe o pega más de 15 caracteres en cualquier campo → se detiene en 15.
7. Llena bien y REGISTRAR → vuelve a ENTRAR con el usuario ya escrito y el InfoBar verde "Usuario registrado…". En `usuarios.json` hay una lista con hash y salt, nunca la contraseña.
8. Registra otro con el mismo nombre en otras mayúsculas (`neondev` vs `Neondev`) → "Ya existe un usuario con ese nombre.".
9. Registra hasta tener 5. Al abrir Registrar usuario otra vez → "Ya hay 5 usuarios registrados, que es el máximo." y REGISTRAR no agrega a nadie.
10. "Volver al inicio de sesión" regresa a ENTRAR sin registrar nada.
11. **Entrar con cada usuario**: la primera vez de cada uno pide CONFIGURA TU PREGUNTA SECRETA → GUARDAR Y ENTRAR → dashboard. SALIR y entra con el siguiente; la segunda vez ya no pide la pregunta. Contraseña de otro usuario → "Usuario o contraseña incorrectos.".
12. Dentro de la app todo se ve igual con cualquier usuario (dashboard, sala, emotes, música, fondos, manual, firma).

## 2. Recuperación de contraseña (por usuario)

1. En ENTRAR, clic en **¿Olvidaste tu contraseña?** → RECUPERA TU CONTRASEÑA pidiendo primero USUARIO (máx. 15 caracteres) y CONTINUAR.
2. Usuario que no existe, o uno que todavía no configura su pregunta → "No se pudo recuperar la contraseña con esos datos." (el mismo mensaje en los dos casos).
3. Usuario válido → el usuario queda fijo y aparece SU pregunta (registra dos usuarios con preguntas distintas para comprobarlo).
4. Respuesta mal + VERIFICAR → "La respuesta no coincide.". Respuesta bien pero con mayúsculas y espacios de más → también pasa.
5. Ya verificada, la respuesta queda fija y aparecen CONTRASEÑA NUEVA y ESCRÍBELA OTRA VEZ. Distintas → "Las dos contraseñas no coinciden.".
6. CAMBIAR CONTRASEÑA → vuelve a ENTRAR con ese usuario escrito e InfoBar verde "Tu contraseña cambió…". La vieja ya no entra; la nueva sí. Los demás usuarios siguen con su contraseña.
7. "Volver al inicio de sesión" regresa sin cambiar nada.

## 2b. Migración de la cuenta anterior y arranque

1. Con el `usuarios.json` de antes (la cuenta única), abre la app: sale ENTRAR sin aviso de "no hay usuarios" y esa cuenta entra con su contraseña de siempre. Si no tenía pregunta, la pide al entrar.
2. Si el archivo trae la cuenta como objeto suelto (`{ "UserName": … }` en vez de `[ … ]`), también entra; al registrar otro usuario o cambiar la contraseña se guarda ya como lista.
3. Una cuenta guardada con nombre de más de 15 caracteres no se borra ni se corrompe (sigue en el archivo tal cual).
4. La app arranca sin congelarse: la ventana aparece, la animación del login corre y se puede escribir de inmediato.

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

## 8. Firma y volumen

1. Abre la app: suena **Audio_02**. La barra de la firma está en 25 y en **05 MÚSICA** el número dice **25** (ver sección 14).
2. Baja la barra a 0 y vuelve a subirla → la música sigue donde iba.
3. Cierra la app con otro volumen y ábrela de nuevo → vuelve a arrancar en 25 con Audio_02.
4. Mueve la barra de la firma y abre MÚSICA: su barra y su número están en el mismo valor. Muévela ahí, regresa: la de la firma se movió igual.
5. Login, dashboard (abajo del estado de la sala) y sala (a la derecha del contador): avatar y **Neondev** en una fila, centrados entre sí; el nombre completo, sin "…" ni encimarse con nada; debajo la barra roja, del ancho de la firma, sin botones de reproducir ni lista.
6. En la sala, la firma queda a media altura de la fila del contador (ni pegada arriba ni abajo).
7. El brillo de vidrio cruza el avatar sin salirse de las esquinas redondeadas.
8. Mouse encima del logo: el nombre pasa a rojo y el borde del avatar se aclara. Clic → abre el repo de GitHub.
9. Con `Audio:MusicFolder` en una carpeta que no existe → la firma dice **Sin música** en lugar de la barra.

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
4. Con un video de fondo, abre 05 MÚSICA: el fondo no se ve ahí (y se pausa); en 03 MANUAL se sigue moviendo, sin sonido. Regresa al dashboard: sigue donde iba, sin reiniciarse.
5. En el Administrador de tareas, con el video corriendo en el dashboard, el uso de CPU no debería trabar la ventana (el reloj avanza cada segundo).

### Errores (InfoBar amarillo en FONDOS)

- **Carpeta faltante**: cambia `Background:Folder` a `noexiste` → "No se encontró la carpeta de fondos: …"; el dashboard queda con el degradado.
- **Carpeta vacía**: apunta `Background:Folder` a una carpeta sin imágenes ni videos → "La carpeta … no tiene imágenes ni videos."
- **Archivo ilegible**: renombra un `.txt` a `roto.mp4` (o `roto.png`) en `fondos\` y elígelo → el fondo cae a su cuadro fijo o al degradado y, al volver a FONDOS, "No se pudo abrir «roto.mp4»…".
- **fondos.json dañado**: escribe basura en `fondos.json` y abre FONDOS → "No se pudo leer o guardar fondos.json…"; la lista sale igual.
- **RECARGAR** después de arreglar la carpeta: el aviso se quita y aparecen las miniaturas.

## 10. Manual de Lain

1. **03 MANUAL** → fondo azul con estática del Manual (ver sección 17; el fondo de la app ya no se ve), ← INICIO | MANUAL arriba a la izquierda y atajos ENTER / → / ← / ESC arriba a la derecha.
   A la derecha, "Paso 01 / 06", el título del paso y, justo debajo, la **vista de ejemplo** del paso (una réplica quieta de la pantalla). Ya no hay texto explicativo bajo el título: lo dicen las notificaciones de Lain.
   La vista de ejemplo ocupa todo el ancho a la derecha de Lain, en 16:9, sin recortes y centrada con márgenes parejos.
   En pantalla completa (si puedes, en monitores de distinta resolución): la réplica crece o se reduce entera, sin encimarse con Lain ni con sus avisos, y cada resaltado sigue sobre su elemento.
2. Abajo a la izquierda, Lain en bucle **sin cuadro negro**: flota sobre el fondo (GIF `gifs\lain-transparente.gif`). Sin ese archivo se usa el video fundido con su penumbra negra (ver sección 10b).
3. Arriba a la izquierda salen las notificaciones de Lain una por una con fundido y el sonido de aviso; la última se queda (textos, orden y tiempos sin cambios).
   **Cada aviso enciende su resaltado** en la vista de ejemplo: borde rojo del proyecto con un halo que late. El anterior se apaga al entrar el siguiente.
4. Revisa cada paso (ENTER o → avanza con destello y fundido corto; ← regresa):

   | Paso | Vista de ejemplo | Resaltado 1 | Resaltado 2 | Resaltado 3 |
   |---|---|---|---|---|
   | 01 Abre la app y entra | Login con `hostdemo`, contraseña de puntos y los enlaces ¿Olvidaste tu contraseña? \| Registrar usuario | COT_0 + ACCESO DEL HOST | Enlace Registrar usuario | ENTRAR |
   | 02 El inicio: genera el QR | Dashboard con la hora actual y SALA INACTIVA | ESTADO (arriba a la derecha) | 01 GENERAR QR | Tarjetas 02 a 06 |
   | 03 Comparte el QR o el enlace | Sala con QR de adorno, `#7F3K` y el enlace de ejemplo | QR | URL DE LA SALA + COPIAR | EN VIVO + tiempo |
   | 04 Ve quién está conectado | La misma sala: "04" y 4 invitados ficticios en la lista | Contador | Lista de conectados | CERRAR SALA |
   | 05 Emotes de 7TV | EMOTES: set `global`, 48 emotes, OK, 2 reportes y 1 quitado | Set + APLICAR | REPORTES | QUITADOS |
   | 06 Música y fondo | MÚSICA: 5 canciones (Duvet suena, barra roja), volumen 25 | Biblioteca | Controles + volumen | ← INICIO |

5. Nada de la vista de ejemplo se puede tocar ni toma el foco: el mouse no hace nada encima y Tab no entra. ENTER siempre avanza de paso.
   La lista de conectados de ejemplo no muestra a los invitados reales aunque haya sala abierta, y la ✕ no hace nada.
6. Bocina (abajo a la derecha) → silencio (icono tachado): ya no suena el aviso; los resaltados siguen.
7. "Gracias, pero no necesito ayuda" o ESC → despedida: el video cambia al de despedida, al centro y más tenue, con estática de fondo y "Gracias por estar aquí. Vuelve pronto."
8. **Cerrar**, ENTER o ESC en la despedida → regresa al dashboard. ← INICIO en cualquier momento también regresa y corta video y sonidos.
9. Vuelve a abrir el manual: empieza otra vez en el paso 1, sin silencio y sin resaltados hasta que entra el primer aviso.
10. Capturas opcionales: pon `paso2.png` en `manual\pasos\` → el recuadro del paso 2 muestra la imagen en lugar de la vista de ejemplo (sin resaltados). Quítala y vuelve a abrir: regresa la vista de ejemplo.
11. Paso 1: el aviso 2 dice "Si es tu primera vez, toca Registrar usuario." y resalta **solo** el enlace "Registrar usuario" de la réplica (borde rojo con halo). La réplica muestra bajo la contraseña, a la derecha, "¿Olvidaste tu contraseña?" | "Registrar usuario" con la barra vertical sutil, igual que el login real. Achica la ventana: los dos enlaces siguen en una línea, sin encimarse ni cortarse, y el resaltado sigue sobre el enlace.
12. Fluidez: con el Administrador de tareas abierto, recorre los 6 pasos; el uso de CPU no debe subir de forma notable (solo opacidad y escala animadas).

### Errores (InfoBar amarillo abajo a la derecha)

- **Archivos que faltan**: renombra `gifs\lain-transparente.gif` **y** `gifs\lain_tele_web.mp4` → "Faltan archivos del manual: lain_tele_web.mp4…"; el manual sigue sin Lain. Si solo falta el video y el GIF está, no hay aviso.
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

## 12. Retoques finales (08/11)

### Manual de Lain

1. Elige un fondo en **02 CAMBIAR FONDO** (imagen o video) y abre **03 MANUAL**: ~~el fondo se ve detrás (velo de ~60 %)~~ reemplazado el 09/11: ahora va el fondo de color del Manual (sección 16) y el texto se lee bien.
2. Lain (el video de la tele) está completa y parada en el borde de abajo: sin hueco debajo, sin cabeza ni pies cortados y sin caja con orillas duras.
3. Sus notificaciones, la fila de atajos (ENTER, →, ←, ESC) y "Gracias, pero no necesito ayuda" no se enciman con ella.
4. La fila de atajos y el botón de abajo se ven como vidrio (degradado claro y borde fino); el marco de la vista de ejemplo tiene un borde rojo tenue; "Paso 0N / 06" lleva una línea roja a cada lado.
5. Toca "Gracias, pero no necesito ayuda": el video de la despedida queda al centro y pegado abajo (no flota ni se corta) y el texto con CERRAR va en un panel de vidrio encima.
6. Avisos, sonido, tiempos, resaltados, atajos, silenciar y ← INICIO funcionan igual que antes.

### Barra de volumen de la firma

1. Abre la app: la barra está en 25 con `Audio_01` cargada en pausa (no suena hasta ▶ o mover la barra).
2. Bajo "Neondev" (login, dashboard y sala) solo hay una bocina y una línea fina; sin número, sin canción, sin botones.
3. Pasa el mouse por la línea: aparece un punto pequeño. Arrastra o haz clic: el volumen sube y baja; el tramo rojo sigue al valor; la bocina cambia (0, bajo, alto).
4. Mouse sobre la bocina: cursor de mano, se pone blanca y el tooltip dice "Silenciar". Clic: la bocina sale tachada, la barra baja a 0 y el tooltip dice "Activar sonido". Clic otra vez: regresa al volumen que tenía.
5. Silencia y sube la barra: el silencio se quita solo.
6. Abre **05 MÚSICA**: su barra, su número y su nota marcan lo mismo que la firma (también el silencio). Cambia el volumen ahí y regresa: la firma lo refleja.
7. En la **sala** en pantalla completa: la firma no se encima con el contador ni se corta.

### Límite de caracteres (`Inputs:MaxCaracteres` = 15)

1. Login: escribe más de 15 caracteres en usuario y en contraseña: se detiene en 15. Pega un texto de 40: se recorta a 15.
2. Registro (sin cuenta): usuario, contraseña y respuesta secreta se detienen en 15, también pegando.
3. Configurar pregunta de una cuenta vieja: la respuesta se detiene en 15.
4. Recuperación: respuesta, contraseña nueva y repetir contraseña se detienen en 15, también pegando.
5. Cambia `Inputs:MaxCaracteres` a 5 en el `appsettings.json` junto al `.exe`, abre la app: los campos se detienen en 5. Con una cuenta guardada antes con más letras, ENTRAR dice "Usuario: máximo 5 caracteres." y la cuenta no se borra (`usuarios.json` intacto). Regrésalo a 15.
6. **04 EMOTES**: el campo del set de 7TV se detiene en 26 (`Inputs:MaxEmoteSetCaracteres`); un ID de 24 o 26 carga normal.

### 10b. Manual: capas, Lain transparente, despedida y letreros (08/11, noche)

1. **Fondo limpio** (09/11: el velo pasó a ser el fondo de color opaco de la sección 16): un solo fondo, sin degradados, sin viñeta, sin barrido, sin piso oscuro y sin texto ASCII sobre la página (el ASCII solo aparece dentro de la réplica del login en el paso 1). En FONDOS elige "sin fondo": el Manual se ve igual (mismo fondo de color).
2. **Lain sin cuadro negro**: en todos los pasos Lain flota sobre el fondo, pegada abajo, sin rectángulo ni orillas visibles, debajo de sus avisos y a la izquierda de la vista de ejemplo, sin tocar la fila de atajos ni "Gracias, pero no necesito ayuda". Pon un fondo claro para notarlo mejor.
3. **Respaldo**: renombra `gifs\lain-transparente.gif` y abre el Manual: aparece el video de Lain con una penumbra negra que se desvanece hacia arriba y a los lados (sin borde recto). Regrésale el nombre.
4. **Despedida**: "Gracias, pero no necesito ayuda" o ESC → el video de despedida al centro y abajo, tenue, con las cuatro orillas fundidas: sin rectángulo azul, sin bordes rectos y sin franja clara a la izquierda ni parches de otro tono a los lados. El panel con "Gracias por estar aquí. Vuelve pronto." y CERRAR se lee encima.
5. **Sin letreros de atajos**: pasa el mouse despacio por toda la pantalla del Manual (fondo, Lain, avisos, atajos, vista de ejemplo, botón de abajo, despedida) y por ← INICIO de Sala, Emotes, Música, Fondos y Manual: no debe salir ningún letrero "Enter", "Esc", "→" ni "←". Igual en Login (formularios de registro y recuperación), Dashboard, APLICAR de Emotes y el diálogo de confirmación (SALIR / ✕).
6. Los únicos letreros que sí salen: el logo Neondev ("Abrir el repositorio del proyecto") y las bocinas (firma, música y manual).
7. Los atajos siguen igual: ENTER avanza (y cierra en la despedida), → / ← cambian de paso, ESC abre la despedida y luego cierra; Esc en ← INICIO de las demás pantallas regresa; Enter en el set de Emotes aplica.

## 13. Validación estricta, portadas 4 y 5, volumen y pantalla completa (09/11)

### Solo letras y números en cada campo

Copia `hola mundo@2` y `ñandú  é!` al portapapeles y prueba en cada campo:

1. **ENTRAR** (usuario y contraseña): escribe un espacio, `@`, `.`, `_` o `-`: no aparece. Pega `hola mundo@2`: queda `holamundo2`; pega `ñandú  é!`: queda `ñandúé`. El cursor queda donde ibas.
2. **Registrar usuario**: usuario, contraseña y repetir contraseña, igual que el punto 1. Registra un usuario con ñ o acento: entra normal.
3. **Pregunta secreta** (primer ENTRAR de una cuenta sin pregunta): la respuesta no acepta espacios ni símbolos, tampoco pegando.
4. **¿Olvidaste tu contraseña?**: usuario, respuesta, contraseña nueva y repetir contraseña, igual. Con una respuesta guardada antes como "Mango Verde", contestar `MangoVerde` funciona.
5. **04 EMOTES**: el set de 7TV quita espacios, `/`, `:` y `.` (una URL pegada queda en un solo bloque y 7TV la rechaza). Un ID de 24 o 26 carga normal; se sigue deteniendo en 26.
6. Los límites siguen: 15 en los campos del login (26 en el set de 7TV), también pegando.
7. La muestra del Manual (paso 1) dice `hostdemo` y la contraseña de puntos se ve igual que antes.

### Portadas 4 y 5

1. **05 MÚSICA**: las canciones que usan `Audio_04` y `Audio_05` muestran al personaje sobre el mismo fondo oscuro que la 1, 2 y 3, sin cuadro blanco ni filo blanco alrededor del pelo o los hombros.
2. Portada grande: nítida (sin pixelar) y del mismo tamaño que las otras (las cinco miden 1024×1024 y salen a 300×300).
3. La 1, 2 y 3 se ven igual que antes.

### Volumen 25 % al abrir

1. Cierra y abre la app: suena sola `Audio_02` (`Audio:InitialTrack` = 2) y la barra de la firma marca 25 %.
2. Abre **05 MÚSICA**: su barra y su número dicen 25 y la canción marcada es `Audio_02`, sonando.
3. Silencia en la firma: barra en 0 y bocina tachada; reactiva: vuelve a 25. Lo mismo desde MÚSICA.
4. Renombra `Audio_02` (o pon uno ilegible con ese nombre) y abre la app: arranca con la primera o salta a la siguiente, sin crashear.

### Pantalla completa

1. La app abre en pantalla completa, sin barra de título ni botones de Windows, en el login y en todas las pantallas.
2. La ✕ de arriba a la derecha: sin sala pregunta "¿Salir de COT_0?"; con sala, "¿Cerrar la sala y salir?". CANCELAR no cierra. No tapa la firma ni otros botones en ninguna pantalla.
3. Alt+F4 hace lo mismo que la ✕.
4. SALIR del dashboard sigue pidiendo confirmación y regresa al login.
5. Atajos: ENTER en el login y en los formularios, Esc en ← INICIO, ENTER / → / ← / ESC en el Manual y Enter en el set de Emotes funcionan igual. Al pasar el mouse por la ✕ y por los botones no sale ningún letrero de atajo.
6. Se ve bien a la resolución del monitor (el diseño es de 1280×720: revisa que nada quede encimado ni perdido en una pantalla más grande).

## 14. Video en el Manual, audio inicial, cierre limpio y despedida (09/11)

### Video de fondo en el Manual

1. Elige un fondo de **video** en 02 CAMBIAR FONDO y abre **03 MANUAL**: el video sigue corriendo, mudo y tapado por el fondo de color (sección 16).
2. Regresa con **← INICIO** y con **ESC**: el video sigue donde iba (no vuelve al inicio ni parpadea) y con su sonido normal (hoy en silencio, `Background:VideoMuted` = true; ponlo en `false` para oírlo fuera del Manual y comprobar que en el Manual se calla).
3. Con un fondo de **imagen** o "Usar cuadro fijo", el Manual se ve igual (mismo fondo de color).

### Audio inicial

1. Abre la app: suena sola **Audio_02**. La barra de la firma marca 25 y en **05 MÚSICA** la barra y el número dicen 25 con Audio_02 marcada.
2. Silencia y reactiva desde la firma y desde MÚSICA: se calla y vuelve a 25.
3. En el Manual, el aviso de cada notificación suena y SILENCIAR lo calla, como antes.

### Cerrar sin errores

1. Cierra con la **✕** y confirma **SALIR**: la app se cierra sin ningún mensaje de error. Repite desde el login, el dashboard, la sala (con QR generado) y el Manual (también en la despedida).
2. Lo mismo con **Alt+F4**.
3. Con la sala abierta: "¿Cerrar la sala y salir?" → **CERRAR SALA Y SALIR**: se cierra limpio y `cloudflared.exe` ya no aparece en el Administrador de tareas.
4. Visor de eventos → Registros de Windows → Aplicación: no aparece "Error de aplicación" de `COT_0.Host.exe`.

### Despedida centrada

1. En el Manual elige "Gracias, pero no necesito ayuda" (o ESC): el video de Lain y el texto "Gracias por estar aquí. Vuelve pronto." quedan juntos y al centro exacto de la pantalla, el texto debajo del video, sin encimarse.
2. El video se ve más grande que antes, nítido, sin rectángulo negro ni orillas duras (las cuatro se pierden en el fondo de color).
3. En pantalla completa a 1920 × 1080 y en un monitor de 1280 × 720: nada se corta (en 720 el grupo se encoge completo) y el botón Cerrar funciona.

## 15. Música muteada en el Manual (09/11)

1. Con la música sonando, anota en qué segundo va (05 MÚSICA) y abre **03 MANUAL**: la música deja de oírse de inmediato; los avisos de Lain suenan normal y SILENCIAR los sigue callando.
2. Quédate ~20 s en el Manual y regresa con **← INICIO**: la música se oye otra vez **desde donde iba** (unos 20 s más adelante que al entrar), la misma pista, sin pausa ni reinicio, al mismo volumen (25 o el que tenías).
3. Repite saliendo con **ESC** (despedida → ESC/Cerrar) y con ENTER hasta el final: en la despedida suena la estática y la música sigue callada; al salir vuelve igual que en el punto 2.
4. **Silencio previo**: silencia con la bocina de la firma o de MÚSICA, entra al Manual y sal: la música **sigue silenciada** y la bocina sigue marcando silencio. Quita el silencio: se oye donde iba.
5. Si una canción termina estando en el Manual, pasa sola a la siguiente (en silencio) y al salir se oye la nueva.
6. **Varias veces seguidas**: entra y sal del Manual 5 veces rápido (← INICIO / 03 MANUAL): al final la música se oye normal (o sigue silenciada si lo estaba) y la bocina no cambió de icono en ningún momento.
7. **Cerrar dentro del Manual**: en un paso y en la despedida, cierra con la ✕ → SALIR y con Alt+F4: la app se cierra sin error y el Visor de eventos no registra "Error de aplicación" de `COT_0.Host.exe`.

## 16. Fondo de color en el Manual y Lain sin recuadro (09/11; el fondo cambió en la sección 17)

1. Con un fondo de **video** elegido, abre **03 MANUAL**: no se ve el video ni la imagen; el Manual tiene su propio fondo oscuro (brillo vino muy tenue arriba a la derecha que baja a negro). El texto y los paneles se leen bien.
2. **Video corriendo debajo**: anota por dónde va el video de fondo (en el Dashboard), entra al Manual ~15 s y sal con **← INICIO**: el video se ve otra vez, **más adelante** de donde estaba (no se reinició ni se pausó) y sin parpadeo. Repite saliendo con **ESC** y con Cerrar de la despedida.
3. **Lain (GIF)**: en los pasos, alrededor de Lain no se ve ningún recuadro ni línea; arriba y a los lados se funde con el negro del fondo.
4. **Lain (video de respaldo)**: renombra `gifs\lain-transparente.gif` y abre el Manual: aparece el video con sus franjas; no se nota el recuadro negro contra el fondo. Regrésale el nombre.
5. **Despedida**: ESC o "Gracias, pero no necesito ayuda": el video y el texto siguen centrados y del mismo tamaño; sus orillas se pierden en el fondo sin recuadro (arriba a la derecha el fondo es un poco más claro: revisa que no se vea un borde ahí).
6. Con fondo de **imagen**, "Usar cuadro fijo" o sin fondo: el Manual se ve igual que en el punto 1.
7. Cierra la app con la ✕ → SALIR estando en un paso y en la despedida: sin errores.

## 17. Fondo azul con estática en el Manual (09/11)

1. Con un fondo de **video** elegido, abre **03 MANUAL**: no se ve el video de la app; se ve el **azul noche** del Manual, con un barrido tenue que baja cada ~14 s, viñeta en las orillas y **estática fina** que tiembla encima (no debe verse como una imagen fija que se mueve en bloque ni dejar orillas sin grano). El texto, los avisos y la vista de ejemplo se leen bien.
2. **Lain (GIF)**: abajo a la izquierda Lain queda en una sombra negra que se desvanece hacia el azul; no se ve ningún recuadro, línea ni corte del grano alrededor de ella.
3. **Lain (video de respaldo)**: renombra `gifs\lain-transparente.gif` y abre el Manual: el video y sus franjas quedan dentro de la sombra, sin recuadro. Regrésale el nombre.
4. **Despedida** (ESC o "Gracias, pero no necesito ayuda"): video y texto centrados y del mismo tamaño que antes; sus orillas se pierden en el azul, sin cuadro negro ni borde; la sombra de la esquina ya no está.
5. **Al salir** (← INICIO, ESC y Cerrar de la despedida) después de ~15 s: el video de fondo se ve otra vez **más adelante** de donde iba (sin reinicio ni parpadeo) y la música se oye desde donde iba (o sigue silenciada si ya lo estaba).
6. **Ligereza**: con el Administrador de tareas abierto, quédate 1 min en el Manual: el uso de CPU no debe subir de forma notable comparado con el Dashboard.
7. Con fondo de **imagen**, "Usar cuadro fijo" o sin fondo: el Manual se ve igual que en el punto 1.

## 17. Reglas nuevas de usuario y contraseña (09/11)

### Usuario solo letras con mayúscula inicial

1. **Registrar usuario**: escribe `erick`: el campo muestra `Erick` al momento. Escribe `ERICK`: queda `Erick`. Escribe o pega `erick 12_@x`: queda `Erickx` (sin números, espacios ni símbolos). El cursor queda donde ibas.
2. Lo mismo en el usuario de ENTRAR y de ¿Olvidaste tu contraseña?
3. Registra `Erick` con una contraseña válida: entra normal; en `usuarios.json` queda `"UserName": "Erick"` y `"RulesVersion": 2`.

### 20 caracteres máximo (menos el set de 7TV)

1. Usuario, contraseña, repetir contraseña, respuesta secreta, contraseña nueva de la recuperación: se detienen en 20, también pegando un texto de 40.
2. **04 EMOTES**: el campo del set de 7TV se sigue deteniendo en 26.

### Contraseña con mensajes exactos

En Registrar usuario (y igual en la contraseña nueva de ¿Olvidaste tu contraseña?), con usuario `Erick`:

| Contraseña | InfoBar |
|---|---|
| `abc` | Mínimo 8 caracteres. Falta un número. Falta un carácter especial (! @ # $ % & * ? . - _). |
| `abcdefgh` | Falta un número. Falta un carácter especial (…). |
| `abcdefg1` | Falta un carácter especial (…). |
| `abcdefg!` | Falta un número. |
| `abcdef1!` | Entra (pasa todas). |

1. Escribe un espacio o pega `abc 12!`: el espacio no aparece (el filtro solo deja letras, números y `! @ # $ % & * ? . - _`). Un símbolo fuera de la lista (`^`, `/`, `+`) tampoco aparece.
2. El mensaje "No uses espacios." solo sale si el texto llega con espacios por código (el filtro los quita antes).

### Cuenta vieja que ya no cumple

1. Con un `usuarios.json` de antes (ej. `hostdemo` con contraseña `abc12345`): en ENTRAR escribe `hostdemo` (el campo lo deja `Hostdemo`) y su contraseña de siempre → InfoBar "Esta cuenta ya no cumple las reglas nuevas. Regístrala de nuevo." y no entra. Con contraseña equivocada → "Usuario o contraseña incorrectos." (no revela nada).
2. Registrar usuario con `Hostdemo` y una contraseña nueva válida → se registra (reemplaza la cuenta vieja con ese nombre, sin contar como usuario nuevo) y ya entra.
3. Una cuenta vieja con nombre válido (`Erick`) pero contraseña sin símbolo: también sale el aviso; se recrea igual o con ¿Olvidaste tu contraseña? (al cambiar la contraseña queda con las reglas nuevas).
4. Una cuenta vieja con números en el nombre (`user1`) no se puede escribir en ENTRAR (el campo quita los números): se registra otra con solo letras. Su entrada sigue en `usuarios.json` tal cual (no se borra).
5. Abrir la app con esas cuentas no cambia `usuarios.json`: solo se reescribe al registrar, cambiar contraseña o configurar la pregunta.

### Manual

1. La muestra del paso 1 dice `Hostdemo`.
