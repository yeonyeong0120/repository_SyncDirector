using UnityEngine;
using Mirror;
using System.Collections;

// 
public class VideoStreamManager : NetworkBehaviour
{
    [Header("카메라 설정")]
    public Camera centerViewCamera;
    public int textureWidth = 640; // 
    public int textureHeight = 480; // 
    public int jpegQuality = 75;    // 

    [Header("스트리밍 설정")]
    public float streamFPS = 15f;   // 

    private RenderTexture renderTexture;
    private Texture2D texture2D;
    private float streamTimer = 0f;

    void Start()
    {
        SetupCamera(); // 
    }

    void SetupCamera()
    {
        // RenderTexture 생성 
        renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
        renderTexture.Create();

        // Texture2D 생성 
        texture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);

        // 센터뷰 카메라가 없으면 자동 생성 
        if (centerViewCamera == null)
        {
            GameObject camObj = new GameObject("CenterViewCamera");
            centerViewCamera = camObj.AddComponent<Camera>();
            centerViewCamera.transform.SetParent(Camera.main.transform); // 메인 카메라 자식으로 설정
            centerViewCamera.transform.localPosition = Vector3.zero;
            centerViewCamera.transform.localRotation = Quaternion.identity;
        }

        centerViewCamera.targetTexture = renderTexture; // 
        centerViewCamera.enabled = true;

        Debug.Log($"[VideoStream] 카메라 설정 완료: {textureWidth}x{textureHeight}"); // 
    }

    void Update()
    {
        // 작업자(Client)만 스트리밍 
        // 주의: NetworkBehaviour가 동작하려면 이 객체가 네트워크상에 스폰되어야 합니다.
        if (!isServer && isLocalPlayer)
        {
            streamTimer += Time.deltaTime;
            if (streamTimer >= 1f / streamFPS)
            {
                CaptureAndSendFrame(); // 
                streamTimer = 0f;
            }
        }
    }

    void CaptureAndSendFrame()
    {
        // RenderTexture에서 Texture2D로 복사 
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;

        // JPEG로 압축 
        byte[] imageData = texture2D.EncodeToJPG(jpegQuality);

        // 네트워크로 전송 
        CmdSendVideoFrame(imageData);
    }

    [Command]
    void CmdSendVideoFrame(byte[] imageData) // 
    {
        // 서버에서 호스트(전문가)에게 전달 
        RpcReceiveVideoFrame(imageData);
    }

    [ClientRpc]
    void RpcReceiveVideoFrame(byte[] imageData) // 
    {
        // 전문가(Host/Server)만 수신하여 처리 
        if (isServer)
        {
            DisplayReceivedFrame(imageData);
        }
    }

    void DisplayReceivedFrame(byte[] imageData)
    {
        // 수신한 이미지를 UI에 표시 
        Texture2D receivedTex = new Texture2D(2, 2);
        receivedTex.LoadImage(imageData);

        // VideoDisplayPanel 싱글톤 인스턴스에 접근하여 표시
        VideoDisplayPanel.Instance?.DisplayFrame(receivedTex);
        // Debug.Log($"[VideoStream] 프레임 수신: {imageData.Length} bytes");
    }

    void OnDestroy()
    {
        // 메모리 해제 
        if (renderTexture != null) renderTexture.Release();
        if (texture2D != null) Destroy(texture2D);
    }
}