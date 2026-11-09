// Lista de la TIENDA que se llena por partes: la cuadrícula pide la siguiente página de 7TV al llegar al final del scroll
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// ISupportIncrementalLoading: el GridView llama LoadMoreItemsAsync solo. Si una página falla, se detiene
/// (HasMoreItems = false) y REINTENTAR la vuelve a pedir con ResumeAsync.
/// </summary>
public sealed class ShopItemsCollection(Func<int, Task<IReadOnlyList<ShopEmoteViewModel>?>> loadPage)
    : ObservableCollection<ShopEmoteViewModel>, ISupportIncrementalLoading
{
    private readonly HashSet<string> _ids = [];
    private int _nextPage = 1;
    private bool _isBusy;
    private bool _isFinished;
    private bool _isPaused;

    public bool HasMoreItems => !_isFinished && !_isPaused;

    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) => LoadNextAsync().AsAsyncOperation();

    /// <summary>Después de un error: vuelve a pedir la página que falló.</summary>
    public Task ResumeAsync()
    {
        _isPaused = false;
        return LoadNextAsync();
    }

    private async Task<LoadMoreItemsResult> LoadNextAsync()
    {
        if (_isBusy || !HasMoreItems) return new LoadMoreItemsResult { Count = 0 };

        _isBusy = true;
        try
        {
            var items = await loadPage(_nextPage);
            if (items is null)
            {
                _isPaused = true;
                return new LoadMoreItemsResult { Count = 0 };
            }

            _nextPage++;
            var added = 0u;
            // 7TV puede repetir un emote entre páginas: se agrega una sola vez
            foreach (var item in items.Where(i => _ids.Add(i.Emote.Id)))
            {
                Add(item);
                added++;
            }
            if (items.Count == 0) _isFinished = true;
            return new LoadMoreItemsResult { Count = added };
        }
        finally
        {
            _isBusy = false;
        }
    }
}
