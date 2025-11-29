using UnityEngine;
using Mirror;

public class HighlightSystem : NetworkBehaviour
{
    // 어디서든 쉽게 접근하기 위해 싱글톤 패턴 사용
    public static HighlightSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    // 1. 전문가가 호출하는 명령 (클라이언트 -> 서버)
    // [Command]는 Mirror의 기능이므로 지우면 안 됩니다!
    [Command(requiresAuthority = false)]
    public void CmdHighlight(string objectID)
    {
        Debug.Log($"[Server] 하이라이트 요청 받음: {objectID}");

        // 2. 서버가 모든 클라이언트에게 전파 (서버 -> 모든 클라이언트)
        RpcHighlight(objectID);
    }

    // 3. 작업자(모두)의 화면에서 실제 효과 실행
    // [ClientRpc]는 Mirror의 기능이므로 지우면 안 됩니다!
    [ClientRpc]
    void RpcHighlight(string objectID)
    {
        Debug.Log($"[Client] 하이라이트 신호 수신: {objectID}");

        // 이름으로 오브젝트 찾기
        GameObject target = GameObject.Find(objectID);
        if (target != null)
        {
            // 효과 재생
            HighlightEffect effect = target.GetComponent<HighlightEffect>();
            if (effect != null) effect.Play();
        }
    }
}