using UnityEngine;

// Model: 서버 호출만 담당
public interface ICardShopModel
{
    void RequestPurchase(int cardId, ulong clientId, int cardPrice);
}

public sealed class CardShopModel : ICardShopModel
{
    public void RequestPurchase(int cardId, ulong clientId, int cardPrice)
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogError("[CardShopModel] DeckManager.Instance is null");
            return;
        }
        // DeckManager.Instance.TryPurchaseCardServerRpc(cardId, clientId, cardPrice);
    }
}
