using UnityEngine;
using Klak.Ndi; // NDI 기능 사용
using System.Collections;
using System.Linq;

public class AutoNDIReceiver : MonoBehaviour
{
    private NdiReceiver receiver;

    void Start()
    {
        receiver = GetComponent<NdiReceiver>();
        // 1초마다 방송을 찾도록 시도합니다.
        StartCoroutine(FindSourceRoutine());
    }

    IEnumerator FindSourceRoutine()
    {
        while (true)
        {
            // 리시버가 있고, 아직 연결된 소스가 없다면?
            if (receiver != null && string.IsNullOrEmpty(receiver.ndiName))
            {
                // 1. 네트워크에 있는 NDI 소스 이름들을 다 가져옵니다.
                var sourceNames = NdiFinder.sourceNames;

                // 2. 만약 하나라도 발견되면?
                if (sourceNames.Any())
                {
                    // 첫 번째 발견된 녀석에게 바로 연결!
                    receiver.ndiName = sourceNames.First();
                    Debug.Log($"[NDI] 소스 발견 및 연결: {receiver.ndiName}");
                }
            }
            // 3. 1초에 한 번씩 검사 (너무 자주하면 렉 걸림)
            yield return new WaitForSeconds(1.0f);
        }
    }
}