using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP.InventorySystem;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private NPC owner;

    public List<ItemData> shopItems;
    public string toggleFilter; //���͸��� ��ü��
    //public Inventory _inventory;
    /*
    public static void Purchase(ItemData item, int quantity) //static ��
    {
        int totalCost = item.Price * quantity;

        if(PlayerStats.Money >= totalCost)
        {
            PlayerStats.Spend(totalCost);
            //�κ��丮�� ������ �߰�
            _inventory.Add(item, quantity);
        }
    }
    */
    public void PickUp()
    {
        //if (!IsStoreManned()) return;

        UIManager.instance.OpenShop(shopItems, toggleFilter);
    }

    private bool IsStoreManned()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4);
        foreach(Collider col in colliders)
        {
           
            if (col.tag != "Npc") continue;

            InteractableCharacter npcInteractable = col.gameObject.GetComponent<InteractableCharacter>();
            if (npcInteractable == null) continue;
            if (npcInteractable.npc.CharacterName() == owner.CharacterName())
            {
                return true;
            }
        }
        return false;
    }
}
