using UnityEngine;
using Mirror;

public class VRCameraLink : NetworkBehaviour
{
    [Header("내 몸통 (가려야 함)")]
    public Renderer bodyRenderer; // 캡슐(Player)의 Mesh Renderer

    // 게임 시작하고 내 캐릭터가 태어날 때 딱 한 번 실행됨
    public override void OnStartLocalPlayer()
    {
        // 1. 씬(Scene)에 미리 놓여있는 OVRCameraRig를 찾는다.
        OVRCameraRig sceneRig = FindFirstObjectByType<OVRCameraRig>();

        if (sceneRig != null)
        {
            // 2. 카메라를 내 몸(Player)의 자식으로 입양한다.
            sceneRig.transform.SetParent(this.transform);

            // 3. 위치를 내 눈높이(0,0,0)로 딱 맞춘다.
            sceneRig.transform.localPosition = Vector3.zero;
            sceneRig.transform.localRotation = Quaternion.identity;

            Debug.Log("카메라 합체 완료!");
        }
        else
        {
            Debug.LogError("Scene 03에 [BuildingBlock] Camera Rig가 없습니다!");
        }

        // 4. 내 몸(캡슐)은 내 눈앞을 가리니까 안 보이게 끈다.
        if (bodyRenderer != null)
        {
            bodyRenderer.enabled = false;
        }
    }

    // 다른 사람(Remote) 캐릭터일 때 실행됨
    void Start()
    {
        if (!isLocalPlayer)
        {
            // 남의 캐릭터는 몸통이 보여야 한다.
            if (bodyRenderer != null)
            {
                bodyRenderer.enabled = true;
            }
        }
    }
}