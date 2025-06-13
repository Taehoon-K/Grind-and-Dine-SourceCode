using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshOnOff : MonoBehaviour
{
    [SerializeField]
    private GameObject obj;
    private ColliderInter colter;
    private void Awake()
    {
        colter = obj.GetComponent<ColliderInter>();
    }
    private void OnEnable()
    {
        colter.canPlaced = false;
    }
    private void OnDisable()
    {
        colter.canPlaced = true;
        colter.Initilaize();
    }
}
