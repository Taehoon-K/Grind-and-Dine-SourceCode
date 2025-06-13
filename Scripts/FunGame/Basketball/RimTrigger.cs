using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimTrigger : MonoBehaviour
{
    [SerializeField] private Transform ballCenter; // �� �߽� Transform
    [SerializeField] private Transform rimCenter; // �� �߽� Transform
    [SerializeField] private float rimRadius = 0.3f; // ���� �ݰ� (�� ũ�⿡ �°� ����)

    private void OnTriggerEnter(Collider other)
    {
        // 1. Ʈ���ſ� ���� ���Դ��� Ȯ��
        if (other.CompareTag("Bell"))
        {
            Rigidbody ballRigidbody = other.transform.GetComponentInParent<Rigidbody>();

            // 2. ���� �ӵ��� �Ʒ� �������� Ȯ��
            if (ballRigidbody.velocity.y < 0) // Y�� �ӵ��� �������� Ȯ��
            {

                Debug.Log("����!");
                HandleGoal();
            }
            else
            {
                Debug.Log("���� �Ʒ��� �������� �ʾҽ��ϴ�.");
            }
        }
    }

    private void HandleGoal()
    {
        BasketBallManager.instance.IsGoal();
    }
}
