using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance { get; private set; }

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
        }
        // 주의: NetworkManager는 기본적으로 DontDestroyOnLoad가 적용되므로 
        // 중복 생성을 막기 위해 기존 것이 있다면 파괴하는 로직이 필요할 수 있습니다.
        else
        {
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
        Debug.Log("[Client] 서버에 연결 성공!");
    }

    // 연결 끊김 시 호출 (클라이언트 측)
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.LogError("[Client] 서버 연결 끊김!");
    }

    // 클라이언트 접속 시 호출 (서버 측)
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"[Host] 클라이언트 연결됨: {conn.address}");
    }
}

// 역할 구분용 Enum
public enum UserRole
{
    None,
    Expert, // 전문가 (Host)
    Worker  // 작업자 (Client)
}
