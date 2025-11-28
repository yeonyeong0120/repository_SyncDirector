using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [Header("설정")]
    public float displayDuration = 3f;

    [Header("UI 참조")]
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;
    public CanvasGroup canvasGroup; // 페이드 효과용

    void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // 1. 페이드 인 (선택사항)
        if (canvasGroup != null)
        {
            yield return StartCoroutine(FadeIn());
        }

        // 2. 로딩 바 애니메이션
        if (loadingBar != null)
        {
            float elapsed = 0f;
            while (elapsed < displayDuration)
            {
                elapsed += Time.deltaTime;
                loadingBar.value = elapsed / displayDuration;

                // 로딩 텍스트 점 애니메이션
                if (loadingText != null)
                {
                    int dots = Mathf.FloorToInt(elapsed * 3) % 4;
                    loadingText.text = "Loading" + new string('.', dots);
                }

                yield return null;
            }
        }
        else
        {
            // 로딩바 없으면 그냥 대기
            yield return new WaitForSeconds(displayDuration);
        }

        // 3. 씬 전환
        Debug.Log("[Intro] RoleSelectScene으로 이동");
        SceneController.Instance.GoToRoleSelect();
    }

    IEnumerator FadeIn()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = elapsed / duration;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}