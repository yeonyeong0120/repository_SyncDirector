using UnityEngine;
using UnityEngine.Events;
using Mirror; // 혹시 몰라서 추가

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
        // 싱글톤 패턴은 유지하되, DDOL은 NetworkManager가 처리하므로 제거 했음...
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 연결 시도 중일 때만 타임아웃 체크
        if (currentState == ConnectionState.Connecting)
        {
            connectionTimer += Time.deltaTime;

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

    // 2. 연결 성공 (CustomNetworkManager에서 호출)
    public void OnConnectionSuccess()
    {
        if (currentState == ConnectionState.Connected) return;

        currentState = ConnectionState.Connected;
        connectionTimer = 0f; // 타이머 초기화
        OnConnected?.Invoke();
        Debug.Log("[ConnectionManager] 연결 성공!");
    }

    // 3. 연결 끊김 (Mirror가 알려줌)
    public void OnConnectionLost()
    {
        // 일단은 Disconnected 처리
        currentState = ConnectionState.Disconnected;
        OnDisconnected?.Invoke();
        Debug.LogWarning("[ConnectionManager] 연결 끊김.");
    }

    // 4. 타임아웃 (시간 초과)
    private void OnConnectionTimeout()
    {
        currentState = ConnectionState.Failed;

        // 타임아웃 발생 시 연결 시도 중단
        // CustomNetworkManager가 있는지 확인 후 호출
        if (CustomNetworkManager.Instance != null)
        {
            CustomNetworkManager.Instance.StopClient();
        }

        OnConnectionFailed?.Invoke();
        Debug.LogError($"[ConnectionManager] {connectionTimeout}초 동안 응답이 없어 연결 실패 처리됨.");
    }
}