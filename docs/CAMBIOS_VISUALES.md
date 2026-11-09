# Cambios visuales en la migración a WinUI 3

Lo que WinUI 3 no soporta igual que WPF y cómo se simplificó. Prioridad: que arranque rápido y no se trabe.

Nota histórica: la columna "WPF" habla del Host viejo, que ya se borró (está en el tag `pre-cierre-winui`). Las rutas de la columna "WinUI 3" son las de `COT_0.Host` final (ver `ESTRUCTURA.md`).

## Login

| WPF | WinUI 3 | Motivo |
|---|---|---|
| Ventana sin bordes (`ChromeWindow` + `CaptionBar` propia) | Contenido extendido a la barra de título; botones de Windows encima | No hace falta dibujar la barra a mano |
| Niebla con `BlurEffect` (radio 30 y 36) | Las mismas elipses con degradado radial, sin blur | WinUI 3 no tiene `BlurEffect`; el degradado ya se ve suave |
| Ceniza flotando (`AshLayer`, 30 partículas) | Quitada | Control propio con `OnRender`; se puede volver a poner con elipses animadas |
| Scanlines (`DrawingBrush` en mosaico de 3 px) | Quitadas | WinUI 3 no tiene `DrawingBrush` |
| `SpacedText` (letter-spacing dibujado a mano) | `TextBlock.CharacterSpacing` | Mismo efecto, nativo |
| `EntryAnimation` (cada bloque sube 18 px con retraso) | `EntranceThemeTransition` escalonada | Animación del sistema, mismo resultado |
| Campo con plantilla propia (línea inferior que pasa al acento) | `TextBox`/`PasswordBox` de WinUI con sus colores cambiados | Mismo look sin rehacer la plantilla |
| Error en un `TextBlock` rojo | `InfoBar` de error | Pedido del cambio |
| Firma (avatar + música) y selector de canciones | Firma migrada (ver "Firma del login"); el selector pasó a `MusicaPage` | Ver abajo |

## Diálogo de confirmación

| WPF | WinUI 3 | Motivo |
|---|---|---|
| `ConfirmWindow`: ventana propia sin bordes, centrada sobre la principal | `ContentDialog` dentro de la misma ventana, con velo oscuro detrás | WinUI 3 no tiene ventanas modales con dueño; el `ContentDialog` es lo nativo |
| Botón de confirmar inclinado (`SlantedShape`) con brillo de acento (`DropShadowEffect`) | Rectángulo con relleno tenue y borde de acento; con el mouse, acento sólido y se corre 6 px | WinUI 3 no tiene `DropShadowEffect` y `SlantedShape` dibuja con `OnRender` |
| Botones alineados a la derecha (CANCELAR, luego confirmar) | Mismo orden, pero confirmar ocupa la mitad del ancho | El `ContentDialog` reparte sus botones en columnas iguales |
| Su propia barra de título con ✕ | Sin barra: se cierra con CANCELAR o Esc | Ya no es una ventana aparte |

## Lista de conectados

| WPF | WinUI 3 | Motivo |
|---|---|---|
| `ItemsControl` dentro de un `ScrollViewer` | `ListView` sin selección; el host va en el `Header` | Pedido del cambio; el `ListView` ya trae su scroll |
| Tonos de avatar con `AlternationIndex` + `DataTrigger` | El ViewModel calcula el tono (`Tone`) y `AvatarTones.For` da el pincel | WinUI 3 no tiene `AlternationIndex` ni `DataTrigger` |
| Botón ✕ invisible hasta pasar el mouse por la fila | Siempre visible en gris tenue; se pone del color de acento con el mouse | Sin `DataTrigger` sobre `IsMouseOver` del padre; además así se ve que se puede expulsar |
| `SpacedText` en los encabezados | `TextBlock.CharacterSpacing` | Mismo efecto, nativo |

## Login: pregunta secreta y recuperación (pantallas nuevas, no existían en el WPF)

| Qué | Cómo se ve | Motivo |
|---|---|---|
| Pregunta secreta al registrarte | `ComboBox` y `TextBox` con el mismo fondo tenue y línea inferior que usuario y contraseña | Misma familia visual que el login, sin plantillas nuevas |
| "¿Olvidaste tu contraseña?" | Enlace de solo texto bajo la contraseña; se aclara con el mouse | Sin subrayado ni fondo de hover del `HyperlinkButton` del sistema |
| Recuperar contraseña | Cambia el contenido del mismo formulario (no hay ventana aparte): pregunta → respuesta → contraseña nueva dos veces | Sin diálogos encima del fondo animado |
| Contraseña cambiada | `InfoBar` verde de éxito | Mismo componente que los errores |
| Más campos en REGISTRARME | El formulario crece hacia arriba y abajo (sigue centrado) | Cabe en 1280×720 sin scroll |

## Dashboard

| WPF | WinUI 3 | Motivo |
|---|---|---|
| Fondo de video en loop con `BlurEffect` y 12 `DataTrigger` (en `MainWindow`) | `CapaFondo` detrás del `Frame` (ver "Fondos"); sin fondo elegido, el degradado radial del login + viñeta | Ver "Fondos" |
| `MenuCard` con `Clip` en forma de paralelogramo | Dos `Path` con la forma (cara clara y capa de acento) | En WinUI `UIElement.Clip` solo acepta rectángulos |
| Brillo de acento al pasar el mouse (`DropShadowEffect`, radio 16) | Quitado: solo cambia a acento sólido y se corre 18 px | WinUI 3 no tiene `DropShadowEffect`; `ThemeShadow` es caro y no da brillo de color |
| Hover con `Trigger IsMouseOver` + 2 `Storyboard` (entrada/salida) | `VisualStateManager` (`PointerOver`, `Pressed`, `Disabled`) con transición de 0.35 s | Lo nativo de WinUI; un solo lugar para el estado |
| `SpacedText` en títulos, fecha y estado | `TextBlock.CharacterSpacing` | Mismo efecto, nativo |
| Estado activo/inactivo con `DataTrigger` (`InactiveStatus`) | Dos propiedades del ViewModel (`IsRoomActive` / `IsRoomInactive`) con `x:Bind` a `Visibility` | WinUI 3 no tiene `DataTrigger` |
| Punto con pulso (`EventTrigger` en una plantilla) | Componente `PuntoPulso` que arranca su `Storyboard` en `Loaded` | WinUI no tiene `EventTrigger` en plantillas; escala y opacidad corren en el compositor |
| Entradas con `EntryAnimation` (tarjetas desde -408 px con retrasos fijos; reloj y estado con fundido de 1.2–1.4 s) | `EntranceThemeTransition` escalonada desde -120 px; reloj y estado con la transición del sistema | Animación del sistema, sin código propio |
| Tarjeta CAMBIAR FONDO abre el selector flotante | Tarjeta 02 abre la pantalla FONDOS | Ver "Fondos" |
| GENERAR QR abre túnel + sala y luego cambia de vista | Lleva a `SalaPage` y ella abre la sala al cargar | El aviso de "abriendo" y los errores se ven en la sala |
| Firma con música (`SignatureBadge`) sobre el pie | Solo en el login (ver "Firma del login"); en el dashboard está la tarjeta 05 MÚSICA | Una sola firma; la biblioteca es su pantalla |

## Dashboard: tarjetas nuevas

