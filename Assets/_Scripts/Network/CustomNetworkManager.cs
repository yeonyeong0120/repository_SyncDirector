using UnityEngine;
using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance { get; private set; }
    public static Action OnWorkerConnected;

    [Header("역할 구분")]
    public UserRole myRole = UserRole.None;

    [Header("연결 정보")]
    public string hostIP = "";

    public override void Awake()
    {
        base.Awake(); // 중요: 부모 클래스(Mirror)의 Awake도 실행해줘야 함

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 원조(Scene 00/01에서 온 녀석)가 있다면, 
            // Scene 03에 미리 배치되어 있던 나는 '가짜'이므로 스스로 사라진다.
            Destroy(gameObject);
        }
    }

    // 전문가 역할로 호스트 시작
    public void StartAsExpert()
    {
        myRole = UserRole.Expert;
        StartHost(); // 호스트(서버+클라이언트) 시작
        hostIP = GetLocalIPAddress();
        Debug.Log($"[Host] 전문가로 시작. IP: {hostIP}");
    }

    // 작업자 역할로 클라이언트 시작
    public void StartAsWorker(string ip)
    {
        // 커넥트매니저한테 연결시도하라고 알렺주기
        ConnectionStateManager.Instance.StartConnecting();

        myRole = UserRole.Worker;
        networkAddress = ip;
        StartClient(); // 클라이언트 시작
        Debug.Log($"[Client] 작업자로 연결 시도: {ip}");
    }

    // 로컬 IP 주소 가져오기 (내부망 IP 확인용)
    string GetLocalIPAddress()
    {
        string localIP = "localhost";
        try
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"IP 주소 가져오기 실패: {e.Message}");
        }
        return localIP;
    }

    // 연결 성공 시 호출 (클라이언트 측)
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        // 연결 성공하면 성공했다규 알려주기
        // 작업자일때만 성공햇다고 알려주셈
        if (myRole == UserRole.Worker)
        {
            if (ConnectionStateManager.Instance != null)
                ConnectionStateManager.Instance.OnConnectionSuccess();
        }

        Debug.Log("[Client] 서버에 연결 성공!");
    }

    // 연결 끊김 시 호출 (클라이언트 측)
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        // 실패하면 실패했다고 알려주기
        // [수정] Null 체크 추가!
        if (ConnectionStateManager.Instance != null)
            ConnectionStateManager.Instance.OnConnectionLost();

        Debug.LogError("[Client] 서버 연결 끊김!");
    }

    // 클라이언트 접속 시 호출 (서버 측)
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"[Host] 클라이언트 연결됨: {conn.address}");

        // 접속시 텍스트 바꾸기 일단임시로...
        if (conn.connectionId > 0)
        {
            OnWorkerConnected?.Invoke();
        }
    }
}

// 역할 구분용 Enum
public enum UserRole
{
    None,
    Expert, // 전문가 (Host)
    Worker  // 작업자 (Client)
}
