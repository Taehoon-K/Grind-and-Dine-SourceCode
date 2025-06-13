using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFront : MonoBehaviour
{
    [SerializeField] private Camera maincamera;
    private void Start()
    {
        maincamera = Camera.main;
    }
    void Update()
    {
        transform.LookAt(transform.position + maincamera.transform.rotation * Vector3.back,
            maincamera.transform.rotation * Vector3.up);
    }
}