| WPF | WinUI 3 | Motivo |
|---|---|---|
| 4 tarjetas (QR, fondo, manual, salir) | 6 tarjetas: se suman 04 EMOTES y 05 MÚSICA; SALIR pasa a 06 | Emotes y música ahora son pantallas propias |
| Regresar a INICIO solo desde la sala | `EncabezadoPantalla` (← INICIO \| TÍTULO) en Sala, Emotes y Música; Esc también regresa | Una sola pieza para todas las pantallas internas |
| `BackLinkButton` con `Trigger IsMouseOver` + 2 `Storyboard` | `TextButton` con `VisualStateManager` (se aclara y se corre 4 px) | WinUI 3 no tiene `Trigger` en estilos |

## Emotes

| WPF | WinUI 3 | Motivo |
|---|---|---|
| Bloque dentro de la sala (`EmotePanel`) | Pantalla `EmotesPage` con el componente `PanelEmotes` | Pedido de la migración; la sala queda más limpia |
| Color del estado con 3 `DataTrigger` (OK / error / resto) | `EmoteStatusBrushes.For(Status)` con `x:Bind` | WinUI 3 no tiene `DataTrigger` |
| "Sin reportes" / "No has quitado emotes" con `DataTrigger` sobre `Tag` | Propiedades `NoReports` / `NoRemoved` del ViewModel → `Visibility` | Igual |
| Filas que entran con opacidad + desplazamiento (`EventTrigger Loaded`) | Filas sin animación de entrada | Sin `EventTrigger`; las listas son cortas y así no parpadean al refrescar |
| Fondo tenue al pasar el mouse por la fila | Quitado | Las filas van en `ItemsControl` (dentro de un solo scroll); el `ListView` anidado se comía la rueda del mouse |
| Botones de fila con `RelativeSource AncestorType` al comando | `Click` en el componente que pasa la fila al comando | WinUI 3 no tiene `AncestorType` |
| Miniatura con `RenderOptions.BitmapScalingMode` | `BitmapImage.DecodePixelWidth = 64` (gif de 7TV si hay; webp si no) | WinUI decodifica a ese tamaño y anima el gif solo |

## Música

| WPF | WinUI 3 | Motivo |
|---|---|---|
| `SongPicker` flotante estilo rofi sobre el login (buscador, flechas, Enter) | Pantalla `MusicaPage`: canción actual (portada 300 px, título, ◀ ▶‖ ▶) y biblioteca en `ListView` | Pedido de la migración; sin capa flotante encima del fondo animado |
| `System.Windows.Media.MediaPlayer` | `Windows.Media.Playback.MediaPlayer` (controles de Windows apagados) | El de WPF no existe en WinUI |
| Duración con TagLibSharp | `StorageFile.Properties.GetMusicPropertiesAsync()` | Nativo y async; un paquete menos |
| Portadas con `CoverDecoder` (decodifica, congela y recorta a cuadrado) | `BitmapImage.DecodePixelWidth` + `Stretch="UniformToFill"` | El recorte lo hace la vista; nada de `Freeze` |
| Iconos con `Path` y `DataTrigger` (nota / nota tachada) | Glifos de Segoe Fluent Icons (`MusicGlyphs`) | Heredan el color del botón; sin plantillas extra |
| `Slider` con plantilla propia (riel de 2 px) | `Slider` de WinUI con el acento | Mismo uso, sin rehacer la plantilla |
| Errores en `MessageBox` | `InfoBar` de aviso (carpeta no encontrada, carpeta vacía, archivo ilegible) | Pedido del cambio |
| SUBIR MÚSICA (copiar archivos a la carpeta) | Quitado por ahora | Necesita `FileOpenPicker` con el HWND; ver `PENDIENTES_LIMPIEZA.md` |

## Sala

| WPF | WinUI 3 | Motivo |
|---|---|---|
| Placa del QR con `Clip` (esquinas cortadas) | `Path` con la misma forma, relleno claro y borde de acento de 1 px | En WinUI `UIElement.Clip` solo acepta rectángulos |
| Brillo de acento que late alrededor del QR (`DropShadowEffect` animado con `EventTrigger`, radio 18 → 34) | Quitado: queda el borde de acento fijo | WinUI 3 no tiene `DropShadowEffect`; `ThemeShadow` es caro y no da brillo de color |
| COPIAR ↔ COPIADO con 2 `DataTrigger` (texto e icono copiar/palomita) | Solo cambia el texto (`CopyLabel` del ViewModel); sin icono | WinUI 3 no tiene `DataTrigger`; un icono menos |
| Botón COPIAR con plantilla propia | `SolidButton` común de `App.xaml` | Mismo look que los demás botones sólidos |
| CERRAR SALA inclinado (`SlantedShape`) con icono de encendido | `ConfirmButton` (rectángulo con relleno tenue y borde de acento, como el diálogo); sin icono | `SlantedShape` dibuja con `OnRender` |
| ← INICIO propio de la sala | `EncabezadoPantalla` compartido (clic o Esc) | Una sola pieza para Sala, Emotes y Música |
| Emotes dentro de la sala (`EmotePanel` al lado de la lista) | Botones de texto EMOTES y MÚSICA abajo a la izquierda; abren su pantalla y ← SALA regresa | La sala queda más limpia; cada pantalla con su espacio |
| Sala se abría desde el dashboard antes de entrar | `SalaPage` abre servidor + túnel + sala al cargar; mientras, un `InfoBar` con barra de progreso | El aviso de "abriendo" se ve dentro de la misma pantalla |
| Errores del túnel/servidor en `MessageBox` | `InfoBar` de error con REINTENTAR (cloudflared no encontrado, túnel lento o caído, puerto ocupado, servidor) | Pedido del cambio |
| Estados abriendo / error / abierta con `DataTrigger` | `IsOpening`, `IsErrorOpen`, `IsRoomOpen` del ViewModel con `x:Bind` | WinUI 3 no tiene `DataTrigger` |
| `EntryAnimation` (QR desde -60 px y columna derecha desde 40 px, con retraso) | `EntranceThemeTransition` con los mismos desplazamientos | Animación del sistema |
| `SpacedText` en código, EN VIVO, ESCANEA y etiquetas | `TextBlock.CharacterSpacing` | Mismo efecto, nativo |
| Rombo con la marca al centro del QR | Igual: `Border` girado 45° + dos `TextBlock` | Ya era barato |

## Firma del login

| WPF | WinUI 3 | Motivo |
|---|---|---|
| Brillo "aero" que cruza el avatar (degradado animado) | Recuperado en `LogoNeondev` (ver "Correcciones después de probar la app") | Es solo una traslación en el compositor |
| Nombre con brillo de acento (`DropShadowEffect`) al pasar el mouse | El nombre pasa al acento y el borde del avatar se aclara (`VisualStateManager`) | WinUI 3 no tiene `DropShadowEffect` |
| 14 `DataTrigger` (hover del enlace, nota, barra de volumen, sin música) | Estados del botón + propiedades `IsMuted` / `IsMusicAvailable` / `IsMusicMissing` con `x:Bind` | WinUI 3 no tiene `DataTrigger` |
| Barra de volumen escondida que aparece con el mouse (opacidad + escala) | Siempre visible | Menos estados; se ve que se puede mover |
| Botón de lista que abría el `SongPicker` | Nombre de la canción que suena debajo del volumen | La biblioteca ahora es `MusicaPage` (después de entrar) |
| Nota sin música → SUBIR MÚSICA | Texto "Sin música" | Sin `FileOpenPicker` todavía |
| Ceniza y scanlines del fondo | Siguen fuera (ver Login) | WinUI 3 no tiene `DrawingBrush` ni `OnRender` |

## Fondos

