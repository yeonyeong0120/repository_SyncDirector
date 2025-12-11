using UnityEngine;
using Klak.Ndi;
using Mirror; // Mirror 필수
using System.Collections;
using System.Linq;

public class ExpertScreenManager : NetworkBehaviour // MonoBehavior 아님!
{
    [Header("연결할 컴포넌트")]
    public GameObject screenPanel;
    public NdiReceiver ndiReceiver;

    // 씬이 로드되고 네트워크가 준비되면 자동으로 실행됨
    void Start()
    {
        StartCoroutine(CheckRoleAndSetup());
    }

    IEnumerator CheckRoleAndSetup()
    {
        // 네트워크가 안정화될 때까지 아주 잠깐 대기
        yield return new WaitForSeconds(0.5f);

        // 1. 내가 서버(Host)인가? = 관리자
        // (NetworkServer.active는 호스트에서만 true입니다)
        if (NetworkServer.active)
        {
            Debug.Log("[시스템] 나는 방장(Host/관리자)입니다. 화면을 켭니다.");
            if (screenPanel != null) screenPanel.SetActive(true);

            if (ndiReceiver != null)
            {
                ndiReceiver.enabled = true;
                StartCoroutine(FindWorkerStream()); // 방송 찾기 시작
            }
        }
        // 2. 나는 클라이언트인가? = 작업자
        else
        {
            Debug.Log("[시스템] 나는 참가자(Client/작업자)입니다. 화면을 끕니다.");
            if (screenPanel != null) screenPanel.SetActive(false);
            if (ndiReceiver != null) ndiReceiver.enabled = false;
        }
    }

    // (이하 동일) 방송 찾는 로직
    IEnumerator FindWorkerStream()
    {
        while (true)
        {
            if (ndiReceiver != null && ndiReceiver.enabled && string.IsNullOrEmpty(ndiReceiver.ndiName))
            {
                var sources = NdiFinder.sourceNames;
                if (sources.Any())
                {
                    ndiReceiver.ndiName = sources.First();
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}