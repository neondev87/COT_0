// Reconexión del circuito de Blazor: reintenta seguido, reintenta al volver a la pestaña
// (iPhone con la pantalla bloqueada) y recarga si el servidor ya no tiene la sesión
(() => {
    const config = document.body.dataset;
    const modal = document.getElementById('components-reconnect-modal');
    const has = state => modal.classList.contains(`components-reconnect-${state}`);

    Blazor.start({
        circuit: { reconnectionOptions: { maxRetries: Number(config.reconnectRetries), retryIntervalMilliseconds: Number(config.reconnectInterval) } }
    });

    new MutationObserver(() => { if (has('rejected')) location.reload(); })
        .observe(modal, { attributes: true, attributeFilter: ['class'] });

    document.addEventListener('visibilitychange', () => {
        if (document.visibilityState === 'visible' && has('failed')) Blazor.reconnect();
    });

    modal.querySelector('[data-retry]').addEventListener('click', () => location.reload());
})();