| WPF | WinUI 3 | Motivo |
|---|---|---|
| `BackgroundPicker`: selector flotante sobre la ventana (abre con fundido, miniaturas que suben 28 px y se enciman 60 px) | Pantalla `FondosPage` con `GridView` de miniaturas de 256×144, sin encimarse ni animación de entrada | Una pantalla más, como Emotes y Música; el `GridView` virtualiza y ya trae scroll |
| Al abrir el selector, el fondo se desenfocaba (`BlurEffect` animado hasta `BackgroundBlurRadius`) y la pantalla se desvanecía | El fondo sigue vivo detrás con un **velo negro** al 75 % (`Background:PickerVeilOpacity`) que entra con `OpacityTransition` | WinUI 3 no tiene `BlurEffect`; desenfocar un video en vivo es lo más caro de todo; el velo es una capa de opacidad en el compositor |
| Miniatura de video: un `MediaElement` pausado por cada video (`VideoThumbnail`, en el segundo `PreviewSeconds`) | Miniatura de Windows (`StorageFile.GetThumbnailAsync`), decodificada a 320 px, una tras otra | Abrir un reproductor por miniatura trababa la PC; la miniatura del explorador ya está en caché |
| Imagen con `ThumbnailImage` (decodificación congelada de WPF) | `BitmapImage.DecodePixelWidth` con `SetSourceAsync` (async) | Nativo de WinUI y async; si no se puede leer, se avisa |
| Fondo de video con `LoopingVideo` (`MediaElement` que se reinicia al terminar) | Un solo `MediaPlayer` (`FondoPlayer`) con `IsLoopingEnabled` y en silencio, pintado por un `MediaPlayerElement` | Pedido del cambio: un solo reproductor reutilizado |
| `VideoDimBrush` encima de imagen y video | Velo negro al 45 % (`Background:VeilOpacity`); sin fondo elegido, 0 | Mismo efecto, configurable |
| Cada fondo nuevo aparecía con fundido de 0.9 s (`EventTrigger Loaded`) | Cambio directo | Sin `EventTrigger`; evita tener dos fondos a la vez durante el fundido |
| Videos pesados (4K, 150 MB) se reproducían siempre | Más de 120 MB (`Background:HeavyVideoMegabytes`): etiqueta PESADO y arrancan como **cuadro fijo**; "Usar cuadro fijo (más ligero)" lo cambia por video | Lo que más se trababa en el Ryzen 5 era el video 4K de fondo |
| Video que no abre: pantalla negra | Cae solo a su cuadro fijo y FONDOS lo avisa con InfoBar | Errores visibles (pedido del cambio) |
| Carpeta faltante: lista vacía sin aviso | InfoBar "No se encontró la carpeta de fondos…" (también carpeta vacía y `fondos.json` dañado) | Pedido del cambio |
| El fondo elegido no se guardaba (al abrir arrancaba con `Background:DefaultFile`) | Se guarda en `fondos.json` junto con el catálogo y el cuadro fijo de cada video | Pedido del cambio: catálogo y selección en JSON |
| Fondo solo en dashboard y sala, debajo de todo | Igual (`CapaFondo` detrás del `Frame`); en las demás páginas se oculta y el video se pausa | Que no gaste mientras no se ve |
| Sala con fondo liso `WindowBackgroundBrush` | Sala transparente: se ve el fondo elegido (o el degradado del dashboard) | Igual que el WPF, que compartía el fondo entre dashboard y sala |
| Botón CAMBIAR FONDO solo en el dashboard | También en la sala (abajo a la izquierda, junto a EMOTES y MÚSICA); ← SALA regresa | Pedido del cambio |

## Manual de Lain

| WPF | WinUI 3 | Motivo |
|---|---|---|
| Ventana aparte (`ManualWindow`, modal sobre la principal) | Página `ManualPage` dentro del `Frame` con ← INICIO | WinUI 3 no tiene ventanas modales con dueño; pedido del cambio (botón de regreso) |
| Video de Lain con shader `LumaKey` (`ShaderEffect` que volvía transparente el negro) | El mismo video con **opacidad fija** (0.9) sobre el azul noche del fondo | WinUI 3 no tiene `ShaderEffect`; Win2D agregaría un paquete y un efecto por cuadro. El fondo del video ya es oscuro y casi se funde |
| `OpacityMask` con degradado sobre el video (borde de arriba que se desvanecía) | Un `Rectangle` con degradado del color del fondo a transparente encima del borde | WinUI 3 no tiene `OpacityMask`; un degradado encima da lo mismo y es gratis |
| Despedida: segundo `LoopingVideo` con `LumaKey` "fantasma" (fuerza 0.32), máscara radial y escala 1.25 | Mismo reproductor: cambia el `Source` y un `StateTrigger` mueve el video al centro (720×440) con opacidad 0.45 | Un reproductor en vez de dos; sin shader ni máscara |
| Estática por píxeles (`NoiseLayer`, `WriteableBitmap` redibujado cada cuadro) que subía en la despedida | **Quitada** | Redibujar píxeles en el hilo de la UI era lo más caro del manual; no hay reemplazo ligero que se parezca. El sonido de estática de la despedida sí se quedó |
| Pérdida de señal (`SignalLossLayer`): ráfagas al azar con temblor, corrimiento del video, franjas y golpes de estática | Un **destello** tenue (rectángulo verde azulado con `OpacityTransition` de 0.15 s) al cambiar de paso | Sin temporizadores al azar ni transformaciones por cuadro; queda la idea de "cambio de canal" |
| Golpes de estática cortos en cada ráfaga (`ManualSound.PlayStaticHit`) | Quitados | Iban atados a `SignalLossLayer` |
| Niebla con `BlurEffect`, 5 franjas de interferencia y línea que bajan a distinto ritmo | **Quitadas**; queda el degradado azul noche, la viñeta y un solo barrido tenue (Storyboard de traslación, 14 s) | Sin blur en WinUI; la traslación corre en el compositor |
| Scanlines (`DrawingBrush` en mosaico de 3 px) encima de todo | Quitadas | WinUI 3 no tiene `DrawingBrush` |
| Ejemplo de cada paso con las **vistas reales** (Dashboard, Sala, SongPicker, diálogo) llenas con datos de muestra (`ManualDemoFactory`) y recuadros con flecha que las seguían (`ManualHighlight`, `ManualCue`) | **Recuperado** (ver "Manual: vistas de ejemplo"): una réplica ligera por pantalla (`VistaEjemplo*`) con datos de `Manual:Demo` y `ResaltadoManual` al lado de cada elemento. La captura `manual\pasos\pasoN.png` queda como alternativa | Las páginas reales toman su ViewModel del contenedor (sala, túnel, música reales); las réplicas usan los mismos estilos y textos sin tocar nada de eso |
| Notificación estilo iPhone con `DropShadowEffect`, escala 0.96→1 y caída de 24 px (`DataTrigger` + Storyboard) | Componente `AvisoLain` igual de forma y colores, sin sombra; entra y sale con `OpacityTransition` | WinUI 3 no tiene `DropShadowEffect`; la transición implícita corre en el compositor |
| Paso que sale con fundido y entra subiendo 18 px (`EntryAnimation`) | Fundido de 0.25 s con `OpacityTransition` | Sin `EventTrigger`/`DataTrigger`; un solo efecto |
| Atajos con `KeyBinding` en la ventana | `KeyboardAccelerators` de la página (ENTER, →, ←, ESC); el ← INICIO del encabezado no se queda con Esc | Lo nativo de WinUI |
| Keycaps y botones con `ControlTemplate.Triggers` | `BotonManual` (un `Button` con tecla opcional) y `VisualStateManager` | WinUI 3 no tiene `Trigger` en plantillas; un solo control para atajos y botones |
| Icono de bocina con `Path` y `DataTrigger` (ondas tachadas) | Glifo de Segoe Fluent Icons (`MusicGlyphs.Speaker`) | Igual que la música |
| Paso 5 "Música y selector de canciones" (bocina de la firma y `SongPicker`) | Paso 6 "Música y fondo" con la réplica de MÚSICA | En WinUI el selector de canciones es una pantalla y los fondos también |
| Textos del manual en `appsettings.json` (`Texts:Manual`) | En `Textos.xaml` (`Text.Manual.*`); en `appsettings.json` solo rutas, volúmenes y tiempos | Igual que el resto de la app WinUI |

