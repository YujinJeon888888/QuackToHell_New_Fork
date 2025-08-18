using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 덱 매니저: 게임 내 전체 카드 매물을 관리하는 매니저
/// 
/// 책임:
/// - 게임 내 전체 카드 매물 관리 (수량, 가격, 재고)
/// - 카드 구매/판매 처리 및 검증
/// - 게임 내 존재하는 카드 데이터 조회 및 제공
/// 
/// 주의: 
/// - 골드 차감은 GameManager에 위임
/// - 실제 카드 데이터는 TotalCardsOnGame에서 카드 키 조회하여 가져와 사용하기
/// </summary>
public class DeckManager : NetworkBehaviour
{
    #region 싱글톤 코드
    //싱글톤 코드
    private static DeckManager _instance;
    public static DeckManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DeckManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DeckManager");
                    _instance = go.AddComponent<DeckManager>();
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
    #endregion

    #region 게임 내 전체 카드 매물 관리(덱 관리)
    /// <summary>
    /// 게임 내 전체 카드 매물 관리 (NetworkVariable로 동기화)
    /// </summary>
    /// <remarks>
    /// <para><strong>Key:</strong> CardID (카드 식별자)</para>
    /// <para><strong>Value:</strong> Count (해당 카드의 개수)</para>
    /// <para><strong>동기화:</strong> 모든 클라이언트에 자동 동기화</para>
    /// <para><strong>권한:</strong> 서버에서만 수정 가능</para>
    /// </remarks>
    private NetworkVariable<Dictionary<int, int>> _totalCardsOnGame = new NetworkVariable<Dictionary<int, int>>(
        new Dictionary<int, int>(),
        writePerm: NetworkVariableWritePermission.Server
    );
    
    public Dictionary<int, int> TotalCardsOnGame
    {
        get { return _totalCardsOnGame.Value; }
    }




    [ServerRpc]
    public void TryPurchaseCardServerRpc(int cardID, ulong clientId, int cardPrice)
    {
        //만약 물량 없으면 리턴 
        if (TotalCardsOnGame[cardID] <= 0)
        {
            // TODO: 구매 성공 여부를 CardShop에게 전달. (ClientRPC, bool값 보내기)
            // cardShop.PurchaseCardResultClientRpc(false);
            return;
        }

        // 플레이어 골드 확인
        int playerGold = PlayerHelperManager.Instance.GetPlayerGoldByClientId(clientId);
        if (playerGold < cardPrice)
        {
            // TODO: 구매 성공 여부를 CardShop에게 전달. (ClientRPC, bool값 보내기)
            // cardShop.PurchaseCardResultClientRpc(false);
            return;
        }

        #region  카드 구매 처리 
        // 물량 감소 (NetworkVariable 동기화)
        var newTotalCards = new Dictionary<int, int>(TotalCardsOnGame);
        newTotalCards[cardID]--;
        _totalCardsOnGame.Value = newTotalCards;

        // GameManager에게 해당 클라이언트의 골드 차감 요청 (책임 분리)
        GameManager.Instance.DeductPlayerGoldServerRpc(clientId, cardPrice);

        Debug.Log($"Player {clientId} purchased card {cardID} for {cardPrice} gold");

        // TODO: 플레이어 인벤토리에 카드 추가

        // TODO: 구매 성공 여부를 CardShop에게 전달. (ClientRPC, bool값 보내기)
        // cardShop.PurchaseCardResultClientRpc(true);

        // TODO: 카드 상태 주입 (Sold)
        
        #endregion
    }


    #endregion
}
