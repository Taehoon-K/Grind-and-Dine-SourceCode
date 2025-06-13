using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCamera : MonoBehaviour
{
    [SerializeField] GameObject cameras;

    private void OnEnable()
    {
        if (cameras != null)
        {
            cameras.SetActive(true);
        }

    }
    private void OnDisable()
    {
        if (cameras != null)
        {
            cameras.SetActive(false);
        }
        
    }
}
