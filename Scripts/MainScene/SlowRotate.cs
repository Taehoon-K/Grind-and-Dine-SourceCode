using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 20, 0); // 1�ʿ� y������ 20�� ȸ��

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
