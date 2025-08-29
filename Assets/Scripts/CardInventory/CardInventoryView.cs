using UnityEngine;
using System.Collections.Generic;

public class CardInventoryView : MonoBehaviour
{
    #region 인벤토리 모습 업데이트
    [Header("인벤토리 UI의 하위 오브젝트: Content를 넣어주세요.")]
    [SerializeField]
    private GameObject content;
    private GameObject cardShopPanel;
    private Animator cardShopPanelAnimator;
    private void Start()
    {
        GameObject cardShopParent = GameObject.FindWithTag("CardShop");
        cardShopPanel = cardShopParent.transform.Find("CardShopPanel").gameObject;
        cardShopPanelAnimator = cardShopPanel.GetComponent<Animator>();
    }
    
    public void UpdateInventoryView(List<InventoryCard> ownedCards)
    {
        //TODO 인벤토리 모습 업데이트
        //1. content 산하 오브젝트 삭제
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        //2. 팩토리에서 카드 아이템 생성
        foreach (var card in ownedCards)
        {
            CardItemFactory.Instance.CreateCardForInventory(card);
        }
    }
    #endregion

    #region 버튼
    public void XButton_OnClick()
    {
        gameObject.SetActive(false);
    }
    public void PlusButton_OnClick()
    {
        cardShopPanel.SetActive(true);
        cardShopPanelAnimator.SetBool("Active", true);
    }
    #endregion
}
