using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement; // 씬 관리 기능 추가

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance { get; private set; }
    public static Action OnWorkerConnected;

    [Header("역할 구분")]
    public UserRole myRole = UserRole.None;

    public string hostIP = "";

    public override void Awake()
    {
        base.Awake(); // Mirror의 기본 초기화 실행

        // 싱글톤 보장: 이미 존재하면 나는(새로 생긴 놈은) 죽는다.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this) // Instance가 내가 아니라면
        {
            Destroy(gameObject);
        }
    }

    // 전문가(Host) 시작
    public void StartAsExpert()
    {
        myRole = UserRole.Expert;

        // 씬 전환이 안전하게 일어나도록 로그 찍기
        Debug.Log("[CustomNetworkManager] 전문가 모드: Host 시작...");

        StartHost(); // -> 이게 성공하면 자동으로 Online Scene으로 넘어갑니다.
    }

    // 작업자(Client) 시작
    public void StartAsWorker(string ip)
    {
        // IP가 비어있으면 로컬호스트로 (테스트용)
        if (string.IsNullOrEmpty(ip)) ip = "localhost";

        // 연결 상태 매니저에게 알림 (ConnectionStateManager가 씬에 있다면)
        if (ConnectionStateManager.Instance != null)
            ConnectionStateManager.Instance.StartConnecting();

        myRole = UserRole.Worker;
        networkAddress = ip;

        Debug.Log($"[CustomNetworkManager] 작업자 모드: {ip} 연결 시도...");
        StartClient();
    }

    // --- 이벤트 콜백 ---

    // [서버] 클라이언트 접속 시 (작업자가 들어왔을 때)
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"[Host] 클라이언트 접속됨: {conn.address}");

        // 내(Host)가 접속한 게 아니라 남(Client)이 접속했을 때만 이벤트 발생
        if (conn.connectionId != 0)
        {
            OnWorkerConnected?.Invoke();
        }
    }

    // [클라이언트] 접속 성공 시
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("[Client] 서버에 연결 성공!");

        if (myRole == UserRole.Worker && ConnectionStateManager.Instance != null)
        {
            ConnectionStateManager.Instance.OnConnectionSuccess();
        }
    }

    // [클라이언트] 접속 끊김 시
    public override void OnClientDisconnect()
    {
        // [추가] 왜 끊겼는지 로그를 찍어 원인을 파악합니다.
        Debug.LogWarning("[Client] 서버와 연결이 끊겨 씬을 복구합니다.");

        base.OnClientDisconnect();

        if (ConnectionStateManager.Instance != null)
            ConnectionStateManager.Instance.OnConnectionLost();

        // 만약 에러로 인해 끊긴 것이라면 씬 컨트롤러를 통해 복귀 메시지를 띄울 수 있습니다.
        if (SceneController.Instance != null)
        {
            SceneController.Instance.ReturnToRoleSelect("네트워크 오류로 연결이 종료되었습니다.");
        }
    }
}

// ▼▼▼ 이 부분이 빠져서 오류가 났었습니다! ▼▼▼
// 역할 구분용 Enum
public enum UserRole
{
    None,
    Expert, // 전문가 (Host)
    Worker  // 작업자 (Client)
}