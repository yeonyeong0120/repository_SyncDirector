using UnityEngine;
using Mirror;

public class FactoryMachine : NetworkBehaviour
{
    [Header("설정")]
    public Animator anim;
    public string animParamName = "IsWorking"; // "IsWorking" Bool 파라미터 이름

    [Header("관리자 연결")]
    public MachineManager manager; // ★ MachineManager 스크립트를 연결할 변수 추가 ★

    // 상태 동기화: 서버에서 상태를 바꾸면 모든 클라이언트가 이 값을 가집니다.
    [SyncVar(hook = nameof(OnStateChanged))]
    public bool isWorking = false;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (anim != null) anim.SetBool(animParamName, isWorking);
    }

    // ★ LeverClick.cs에서 Worker 역할일 때 이 함수를 호출합니다. ★
    [Command(requiresAuthority = false)]
    public void CmdToggleWork()
    {
        // 서버에서만 상태를 변경
        isWorking = !isWorking;
        Debug.Log($"[서버] Worker 요청: 작동 상태 변경 -> {isWorking}");

        // ★ [핵심 추가] 상태가 바뀔 때마다 MachineManager에게 확인 요청 ★
        if (manager != null)
        {
            manager.CheckAllMachinesStatus();
        }
    }

    // 상태(isWorking)가 변하면 모든 클라이언트에서 실행됩니다.
    void OnStateChanged(bool oldState, bool newState)
    {
        Debug.Log($"[애니메이션] 상태 동기화: {oldState} -> {newState}");

        if (anim != null)
        {
            // 동기화된 상태에 따라 애니메이션을 제어합니다.
            anim.SetBool(animParamName, newState);
        }
    }
}