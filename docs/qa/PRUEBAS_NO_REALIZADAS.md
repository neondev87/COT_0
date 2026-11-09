# Pruebas no realizadas (revisión manual)

Todo lo demás se probó en la app real con UI Automation, teclado y capturas de la ventana, más un invitado en Edge headless por el túnel.

| # | Prueba | Por qué no | Cómo revisarla |
|---|---|---|---|
| 1 | Animación de vidrio dentro del avatar de la firma | Es una animación visual al pasar el mouse; las capturas estáticas no la muestran bien | Pasa el mouse por el logo en login, dashboard y sala |
| 2 | "Sin sonido hasta subir el volumen" (Audio_01 lista en silencio) | No hay forma de oír la salida desde el entorno de prueba | Abre la app y confirma que no suena nada con la barra en 0 |
| 3 | El punto de la barra aparece con el mouse | Requiere ver el hover en vivo | Pasa el mouse sobre la barra delgada de la firma |
| 4 | Sala sin internet | Desconectar la red habría cortado el entorno de prueba | Desconecta el Wi-Fi, abre la sala y revisa el InfoBar y REINTENTAR |
| 5 | QUITAR, IGNORAR y RESTAURAR de emotes | Hace falta que un invitado reporte un emote desde el chat | Reporta un emote desde el celular y usa los tres botones en Emotes |
| 6 | Música: sin canciones o carpeta faltante → InfoBar | Habría que mover la carpeta real `musica` o cambiar `Paths:MediaRoot` (prohibido tocar appsettings) | Renombra temporalmente `COT_0_P\musica` y abre Música |
| 7 | Fondos: elegir una imagen | La carpeta `fondos` solo tiene videos | Copia un .jpg a `COT_0_P\fondos`, RECARGAR y elígelo |
| 8 | Fondos: carpeta faltante o vacía → InfoBar | Misma razón que el 6 | Renombra temporalmente `COT_0_P\fondos` |
| 9 | Sonido de las notificaciones de Lain y "Silenciar el manual" | No se puede oír; el botón sí responde, pero su nombre accesible no cambia ("Silenciar el manual") | Abre el Manual con volumen del sistema y prueba el botón de bocina |

## No aplica

- **Tamaño mínimo, maximizada y "reduce y maximiza" (secciones 3, 8 y 11):** la ventana es fija de 1280×720 lógicos (`Services/Ventana/WindowSetup.cs`: `IsResizable=false`, `IsMaximizable=false`). Forzada por código a 300×200 se encima todo, pero un usuario no puede llegar a ese tamaño.

## También conviene revisar a mano

- **Celular real:** el invitado se simuló con Edge headless (InPrivate) por el túnel. Faltan escanear el QR con la cámara, subir foto desde la galería, notas de voz e imágenes.
- **Puntos blancos sobre el video:** en las capturas de los videos de fondo aparecen puntos blancos sueltos. Mira la ventana en vivo para saber si son reales o del método de captura.

## Notas del proceso

- Al probar el clic del logo se abrió una pestaña de `github.com/neondev87/COT_0` en tu Edge.
- El primer intento de Edge headless usó un perfil temporal que Edge firmó solo con tu cuenta de Windows. Lo cerré y borré el perfil de inmediato; los siguientes intentos fueron en InPrivate y sin sincronización.
