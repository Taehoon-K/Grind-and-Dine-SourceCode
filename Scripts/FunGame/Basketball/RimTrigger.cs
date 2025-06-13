using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimTrigger : MonoBehaviour
{
    [SerializeField] private Transform ballCenter; // 림 중심 Transform
    [SerializeField] private Transform rimCenter; // 림 중심 Transform
    [SerializeField] private float rimRadius = 0.3f; // 림의 반경 (림 크기에 맞게 조정)

    private void OnTriggerEnter(Collider other)
    {
        // 1. 트리거에 공이 들어왔는지 확인
        if (other.CompareTag("Bell"))
        {
            Rigidbody ballRigidbody = other.transform.GetComponentInParent<Rigidbody>();

            // 2. 공의 속도가 아래 방향인지 확인
            if (ballRigidbody.velocity.y < 0) // Y축 속도가 음수인지 확인
            {

                Debug.Log("골인!");
                HandleGoal();
            }
            else
            {
                Debug.Log("공이 아래로 떨어지지 않았습니다.");
            }
        }
    }

    private void HandleGoal()
    {
        BasketBallManager.instance.IsGoal();
    }
}
