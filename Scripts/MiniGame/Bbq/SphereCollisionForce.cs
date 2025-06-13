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
        // 자신의 Rigidbody를 미리 가져오기
        rb = GetComponent<Rigidbody>();
        GetComponent<SphereCollider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        HumanoidControllerSub parentObj = GetComponentInParent<HumanoidControllerSub>();

        if (rb != null && collision.gameObject.CompareTag("ItemActive"))
        {
            parentObj.Dead(true);
            // 충돌한 물체의 속도 방향 가져오기 (충돌 상대가 날아온 방향)
            Vector3 incomingDirection = -collision.relativeVelocity.normalized; // 반대 방향

            // 그 방향으로 forceMultiplier 값을 곱한 힘을 자기 자신에게 가한다.
            rb.AddForce(incomingDirection * forceMultiplier, ForceMode.Impulse);
        }
    }
}
