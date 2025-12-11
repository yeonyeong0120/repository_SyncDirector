using UnityEngine;
using Mirror;

// [주의] UserRole Enum 및 CustomNetworkManager는 프로젝트에 정의되어 있어야 합니다.

public class LeverClick : NetworkBehaviour
{
    [Header("연결 대상")]
    public HighlightEffect highlightEffect;
    public FactoryMachine factoryMachine;

    public void Interact()
    {
        // 1. 네트워크 연결 및 Spawning 확인
        // isSpawned 대신, isClient (클라이언트 연결 여부) 또는 isServer (서버/호스트 여부)를 사용합니다.
        // 클라이언트로 접속된 상태가 아니거나, CustomNetworkManager가 없으면 로컬 실행으로 간주합니다.
        if (!isClient || CustomNetworkManager.Instance == null)
        {
            if (highlightEffect != null) highlightEffect.Play();
            Debug.LogWarning("네트워크 연결 또는 매니저 없음. 로컬 하이라이트만 실행됨.");
            return;
        }

        // 2. 역할 정보 가져오기
        UserRole myRole = CustomNetworkManager.Instance.myRole;

        // 3. 서버에게 역할 정보와 함께 명령을 요청
        CmdExecuteAction(myRole);
    }

    // 클라이언트 -> 서버로 명령 요청 (클릭한 사람의 역할 정보를 같이 전달)
    [Command(requiresAuthority = false)]
    void CmdExecuteAction(UserRole role)
    {
        // 이 함수는 서버에서 실행됩니다.

        // 1. 시각적 피드백: Expert/Worker 누가 눌렀든 항상 깜빡임 동기화
        RpcSyncHighlight();

        // 2. 기능 실행: 오직 Worker 역할일 때만 기계 작동 (Expert의 기능과 분리)
        if (role == UserRole.Worker && factoryMachine != null)
        {
            // Mirror 네트워크 동기화를 위해 [Command] 함수를 호출합니다.
            factoryMachine.CmdToggleWork();
            Debug.Log("[서버] Worker 클릭 감지: 기계 작동 명령 전달 완료.");
        }
        else if (role == UserRole.Expert)
        {
            Debug.Log("[서버] Expert 클릭 감지: 하이라이트만 실행됨.");
        }
    }

    // 서버 -> 모든 클라이언트로 깜빡임 전달 (ClientRpc)
    [ClientRpc]
    void RpcSyncHighlight()
    {
        if (highlightEffect != null)
        {
            highlightEffect.Play();
        }
    }
}