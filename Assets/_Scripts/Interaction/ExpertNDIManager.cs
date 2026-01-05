using UnityEngine;
using Klak.Ndi;
using Mirror;
using System.Collections;
using System.Linq;

public class ExpertNDIManager : NetworkBehaviour
{
    [Header("연결 설정")]
    public GameObject monitorObject; // 영상을 띄울 Quad
    public NdiReceiver ndiReceiver;  // NDI 리시버 컴포넌트

    // 이전에 약속한 고정 스트림 이름
    private string targetStreamName = "SyncDirector_Worker_Stream";

    void Start()
    {
        // 씬 시작 시 역할에 따른 초기화 실행
        StartCoroutine(InitializeExpertScreen());
    }

    IEnumerator InitializeExpertScreen()
    {
        // 네트워크 상태가 안정될 때까지 대기
        yield return new WaitForSeconds(0.5f);

        // 1. 내가 전문가(Host)인지 확인
        if (CustomNetworkManager.Instance.myRole == UserRole.Expert)
        {
            Debug.Log("[NDI] 전문가 모드: 수신 시스템 활성화");

            if (monitorObject != null) monitorObject.SetActive(true);

            if (ndiReceiver != null)
            {
                ndiReceiver.enabled = true;
                // [수정] 소스 찾기 루틴 시작
                StartCoroutine(SearchWorkerStream());
            }
        }
        else
        {
            // 작업자라면 전문가용 모니터를 끕니다.
            if (monitorObject != null) monitorObject.SetActive(false);
            if (ndiReceiver != null) ndiReceiver.enabled = false;
        }
    }

    IEnumerator SearchWorkerStream()
    {
        while (true)
        {
            // 리시버가 켜져 있고 아직 채널이 연결되지 않았을 때만 검색
            if (ndiReceiver != null && string.IsNullOrEmpty(ndiReceiver.ndiName))
            {
                var sources = NdiFinder.sourceNames;

                // [수정] 네트워크상의 소스 중 우리가 정한 이름이 있는지 확인
                var targetSource = sources.FirstOrDefault(s => s.Contains(targetStreamName));

                if (targetSource != null)
                {
                    ndiReceiver.ndiName = targetSource;
                    Debug.Log($"[NDI] 작업자 방송 발견 및 연결 완료: {targetSource}");
                }
            }
            // 1초마다 반복 검색
            yield return new WaitForSeconds(1.0f);
        }
    }
}