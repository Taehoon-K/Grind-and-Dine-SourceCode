using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats //돈 관리,이름,난이도도 관리
{
    public static int Money { get; private set; }
    public const string CURRENCY = "\u20A9";
    public static int TotalMoney { get; private set; }

    //빚
    public static int TotalDept { get; private set; }
    public static int WeekToDept { get; private set; }
    public static int FailDept { get; private set; } //빚 못갚은 횟수, 3번 못갚으면 겜오버

    public static string PlayerName{get; private set;}
    public static int Difficulty { get; private set; } // 0:쉬움, 1: 보통 2: 어려움 3: 매우 어려움
    public static bool IsWoman { get; private set; } //폴스면 남자 트루면 여자

    public static bool needToSpawnSean = false;

    public static void Spend(int cost) //소비
    {
        Debug.Log("돈 차감 ㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁ "+ cost);
        if(cost > Money)
        {
            Debug.LogError("Player do not have money");
            return;
        }
        Money -= cost;
        if(cost > 0)
        {
            UIManager.instance.MinusMoney(cost);  //돈 나가는 효과
        }
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //가변저장소에 돈 변수 보내기
    }
    public static void Earn(int income)
    {
        Money += income;
        TotalMoney += income;
        if(income > 0)
        {
            UIManager.instance.AddMoney(income); //돈 들어오는 효과
        }
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //가변저장소에 돈 변수 보내기
    }
    public static int GetMoney()
    {
        return Money;
    }
    public static void LoadMoney(int cost) //세이브 로드시 돈 로드
    {
        Money = cost;
        UIManager.instance.RenderPlayerStats();
        StatusManager.instance.UpdateStorageMoney(); //가변저장소에 돈 변수 보내기
    }

    //이름 관리
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
    public static void LoadTotalMoney(int cost) //세이브 로드시 돈 로드
    {
        TotalMoney = cost;
        UIManager.instance.RenderPlayerStats();
    }

    #region Dept
    //빚
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

    public static bool Initilaze() //일주일 지났을때 빚 증가 후 일주일빚 초기화
    {
        //이번주 빚 남았는지 검사 코드 추가할것
        if(WeekToDept == 0)
        {
            TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //빚 증가
            WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);  //이번주 빚 재생성

            return true;
        }
        else
        {
            return false;
        }
    }
    public static void SpendLastDept(bool isNull) //빚쟁이 찾아왔을 시 빚 갚았으면, 트루일 시 이번 주 빚 무효로 했을때임
    {
        if (!isNull) //만약 갚은거라면
        {
            TotalDept -= WeekToDept;
        }   
        TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //빚 증가
        WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);  //이번주 빚 재생성
        StatusManager.instance?.RenderDept();
    }
    public static void TurnoverLastDept() //빚쟁이 찾아왔을 시 다음주로 넘기면
    {
        TotalDept = Mathf.RoundToInt(TotalDept * 1.02f); //빚 증가
        WeekToDept += Mathf.RoundToInt(TotalDept * 0.04f);  //저번주 빚 얹어서 
        StatusManager.instance?.RenderDept();
    }


    public static void DeptInitilaze() //게임 시작할때 빚 설정하기
    {
        Money = 0; //빛 설정 시 돈도 초기화
        TotalMoney = 0;
        TotalDept = (GetDiffy() + 1) * 1000000;
        WeekToDept = Mathf.RoundToInt(TotalDept * 0.04f);
        StatusManager.instance?.RenderDept();
    }

    public static void SpendDept(int cost) //빚갚는 함수,프롬프트에서 쓰는 용도
    {
        TotalDept -= cost;
        WeekToDept = Mathf.Max(0, WeekToDept - cost); //0아래로 안떨어지게
        StatusManager.instance?.RenderDept();
    }
}
