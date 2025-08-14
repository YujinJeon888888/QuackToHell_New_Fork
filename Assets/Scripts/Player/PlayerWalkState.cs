using UnityEngine;

public class PlayerWalkState : State
{
    private Animator animator;
    private SpriteRenderer head;
    
    private void Start()
    {
        animator = gameObject.transform.Find("Body").gameObject.GetComponent<Animator>();
        head = gameObject.transform.Find("Head").gameObject.GetComponent<SpriteRenderer>();
    }
    
    public override void OnStateEnter()
    {
        TriggerWalkAnimation();
    }
    
    // 트리거 방식으로 애니메이션 제어
    public void TriggerWalkAnimation()
    {
        if (animator == null)
        {
            // animator가 null이면 다시 찾기
            animator = gameObject.transform.Find("Body").gameObject.GetComponent<Animator>();
        }
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            Debug.LogError("PlayerWalkState: Animator not found!");
        }
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {
        //*머리 default: 오른쪽 바라봄
        //왼쪽 키 누르면 머리 플립
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            head.flipX = true;
        }
        //오른쪽 키 누르면 머리 플립x
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            head.flipX = false;
        }
    }
}
