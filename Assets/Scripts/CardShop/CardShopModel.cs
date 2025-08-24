using UnityEngine;

public interface ICardShopModel
{
    void RequestPurchase(InventoryCard card, ulong clientId);

    bool IsLocked { get; set; }
    bool TryReRoll();
    //Test: 유진
    void DisplayCardsForSale();
}

public sealed class CardShopModel : ICardShopModel
{
    public bool IsLocked { get; set; }

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

    public bool TryReRoll()
    {
        if (IsLocked) return false;

        // TODO: 실제 카드 목록 섞기(지금은 성공만 반환)
        return true;
    }
    /////////////////////////////////////////////////////
    /// Test: 유진
    /// /////////////////////////////////////////////////
    

    //[Obsolete("카드 생성 테스트문 - 유진")]
    public void DisplayCardsForSale()
    {
        // 카드 크기: 200x350
        float cardWidth = 200f;
        float cardHeight = 350f;
        float spacing = 50f; // 카드 간 간격
        
        // 첫 번째 행 (위쪽)
        CardItemFactory.Instance.CreateCardForSale(10000, new Vector3(-cardWidth - spacing/2, cardHeight/2, 0));
        CardItemFactory.Instance.CreateCardForSale(20000, new Vector3(0, cardHeight/2, 0));
        CardItemFactory.Instance.CreateCardForSale(30000, new Vector3(cardWidth + spacing/2, cardHeight/2, 0));
        
        // 두 번째 행 (아래쪽)
        CardItemFactory.Instance.CreateCardForSale(10100, new Vector3(-cardWidth/2 - spacing/4, -cardHeight/2, 0));
        CardItemFactory.Instance.CreateCardForSale(20200, new Vector3(cardWidth/2 + spacing/4, -cardHeight/2, 0));
    }
}
