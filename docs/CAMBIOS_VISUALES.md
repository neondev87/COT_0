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

## Pendiente (se llena al migrar cada pantalla)

- Dashboard: sombras de `MenuCard` (`DropShadowEffect`) → borde de color o `ThemeShadow`.
- Fondo de video: sin `BlurEffect`; bajar resolución del video si se traba.
- Manual de Lain: shader `LumaKey` → video con transparencia; estática por pixeles → animación sencilla.
