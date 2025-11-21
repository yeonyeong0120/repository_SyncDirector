using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkTestUI : MonoBehaviour
{
    [Header("UI References")]
    public Button expertButton;
    public Button workerButton;
    public TMP_InputField ipInputField;
    public Button connectButton;

    // 자동 검색 버튼 변수 추가
    public Button autoDiscoverButton;

    public TextMeshProUGUI statusText;
    public GameObject workerPanel;

    void Start()
    {
        // 초기 상태 설정
        workerPanel.SetActive(false);

        // 버튼 이벤트 연결
        expertButton.onClick.AddListener(OnExpertButtonClick);
        workerButton.onClick.AddListener(OnWorkerButtonClick);
        connectButton.onClick.AddListener(OnConnectButtonClick);

        // 자동 검색 버튼 리스너 연결
        // (혹시 Inspector에서 연결 안 했을 때 에러 안 나게 null 체크)
        if (autoDiscoverButton != null)
        {
            autoDiscoverButton.onClick.AddListener(OnAutoDiscoverClick);
        }

        statusText.text = "역할을 선택하세요";
    }

    // 전문가 버튼 클릭 시
    void OnExpertButtonClick()
    {
        CustomNetworkManager.Instance.StartAsExpert();

        // 전문가도 "나 여기 있어!"라고 방송(Broadcasting) 시작해야 함
        UDPDiscovery.Instance.StartBroadcasting();

        statusText.text = $"전문가 모드\n내 IP: {CustomNetworkManager.Instance.hostIP}\n작업자 연결 대기 중...";

        expertButton.gameObject.SetActive(false);
        workerButton.gameObject.SetActive(false);
        // 자동 검색 버튼도 숨김
        if (autoDiscoverButton != null) autoDiscoverButton.gameObject.SetActive(false);
    }

    // 작업자 버튼 클릭 시
    void OnWorkerButtonClick()
    {
        workerPanel.SetActive(true);
        statusText.text = "전문가 IP를 입력하거나\n자동 검색을 누르세요";

        expertButton.gameObject.SetActive(false);
        workerButton.gameObject.SetActive(false);
    }

    // 연결(Connect) 버튼 클릭 시 (수동 입력)
    void OnConnectButtonClick()
    {
        string ip = ipInputField.text;
        if (string.IsNullOrEmpty(ip))
        {
            statusText.text = "IP 주소를 입력하세요!";
            return;
        }

        CustomNetworkManager.Instance.StartAsWorker(ip);
        statusText.text = $"연결 시도 중: {ip}...";
    }

    // 자동 검색 버튼 클릭 시 실행될 함수
    void OnAutoDiscoverClick()
    {
        statusText.text = "자동 검색 중... (전문가 찾는 중)";

        // 전문가를 찾기 위해 리스닝(듣기) 시작!
        UDPDiscovery.Instance.StartListening();
    }

    void Update()
    {
        // 연결 상태 실시간 업데이트
        if (CustomNetworkManager.Instance.myRole == UserRole.Expert)
        {
            if (Mirror.NetworkServer.connections.Count > 1)
            {
                statusText.text = "작업자 연결됨!";
            }
        }
        else if (CustomNetworkManager.Instance.myRole == UserRole.Worker)
        {
            if (Mirror.NetworkClient.isConnected)
            {
                statusText.text = "전문가에 연결됨!";
            }
        }
    }
}