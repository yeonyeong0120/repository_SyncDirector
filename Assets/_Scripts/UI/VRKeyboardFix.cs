using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class VRKeyboardFix : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private TMP_InputField inputField;
    private TouchScreenKeyboard overlayKeyboard;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    // [변경점] 클릭이 아니라 '선택(포커스)' 되었을 때 키보드를 엽니다.
    // InputField가 활성화되면 무조건 실행되므로 훨씬 확실합니다.
    public void OnSelect(BaseEventData eventData)
    {
        // 이미 키보드가 열려있으면 패스
        if (overlayKeyboard != null && overlayKeyboard.status == TouchScreenKeyboard.Status.Visible)
            return;

        // 키보드 강제 오픈 (숫자 패드)
        // 두 번째 인자: false (자동완성 끔), 세 번째: false (멀티라인 끔), 네 번째: false (비번 끔)
        overlayKeyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.DecimalPad, false, false, false);

        Debug.Log("VR 키보드 오픈 시도!");
    }

    // 다른 곳을 클릭해서 선택이 풀렸을 때
    public void OnDeselect(BaseEventData eventData)
    {
        if (overlayKeyboard != null)
        {
            overlayKeyboard = null;
        }
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