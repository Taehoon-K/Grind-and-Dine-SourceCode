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
    protected GameObject resultPanelConfirm; //���â �г� Ȯ�ι�ư
    [SerializeField]
    protected GameObject[] titlePanel; //�۾� �Ϸ� �ƴϸ� ���� ���� ����

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

        if (!quit) //�̰�ٸ�
        {
            titlePanel[0].SetActive(true); //�۾� �Ϸ� �̹��� ����

            /*int feeMoney = total * ((PlayerStats.Difficulty + 1) * 10) / 100;
            int finalMoney = total - feeMoney;
            betMoneyText.text = "\u20A9" + total.ToString("N0");
            feeText.text = "-\u20A9" + feeMoney.ToString("N0");
            totalText.text = "\u20A9" + finalMoney.ToString("N0");
            PlayerStats.Earn(finalMoney); //�� ����ŭ ����*/

            isWin = true;
        }
        else //���� ��
        {
            titlePanel[1].SetActive(true); //�۾� ���� �̹��� ����
            isWin = false;
            /* betMoneyText.text = "\u20A9" + total.ToString("N0");
             feeText.text = "-\u20A9" + 0.ToString("N0");*/


             if (StatusManager.instance.GetSelectedPerk(2, 1) == 1 && Random.value < 0.5f) //���� �� �����ٸ�
             {
                isWin = true;
                bottomPerk.SetActive(true);
                 //totalText.text = "\u20A9" + 0.ToString("N0");
             }
             /*else
             { 
                 string text = "-\u20A9" + total.ToString("N0");
                 totalText.text = "<color=red>" + text + "</color>"; //������ ����
                 PlayerStats.Spend(total); //�� ����ŭ ����
             } */
            
        }

        Mood();
        Hunger();
        Fatigue();  //��ġ�� �����ϱ�
    }

    private int Calculator(int num) //���̵��� �׷��� ���� ��ġ ���� �ο�
    {
        int random = Random.Range(-30, 30);
        return PlayerStats.GetDiffy() * 20 + num + random;
    }
    private int CalculatorMood(int num) //���̵��� �׷��� ���� ��ġ ���� �ο�, mood�뵵
    {
        int random = Random.Range(-30, 30);
        return PlayerStats.GetDiffy() * -20 + num + random;
    }

    protected void Hunger()
    {
        previousStatHungry.text = ((int)stats.currentHungry / 10).ToString();              //���� ����� ��ġ ��Ÿ����
        previousGraphHunger.fillAmount = (float)stats.currentHungry / 1000;
        stats.currentHungry -= simulStat.hungry;                             //����� �߰�
        stats.currentHungry = Mathf.Max(stats.currentHungry, 0);                     //0�Ʒ��� �ȶ�������
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
        if (isWin)//���� �̰�ٸ�
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
        if (isWin)//���� �̰�ٸ�
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
        if (isWin)//���� �̰�ٸ�
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

    public virtual void Confirm() //Ȯ�� ��ư ������ ȣ��� �Լ�
    {
        TimeManager.instance.SkipTime(playTime);    
        //  Time.timeScale = 1;
        TimeManager.instance.TimeTicking = true; //�ð� �ٽ� ����

        StatusManager.instance.LoadStatus(stats);

        Cursor.visible = false;
        //���� �̻� �����ϱ�
        //�� �̵�
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
    }
}
