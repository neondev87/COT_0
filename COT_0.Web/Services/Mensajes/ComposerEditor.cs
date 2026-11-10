using COT_0.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace COT_0.Web.Services.Mensajes;

/// <summary>Implementa IComposerEditor: puente con composer.js para el cuadro de escritura (Scoped).</summary>
public sealed class ComposerEditor(IJSRuntime js, ILogger<ComposerEditor> logger) : IComposerEditor
{
    private DotNetObjectReference<ComposerEditor>? _self;
    private ElementReference? _editor;
    private MessageDraft? _draft;

    public event Action? Changed;

    public async Task AttachAsync(ElementReference editor, MessageDraft draft)
    {
        _editor = editor;
        _draft = draft;
        _self ??= DotNetObjectReference.Create(this);
        await js.InvokeVoidAsync("cotComposer.attach", editor, _self, draft.MaxLength);
    }

    [JSInvokable]
    public void OnInput(string text) => SetText(text);

    public async Task InsertEmoteAsync(RoomEmote emote, bool replaceQuery)
    {
        if (_editor is not { } editor) return;

        var data = new { emote.Name, emote.Small.Url, emote.Width, emote.Height };
        // null: no cabe por el límite de caracteres
        if (await js.InvokeAsync<string?>("cotComposer.insertEmote", editor, data, replaceQuery) is { } text) SetText(text);
    }

    public async Task ClearAsync()
    {
        if (_editor is { } editor) await js.InvokeVoidAsync("cotComposer.clear", editor);
    }

    private void SetText(string text)
    {
        if (_draft is null) return;

        _draft.Text = text;
        Changed?.Invoke();
    }

    public async Task DetachAsync()
    {
        try
        {
            if (_editor is { } editor) await js.InvokeVoidAsync("cotComposer.detach", editor);
        }
        catch (Exception ex) when (ex is JSDisconnectedException or JSException or TaskCanceledException)
        {
            // El circuito ya se cerró: no hay nada que soltar en el navegador
            logger.LogDebug(ex, "No se pudo soltar el cuadro de escritura.");
        }

        _editor = null;
        _self?.Dispose();
        _self = null;
    }

    public async ValueTask DisposeAsync() => await DetachAsync();
}
