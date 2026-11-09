# Reporte QA – COT_0 (rama host-winui)

Fecha: 08/11/2026 · Commit probado: `cd4d4f4` · Build: `dotnet build COT_0.sln --no-incremental` → **0 errores, 0 warnings**.

## Correcciones (08/11/2026)

Commits en `host-winui`, uno por fallo. Cada uno compila con 0 errores y 0 warnings. Se volvieron a correr los pasos de prueba en la app con UI Automation y teclado real, y al final se restauraron los JSON de `%LocalAppData%\COT_0` (mismos hashes).

| Fallo | Estado | Commit | Verificación |
|---|---|---|---|
| F1 · ENTER en login, registro, recuperación y pregunta | **CORREGIDO** | `6366535` | ENTER envía en las cuatro (incluye los errores "no coinciden" y "Elige una pregunta secreta."); el clic en ENTRAR sigue funcionando |
| F2 · Puerto 5050 ocupado y REINTENTAR | **CORREGIDO** | `23be42a` | Ocupado: "El puerto 5050 ya está ocupado por otra app…" (también al reintentar); al liberarlo, REINTENTAR abre la sala, el túnel funciona y `/join` responde 200 |
| F3 · CERRAR SALA sin confirmación | **CORREGIDO** | `08904b5` | Diálogo "¿Cerrar la sala?…"; ESC cancela y la sala sigue; confirmar cierra la sala y el túnel |
| F4 · SALIR sin confirmación sin sala | **CORREGIDO** | `4717485` | Sin sala: "¿Cerrar sesión y volver al inicio de sesión?"; con sala: "Hay una sala activa…" (CANCELAR no cierra nada) |
| F5 · ✕ sin confirmación sin sala | **CORREGIDO** | `4717485` | Sin sala: "¿Salir de COT_0?"; con sala: "¿Cerrar la sala y salir?". Una segunda ✕ con el diálogo abierto no abre otro diálogo ni cierra la ventana; al confirmar, la app cierra y libera el 5050 y cloudflared |
| F6 · ENTER en el Manual regresa al inicio | **CORREGIDO** | `9c4c5aa` | Foco inicial en la página (ya no en ← INICIO); ENTER avanza 01 → 02 → 03; ESC, Cerrar y el clic en ← INICIO siguen igual |
| F13 · Accesos a disco síncronos | **CORREGIDO** | `7a635f6` | `JsonStore` y `MediaLog` crean la carpeta con `Task.Run`, y la fecha en `SongCatalog` también va con `Task.Run`. Los demás (`CacheFolders`, `FondoCatalog`, `FondoFrameCache`, `MusicLibrary`, `CoverLibrary`) ya corrían dentro de `Task.Run`. .NET no trae versión async de esas API. Mismo formato y misma ubicación de los JSON |
| F14 · Textos en inglés | **CORREGIDO** | `23be42a`, `b5227b1` | Mensaje del puerto en español; aviso de Blazor "Ocurrió un error inesperado. Recargar"; `/Error` en español; el error genérico del servidor ya no pega el mensaje en inglés de la excepción |
| F7–F12 | Sin cambios | — | Fuera de esta ronda |

Notas:
- F2: no se crea un servidor web nuevo en cada intento, porque el dashboard y la sala usan las mismas instancias del contenedor de la web (`IChatService`, `IRoomService`) y uno nuevo las dejaría desconectadas. En lugar de eso, `PortProbe` revisa el puerto antes de cada intento y el servidor solo arranca cuando el puerto está libre. Si el servidor falla al enlazar por otra causa, se repite el error original y no "Server has already started".
- **Hallazgo nuevo (no corregido, ya existía en `04fea46`)**: al cerrar la app desde el dashboard (con o sin sala, con o sin diálogo) Windows registra un crash `0xc000027b` en `Microsoft.UI.Xaml.dll` (Visor de eventos → Aplicación, Id 1000). Al cerrar desde el login no pasa. La app se cierra igual y libera el puerto y cloudflared, por eso no se nota.
- Observación: si la segunda ✕ le quita el foco al diálogo, ESC ya no lo cierra; CANCELAR con clic sí.

