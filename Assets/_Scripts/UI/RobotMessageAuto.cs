using UnityEngine;
using TMPro;
using System.Collections; // 코루틴 사용을 위해 필요

public class RobotMessageAuto : MonoBehaviour
{
    [Header("연결할 텍스트")]
    public TextMeshProUGUI bubbleText;

    [Header("대사 목록")]
    [TextArea]
    public string[] messages;

    [Header("설정")]
    public float changeInterval = 2.0f; // 몇 초마다 바꿀지 설정

    private int currentIndex = 0;

    void Start()
    {
        // 시작하자마자 코루틴(타이머) 실행
        if (messages.Length > 0 && bubbleText != null)
        {
            // 첫 번째 대사 먼저 보여주기
            bubbleText.text = messages[0];
            StartCoroutine(AutoChangeMessage());
        }
    }

    // 2초마다 실행되는 루프
    IEnumerator AutoChangeMessage()
    {
        while (true) // 무한 반복
        {
            yield return new WaitForSeconds(changeInterval); // 설정한 시간만큼 대기

            // 다음 대사로 넘어가기
            currentIndex = (currentIndex + 1) % messages.Length;

            if (bubbleText != null)
                bubbleText.text = messages[currentIndex];
        }
    }
}