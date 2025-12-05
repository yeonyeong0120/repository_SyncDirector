using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionUI : MonoBehaviour
{
    [Header("UI 요소")]
    public TMP_InputField ipInput;      // IP 입력창
    public Button manualConnectBtn;     // 수동 연결 버튼
    public Button autoDiscoverBtn;      // 자동 검색 버튼
    public TextMeshProUGUI statusText;  // 상태 텍스트

    [Header("추가 설정")]
    public float discoveryTimeout = 10.0f; // 타임아웃 관련

    void Start()
    {
        statusText.text = "";
        statusText.color = Color.darkGray;
        // 작업자가 들어오면 실행될 코드 (이벤트 구독)
        CustomNetworkManager.OnWorkerConnected += () => {
            statusText.text = "작업자 접속 완료! (협업 시작)";
            statusText.color = Color.green;

            // 3초 뒤에 텍스트 끄기 (선택)
            Invoke(nameof(HideText), 3f);
        };

        // 버튼에 기능 연결
        manualConnectBtn.onClick.AddListener(OnManualConnect);
        autoDiscoverBtn.onClick.AddListener(OnAutoDiscover);

        // 팀장님의 상태 매니저(ConnectionStateManager) 이벤트 구독
        if (ConnectionStateManager.Instance != null)
        {
            // 연결 성공하면 -> OnConnectionSuccess 함수 실행
            ConnectionStateManager.Instance.OnConnected.AddListener(OnConnectionSuccess);

            // 연결 실패하면 -> 빨간 글씨로 상태 업데이트
            ConnectionStateManager.Instance.OnConnectionFailed.AddListener(() => UpdateStatus("연결 실패! 다시 시도하세요.", Color.red));
        }
    }

    // [수동 연결] 버튼 눌렀을 때
    void OnManualConnect()
    {
        string ip = ipInput.text;
        if (string.IsNullOrEmpty(ip))
        {
            UpdateStatus("IP 주소를 입력해주세요!", Color.red);
            return;
        }

        // 팀장님의 네트워크 매니저로 연결 시도
        CustomNetworkManager.Instance.StartAsWorker(ip);
        UpdateStatus($"연결 시도 중... ({ip})", Color.yellow);
    }

    // [자동 검색] 버튼 눌렀을 때 (핵심 기능!)
    void OnAutoDiscover()
    {
        UpdateStatus("자동 검색 중... (전문가 찾는 중)", Color.yellow);

        // 팀장님의 UDPDiscovery 실행 (듣기 모드)
        if (UDPDiscovery.Instance != null)
        {
            UDPDiscovery.Instance.StartListening();
            Invoke(nameof(OnDiscoveryTimeout), discoveryTimeout);
        }    
        else
        {
            Debug.LogError("UDPDiscovery가 없습니다! NetworkManager 프리팹을 확인하세요.");
        }
    }

    


    // 연결 성공 시 실행됨
    void OnConnectionSuccess()
    {
        UpdateStatus("연결 성공! 공장으로 이동합니다.", Color.green);

        // 1초 뒤에 협업 씬(03_Collaboration)으로 이동
        Invoke(nameof(MoveToCollaboration), 1.0f);
    }

    void MoveToCollaboration()
    {
        // 씬 이동
        SceneController.Instance.GoToCollaboration();
    }

    // 텍스트 상태 변경용 함수
    void UpdateStatus(string msg, Color color)
    {
        if (statusText != null)
        {
            statusText.text = msg;
            statusText.color = color;
        }
    }

    void OnDiscoveryTimeout()
    {
        // 아직 연결이 안 됐다면? (역할이 안정해졌다면)
        if (CustomNetworkManager.Instance.myRole == UserRole.None)
        {
            // 검색 중단
            if(UDPDiscovery.Instance != null) UDPDiscovery.Instance.StopDiscovery();
            
            UpdateStatus("검색 실패! (전문가를 못 찾음)", Color.red);
        }
    }

    void HideText()
    {
        if (statusText != null)
        {
            statusText.text = ""; // 텍스트 내용을 비웁니다.
        }
    }
}// 클래스