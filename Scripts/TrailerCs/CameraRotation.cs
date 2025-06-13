using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform targetObject; // ȸ���� ������Ʈ
    public Vector3 targetEulerAngles; // ��ǥ ȸ�� ���� (��: (0, 180, 0))
    public float rotationSpeed = 30f; // ȸ�� �ӵ� (��/��)

    private Quaternion targetRotation;

    void Start()
    {
        // ��ǥ ȸ���� ����
        targetRotation = Quaternion.Euler(targetEulerAngles);
    }

    void Update()
    {
        // ���� ȸ�� �� ��ǥ ȸ������ õõ�� ȸ��
        targetObject.rotation = Quaternion.RotateTowards(
            targetObject.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
