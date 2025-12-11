using UnityEngine;
using Mirror;
using UnityEngine.EventSystems; // 클릭 감지용

public class FactoryMachine : NetworkBehaviour, IPointerClickHandler
{
    [Header("설정")]
    public Animator anim; // 기계의 애니메이터 연결
    public string myAnimationName = "Work"; // 실행할 애니메이션 이름 (Animator창에 있는 박스 이름)

    // 네트워크 변수 (모든 사람 화면에서 상태 동기화)
    // hook: 이 변수가 바뀌면 OnStateChanged 함수를 자동 실행해라!
    [SyncVar(hook = nameof(OnStateChanged))]
    public bool isWorking = false;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    // 클릭했을 때 실행됨 (전문가/작업자 구분)
    public void OnPointerClick(PointerEventData eventData)
    {
        // 내 역할 가져오기
        UserRole myRole = CustomNetworkManager.Instance.myRole;

        // 1. 전문가가 클릭함 -> 하이라이트 켜기
        if (myRole == UserRole.Expert)
        {
            // 팀원이 만든 시스템 호출 (이름으로 찾아서 명령)
            if (HighlightSystem.Instance != null)
                HighlightSystem.Instance.CmdHighlight(this.gameObject.name);
        }
        // 2. 작업자가 클릭함 -> 작동 멈춤/시작 토글
        else if (myRole == UserRole.Worker)
        {
            // 서버에 "상태 바꿔줘!" 요청
            CmdToggleWork();
        }
    }

    // [서버 명령] 작업 상태 뒤집기 (켜짐<->꺼짐)
    [Command(requiresAuthority = false)] // 권한 없어도 클릭 가능하게 설정
    void CmdToggleWork()
    {
        isWorking = !isWorking; // true면 false로, false면 true로
    }

    // [상태 변경 시 자동 실행] 애니메이션 제어
    void OnStateChanged(bool oldState, bool newState)
    {
        if (anim != null)
        {
            if (newState == true)
            {
                // 작동 시작!
                anim.Play(myAnimationName);
                anim.speed = 1; // 재생 속도 정상
            }
            else
            {
                // 작동 멈춤!
                // anim.Play("Idle"); // Idle로 보내거나
                anim.speed = 0; // 그냥 그 자리에서 얼음(일시정지) 시키려면 속도를 0으로
            }
        }
    }
}