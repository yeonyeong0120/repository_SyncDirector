using UnityEngine;

public class AutoMove : MonoBehaviour
{
    [Header("이동 설정")]
    public Vector3 moveOffset = new Vector3(0, 2.0f, 0); // 이동할 방향과 거리 (기본: 위로 2칸)
    public float speed = 3.0f;  // 이동 속도

    private Vector3 targetPosition; // 도착할 목표 지점

    void Start()
    {
        // 게임이 시작되자마자 목표 위치를 계산해둡니다.
        // 현재 위치 + 설정한 거리
        targetPosition = transform.position + moveOffset;
    }

    void Update()
    {
        // 현재 위치가 목표 위치와 다르면 계속 이동합니다.
        if (transform.position != targetPosition)
        {
            // MoveTowards: 현재 위치에서 목표 위치로 부드럽게 이동시키는 함수
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}