# Pruebas manuales (Host WinUI)

La solución no tiene proyectos de prueba: esto se revisa a mano con `dotnet run --project COT_0.HostWinUI` (o el .exe de `bin`).
Antes de empezar: `dotnet build COT_0.HostWinUI` en 0 errores y 0 warnings.

## Preparación

- En `COT_0.HostWinUI/appsettings.json`, `Paths:MediaRoot` apunta a tu carpeta `COT_0_P` (fuera del repo).
- Dentro: `musica\` con 2 o 3 canciones (.mp3/.wav/.m4a) y `portadas\` con al menos una imagen que se llame igual que una canción.
- Para probar errores, ten a mano un archivo que no sea audio renombrado a `.mp3` (por ejemplo, un .txt).

## Firma del login

1. Abre la app. Arriba a la derecha aparece el avatar y **Neondev**; debajo, la nota, la barra de volumen (25) y el nombre de la canción que suena.
2. La música empieza sola (con `Audio:DefaultFile` o la primera en orden alfabético).
3. Pasa el mouse por el avatar o el nombre: el nombre se pone rojo y el borde del avatar se aclara. Sin brillo ni desenfoque.
4. Clic en el nombre: se abre `https://github.com/neondev87/COT_0` en el navegador.
5. Clic en la nota: se silencia y el icono cambia a bocina tachada. Otro clic: vuelve a sonar.
6. Silencia y mueve la barra de volumen: el silencio se quita solo y el número cambia de 0 a 100.
7. Deja terminar una canción (o usa una corta): pasa a la siguiente y el nombre debajo de la firma cambia.
8. Sin música: cambia `Audio:MusicFolder` a una carpeta que no exista y abre la app. La firma dice **Sin música** y no hay barra de volumen.

## Música

1. Entra con tu cuenta y en el dashboard toca **05 MÚSICA**.
2. Se ve la lista con portada, título y duración; la que suena tiene una barra roja a la izquierda y su portada grande a la izquierda.
3. Clic en otra canción: empieza a sonar, la barra roja se mueve y cambian la portada grande y el título.
4. ▶‖ pausa y reanuda (el icono cambia). ◀ y ▶ van a la anterior y a la siguiente; en la última, ▶ vuelve a la primera.
5. Mueve el volumen aquí y regresa a INICIO → SALIR: en el login, la firma muestra el mismo volumen y la misma canción.
6. **Carpeta no encontrada**: con `Audio:MusicFolder` apuntando a una carpeta que no existe, abre MÚSICA → InfoBar amarillo con la ruta.
7. **Carpeta vacía**: con una carpeta que existe pero sin canciones → InfoBar "no tiene canciones".
8. **Archivo ilegible**: con el .txt renombrado a .mp3 en la carpeta, elígelo → InfoBar "No se pudo leer «…»" y salta a la siguiente.
9. Agrega una canción a la carpeta, regresa a INICIO y vuelve a abrir MÚSICA: aparece en la lista.
10. **INICIO** (o Esc) regresa al dashboard sin cortar la música.

## Emotes

1. En el dashboard toca **04 EMOTES**. Se ve SET DE 7TV con el campo, APLICAR, el estado y "0 emotes cargados" (o los del set por defecto de `Chat:EmoteSetId`).
2. Escribe un ID o una URL de set de 7TV (`https://7tv.app/emote-sets/{id}`) y presiona APLICAR o Enter: el estado dice CARGANDO… y luego **OK** en claro con el número de emotes.
3. ID inventado (ej. `abc`) → estado en rojo con el motivo (ID inválido o 404).
4. Sin internet → estado en rojo "SIN CONEXIÓN CON 7TV".
5. Campo vacío + APLICAR → **SIN EMOTES** y 0 cargados.
6. Reportes (necesita la sala abierta con un invitado; hoy la sala WinUI todavía no abre el túnel, así que esto se prueba con el Host WPF o cuando se migre la sala): un invitado mantiene presionado un emote en el chat → REPORTAR. Aparece en REPORTES con su miniatura y "1 reporte(s)".
7. QUITAR: el emote pasa a QUITADOS ("Se muestra como texto") y en el chat se ve como texto. RESTAURAR lo regresa.
8. IGNORAR: desaparece de REPORTES; si alguien lo vuelve a reportar, regresa.
9. Sin reportes ni quitados se ven "Sin reportes" y "No has quitado emotes".
10. **INICIO** (o Esc) regresa al dashboard.

## Sala

1. En el dashboard toca **01 GENERAR QR**: abre la sala con el encabezado ← INICIO | SALA. INICIO o Esc regresa.
