using UnityEngine;
using System.Collections;

public class HighlightEffect : MonoBehaviour
{
    private Color originalColor; // 머터리얼 대신 '색깔'을 기억합니다
    private Renderer myRenderer;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        // 시작할 때 원래 색깔을 기억해둡니다 (예: 흰색)
        originalColor = myRenderer.material.color;
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        // 1. 빨간색으로 변경
        myRenderer.material.color = Color.red;

        // 2. 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        // 3. 아까 기억해둔 원래 색깔로 복구
        myRenderer.material.color = originalColor;
    }
}