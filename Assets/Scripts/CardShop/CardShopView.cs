using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public interface ICardShopView
{
    event Action<InventoryCard, ulong> OnClickBuy; // (card, clientId)
    void ShowLoading(bool on);
    void ShowResult(bool success, string msg);
}

public sealed class CardShopView : MonoBehaviour, ICardShopView
{
    [Header("UI")]
    //[SerializeField] private Button buyButton;
    [SerializeField] private TMP_InputField cardIdInput;
    [SerializeField] private TMP_InputField priceInput;
    [SerializeField] private TMP_Text statusText;

    public event Action<InventoryCard, ulong> OnClickBuy;

    private void Awake()
    {
        /*buyButton.onClick.AddListener(() =>
        {
            if (int.TryParse(cardIdInput.text, out var cardId))
            {
                var card = new InventoryCard { CardID = cardId };
                OnClickBuy?.Invoke(card, 0UL);
            }
        });*/
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
