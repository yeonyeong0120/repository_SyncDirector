using UnityEngine;

public class RoleLayoutManager : MonoBehaviour
{
    [Header("그룹별 오브젝트 (통째로 묶어서 넣으세요)")]
    public GameObject expertGroup; // 전문가용 UI/오브젝트 모음
    public GameObject workerGroup; // 작업자용 UI/오브젝트 모음

    void Start()
    {
        // 1. 내 역할 확인 (CustomNetworkManager에게 물어봄)
        if (CustomNetworkManager.Instance == null)
        {
            Debug.LogError("NetworkManager가 없습니다! 역할을 확인할 수 없습니다.");
            return;
        }

        UserRole myRole = CustomNetworkManager.Instance.myRole;
        Debug.Log($"[RoleLayout] 현재 역할에 맞춰 화면을 세팅합니다: {myRole}");

        // 2. 역할에 따라 그룹 껐다 켜기
        if (myRole == UserRole.Expert)
        {
            // 전문가는 컨트롤 패널을 봐야 함
            if (expertGroup != null) expertGroup.SetActive(true);
            if (workerGroup != null) workerGroup.SetActive(false);

            // ▼▼▼ [추가] 전문가일 때 방송(Broadcasting)을 시작하는 코드 추가 ▼▼▼
            if (UDPDiscovery.Instance != null)
            {
                UDPDiscovery.Instance.StartBroadcasting();
                Debug.Log("전문가: 자동 검색 브로드캐스트 시작됨!");
            }
        }
        else if (myRole == UserRole.Worker)
        {
            // 작업자는 작업에 집중 (패널 숨김)
            if (expertGroup != null) expertGroup.SetActive(false);
            if (workerGroup != null) workerGroup.SetActive(true);
        }
    }
}