## Cómo se probó

- La app se corrió desde `COT_0.Host\bin\x64\Debug\...\COT_0.Host.exe` y se manejó **como persona** con UI Automation (clics, teclado real con SendKeys, escribir y pegar en campos, abrir y cerrar pantallas), con capturas solo de la ventana de COT_0.
- Invitado real: Edge headless (InPrivate) manejado por CDP, entrando por el **túnel público de Cloudflare** a `/join` y `/chat`.
- Lectura de código y de los JSON de `%LocalAppData%\COT_0` (respaldados antes y restaurados al final: 0 diferencias de hash).
- Datos de prueba: `pruebaunoxxxxxx`, `pruebadosyyyyyy`, `pruebatres`, `José`, `pruebacuatro`; invitados `InvitadoQA` y `<script>x</script> 😀`.

## Resumen ejecutivo

| Resultado | Pruebas |
|---|---|
| PASA | 97 |
| FALLA | 14 |
| NO PROBADO | 9 |
| NO APLICA (ventana fija 1280×720, no se puede maximizar ni redimensionar) | 3 |
| **Total** | **123** |

Fallos por gravedad: **2 críticos** (por regla del profe, impacto bajo), **6 mayores**, **4 menores**, **2 estéticos**. Ningún crash en toda la sesión (≈ 80 navegaciones, 17 cambios rápidos de fondo, 9 aperturas de sala).

## Resultados por sección

| # | Sección | Pasa | Falla | No probado | N/A |
|---|---|---|---|---|---|
| 1 | Login y usuarios | 23 | 5 | 0 | 0 |
| 2 | Dashboard y navegación | 7 | 0 | 0 | 0 |
| 3 | Firma y volumen | 7 | 1 | 3 | 1 |
| 4 | Sala, QR y sesiones | 9 | 3 | 1 | 0 |
| 5 | Emotes (7TV) | 7 | 1 | 1 | 0 |
| 6 | Música | 3 | 0 | 1 | 0 |
| 7 | Fondos | 10 | 0 | 2 | 0 |
| 8 | Manual de Lain | 10 | 1 | 1 | 1 |
| 9 | Inputs y validaciones | 3 | 0 | 0 | 0 |
| 10 | Datos, arquitectura y reglas | 12 | 2 | 0 | 0 |
| 11 | Ligereza y estabilidad | 6 | 1 | 0 | 1 |

### 1. Login y usuarios

