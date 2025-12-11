using UnityEngine;
using Mirror;
// OVR 기능을 쓰기 위해 네임스페이스 추가 가능하지만, 
// 여기서는 GetComponent로 직접 찾아서 처리합니다.

public class VRPlayerSetup : NetworkBehaviour
{
    [Header("연결 대상")]
    public GameObject cameraRig; // [BuildingBlock] Camera Rig
    public Renderer bodyMesh;    // Player (본체)

    void Start()
    {
        // 안전장치: cameraRig가 비어있으면 찾기
        if (cameraRig == null)
            cameraRig = GetComponentInChildren<OVRCameraRig>()?.gameObject;

        // 1. [내 캐릭터]라면? -> "내 뇌(Manager)와 눈(Camera)을 켠다"
        if (isLocalPlayer)
        {
            if (cameraRig != null) cameraRig.SetActive(true);
            if (bodyMesh != null) bodyMesh.enabled = false; // 내 몸 안 보이게

            // ★ 중요: 내 OVRManager는 켜야 함!
            OVRManager myManager = GetComponentInChildren<OVRManager>();
            if (myManager != null) myManager.enabled = true;
        }
        // 2. [남의 캐릭터]라면? -> "남의 뇌와 눈을 제거한다"
        else
        {
            // 카메라 끄기
            if (cameraRig != null) cameraRig.SetActive(false);
            if (bodyMesh != null) bodyMesh.enabled = true; // 남의 몸 보이게

            // ★★★ 핵심 해결책: 남의 OVRManager는 컴포넌트 자체를 파괴한다! ★★★
            // 그냥 끄는(disable) 것보다 파괴(Destroy)하는 게 가장 확실합니다.
            OVRManager remoteManager = GetComponentInChildren<OVRManager>();
            if (remoteManager != null)
            {
                Destroy(remoteManager);
            }

            // 오디오 리스너도 파괴 (소리 충돌 방지)
            AudioListener remoteListener = GetComponentInChildren<AudioListener>();
            if (remoteListener != null)
            {
                Destroy(remoteListener);
            }
        }
    }
}