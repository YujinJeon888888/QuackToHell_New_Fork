using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Threading.Tasks;
using System.Collections.Generic;

public class LobbyController : NetworkBehaviour
{
    #region 카드데이터 로드

    private bool isCardDataLoaded = false;

    public override async void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //호스트만 데이터 로드
        if (!IsHost)
        {
            return;
        }

        // CardDataView가 초기화될 때까지 대기
        while (CardDataView.Presenter == null)
        {
            await Task.Yield();
        }

        // 데이터 로딩 완료까지 대기
        await CardDataView.Presenter.WhenReadyAsync();

        // 이제 안전하게 데이터 사용
        Debug.Log($"로드된 카드 수: {CardDataView.Presenter.CardCount}");

        foreach (var value in CardDataView.Presenter.Cards)
        {
            Debug.Log($"{value.Value.CardID}의 총량: {value.Value.AmountOfCardItem}");
        }
    }
 
    #endregion

    #region 게임 버튼

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
        //호스트만 게임 시작 가능
        if (!IsOwner)
        {
            Debug.LogError("Only the owner can start the game!");
            return;
        }

        // 2명 미만이면 시작 못 함
        if(NetworkManager.Singleton.ConnectedClientsList.Count < 2)
        {
            return;
        }

        if (IsHost)
        {
            //호스트가 클라이언트에게 데이터 전달: 멀티캐스트
            IReadOnlyDictionary<int, CardDef> cardData = CardDataView.Presenter.Cards;
            //직렬화 가능 타입으로 변환
            CardKeyValuePair[] cardKeyValuePairs = new CardKeyValuePair[cardData.Count];
            int index = 0;
            foreach (var card in cardData)
            {
                cardKeyValuePairs[index] = new CardKeyValuePair { Key = card.Key, Value = card.Value };
                index++;
            }
            LoadCardDataClientRpc(cardKeyValuePairs);
        }

       
        //본인 데이터가 모두 초기화되면, 씬 이동.
        LoadVillageSceneServerRpc();
    }

    [ClientRpc]
    private void LoadCardDataClientRpc( CardKeyValuePair[] cardKeyValuePairs)
    {
        LoadCardData(cardKeyValuePairs);
    }

    private async void LoadCardData( CardKeyValuePair[] cardKeyValuePairs)
    {
        //DeckManager에게 데이터 전달
        await DeckManager.Instance.SetTotalCardsOnGame(cardKeyValuePairs);
        //CardItemFactory에게 데이터 전달
        await CardItemFactory.Instance.SetCardData(cardKeyValuePairs);

        isCardDataLoaded = true;
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
    #endregion
}
