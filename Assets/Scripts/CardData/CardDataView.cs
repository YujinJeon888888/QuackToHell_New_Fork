using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public sealed class CardDataView : MonoBehaviour
{
    [Header("Google Sheets CSV URLs")]
    [SerializeField] string cardCsvUrl;     // Card_Table
    [SerializeField] string stringCsvUrl;   // String_Table
    [SerializeField] string resourceCsvUrl; // Resource_Table

    public static CardDataPresenter Presenter { get; private set; }
    CancellationTokenSource _cts;

    async void Start()
    {
#if UNITY_2023_1_OR_NEWER
        var exists = Object.FindObjectsByType<CardDataView>(FindObjectsSortMode.None);
#else
    var exists = Object.FindObjectsOfType<CardDataView>();
#endif
        if (exists.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _cts = new CancellationTokenSource();

        Presenter ??= new CardDataPresenter();
        try
        {
            await Presenter.PreloadAsync(cardCsvUrl, stringCsvUrl, resourceCsvUrl, _cts.Token);
            Debug.Log($"[CardData] Ready. Cards={Presenter.CardCount}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[CardData] init failed: {ex.Message}");
        }
    }

    void OnDestroy() { _cts?.Cancel(); _cts?.Dispose(); }
}
