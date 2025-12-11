using UnityEngine;
using Mirror; // 미러 필수

public class VRPlayerSetup : NetworkBehaviour
{
    [Header("남의 것이면 꺼버릴 대상")]
    public GameObject cameraRig; // [BuildingBlock] Camera Rig를 여기에 연결

    void Start()
    {
        // 만약 이 플레이어가 "내 캐릭터(Local Player)"라면?
        if (isLocalPlayer)
        {
            // 내 카메라는 켜둔다. (아무것도 안 해도 됨)
            // 내 캐릭터의 위치를 잡기 위해 오디오 리스너 등을 활성화
            if (cameraRig != null) cameraRig.SetActive(true);
        }
        // 만약 이 플레이어가 "남의 캐릭터(Remote Player)"라면?
        else
        {
            // 남의 카메라는 끈다! (이게 핵심)
            // 그래야 내 기기에서 카메라가 2개가 되는 걸 막을 수 있음
            if (cameraRig != null) cameraRig.SetActive(false);
        }
    }
}