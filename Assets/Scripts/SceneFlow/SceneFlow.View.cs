using UnityEngine;
using UnityEngine.UI;

/// 하나로 통합된 View 컴포넌트:
/// Host/Client UI, 씬 전환 버튼, Boot 자동 점프
/// 인스펙터의 Mode로 용도를 전환합니다.

public class QuickNetView : MonoBehaviour
{
    public enum Mode { HostClientUI, SceneSwitchButton, BootAutoJump }

    [Header("공통")]
    [SerializeField] Mode mode = Mode.HostClientUI;

    [Header("Host/Client UI")]
    [SerializeField] string firstScene = SceneNames.Village;
    [SerializeField] string address = "127.0.0.1";
    [SerializeField] ushort port = 7777;

    [Header("Scene Switch / BootAutoJump 공용")]
    [SerializeField] string sceneName = SceneNames.Village;

    Button _btn;

    QuickNetPresenter _net;
    SceneSwitchPresenter _switcher;
    BootLoaderPresenter _boot;

    void Awake()
    {
        var svc = new NgoSceneService();
        _net = new QuickNetPresenter(svc);
        _switcher = new SceneSwitchPresenter(svc);
        _boot = new BootLoaderPresenter(svc);

        // 버튼에 붙여 사용하는 경우(씬 전환 버튼)
        _btn = GetComponent<Button>();
        if (mode == Mode.SceneSwitchButton && _btn)
        {
            _btn.onClick.AddListener(OnClickSwitch);
            _btn.interactable = false; // 준비되면 Update에서 활성
        }
    }

    void Start()
    {
        // Boot 자동 점프 모드
        if (mode == Mode.BootAutoJump) StartCoroutine(CoBootJump());
    }

    System.Collections.IEnumerator CoBootJump()
    {
        yield return null;               // 싱글톤 Awake 보장
        _boot.Jump(sceneName);           // sceneName을 nextScene 용도로 사용
    }

    void Update()
    {
        // NetSceneController가 Spawn되기 전 버튼 잠금
        if (mode == Mode.SceneSwitchButton && _btn && !_btn.interactable && _switcher.CanSwitch())
            _btn.interactable = true;
    }

    // 버튼 OnClick 바인딩용 공개 메서드
    public void OnClickHost() => _net.StartHost(address, port, firstScene);
    public void OnClickClient() => _net.StartClient(address, port);
    public void OnClickShutdown() => _net.Shutdown();

    // 씬 전환 버튼(Mode = SceneSwitchButton)
    public void OnClickSwitch()
    {
        if (_switcher.CanSwitch()) _switcher.RequestSwitch(sceneName);
    }
}
