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
    private PlayerModel playerModel;
    //닉네임
    private void Start()
    {
        playerModel = gameObject.GetComponent<PlayerModel>();
        var canvas = gameObject.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            nicknameText = canvas.GetComponentInChildren<TextMeshProUGUI>();
        }
        
    }
    
    public void UpdateNickname(string nickname)
    {
        if (nicknameText != null)
        {
            nicknameText.text = nickname;
        }
    }

    //움직임
    public EventHandler OnMovementInput;

    public class OnMovementInputEventArgs: EventArgs{
        
        public int XDirection { get; private set; }
        public int YDirection { get; private set; }
        

        public OnMovementInputEventArgs(int inputXDirection, int inputYDirection)
        {
            XDirection = inputXDirection;
            YDirection = inputYDirection;
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
