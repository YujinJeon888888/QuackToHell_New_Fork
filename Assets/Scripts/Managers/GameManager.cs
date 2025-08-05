using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //현재 플레이어 수
    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0);
    public int GetNextPlayerNumber()
    {
        return playerCount.Value++;
    }

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
}
