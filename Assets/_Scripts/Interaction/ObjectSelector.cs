using UnityEngine;
// Mirror는 아직 안 써도 되지만 선언해둡니다.

public class ObjectSelector : MonoBehaviour
{
    [Header("설정")]
    public float rayDistance = 5f;
    public LineRenderer laserLine; // (선택사항) 레이저 시각화

    // 중요: 레이저가 충돌한 물체가 이 태그를 달고 있어야 함
    private string targetTag = "Interactable";

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // ▼▼▼▼▼▼ [범인 색출 로그 추가] ▼▼▼▼▼▼
            // 레이저가 닿은 물체 이름을 무조건 출력합니다.
            Debug.Log($"레이저가 때린 것: {hit.collider.name}");
            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            if (hit.collider.CompareTag(targetTag))
            {
                if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetMouseButtonDown(0))
                {
                    // ... (기존 코드 유지) ...
                    hit.collider.GetComponent<HighlightEffect>().Play();
                }
            }
        }
    }
}