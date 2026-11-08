# Pruebas manuales (Host WinUI)

No hay proyectos de prueba: esto se revisa a mano con `dotnet run --project COT_0.HostWinUI`.
Antes: `dotnet build COT_0.HostWinUI` en 0 errores y 0 warnings.

## Preparación

- `Paths:MediaRoot` en `COT_0.HostWinUI/appsettings.json` apunta a tu carpeta `COT_0_P` (fuera del repo), con `musica\` (2 o 3 canciones) y `portadas\`.
- `cloudflared` instalado (`winget install Cloudflare.cloudflared`) o su ruta en `Tunnel:CloudflaredPath`.
- Para empezar de cero, borra `%LocalAppData%\COT_0\` (ahí viven `usuarios.json`, `visitas.json` y `sesiones.json`).
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
