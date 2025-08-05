using UnityEngine;
using Unity.Netcode;

public class PlayerPresenter : MonoBehaviour
{
    private PlayerModel playerModel;
    private PlayerView playerView;
    public void Initialize(PlayerModel playerModel, PlayerView playerView)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }
    }


}
