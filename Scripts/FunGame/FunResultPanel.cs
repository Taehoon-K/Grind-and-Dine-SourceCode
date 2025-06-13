using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FunResultPanel : MonoBehaviour
{

    protected int total;
    [Header("Numeric")]
    [SerializeField] private SimulationStat simulStat;
    [SerializeField] private int playTime;

    [Header("Text")]
   /* [SerializeField]
    protected TextMeshProUGUI totalText;
    [SerializeField]
    protected TextMeshProUGUI betMoneyText;
    [SerializeField]
    protected TextMeshProUGUI feeText;*/
    protected Status stats;

    [SerializeField]
    protected TextMeshProUGUI previousStatFatigue;
    [SerializeField]
    protected TextMeshProUGUI previousStatHungry;
    [SerializeField]
    protected TextMeshProUGUI previousStatAngry;
    [SerializeField] protected TextMeshProUGUI previousStatSad;
    [SerializeField] protected TextMeshProUGUI previousStatBored;
    [SerializeField]
    protected TextMeshProUGUI currentStatFatigue;
    [SerializeField]
    protected TextMeshProUGUI currentStatHungry;
    [SerializeField]
    protected TextMeshProUGUI currentStatAngry;
    [SerializeField] protected TextMeshProUGUI currentStatSad;
    [SerializeField] protected TextMeshProUGUI currentStatBored;

    [Header("FillImage")]
    [SerializeField]
    protected Image previousGraphFatigue;
    [SerializeField]
    protected Image currentGraphFatigue;
    [SerializeField]
    protected Image previousGraphHunger;
    [SerializeField]
    protected Image currentGraphHunger;

    [Header("Card")]
    [SerializeField]
    protected GameObject resultPanelConfirm; //결과창 패널 확인버튼
    [SerializeField]
    protected GameObject[] titlePanel; //작업 완료 아니면 실패 제목 띄우기

    [Header("Scene")]
    [SerializeField]
    public SceneTransitionManager.Location locationToSwitch;

    [Header("Perk")]
    [SerializeField]
    private GameObject bottomPerk;

    bool isWin;

    private void Start()
    {
        Cursor.visible = true;
    }

    public virtual void Render(bool quit = false)
    {
        if (YutnoriManager.instance != null)
        {
           // total = YutnoriManager.instance.money;
            stats = YutnoriManager.instance.stat;
        }
        else if (BasketBallManager.instance != null)
        {
           // total = BasketBallManager.instance.money;
            stats = BasketBallManager.instance.stat;
        }

        if (!quit) //이겼다면
        {
            titlePanel[0].SetActive(true); //작업 완료 이미지 띄우기

            /*int feeMoney = total * ((PlayerStats.Difficulty + 1) * 10) / 100;
            int finalMoney = total - feeMoney;
            betMoneyText.text = "\u20A9" + total.ToString("N0");
            feeText.text = "-\u20A9" + feeMoney.ToString("N0");
            totalText.text = "\u20A9" + finalMoney.ToString("N0");
            PlayerStats.Earn(finalMoney); //총 돈만큼 증가*/

            isWin = true;
        }
        else //졌을 때
        {
            titlePanel[1].SetActive(true); //작업 실패 이미지 띄우기
            isWin = false;
            /* betMoneyText.text = "\u20A9" + total.ToString("N0");
             feeText.text = "-\u20A9" + 0.ToString("N0");*/


             if (StatusManager.instance.GetSelectedPerk(2, 1) == 1 && Random.value < 0.5f) //만약 퍽 터졌다면
             {
                isWin = true;
                bottomPerk.SetActive(true);
                 //totalText.text = "\u20A9" + 0.ToString("N0");
             }
             /*else
             { 
                 string text = "-\u20A9" + total.ToString("N0");
                 totalText.text = "<color=red>" + text + "</color>"; //빨간색 변경
                 PlayerStats.Spend(total); //총 돈만큼 감소
             } */
            
        }

        Mood();
        Hunger();
        Fatigue();  //수치들 증감하기
    }

    private int Calculator(int num) //난이도나 그런거 따라 수치 랜덤 부여
    {
        int random = Random.Range(-30, 30);
        return PlayerStats.GetDiffy() * 20 + num + random;
    }
    private int CalculatorMood(int num) //난이도나 그런거 따라 수치 랜덤 부여, mood용도
    {
        int random = Random.Range(-30, 30);
        return PlayerStats.GetDiffy() * -20 + num + random;
    }

    protected void Hunger()
    {
        previousStatHungry.text = ((int)stats.currentHungry / 10).ToString();              //이전 배고픔 수치 나타내기
        previousGraphHunger.fillAmount = (float)stats.currentHungry / 1000;
        stats.currentHungry -= simulStat.hungry;                             //배고픔 추가
        stats.currentHungry = Mathf.Max(stats.currentHungry, 0);                     //0아래로 안떨어지게
        currentStatHungry.text = ((int)stats.currentHungry / 10).ToString();
        currentGraphHunger.fillAmount = (float)stats.currentHungry / 1000;

    }
    protected void Fatigue()
    {
        previousStatFatigue.text = ((int)stats.currentFatigue / 10).ToString();
        previousGraphFatigue.fillAmount = (float)stats.currentFatigue / 1000;
        stats.currentFatigue -= simulStat.fatigue;
        stats.currentFatigue = Mathf.Max(stats.currentFatigue, 0);
        currentStatFatigue.text = ((int)stats.currentFatigue / 10).ToString();
        currentGraphFatigue.fillAmount = (float)stats.currentFatigue / 1000;
    }
    protected void Mood()
    {
         previousStatAngry.text = (stats.currentAngry / 10).ToString();
        if (isWin)//만약 이겼다면
        {
            stats.currentAngry += (int)(simulStat.angry * 1.5f);
        }
        else
        {
            stats.currentAngry += simulStat.angry;
        }
         stats.currentAngry = Mathf.Max(stats.currentAngry, 0);
         currentStatAngry.text = (stats.currentAngry / 10).ToString();

         previousStatSad.text = (stats.currentSadness / 10).ToString();
        if (isWin)//만약 이겼다면
        {
            stats.currentSadness += (int)(simulStat.sadness * 1.5f);
        }
        else
        {
            stats.currentSadness += simulStat.sadness;
        }
         stats.currentSadness = Mathf.Max(stats.currentSadness, 0);
         currentStatSad.text = (stats.currentSadness / 10).ToString();

         previousStatBored.text = (stats.currentBoredom / 10).ToString();
        if (isWin)//만약 이겼다면
        {
            stats.currentBoredom += (int)(simulStat.boredom * 1.5f);
        }
        else
        {
            stats.currentBoredom += simulStat.boredom;
        }
         stats.currentBoredom = Mathf.Max(stats.currentBoredom, 0);
         currentStatBored.text = (stats.currentBoredom / 10).ToString();

        if (simulStat.angry > 0) 
        {
            if (ColorUtility.TryParseHtmlString("#840000", out Color color))
            {
                currentStatAngry.color = color;
            }
        }
        if (simulStat.sadness > 0)
        {
            if (ColorUtility.TryParseHtmlString("#840000", out Color color))
            {
                currentStatSad.color = color;
            }
        }
        if (simulStat.boredom > 0)
        {
            if (ColorUtility.TryParseHtmlString("#840000", out Color color))
            {
                currentStatBored.color = color;
            }
        }

    }

    public virtual void Confirm() //확인 버튼 누를시 호출될 함수
    {
        TimeManager.instance.SkipTime(playTime);    
        //  Time.timeScale = 1;
        TimeManager.instance.TimeTicking = true; //시간 다시 돌게

        StatusManager.instance.LoadStatus(stats);

        Cursor.visible = false;
        //상태 이상 적용하기
        //씬 이동
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
    }
}
