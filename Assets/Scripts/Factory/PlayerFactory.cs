using UnityEngine;
using Unity.Netcode;
/// <summary>
/// 플레이어 생성 담당
/// </summary>

public class PlayerFactory : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerInitialState;
    public Transform playerSpawnPoint;

    //TODO: 초기값으로 플레이어 생성
    public void CreatePlayer(string inputNickName="Player_", PlayerJob inputPlayerJob =PlayerJob.None)
    {
        // 플레이어 생성
        var player = Instantiate(playerPrefab, playerSpawnPoint);

        // 플레이어 데이터 초기화
        PlayerStatusData playerStatusData = new PlayerStatusData
        {
            Nickname = inputNickName+GameManager.Instance.GetNextPlayerNumber(),
            Job = inputPlayerJob,
            Credibility = PlayerStatusData.MaxCredibility,
            Spellpower = PlayerStatusData.MaxSpellpower,
            Gold = 0,
            MoveSpeed = 5f // 기본 이동 속도 설정
        };

        //속성 세팅
        player.GetComponent<PlayerModel>().PlayerStatusData=playerStatusData;
        //상태 세팅
        player.GetComponent<PlayerModel>().SetState(Instantiate(playerInitialState).GetComponent<PlayerIdleState>());
    }
}