| Prueba | Esperado | Obtenido | Resultado |
|---|---|---|---|
| Abrir la app | Login, botón ENTRAR, enlaces lado a lado | Igual; "¿Olvidaste tu contraseña? \| Registrar usuario" sin encimarse | PASA |
| Registro: campos | Solo nombre, contraseña, repetir | Exacto | PASA |
| Contraseñas distintas | Error claro | "Las dos contraseñas no coinciden." | PASA |
| Nombre repetido con otras mayúsculas (`PRUEBAUNOXXXXXX`) | Error | "Ya existe un usuario con ese nombre." | PASA |
| Nombre con espacios / con `!` | Error | "Tu usuario solo puede tener letras y números…" | PASA |
| Nombre con acentos (`José`) | — | Aceptado (é cuenta como letra). Ver observaciones | PASA |
| Nombre vacío | Error | "Escribe tu usuario." | PASA |
| Contraseña con espacio / `#` | Error | "Tu contraseña solo puede tener letras y números…" | PASA |
| 25 caracteres escritos y pegados (usuario y contraseña) | Se detiene en 15 | Queda en 15 en ambos casos | PASA |
| Registro exitoso | Regresa al login con el nombre | InfoBar "Usuario registrado…" y nombre puesto | PASA |
| Sexto usuario | Mensaje claro | "Ya hay 5 usuarios registrados, que es el máximo." (también al abrir el formulario) | PASA |
| Entrar con cada uno de los 5 | Dashboard | Los 5 entran (incluido `JOSÉ` en mayúsculas) | PASA |
| Contraseña incorrecta / usuario inexistente | Mismo error, sin revelar | Ambos: "Usuario o contraseña incorrectos."; limpia la contraseña | PASA |
| Campos vacíos | Error | "Escribe tu usuario." | PASA |
| Pregunta secreta en el primer ENTRAR | Lista + respuesta ≤ 15 | 6 preguntas; sin pregunta → "Elige una pregunta secreta."; vacía o solo espacios → "Escribe tu respuesta."; pegar 20 → 15 | PASA |
| Respuesta solo letras y números | Rechazar símbolos | **Acepta `Mango Verde!` y `<script>!`** | **FALLA (F7)** |
| Recuperar: usuario inexistente | Mensaje genérico | "No se pudo recuperar la contraseña con esos datos." | PASA |
| Recuperar: usuario sin pregunta | Mismo mensaje genérico | Misma rama de código que inexistente (`PasswordRecoveryService.GetQuestionKeyAsync`) | PASA |
| Pregunta de ESE usuario | La suya | `PRUEBATRES` → "¿En qué ciudad naciste?"; el usuario queda bloqueado | PASA |
| Respuesta con espacios y mayúsculas (`' MONTER rey '` vs `Monterrey`) | Aceptada | Aceptada | PASA |
| Respuesta incorrecta | Error | "La respuesta no coincide." | PASA |
| Contraseña nueva distinta / con espacio | Error | "no coinciden" / "solo letras y números" | PASA |
| Entrar después del cambio | Nueva sí, vieja no | Vieja → incorrecta; nueva → dashboard | PASA |
| usuarios.json | Solo hash + salt, válido, 5 cuentas | 5 cuentas; hash 44 y salt 24 caracteres en Base64 (PBKDF2-SHA256, 100 000 iteraciones); 0 coincidencias de contraseñas o respuestas en texto plano | PASA |
| SALIR | Pide confirmación | **Sin sala activa vuelve al login sin preguntar** | **FALLA (F4)** → corregido `4717485` |
| Cerrar con ✕ | Pide confirmación | **Sin sala activa cierra sin preguntar** (con sala activa sí pregunta) | **FALLA (F5)** → corregido `4717485` |
| ENTER en login, registro, recuperación y pregunta | Envía el formulario | **No hace nada en ninguna de las cuatro** | **FALLA (F1)** → corregido `6366535` |
| Formulario con InfoBar a 1280×720 | Sin cortes | Título pegado al borde superior y "Volver al inicio de sesión" casi toca el pie | **FALLA (F12)** |

### 2. Dashboard y navegación

| Prueba | Resultado |
|---|---|
| Reloj con segundos, fecha, estado, 6 tarjetas y pie completos | PASA |
| Ninguna tarjeta abre una pantalla rota (GENERAR QR, CAMBIAR FONDO, MANUAL, EMOTES, MÚSICA, SALIR) | PASA |
| CAMBIAR FONDO abre Fondos | PASA |
| Ida y vuelta Dashboard ↔ Fondos/Emotes/Música/Manual/Sala con ← INICIO (80 navegaciones, 0 fallos) | PASA |
| ESC regresa al inicio en Fondos, Emotes y Música | PASA |
| ESC en Manual abre la despedida y "Cerrar" regresa | PASA |
| Firma de Neondev completa en el dashboard | PASA |

### 3. Firma de Neondev y volumen

