using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ItemActive")
        {
            Destroy(other.gameObject);//���ƿ°� �����
            SoundManager.instance.PlaySound2D("trashcan");
        }
    }
}
