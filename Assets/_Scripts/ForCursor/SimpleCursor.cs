using UnityEngine;

public class SimpleCursor : OVRCursor
{
    // 커서가 레이저를 따라다니게 하는 필수 함수
    public override void SetCursorRay(Transform ray) { }

    // 레이저가 물체에 닿았을 때 커서 위치를 옮겨주는 함수
    public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
    {
        // 내 위치(Sphere)를 레이저가 닿은 곳(dest)으로 이동!
        transform.position = dest;
        // 표면에 착 달라붙게 회전
        transform.rotation = Quaternion.LookRotation(normal);
    }
}