using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoleSelectUI : MonoBehaviour
{
    [Header("메인 UI")]
    public Button expertButton;      // 전문가 버튼
    public Button workerButton;      // 작업자 버튼
    public TextMeshProUGUI statusText; // 상태 메시지

    void Start()
    {
        // 1. 버튼 기능 연결
        expertButton.onClick.AddListener(OnExpertClick);
        workerButton.onClick.AddListener(OnWorkerClick);

        statusText.text = "역할을 선택하세요.";

        // 2. 연결 성공하면 텍스트 바꿔주기 (초록색)
        if (ConnectionStateManager.Instance != null)
        {
            ConnectionStateManager.Instance.OnConnected.AddListener(() => {
                statusText.text = "연결 성공! 공장으로 이동합니다...";
                statusText.color = Color.green;
            });
        }
    }

    // --- 전문가(Host) 버튼 눌렀을 때 ---
    void OnExpertClick()
    {
        statusText.text = "전문가 모드 시작... (방송 중)";
        statusText.color = Color.yellow;

        // 버튼 두 번 못 누르게 숨김
        expertButton.gameObject.SetActive(false);
        workerButton.gameObject.SetActive(false);

        // 1. 호스트 시작 (NetworkManager가 알아서 03씬으로 이동시킴)
        CustomNetworkManager.Instance.StartAsExpert();

        // 2. "나 여기 있어!" 방송 시작 (중요)
        if (UDPDiscovery.Instance != null)
        {
            UDPDiscovery.Instance.StartBroadcasting();
        }
    }

    // --- 작업자(Worker) 버튼 눌렀을 때 ---
    void OnWorkerClick()
    {
        // ★★★ [수정됨] 메뉴 안 띄우고 바로 검색 시작! ★★★
        statusText.text = "전문가 찾는 중... (자동 검색)";
        statusText.color = Color.yellow;

        // 버튼 숨김
        expertButton.gameObject.SetActive(false);
        workerButton.gameObject.SetActive(false);

        // 1. 바로 듣기 모드 시작 (찾으면 알아서 연결함)
        if (UDPDiscovery.Instance != null)
        {
            UDPDiscovery.Instance.StartListening();
        }
        else
        {
            statusText.text = "오류: UDPDiscovery가 없습니다.";
            statusText.color = Color.red;
        }
    }
}