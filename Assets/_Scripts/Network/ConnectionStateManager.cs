using UnityEngine;
using UnityEngine.Events;

// 연결 상태 정의
public enum ConnectionState
{
    Disconnected, // 끊김 (기본)
    Connecting,   // 연결 시도 중
    Connected,    // 연결 성공
    Failed        // 연결 실패 (타임아웃 등)
}

public class ConnectionStateManager : MonoBehaviour
{
    public static ConnectionStateManager Instance { get; private set; }

    [Header("현재 상태")]
    public ConnectionState currentState = ConnectionState.Disconnected;

    [Header("타임아웃 설정 (초)")]
    public float connectionTimeout = 10f; // 10초 동안 연결 안 되면 실패 처리
    private float connectionTimer = 0f;

    [Header("이벤트 (UI 팀원이 갖다 쓸 것들)")]
    public UnityEvent OnConnecting;       // 연결 시도 시작할 때
    public UnityEvent OnConnected;        // 연결 성공했을 때
    public UnityEvent OnDisconnected;     // 연결 끊겼을 때
    public UnityEvent OnConnectionFailed; // 연결 실패했을 때

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 연결 시도 중일 때만 시간을 잽니다 (타임아웃 체크)
        if (currentState == ConnectionState.Connecting)
        {
            connectionTimer += Time.deltaTime;

            // 정해진 시간(10초)을 넘기면 에러 처리
            if (connectionTimer > connectionTimeout)
            {
                OnConnectionTimeout();
            }
        }
    }

    // 1. 연결 시도 시작 (버튼 눌렀을 때 호출)
    public void StartConnecting()
    {
        currentState = ConnectionState.Connecting;
        connectionTimer = 0f;
        OnConnecting?.Invoke();
        Debug.Log("[상태] 연결 시도 중...");
    }

    // 2. 연결 성공 (Mirror가 알려줌)
    public void OnConnectionSuccess()
    {
        // 이미 연결된 상태면 무시
        if (currentState == ConnectionState.Connected) return;

        currentState = ConnectionState.Connected;
        connectionTimer = 0f;
        OnConnected?.Invoke(); // 구독자들에게 알림
        Debug.Log("[상태] 연결 성공!");
    }

    // 3. 연결 끊김 (Mirror가 알려줌)
    public void OnConnectionLost()
    {
        currentState = ConnectionState.Disconnected;
        OnDisconnected?.Invoke();
        Debug.LogWarning("[상태] 연결 끊김!");
    }

    // 4. 타임아웃 (시간 초과)
    private void OnConnectionTimeout()
    {
        currentState = ConnectionState.Failed;

        // Mirror의 연결 시도도 중단시켜야 함
        CustomNetworkManager.Instance.StopClient();

        OnConnectionFailed?.Invoke();
        Debug.LogError("[상태] 연결 시간 초과 (타임아웃)!");
    }
}