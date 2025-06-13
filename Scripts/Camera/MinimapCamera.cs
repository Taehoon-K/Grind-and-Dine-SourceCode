using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (플레이어)
    public Transform followYRotationTarget; // 회전값을 따를 대상 (예: 플레이어)
    public Vector3 offset = new Vector3(0, 20f, 0); // 높이 조정

    private void LateUpdate()
    {
        if (target == null) return;

        // 위치만 따라가고
        transform.position = target.position + offset;

        // Y축만 b 오브젝트의 회전을 따르고 나머지는 고정
        float yRotation = followYRotationTarget.eulerAngles.y;
        transform.rotation = Quaternion.Euler(90f, yRotation, 0f);
    }
}
