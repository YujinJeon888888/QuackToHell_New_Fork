using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using TMPro;

/// <summary>
/// 시각 및 입력 처리
/// </summary>
public class PlayerView : NetworkBehaviour
{
    private TextMeshProUGUI nicknameText;
    
    //닉네임
    private void Start()
    {
        var canvas = gameObject.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            nicknameText = canvas.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // PlayerStatusData 전체의 OnValueChanged 이벤트 구독
        var playerModel = GetComponent<PlayerModel>();
        if (playerModel != null && playerModel.PlayerStatusData != null)
        {
            // 초기값 설정
            UpdateNickname(playerModel.PlayerStatusData.Value.Nickname);
            
            // PlayerStatusData 변경 시 닉네임 업데이트
            playerModel.PlayerStatusData.OnValueChanged += (previousValue, newValue) =>
            {
                UpdateNickname(newValue.Nickname);
            };
        }
    }
    
    private void UpdateNickname(string nickname)
    {
        if (nicknameText != null)
        {
            nicknameText.text = nickname;
            Debug.Log($"PlayerView: Nickname updated to '{nickname}' for {gameObject.name}");
        }
    }

    //움직임
    public EventHandler OnMovementInput;

    public class OnMovementInputEventArgs: EventArgs{
        
        private int xDirection;
        private int yDirection;
        
        public int XDirection
        {
            get { return xDirection; }
        }
        public int YDirection
        {
            get { return yDirection; }
        }

        public OnMovementInputEventArgs(int inputXDirection, int inputYDirection)
        {
            xDirection = inputXDirection;
            yDirection = inputYDirection;
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        int inputXDirection = 0;
        int inputYDirection = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            inputYDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            inputYDirection = -1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            inputXDirection = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            inputXDirection = 1;
        }

        if (inputXDirection != 0f || inputYDirection != 0f)
        {
            OnMovementInput?.Invoke(this, new OnMovementInputEventArgs(inputXDirection, inputYDirection));
        }
        else
        {
            OnMovementInput?.Invoke(this, new OnMovementInputEventArgs(0, 0));
        }
    }
}
