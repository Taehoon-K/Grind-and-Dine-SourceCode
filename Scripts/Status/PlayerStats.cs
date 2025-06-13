using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats //�� ����,�̸�,���̵��� ����
{
    public static int Money { get; private set; }
    public const string CURRENCY = "\u20A9";
    public static int TotalMoney { get; private set; }

    //��
    public static int TotalDept { get; private set; }
    public static int WeekToDept { get; private set; }
    public static int FailDept { get; private set; } //�� ������ Ƚ��, 3�� �������� �׿���

    public static string PlayerName{get; private set;}
    public static int Difficulty { get; private set; } // 0:����, 1: ���� 2: ����� 3: �ſ� �����
    public static bool IsWoman { get; private set; } //������ ���� Ʈ��� ����

    public static bool needToSpawnSean = false;

    public static void Spend(int cost) //�Һ�
    {
        Debug.Log("�� ���� ������������������������ "+ cost);
        if(cost > Money)
        {
            Debug.LogError("Player do not have money");
            return;
        }
        Money -= cost;
        if(cost > 0)
        {
            UIManager.instance.MinusMoney(cost);  //�� ������ ȿ��
        }
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //��������ҿ� �� ���� ������
    }
    public static void Earn(int income)
    {
        Money += income;
        TotalMoney += income;
        if(income > 0)
        {
            UIManager.instance.AddMoney(income); //�� ������ ȿ��
        }
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //��������ҿ� �� ���� ������
    }
    public static int GetMoney()
    {
        return Money;
    }
    public static void LoadMoney(int cost) //���̺� �ε�� �� �ε�
    {
        Money = cost;
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //��������ҿ� �� ���� ������
    }

    //�̸� ����
    public static string GetName()
    {
        return PlayerName;
    }
    public static void SetName(string name)
    {
        PlayerName = name;
    }

    public static int GetDiffy()
    {
        return Difficulty;
    }
    public static void SetDiffy(int diffy) 
    {
        Difficulty = diffy;
    }
    public static bool GetSex()
    {
        return IsWoman;
    }
    public static void SetSex(bool sex)
    {
        IsWoman = sex;
    }
    public static int GetTotalMoney()
    {
        return TotalMoney;
    }
    public static void LoadTotalMoney(int cost) //���̺� �ε�� �� �ε�
    {
        TotalMoney = cost;
        UIManager.instance.RenderPlayerStats();
    }

    #region Dept
    //��
    public static int GetTotalDept()
    {
        return TotalDept;
    }
    public static void LoadTotalDept(int cost) 
    {
        TotalDept = cost;
    }
    public static int GetWeekDept()
    {
        return WeekToDept;
    }
    public static void LoadWeekDept(int cost) 
    {
        WeekToDept = cost;
        
    }
    public static int GetFailDept()
    {
        return FailDept;
    }
    public static void LoadFailDept(int cost) 
    {
        FailDept = cost;

    }
    #endregion

    public static bool Initilaze() //������ �������� �� ���� �� �����Ϻ� �ʱ�ȭ
    {
        //�̹��� �� ���Ҵ��� �˻� �ڵ� �߰��Ұ�
        if(WeekToDept == 0)
        {
            TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //�� ����
            WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);  //�̹��� �� �����

            return true;
        }
        else
        {
            return false;
        }
    }
    public static void SpendLastDept(bool isNull) //������ ã�ƿ��� �� �� ��������, Ʈ���� �� �̹� �� �� ��ȿ�� ��������
    {
        if (!isNull) //���� �����Ŷ��
        {
            TotalDept -= WeekToDept;
        }   
        TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //�� ����
        WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);  //�̹��� �� �����
        StatusManager.instance?.RenderDept();
    }
    public static void TurnoverLastDept() //������ ã�ƿ��� �� �����ַ� �ѱ��
    {
        TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //�� ����
        WeekToDept += Mathf.RoundToInt(TotalDept * 0.04f);  //������ �� �� 
        StatusManager.instance?.RenderDept();
    }


    public static void DeptInitilaze() //���� �����Ҷ� �� �����ϱ�
    {
        Money = 0; //�� ���� �� ���� �ʱ�ȭ
        TotalMoney = 0;
        TotalDept = (GetDiffy() + 1) * 1000000;
        WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);
        StatusManager.instance?.RenderDept();
    }

    public static void SpendDept(int cost) //������ �Լ�,������Ʈ���� ���� �뵵
    {
        TotalDept -= cost;
        WeekToDept = Mathf.Max(0, WeekToDept - cost); //0�Ʒ��� �ȶ�������
        StatusManager.instance?.RenderDept();
    }
}
