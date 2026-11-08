# Cambios visuales en la migración a WinUI 3

Lo que WinUI 3 no soporta igual que WPF y cómo se simplificó. Prioridad: que arranque rápido y no se trabe.

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
| SUBIR MÚSICA (copiar archivos a la carpeta) | Quitado por ahora | Necesita `FileOpenPicker` con el HWND; ver pendientes |

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
| Ejemplo de cada paso con las **vistas reales** (Dashboard, Sala, SongPicker, diálogo) llenas con datos de muestra (`ManualDemoFactory`) y recuadros con flecha que las seguían (`ManualHighlight`, `ManualCue`) | Captura opcional por paso (`manual\pasos\pasoN.png`); sin captura, la **lista de avisos del paso**: los que ya salieron se encienden | Instanciar 6 pantallas de muestra dentro de un `Viewbox` era pesado y obligaba a que cada ViewModel aceptara datos falsos; las páginas de WinUI toman su ViewModel del contenedor. `ManualHighlight` buscaba elementos por nombre en el árbol en cada `LayoutUpdated` |
| Notificación estilo iPhone con `DropShadowEffect`, escala 0.96→1 y caída de 24 px (`DataTrigger` + Storyboard) | Componente `AvisoLain` igual de forma y colores, sin sombra; entra y sale con `OpacityTransition` | WinUI 3 no tiene `DropShadowEffect`; la transición implícita corre en el compositor |
| Paso que sale con fundido y entra subiendo 18 px (`EntryAnimation`) | Fundido de 0.25 s con `OpacityTransition` | Sin `EventTrigger`/`DataTrigger`; un solo efecto |
| Atajos con `KeyBinding` en la ventana | `KeyboardAccelerators` de la página (ENTER, →, ←, ESC); el ← INICIO del encabezado no se queda con Esc | Lo nativo de WinUI |
| Keycaps y botones con `ControlTemplate.Triggers` | `BotonManual` (un `Button` con tecla opcional) y `VisualStateManager` | WinUI 3 no tiene `Trigger` en plantillas; un solo control para atajos y botones |
| Icono de bocina con `Path` y `DataTrigger` (ondas tachadas) | Glifo de Segoe Fluent Icons (`MusicGlyphs.Speaker`) | Igual que la música |
| Paso 5 "Música y selector de canciones" (bocina de la firma y `SongPicker`) | "Música y fondos" (tarjetas 05 MÚSICA y 02 CAMBIAR FONDO) | En WinUI el selector de canciones es una pantalla y los fondos también |
| Textos del manual en `appsettings.json` (`Texts:Manual`) | En `Textos.xaml` (`Text.Manual.*`); en `appsettings.json` solo rutas, volúmenes y tiempos | Igual que el resto de la app WinUI |

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
