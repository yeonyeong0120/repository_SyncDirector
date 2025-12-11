using UnityEngine;
using Mirror;

public class CameraSync : NetworkBehaviour
{
    public Transform workerCamera;
    public Transform expertCamera;

    [Header("NDI 비활성화")]
    public bool disableNDI = true;

    void Start()
    {
        if (disableNDI)
        {
            DisableAllNDI();
        }
    }

    void Update()
    {
        if (isServer && workerCamera != null)
        {
            RpcSyncCamera(workerCamera.position, workerCamera.rotation);
        }
    }

    [ClientRpc]
    void RpcSyncCamera(Vector3 pos, Quaternion rot)
    {
        if (!isServer && expertCamera != null)
        {
            expertCamera.position = pos;
            expertCamera.rotation = rot;
        }
    }

    void DisableAllNDI()
    {
        var senders = FindObjectsOfType<Klak.Ndi.NdiSender>();
        foreach (var sender in senders)
        {
            sender.enabled = false;
            Debug.Log("NDI Sender 비활성화");
        }

        var receivers = FindObjectsOfType<Klak.Ndi.NdiReceiver>();
        foreach (var receiver in receivers)
        {
            receiver.enabled = false;
            Debug.Log("NDI Receiver 비활성화");
        }
    }
}