using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputAutoSetup : MonoBehaviour
{
    void Start()
    {
        // 1. 숨어있는 EventSystem과 OVRInputModule을 찾는다.
        OVRInputModule inputModule = FindFirstObjectByType<OVRInputModule>();

        if (inputModule == null)
        {
            Debug.LogError("?? OVRInputModule을 찾을 수 없습니다! EventSystem을 확인하세요.");
            return;
        }

        // 2. 내 오른손(RightHandAnchor)을 찾는다.
        // (보통 [BuildingBlock] Camera Rig -> TrackingSpace -> RightHandAnchor 경로에 있음)
        GameObject rightHand = GameObject.Find("RightHandAnchor");

        if (rightHand != null)
        {
            // 3. 강제로 연결한다! (이제 손에서 레이저가 나감)
            inputModule.rayTransform = rightHand.transform;
            Debug.Log("?? [VR Setup] 오른손 컨트롤러가 레이캐스트 주체로 연결되었습니다!");
        }
        else
        {
            Debug.LogError("?? RightHandAnchor를 찾을 수 없습니다! 카메라 리그 이름을 확인하세요.");
        }
    }
}