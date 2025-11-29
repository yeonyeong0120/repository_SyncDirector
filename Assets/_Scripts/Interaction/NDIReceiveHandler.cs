using UnityEngine;
using Klak.Ndi; // NDI 기능
using Mirror;   // 역할 확인용

public class NDIReceiveHandler : MonoBehaviour
{
    private NdiReceiver ndiReceiver;

    void Start()
    {
        // 1. 내 오브젝트(RawImage)에 붙어있는 NdiReceiver를 가져옵니다.
        ndiReceiver = GetComponent<NdiReceiver>();

        // 2. 일단 끕니다 (작업자는 볼 필요가 없으니까요)
        if (ndiReceiver != null)
        {
            ndiReceiver.enabled = false;
        }
    }

    void Update()
    {
        if (CustomNetworkManager.Instance == null) return;

        // 3. 내 역할이 'Expert(전문가)'인지 확인 [가이드 126번 항목]
        if (CustomNetworkManager.Instance.myRole == UserRole.Expert)
        {
            // NDI 리시버가 꺼져있다면 켭니다.
            if (ndiReceiver != null && !ndiReceiver.enabled)
            {
                ndiReceiver.enabled = true;
                Debug.Log("[NDI] 전문가(Expert) 모드 감지: 수신 시작");
            }
        }
    }
}