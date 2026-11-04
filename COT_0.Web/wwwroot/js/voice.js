// Interop mínimo de notas de voz: micrófono (MediaRecorder) y <audio>. Validar, guardar y avisar lo hace C#.
window.cotVoice = (() => {
    const errorCodes = { NotAllowedError: 'denied', SecurityError: 'denied', NotFoundError: 'nodevice', OverconstrainedError: 'nodevice', NotReadableError: 'busy' };
    const stopTracks = s => s.stream?.getTracks().forEach(t => t.stop());

    // ---- Grabar: mantener presionado, soltar envía, deslizar a la izquierda cancela ----
    function attach(form, dotnet, config) {
        const mic = form.querySelector('.voice-mic');
        let session = null;

        const slide = dx => {
            form.style.setProperty('--voice-slide', `${dx}px`);
            if (dx <= -config.cancelPx) form.dataset.voiceCancel = '';
            else delete form.dataset.voiceCancel;
        };

        async function begin(e) {
            if (session || e.button > 0) return;
            e.preventDefault();
            mic.setPointerCapture(e.pointerId);

            if (!navigator.mediaDevices?.getUserMedia) return dotnet.invokeMethodAsync('OnFailed', window.isSecureContext ? 'unsupported' : 'insecure');
            const mime = window.MediaRecorder && config.mimeCandidates.find(t => MediaRecorder.isTypeSupported(t));
            if (!mime) return dotnet.invokeMethodAsync('OnFailed', 'unsupported');

            const s = session = { startX: e.clientX, chunks: [] };
            try {
                s.stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            } catch (err) {
                if (session === s) session = null;
                return dotnet.invokeMethodAsync('OnFailed', errorCodes[err?.name] ?? 'error');
            }

            // Soltó mientras se pedía el permiso: no se graba nada
            if (s.released) {
                stopTracks(s);
                return dotnet.invokeMethodAsync('OnCancelled', true);
            }

            s.recorder = new MediaRecorder(s.stream, { mimeType: mime });
            s.recorder.ondataavailable = ev => { if (ev.data.size > 0) s.chunks.push(ev.data); };
            s.recorder.onstop = () => finish(s);
            s.recorder.start();
            s.startedAt = performance.now();
            // Al llegar al límite se ENVÍA
            s.limit = setTimeout(() => end(true), config.maxMs);
            dotnet.invokeMethodAsync('OnStarted');
        }

        function end(send) {
            const s = session;
            if (!s) return;
            session = null;
            s.released = true;
            slide(0);
            if (!s.recorder) return;

            clearTimeout(s.limit);
            s.send = send;
            s.duration = Math.min(performance.now() - s.startedAt, config.maxMs);
            if (s.recorder.state !== 'inactive') s.recorder.stop();
        }

        // El Blob recién está completo en onstop
        function finish(s) {
            stopTracks(s);
            if (!s.send) return dotnet.invokeMethodAsync('OnCancelled', false);
            if (s.duration < config.minMs || !s.chunks.length) return dotnet.invokeMethodAsync('OnCancelled', true);

            const blob = new Blob(s.chunks, { type: s.recorder.mimeType });
            dotnet.invokeMethodAsync('OnRecorded', DotNet.createJSStreamReference(blob), Math.round(s.duration));
        }

        mic.addEventListener('pointerdown', begin);
        mic.addEventListener('pointermove', e => { if (session) slide(Math.max(-150, Math.min(0, e.clientX - session.startX))); });
        mic.addEventListener('pointerup', () => end(!('voiceCancel' in form.dataset)));
        mic.addEventListener('pointercancel', () => end(false));
        mic.addEventListener('contextmenu', e => e.preventDefault());
        form.cotVoiceDetach = () => end(false);
    }

    function detach(form) {
        form?.cotVoiceDetach?.();
    }

    // ---- Reproducir: el estado sale de los eventos reales del <audio> ----
    const format = sec => {
        const s = Math.max(0, Math.round(sec || 0));
        return `${Math.floor(s / 60)}:${String(s % 60).padStart(2, '0')}`;
    };
    const parts = audio => {
        const bubble = audio.closest('.voice');
        return bubble && { bubble, fill: bubble.querySelector('.voice-fill'), time: bubble.querySelector('.voice-time'), duration: Number(bubble.dataset.duration) };
    };

    function draw(audio) {
        const p = parts(audio);
        if (!p) return;
        const current = audio.currentTime;
        p.fill.style.transform = `scaleX(${Math.min(1, Math.max(0, current / p.duration))})`;
        p.time.textContent = format(current > .05 ? p.duration - current : p.duration);
    }

    function loop(audio) {
        draw(audio);
        if (!audio.paused) requestAnimationFrame(() => loop(audio));
    }

    const setState = (audio, state) => {
        const p = parts(audio);
        if (p) p.bubble.dataset.state = state;
    };

    // Los eventos de <audio> no burbujean: se escuchan en captura
    const on = (name, handler) => document.addEventListener(name, e => { if (e.target instanceof HTMLAudioElement) handler(e.target); }, true);

    // Solo un audio a la vez
    on('play', audio => document.querySelectorAll('audio').forEach(a => { if (a !== audio) a.pause(); }));
    on('playing', audio => { setState(audio, 'playing'); loop(audio); });
    on('pause', audio => { setState(audio, ''); draw(audio); });
    on('ended', audio => { audio.currentTime = 0; setState(audio, ''); draw(audio); });
    on('seeked', draw);
    on('error', audio => setState(audio, 'error'));

    document.addEventListener('click', e => {
        const bubble = e.target.closest('.voice');
        if (!bubble || bubble.dataset.state === 'error') return;
        const audio = bubble.querySelector('audio');

        const track = e.target.closest('.voice-track');
        if (track) {
            const rect = track.getBoundingClientRect();
            audio.currentTime = Math.min(1, Math.max(0, (e.clientX - rect.left) / rect.width)) * Number(bubble.dataset.duration);
            return;
        }

        if (!e.target.closest('.voice-play')) return;
        if (!audio.paused) return audio.pause();

        // "Reproduciendo" recién cuando llega el evento playing; si play() falla se avisa
        bubble.dataset.state = 'loading';
        // AbortError = lo pausó otro audio antes de arrancar: no es un error
        audio.play().catch(err => { bubble.dataset.state = err?.name === 'AbortError' ? '' : 'error'; });
    });

    return { attach, detach };
})();
