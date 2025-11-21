using UnityEngine;
using extOSC; // extOSC 네임스페이스 필요

public class OSCManager : MonoBehaviour
{
    public static OSCManager Instance { get; private set; }

    [Header("OSC 설정")]
    public string remoteIP = "127.0.0.1"; // 테스트용 로컬 IP 
    public int remotePort = 8000;         // 전문가(PC)가 받을 포트 
    public int localPort = 9000;          // VR(작업자)가 받을 포트 

    private OSCTransmitter transmitter;
    private OSCReceiver receiver;

    void Awake()
    {
        // 싱글톤 설정 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupOSC(); // 
    }

    void SetupOSC()
    {
        // Transmitter 설정 (메시지 송신용) 
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        transmitter.RemoteHost = remoteIP;
        transmitter.RemotePort = remotePort;

        // Receiver 설정 (메시지 수신용) 
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = localPort;

        // 메시지 핸들러(수신 시 실행할 함수) 등록 
        receiver.Bind("/highlight", OnHighlightMessage);
        receiver.Bind("/effect", OnEffectMessage);
        receiver.Bind("/manual", OnManualMessage);

        Debug.Log($"[OSC] 초기화 완료 - 송신대상: {remoteIP}:{remotePort}, 수신포트: {localPort}");
    }

    // 네트워크 연결 시 상대방 IP 재설정 함수 
    public void SetRemoteIP(string ip)
    {
        remoteIP = ip;
        if (transmitter != null)
            transmitter.RemoteHost = ip;
        Debug.Log($"[OSC] 원격 IP 설정 변경: {ip}");
    }

    #region 송신 함수들 (전문가가 사용)
    // 
    public void SendHighlight(string objectID)
    {
        var message = new OSCMessage("/highlight");
        message.AddValue(OSCValue.String(objectID));
        transmitter.Send(message);
        Debug.Log($"[OSC] 하이라이트 전송: {objectID}");
    }

    // 
    public void SendEffect(string effectType, string objectID)
    {
        var message = new OSCMessage("/effect");
        message.AddValue(OSCValue.String(effectType));
        message.AddValue(OSCValue.String(objectID));
        transmitter.Send(message);
        Debug.Log($"[OSC] 이펙트 전송: {effectType} on {objectID}");
    }

    // 
    public void SendManual(string manualID)
    {
        var message = new OSCMessage("/manual");
        message.AddValue(OSCValue.String(manualID));
        transmitter.Send(message);
        Debug.Log($"[OSC] 매뉴얼 전송: {manualID}");
    }
    #endregion

    #region 수신 콜백들 (작업자가 받음)
    // 
    void OnHighlightMessage(OSCMessage message)
    {
        if (message.Values.Count > 0)
        {
            string objectID = message.Values[0].StringValue;
            Debug.Log($"[OSC] 하이라이트 수신: {objectID}");

            // TODO: Week 3에서 구현할 하이라이트 시스템 연결
            // HighlightSystem.Instance?.HighlightObject(objectID);
        }
    }

    // 
    void OnEffectMessage(OSCMessage message)
    {
        if (message.Values.Count > 1)
        {
            string effectType = message.Values[0].StringValue;
            string objectID = message.Values[1].StringValue;
            Debug.Log($"[OSC] 이펙트 수신: {effectType} on {objectID}");

            // TODO: Week 3에서 구현할 이펙트 시스템 연결
            // EffectSystem.Instance?.PlayEffect(effectType, objectID);
        }
    }

    // 
    void OnManualMessage(OSCMessage message)
    {
        if (message.Values.Count > 0)
        {
            string manualID = message.Values[0].StringValue;
            Debug.Log($"[OSC] 매뉴얼 수신: {manualID}");

            // TODO: Week 4에서 구현할 매뉴얼 시스템 연결
            // ManualDisplaySystem.Instance?.ShowManual(manualID);
        }
    }
    #endregion

    void OnDestroy()
    {
        //

        /*
            if (receiver != null)
            {
                receiver.Unbind("/highlight");
                receiver.Unbind("/effect");
                receiver.Unbind("/manual");
            }
        */
    }

}