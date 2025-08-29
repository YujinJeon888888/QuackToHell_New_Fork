using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public interface ICardShopModel
{
    void RequestPurchase(InventoryCard card, ulong clientId);

    bool IsLocked { get; set; }
    bool TryReRoll();
    void DisplayCardsForSale();
}

public sealed class CardShopModel 
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

    #region 카드 목록 새로고침
    public bool TryReRoll()
    {
        if (IsLocked) return false;

        // 새로 뿌리기
        DisplayCardsForSale();

        return true;
    }

    private Transform _row;
    private Transform GetRow()
    {
        Debug.Log($"[CardShopModel] GetRow 실행됨");
        if (_row == null)
        {
            GameObject rowParent = GameObject.FindWithTag("CardShop");
            if (rowParent != null)
            {
                // 모든 자손에서 CardShopRow 찾기 (비활성화된 것도 포함)
                Transform[] allChildren = rowParent.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.name == "CardShopRow")
                    {
                        _row = child;
                        break;
                    }
                }
            }
        }
        return _row;
    }
    public void DisplayCardsForSale()
    {
        Debug.Log($"[CardShopModel] 카드 진열함수 들어옴");
        if (IsLocked)
        {
            Debug.Log("[CardShopModel] Locked: skip DisplayCardsForSale");
            return;
        }

        var row = GetRow();
        if (row == null) return;
        Debug.Log($"[CardShopModel] 카드 진열함수: 부모오브젝트 세팅 완료");

        // 기존 카드 다 지우기
        for (int i = row.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(row.GetChild(i).gameObject);
        }

        // 5개 생성해서 Row 밑으로 붙이기
        int[] cardIds = { 10000, 20000, 30000, 10100, 20200 };
        for (int i = 0; i < cardIds.Length; i++)
        {
            Debug.Log($"[CardShopModel] 카드 진열함수: 카드 생성");
            CardItemFactory.Instance.CreateCardForSale(cardIds[i], Vector3.zero);
        }

        // 스폰된 카드들을 전부 Row 밑으로 이동
        var spawned = GameObject.FindGameObjectsWithTag("CardForSale");
        foreach (var go in spawned)
        {
            Debug.Log($"[CardShopModel] 카드 진열함수: 카드의 부모오브젝트 세팅");
            go.transform.SetParent(row, false);
        }
    }
    #endregion
}
