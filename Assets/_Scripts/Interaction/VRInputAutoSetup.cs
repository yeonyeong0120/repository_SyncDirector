using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; // [수정] IEnumerator를 쓰기 위해 꼭 필요한 줄입니다!

public class VRInputAutoSetup : MonoBehaviour
{
    // void Start() 대신 이걸 씁니다. (기존 void Start는 지워야 합니다)
    IEnumerator Start()
    {
        // 씬 로드 후 오브젝트들이 안정화될 때까지 1초 기다립니다.
        yield return new WaitForSeconds(1.0f);

        // 1. 숨어있는 EventSystem과 OVRInputModule을 찾는다.
        OVRInputModule inputModule = FindFirstObjectByType<OVRInputModule>();

        if (inputModule == null)
        {
            Debug.LogError("OVRInputModule을 찾을 수 없습니다! EventSystem을 확인하세요.");
            yield break; // 더 이상 진행하지 않고 멈춤
        }

        // 2. 내 오른손(RightHandAnchor)을 찾는다.
        // (보통 [BuildingBlock] Camera Rig -> TrackingSpace -> RightHandAnchor 경로에 있음)
        GameObject rightHand = GameObject.Find("RightHandAnchor");

        if (rightHand != null)
        {
            // 3. 강제로 연결한다! (이제 손에서 레이저가 나감)
            inputModule.rayTransform = rightHand.transform;
            Debug.Log("[VR Setup] 오른손 컨트롤러가 레이캐스트 주체로 연결되었습니다!");
        }
        else
        {
            Debug.LogError("RightHandAnchor를 찾을 수 없습니다! 카메라 리그 이름을 확인하세요.");
        }
    }
}