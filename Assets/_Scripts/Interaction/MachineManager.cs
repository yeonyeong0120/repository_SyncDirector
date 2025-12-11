using UnityEngine;
using Mirror;

public class MachineManager : NetworkBehaviour
{
    [Header("기계 연결")]
    public FactoryMachine leverMachine; // Pc.001에 붙은 FactoryMachine
    public FactoryMachine valveMachine; // Rotate_Obj에 붙은 FactoryMachine
    public GoldMove goldPile;          // GoldPile에 붙은 GoldMove

    // 이 오브젝트가 서버에서 스폰될 때 한 번 실행됩니다.
    public override void OnStartServer()
    {
        base.OnStartServer();

        // 씬 오브젝트인 GoldPile을 네트워크에 등록하여 클라이언트에게 알려줍니다.
        if (goldPile != null && goldPile.netId == 0)
        {
            // GoldPile이 이미 씬에 있으므로 강제로 스폰합니다.
            NetworkServer.Spawn(goldPile.gameObject);
            Debug.Log("GoldPile 오브젝트를 네트워크에 수동으로 스폰했습니다.");
        }
    }

    // ★ 이 함수는 서버에서 레버나 밸브가 작동할 때마다 호출됩니다. ★
    [Server]
    public void CheckAllMachinesStatus()
    {
        // 1. 레버와 밸브의 작동 상태(isWorking)를 확인합니다.
        bool isLeverOn = leverMachine.isWorking;
        bool isValveOn = valveMachine.isWorking;

        Debug.Log($"[서버] 상태 체크: 레버={isLeverOn}, 밸브={isValveOn}");

        // 2. 두 기계가 모두 켜져 있다면 골드바를 이동시킵니다.
        if (isLeverOn && isValveOn)
        {
            if (goldPile != null)
            {
                goldPile.RpcStartMoveOut(); // 골드바 이동 명령을 모든 클라이언트에게 보냅니다.
                Debug.Log("★★★ 모든 기계 작동 완료! 골드바 출현! ★★★");
            }
        }
    }
}