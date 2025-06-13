using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    bool flag;
    public GameObject doorL;
    public GameObject doorR;
    void Start()
    {
        flag = false;
    }

    void Update()
    {
        if (flag == true)
        {
            if (doorL.transform.localPosition.x >= -4.5f)
            {
                doorL.transform.Translate(-0.01f, 0, 0);
            }
            if (doorR.transform.localPosition.x < -0.283f)
            {
                doorR.transform.Translate(0.01f, 0, 0);
            }
        }
        if (flag == false)
        {
            if (doorL.transform.localPosition.x < -3.5f)
            {
                doorL.transform.Translate(0.01f, 0, 0);
            }
            if (doorR.transform.localPosition.x >= -1.15f)
            {
                doorR.transform.Translate(-0.01f, 0, 0);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            flag = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            flag = false;
        }
    }
}
