using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class CardItemView : NetworkBehaviour
{
    #region 구매 클릭 입력 이벤트
    //인자로, 구매하려는 플레이어의 클라이언트 아이디 전달
    public event System.Action<ulong> OnPurchaseClicked;

    private void OnMouseDown()
    {
        //만약 오브젝트가 Card for Sale이라면 구매 클릭 이벤트 전달
        if (gameObject.name == "Card for Sale")
        {
            OnPurchaseClicked?.Invoke(NetworkManager.Singleton.LocalClientId);
        }
    }

    #endregion
}
