using UnityEngine;
using UnityEngine.UI;

public class Debug_Button : MonoBehaviour
{
    public bool isDebugActive = false;

    // [추가] 버튼 컴포넌트를 가져오기 위한 변수
    private Button myButton;

    // [추가] 시작할 때 버튼 컴포넌트를 찾아둡니다.
    private void Start()
    {
        myButton = GetComponent<Button>();
    }

    // [추가] 매 프레임 변수 상태를 확인합니다.
    private void Update()
    {
        // 변수가 true가 되면
        if (isDebugActive)
        {
            // 버튼에 연결된 OnClick 이벤트를 강제로 실행합니다.
            if (myButton != null)
            {
                myButton.onClick.Invoke();
                Debug.Log("Debug_Button: 변수 체크로 인해 버튼이 강제 클릭되었습니다.");
            }

            // [중요] 무한 클릭을 방지하기 위해 다시 false로 되돌립니다.
            isDebugActive = false;
        }
    }

    // 기존 OnClickDebugButton 함수는 버튼이 클릭될 때 실행되는 '대상'이므로 그대로 두거나,
    // 만약 이 스크립트 자체가 기능을 갖고 있다면 유지합니다.
}