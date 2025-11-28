using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("씬 이름 설정")]
    public string introSceneName = "00_Intro";              // ⭐ 수정
    public string roleSelectSceneName = "01_RoleSelect";    // ⭐ 수정
    public string connectionSceneName = "02_Connection";     // ⭐ 수정
    public string collaborationSceneName = "03_Collaboration"; // ⭐ 수정

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GoToIntro()
    {
        Debug.Log("[SceneController] → 00_Intro 이동");
        SceneManager.LoadScene(introSceneName);
    }

    public void GoToRoleSelect()
    {
        Debug.Log("[SceneController] → 01_RoleSelect 이동");
        SceneManager.LoadScene(roleSelectSceneName);
    }

    public void GoToConnection()
    {
        Debug.Log("[SceneController] → 02_Connection 이동");
        SceneManager.LoadScene(connectionSceneName);
    }

    public void GoToCollaboration()
    {
        Debug.Log("[SceneController] → 03_Collaboration 이동");
        SceneManager.LoadScene(collaborationSceneName);
    }

    public void ReturnToRoleSelect(string errorMessage)
    {
        Debug.LogWarning($"[SceneController] 에러 발생: {errorMessage}");
        Debug.Log("[SceneController] → 01_RoleSelect으로 복귀");
        SceneManager.LoadScene(roleSelectSceneName);
    }
}