using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPDiscovery : MonoBehaviour
{
    public static UDPDiscovery Instance { get; private set; }

    [Header("Discovery 설정")]
    public int discoveryPort = 47777; // Discovery용 포트 (Mirror 포트와 달라야 함)
    public float broadcastInterval = 1f; // 1초마다 방송

    private UdpClient udpClient;
    private bool isDiscovering = false;
    private string myIP;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        myIP = GetLocalIPAddress();
    }

    // [Sender - 전문가] : "나 여기 있어!"라고 계속 방송하기
    public void StartBroadcasting()
    {
        if (isDiscovering) return;

        isDiscovering = true;
        // 1초마다 BroadcastDiscovery 함수 실행
        InvokeRepeating(nameof(BroadcastDiscovery), 0f, broadcastInterval);

        // 동시에 응답도 들어야 하므로 리스닝 시작
        //StartListening();

        Debug.Log($"[Discovery] 브로드캐스트 시작 (내 IP: {myIP})");
    }

    void BroadcastDiscovery()
    {
        try
        {
            // 메시지 포맷: "DISCOVER:내IP"
            string message = $"DISCOVER:{myIP}";
            byte[] data = Encoding.UTF8.GetBytes(message);

            // 로컬 네트워크 전체에 쏘기 (255.255.255.255)
            UdpClient sender = new UdpClient();
            sender.EnableBroadcast = true;

            // Broadcast(255.255.255.255) 대신 로컬호스트(127.0.0.1)로 강제 전송
            // 이유: 윈도우에서는 자기 자신에게 Broadcast가 안 가는 경우가 많음
            // ★ 나중에 VR 기기 테스트할 때는 아래 줄을 주석 처리하고, 그 아래 줄을 푸세요! ★
            IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), discoveryPort);
            // IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast, discoveryPort); // <-- 원래 코드 (VR용)

            sender.Send(data, data.Length, broadcastEP);
            sender.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Discovery] 브로드캐스트 에러: {e.Message}");
        }
    }

    // [Receiver - 작업자] : 귀 열고 듣기
    public void StartListening()
    {
        try
        {
            // 이미 열려있으면 닫고 다시 열기
            if (udpClient != null) udpClient.Close();

            udpClient = new UdpClient(discoveryPort);
            udpClient.BeginReceive(OnReceiveData, null); // 비동기 수신 대기

            Debug.Log($"[Discovery] 수신 대기 중... (Port: {discoveryPort})");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Discovery] 리스닝 실패: {e.Message}");
        }
    }

    // 데이터가 들어왔을 때 실행되는 콜백 함수
    // UDPDiscovery.cs 의 OnReceiveData 함수

    void OnReceiveData(IAsyncResult result)
    {
        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, discoveryPort);
            byte[] data = udpClient.EndReceive(result, ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);

            // 1. 작업자가 전문가의 방송("DISCOVER")을 들었을 때
            if (message.StartsWith("DISCOVER:"))
            {
                // ★★★ 수정 1: 내가 이미 전문가(Host)라면, 내 방송은 무시한다! ★★★
                if (CustomNetworkManager.Instance.myRole == UserRole.Expert) return;

                string senderIP = message.Replace("DISCOVER:", "");

                Debug.Log($"[Discovery] 전문가 발견! ({senderIP}) -> 바로 연결 시도");

                // 메인 스레드에서 연결 실행
                MainThreadDispatcher.Enqueue(() => {
                    StopDiscovery(); // 찾았으니 검색 중단

                    // ★★★ 수정 2: 내 역할을 '작업자'로 확실하게 도장 찍기 (UI 업데이트용) ★★★
                    CustomNetworkManager.Instance.myRole = UserRole.Worker;

                    // ★★★ 수정 3: 테스트 중에는 IP 무시하고 무조건 'localhost'로 연결! ★★★
                    // (나중에 VR 기기로 할 때는 senderIP로 바꿔야 합니다)
                    CustomNetworkManager.Instance.StartAsWorker("localhost");
                });
            }

            // (RESPONSE 처리 부분은 삭제하셨으니 생략)

            // 계속 듣기
            udpClient.BeginReceive(OnReceiveData, null);
        }
        catch (Exception e)
        {
            // 종료될 때 에러는 무시
        }
    }

    // 응답 보내기 (작업자 -> 전문가)
    void SendResponse(string targetIP)
    {
        try
        {
            string message = $"RESPONSE:{myIP}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            UdpClient sender = new UdpClient();

            // 응답도 안전하게 127.0.0.1로 보냄 (PC 테스트용)
            IPEndPoint targetEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), discoveryPort);
            // IPEndPoint targetEP = new IPEndPoint(IPAddress.Parse(targetIP), discoveryPort); // <-- 원래 코드 (VR용)

            sender.Send(data, data.Length, targetEP);
            sender.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Discovery] 응답 전송 실패: {e.Message}");
        }
    }

    // ---------------------------------------------------------
    // 공통 유틸리티
    // ---------------------------------------------------------
    public void StopDiscovery()
    {
        isDiscovering = false;
        CancelInvoke(nameof(BroadcastDiscovery));
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }
        Debug.Log("[Discovery] 종료");
    }

    string GetLocalIPAddress()
    {
        string localIP = "127.0.0.1";
        try
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (Exception e) { Debug.LogError(e.Message); }
        return localIP;
    }

    void OnDestroy()
    {
        StopDiscovery();
    }
}

// 비동기 스레드(UDP)에서 메인 스레드(Unity API)를 호출하기 위한 헬퍼 클래스
// CustomNetworkManager 같은 Unity 함수는 메인 스레드에서만 실행돼야 함!
public class MainThreadDispatcher : MonoBehaviour
{
    private static System.Collections.Generic.Queue<Action> _executionQueue = new System.Collections.Generic.Queue<Action>();

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    public static void Enqueue(Action action)
    {
        if (action == null) return;
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    // 자동 생성
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // FindObjectOfType -> FindFirstObjectByType 으로 변경
        if (FindFirstObjectByType<MainThreadDispatcher>() == null)
        {
            GameObject go = new GameObject("MainThreadDispatcher");
            go.AddComponent<MainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }
    }
}