using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats //儀 淫軒,戚硯,貝戚亀亀 淫軒
{
    public static int Money { get; private set; }
    public const string CURRENCY = "\u20A9";
    public static int TotalMoney { get; private set; }

    //鮭
    public static int TotalDept { get; private set; }
    public static int WeekToDept { get; private set; }
    public static int FailDept { get; private set; } //鮭 公葦精 判呪, 3腰 公葦生檎 易神獄

    public static string PlayerName{get; private set;}
    public static int Difficulty { get; private set; } // 0:習崇, 1: 左搭 2: 嬢形崇 3: 古酔 嬢形崇
    public static bool IsWoman { get; private set; } //虹什檎 害切 闘欠檎 食切

    public static bool needToSpawnSean = false;

    public static void Spend(int cost) //社搾
    {
        Debug.Log("儀 託姶 けけけけけけけけけけけけ "+ cost);
        if(cost > Money)
        {
            Debug.LogError("Player do not have money");
            return;
        }
        Money -= cost;
        if(cost > 0)
        {
            UIManager.instance.MinusMoney(cost);  //儀 蟹亜澗 反引
        }
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //亜痕煽舌社拭 儀 痕呪 左鎧奄
    }
    public static void Earn(int income)
    {
        Money += income;
        TotalMoney += income;
        if(income > 0)
        {
            UIManager.instance.AddMoney(income); //儀 級嬢神澗 反引
        }
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //亜痕煽舌社拭 儀 痕呪 左鎧奄
    }
    public static int GetMoney()
    {
        return Money;
    }
    public static void LoadMoney(int cost) //室戚崎 稽球獣 儀 稽球
    {
        Money = cost;
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //亜痕煽舌社拭 儀 痕呪 左鎧奄
    }

    //戚硯 淫軒
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
    public static void LoadTotalMoney(int cost) //室戚崎 稽球獣 儀 稽球
    {
        TotalMoney = cost;
        UIManager.instance.RenderPlayerStats();
    }

    #region Dept
    //鮭
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

    public static bool Initilaze() //析爽析 走概聖凶 鮭 装亜 板 析爽析鮭 段奄鉢
    {
        //戚腰爽 鮭 害紹澗走 伊紫 坪球 蓄亜拝依
        if(WeekToDept == 0)
        {
            TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //鮭 装亜
            WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);  //戚腰爽 鮭 仙持失

            return true;
        }
        else
        {
            return false;
        }
    }
    public static void SpendLastDept(bool isNull) //鮭戦戚 達焼尽聖 獣 鮭 葦紹生檎, 闘欠析 獣 戚腰 爽 鮭 巷反稽 梅聖凶績
    {
        if (!isNull) //幻鉦 葦精暗虞檎
        {
            TotalDept -= WeekToDept;
        }   
        TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //鮭 装亜
        WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);  //戚腰爽 鮭 仙持失
        StatusManager.instance?.RenderDept();
    }
    public static void TurnoverLastDept() //鮭戦戚 達焼尽聖 獣 陥製爽稽 角奄檎
    {
        TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //鮭 装亜
        WeekToDept += Mathf.RoundToInt(TotalDept * 0.04f);  //煽腰爽 鮭 擾嬢辞 
        StatusManager.instance?.RenderDept();
    }


    public static void DeptInitilaze() //惟績 獣拙拝凶 鮭 竺舛馬奄
    {
        Money = 0; //笹 竺舛 獣 儀亀 段奄鉢
        TotalMoney = 0;
        TotalDept = (GetDiffy() + 1) * 1000000;
        WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);
        StatusManager.instance?.RenderDept();
    }

    public static void SpendDept(int cost) //鮭葦澗 敗呪,覗繋覗闘拭辞 床澗 遂亀
    {
        TotalDept -= cost;
        WeekToDept = Mathf.Max(0, WeekToDept - cost); //0焼掘稽 照恭嬢走惟
        StatusManager.instance?.RenderDept();
    }
}
