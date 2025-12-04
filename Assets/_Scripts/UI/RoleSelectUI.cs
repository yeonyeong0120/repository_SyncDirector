using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // ? 추가

public class RoleSelectUI : MonoBehaviour
{
    [Header("UI References")]
    public Button expertButton;
    public Button workerButton;
    public TextMeshProUGUI descriptionText;

    [Header("설명 텍스트")]
    public string expertDescription = "전문가는 원격에서 작업자를 지원합니다.\n? 매뉴얼 제공\n? 부품 선택 → 하이라이트";
    public string workerDescription = "작업자는 현장에서 전문가의 도움을 받습니다.\n? 현장 영상 공유\n? 지시 수신";

    void Start()
    {
        expertButton.onClick.AddListener(OnExpertSelected);
        workerButton.onClick.AddListener(OnWorkerSelected);

        descriptionText.text = "역할을 선택하세요";
    }

    void OnExpertSelected()
    {
        Debug.Log("[UI] 전문가 역할 선택");

        if (CustomNetworkManager.Instance != null)
        {
            CustomNetworkManager.Instance.StartAsExpert();
        }

        SceneController.Instance.GoToCollaboration();
    }

    void OnWorkerSelected()
    {
        Debug.Log("[UI] 작업자 역할 선택");
        SceneController.Instance.GoToConnection();
    }
}