| Prueba | Resultado |
|---|---|
| Firma completa en login y dashboard | PASA |
| Firma en la Sala sin encimarse | **FALLA (F11)**: tapa "Personas conectadas" |
| Maximizada | N/A: la ventana no se puede maximizar (`WindowSetup`: `IsMaximizable=false`, `IsResizable=false`) |
| Clic en el logo → `https://github.com/neondev87/COT_0` desde `Branding:RepoUrl` | PASA (se abrió "neondev87/COT_0" en Edge) |
| Animación de vidrio dentro del avatar | NO PROBADO |
| Solo bocina y barra delgada | PASA |
| Abre con volumen 0 y Audio_01 lista | PASA (barra en 0 al reiniciar; Audio_01 seleccionada) |
| Sin sonido hasta subir el volumen | NO PROBADO (no se puede oír) |
| Bocina silencia y reactiva recordando el volumen (40 → 0 → 40) | PASA |
| Mover la barra con mudo activo quita el mudo | PASA |
| El punto aparece con el mouse | NO PROBADO |
| Volumen único Firma ↔ Música (30 se ve en Música; 15 en Música se ve en la Firma) | PASA |
| El mudo no se guarda entre sesiones | PASA (al reiniciar el botón dice "Silenciar") |

### 4. Sala, QR y sesiones

| Prueba | Resultado |
|---|---|
| Al entrar: servidor, túnel (≈14 s la primera vez, ≈4 s después), QR, enlace, COPIAR ("COPIADO" y portapapeles correcto), código y contador EN VIVO | PASA |
| Sin cloudflared (PATH sin cloudflared): InfoBar en español con la causa y REINTENTAR, sin crash | PASA |
| Sin internet | NO PROBADO |
| Puerto 5050 ocupado | **FALLA (F2)** → corregido `23be42a` |
| Invitado por el túnel: aparece en la lista, avatar con iniciales, "hace 0 min" → "hace 1 min" | PASA |
| ✕ expulsa al invitado (le aparece "El anfitrión te expulsó de la sala.") | PASA |
| CERRAR SALA pide confirmación | **FALLA (F3)** → corregido `08904b5` |
| CERRAR SALA cierra la sesión, detiene el túnel (530 en la URL pública) y `/join` local dice "La sala está cerrada" | PASA |
| sesiones.json: 4, 5, 6, 7, 8 y 9 consecutivas, con inicio, fin e invitados | PASA |
| Solo invitados, no el host | PASA |
| visitas.json se escribe y es JSON válido | PASA |
| Barra inferior (EMOTES, MÚSICA, CAMBIAR FONDO, aviso) y CERRAR SALA completos | PASA (el aviso se parte en dos líneas, sin cortarse) |
| Encabezado coherente cuando falla la apertura | **FALLA (F9)**: dice "SALA ACTIVA" |

### 5. Emotes (7TV)

| Prueba | Resultado |
|---|---|
| Panel sin textos encimados | PASA |
| Límite de 26 caracteres al escribir y al pegar (40 → 26) | PASA |
| `global` → "45 emotes cargados · OK" | PASA |
| ID válido `01FE9DRF000009TR6M9N941CYW` → "964 emotes cargados · OK" | PASA |
| ID inválido (`zzzz`, `../../etc`, 25 caracteres) → "ESE ID O URL NO ES DE UN SET DE 7TV"; ULID inexistente → "(404)" | PASA |
| Campo vacío o con espacios → "SIN EMOTES", 0 cargados, sin crash | PASA (ver observaciones) |
| Pegar una URL de 7TV | **FALLA (F8)** |
| REPORTES y QUITADOS vacíos ("Sin reportes", "No has quitado emotes") | PASA |
| QUITAR, IGNORAR y RESTAURAR con reportes reales | NO PROBADO |

### 6. Música

| Prueba | Resultado |
|---|---|
| Biblioteca con portada y duración (Audio_01 0:55, Audio_02 1:46, Audio_03 0:56) | PASA |
| Reproducir/pausar, siguiente (vuelve al inicio después de la última), anterior y elegir fila | PASA |
| Portada grande sin pixelarse ni recortarse | PASA |
| Sin canciones o carpeta faltante → InfoBar | NO PROBADO |

### 7. Fondos

