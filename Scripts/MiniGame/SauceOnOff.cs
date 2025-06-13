using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauceOnOff : MonoBehaviour
{
    [SerializeField]
    private GameObject obj;
    private ColliderSauce colter;
    private void Awake()
    {
        colter = obj.GetComponent<ColliderSauce>();
    }
    private void OnEnable()
    {
        //colter.IsCook = true;
        colter.canPlaced = false;
    }
    private void OnDisable()
    {
        colter.canPlaced = true;
        //colter.IsCook = false;
    }
}
