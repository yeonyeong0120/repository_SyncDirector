using UnityEngine;
using Mirror;
using System.Collections;

public class GoldMove : NetworkBehaviour
{
    // 이동 상태를 정의하는 Enum (내부적으로 사용)
    public enum MoveState { Stopped, MovingY, MovingX }

    [Header("골드바 이동 설정")]
    public float moveZDistance = -6.5f; // Y축 이동 거리 (현재 설정된 -6.5)
    public float moveXDistance = -3.5f; // X축 추가 이동 거리 (새로 추가된 -3.5)
    public float moveSpeed = 3.0f;    // 움직이는 속도

    [Header("폭발 설정")]
    public float explosionJumpHeight = 3.0f;
    public float explosionDelay = 0.5f;
    public GameObject explosionPrefab;

    // 내부 사용 변수
    private Vector3 initialPosition; // 씬 시작 시 위치
    private Vector3 targetPosition;  // 현재 단계의 목표 위치
    private bool hasExploded = false;

    // ★★★ 현재 이동 상태를 추적합니다 (네트워크 동기화는 필요 없음) ★★★
    private MoveState currentMoveState = MoveState.Stopped;

    void Start()
    {
        initialPosition = transform.position;
        // 초기에는 정지 상태
        currentMoveState = MoveState.Stopped;
    }

    // Worker 협업 완료 시 MachineManager가 호출
    [ClientRpc]
    public void RpcStartMoveOut()
    {
        if (currentMoveState == MoveState.Stopped && !hasExploded)
        {
            // 1. 시작 시 Y축 이동 목표 설정 후 이동 시작
            targetPosition = initialPosition + transform.up * moveZDistance; // Y축 이동
            currentMoveState = MoveState.MovingY;

            Debug.Log("골드바 1단계 (Y축) 이동 시작!");
        }
    }

    void Update()
    {
        // 서버만 움직임을 계산하고 Network Transform이 동기화합니다.
        if (!isServer || currentMoveState == MoveState.Stopped || hasExploded) return;

        // 1. 서버에서 위치를 목표를 향해 업데이트
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // 2. 목표 도달 확인
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            HandleTargetReached();
        }
    }

    // ★★★ 목표 도달 시 다음 단계를 처리하는 로직 ★★★
    void HandleTargetReached()
    {
        if (currentMoveState == MoveState.MovingY)
        {
            // 1단계(Y축) 완료 -> 2단계(X축) 시작

            // 현재 위치를 기준으로 X축 목표 위치 설정
            targetPosition = transform.position + transform.right * moveXDistance; // X축 이동
            currentMoveState = MoveState.MovingX;

            Debug.Log("골드바 2단계 (X축) 이동 시작!");
        }
        else if (currentMoveState == MoveState.MovingX)
        {
            // 2단계(X축) 완료 -> 폭발

            currentMoveState = MoveState.Stopped;
            hasExploded = true;

            RpcExplodeSequence(); // 폭발 시퀀스 시작
        }
    }

    [ClientRpc]
    void RpcExplodeSequence()
    {
        StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine()
    {
        // 폭발 점프 효과
        Vector3 jumpPosition = targetPosition + Vector3.up * explosionJumpHeight;
        transform.position = jumpPosition;

        yield return new WaitForSeconds(explosionDelay);

        DoExplosion();
    }

    void DoExplosion()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}