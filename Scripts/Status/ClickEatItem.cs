using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEatItem : MonoBehaviour
{
    [SerializeField] private int itemCode; //Ŭ�� �� �ٷ� ���� ������ �ڵ�
    

    public void InteractiveThisItem() //���� �� ���� Ŭ���ߴٸ�
    {
        gameObject.SetActive(false);
        FindObjectOfType<Throwing>().EatReadyCutscene(itemCode); //�Ա� ����
    }
}
