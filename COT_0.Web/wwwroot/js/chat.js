// Interop mínimo del chat (no se puede hacer desde C#): Enter envía, scroll automático y aviso de mensajes nuevos
window.cotChat = {
    attach(page) {
        const scroll = page.querySelector('.msg-scroll');
        const pill = page.querySelector('.msg-new');
        const input = page.querySelector('.chat-text');
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

        input.addEventListener('keydown', e => {
            if (e.key !== 'Enter' || e.shiftKey || e.isComposing || e.keyCode === 229) return;
            e.preventDefault();
            // Directo al form: el botón puede seguir deshabilitado hasta que el servidor vea el texto
            if (input.form.requestSubmit) input.form.requestSubmit();
            else input.form.dispatchEvent(new Event('submit', { bubbles: true, cancelable: true }));
        });

        toBottom(false);
        return { id: Intl.DateTimeFormat().resolvedOptions().timeZone, offsetMinutes: -new Date().getTimezoneOffset() };
    }
};
