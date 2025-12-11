using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    void Awake()
    {
        // 씬이 바뀌어도 나를 파괴하지 마라!
        DontDestroyOnLoad(this.gameObject);
    }
}