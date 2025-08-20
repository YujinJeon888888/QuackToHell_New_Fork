using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public interface ICardShopView
{
    event Action<int, ulong, int> OnClickBuy; // (cardId, clientId, cardPrice)
    void ShowLoading(bool on);
    void ShowResult(bool success, string msg);
}

public sealed class CardShopView : MonoBehaviour, ICardShopView
{
    [Header("UI")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_InputField cardIdInput;
    [SerializeField] private TMP_InputField priceInput;
    [SerializeField] private TMP_Text statusText;

    public event Action<int, ulong, int> OnClickBuy;

    private void Awake()
    {
        buyButton.onClick.AddListener(() =>
        {
            int.TryParse(cardIdInput.text, out var cardId);
            int.TryParse(priceInput.text, out var price);
            // clientId는 로컬 클라의 ClientId를 전달 or Presenter가 채워줄 수도 있음
            OnClickBuy?.Invoke(cardId, 0UL, price);
        });
    }

    public void ShowLoading(bool on)
    {
        if (statusText) statusText.text = on ? "Processing..." : "";
    }

    public void ShowResult(bool success, string msg)
    {
        if (statusText) statusText.text = success ? $"✅ {msg}" : $"❌ {msg}";
    }
}
