using UnityEngine;

public class HomeButton : MonoBehaviour
{
    public void OnClickHome()
    {
        Debug.Log("[HomeButton] 홈으로 이동!");
        // SceneController가 있으면 역할 선택 화면으로 이동
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoToRoleSelect();
        }
    }
}