using UnityEngine;
using Unity.Netcode;
/// <summary>
/// 플레이어 생성 담당
/// </summary>

public class PlayerFactory : NetworkBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerInitialState;
    public Transform playerSpawnPoint;

    private ulong clientId;
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        // 클라이언트가 연결될 때 이벤트를 받아 clientId를 얻습니다.
        NetworkManager.Singleton.OnClientConnectedCallback += SetClientID;
        
    }

    private void SetClientID(ulong inputClientId)
    {
        clientId = inputClientId;
    }

    [Rpc(SendTo.Server)]
    public void SpawnPlayerServerRpc(string inputNickName="Player_", PlayerJob inputPlayerJob =PlayerJob.None)
    {
        // 플레이어 생성
        var player = Instantiate(playerPrefab, playerSpawnPoint);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
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

        //참조 이어주기: Presenter에게
        player.GetComponent<PlayerPresenter>().Initialize(player.GetComponent<PlayerModel>(),player.GetComponent<PlayerView>());
    
    
    }


}