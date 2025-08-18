using UnityEngine;

public class CardItemSoldState : State
{
    public override void OnStateEnter()
    {
        // 만약 Sold상태면, 오브젝트 비활성화. (안보이게 + 클릭 안 되게)
        gameObject.SetActive(false);
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {
        
    }
}
