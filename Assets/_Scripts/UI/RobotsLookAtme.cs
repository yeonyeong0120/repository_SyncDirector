using UnityEngine;

public class SimpleLookAt : MonoBehaviour
{
    private Transform target;

    void Update()
    {
        // VR 카메라(내 눈)를 찾음
        if (target == null)
        {
            var cam = Camera.main; // 혹은 FindObjectOfType<Camera>();
            if (cam != null) target = cam.transform;
        }

        if (target != null)
        {
            // 타겟을 바라보되, Y축(위아래) 회전은 억제하고 싶다면 아래 주석 해제
            // Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            // transform.LookAt(targetPos);
            // 사용자 눈높이랑 맞춤,,
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.LookAt(targetPos);

            //transform.LookAt(target); // 그냥 쳐다보기
            transform.Rotate(0, 180, 0);
        }
    }
}