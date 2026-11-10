// Interop del cuadro de escritura (contenteditable; Blazor no lo puede enlazar): muestra los emotes como imagen
// y le pasa a C# el texto con las claves. Enter envía, Shift+Enter salto de línea, Tab completa la sugerencia de ":"
window.cotComposer = (() => {
    const isEmote = n => n?.nodeType === 1 && n.matches('img.emote-inline');
    const isPad = text => /^[  ]?$/.test(text);
    // Ignora nodos de texto vacíos que dejan los navegadores al partir texto
    const skipEmpty = (n, step) => { while (n?.nodeType === 3 && !n.data) n = n[step]; return n; };

    // DOM → texto. El emote va como su clave separada por espacios (si no, el chat no lo reconoce).
    // Un <br> sin nada después es el relleno del navegador para la última línea: no cuenta
    function serialize(root) {
        let out = '';
        let pad = false;
        const walk = parent => parent.childNodes.forEach(n => {
            if (n.nodeType === 3) {
                const text = n.data.replace(/ /g, ' ');
                if (pad && text && !/^\s/.test(text)) out += ' ';
                if (text) pad = false;
                out += text;
            } else if (isEmote(n)) {
                if (out && !/\s$/.test(out)) out += ' ';
                out += n.dataset.emote;
                pad = true;
            } else if (n.nodeName === 'BR') {
                if (skipEmpty(n.nextSibling, 'nextSibling')) { out += '\n'; pad = false; }
            } else {
                if (/^(DIV|P)$/.test(n.nodeName) && out && !out.endsWith('\n')) out += '\n';
                walk(n);
            }
        });
        walk(root);
        return out;
    }

    const serializeRange = range => {
        const box = document.createElement('div');
        box.append(range.cloneContents());
        return serialize(box);
    };

    const rangeIn = root => {
        const sel = getSelection();
        return sel.rangeCount && root.contains(sel.getRangeAt(0).commonAncestorContainer) ? sel.getRangeAt(0) : null;
    };

    const endOf = root => {
        const r = document.createRange();
        r.selectNodeContents(root);
        r.collapse(false);
        return r;
    };

    // Cursor entre nodos justo después de un texto → al final de ese texto (para poder quitarle los espacios)
    function intoText(range) {
        const prev = range.collapsed && range.startContainer.childNodes[range.startOffset - 1];
        if (prev?.nodeType !== 3) return range;
        const r = document.createRange();
        r.setStart(prev, prev.data.length);
        return r;
    }

    // Deja el cursor en range; solo enfoca (abre el teclado del celular) si se pide
    function place(root, range, focus) {
        root.cotRange = range.cloneRange();
        if (!focus) return;
        root.focus({ preventScroll: true });
        const sel = getSelection();
        sel.removeAllRanges();
        sel.addRange(range);
    }

    // Inserta nodos en el cursor y lo deja después. Si termina en <br>, otro <br> de relleno para que se vea la línea
    function insert(range, nodes) {
        range.deleteContents();
        const frag = document.createDocumentFragment();
        frag.append(...nodes);
        const last = frag.lastChild;
        range.insertNode(frag);
        if (last.nodeName === 'BR' && !skipEmpty(last.nextSibling, 'nextSibling')) last.after(document.createElement('br'));
        range.setStartAfter(last);
        range.collapse(true);
    }

    // Texto pegado: solo texto plano, los saltos de línea como <br>
    const textNodes = text => text.split('\n').flatMap((line, i) => i ? [document.createElement('br'), line] : [line]).filter(n => n !== '');

    function emoteImage(emote) {
        const img = document.createElement('img');
        img.className = 'emote-inline';
        img.src = emote.url; // animado siempre, como en el chat
        img.alt = emote.name;
        img.width = emote.width;
        img.height = emote.height;
        img.draggable = false;
        img.decoding = 'async';
        img.referrerPolicy = 'no-referrer';
        img.dataset.emote = emote.name;
        return img;
    }

    // Lo que se está escribiendo después de ":" al final (mismo criterio que MessageDraft.EmoteQuery)
    function queryRange(root) {
        let n = root.lastChild;
        while (n && (n.nodeName === 'BR' || (n.nodeType === 3 && !n.data))) n = n.previousSibling;
        const match = n?.nodeType === 3 && /(^|\s):[^\s:]*$/.exec(n.data);
        if (!match) return endOf(root);
        const r = document.createRange();
        r.setStart(n, match.index + match[1].length);
        r.setEnd(n, n.data.length);
        return r;
    }

    // Con texto antes: exactamente dos espacios antes del emote; vacío o inicio de línea: ninguno. Uno después para seguir escribiendo
    function insertEmote(root, emote, replaceQuery) {
        const saved = root.cotRange && root.contains(root.cotRange.startContainer) ? root.cotRange : null;
        const range = intoText(replaceQuery ? queryRange(root) : rangeIn(root) ?? saved ?? endOf(root));
        if (serialize(root).length - serializeRange(range).length + emote.name.length + 3 > root.cotMax) return null;

        range.deleteContents();
        const node = range.startContainer;
        if (node.nodeType === 3) {
            const head = node.data.slice(0, range.startOffset);
            const kept = head.replace(/[  ]+$/, '').length;
            node.deleteData(kept, head.length - kept);
            range.setStart(node, kept);
            range.collapse(true);
        }
        const before = document.createRange();
        before.setStart(root, 0);
        before.setEnd(range.startContainer, range.startOffset);
        const head = serializeRange(before);
        const prefix = head && !head.endsWith('\n') ? ['  '] : [];

        insert(range, [...prefix, emoteImage(emote), ' ']);
        // Con la carita abierta en el celular no se abre el teclado; con mouse o si ya estaba escribiendo, sí
        place(root, range, document.activeElement === root || matchMedia('(pointer: fine)').matches);
        return serialize(root);
    }

    // Backspace justo después de un emote (o de su espacio) lo borra entero, con los espacios que se le pusieron antes
    function removeEmoteBefore(root) {
        const range = rangeIn(root);
        if (!range?.collapsed) return false;
        const { startContainer: node, startOffset: offset } = range;
        let prev = node.childNodes[offset - 1];
        let pad = null;
        if (node.nodeType === 3) {
            if (!isPad(node.data.slice(0, offset))) return false;
            if (offset) pad = [node, offset];
            prev = node.previousSibling;
        }
        prev = skipEmpty(prev, 'previousSibling');
        if (prev?.nodeType === 3) {
            if (pad || !isPad(prev.data)) return false;
            pad = [prev, prev.data.length];
            prev = skipEmpty(prev.previousSibling, 'previousSibling');
        }
        if (!isEmote(prev)) return false;

        if (pad) pad[0].deleteData(0, pad[1]);
        const before = skipEmpty(prev.previousSibling, 'previousSibling');
        const parent = prev.parentNode;
        const index = [...parent.childNodes].indexOf(prev);
        prev.remove();
        if (before?.nodeType === 3) {
            before.data = before.data.replace(/[  ]{1,2}$/, '');
            range.setStart(before, before.data.length);
        } else {
            range.setStart(parent, index);
        }
        range.collapse(true);
        place(root, range, true);
        return true;
    }

    function attach(root, dotnet, max) {
        const form = root.closest('form');
        root.cotMax = max;

        const sync = () => dotnet.invokeMethodAsync('OnInput', serialize(root));
        const submit = () => form.requestSubmit ? form.requestSubmit() : form.dispatchEvent(new Event('submit', { bubbles: true, cancelable: true }));
        const roomFor = (range, text) => serialize(root).length - (range ? serializeRange(range).length : 0) + text.length <= max;

        function lineBreak() {
            const range = rangeIn(root);
            if (!range || !roomFor(range, '\n')) return;
            insert(range, [document.createElement('br')]);
            place(root, range, true);
            sync();
        }

        root.addEventListener('keydown', e => {
            const suggestion = e.key === 'Tab' && !e.shiftKey && form.querySelector('.emote-panel.suggest .emote-option');
            if (suggestion) {
                e.preventDefault();
                suggestion.click();
                return;
            }
            if (e.key !== 'Enter' || e.isComposing || e.keyCode === 229) return;
            e.preventDefault();
            if (e.shiftKey) lineBreak();
            else submit();
        });

        root.addEventListener('beforeinput', e => {
            const type = e.inputType;
            // Sin negritas ni nada: solo texto y emotes
            if (type.startsWith('format')) return e.preventDefault();
            // Enter del teclado del celular (llega sin keydown útil)
            if (type === 'insertParagraph' || type === 'insertLineBreak') {
                e.preventDefault();
                return type === 'insertParagraph' ? submit() : lineBreak();
            }
            if (type === 'deleteContentBackward' && removeEmoteBefore(root)) {
                e.preventDefault();
                return sync();
            }
            // Límite de caracteres (mientras se compone una palabra no se puede frenar; C# recorta al enviar)
            if (type.startsWith('insert') && e.data && !e.isComposing && !roomFor(rangeIn(root), e.data)) e.preventDefault();
        });

        root.addEventListener('input', e => {
            // Vacío de verdad: vuelve el placeholder
            if (!e.isComposing && !root.textContent && !root.querySelector('img')) root.replaceChildren();
            sync();
        });

        root.addEventListener('paste', e => {
            e.preventDefault();
            const range = rangeIn(root);
            if (!range) return;
            const room = max - serialize(root).length + serializeRange(range).length;
            const text = (e.clipboardData?.getData('text/plain') ?? '').replace(/\r\n?/g, '\n').slice(0, Math.max(0, room));
            if (!text) return;
            insert(range, textNodes(text));
            place(root, range, true);
            sync();
        });

        // Copiar y cortar: el texto con las claves, no la imagen
        const copy = cut => e => {
            const range = rangeIn(root);
            if (!range || range.collapsed) return;
            e.preventDefault();
            e.clipboardData.setData('text/plain', serializeRange(range));
            if (!cut) return;
            range.deleteContents();
            sync();
        };
        root.addEventListener('copy', copy(false));
        root.addEventListener('cut', copy(true));
        root.addEventListener('drop', e => e.preventDefault());

        // Se guarda el cursor para insertar el emote ahí aunque el foco esté en el buscador
        root.cotSelection = () => { const r = rangeIn(root); if (r) root.cotRange = r.cloneRange(); };
        document.addEventListener('selectionchange', root.cotSelection);
    }

    function detach(root) {
        if (root?.cotSelection) document.removeEventListener('selectionchange', root.cotSelection);
    }

    function clear(root) {
        root.replaceChildren();
        root.cotRange = null;
    }

    return { attach, detach, insertEmote, clear };
})();