| Prueba | Resultado |
|---|---|
| Miniaturas, nombre limpio, tamaño y PESADO solo en Gojo (151 MB > 120 MB) | PASA |
| Elegir imagen | NO PROBADO (no hay imágenes en la carpeta de fondos) |
| Elegir video (Batman, Gojo 4K, Chill) | PASA |
| "Usar cuadro fijo" se guarda (`UseStill: true`) | PASA |
| SIN FONDO (`SelectedFile: ""`) | PASA |
| RECARGAR (3 fondos) | PASA |
| El video de Gojo abre | PASA |
| Persistencia en fondos.json (JSON válido) y fondo visible en Dashboard, Sala y Fondos | PASA |
| Carpeta faltante o vacía → InfoBar | NO PROBADO |
| Un solo video a la vez y pausa fuera de Dashboard/Sala/Fondos | PASA (un solo `MediaPlayer` en `FondoPlayer`; Emotes y Música sin video) |
| Checkbox y selección en rojo del proyecto | PASA |
| 17 cambios rápidos de fondo sin crash ni InfoBar | PASA |

### 8. Manual de Lain

| Prueba | Resultado |
|---|---|
| 6 pasos con vista de ejemplo (login, inicio, sala, emotes, manual, música) | PASA |
| Paso 1: réplica con los dos enlaces; aviso 2 "Si es tu primera vez, toca Registrar usuario." resalta "Registrar usuario" | PASA |
| Sin texto duplicado bajo el título | PASA |
| Las notificaciones avanzan solas (3 por paso) | PASA |
| → y ← (← deshabilitada en el paso 1, → en el 6) | PASA |
| ENTER avanza | **FALLA (F6)** → corregido `9c4c5aa` |
| ESC → despedida; "Cerrar" → dashboard | PASA |
| Sonido de notificaciones y "Silenciar el manual" | NO PROBADO (no se puede oír) |
| "Gracias, pero no necesito ayuda" → despedida | PASA |
| ← INICIO | PASA |
| Lain pegada abajo sin flotar, también en la despedida | PASA |
| Fondo más transparente con velo y paneles de vidrio | PASA |
| Reducir y maximizar | N/A (ventana fija) |

### 9. Inputs y validaciones

| Prueba | Resultado |
|---|---|
| Límite al escribir y al pegar en login, registro, recuperación, respuesta (15) y 7TV (26) | PASA |
| Emoji, comillas, `<script>`, espacios al inicio/fin en usuario y contraseña → rechazados con mensaje; invitado `<script>x</script> 😀` se muestra como texto en host y chat y se guarda escapado en JSON | PASA (la respuesta secreta acepta símbolos, ver F7) |
| Rutas `..\` / `../` en 7TV → "no es un set" | PASA |

### 10. Datos, arquitectura y reglas

| Prueba | Resultado |
|---|---|
| usuarios, visitas, sesiones y fondos son JSON válido (nunca XML) | PASA |
| Persisten al reiniciar (5 usuarios, fondo elegido) | PASA |
| `XmlSerializer` o datos `.xml`: 0 | PASA |
| E/S síncrona fuera de `File.Exists`/`Directory.Exists` | **FALLA (F13)** → corregido `7a635f6` |
| `.Result`, `.Wait()`, `GetAwaiter().GetResult()`: 0 | PASA |
| Contraseñas en texto plano en código, logs o JSON: 0 | PASA |
| Una clase por archivo | PASA (solo `KillOnCloseJob.cs` y `ChatBrowser.cs` tienen un tipo privado anidado) |
| Todos los `.cs` con `///` en español | PASA |
| "WinUI", "Wpf" o nombres de migración solo en `<UseWinUI>` | PASA |
| Carpetas de primer nivel del Host: Assets, Models, Services, Views | PASA |
| Subcarpetas en español | PASA |
| Sin voseo y sin textos de interfaz en otro idioma | **FALLA (F14)** → corregido `23be42a`, `b5227b1` |
| Música, fondos, portadas, gifs y manual fuera de `git ls-files` y en `.gitignore` | PASA |
| Build 0 errores y 0 warnings | PASA |

