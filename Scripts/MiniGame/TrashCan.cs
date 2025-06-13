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
            Destroy(other.gameObject);//날아온거 지우기
            SoundManager.instance.PlaySound2D("trashcan");
        }
    }
}
