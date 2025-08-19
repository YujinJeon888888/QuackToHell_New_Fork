using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전체를 관리하는 중앙 매니저
/// 
/// 책임:
/// - 게임 상태 및 씬 관리 (씬 전환, 게임 시작/종료)
/// - 플레이어 골드 관리 (차감, 증가, 검증)
/// - 게임 규칙 및 밸런스 관리
/// - 전역 이벤트 및 시스템 간 조율
/// - 게임 데이터 저장/로드 관리
/// 
/// 주의: 플레이어 개별 데이터는 PlayerManager를 통해 접근
/// </summary>
public class GameManager : NetworkBehaviour
{

    //싱글톤 코드
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //persistent씬에서 시작해서 바로 로비씬으로 전환
        SceneManager.LoadScene("LobbyScene");
    }

    /// <summary>
    /// 서버에서 특정 클라이언트의 골드를 차감하는 RPC
    /// </summary>
    /// <param name="clientId">골드를 차감할 클라이언트 ID</param>
    /// <param name="amount">차감할 골드 양</param>
    [ServerRpc]
    public void DeductPlayerGoldServerRpc(ulong clientId, int amount)
    {
        // 서버에서만 실행
        if (IsHost)
        {
            // NetworkManager로 정확한 클라이언트의 PlayerObject 찾기
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                var player = client.PlayerObject.GetComponent<PlayerModel>();
                if (player != null)
                {
                    // 해당 클라이언트의 골드 차감
                    var newStatusData = player.PlayerStatusData.Value;
                    newStatusData.Gold -= amount;
                    player.PlayerStatusData.Value = newStatusData;
                    
                    Debug.Log($"Player {clientId} gold deducted by {amount}. Remaining gold: {newStatusData.Gold}");
                }
                else
                {
                    Debug.LogError($"PlayerModel component not found on PlayerObject for ClientId {clientId}");
                }
            }
            else
            {
                Debug.LogError($"Client {clientId} not found in NetworkManager");
            }
        }
    }
}
