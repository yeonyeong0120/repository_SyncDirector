using UnityEngine;

public class ButtonMove : MonoBehaviour
{
    [Header("이동 설정")]
    public Vector3 moveOffset = new Vector3(0, 2.0f, 0); // 이동할 거리
    public float speed = 3.0f;

    private Vector3 targetPosition;
    private bool isMoving = false; // 버튼을 눌렀는지 확인하는 변수

    // 시작할 때는 아무것도 안 함 (목표 위치만 현재 위치로 잡아둠)
    void Start()
    {
        targetPosition = transform.position;
    }

    // ★ 이 함수를 버튼이랑 연결할 겁니다!
    public void ClickToMove()
    {
        // 버튼을 누르면 목표 위치를 설정하고 이동 스위치를 켭니다.
        targetPosition = transform.position + moveOffset;
        isMoving = true;
    }

    void Update()
    {
        // 스위치가 켜졌을 때만 이동
        if (isMoving && transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}