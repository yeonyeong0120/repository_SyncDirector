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
            // 1. 소스 이름이 아직 비어있다면 검색을 시도합니다.
            if (ndiReceiver != null && string.IsNullOrEmpty(ndiReceiver.ndiName))
            {
                var sources = NdiFinder.sourceNames;
                if (sources != null)
                {
                    var targetSource = sources.FirstOrDefault(s => s != null && s.Contains(targetStreamName));
                    if (targetSource != null)
                    {
                        ndiReceiver.ndiName = targetSource;
                        Debug.Log($"[NDI] 소스 발견 및 연결 시도: {targetSource}");
                    }
                }
            }

            // 2. 소스는 연결되었는데 화면이 아직 하얗다면 (텍스처 바인딩 확인)
            // [수정] 연결된 이후에도 텍스처가 머티리얼에 들어갔는지 주기적으로 확인합니다.
            if (ndiReceiver != null && !string.IsNullOrEmpty(ndiReceiver.ndiName))
            {
                Renderer rend = monitorObject.GetComponent<Renderer>();
                // 리시버의 텍스처가 준비되었고, 아직 머티리얼에 연결되지 않았다면 바인딩합니다.
                if (rend != null && ndiReceiver.texture != null && rend.material.mainTexture != ndiReceiver.texture)
                {
                    rend.material.mainTexture = ndiReceiver.texture;
                    Debug.Log("[NDI] 텍스처 바인딩 완료 - 이제 화면이 보여야 합니다.");
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    //
}