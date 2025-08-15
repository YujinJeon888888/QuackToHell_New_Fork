using UnityEngine;

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CardCatalogBootstrap : MonoBehaviour
{
    [Tooltip("csv 링크")]
    [SerializeField] private string csvUrl;

    CancellationTokenSource _cts;

    async void Start()
    {
        DontDestroyOnLoad(gameObject);
        _cts = new CancellationTokenSource();

        try
        {
            var source = new GoogleSheetCsvCatalogService(csvUrl);
            await CardCatalogRepository.Instance.InitializeAsync(source, _cts.Token);
            Debug.Log($"[CardCatalog] Ready. Count = {CardCatalogRepository.Instance.AsReadOnly().Count}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[CardCatalog] init failed: {ex.Message}");
        }
    }

    void OnDestroy() { _cts?.Cancel(); _cts?.Dispose(); }
}
