using UnityEngine;

public class CardItemPresenter : MonoBehaviour
{
    #region 모델, 뷰 참조
    private CardItemModel cardItemModel;
    private CardItemView cardItemView;
    private void Awake()
    {
        cardItemModel = GetComponent<CardItemModel>();
        cardItemView = GetComponent<CardItemView>();
    }
    #endregion

    #region 구매 클릭 입력 이벤트 전달
    private void Start()
    {
        cardItemView.OnPurchaseClicked += CardItemView_OnPurchaseClicked;
    }

    private void CardItemView_OnPurchaseClicked(ulong inputClientId)
    {        
        int cardID = cardItemModel.CardItemStatusData.CardID;
        int cardPrice = cardItemModel.CardItemStatusData.Price;
        // TODO:CardShop에게 카드 구매 요청 : 카드 아이디, 플레이어 아이디, 카드 가격 보내주기
        //cardShopModel.TryPurchaseCard(cardID, inputClientId, cardPrice);
    }
    #endregion
}
