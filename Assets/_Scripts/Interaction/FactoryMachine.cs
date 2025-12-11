using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class FactoryMachine : NetworkBehaviour, IPointerClickHandler
{
    [Header("설정")]
    public Animator anim;
    // ★ 주의: 애니메이터 창의 'Parameters' 탭에 이 이름으로 Bool을 만들어야 합니다!
    public string animParamName = "IsWorking";

    [SyncVar(hook = nameof(OnStateChanged))]
    public bool isWorking = false;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    // 클릭 감지 (여기가 반응 없으면 Raycaster 문제)
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"<color=cyan>[클릭 감지됨!] {gameObject.name}을 클릭했습니다.</color>");

        if (CustomNetworkManager.Instance == null)
        {
            Debug.LogError("NetworkManager가 없습니다!");
            return;
        }

        UserRole myRole = CustomNetworkManager.Instance.myRole;
        Debug.Log($"내 역할 확인: {myRole}");

        if (myRole == UserRole.Expert)
        {
            Debug.Log("전문가 권한: 하이라이트 요청 보냄");
            if (HighlightSystem.Instance != null)
                HighlightSystem.Instance.CmdHighlight(this.gameObject.name);
        }
        else if (myRole == UserRole.Worker)
        {
            Debug.Log("작업자 권한: 작동 토글 요청 보냄");
            CmdToggleWork();
        }
        else
        {
            Debug.LogWarning("역할이 None입니다. RoleSelect를 거쳐왔나요?");
        }
    }

    [Command(requiresAuthority = false)]
    void CmdToggleWork()
    {
        isWorking = !isWorking;
        Debug.Log($"[서버] 작동 상태 변경 -> {isWorking}");
    }

    // 상태가 변하면 애니메이션 제어
    void OnStateChanged(bool oldState, bool newState)
    {
        Debug.Log($"[애니메이션] 상태 변경: {oldState} -> {newState}");

        if (anim != null)
        {
            // Bool 파라미터로 제어 (가장 확실한 방법)
            anim.SetBool(animParamName, newState);
        }
    }
}