using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; // 클릭 감지용

public class VRKeyboardFix : MonoBehaviour, IPointerClickHandler
{
    private TMP_InputField inputField;
    private TouchScreenKeyboard overlayKeyboard;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    // 입력창을 클릭했을 때 실행됨
    public void OnPointerClick(PointerEventData eventData)
    {
        // 이미 키보드가 열려있다면 무시
        if (overlayKeyboard != null && overlayKeyboard.status == TouchScreenKeyboard.Status.Visible)
            return;

        // 시스템 키보드 열기 (숫자/기호 전용 패드)
        // IP 주소 입력이므로 NumberPad나 URL 타입을 쓰면 편합니다.
        overlayKeyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.DecimalPad);
    }

    void Update()
    {
        // 키보드가 열려있을 때, 타이핑한 내용을 UI에 실시간 반영
        if (overlayKeyboard != null && overlayKeyboard.status == TouchScreenKeyboard.Status.Visible)
        {
            inputField.text = overlayKeyboard.text;
        }
    }
}