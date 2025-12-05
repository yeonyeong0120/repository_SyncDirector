using UnityEngine;
using TMPro;

public class ExpertWaitingUI : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject waitingPanel;       // 대기 문구가 포함된 패널 (또는 텍스트 오브젝트)
    public TextMeshProUGUI statusText;    // 문구를 표시할 텍스트

    void Start()
    {
        // 1. 내가 전문가인지 확인
        if (CustomNetworkManager.Instance == null) return;

        if (CustomNetworkManager.Instance.myRole == UserRole.Expert)
        {
            // 전문가라면 대기 패널 보여주기
            if (waitingPanel != null) waitingPanel.SetActive(true);
            UpdateText("작업자 접속 대기 중...", Color.darkGray);

            // 2. 작업자가 들어왔을 때 실행될 이벤트 연결
            CustomNetworkManager.OnWorkerConnected += HandleWorkerConnected;
        }
        else
        {
            // 작업자라면 이 패널을 아예 숨김
            if (waitingPanel != null) waitingPanel.SetActive(false);
            Debug.Log("Worker");
        }
    }

    void OnDestroy()
    {
        // 이벤트 연결 해제 (메모리 누수 방지)
        CustomNetworkManager.OnWorkerConnected -= HandleWorkerConnected;
    }

    // 작업자가 접속했을 때 실행되는 함수
    void HandleWorkerConnected()
    {
        UpdateText("작업자 접속 완료! (협업 시작)", Color.green);

        // 3초 뒤에 패널 숨기기
        Invoke(nameof(HidePanel), 3.0f);
    }

    void HidePanel()
    {
        if (waitingPanel != null) waitingPanel.SetActive(false);
    }

    void UpdateText(string msg, Color color)
    {
        if (statusText != null)
        {
            statusText.text = msg;
            statusText.color = color;
        }
    }
}