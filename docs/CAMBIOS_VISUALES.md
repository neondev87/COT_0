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
| Firma (avatar + música) y selector de canciones | Pendiente | Se migra junto con la música |

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
| Fondo de video en loop con `BlurEffect` y 12 `DataTrigger` (en `MainWindow`) | Degradado radial del login + viñeta | Sin video ni blur hasta migrar el selector de fondos; carga instantánea |
| `MenuCard` con `Clip` en forma de paralelogramo | Dos `Path` con la forma (cara clara y capa de acento) | En WinUI `UIElement.Clip` solo acepta rectángulos |
| Brillo de acento al pasar el mouse (`DropShadowEffect`, radio 16) | Quitado: solo cambia a acento sólido y se corre 18 px | WinUI 3 no tiene `DropShadowEffect`; `ThemeShadow` es caro y no da brillo de color |
| Hover con `Trigger IsMouseOver` + 2 `Storyboard` (entrada/salida) | `VisualStateManager` (`PointerOver`, `Pressed`, `Disabled`) con transición de 0.35 s | Lo nativo de WinUI; un solo lugar para el estado |
| `SpacedText` en títulos, fecha y estado | `TextBlock.CharacterSpacing` | Mismo efecto, nativo |
| Estado activo/inactivo con `DataTrigger` (`InactiveStatus`) | Dos propiedades del ViewModel (`IsRoomActive` / `IsRoomInactive`) con `x:Bind` a `Visibility` | WinUI 3 no tiene `DataTrigger` |
| Punto con pulso (`EventTrigger` en una plantilla) | Componente `PuntoPulso` que arranca su `Storyboard` en `Loaded` | WinUI no tiene `EventTrigger` en plantillas; escala y opacidad corren en el compositor |
| Entradas con `EntryAnimation` (tarjetas desde -408 px con retrasos fijos; reloj y estado con fundido de 1.2–1.4 s) | `EntranceThemeTransition` escalonada desde -120 px; reloj y estado con la transición del sistema | Animación del sistema, sin código propio |
| Tarjeta CAMBIAR FONDO abre el selector | Tarjeta visible pero apagada ("Próximamente en WinUI") | El selector de fondos se migra después |
| GENERAR QR abre túnel + sala | Por ahora solo lleva a `SalaPage` | Abrir la sala se migra con `SalaPage` |
| Firma con música (`SignatureBadge`) sobre el pie | Quitada por ahora | Se migra con la música |

## Pendiente (se llena al migrar cada pantalla)

- Fondo de video: sin `BlurEffect`; bajar resolución del video si se traba.
- Manual de Lain: shader `LumaKey` → video con transparencia; estática por pixeles → animación sencilla.
