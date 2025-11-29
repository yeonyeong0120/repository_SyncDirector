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
        // 1. 컨트롤러 위치에서 앞방향으로 레이저 발사 준비
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 디버그용: 씬 화면에 빨간 선 그리기
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        // 2. 레이저 쏘기!
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // 3. 맞은 놈이 "Interactable" 태그를 달고 있는가?
            if (hit.collider.CompareTag(targetTag))
            {
                // 4. (테스트) 트리거를 당기거나 마우스를 클릭하면
                if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetMouseButtonDown(0))
                {
                    string objectID = hit.collider.gameObject.name;
                    Debug.Log($"[VR선택] 장비 인식됨: {objectID}");

                    // 여기에 나중에 "HighlightSystem.Instance.CmdHighlight(objectID)"를 넣을 겁니다.
                    // 지금은 로컬 테스트용으로 직접 효과 재생
                    hit.collider.GetComponent<HighlightEffect>().Play();
                }
            }
        }
    }
}