using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkTestUI : MonoBehaviour
{
    [Header("UI References")]
    public Button expertButton;
    public Button workerButton;
    public TMP_InputField ipInputField; // 일반 InputField가 아니라 TMP용입니다
    public Button connectButton;
    public TextMeshProUGUI statusText; // 일반 Text가 아니라 TMP용입니다
    public GameObject workerPanel;

    void Start()
    {
        // 초기 상태 설정
        workerPanel.SetActive(false);

        // 버튼 이벤트 연결 (클릭 시 실행될 함수 지정)
        expertButton.onClick.AddListener(OnExpertButtonClick);
        workerButton.onClick.AddListener(OnWorkerButtonClick);
        connectButton.onClick.AddListener(OnConnectButtonClick);

        statusText.text = "역할을 선택하세요";
    }

    // 전문가 버튼 클릭 시
    void OnExpertButtonClick()
    {
        // 아까 만든 매니저를 호출!
        CustomNetworkManager.Instance.StartAsExpert();

        statusText.text = $"전문가 모드\n내 IP: {CustomNetworkManager.Instance.hostIP}\n작업자 연결 대기 중...";

        // 버튼 숨기기
        expertButton.gameObject.SetActive(false);
        workerButton.gameObject.SetActive(false);
    }

    // 작업자 버튼 클릭 시
    void OnWorkerButtonClick()
    {
        // IP 입력 패널 표시
        workerPanel.SetActive(true);
        statusText.text = "전문가 IP를 입력하세요";

        // 버튼 숨기기
        expertButton.gameObject.SetActive(false);
        workerButton.gameObject.SetActive(false);
    }

    // 연결(Connect) 버튼 클릭 시
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

    void Update()
    {
        // 연결 상태 실시간 업데이트 확인
        if (CustomNetworkManager.Instance.myRole == UserRole.Expert)
        {
            // 서버에 연결된 사람이 1명보다 많으면 (나 + 상대방)
            if (Mirror.NetworkServer.connections.Count > 1)
            {
                statusText.text = "작업자 연결됨!";
            }
        }
        else if (CustomNetworkManager.Instance.myRole == UserRole.Worker)
        {
            // 클라이언트로서 연결이 되었는지 확인
            if (Mirror.NetworkClient.isConnected)
            {
                statusText.text = "전문가에 연결됨!";
            }
        }
    }


}  // 클래스
