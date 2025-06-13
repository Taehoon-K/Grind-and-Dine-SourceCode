using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayGame : MonoBehaviour
{
    public string simpleTalk; //������ ��ȭ��
    public void PickUp()
    {
        //if (!IsStoreManned()) return;

        //UIManager.instance.OpenGame(gameNumber);
    }

    private bool IsStoreManned()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Extra")) 
            { 
                return true;
            }
        }
        return false;
    }
}
