using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

/// <summary>
/// 카드 아이템 생성 팩토리
/// 카드 정보를 미리 로드한 뒤에, 생성해야 할 때 사용
/// </summary>
public class CardItemFactory : MonoBehaviour
{
    #region 싱글톤 코드
    //싱글톤로직
    private static CardItemFactory _instance;
    public static CardItemFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CardItemFactory>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CardItemFactory");
                    _instance = go.AddComponent<CardItemFactory>();
                }
            }
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region 생성할 카드 청사진(Definition)
    private readonly Dictionary<int, CardDef> _cards = new();
    public IReadOnlyDictionary<int, CardDef> Cards => _cards;

    public async Task SetCardData(CardKeyValuePair[] cardKeyValuePairs)
    {
        if (cardKeyValuePairs == null)
        {
            Debug.LogError("[CardItemFactory] 전달받은 카드 데이터가 null입니다.");
            return;
        }

        foreach (var card in cardKeyValuePairs)
        {
            _cards[card.Key] = card.Value;
        }
        Debug.Log($"[CardItemFactory] {_cards.Count}개 카드 데이터 설정 완료");
        foreach (var card in _cards)
        {
            Debug.Log($"[CardItemFactory] CardID: {card.Key}, CardNameKey: {card.Value.CardNameKey}");
        }
    }

    #endregion

    #region 카드 아이템 생성
    public GameObject cardItemForSalePrefab;

    public void CreateCardForSale(int cardId, Vector3 inputPosition)
    {
        #region 유효한 요청인지 확인
        //카드 ID가 존재하는지 확인
        if (!_cards.ContainsKey(cardId))
        {
            Debug.LogError($"[CardItemFactory] 카드 아이디에 맞는 카드 데이터가 없습니다. CardID: {cardId}");
            return;
        }
        #endregion

        //프리팹 생성
        GameObject cardItemForSale = Instantiate(cardItemForSalePrefab, inputPosition, Quaternion.identity);
        //주입할 데이터 생성
        CardDef cardDef = _cards[cardId];
        CardItemStatusData cardItemStatusData = new CardItemStatusData
        {
            CardItemID = cardId,
            CardID = cardId,
            //TODO: 가격 및 발언력 설정
            Price = cardDef.BasePrice,
            Cost = cardDef.BaseCost
        };

        //데이터 주입
        var cardItemModel = cardItemForSale.GetComponentInChildren<CardItemModel>();
        cardItemModel.CardDefData = cardDef;
        cardItemModel.CardItemStatusData = cardItemStatusData;
        Debug.Log($"[CardItemFactory] 카드 아이템 생성 완료. CardID: {cardId}, CardName: {cardDef.CardNameKey}, Price: {cardItemStatusData.Price}, Cost: {cardItemStatusData.Cost}, CardItemID: {cardItemStatusData.CardItemID}");
        //캔버스 부착
        cardItemForSale.transform.SetParent(GameObject.Find("CardShopCanvas").transform);
        cardItemForSale.transform.localScale = Vector3.one;
        cardItemForSale.transform.localPosition = inputPosition;
    }
    #endregion

    #region 테스트용 코드
    [Obsolete("카드 생성되는지 테스트하는 버튼")]
    public void OnTestCreateCardForSaleButton()
    {
        CardItemFactory.Instance.CreateCardForSale(20000, Vector3.zero);
    }
    #endregion
}
