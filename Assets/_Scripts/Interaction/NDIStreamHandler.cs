using UnityEngine;
using Klak.Ndi; // NDI 기능을 쓰기 위해 필수
using Mirror;   // CustomNetworkManager를 알기 위해 필요

public class NDIStreamHandler : MonoBehaviour
{
    private NdiSender ndiSender;

    // [수정] Inspector에서 직접 송출용 카메라를 연결할 수 있게 필드를 만듭니다.
    [Header("NDI 송출 설정")]
    public Camera ndiCamera;

    void Start()
    {
        // 내 오브젝트에 붙어있는 NdiSender를 가져옵니다.
        ndiSender = GetComponent<NdiSender>();

        // 1. 시작 시에는 리소스 절약을 위해 무조건 꺼둡니다.
        if (ndiSender != null)
        {
            ndiSender.enabled = false;
        }

        if (ndiCamera != null)
        {
            ndiCamera.enabled = false;
        }
    }

    void Update()
    {
        if (CustomNetworkManager.Instance == null) return;

        // 2. 내 역할이 'Worker'인지 확인합니다.
        if (CustomNetworkManager.Instance.myRole == UserRole.Worker)
        {
            // 아직 꺼져있다면 카메라와 송신기를 활성화합니다.
            if (ndiSender != null && !ndiSender.enabled)
            {
                // [중요] 카메라를 먼저 켜서 렌더 텍스처를 그리게 한 뒤 송신기를 켭니다.
                if (ndiCamera != null)
                {
                    ndiCamera.enabled = true;
                }

                ndiSender.enabled = true;
                Debug.Log("[NDI] 작업자 모드: 시점 송출을 시작합니다.");
            }
        }
        else
        {
            // 전문가(Expert)라면 송출 관련 기능을 확실히 꺼서 성능을 확보합니다.
            if (ndiSender != null && ndiSender.enabled)
            {
                ndiSender.enabled = false;
                if (ndiCamera != null)
                {
                    ndiCamera.enabled = false;
                }
            }
        }
    }

    // space
}