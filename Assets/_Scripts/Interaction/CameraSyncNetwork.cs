using UnityEngine;
using Mirror;

public class CameraSyncNetwork : NetworkBehaviour
{
    private Transform workerCamera;
    private Transform expertCamera;

    void Start()
    {
        Invoke("FindCameras", 1f);
    }

    void FindCameras()
    {
        GameObject workerGroup = GameObject.Find("Worker_Group");
        GameObject expertGroup = GameObject.Find("Expert_Group");

        if (workerGroup != null && workerGroup.activeInHierarchy)
        {
            workerCamera = workerGroup.GetComponentInChildren<Camera>()?.transform;
            Debug.Log("Worker Camera 찾음!");
        }

        if (expertGroup != null && expertGroup.activeInHierarchy)
        {
            expertCamera = expertGroup.GetComponentInChildren<Camera>()?.transform;
            Debug.Log("Expert Camera 찾음!");
        }
    }

    void Update()
    {
        if (isServer && workerCamera != null)
        {
            transform.position = workerCamera.position;
            transform.rotation = workerCamera.rotation;
        }

        if (isClient && !isServer && expertCamera != null)
        {
            expertCamera.position = transform.position;
            expertCamera.rotation = transform.rotation;
        }
    }
}