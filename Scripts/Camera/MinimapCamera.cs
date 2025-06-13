using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform target; // ���� ��� (�÷��̾�)
    public Transform followYRotationTarget; // ȸ������ ���� ��� (��: �÷��̾�)
    public Vector3 offset = new Vector3(0, 20f, 0); // ���� ����

    private void LateUpdate()
    {
        if (target == null) return;

        // ��ġ�� ���󰡰�
        transform.position = target.position + offset;

        // Y�ุ b ������Ʈ�� ȸ���� ������ �������� ����
        float yRotation = followYRotationTarget.eulerAngles.y;
        transform.rotation = Quaternion.Euler(90f, yRotation, 0f);
    }
}
