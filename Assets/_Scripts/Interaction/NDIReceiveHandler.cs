using UnityEngine;
using Klak.Ndi; // NDI 기능
using Mirror;   // 역할 확인용
using System.Linq;
using System.Collections;

public class NDIReceiveHandler : MonoBehaviour
{
    private NdiReceiver ndiReceiver;
    private UnityEngine.UI.RawImage rawImage;

    void Start()
    {
        // 1. 내 오브젝트(RawImage)에 붙어있는 NdiReceiver를 가져옵니다.
        ndiReceiver = GetComponent<NdiReceiver>();
        rawImage = GetComponent<UnityEngine.UI.RawImage>();

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
            if (ndiReceiver != null)
            {
                // 1. 꺼져있으면 켜기
                if (!ndiReceiver.enabled)
                {
                    ndiReceiver.enabled = true;
                    Debug.Log("[NDI] 리시버 활성화됨. 소스 찾는 중...");
                }

                // 2. 이름(채널)이 비어있으면 계속 검색
                if (string.IsNullOrEmpty(ndiReceiver.ndiName))
                {
                    FindSource();
                }
            }
        }
    }

    void FindSource()
    {
        // 네트워크에 있는 모든 NDI 소스 검색
        var sources = NdiFinder.sourceNames;

        // 발견된 게 있다면?
        string foundName = sources.FirstOrDefault();

        if (!string.IsNullOrEmpty(foundName))
        {
            ndiReceiver.ndiName = foundName; // 채널 고정!
            Debug.Log($"[NDI] 방송 찾음! 연결 시도: {foundName}");

            // 혹시 텍스처 연결 안 됐을까 봐 강제 연결
            if (ndiReceiver.targetTexture == null && rawImage != null)
            {
                // 렌더 텍스처가 없으면 임시로 생성해서라도 보여줌
                // (하지만 팀장님이 STEP 1을 했다면 이 코드는 실행 안 됨)
            }
        }
    }
}