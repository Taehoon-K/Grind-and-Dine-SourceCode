using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PP.InventorySystem;


public class TradeTester : MonoBehaviour
{
    public Inventory _inventory;

    public ItemData[] _itemDataArray;
    
    [Space(12)]
    public Button _removeAllButton;

    [Space(8)]
    public Button _AddArmorA1;
    public Button _AddArmorB1;
    public Button _AddSwordA1;
    public Button _AddSwordB1;
    public Button _AddPortionA1;
    public Button _AddPortionA50;
    public Button _AddPortionB1;
    public Button _AddPortionB50;

    private void Start()
    {
        /*if (_itemDataArray?.Length > 0)
        {
            for (int i = 0; i < _itemDataArray.Length; i++)
            {
                _inventory.Add(_itemDataArray[i], 3);

                if(_itemDataArray[i] is CountableItemData)
                    _inventory.Add(_itemDataArray[i], 255);
            }
        }*/

        _removeAllButton.onClick.AddListener(() =>
        {
            int capacity = _inventory.Capacity;
            for(int i = 0; i < capacity; i++)
                _inventory.Remove(i);
        });

        _AddArmorA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[0]));
        _AddArmorB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[1]));

        _AddSwordA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[2]));
        _AddSwordB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[3]));

        _AddPortionA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[4]));
        _AddPortionA50.onClick.AddListener(() => _inventory.Add(_itemDataArray[4], 50));
        _AddPortionB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[5]));
        _AddPortionB50.onClick.AddListener(() => _inventory.Add(_itemDataArray[5], 50));
    }

}