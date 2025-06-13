using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCoffee : MonoBehaviour
{
    [SerializeField]
    private BoxCollider coffeeColider,sideColider;

    public GameObject[] sideObject;

    private bool sideOn, coffeeOn; //���̵�� Ŀ�� �ִ��� ����
    public int sideId;
    private bool[] ingredient;
    private bool mixOn;

    public void SetCoffee(bool[] igre, bool mix, bool on)
    {
        coffeeOn = on;
        coffeeColider.enabled = on;
        ingredient = igre;
        mixOn = mix;
    }
    public bool GetCoffee() //�̹� Ŀ�� �ִ��� ����
    {
        return coffeeOn;
    }
    public void OffCoffee() //Ŀ�� �������� ȣ��Ǵ� �Լ�, ����ũ ���̾�
    {
        coffeeOn = false; ingredient = null;
        coffeeColider.enabled = false;
    }
    public void SetSide(int id, bool on)
    {
        sideOn = on;
        sideColider.enabled = on;
        sideId = id;

        if (!on)
        {
            OffSide();
        }
        //���̵�ڽ��ݶ��̴��� ��ũ�� ���������� �ؼ�, id�� �ٽ� ������ �� �ְ�
    }
    private void OffSide() //���̵� �� ����
    {
        for (int i = 0; i < sideObject.Length; i++) 
        {
            sideObject[i].SetActive(false);
        }
    }

    public void RingBell() //�Ϸ� �� ������ ȣ��� �Լ�
    {
        //���Ҹ� �߶�
        SoundManager.instance.PlaySound2D("bell"); //���� �Ҹ� ���
        //Ŀ�� ������ ����
        if (!coffeeOn)
        {
            CoffeeTableManager.instance.NoticeCreate("noCoffee_key");
            //���� �����ٴ� �ȳ���
            return;
        }
        //�˻� �ڵ�
        if (CoffeeTableManager.instance.CheckCoffee(ingredient, mixOn, sideOn, sideId))
        {
            CalculMoney(); //�� ���
            Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
            OffCoffee(); //Ŀ�� ����
            SetSide(0,false); //���̵� ����
        }
        
    }
    private void CalculMoney()
    {
        if (sideOn)
        {
            int a = Random.Range(1, 11) * 100 + 800;
            CoffeeTableManager.instance.AddTip(a);
        }
        else
        {
            int a = Random.Range(1, 8) * 100 + 800;
            CoffeeTableManager.instance.AddTip(a);
        }
        /*
        int tip = Random.Range(1, 101);
        if (StatusManager.instance != null && tip <= StatusManager.instance.GetStatus().luckyLevel)
        {
            int a = Random.Range(50, 101) * 10;
            CoffeeTableManager.instance.AddTip(a);
        }*/
    }
}
