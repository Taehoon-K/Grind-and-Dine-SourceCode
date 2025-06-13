using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCollisionForce : MonoBehaviour
{
    [SerializeField]
    private float forceMultiplier = 10f;
    private Rigidbody rb;

    void Start()
    {
        // �ڽ��� Rigidbody�� �̸� ��������
        rb = GetComponent<Rigidbody>();
        GetComponent<SphereCollider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        HumanoidControllerSub parentObj = GetComponentInParent<HumanoidControllerSub>();

        if (rb != null && collision.gameObject.CompareTag("ItemActive"))
        {
            parentObj.Dead(true);
            // �浹�� ��ü�� �ӵ� ���� �������� (�浹 ��밡 ���ƿ� ����)
            Vector3 incomingDirection = -collision.relativeVelocity.normalized; // �ݴ� ����

            // �� �������� forceMultiplier ���� ���� ���� �ڱ� �ڽſ��� ���Ѵ�.
            rb.AddForce(incomingDirection * forceMultiplier, ForceMode.Impulse);
        }
    }
}
