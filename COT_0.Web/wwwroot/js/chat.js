// Interop mínimo del chat (no se puede hacer desde C#): scroll automático y aviso de mensajes nuevos (el cuadro de escritura está en composer.js)
window.cotChat = {
    attach(page, holdMs) {
        const scroll = page.querySelector('.msg-scroll');
        const pill = page.querySelector('.msg-new');
        const isMessage = n => n.nodeType === 1 && (n.matches('.msg-line, .msg-sys') || n.querySelector('.msg-line, .msg-sys'));
        let atBottom = true;

        const toBottom = smooth => scroll.scrollTo({ top: scroll.scrollHeight, behavior: smooth ? 'smooth' : 'auto' });
        const showPill = on => pill.classList.toggle('on', on);

        scroll.addEventListener('scroll', () => {
            atBottom = scroll.scrollHeight - scroll.scrollTop - scroll.clientHeight < 80;
            if (atBottom) showPill(false);
        }, { passive: true });
        pill.addEventListener('click', () => { showPill(false); toBottom(true); });

        // Mensaje nuevo: baja si ya estaba abajo (o si es propio); si está leyendo arriba, solo avisa
        new MutationObserver(records => {
            const added = records.flatMap(r => [...r.addedNodes]).filter(isMessage);
            if (!added.length) return;
            if (atBottom || added.some(n => n.closest('.msg-own'))) toBottom(false);
            else showPill(true);
        }).observe(scroll, { childList: true, subtree: true });

        // El teclado del celular achica la pantalla: sigue pegado abajo
        new ResizeObserver(() => { if (atBottom) toBottom(false); }).observe(scroll);

        // Mantener presionado un emote = clic derecho (Safari de iPhone no dispara contextmenu; Blazor lo atiende)
        let hold = null;
        const release = () => { clearTimeout(hold); hold = null; };
        scroll.addEventListener('pointerdown', e => {
            const emote = e.pointerType !== 'mouse' && e.target.closest('.emote');
            if (!emote) return;
            release();
            hold = setTimeout(() => {
                hold = null;
                emote.dispatchEvent(new MouseEvent('contextmenu', { bubbles: true, cancelable: true }));
            }, holdMs);
        });
        ['pointerup', 'pointercancel', 'contextmenu'].forEach(t => scroll.addEventListener(t, release, true));

        toBottom(false);
        return { id: Intl.DateTimeFormat().resolvedOptions().timeZone, offsetMinutes: -new Date().getTimezoneOffset() };
    }
};
