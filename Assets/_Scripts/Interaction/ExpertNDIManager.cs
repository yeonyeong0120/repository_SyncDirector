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

    // [추가] Renderer 변수를 클래스 멤버로 선언하여 Update와 Coroutine 모두에서 쓰게 합니다.
    private Renderer rend;

    void Start()
    {
        // 씬 시작 시 역할에 따른 초기화 실행
        StartCoroutine(InitializeExpertScreen());
    }

    void Update()
    {
        // [수정] 렌더러나 리시버가 없으면 실행하지 않도록 보호막을 칩니다.
        if (monitorObject == null || ndiReceiver == null || ndiReceiver.texture == null) return;

        // 렌더러가 아직 할당 안 됐다면 가져옵니다.
        if (rend == null) rend = monitorObject.GetComponent<Renderer>();

        if (rend != null)
        {
            // [수정] .material 대신 .sharedMaterial을 사용하여 메모리 렉을 방지합니다.
            if (rend.sharedMaterial.mainTexture != ndiReceiver.texture)
            {
                rend.sharedMaterial.mainTexture = ndiReceiver.texture;
                Debug.Log("[NDI] 텍스처가 실시간으로 바인딩되었습니다.");
            }
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
            // 소스 이름 탐색 로직은 유지합니다.
            if (ndiReceiver != null && string.IsNullOrEmpty(ndiReceiver.ndiName))
            {
                var sources = NdiFinder.sourceNames;
                if (sources != null && sources.Any())
                {
                    var targetSource = sources.FirstOrDefault(s => s != null && s.Contains(targetStreamName));
                    if (targetSource != null)
                    {
                        ndiReceiver.ndiName = targetSource;
                    }
                }
            }

            // [삭제 권장] 아래의 텍스처 바인딩 로직은 이제 Update에서 처리하므로 
            // 코루틴에서는 소스 이름 찾는 것에만 집중하게 합니다.

            yield return new WaitForSeconds(2.0f);
        }
    }

    //
}