### Manual: vistas de ejemplo (recuperado del WPF)

Antes de esto el manual solo mostraba el texto de Lain y una lista de avisos. Ahora cada paso tiene, a la derecha, la pantalla que explica
(réplica quieta a 1280×720 escalada con `Viewbox`, como el `ManualStepView` del WPF), y cada aviso de Lain enciende su resaltado.

| Paso (WPF) | Paso (WinUI) | Vista de ejemplo | Resaltados (aviso 1 / 2 / 3) |
|---|---|---|---|
| 1 Bienvenida (dashboard, GENERAR QR) | 1 Abre la app y entra | `VistaEjemploLogin` | título / usuario y contraseña / ENTRAR |
| 2 QR y entrada (sala) | 2 El inicio: genera el QR | `VistaEjemploInicio` (TarjetaMenu reales) | ESTADO / 01 GENERAR QR / tarjetas 02-06 |
| 3 El chat (sala) | 3 Comparte el QR o el enlace | `VistaEjemploSala` | QR / URL + COPIAR / EN VIVO |
| — | 4 Ve quién está conectado | `VistaEjemploSala` (ListaConectados real) | contador / lista / CERRAR SALA |
| 4 Emotes (bloque de la sala) | 5 Emotes de 7TV | `VistaEjemploEmotes` | set + APLICAR / REPORTES / QUITADOS |
| 5 Música (SongPicker) | 6 Música y fondo | `VistaEjemploMusica` | biblioteca / controles + volumen / ← INICIO |
| 6 Salir (SALIR + diálogo) | dentro de 4 y 6 (CERRAR SALA, ← INICIO) | — | — |

Los textos se reescribieron para contar el recorrido real del host (abrir, entrar, GENERAR QR, compartir, conectados, emotes, música);
el chat del invitado ya no tiene paso propio porque el host no lo ve. "Bienvenido a COT_0" (`Text.Manual.Heading`) se quitó para dejarle
alto a la vista; el paso 1 ahora tiene título.

| WPF | WinUI 3 | Motivo |
|---|---|---|
| `ManualHighlight` buscaba el elemento por `x:Name` en el árbol en cada `LayoutUpdated` | `ResaltadoManual` va en la misma celda que el elemento, con margen negativo | Sin recorrer el árbol ni recalcular en cada layout |
| Recuadro verde azulado con `DropShadowEffect`, pulso y flecha que se movía | Borde rojo del proyecto (3 px) + halo de 2 px que late con opacidad y escala (`Storyboard`, corre en el compositor); **sin flecha** | Sin sombras; el rojo es el acento de la app |
| `ManualCue.Phase` heredada por todo el escenario | `VistaEjemplo.Phase` → `x:Bind` a cada `ResaltadoManual` (From/To, y `OnlyInStep` en la sala que sirve a dos pasos) | WinUI no hereda propiedades adjuntas |
| `ManualDemoFactory` con el QR real de la URL de muestra | `ManualDemoViewModel` con `Manual:Demo`; el QR es un `Path` de adorno que no se puede escanear | Nada de QR real ni red |
| Portadas reales en la muestra del SongPicker | Recuadro con nota (el de "sin portada") | Las portadas son archivos locales del host |
| Miniaturas de emotes vacías | Recuadro con la inicial del emote | `EmoteRowViewModel` descarga de 7TV |
| Firma (logo + volumen) en las muestras | Quitada de las réplicas | Su barra está atada al volumen real |
| `EncabezadoPantalla` | `EncabezadoEjemplo` (solo dibujo) | El real se queda con Esc y navega |
| `IsEnabled=False` en el escenario | `IsHitTestVisible=False`, `IsTabStop=False` y el foco se cancela (`GettingFocus`) | Deshabilitar cambia los colores; así ENTER siempre es del manual |
| Cambio de paso: fundido + entrada subiendo 18 px | Fundido de 0.25 s (el mismo del texto) | Un solo efecto |

Sigue simplificado: sin flechas en los resaltados, sin niebla/estática/scanlines (ver arriba), réplicas sin animaciones de entrada y la de EMOTES
dibujada aparte (no reutiliza `PanelEmotes` porque toma la sala real).

## Correcciones después de probar la app (08/11)

### Fondos: video de Gojo y miniaturas vacías

| Qué se veía | Causa real (verificada) | Qué quedó |
|---|---|---|
| Al elegir el video de Gojo salía "No se pudo abrir…" | **No era el video**: Gojo (H.264 High nivel 5.2, 4K 60 fps, 151 MB) abre y se reproduce bien con `MediaPlayer`, por `Uri` y por `StorageFile` (probado: 838 cuadros en 6 s). Como pesa más de 120 MB arranca como **cuadro fijo**, y el cuadro fijo pedía `GetThumbnailAsync(ThumbnailMode.SingleItem)`, que para estos videos regresa `ThumbnailType.Icon` (el icono del tipo de archivo). El código lo descartaba (`null`) y lo reportaba como "no se pudo abrir" | El cuadro fijo sale de `FondoFrameCache`: ffmpeg → miniatura de Windows con `VideosView` → `MediaComposition`. Se guarda una vez en `%LocalAppData%\COT_0\cache\fondos\<hash>.jpg` |
| Las 3 miniaturas de la cuadrícula con el icono vacío | La misma: las tres regresaban `Icon` con `SingleItem` | La miniatura usa el mismo JPEG de la caché, con el nombre limpio abajo (sin el `[ID]` de YouTube y con `：＂｜` normales) y el nombre completo en el tooltip |
| El aviso solo decía "No se pudo abrir" | Un solo `FondoError.Unreadable` para todo | `MediaFailed` guarda `Error` + `ExtendedErrorCode` en `registro-medios.jsonl` y `FondoFailures` lo traduce: archivo que ya no está, bloqueado, sin códec (HEVC/AV1), no decodifica o sin cuadro fijo. El InfoBar dice la causa y el código |
| Nombres con `：`, `＂`, `｜`, corchetes | No eran la causa (`new Uri` los escapaba bien), pero era frágil | El video se abre con `StorageFile.GetFileFromPathAsync` + `MediaSource.CreateFromStorageFile`; ffmpeg recibe la ruta por `ArgumentList` |
| CheckBox "Usar cuadro fijo" azul | Acento de Windows | `SystemAccentColor*` y las llaves `CheckBox*Checked*` / `Slider*` de `App.xaml` con el rojo `#B3122E` |
| Aviso "Elige el fondo…" y los controles en la misma celda | Se enciman si el texto crece | Columnas `*` / `Auto`; el aviso hace wrap (2 líneas) y se recorta |

Imágenes por tarjeta en el WPF: **no había**. El `BackgroundPicker` del WPF pintaba cada miniatura con un `MediaElement` pausado en `Background:PreviewSeconds` (3 s) o la imagen misma; en `COT_0.Host/Assets` solo están `ascii-bg.png` y `neondev-avatar.png`, que ya estaban en `Assets/Imagenes`. Por eso el cuadro se saca a los 3 s (`Background:FrameSeconds`).

### Sala: barra de abajo

