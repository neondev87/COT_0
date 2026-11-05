// Interop mínimo del manual (no se puede hacer desde C#): sonido de los avisos de Lain y atajos de teclado
(() => {
    const sound = () => document.getElementById('manual-sfx');
    const muted = () => document.querySelector('.manual')?.dataset.muted === 'true';
    // El toque en EMPEZAR ya hizo sonar el primer aviso: no se repite
    let skipUntil = 0;

    function play() {
        const audio = sound();
        if (!audio || muted()) return;
        audio.currentTime = 0;
        audio.play().catch(() => { /* sin permiso de audio todavía: el manual sigue igual */ });
    }

    // iPhone y Android solo dejan sonar audio dentro de un toque: arranca con EMPEZAR
    document.addEventListener('click', e => {
        if (!e.target.closest('[data-manual-start]')) return;
        play();
        skipUntil = performance.now() + 1500;
    });

    document.addEventListener('animationstart', e => {
        if (!e.target.classList.contains('manual-notif') || !e.animationName.startsWith('manualNotifIn')) return;
        if (performance.now() < skipUntil) return;
        play();
    });

    document.addEventListener('keydown', e => {
        if (e.altKey || e.ctrlKey || e.metaKey || e.defaultPrevented || !document.querySelector('.manual')) return;
        // Enter sobre un botón enfocado hace lo suyo
        if (e.key === 'Enter' && e.target.closest('button, a')) return;
        const target = document.querySelector(`.manual [data-keys~="${CSS.escape(e.key)}"]`);
        if (!target || target.disabled) return;
        e.preventDefault();
        target.click();
    });
})();
