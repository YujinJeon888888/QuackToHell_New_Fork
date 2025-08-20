using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class CardInventoryPresenter : MonoBehaviour
{
    private CardInventoryModel _cardInventoryModel;
    private CardInventoryView _cardInventoryView;

    private void Awake()
    {
        _cardInventoryModel = PlayerHelperManager.Instance.GetPlayerGameObjectByClientId(NetworkManager.Singleton.LocalClientId).GetComponent<CardInventoryModel>();
        _cardInventoryView = gameObject.GetComponent<CardInventoryView>();
    }

    private void Start()
    {
        _cardInventoryModel.OwnedCards.OnValueChanged += CardInventoryModel_OwnedCardsOnValueChanged;
        //초기 뷰 업데이트
        _cardInventoryView.UpdateInventoryView(_cardInventoryModel.OwnedCards.Value);
        //TODO: 정렬기능 추가되면, 정렬 enum에 따라 다른 정렬 함수 호출
        switch (_cardInventoryModel.SortingOption)
        {
            case InventorySotringOption.RecentlyAcquired:
                _cardInventoryModel.SortCardsByAcquiredTicks();
                break;
            default:
                break;
        }
    }

    private void CardInventoryModel_OwnedCardsOnValueChanged(List<InventoryCard> previousValue, List<InventoryCard> newValue)
    {
        //view 업데이트 함수 호출
        CardInventoryView cardInventoryView = gameObject.GetComponent<CardInventoryView>();
        cardInventoryView.UpdateInventoryView(newValue);
        //TODO: 정렬기능 추가되면, 정렬 enum에 따라 다른 정렬 함수 호출
        switch (_cardInventoryModel.SortingOption)
        {
            case InventorySotringOption.RecentlyAcquired:
                _cardInventoryModel.SortCardsByAcquiredTicks();
                break;
            default:
                break;
        }
    }
}
