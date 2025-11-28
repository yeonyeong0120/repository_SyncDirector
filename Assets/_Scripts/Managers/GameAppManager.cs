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

        // 앱이 백그라운드로 넘어가도 네트워크가 안 끊기게 함
        Application.runInBackground = true;

        DontDestroyOnLoad(gameObject);
    }
}