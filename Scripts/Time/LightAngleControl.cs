using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAngleControl : MonoBehaviour
{
    // Directional Light�� ������ ������ �� ����� �����Դϴ�.
    public Light directionalLight;

    // Update �Լ��� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // Directional Light�� ���� ȸ�� ������ �����ɴϴ�.
        float rotationX = transform.rotation.eulerAngles.x;
        // ���� ȸ�� ������ 180�� �̻��̶�� (��, ���� �Ʒ��� ���ϴ� ���)
        if (rotationX > 270f && rotationX <360f)
        {
            // ������ 0���� �����մϴ�.
            directionalLight.intensity = 0f;
        }
        else
        {
            // �׷��� �ʴٸ� �⺻ ������ �����մϴ�. (���⼭�� 1.0f�� ����)
            directionalLight.intensity = 1.0f;
        }
    }
}