### 11. Ligereza y estabilidad

Mediciones (`Get-Process`, CPU como % del total de 12 núcleos):

| Escenario | Working set | Privada | CPU |
|---|---|---|---|
| Dashboard en reposo con video de fondo | 351 MB | 347 MB | 0.5 % |
| Sala abierta con QR y túnel | 459 MB | — | 0.6 % |
| Dashboard con video y música sonando | 455 MB | 309 MB | 0.5 % (56 hilos) |
| Fondos justo después de 17 cambios (Gojo 4K) | 744 MB pico → 612 MB a los 30 s | 446 MB | 0.4 % |
| 20 rondas de navegación (80 pantallas) | 394 → 416 MB | 231 → 262 MB, estable desde la ronda 10 | — |

| Prueba | Resultado |
|---|---|
| Sin desenfoque, acrílico, sombras ni shaders (búsqueda en XAML) | PASA |
| Abrir y cerrar cada pantalla 20 veces: sin crash, sin bloqueo y memoria estable | PASA (los handles suben de 2379 a 2898, ver observaciones) |
| Cambio rápido de fondos | PASA |
| Abrir y cerrar la sala 6 veces | PASA |
| El puerto 5050 y cloudflared se liberan al cerrar la app | PASA (0 escuchas y 0 procesos) |
| Logs sin excepciones | **FALLA (F10)** |
| Tamaño mínimo y maximizada | N/A (ventana fija 1280×720) |

## Fallos ordenados por gravedad

### Críticos (rompen una regla del profe; impacto bajo en uso)

**F13 · Quedan accesos a disco síncronos** — CORREGIDO (`7a635f6`)
- Pasos: buscar en el código `Directory.CreateDirectory`, `EnumerateFiles`, `File.Move`, `File.GetLastWriteTimeUtc`, `FileInfo`.
- Esperado: solo `File.Exists`/`Directory.Exists`.
- Obtenido:
  - `COT_0.Host/Services/Almacenamiento/JsonStore.cs:19` (`Directory.CreateDirectory` en el getter `Folder`, en el hilo que llama a cada Load/Save).
  - `Services/Cache/CacheFolders.cs:18`.
  - `Services/Musica/SongCatalog.cs:34` (`File.GetLastWriteTimeUtc`).
  - `Services/Fondos/FondoFrameCache.cs:45` y `:54`.
  - `Services/Fondos/FondoCatalog.cs:31`.
  - `Services/Musica/MusicLibrary.cs:19` y `CoverLibrary.cs:20`.
  - Varios están dentro de `Task.Run` y no bloquean la UI, pero siguen siendo API síncronas.

**F14 · Textos de interfaz en inglés** — CORREGIDO (`23be42a`, `b5227b1`)
- Pasos: (a) ocupar 127.0.0.1:5050 y abrir la sala; (b) cualquier error de circuito en la web.
- Esperado: todo en español de México.
- Obtenido: (a) "No se pudo levantar el servidor web: **Server has already started.**" (b) `COT_0.Web/Views/Plantillas/MainLayout.razor:7-8` "An unhandled error has occurred. Reload".
- Causa probable: `Text.Room.Error.ServerFailed` (`Views/Estilos/Textos.xaml:124`) pega `ex.Message` ({3}) desde `RoomOpenErrors.Message`.

### Mayores

**F1 · ENTER no envía ningún formulario del login** — CORREGIDO (`6366535`)
- Pasos: escribir `pruebacuatro`/`abc123` con el teclado, foco en la contraseña, presionar ENTER (igual en Registrar, Recuperar y pregunta secreta).
- Esperado: ENTRAR / REGISTRAR / CONTINUAR / GUARDAR Y ENTRAR.
- Obtenido: no pasa nada (sin error y sin navegar). Con clic sí funciona.
- Causa probable: hay tres `KeyboardAccelerator Key="Enter"` en el mismo árbol de la página (`Views/Pantallas/LoginPage.xaml:159`, `Views/Componentes/FormularioRegistro.xaml:31`, `FormularioRecuperacion.xaml:50`), dos de ellos dentro de paneles colapsados; el acelerador no llega al botón visible.

