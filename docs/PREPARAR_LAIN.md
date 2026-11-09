# Preparar a Lain con transparencia (opción A)

El video de Lain (`gifs\lain_tele_web.mp4`, 480 × 854, 30 fps, sin audio en uso: el reproductor del manual siempre lo tuvo en silencio) no tiene canal alfa: su fondo es negro puro (#000000, muestreado con ffmpeg). Para que Lain flote sobre el fondo del Manual sin cuadro negro se prepara **una sola vez**, fuera de la app, una copia GIF con el negro convertido en transparente. Sin shader ni filtros en vivo: la app solo muestra el GIF.

## Dónde queda

- Archivo: `gifs\lain-transparente.gif`, junto al original, dentro de `Paths:MediaRoot` (`C:\Users\neonpc\Documents\COT_0_P`). Fuera del repo e ignorado por git (`/gifs/` y `lain-transparente*.gif` en `.gitignore`).
- Configuración: `Manual:LainCutout` en `COT_0.Host/appsettings.json`.
- Si el archivo no está (otra computadora) o no se puede leer, el Manual usa el video original fundido con franjas negras (opción B) y no muestra error por el GIF que falta.

## Comando (Git Bash o PowerShell, desde `C:\Users\neonpc\Documents\COT_0_P\gifs`)

```
ffmpeg -y -i lain_tele_web.mp4 -filter_complex "fps=12,scale=-2:560:flags=lanczos,format=rgba,colorkey=0x000000:0.10:0.06,split[a][b];[a]palettegen=reserve_transparent=1:stats_mode=full:max_colors=128[p];[b][p]paletteuse=alpha_threshold=96:dither=none" -loop 0 lain-transparente.gif
```

- `fps=12` y `scale=-2:560`: 12 cuadros por segundo y 314 × 560 (el tamaño exacto con que se muestra, sin reescalar en la app).
- `colorkey=0x000000:0.10:0.06`: el negro y lo casi negro (ruido de compresión) se vuelve transparente, con un margen suave; el azul de la figura queda lejos del negro y no se toca.
- `palettegen=reserve_transparent=1` + `paletteuse=alpha_threshold=96`: el GIF reserva un color para lo transparente y corta el alfa suave en 96 (el GIF solo tiene transparencia de sí/no). `dither=none` evita puntos sueltos en el fondo.
- `-loop 0`: bucle infinito.

Resultado de la última vez (08/11): **2.1 MB**, 314 × 560, 12 fps, 93 cuadros.

## Cómo revisarlo

```
ffmpeg -v error -y -i lain-transparente.gif -vf "select=eq(n\,24)" -frames:v 1 -pix_fmt rgba cuadro24.png
ffmpeg -v error -y -f lavfi -i "color=c=0xC03030:s=314x560:d=1" -i cuadro24.png -filter_complex "[0:v][1:v]overlay=format=auto" -frames:v 1 cuadro24_rojo.png
ffmpeg -v error -i lain-transparente.gif -vf "format=rgba,alphaextract,signalstats,metadata=print:key=lavfi.signalstats.YAVG:file=-" -f null -
```

1. Abre `cuadro24_rojo.png`: el fondo debe verse rojo liso (transparente) y la figura azul completa, sin contorno negro grueso.
2. El último comando imprime el promedio del canal alfa de cada cuadro: debe quedar entre ~10 % y ~15 % opaco en **todos** los cuadros (si algún cuadro da 100 %, el negro no se quitó).

## Regenerarlo

Si cambias el video de Lain, borra `lain-transparente.gif` y vuelve a correr el comando. Si el borde se come partes oscuras de la figura, baja el `0.10` de `colorkey` (por ejemplo `0.07`); si queda ruido negro alrededor, súbelo (por ejemplo `0.14`).
