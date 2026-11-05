// Interop mínimo del manual (no se puede hacer desde C#): sonido de los avisos de Lain, atajos de teclado,
// video de Lain (sin audio; quieto con movimiento reducido) y apagado de la tele del final
(() => {
    const root = () => document.querySelector('.manual');
    const sound = () => document.getElementById('manual-sfx');
    const muted = () => root()?.dataset.muted === 'true';
    const reduced = () => matchMedia('(prefers-reduced-motion: reduce)').matches;
    const TV_OFF_MS = 700;
    // El toque en EMPEZAR ya hizo sonar el primer aviso: no se repite
    let skipUntil = 0;

    function play() {
        const audio = sound();
        if (!audio || muted()) return;
        audio.currentTime = 0;
        audio.play().catch(() => { /* sin permiso de audio todavía: el manual sigue igual */ });
    }

    // Blazor crea el <video> sin la propiedad muted: se fuerza para que el autoplay funcione en iPhone y Android
    document.addEventListener('loadedmetadata', e => {
        const video = e.target;
        if (!(video instanceof HTMLVideoElement) || !video.closest('.manual')) return;
        video.muted = true;
        if (reduced()) video.pause();
        else video.play().catch(() => { /* bajo consumo: se queda el poster */ });
    }, true);

    document.addEventListener('click', e => {
        const manual = root();
        if (!manual) return;
        // iPhone y Android solo dejan sonar audio dentro de un toque: arranca con EMPEZAR
        if (e.target.closest('[data-manual-start]')) {
            play();
            skipUntil = performance.now() + 1500;
            return;
        }
        // Tocar la tele la apaga; con la tele apagada, cualquier toque fuera de un botón la enciende
        if (e.target.closest('[data-manual-tv]') || (manual.dataset.tele === 'off' && !e.target.closest('a, button'))) {
            manual.dataset.tele = manual.dataset.tele === 'off' ? '' : 'off';
            return;
        }
        // ENTRAR A LA SALA: la tele se apaga y luego se navega
        const leave = e.target.closest('[data-manual-leave]');
        if (!leave || reduced()) return;
        e.preventDefault();
        e.stopPropagation();
        manual.dataset.tele = 'off';
        setTimeout(() => location.assign(leave.href), TV_OFF_MS);
    }, true);

    document.addEventListener('animationstart', e => {
        if (!e.target.classList.contains('manual-notif') || !e.animationName.startsWith('manualNotifIn')) return;
        if (performance.now() < skipUntil) return;
        play();
    });

    document.addEventListener('keydown', e => {
        if (e.altKey || e.ctrlKey || e.metaKey || e.defaultPrevented || !root()) return;
        // Enter sobre un botón enfocado hace lo suyo
        if (e.key === 'Enter' && e.target.closest('button, a')) return;
        const target = document.querySelector(`.manual [data-keys~="${CSS.escape(e.key)}"]`);
        if (!target || target.disabled) return;
        e.preventDefault();
        target.click();
    });
})();