**F2 · Puerto 5050 ocupado: mensaje equivocado y REINTENTAR nunca se recupera** — CORREGIDO (`23be42a`)
- Pasos: ocupar 127.0.0.1:5050 y ::1:5050, abrir la app, entrar, GENERAR QR; liberar el puerto y presionar REINTENTAR.
- Esperado: InfoBar "puerto ocupado" y que REINTENTAR funcione al liberarlo.
- Obtenido: "No se pudo levantar el servidor web: Server has already started." (antes y después de liberar el puerto); hay que reiniciar la app. El log sí trae `AddressInUseException`.
- Causa probable: `App.xaml.cs:99` (`TryStartWebServerAsync`) arranca Kestrel al abrir la app y se traga el error; `Services/Sala/WebServerHost.cs` `StartAsync` reutiliza la misma `WebApplication` (el `Lazy` no se recrea tras fallar), así que el segundo intento lanza `InvalidOperationException` y `RoomOpenErrors.Classify` ya no lo reconoce como `PortInUse`.

**F3 · CERRAR SALA no pide confirmación** — CORREGIDO (`08904b5`)
- Pasos: abrir la sala con un invitado y presionar CERRAR SALA.
- Esperado: diálogo de confirmación.
- Obtenido: cierra al instante, saca a todos y regresa al dashboard.
- Causa probable: `Views/ModelosDeVista/SalaViewModel.cs` `CloseRoomAsync` no llama a `RoomSession.ConfirmLeaveAsync`.

**F4 · SALIR no pide confirmación sin sala activa** — CORREGIDO (`4717485`)
- Pasos: dashboard sin sala → SALIR.
- Esperado: confirmación.
- Obtenido: vuelve al login directo.
- Causa probable: `Views/ModelosDeVista/DashboardViewModel.cs` `LogoutAsync` solo confirma si `_session.IsActive`.

**F5 · Cerrar con ✕ no pide confirmación sin sala activa** — CORREGIDO (`4717485`)
- Pasos: cualquier pantalla sin sala → ✕ de la ventana.
- Esperado: confirmación.
- Obtenido: la app se cierra.
- Causa probable: `Services/Ventana/CloseGuard.cs` `OnClosing` solo confirma con `session.IsActive`. Con sala activa sí aparece "Hay una sala activa. ¿Cerrar la sala y salir?" y ESC lo cancela bien.

**F6 · ENTER en el Manual regresa al inicio en vez de avanzar** — CORREGIDO (`9c4c5aa`)
- Pasos: abrir MANUAL y presionar ENTER.
- Esperado: siguiente paso (la guía dice "ENTER empezar").
- Obtenido: vuelve al dashboard.
- Causa probable: al navegar, el foco queda en el botón "← INICIO" de `Views/Componentes/EncabezadoPantalla.xaml` (verificado con UI Automation: `FocusedElement = INICIO`); ENTER activa ese botón antes que el acelerador de `Views/Pantallas/ManualPage.xaml:27`.

### Menores

**F7 · La respuesta secreta acepta símbolos**
- Pasos: primer ENTRAR, respuesta `Mango Verde!` (o `<script>!`), GUARDAR Y ENTRAR.
- Esperado: "solo letras y números".
- Obtenido: se guarda (hash de la respuesta normalizada con `!`).
- Causa probable: `Services/Validaciones/SecurityAnswerValidator.cs` `ValidateAnswer` no revisa caracteres (solo vacío y largo).

