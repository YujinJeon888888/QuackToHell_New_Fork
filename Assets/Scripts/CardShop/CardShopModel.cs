using UnityEngine;

public interface ICardShopModel
{
    void RequestPurchase(int cardId, ulong clientId, int cardPrice);

    bool IsLocked { get; set; }
    bool TryReRoll();
}

public sealed class CardShopModel : ICardShopModel
{
    public bool IsLocked { get; set; }

    public void RequestPurchase(int cardId, ulong clientId, int cardPrice)
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogError("[CardShopModel] DeckManager.Instance is null");
            return;
        }
        Debug.Log("[CardShopModel] RequestPurchase 실행됨");
        DeckManager.Instance.TryPurchaseCardServerRpc(card, clientId);
    }

    public bool TryReRoll()
    {
        if (IsLocked) return false;

        // TODO: 실제 카드 목록 섞기(지금은 성공만 반환)
        return true;
    }
}
