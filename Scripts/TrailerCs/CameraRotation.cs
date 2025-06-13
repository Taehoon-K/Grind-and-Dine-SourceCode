using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform targetObject; // 회전할 오브젝트
    public Vector3 targetEulerAngles; // 목표 회전 방향 (예: (0, 180, 0))
    public float rotationSpeed = 30f; // 회전 속도 (도/초)

    private Quaternion targetRotation;

    void Start()
    {
        // 목표 회전값 저장
        targetRotation = Quaternion.Euler(targetEulerAngles);
    }

    void Update()
    {
        // 현재 회전 → 목표 회전으로 천천히 회전
        targetObject.rotation = Quaternion.RotateTowards(
            targetObject.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
