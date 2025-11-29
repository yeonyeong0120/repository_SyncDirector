using UnityEngine;
using Klak.Ndi; // NDI 기능을 쓰기 위해 필수
using Mirror;   // CustomNetworkManager를 알기 위해 필요

public class NDIStreamHandler : MonoBehaviour
{
    private NdiSender ndiSender;

    void Start()
    {
        // 1. 내 오브젝트에 붙어있는 NdiSender 컴포넌트를 가져옵니다.
        ndiSender = GetComponent<NdiSender>();

        // 2. 시작할 때는 무조건 꺼둡니다. (전문가일 수도 있으니까요)
        if (ndiSender != null)
        {
            ndiSender.enabled = false;
        }
    }

    void Update()
    {
        // 3. CustomNetworkManager가 존재하는지 확인
        if (CustomNetworkManager.Instance == null) return;

        // 4. 내 역할이 'Worker(작업자)'인지 확인 [가이드 요구사항]
        if (CustomNetworkManager.Instance.myRole == UserRole.Worker)
        {
            // NDI가 꺼져있다면 켭니다.
            if (ndiSender != null && !ndiSender.enabled)
            {
                ndiSender.enabled = true;
                Debug.Log("[NDI] 작업자(Worker) 모드 감지: 송출 시작");
            }
        }
    }
}