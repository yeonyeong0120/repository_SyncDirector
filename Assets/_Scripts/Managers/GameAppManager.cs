using UnityEngine;

public class GameAppManager : MonoBehaviour
{
    void Awake()
    {
        // 프레임 속도 고정 // 멀미방지~_~
        // Quest 2 기준 72Hz 또는 90Hz가 국룰
        Application.targetFrameRate = 72;

        // 화면 꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 수직 동기화 끄기
        QualitySettings.vSyncCount = 0;

        DontDestroyOnLoad(gameObject);
    }
}