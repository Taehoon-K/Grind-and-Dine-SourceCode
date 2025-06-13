using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRenderer;
using UnityEngine;

public class ObjectVisibility : MonoBehaviour
{
    private Camera mainCamera;
    private float maxDistance = 100f; // �ִ� �Ÿ� 100 ����
    private bool hasBeenVisible = false; // ������Ʈ�� �̹� �������� ����

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // �̹� ���� ������Ʈ�� �� �̻� �˻����� ����
        if (hasBeenVisible)
        {
            return;
        }

        // ī�޶�� ������Ʈ ���� �Ÿ� ���
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

        // �Ÿ� 100 ������ ��츸 Ȱ��ȭ
        bool isWithinDistance = distance <= maxDistance;

        /*// ȭ�鿡 ���̴��� ���ο� �Ÿ� ������ �Բ� Ȯ��
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool isVisible = screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;*/

        // ȭ�鿡 ���̰�, �Ÿ� ������ �����ϴ� ��츸 Ȱ��ȭ
        if (isWithinDistance)
        {
            gameObject.SetActive(true);
            hasBeenVisible = true; // �� �� ���̸� ���� ����
        }
    }
}
