using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 20, 0); // 1초에 y축으로 20도 회전

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
