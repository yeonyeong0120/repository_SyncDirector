using UnityEngine;
using Mirror;

public class VRPlayerSetup : NetworkBehaviour
{
    [Header("연결 대상")]
    public GameObject cameraRig; // [BuildingBlock] Camera Rig 연결
    public Renderer bodyMesh;    // Player (본체) 연결

    void Start()
    {
        // ★ 안전장치: cameraRig가 연결 안 되어 있으면 찾아서라도 넣음
        if (cameraRig == null)
            cameraRig = GetComponentInChildren<OVRCameraRig>()?.gameObject;

        // 1. 내 캐릭터(Local Player)라면? -> "주인님 어서오세요"
        if (isLocalPlayer)
        {
            // 내 카메라는 켠다.
            if (cameraRig != null) cameraRig.SetActive(true);

            // 내 몸(캡슐)은 내 시야를 가리니까 끈다.
            if (bodyMesh != null) bodyMesh.enabled = false;

            // (추가) 내 오디오 리스너 켜기
            AudioListener listener = cameraRig.GetComponentInChildren<AudioListener>();
            if (listener != null) listener.enabled = true;
        }
        // 2. 남의 캐릭터(Remote Player)라면? -> "당신은 그냥 껍데기일 뿐"
        else
        {
            // ★ 핵심: 남의 카메라는 통째로 꺼버린다! (OVRManager 충돌 방지)
            if (cameraRig != null) cameraRig.SetActive(false);

            // 남의 몸은 보여야 한다.
            if (bodyMesh != null) bodyMesh.enabled = true;

            // (추가) 혹시 모르니 오디오 리스너도 확실히 끈다
            AudioListener listener = GetComponentInChildren<AudioListener>();
            if (listener != null) listener.enabled = false;
        }
    }
}