using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

// Presenter: View 입력 → Model 호출, 결과 ClientRpc 수신 → View 반영
public sealed class CardShopPresenter : NetworkBehaviour
{
    [SerializeField] private MonoBehaviour viewBehaviour; // CardShopView 할당
    private ICardShopView _view;
    private ICardShopModel _model;

    private static readonly Dictionary<ulong, CardShopPresenter> s_serverByClient = new();

    private void Awake()
    {
        _view = (ICardShopView)viewBehaviour;
        _model = new CardShopModel();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner && _view != null)
            _view.OnClickBuy += OnClickBuy;

        if (IsServer)
            s_serverByClient[OwnerClientId] = this;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner && _view != null)
            _view.OnClickBuy -= OnClickBuy;

        if (IsServer)
            s_serverByClient.Remove(OwnerClientId);
    }

    private void OnClickBuy(int cardId, ulong inputClientId, int cardPrice)
    {
        var clientId = inputClientId == 0UL ? OwnerClientId : inputClientId;

        _view.ShowLoading(true);
        TryPurchaseCard(cardId, clientId, cardPrice);
    }
    public void TryPurchaseCard(int cardID, ulong inputClientId, int cardPrice)
    {
        var clientId = inputClientId == 0UL ? OwnerClientId : inputClientId;
        _model.RequestPurchase(cardID, clientId, cardPrice);
        // 결과는 아래 ClientRpc로 받음
    }

    [ClientRpc]
    private void PurchaseClientRpc(bool success, ClientRpcParams sendParams = default)
    {
        if (_view == null) return;
        _view.ShowLoading(false);
        _view.ShowResult(success, success ? "구매 성공" : "구매 실패");
    }

    // 서버에서 특정 클라의 Presenter로 결과 보내는 헬퍼(DeckManager에서 호출하면 됨)
    public static void ServerSendResultTo(ulong clientId, bool success)
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;
        if (s_serverByClient.TryGetValue(clientId, out var presenter))
        {
            var p = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } };
            presenter.PurchaseClientRpc(success, p);
        }
    }
}
