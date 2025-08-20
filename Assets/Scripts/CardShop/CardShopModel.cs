using UnityEngine;

// Model: 서버 호출만 담당
public interface ICardShopModel
{
    void RequestPurchase(InventoryCard card, ulong clientId);
}

public sealed class CardShopModel : ICardShopModel
{
    public void RequestPurchase(InventoryCard card, ulong clientId)
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogError("[CardShopModel] DeckManager.Instance is null");
            return;
        }
        Debug.Log("[CardShopModel] RequestPurchase 실행됨");
        DeckManager.Instance.TryPurchaseCardServerRpc(card, clientId);
    }
}
