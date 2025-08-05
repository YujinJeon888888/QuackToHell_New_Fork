// PlayerModel.cs
using System;
using Unity.Netcode;
/// <summary>
/// 로직 관리
/// </summary>
public class PlayerModel : NetworkBehaviour
{
    //속성값
    private PlayerStatusData _playerStatusData;
    public PlayerStatusData PlayerStatusData
    {
        get { return _playerStatusData; }
        set { _playerStatusData = value; }
    }


    //상태에 따라 행동
    //상태 주입: State를 상속받은 클래스의 인스턴스를 주입받음
    State _preState;
    State _tempState;
    State _curState;

    public void SetState(State state)
    {
        _tempState = _curState;
        _curState = state;
        _preState = _tempState;
    }
    public void Action()
    {
        if (_preState != null)
        {
            _preState.OnStateExit();
        }
        _curState.OnStateEnter();
    }
}