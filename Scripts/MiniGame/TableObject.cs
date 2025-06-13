using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableObject : MonoBehaviour
{
    protected bool subOn = false; //���� ���� ����
    public bool SubOn { get { return subOn; } set { subOn = value; } }
    protected bool drinkOn = false; //����� ���� ����
    public bool DrinkOn { get { return drinkOn; } set { drinkOn = value; } }
    protected bool mainOn = true;
    protected int mMenu, sMenu, dMenu;  //���߿� �ο� �� �ʱ�ȭ ������ �־�ߵ�,eattiming�� �ʱ�ȭ
    public int myindex; //�ڱ� ���̺� ��ȣ ����

    private bool isSubPlate = false; //�̰� ���������� ���������� ���� �ΰ�
    [SerializeField]
    protected GameObject dirtyPlate; //�ٸ����� Ű�� ������͵�

    [SerializeField]
    protected Image images_Gauge; //ġ��� ������
    [SerializeField]
    protected GameObject images_Clean; //ġ��� ���϶� Ű�� ������ �̹���
    //[SerializeField]
    //private GameObject Fx_bubble; //ġ��� ���϶� Ű�� ��ƼŬ �̹���
    protected bool isClean = false;  //ġ���������

    protected float fillSpeed = 0.5f; // �������� ä������ �ӵ�
    protected float fillAmount; // ������ ä���� ����


    [SerializeField]
    private GameObject trayWash;

    public GameObject[] drinkObject; //���� �޽� �״ٲ��� �뵵

    public void MenuNumber(int m,int s, int d)
    {
        mMenu = m;
        sMenu = s;
        dMenu = d;
    }

    public bool CheckTablePlate(int index2) //���� �������Ҷ� üũ ,���ؽ��� ����ִ� ���� id
    {
        if (index2 == mMenu && mainOn)
        {
            mainOn = false;
            EatGood();
            SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //���׼� �߰�
            return true;
        }else if (index2 == sMenu && subOn)
        {
            subOn = false;
            isSubPlate = true;
            EatGood();
            SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //���׼� �߰�
            return true;
        }
        else
        {
            return false;
        }    
    }


    public virtual bool CheckTableDrink(int index) //����� üũ, �ε����� ����ִ� ����� ���̵�
    {
        if (DrinkOn && TransfDrink(index)==dMenu) //����� �� ��ġ�ϸ�
        {
            DrinkOn = false;
            EatGood();
            return true;
        }
        return false;
    }


    private int TransfDrink(int n)
    {
        switch (n)
        {
            case 50:
                return 0;
            case 51:
                return 1;
            case 52:
                return 2;
            default:
                return -1;
        }
    }

    protected void EatGood()
    {
        if (!subOn && !mainOn && !drinkOn)
        { //�޴� �� ������
            Debug.Log("eat");
            ChiTableManager.instance.HumanoidEat(myindex); //chitable �Ŵ��� ���ļ� �޸ӳ��̵� ��Ʈ�ѷ��� �Լ� ����
        }
    }
    public virtual void EatFinish(bool angry = false) //�ٸ԰� ������ ���÷� �ٲٱ�
    {
        Debug.Log("Eatfinish ȣ�⤩��������������������������������");
        /* Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
         if (isSubPlate)
         {
             Destroy(gameObject.transform.GetChild(1).GetChild(0).gameObject);
         }//���� �ִ� ������Ʈ�� ����*/

        if (gameObject.transform.childCount > 0 && gameObject.transform.GetChild(0).childCount > 0)
        {
            if (gameObject.transform.GetChild(0).GetChild(0).gameObject != null)
            {
                Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
            }
        }

        if (gameObject.transform.childCount > 1 && gameObject.transform.GetChild(1).childCount > 0)
        {
            if (gameObject.transform.GetChild(1).GetChild(0).gameObject != null)
            {
                Destroy(gameObject.transform.GetChild(1).GetChild(0).gameObject);
            }
        }

        dirtyPlate.SetActive(true);
        CalculMoney();
        SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //���Ҹ� �߰�
        gameObject.layer = 15; //ī�޶��̿����� ����ĳ��Ʈ �ٲٱ�
        
    }
    public void TableAngry() //ȭ���� ������ ���̺� �������ϴ¿뵵
    {
        dirtyPlate.SetActive(true);
    }
    public bool IsClean
    {
        get { return isClean; }
        set
        {
            images_Clean.SetActive(value);
            //Fx_bubble.SetActive(value);
            isClean = value;
        }
    }
    private void Update()
    {
        if (isClean)
        {
            //Debug.Log(fillAmount);
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp01(fillAmount); // fillAmount ���� 0�� 1 ���̿� �ֵ��� Ŭ����
            images_Gauge.fillAmount = fillAmount; // ������ �̹����� ä���� ���� �ݿ�
            if (fillAmount == 1)
            {
                fillAmount = 0; //������ ó������
                Reset1(); //���� �� ���ְ� �ʱ�ȭ
            }
        }
    }

    public virtual void Reset1()
    {
        gameObject.layer = 14;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        mainOn = true;
        isSubPlate = false;
        dirtyPlate.SetActive(false);
        //���̺�Ŵ��� ������Ʈ Ű��
        ChiTableManager.instance.TableOnAgain(myindex);

        drinkObject[dMenu].SetActive(false); //��� �޽� ����

    }

    protected virtual void CalculMoney()
    {
        if (isSubPlate)
        {
            int a = Random.Range(11, 21) * 100 + 1500;
            ChiTableManager.instance.AddTip(a);
        }
        else
        {
            int a = Random.Range(1, 11) * 100 + 1500;
            ChiTableManager.instance.AddTip(a);
        }

        /*int tip = Random.Range(1, 21);
        if (StatusManager.instance != null && GetLuckBasedBoolean())
        {
            int a = Random.Range(50, 101) * 10;
            ChiTableManager.instance.AddTip(a);
        }*/
    }
    private bool GetLuckBasedBoolean()
    {
        int luck = StatusManager.instance.GetLuckLevel();
        // �� ��ġ�� Ȯ���� ��ȯ (-10 �� 25%, 0 �� 50%, 10 �� 75%)
        float probability = 0.5f + (luck * 0.025f);

        // 0 ~ 1 ���� �� ���� �� Ȯ���� ��
        return Random.value < probability;
    }

}
