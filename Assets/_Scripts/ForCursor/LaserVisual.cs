using UnityEngine;

public class LaserVisual : MonoBehaviour
{
    private LineRenderer line;
    public float maxDistance = 3.0f;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        // 선이 월드 좌표계에서 그려지도록 설정
        line.useWorldSpace = true;
    }

    void Update()
    {
        // 시작점: 현재 내 손의 위치
        line.SetPosition(0, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            // 부딪혔다면? 그 위치(hit.point)까지만 그린다
            line.SetPosition(1, hit.point);
        }
        else
        {
            // 안 부딪혔다면? 최대 길이만큼 쭉 그린다
            line.SetPosition(1, transform.position + (transform.forward * maxDistance));
        }
    }
}