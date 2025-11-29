using UnityEngine;
using Mirror;

public class ManualSystem : NetworkBehaviour
{
    // 어디서든 쉽게 접근하기 위한 싱글톤
    public static ManualSystem Instance;

    [Header("매뉴얼 페이지 목록")]
    // 여기에 아까 만든 패널들을 등록할 겁니다.
    public GameObject[] manualPages;

    private void Awake()
    {
        Instance = this;
    }

    // 1. 전문가가 호출 (서버로 요청)
    [Command(requiresAuthority = false)]
    public void CmdShowManual(int pageIndex)
    {
        // 서버가 받아서 모든 클라이언트(작업자)에게 뿌림
        RpcShowManual(pageIndex);
    }

    // 2. 작업자 화면에서 실행
    [ClientRpc]
    void RpcShowManual(int pageIndex)
    {
        Debug.Log($"[매뉴얼] {pageIndex}번 페이지 표시 요청됨");

        // 일단 켜져 있는 모든 매뉴얼을 끕니다 (초기화)
        for (int i = 0; i < manualPages.Length; i++)
        {
            if (manualPages[i] != null)
                manualPages[i].SetActive(false);
        }

        // 요청받은 번호가 유효하면 그 페이지만 켭니다
        if (pageIndex >= 0 && pageIndex < manualPages.Length)
        {
            if (manualPages[pageIndex] != null)
                manualPages[pageIndex].SetActive(true);
        }
    }

    // (선택 기능) 매뉴얼 닫기
    [Command(requiresAuthority = false)]
    public void CmdCloseAll()
    {
        RpcShowManual(-1); // -1번은 없으니까 로직상 다 꺼짐
    }
}