| Antes | Ahora |
|---|---|
| Accesos (EMOTES, MÚSICA, CAMBIAR FONDO) y aviso + CERRAR SALA en la misma celda: a 1280 px la columna derecha mide 656 px y los botones (316) + CERRAR SALA (`MinWidth` 260 + 22 de margen) + el aviso (176) no cabían: se encimaban y CERRAR SALA se salía por la derecha | `Grid` de 3 columnas: accesos `Auto` \| aviso `*` (wrap a 2 líneas + recorte, alineado a la derecha) \| CERRAR SALA fijo de 196 px (`CloseRoomWidth`; el contenido mide 156). El aviso queda con ~112 px y se parte en "Al cerrar, todos / salen de la sala"; si falta espacio se encoge a 0 antes de tapar botones |
| Encabezado (← INICIO \| SALA #código y EN VIVO) en la misma celda | Columnas `*` / `Auto` |

Medidas sacadas con las fuentes del proyecto (Barlow, Barlow Condensed) y PIL, no a ojo.

### Música: portadas

| Antes | Causa | Ahora |
|---|---|---|
| Portada grande pixelada | Reusaba la imagen de la fila, decodificada a `Audio:CoverDecodeSize` (96 px), estirada a 300 | `CoverImages` la decodifica aparte: lee el tamaño real con `BitmapDecoder` y la muestra a lo más en `Audio:CoverLargeSize` (300) sin pasar de `Audio:CoverMaxUpscale` (2×) su tamaño; al reducir, el decodificador de Windows escala con alta calidad |
| Recuadro más grande que la imagen y portada recortada | `Stretch="UniformToFill"` dentro de un `Border` fijo de 300×300 | `Stretch="Uniform"` y el `Border` toma el tamaño de la imagen (ej. `Audio_04.jpg` de 736×920 sale a 240×300). Sin portada queda el recuadro de 300 con la nota |
| Miniaturas de la lista recortadas | `UniformToFill` | `Uniform` en su cuadro de 40 px |
| Volumen azul | Acento de Windows | Rojo del proyecto (ver Fondos) |

Las portadas reales miden 1024×1024 y 736×920: nunca hace falta ampliar, así que **no se generó** versión con ffmpeg/lanczos (no mejoraría nada frente al tope). Si algún día hay portadas chicas, el tope las muestra más chicas en lugar de pixeladas.

### Emotes (7TV)

| Antes | Causa | Ahora |
|---|---|---|
| "SET DE 7TV" encimado con "745 emotes cargados", campo y APLICAR desproporcionados, "OK" suelto | `EmotesPage` ponía el panel con `HorizontalAlignment="Left"` y `MaxWidth` sin ancho: se encogía a su contenido. Título y contador en la misma celda | El panel ocupa una columna de hasta 720 px. Arriba un `Grid` de 3 filas: título \| contador; campo \| APLICAR (misma altura `InputHeight`, `MinWidth` 132, texto centrado); estado abajo en las dos columnas con wrap |
| Detalle de reportes y quitados sin recorte | — | `TextTrimming` |

### Revisión general

- **Login**: el subtítulo "CONFIGURA TU PREGUNTA SECRETA" mide 346 px y el hueco entre las dos líneas del formulario de 360 es de 264: empujaba la línea derecha fuera. Ahora tiene `MaxWidth` 272 y hace wrap centrado.
- **Lista de conectados**: encabezado USUARIO / CONECTADO DESDE en columnas.
- Dashboard, Manual, aviso de Lain, diálogo y recuperación: sin textos en la misma celda ni anchos fijos que corten.

### Logo Neondev (rescatado del WPF)

| WPF (`Controls/SignatureBadge.xaml`) | WinUI (`Views/Componentes/LogoNeondev`) |
|---|---|
| Avatar `neondev-avatar.png` de 64 px con esquinas de 14 sobre `#1C1C24`, borde de 1 px `#1FFFFFFF` (`#59FFFFFF` con el mouse) y "Neondev" en Redaction 50 a 36 px | Igual: mismas medidas, colores e imagen (ya estaba en `Assets/Imagenes`) |
| Brillo "aero": `LinearGradientBrush` (`#00→#4D→#00` blanco, de 0,0 a 1,0.6) con `RelativeTransform` X de -1 a 1, 3 s, ida y vuelta, `SineEase`, desde `Loaded` (`EventTrigger`) | Mismo degradado en un `Rectangle` que se mueve con `TranslateTransform` de -64 a 64 (corre en el compositor). Sin `EventTrigger` en WinUI: el `Storyboard` vive en los estados `Normal` y `PointerOver` de la plantilla, así arranca solo. El `Border` con `CornerRadius` recorta el brillo |
| Con el mouse: nombre al acento con `DropShadowEffect` (brillo rojo) | Nombre al acento y borde más claro; **sin** sombra (cara) |
| Abría `Links:GitHub` | Abre `Branding:RepoUrl` (`https://github.com/neondev87/COT_0`) con `Launcher.LaunchUriAsync`; tooltip "Abrir el repositorio del proyecto" y cursor de mano |
| Lugares: login (arriba a la derecha) y dashboard (abajo a la derecha, sobre el pie) | Login (dentro de `Firma`, mismo lugar), dashboard **arriba a la derecha, abajo del estado de la SALA** y sala **arriba a la derecha, abajo de EN VIVO** (donde se pidió). En el WPF del dashboard estaba abajo; si se quiere ahí, es mover una línea |

No hubo que reconstruir nada pesado: el original no usaba shader ni blur, solo un degradado animado. En la migración anterior se había quitado ("App ligera"); ahora volvió.

### Firma: tamaños, alineación y volumen

| Antes | Ahora |
|---|---|
| Avatar de 64 px, esquinas de 14 | **80 px** (64 × 1.25) y esquinas de **17.5** (14 × 1.25); el brillo recorre de -80 a 80 y el `Border` redondeado lo sigue recortando |
| "Neondev" a 36 px (`LineHeight` 36, `Margin` 16,2 y `VerticalAlignment="Top"`) | **50.4 px** (36 × 1.4), `VerticalAlignment="Center"`, `TextLineBounds="Tight"` (la caja es la de las letras, así el centro coincide con el del avatar), sin márgenes verticales y con `TextTrimming` por si falta espacio |
| Avatar y nombre en un `StackPanel` horizontal | `Grid` de dos columnas `Auto` con `ColumnSpacing` 16, los dos centrados en vertical |
| Medidas repetidas en la plantilla | Recursos `LogoAvatarSize`, `LogoAvatarRadius`, `LogoNameFontSize`, `LogoShineStart` y `LogoShineEnd` arriba de `LogoNeondev.xaml` |
| Firma del login: nota, barra de 130 px, número y nombre de la canción | Solo la barra de volumen, roja (`VolumeSlider` + acento del proyecto), del ancho del logo. Reproducir, siguiente y lista quedan en MÚSICA |
| Dashboard y sala: solo `LogoNeondev` | `Firma` completa (logo + barra) en login, dashboard y sala. En la sala va centrada a media altura de la fila del contador (antes arriba) |

Un solo volumen: las tres firmas y MÚSICA leen el mismo `MusicService`. Arranca siempre en 25 (`Audio:DefaultVolume`, no se guarda entre sesiones; antes 0) con `Audio_01.mp3` (`Audio:DefaultFile`) cargada en pausa; empieza a sonar con ▶ en MÚSICA o al mover la barra de volumen.

### Manual: sin texto duplicado y vista de ejemplo más grande

| Antes | Ahora |
|---|---|
| Debajo del título de cada paso, un párrafo (`Text.Manual.StepN.Text`) que repetía lo que dicen los avisos de Lain | Quitado: queda "Paso 0N / 06" y el título corto. Se borraron las 6 claves de `Textos.xaml`, la propiedad `ManualStepViewModel.Text` y el estilo `ManualStepText` (su formato pasó a `ManualStepNumberText`) |
| Vista de ejemplo en un marco que se estiraba a todo el ancho (con `MinHeight` 180) y la réplica centrada adentro; pasos a 402 px del borde | El marco se ajusta a la réplica 1280×720 escalada (`Viewbox` `Stretch="Uniform"`), centrado en todo el espacio a la derecha de Lain; pasos a 386 px (32 + columna de 330 + 24). A 1280×720 pasa de ~827×465 a ~862×485 (~+4 % por lado, ~+9 % de área) y crece con la ventana |

Notificaciones, sonido, tiempos, resaltados, atajos, silencio, despedida y ← INICIO quedan igual. Los resaltados viven dentro de la réplica, así que escalan con ella.

### Manual: Lain en el borde, fondo transparente y paneles de vidrio

| Antes | Ahora |
|---|---|
| Video de Lain en una caja de 330 × 460 con 20 px de margen abajo: flotaba y el fundido de arriba se comía parte del encuadre | Caja de **315 × 560** (la proporción del video, 480 × 854), `VerticalAlignment="Bottom"` y **sin margen inferior**: la figura queda completa y parada en el borde. Orillas de arriba, izquierda y derecha fundidas con degradados (`LainFadeBrush`, `LainFadeLeftBrush`, `LainFadeRightBrush`). Las notificaciones terminan antes de donde empieza la figura y los pasos siguen a 386 px, así que no se enciman |
| Despedida: video de 720 × 440 a 60 px del borde de arriba (colgaba) | **900 × 634** centrado y pegado abajo, como fondo al 45 %; el texto y CERRAR van en un panel de vidrio encima |
| Página con fondo sólido azul (`ManualBaseBrush`) y viñeta propia | Página transparente: el fondo de la app (`CapaFondo`) se ve a través de un **velo** radial `#050507` al **55 % en el centro y 65 % en las orillas** (`ManualVeilBrush`). La viñeta la pone `CapaFondo`. Si el fondo es video, en el manual se muestra su cuadro fijo (no dos videos a la vez) |
| Paleta azul verdosa de Lain en atajos, botones y marco | Paleta del proyecto: **vidrio** (`ManualGlassBrush`, degradado blanco 10 % → 2 %, borde claro de 1 px, esquinas de 12) en la fila de atajos, el botón "Gracias, pero no necesito ayuda" y la despedida; marco de la vista de ejemplo con **borde rojo tenue** (`ManualFrameBrush`, rojo al 35 %); teclas con canto rojo |
| "Paso 0N / 06" en Inter | Barlow Condensed con espaciado y **una línea roja a cada lado**; título en Barlow Condensed Medium |
| Sin piso | Degradado de 240 px que oscurece el borde de abajo (`ManualFloorBrush`) para asentar a Lain |

Sin desenfoque, sombras ni `OpacityMask`: todo es degradados quietos. Notificaciones (textos, sonido, orden y tiempos), resaltados, atajos, silencio, despedida y ← INICIO quedan igual.

### Barra de volumen de la firma: minimalista

| Antes | Ahora |
|---|---|
| WPF: nota, barra de 130 px, número, botón de lista y "SUBIR MÚSICA"; volumen que aparecía con hover. WinUI: `Slider` de WinUI a todo el ancho del logo (riel, thumb grande con anillo y estados por defecto) | **Bocina + línea**, nada más, alineadas bajo "Neondev" (`VolumeRowMargin` 96 = avatar + separación) |
| Thumb de WinUI siempre visible | `ControlTemplate` propio (`MiniVolumeSlider` en `Views/Estilos/Volumen.xaml`): línea de **2 px** gris tenue con tramo **rojo** hasta el valor y un punto de **10 px** que solo aparece (y crece) con el mouse, al arrastrar o con foco de teclado. Zona de clic de 24 px de alto aunque la línea sea fina |
| Clic en la nota de MÚSICA para silenciar | Bocina (Segoe Fluent Icons, 14 px, gris claro → blanco con hover, rojo al presionar), cursor de mano (`BotonBocina`) y tooltip "Silenciar" / "Activar sonido". El glifo cambia por rangos: tachada (silencio), 0, bajo (< 50) y alto |
| En silencio la barra seguía marcando el volumen | En silencio la barra marca **0 sin perder el volumen**; al quitar el silencio regresa. Si subes la barra estando en silencio, se quita solo. MÚSICA usa el mismo `MusicControlsViewModel.Volume`, así que su barra y su número también marcan 0 |

Evitado por ligereza: sombras del thumb, brillo del tramo rojo, animación del ancho y el panel que se abría con hover del WPF. Solo opacidad y escala del punto (0.15 s). Sigue habiendo un solo `MusicService`: arranca en 0 con `Audio_01` cargada y el silencio no se guarda entre sesiones.

### Login: Registrar usuario (varios usuarios)

- El botón principal siempre dice **ENTRAR**; ya no cambia a REGISTRARME cuando no hay cuenta. Sin usuarios, arriba de los campos sale un aviso tenue (Barlow 13, `Light62Brush`, centrado) que apunta a "Registrar usuario".
- Abajo de la contraseña, alineados a la derecha en una sola línea: **¿Olvidaste tu contraseña?** | **Registrar usuario**. Los dos usan `LoginLinkButton` (mismo estilo) y los separa `LoginLinkSeparator`: barra vertical de 1×12 px, `Light55Brush` al 50 %, con 10 px a cada lado. Caben en los 360 px del formulario sin cortarse.
- **Formulario de registro** (`Views/Componentes/FormularioRegistro`): mismo aspecto que la recuperación (etiquetas `FieldLabelText`, campos con línea de acento, InfoBar de error, botón `LoginButton` REGISTRAR y enlace "Volver al inicio de sesión"). Subtítulo: REGISTRA UN USUARIO. Pide solo usuario, contraseña y repetirla.
- **Recuperación**: ahora el primer campo es USUARIO con botón CONTINUAR; la pregunta y la respuesta aparecen debajo cuando se encuentra el usuario.
- InfoBar verde reutilizado: "Usuario registrado…" o "Tu contraseña cambió…", según de dónde regreses.

### Manual de Lain: paso 1 con el login nuevo

- Aviso 2 del paso 1 (`Text.Manual.Step1.Notice2`): "Escribe tu usuario y tu contraseña. Si es tu primera vez, crea tu cuenta." → "Si es tu primera vez, toca Registrar usuario." Mismo sonido, orden y tiempos.
- Réplica del login (`VistaEjemploLogin`): bajo la contraseña, alineados a la derecha como en el login real, **¿Olvidaste tu contraseña?** | **Registrar usuario** con `LoginLinkButton` y `LoginLinkSeparator` (sin estilos nuevos). Botón ENTRAR igual.
- Resaltado 2 del paso 1: pasa de USUARIO y CONTRASEÑA al enlace **Registrar usuario**. Los resaltados 1 (título) y 3 (ENTRAR) no cambian.

## Manual: fondo ordenado, Lain sin cuadro negro y sin letreros de atajos (08/11, noche)

Diagnóstico completo de las capas de antes en `docs/qa/CAPAS_MANUAL.md`.

### Fondo del Manual: tres capas

| Antes | Ahora |
|---|---|
| CapaFondo con degradado del login, velo propio al 45 % y viñeta, más el velo radial del Manual (55–65 %), un barrido animado (`ManualSweepBrush`) y un piso oscuro de 240 px (`ManualFloorBrush`): cuatro oscurecimientos encimados | (1) **Fondo elegido como cuadro fijo** (CapaFondo, nunca video en el Manual; sin degradado, sin velo propio y sin viñeta: `CapaFondoViewModel.IsDecorVisible = false` y `VeilOpacity = 0` en el Manual). (2) **Un solo velo** `ManualVeilBrush` negro uniforme al **60 %** (`#99000000`, en `Views/Estilos/Manual.xaml`). (3) El contenido |
| Sin fondo elegido: degradado gris del login con halo claro | Negro limpio (`LoginBaseBrush` #080808) con el mismo velo |
| `ascii-bg.png` | Solo dentro de la réplica del login (paso 1), porque es la pantalla real que se muestra |

Los paneles de vidrio (atajos, "Gracias, pero no necesito ayuda", despedida y el marco de la vista de ejemplo) no cambian: sobre el velo plano se ven parejos. El destello de 0.15 s al cambiar de paso se queda (es transición, no capa fija).

### Lain transparente (opción A, con respaldo B)

| Antes | Ahora |
|---|---|
| Video de Lain (sin alfa, fondo #000000) en una caja de 315 × 560 al 90 % con degradados #050507 arriba y a los lados: se veía un rectángulo negro con caracteres ASCII | **Opción A**: `gifs\lain-transparente.gif` (ffmpeg, `colorkey` del negro, ver `docs/PREPARAR_LAIN.md`) en un `Image` (BitmapImage, se anima solo) de 560 de alto, pegado abajo con 40 px a la izquierda. Sin caja ni degradados |
| — | **Opción B** (si el GIF no está o no se puede leer): el mismo video dentro de `VideoHost`, rodeado de franjas del negro exacto del video que se desvanecen hacia afuera (40 px a la izquierda, 100 a la derecha, 140 arriba) y esquinas radiales (`EdgeTop/Bottom/Left/RightBrush`, `EdgeCornerTopLeft/RightBrush`). Como el borde del video y la franja son del mismo negro, no hay salto |

Lain queda debajo de sus avisos (la figura empieza ~120 px más abajo del borde de su caja) y a la izquierda de la columna de pasos (386 px), así que no se encima con la fila de atajos ni con "Gracias, pero no necesito ayuda". Verificado con ffmpeg: fondo transparente en los 93 cuadros (≈ 12 % de píxeles opacos por cuadro) y sin halo negro sobre un fondo rojo de prueba.

### Despedida sin rectángulo duro

| Antes | Ahora |
|---|---|
| Video de 900 × 634 al 45 % con degradados de 120/40/40 px pensados para Lain y nada abajo: rectángulo azul (#06122C) con bordes marcados y una franja clara a la izquierda (la orilla izquierda del video original es más clara) | Mismo video, tamaño y opacidad, con **degradados hacia adentro en los cuatro lados** (180 px a los lados y 127 arriba y abajo, 20 % de cada lado) que llevan su azul al negro, y **franjas negras hacia afuera** (120 px a los lados, 80 arriba, esquinas radiales). Todo dentro de `VideoHost` con una sola opacidad (0.45): no queda borde ni parche de otro tono, solo una penumbra negra que se pierde en el velo |

El panel de vidrio con "Gracias por estar aquí. Vuelve pronto." y CERRAR no cambia y queda encima. Sin blur, sin shader ni `OpacityMask` (WinUI no lo tiene en `UIElement`; una máscara por Composition sobre el video no se puede revisar sin ver la app).

### Sin letreros de atajos ("Enter", "Esc", flechas)

| Antes | Ahora |
|---|---|
| Al pasar el mouse por el Manual salía un letrero "Enter" (tooltip automático de WinUI para los `KeyboardAccelerators` de la página); igual "Esc" en ← INICIO de las pantallas internas | `KeyboardAcceleratorPlacementMode="Hidden"` en `ManualPage` y en el botón ← INICIO de `EncabezadoPantalla` (APLICAR de Emotes ya lo tenía). Por defecto: `ShortcutHints` en cada página al navegar y `Hidden` en los estilos de botón. Los atajos funcionan igual |
| Tooltips en ✕ de conectados ("Sacar de la sala"), en cada fondo (nombre del archivo) y en anterior / reproducir / siguiente de Música | Quitados (se conserva su `AutomationProperties.Name`). Solo quedan el del logo Neondev y los de las bocinas |

## Validación, portadas 4 y 5, volumen y pantalla completa (09/11)

| Antes | Ahora |
|---|---|
| Los campos aceptaban espacios, @ y símbolos al escribir; se rechazaban hasta ENTRAR | Los campos de estilo `LoginTextBox` / `LoginPasswordBox` (usuario, contraseña, repetir contraseña, respuesta secreta y set de 7TV) quitan al momento lo que no sea letra o número (ñ y acentos sí), también al pegar (`FiltroCampo` + `InputRules`). Los errores siguen en el InfoBar; nuevos: "Tu respuesta solo puede tener letras y números, sin espacios ni símbolos." y "ERROR · SOLO LETRAS Y NÚMEROS, SIN ESPACIOS NI SÍMBOLOS" en Emotes |
| El set de 7TV aceptaba una URL ("ERROR · ESE ID O URL…") | Solo el ID (letras y números); el texto dice "ERROR · ESE ID NO ES DE UN SET DE 7TV" |
| Usuario de la muestra del Manual `host_demo` | `hostdemo` (el guion bajo ya no es válido) |
| Portadas 4 y 5 con fondo blanco (720×720 y 736×736) | Mismo proceso que la 1, 2 y 3 (`portadas/procesar.py`: rembg `isnet-anime`, sujeto al 80 % en lienzo de 1024 color `#0B0B0F`) y la máscara encogida 2 px para quitar el filo blanco. Ojo: la 1–3 no son PNG con alfa sino ese lienzo oscuro; la 4 y 5 quedaron igual |
| Volumen al abrir: 0 | 25 (`Audio:DefaultVolume`) |
| Ventana de 1280×720 centrada con los botones de Windows | Pantalla completa siempre (`AppWindowPresenterKind.FullScreen`). Como pantalla completa no tiene botones de Windows, una ✕ propia (46×32, arriba a la derecha, donde estaba la de Windows) hace lo mismo que la de antes: pregunta con su confirmación (`CloseGuard.RequestCloseAsync`). Alt+F4 también pasa por la confirmación. Ya no hay botón de minimizar |

## Video en el Manual, audio inicial, cierre limpio y despedida centrada (09/11)

| Antes | Ahora |
|---|---|
| Al entrar al Manual el video de fondo se detenía y se veía su cuadro fijo | El video sigue en bucle detrás del velo del Manual (60 %), **mudo** (`FondoPlayer.SetSilenced`). Al salir (← INICIO o ESC) recupera su sonido normal (`Background:VideoMuted`, hoy `true` = sin sonido, como siempre) sin reiniciarse ni parpadear: es el mismo reproductor y el mismo `Source`. Fondo de imagen o cuadro fijo: sin cambios |
| La app abría con `Audio_01` cargada en pausa | Abre **sonando `Audio_02`** a 25 (`Audio:InitialTrack` = 2 con `Audio:TrackNameFormat` = `Audio_{0:00}`; `Audio:DefaultVolume` = 25). Si no existe, arranca con la primera; si no se puede leer, salta a la siguiente (como ya hacía) |
| Al cerrar la app salía un error (0xC000027B en `Microsoft.UI.Xaml.dll`) o se quedaba colgada | Cierra limpio desde la ✕, Alt+F4 y SALIR del diálogo (ver causa en `REPORTE_CIERRE.md`) |
| Despedida: video de 900 × 634 abajo y al centro, con el texto encima de la parte de abajo del video | Video de **1125 × 793** (+25 %, casi su tamaño real de 1022 × 720: no se pixela) con el texto **debajo** (34 px, +30 %, estilo `ManualFarewellText`) y los dos **centrados en toda la pantalla**. Se conservan las orillas fundidas hacia adentro (225 / 159) y las franjas negras hacia afuera, ahora también abajo (esquinas `EdgeCornerBottomLeft/RightBrush`). Un `Viewbox` que solo reduce (`DownOnly`) encoge el grupo completo en pantallas de menos de ~1050 px de alto en lugar de cortarlo |

## Música muteada en el Manual (09/11)

| Antes | Ahora |
|---|---|
| La música de fondo seguía sonando encima de Lain, los avisos y la estática | En el Manual (pasos y despedida) la música queda **muteada pero sin pausa**: la canción sigue avanzando en silencio (`MusicService.IsHushed`). Al salir (← INICIO, ESC o Cerrar en la despedida) se oye desde donde iba, sin reiniciarse ni cambiar de pista. Es un silencio aparte del de la bocina: si el usuario ya la había silenciado, sigue silenciada al salir, y la bocina de la firma y de MÚSICA no cambia de icono. Volumen sin cambios (25 por defecto). Avisos, estática y SILENCIAR del Manual, igual. Sin claves nuevas en `appsettings.json` |

## Fondo de color en el Manual (09/11)

| Antes | Ahora |
|---|---|
| Fondo elegido visible detrás de un velo negro al 60 % (`ManualVeilBrush`) | Fondo de color **opaco** `ManualBackdropBrush` (`Views/Estilos/Manual.xaml`): radial con brillo vino oscuro del acento (`#1C0C14`) arriba a la derecha, negro azulado (`#0B0A12`) y negro exacto del cuadro de Lain (`ManualBlackColor`, `#000000`) desde el 70 % del radio: toda la zona de Lain (abajo a la izquierda) es negro puro y el centro (despedida) casi negro. El video de fondo **sigue corriendo debajo**, en bucle y mudo, pero no se ve; al salir vuelve por donde iba |
| GIF de Lain (opción A) sin tratamiento de orillas | Orillas de arriba y de los lados fundidas hacia adentro (36 px, `LainCutoutFade`) con las mismas brochas `Edge*`: lo que le quede de cuadro al GIF se pierde en el negro |
| Video de respaldo (opción B) y despedida con franjas sobre el fondo visto a través del velo | Mismas franjas y tamaños, ahora sobre el fondo de color; como empiezan en el mismo negro, no queda recuadro |

## Fondo azul con estática en el Manual (09/11)

| Antes | Ahora |
|---|---|
| Fondo de color opaco `ManualBackdropBrush` (vino oscuro → negro) | Vuelve el **azul noche** de siempre del Manual (`ManualBaseBrush`: `#0E2733` → `#08172F` → `#040C1A`, el de antes de `648df7a`), ahora **opaco**, con su **barrido tenue** (14 s) y la **viñeta**. Va en `Views/Estilos/ManualFondo.xaml` (diccionario nuevo con todo lo del fondo y las orillas de Lain) y en el componente `FondoManual` |
| Sin estática en WinUI (la del WPF redibujaba píxeles cada cuadro) | **Estática** ligera (`EstaticaManual`): una textura fija `Assets/Imagenes/Manual/estatica.png` (960 × 540, puntos blancos con alfa, generada una vez) al 8 %, un poco más grande que la pantalla, que **salta de lugar cada 60 ms** (8 posiciones con `DiscreteDoubleKeyFrame` sobre una traslación: corre en el compositor). Sin shaders, sin desenfoque y sin código por cuadro. Va encima de Lain para que su cuadro no corte el grano, y debajo de avisos, pasos y despedida |
| Lain sobre negro puro (el fondo terminaba en `#000000` en su esquina) | **Sombra** radial negra abajo a la izquierda (`LainShadeBrush`, 820 × 1000) que recibe el negro del cuadro de Lain y sus franjas/orillas (`Edge*`, sin cambios) y se pierde en el azul: no queda recuadro negro. Se oculta en la despedida |
| Despedida con orillas y franjas negras | Orillas hacia adentro y franjas hacia afuera en el **azul de su propio video** (`#06122C`, brochas `Farewell*`): se funde con el azul del fondo. Tamaño (1125 × 793), texto y centrado sin cambios |
| — | Video de fondo de la app: igual que antes, sigue corriendo debajo (mudo) y tapado; al salir se ve y se oye desde donde iba. Música muteada sin pausa en el Manual, igual |

## Reglas nuevas de usuario y contraseña (09/11)

| Antes | Ahora |
|---|---|
| Usuario: letras y números | Solo letras; al escribir se normaliza a primera mayúscula y resto minúscula (`erick` → `Erick`), estilo `UserNameTextBox` (`App.xaml`) |
| Contraseña: letras y números, sin símbolos | 8 a 20 caracteres, al menos un número y un carácter especial (`! @ # $ % & * ? . - _`), sin espacios; el filtro deja escribir esos símbolos |
| Mensaje "Tu contraseña solo puede tener letras y números…" | El InfoBar dice cada regla que falta: "Mínimo 8 caracteres." "Falta un número." "Falta un carácter especial (…)." "No uses espacios." |
| Límite de 15 en todos los campos | 20 (`Inputs:MaxCaracteres`); el set de 7TV sigue en 26 |
| Cuenta de antes entraba igual | "Esta cuenta ya no cumple las reglas nuevas. Regístrala de nuevo." |
| Usuario de la muestra del Manual `hostdemo` | `Hostdemo` |

## Emotes como mini tienda (09/11)

| Antes | Ahora |
|---|---|
| EMOTES: fondo sólido (`WindowBackgroundBrush`), video de fondo pausado | Página transparente: el video de fondo sigue corriendo detrás de **un panel** centrado (`ShopPanel`): degradado negro azulado al 90 → 84 % (`ShopPanelBrush`), esquinas de 10 px y borde fino de acento que se apaga en diagonal (`ShopPanelBorderBrush`), máx. 1180 px, márgenes 64/48/40 |
| Un solo panel: set de 7TV, reportes, quitados | Pestañas **USUARIOS \| TIENDA \| TICKETS** (`ShopTab`: texto condensado, línea de acento abajo al elegir, 0.15 s). El panel de siempre queda en el recuadro **SET Y REPORTES** |
| — | USUARIOS: tarjetas con la inicial en un tono de avatar y el nombre; clic → recuadro con su stock sobre el velo negro (`DialogVeilBrush`) |
| — | TIENDA: buscador + BUSCAR, "Te quedan X de 10" en rojo claro, cuadrícula de tarjetas (miniatura quieta, nombre, COMPRAR con contorno de acento que se llena al pasar el mouse; COMPRADO apagado) |
| — | TICKETS: tarjetas con número, fecha, hora, miniaturas y DESCARGAR PDF; vacío: "Aún no has comprado nada." + IR A LA TIENDA |
| — | Diálogo "¿Quieres descargar tu ticket en PDF?" SÍ / NO (mismo `ConfirmacionDialogo`) |
| Picker de emotes del chat: solo los primeros 120; el resto solo buscando | Sin buscar se ven todos (scroll dentro del picker); el tope solo aplica a búsquedas |
| Manual paso 5: "Pega el ID o la URL de un set…" | "En TIENDA compras hasta 10 emotes cada 24 horas; entran solos a tu sala. En SET Y REPORTES pegas el ID…" (la vista de ejemplo del paso 5 sigue mostrando el panel viejo) |
