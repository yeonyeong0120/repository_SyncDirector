using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 
public class VideoDisplayPanel : MonoBehaviour
{
    public static VideoDisplayPanel Instance { get; private set; } // 

    [Header("UI References")]
    public RawImage videoDisplay;       // 
    public TextMeshProUGUI statusText;  // 

    void Awake()
    {
        // 싱글톤 패턴 구현 
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (statusText != null)
            statusText.text = "영상 대기 중..."; // 
    }

    public void DisplayFrame(Texture2D frame)
    {
        // 받은 텍스처를 RawImage에 적용 
        videoDisplay.texture = frame;
        if (statusText != null)
            statusText.text = "영상 스트리밍 중";
    }
}