using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableObjectBbq : TableObject
{

    // TableState ��ü�� �����ϴ� ����Ʈ �ʱ�ȭ
    private List<TableState> sidemenu = new List<TableState>
    {
        new TableState(new bool[8] { false, false, true, false, false, true, false, false }, true),
        new TableState(new bool[8] { false, false, false, true, false, true, false, false }, true),
        new TableState(new bool[8] { false, true, false, false, true, false, true, true }, false),
        new TableState(new bool[8] { false, true, true, false, false, false, true, true }, false),
        new TableState(new bool[8] { true, false, true, false, false, false, false, false }, false),
        new TableState(new bool[8] { true, false, false, true, false, true, false, false }, true)
    };

    public bool isCharcoal; //�� �÷������� üũ
    private bool isAngry;
    private bool isSub; //����޴� �ִ���
    public GameObject[] meatObject; //��� �޽� �״ٲ��� �뵵
    //public GameObject[] drinkObject; //���� �޽� �״ٲ��� �뵵
    public GameObject fxOther; //����� ���� ������Ʈ
    public GameObject fireFx; //�� �ø��� �ҿö����
    private bool isTip; //�� �ٰ��� ����
    public bool CheckTableCharcoal() //�� �˻��ϴ� �ڵ�
    {
        if (!isCharcoal) //�� �ȿ÷���������
        {
            isCharcoal = true;
            fireFx.SetActive(true);
            ((BbqTableManager)ChiTableManager.instance).HumanoidOrder(myindex);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CheckTableMain(int index) //��� �˻��ϴ� �ڵ�
    {
        if (!isCharcoal) //�� �ȿ÷���������
        {
            return false;
        }
        else
        {
            if(index == mMenu && mainOn)
            {
                mainOn = false;
                EatGood();
                SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //���׼� �߰�
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public void CheckTableSub(bool[] ingre, bool isCook, bool isFireBowl, bool isBurn) //�ø� �˻� �ڵ�
    {
        if(isBurn || sidemenu[sMenu].isCook != isCook)
        {
            Angry();
            return;
        }

        // ��ġ�ϴ� ����� ������ ���� ���� ����
        int matchCount = 0;

        if(isFireBowl == sidemenu[sMenu].isCook) //���� �׸� ���� ������
        {
            matchCount++;
        }
        // ingre �迭�� �� ��Ҹ� ���� ���¿� ��
        for (int i = 0; i < ingre.Length; i++)
        {
            if (ingre[i] == sidemenu[sMenu].ingre[i])
            {
                matchCount++;
            }
        }

        // ��ġ�ϴ� ��� ������ ���� ���� �б�
        if (matchCount == 9) //��� ��ġ
        {
            subOn = false;
            EatGood();
            isTip = true;
            isSub = true; //���� ���� å��
            SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //���׼� �߰�
        }
        else if (matchCount >= 7)
        {
            subOn = false;
            EatGood();
            isSub = true; //���� ���� å��
            SoundManager.instance.PlaySound2D("hmmQuestion" + SoundManager.Range(1, 3)); //���׼� �߰�
        }
        else
        {
            // �� �� ���� ��ġ�� ���
            Angry();
        }

    }
    public override bool CheckTableDrink(int index) //����� üũ, �ε����� ����ִ� ����� ���̵�
    {
        if (DrinkOn && index == dMenu) //����� �� ��ġ�ϸ�
        {
            DrinkOn = false;
            EatGood();
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (isClean)
        {
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp01(fillAmount); // fillAmount ���� 0�� 1 ���̿� �ֵ��� Ŭ����
            images_Gauge.fillAmount = fillAmount; // ������ �̹����� ä���� ���� �ݿ�
            if (fillAmount == 1)
            {
                //fillAmount = 0; //������ ó������
                Reset1(); //���� �� ���ְ� �ʱ�ȭ
            }
        }
    }
    public override void Reset1()
    {
        gameObject.layer = 14;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        mainOn = true;
        dirtyPlate.SetActive(false);
        //���̺�Ŵ��� ������Ʈ Ű��
        ChiTableManager.instance.TableOnAgain(myindex);
        isAngry = false;
        isCharcoal = false;
        meatObject[mMenu].SetActive(false); //��� �޽� ����
        drinkObject[dMenu].SetActive(false); //��� �޽� ����
        fxOther.SetActive(false); //���� ���� ����
        fireFx.SetActive(false); //���� �� ����
        isTip = false;
        isSub = false;
        fillAmount = 0; //������ �ʱ�ȭ
        if (gameObject.transform.GetChild(0).childCount > 0 &&
                gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Bowl>() != null)
        {
            Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
        }

    }
    public override void EatFinish(bool angry = false) //�ٸ԰� ������ ���÷� �ٲٱ�
    {
        if (!isAngry && !angry) //ȭ �ȳ�����
        {
            meatObject[mMenu].SetActive(false);
            if (gameObject.transform.GetChild(0).childCount > 0 && 
                gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Bowl>() != null)
            {
                gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Bowl>().CleanIngre();
            }//�׸� �� ���� ���ֱ�
            dirtyPlate.SetActive(true);
            CalculMoney();
            //ChiTableManager.instance.Money += 3000; //�� 3000�� �߰�, ���߿� �ٲٱ�
            SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //���Ҹ� �߰�
        }
        gameObject.layer = 15; //ī�޶��̿����� ����ĳ��Ʈ �ٲٱ�      
    }

    public void Angry() //�ȸ԰� ����
    {
        isAngry = true;
        ((BbqTableManager)ChiTableManager.instance).HumanoidAngry(myindex);
    }

    protected override void CalculMoney() //�� ����ϴ� �Լ�
    {
        if (isSub & isTip)
        {
            int a = Random.Range(11, 21) * 100 + 2000;
            ChiTableManager.instance.AddTip(a);
        }
        else
        {
            int a = Random.Range(1, 11) * 100 + 2000;
            ChiTableManager.instance.AddTip(a);
        }
    }
}
