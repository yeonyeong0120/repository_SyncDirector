using UnityEngine;

public class TestButton : MonoBehaviour
{
    // 버튼을 누르면 실행될 함수
    public void OnClick()
    {
        // ManualSystem이 존재하면, 0번(첫 번째) 매뉴얼을 보여달라고 서버에 요청
        if (ManualSystem.Instance != null)
        {
            Debug.Log("[테스트버튼] 매뉴얼 팝업 요청 보냄");
            ManualSystem.Instance.CmdShowManual(0);
        }
        else
        {
            Debug.LogError("ManualSystem이 없습니다! NetworkManager에 붙어있는지 확인하세요.");
        }
    }
}