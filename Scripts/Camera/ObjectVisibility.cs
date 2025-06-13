using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRenderer;
using UnityEngine;

public class ObjectVisibility : MonoBehaviour
{
    private Camera mainCamera;
    private float maxDistance = 100f; // 최대 거리 100 설정
    private bool hasBeenVisible = false; // 오브젝트가 이미 보였는지 여부

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 이미 보인 오브젝트는 더 이상 검사하지 않음
        if (hasBeenVisible)
        {
            return;
        }

        // 카메라와 오브젝트 간의 거리 계산
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

        // 거리 100 이하인 경우만 활성화
        bool isWithinDistance = distance <= maxDistance;

        /*// 화면에 보이는지 여부와 거리 조건을 함께 확인
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool isVisible = screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;*/

        // 화면에 보이고, 거리 조건을 만족하는 경우만 활성화
        if (isWithinDistance)
        {
            gameObject.SetActive(true);
            hasBeenVisible = true; // 한 번 보이면 상태 변경
        }
    }
}
