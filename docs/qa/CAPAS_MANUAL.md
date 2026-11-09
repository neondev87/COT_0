# Capas de la pantalla del Manual (diagnóstico antes del cambio)

Estado del código en `host-winui` antes de ordenar el fondo. De atrás hacia adelante.
"Opacidad" es la del elemento; el alfa del color va en el color (#AARRGGBB).

## Desde MainWindow (`MainWindow.xaml` → `Views/Componentes/CapaFondo.xaml`)

| # | Capa | Archivo | Opacidad / color | Para qué |
|---|------|---------|------------------|----------|
| 1 | Fondo base del Grid | CapaFondo.xaml (`LoginBaseBrush`, App.xaml) | 1 · #080808 | Negro de respaldo |
| 2 | Degradado del login | CapaFondo.xaml (`LoginBackgroundBrush`, App.xaml) | 1 · radial #1B1B1C → #0F0F10 → #070707 | Fondo gris cuando no hay fondo elegido; tapa la capa 1 |
| 3 | Video del fondo elegido | CapaFondo.xaml (`MediaPlayerElement`) | 1 | Oculto en el Manual (solo cuadro fijo) |
| 4 | Cuadro fijo / imagen del fondo elegido | CapaFondo.xaml (`Image`, UniformToFill) | 1 | El fondo de la app |
| 5 | Velo de CapaFondo | CapaFondo.xaml (`Rectangle` negro) | `Background:VeilOpacity` = 0.45 (0 sin fondo) · #000000 | Oscurecer el fondo en Dashboard/Sala |
| 6 | Viñeta | CapaFondo.xaml (`VignetteBrush`, App.xaml) | 1 · radial transparente → #CC000000 en las orillas | Orillas oscuras |

## Propias de ManualPage (`Views/Pantallas/ManualPage.xaml`, brochas en `Views/Estilos/Manual.xaml`)

| # | Capa | Opacidad / color | Para qué |
|---|------|------------------|----------|
| 7 | Velo del Manual (`ManualVeilBrush`, fondo del Grid raíz) | radial #8C050507 (55 %) centro → #A6050507 (65 %) orillas | Legibilidad |
| 8 | Barrido (`ManualSweepBrush`, 120 px, animado 14 s en bucle) | franja #08FFFFFF | Efecto "tele" |
| 9 | Piso (`ManualFloorBrush`, 240 px abajo) | #00000000 → #B3000000 | "Asentar" a Lain |
| 10 | VideoHost 315 × 560 (abajo a la izquierda) | Opacity 0.9 (0.45 y 900 × 634 al centro en la despedida) | Video de Lain / despedida |
| 10a | Video (`MediaPlayerElement`, Uniform) | fondo del video **#000000** (Lain) / **#06122C** azul (despedida) | El contenido; sin transparencia: es un rectángulo |
| 10b | `LainFadeBrush` arriba (120 px) | #E6050507 → transparente | Fundir el borde de arriba |
| 10c | `LainFadeLeftBrush` / `LainFadeRightBrush` (40 px) | #B3050507 → transparente | Fundir los lados |
| 11 | Encabezado (← INICIO \| MANUAL) | — | Navegación |
| 12 | Notificaciones de Lain (`AvisoLain`) | #242426 opaco | Avisos |
| 13 | Panel de vidrio de atajos (`ManualGlassPanel`) | #1AFFFFFF → #06FFFFFF, borde #1FFFFFFF | Atajos |
| 14 | Marco de la vista de ejemplo (`ManualStageBrush` + `ManualFrameBrush`) | #B30A0A0C → #CC070708, borde #59B3122E | Réplica del paso |
| 14a | Dentro, en el paso 1: réplica del login con `ascii-bg.png` (0.375), sombra, placa y viñeta propias | — | Es la pantalla real que se muestra (válido) |
| 15 | Botón "Gracias, pero no necesito ayuda" (vidrio) | `ManualGlassBrush` | Saltar a la despedida |
| 16 | Panel de despedida (vidrio) | `ManualGlassBrush` | Texto y Cerrar |
| 17 | InfoBar de errores | — | Archivos que faltan |
| 18 | Bocina (silenciar) | — | Silencio |
| 19 | Destello al cambiar de paso (`SignalFlashBrush`) | #24FFFFFF, opacidad 0 ↔ 1 en 0.15 s | Transición corta, no es fondo fijo |

## Qué sobra o se contradice

- **Tres oscurecimientos encimados**: velo de CapaFondo (5, 45 %) + viñeta (6) + velo radial del Manual (7, 55–65 %). Sobre el fondo elegido queda ~80 % de negro al centro y casi negro en las orillas; no se sabe cuál capa manda.
- **Degradado del login (2)** en el Manual: sin fondo elegido el Manual se ve gris con un halo claro arriba en lugar de negro limpio.
- **Piso (9)**: un cuarto oscurecimiento, solo abajo; junto con la viñeta hace una franja negra que no se explica.
- **Barrido (8)**: textura animada sin función que ensucia la pantalla.
- **Video de Lain (10a)**: el video no tiene alfa; su fondo #000000 es más oscuro que el fondo velado → el rectángulo negro de 315 × 560 se nota. Los degradados 10b/10c son de otro color (#050507) y de otro alfa que lo que hay alrededor, así que dibujan parches de otro tono en vez de esconder el borde. La figura de Lain está hecha de caracteres ASCII: el rectángulo negro con texto es la "textura de texto ASCII" que se ve encima del fondo.
- **Despedida (10 en estado Farewell)**: el mismo host se estira a 900 × 634 con opacidad 0.45, pero los degradados siguen midiendo 120/40/40 px y no hay degradado abajo; el fondo azul #06122C del video forma un rectángulo de otro tono, y la orilla izquierda del video (más clara en el original) queda como franja clara sin fundir.
- **`ascii-bg.png`** solo existe dentro de la réplica del login (14a): ahí se queda.

## Cómo queda (ver `docs/CAMBIOS_VISUALES.md`)

1. Fondo elegido como cuadro fijo (CapaFondo sin degradado, sin velo propio y sin viñeta en el Manual; sin fondo elegido: negro #080808).
2. Un solo velo uniforme `ManualVeilBrush` #99000000 (60 %).
3. Contenido: Lain (GIF con transparencia, o el video fundido como respaldo), notificaciones, paneles de vidrio y la despedida con sus cuatro orillas fundidas.
