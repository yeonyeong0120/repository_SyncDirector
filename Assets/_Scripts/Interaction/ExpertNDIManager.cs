using UnityEngine;
using Klak.Ndi;
using Mirror;
using System.Collections;
using System.Linq;

public class ExpertNDIManager : MonoBehaviour
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

    void Update()
    {
        // CustomNetworkManager.Instance.myRole로 내 역할을 확인하는 로직은
        // MonoBehaviour에서도 그대로 작동합니다.
        if (CustomNetworkManager.Instance == null) return;

        if (CustomNetworkManager.Instance.myRole == UserRole.Expert)
        {
            // 전문가 로직 실행
        }
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
            if (ndiReceiver != null && string.IsNullOrEmpty(ndiReceiver.ndiName))
            {
                // [보완] NdiFinder.sourceNames가 null인지 체크하여 에러를 방지합니다.
                var sources = NdiFinder.sourceNames;
                if (sources != null)
                {
                    var targetSource = sources.FirstOrDefault(s => s != null && s.Contains(targetStreamName));

                    if (targetSource != null)
                    {
                        ndiReceiver.ndiName = targetSource;

                        // ▼▼▼ [추가] 수신된 텍스처를 Quad의 머티리얼에 실시간으로 연결합니다. ▼▼▼
                        // Quad의 Renderer를 가져와서 메인 텍스처를 NDI 수신 텍스처로 바꿉니다.
                        Renderer rend = monitorObject.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            // NdiReceiver가 제공하는 텍스처를 머티리얼에 할당합니다.
                            rend.material.mainTexture = ndiReceiver.texture;
                        }
                        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

                        Debug.Log($"[NDI] 연결 완료 및 텍스처 바인딩: {targetSource}");
                    }
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    //
}