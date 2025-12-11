using UnityEngine;
using Mirror;

public class VRPlayerSetup : NetworkBehaviour
{
    [Header("연결 대상")]
    public GameObject cameraRig; // [BuildingBlock] Camera Rig
    public Renderer bodyMesh;    // 플레이어의 캡슐(몸통) Mesh Renderer

    void Start()
    {
        // 1. 내 캐릭터(Local Player)라면?
        if (isLocalPlayer)
        {
            // 내 카메라는 켠다.
            if (cameraRig != null) cameraRig.SetActive(true);

            // ★ 중요: 내 몸(캡슐)은 내 눈에 안 보이게 끈다! (그래야 시야 안 가림)
            if (bodyMesh != null) bodyMesh.enabled = false;
        }
        // 2. 남의 캐릭터(Remote Player)라면?
        else
        {
            // 남의 카메라는 끈다.
            if (cameraRig != null) cameraRig.SetActive(false);

            // 남의 몸은 보여야 한다! (그래야 상대방이 보임)
            if (bodyMesh != null) bodyMesh.enabled = true;
        }
    }
}