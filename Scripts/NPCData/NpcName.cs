using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP.InventorySystem;
using UnityEngine.Localization;

public class NpcName : MonoBehaviour
{
    [SerializeField] private LocalizedString mini_name_key;
    public ItemData _itemDataArray;


    public LocalizedString Mini_name_key
    {
        get
        {
            return mini_name_key; // 속성 값을 반환
        }
    }
}
