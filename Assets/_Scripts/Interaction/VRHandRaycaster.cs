using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VRVisualRaycaster : MonoBehaviour
{
    [Header("설정")]
    public float rayLength = 5.0f;
    public LayerMask layerMask = -1; // -1은 Everything (모두 감지)

    // 오큘러스 입력 설정 (오른손 기준)
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;

    private LineRenderer lineRenderer;

    void Start()
    {
        // 1. 라인 렌더러(레이저) 자동 세팅
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red; // 평소엔 빨간색
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) Debug.Log("검지 트리거 눌림!");
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)) Debug.Log("중지 그립 눌림!");
        if (OVRInput.GetDown(OVRInput.Button.One)) Debug.Log("A 버튼 눌림!");
        if (OVRInput.GetDown(OVRInput.Button.Two)) Debug.Log("B 버튼 눌림!");

        // 2. 레이저 쏘기
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, rayLength, layerMask);

        // 레이저 끝점 계산
        Vector3 endPosition = isHit ? hit.point : transform.position + (transform.forward * rayLength);

        // 3. 눈에 보이는 레이저 그리기
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);

        // 4. 충돌 감지 시 색깔 변경 (녹색)
        if (isHit)
        {
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;

            // 충돌한 물체가 LeverClick을 가지고 있는지 확인
            LeverClick target = hit.collider.GetComponent<LeverClick>();

            // 5. 버튼 입력 확인 (트리거 당김)
            // (One, Two 등 아무거나 눌러도 반응하게 Any로 테스트)
            if (OVRInput.GetDown(clickButton, controller) || OVRInput.GetDown(OVRInput.Button.Any))
            {
                if (target != null)
                {
                    target.Interact(); // 상호작용 실행
                }
            }
        }
        else
        {
            // 허공을 가리킬 땐 빨간색
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
    }
}