**F8 · No cabe una URL de 7TV aunque el mensaje la pide**
- Pasos: pegar `https://7tv.app/emote-sets/01FE9DRF000009TR6M9N941CYW`.
- Esperado: aceptarla (el error dice "ESE ID O URL…" y `EmoteSetId.Parse` sí entiende URLs).
- Obtenido: se corta a 26 caracteres.
- Causa probable: `Inputs:MaxEmoteSetCaracteres = 26` en `appsettings.json` contra `COT_0.Web/Services/Emotes/EmoteSetId.cs` y `Textos.xaml:138`.

**F9 · "SALA ACTIVA" con la sala sin abrir**
- Pasos: abrir la sala con error (puerto o cloudflared).
- Esperado: título que no diga activa.
- Obtenido: el encabezado sigue diciendo "SALA ACTIVA" junto al InfoBar de error.
- Causa probable: título fijo en `Views/Pantallas/SalaPage.xaml`.

**F10 · Excepción no manejada en un circuito Blazor al cerrar la sala**
- Pasos: invitado conectado → CERRAR SALA.
- Obtenido en la consola: `fail: RemoteNavigationManager … Navigation failed when changing the location to /join?reason=closed … TaskCanceledException` y `Unhandled exception in circuit`.
- Causa probable: la web navega al invitado cuando la conexión ya se está cerrando (COT_0.Web, aviso de sala cerrada); no afecta al host.

### Estéticos

**F11 · La firma tapa "Personas conectadas" en la Sala** (`capturas/02_sala_firma_tapa_personas_conectadas.png`)
- Pasos: abrir la sala a 1280×720 con 0 conectados.
- Obtenido: el bloque de la firma se dibuja encima y el texto queda "Personas conectada…"; con "01" apenas cabe.
- Causa probable: `Views/Pantallas/SalaPage.xaml:125` (columna de la firma sin espacio reservado junto al contador).

**F12 · Formulario del login apretado con InfoBar abierto** (`capturas/01_login_registro_infobar_pegado.png`)
- Pasos: Registrar usuario con error visible.
- Obtenido: "COT_0" queda pegado al borde superior y "Volver al inicio de sesión" a 7 px del pie.
- Causa probable: `Views/Pantallas/LoginPage.xaml`, el formulario va centrado en vertical sin `ScrollViewer` ni margen mínimo.

`capturas/03_sala_puerto_ocupado_mensaje_ingles.png` muestra F2, F9 y F14.

## Observaciones (no cuentan como fallo)

- `char.IsLetterOrDigit` acepta acentos y letras de otros alfabetos en usuario y contraseña (`José` se registró). Si solo se quiere a-z/0-9, hay que ajustar `CredentialValidator.IsAlphanumeric`.
- APLICAR con el campo de 7TV vacío quita el set ("SIN EMOTES") en lugar de mostrar un error; parece intencional.
- Kestrel corre desde que abre la app aunque la sala esté cerrada (diseño documentado en `RoomSession`); `/join` responde "La sala está cerrada".
- La sesión #3 del respaldo original tiene `EndedAt: null` (no se reprodujo: cerrar con ✕ o CERRAR SALA siempre guardó el fin; probablemente fue un cierre forzado).
- Los handles suben unos 6 por navegación (2379 → 2898 en 80 pantallas) aunque la memoria se mantiene estable.
- Las tarjetas de Fondos se anuncian a UI Automation como `FondoRowViewModel` y los botones EMOTES/MÚSICA/CAMBIAR FONDO de la Sala no tienen nombre accesible.
- Nombre de fondo: conserva el prefijo "recorte" de `recorte_Jujutsu…`.
- En el paso 6 del Manual Lain dice "la música arranca en silencio", pero la réplica muestra el volumen en 25 (`Manual:Demo:Volume`).
- El atajo dice "ENTER empezar" en todos los pasos.
- En las capturas de videos aparecen puntos blancos sueltos sobre el fondo; puede ser un artefacto de la captura (`PrintWindow`) con video por hardware. Revísalo a ojo.
- No hay límite de intentos en la recuperación de contraseña.
