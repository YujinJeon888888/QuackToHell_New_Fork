using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

//TODO: 나중에 수정하기
public class LobbyController : NetworkBehaviour
{
    public void OnJoinAsClientButton()
    {
        // 네트워크 연결 완료 이벤트 구독
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        
        // 클라이언트로 세션에 참여
        NetworkManager.Singleton.StartClient();
    }

    public void OnJoinAsHostButton()
    {
        // 네트워크 연결 완료 이벤트 구독
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        
        // 호스트(서버+클라이언트)로 세션 생성 및 참여
        NetworkManager.Singleton.StartHost();
    }
    
    public void OnStartGameButton()
    {
        if (!IsOwner)//default 오너: 호스트
        {
            Debug.LogError("Only the owner can start the game!");
            return;
        }

        //클라이언트가 2명 이상이면, 마을 씬 이동: invoke를 3초뒤에
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            LoadVillageSceneServerRpc();
        }
    }

    private void OnClientConnected(ulong clientId)
    {

        // 자신의 클라이언트가 연결되었을 때만 플레이어 스폰
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // 이벤트 구독 해제 (한 번만 실행되도록)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

            PlayerSpawn();
        }


    }
    
    [ServerRpc]
    private void LoadVillageSceneServerRpc()
    {
        // 모든 클라이언트를 VillageScene으로 이동
        NetworkManager.Singleton.SceneManager.LoadScene("VillageScene", LoadSceneMode.Single);
    }

    private void PlayerSpawn()
    {
        PlayerFactory playerFactory = FindObjectOfType<PlayerFactory>();
        if (playerFactory != null)
        {
            playerFactory.SpawnPlayerServerRpc();
        }
        else
        {
            Debug.LogError("PlayerFactory not found in the scene.");
        }
    }
}
