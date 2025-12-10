using UnityEngine;
using TMPro; // 텍스트 프로 사용
using UnityEngine.EventSystems; // 클릭 감지용

public class RobotMessageChanger : MonoBehaviour, IPointerClickHandler
{
    [Header("연결할 텍스트")]
    public TextMeshProUGUI bubbleText; // 말풍선 텍스트(TMP)를 여기에

    [Header("대사 목록")]
    [TextArea] // 인스펙터에서 줄바꿈 가능하게
    public string[] messages; // 보여줄 대사들을 여기에 적으세요

    private int currentIndex = 0;

    // 로봇을 클릭했을 때 실행
    public void OnPointerClick(PointerEventData eventData)
    {
        if (messages.Length == 0 || bubbleText == null) return;

        // 다음 대사로 넘어가기 (순환)
        currentIndex = (currentIndex + 1) % messages.Length;

        // 텍스트 변경
        bubbleText.text = messages[currentIndex];

        // (선택사항) 로그 확인
        Debug.Log($"로봇 대사 변경: {messages[currentIndex]}");
    }
}