using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static UIManager;

public class ResultPanel : MonoBehaviour
{
    protected int[] experienceRequired = { 100, 280, 390, 530, 850, 1150, 1500, 2100, 3100, 5000};

    protected int money, tip, total, bonus, penalty;

    [Header("Numeric")]
  /*  [SerializeField]
    private int skillNum; //��ų ����
    [SerializeField]
    private int hungerNumeric; //����� ���� ��ġ
    [SerializeField]
    private int fatigueNumeric; //�Ƿε� ���� ��ġ
    [SerializeField]
    private int stressNumeric; //��Ʈ���� ���� ���� ��ġ*/

    [SerializeField] private SimulationJobStat jobStat;

    [Header("Text")]
    [SerializeField]
    protected TextMeshProUGUI moneyText;
    [SerializeField]
    protected TextMeshProUGUI tipText;
    [SerializeField]
    protected TextMeshProUGUI totalText;
    protected Status stats;

    [SerializeField]
    protected TextMeshProUGUI previousStatFatigue;
    [SerializeField]
    protected TextMeshProUGUI previousStatHungry;
    [SerializeField]
    protected TextMeshProUGUI previousStatSkill;
    [SerializeField]
    protected TextMeshProUGUI currentStatFatigue;
    [SerializeField]
    protected TextMeshProUGUI currentStatHungry;
    [SerializeField]
    protected TextMeshProUGUI currentStatSkill;

    [Header("Mood")]
    [SerializeField]
    protected TextMeshProUGUI previousStatAnger;
    [SerializeField]
    protected TextMeshProUGUI currentStatAnger;
    [SerializeField]
    protected TextMeshProUGUI previousStatSad;
    [SerializeField]
    protected TextMeshProUGUI currentStatSad;
    [SerializeField]
    protected TextMeshProUGUI previousStatBoredom;
    [SerializeField]
    protected TextMeshProUGUI currentStatBoredom;

    [Header("FillImage")]
    [SerializeField]
    protected Image previousGraphFatigue;
    [SerializeField]
    protected Image currentGraphFatigue;
    [SerializeField]
    protected Image previousGraphHunger;
    [SerializeField]
    protected Image currentGraphHunger;
    [SerializeField]
    protected Image previousGraphSkill;
    [SerializeField]
    protected Image currentGraphSkill;

    [Header("Image")]
    [SerializeField]
    protected GameObject[] moodleImage;
    [SerializeField]
    protected Sprite[] CardImage;
   /* [SerializeField]
    protected Image moodImage;
    [SerializeField]
    protected GameObject[] moodText;
    [SerializeField]
    protected Sprite[] moodSprite;*/
    [SerializeField]
    protected GameObject[] moodleText; //�ش� ���� �ؽ�Ʈ

    [Header("Card")]
    [SerializeField]
    protected GameObject[] cardObject; //6���� ī��
    [SerializeField]
    protected GameObject cardPanel; //ī�� �г�
    [SerializeField]
    protected GameObject cardPanelConfirm; //ī�� �г� Ȯ�ι�ư
    [SerializeField]
    protected GameObject resultPanelConfirm; //���â �г� Ȯ�ι�ư
    [SerializeField]
    protected GameObject[] titlePanel; //�۾� �Ϸ� �ƴϸ� ���� ���� ����

    [Header("Scene")]
    [SerializeField]
    public SceneTransitionManager.Location locationToSwitch;
    [SerializeField] private int jobTime; //��ŵ�� �۾� �ð�

    /*[SerializeField]
    private int whatMini;*/

    [Header("Perk & Skill")]
    [SerializeField] protected Image notGetSkill; //���� �̻� �� ��� xǥ��
    [SerializeField] protected GameObject BonusGameObject;
    [SerializeField] protected GameObject PenaltyGameObject;
    [SerializeField] protected TextMeshProUGUI bonusMoney;
    [SerializeField] protected TextMeshProUGUI penaltyMoney;

    protected int[] card = new int[6]; // ũ�� 6�� �迭 b[]


    protected void OnEnable()
    {
        resultPanelConfirm.SetActive(false);
        PenaltyGameObject.SetActive(false);
        BonusGameObject.SetActive(false);
        notGetSkill.gameObject.SetActive(false);
        System.Array.Clear(card, 0, card.Length); //0���� �迭 �ʱ�ȭ
       //Render(true);
    }
    public virtual void Render(bool quit = false)
    {
        /*if (UIManager.instance != null) 
        {
            UIManager.instance.CurrentUIState = UIState.TimedUI;
        }*/
        Cursor.visible = true;
        if (!quit) //���� �߰��� ������ �ƴϸ�
        {
            if(ChiTableManager.instance != null)
            {
                money = ChiTableManager.instance.Money;
                tip = ChiTableManager.instance.Tip;
            }
            else if(CoffeeTableManager.instance != null)
            {
                money = CoffeeTableManager.instance.Money;
                tip = CoffeeTableManager.instance.Tip;
            }
            else if (BrickMoveManager.instance != null)
            {
                money = BrickMoveManager.instance.Money;
                tip = BrickMoveManager.instance.Tip;
            }
            else if (GasStationManager.instance != null)
            {
                money = GasStationManager.instance.Money;
                tip = GasStationManager.instance.Tip;
            }

            /* if (StatusManager.instance.GetStatus().miniPrice[whatMini] < money) //���� �̹��� �� ���� �� ũ�ٸ�
             {
                 StatusManager.instance.GetStatus().miniPrice[whatMini] = money; //�����
             }*/

            total = money + tip;
            titlePanel[0].SetActive(true); //�۾� �Ϸ� �̹��� ����
        }
        else
        {
            titlePanel[1].SetActive(true); //�۾� ���� �̹��� ����
        }

        if (StatusManager.instance.GetSelectedPerk(2, 0) == 1) //���� ���ʽ� �� ����ٸ�
        {
            BonusGameObject.SetActive(true);
            bonus = money / 10;
            total += bonus;
            bonusMoney.text = "\u20A9" + bonus.ToString();
        }
        if (StatusManager.instance.GetMoodle()[13].isActive) //���� ���� Ȱ��ȭ���ִٸ�
        {
            PenaltyGameObject.SetActive(true);
            penalty = (money/10) * 3;
            total -= penalty;
            penaltyMoney.text = "\u20A9" + penalty.ToString();
        }


        moneyText.text = "\u20A9" + money.ToString();
        tipText.text = "\u20A9" + tip.ToString();
        totalText.text = "\u20A9" + total.ToString();
        PlayerStats.Earn(total); //�� ����ŭ ����

        if (ChiTableManager.instance != null)
        {
            stats = ChiTableManager.instance.stat;
        }
        else if (CoffeeTableManager.instance != null)
        {
            stats = CoffeeTableManager.instance.stat;
        }
        else if (BrickMoveManager.instance != null)
        {
            stats = BrickMoveManager.instance.stat;
        }
        else if (GasStationManager.instance != null)
        {
            stats = GasStationManager.instance.stat;
        }


        //previousStatFatigue.text = stats.currentFatigue.ToString();
        previousStatSkill.text = stats.level[jobStat.skillNumber].ToString(); //��ų �ε��� �־ �ش� ��ų ��������
        if (StatusManager.instance.GetMoodle()[3].isActive) //���� ���� Ȱ��ȭ ���¶��
        {
            //���߿� �ʿ��Ҷ� �ֱ�
            notGetSkill.gameObject.SetActive(true);
        }
        else
        {
            if(StatusManager.instance.GetSelectedPerk(6, 0) == 0) //���� ��� �� ����ٸ�
            {
                AddExperience(jobStat.skillNumber, Mathf.RoundToInt(jobStat.skillAmount * 1.2f));
            }
            else
            {
                AddExperience(jobStat.skillNumber, jobStat.skillAmount);
            }
        }
        currentStatSkill.text = stats.level[jobStat.skillNumber].ToString(); //����ġ ��� �� ��ų ���� ��������

        Mood();
        Hunger();
        Fatigue();  //��ġ�� �����ϱ�

        //���� ī�� ���� ���ϱ�
        int cardNum = BadCard();
        // cardNum�� ����ŭ 1~6 �� ������ ���� �迭�� �ֱ�
        for (int i = 0; i < cardNum; i++)
        {
            card[i] = Random.Range(1, 7); // 1���� 6������ ������ ����
        }

        // ������ �ε����� 0���� ��������. �迭�� �������� ����
        card = card.OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Card>().frontCard = CardImage[card[i]];  //ī�� �ո� ä���ֱ�
        }

       // Time.timeScale = 0; //�ð� ���߱�
        Invoke(nameof(PanelOn), 3f); //3�� �ڿ� �г� ������ ����
    }

    protected int Calculator(int num) //���̵��� �׷��� ���� ��ġ ���� �ο�
    {
        int random = Random.Range(-50, 60);
       // return PlayerStats.GetDiffy() * 20 + num + random
            return num + random;
    }

    public void AddExperience(int skillIndex, int experience)
    {
        if (stats.level[skillIndex] == 10) //�����̸� ����
        {
            previousGraphSkill.fillAmount = 1; //��� �׷����� ��
            currentGraphSkill.fillAmount = 0;
            return;
        }
        int currentExperience = stats.skillAmount[skillIndex];
        int newExperience = currentExperience + experience;
        int nextLevelExperience = experienceRequired[stats.level[skillIndex]]; //�������� ���� ���� ��


        if (nextLevelExperience <= newExperience)
        {
            stats.level[skillIndex]++;
            stats.skillAmount[skillIndex] = newExperience - nextLevelExperience;

            if(stats.level[skillIndex] == 10) //���� 10�� ����ٸ�
            {
                previousGraphSkill.fillAmount = 1; //��� �׷����� ��
                currentGraphSkill.fillAmount = 0;
            }
            else
            {
                previousGraphSkill.fillAmount = 0; //������������ ��� �׷����� 0
                currentGraphSkill.fillAmount = newExperience - nextLevelExperience / experienceRequired[stats.level[skillIndex]];
            }   
        }
        else
        {
            previousGraphSkill.fillAmount = (float)currentExperience / nextLevelExperience;
            currentGraphSkill.fillAmount = (float)newExperience / nextLevelExperience;
            stats.skillAmount[skillIndex] = newExperience;
        }
    }

    protected virtual void Hunger()
    {
        int hungry = (int)stats.currentHungry;
        previousStatHungry.text = (hungry / 10).ToString();
        previousGraphHunger.fillAmount = (float)hungry / 1000;

        int calcul = Calculator((int)jobStat.hungry);
        //
        //StatusManager.instance?.AddHungry(calcul * -1, true);
        


        hungry -= calcul;
        hungry = Mathf.Max(hungry, 0);
        currentStatHungry.text = ((int)hungry / 10).ToString();
        currentGraphHunger.fillAmount = (float)hungry / 1000;

        stats.currentHungry = hungry;
        /*previousStatHungry.text = ((int)stats.currentHungry / 10).ToString();              //���� ����� ��ġ ��Ÿ����
        previousGraphHunger.fillAmount = (float)stats.currentHungry / 1000;           
        stats.currentHungry -= Calculator((int)jobStat.hungry);                             //����� �߰�
        stats.currentHungry = Mathf.Max(stats.currentHungry, 0);                     //0�Ʒ��� �ȶ�������
        currentStatHungry.text = ((int)stats.currentHungry / 10).ToString();
        currentGraphHunger.fillAmount = (float)stats.currentHungry / 1000;*/
    }
    protected virtual void Fatigue()
    {
        int fatigue = (int)stats.currentFatigue;
        previousStatFatigue.text = (fatigue / 10).ToString();
        previousGraphFatigue.fillAmount = (float)fatigue / 1000;

        int calcul = Calculator((int)jobStat.fatigue);
        //StatusManager.instance?.AddFatigue(calcul * -1, true);

        fatigue -= calcul;
        fatigue = Mathf.Max(fatigue, 0);
        currentStatFatigue.text = ((int)fatigue / 10).ToString();
        currentGraphFatigue.fillAmount = (float)fatigue / 1000;

        stats.currentFatigue = fatigue;
        /*previousStatFatigue.text = ((int)stats.currentFatigue / 10).ToString();
        previousGraphFatigue.fillAmount = (float)stats.currentFatigue / 1000;
        stats.currentFatigue -= Calculator((int)jobStat.fatigue);
        stats.currentFatigue = Mathf.Max(stats.currentFatigue, 0);
        currentStatFatigue.text = ((int)stats.currentFatigue / 10).ToString();
        currentGraphFatigue.fillAmount = (float)stats.currentFatigue / 1000;*/
    }
    protected virtual void Mood()
    {
        int angry = stats.currentAngry;
        previousStatAnger.text = (angry / 10).ToString();
        angry += jobStat.angry;
        angry = Mathf.Min(angry, 1000);
        currentStatAnger.text = (angry / 10).ToString();

        int sadness = stats.currentSadness;
        previousStatSad.text = (sadness / 10).ToString();
        sadness += jobStat.sadness;
        sadness = Mathf.Min(sadness, 1000);
        currentStatSad.text = (sadness / 10).ToString();

        int boredom = stats.currentBoredom;
        previousStatBoredom.text = (boredom / 10).ToString();
        boredom += jobStat.boredom;
        boredom = Mathf.Min(boredom, 1000);
        currentStatBoredom.text = (boredom / 10).ToString();

        stats.currentAngry = angry;
        stats.currentSadness = sadness;
        stats.currentBoredom = boredom;

        /* previousStatAnger.text = (stats.currentAngry / 10).ToString();
         stats.currentAngry += Calculator(jobStat.angry);
         stats.currentAngry = Mathf.Min(stats.currentAngry, 1000);
         currentStatAnger.text = (stats.currentAngry / 10).ToString();

         previousStatSad.text = (stats.currentSadness / 10).ToString();
         stats.currentSadness += Calculator(jobStat.sadness);
         stats.currentSadness = Mathf.Min(stats.currentSadness, 1000);
         currentStatSad.text = (stats.currentSadness / 10).ToString();

         previousStatBoredom.text = (stats.currentBoredom / 10).ToString();
         stats.currentBoredom += Calculator(jobStat.boredom);
         stats.currentBoredom = Mathf.Min(stats.currentBoredom, 1000);
         currentStatBoredom.text = (stats.currentBoredom / 10).ToString();*/
    }

    public virtual void Confirm() //Ȯ�� ��ư ������ ȣ��� �Լ�
    {

        //UIManager.instance.CurrentUIState = UIState.None; //�̴ϰ��ӿ����� �ϴ� ��Ȱ��ȭ ���¶� �ּ����� ��������
        Cursor.visible = false;
        //  Time.timeScale = 1;
        TimeManager.instance.TimeTicking = true; //�ð� �ٽ� ����


        TimeManager.instance.SkipTime(jobTime);
        StatusManager.instance.LoadStatus(stats);  //���� �ٲ� �� �ð� ������

        StatusManager.instance.MoodleChange(13, true, 6 * 60, true); //���� ���� Ű��
        //���� �̻� �����ϱ�
        //�� �̵�
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);

        QuestManager.instance.CompleteObjective(0, 1);
    }
    public void ClickCard(int index) //ī�� ���ý� ȣ��Ǵ� �Լ�
    {
        int sickNum = card[index]; //�����̻� ��ȣ �ޱ�

        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = false;  //��� ī�� ��Ȱ��ȭ
            if(i != index)
            {
                cardObject[i].GetComponent<Card>().AfterRotate(); //������ ī�� ������
            }
        }

        cardPanelConfirm.SetActive(true);
        moodleImage[sickNum].SetActive(true); //�ش� ���� ������ ����
        moodleText[sickNum].SetActive(true);

        if(sickNum != 0) //���� ȿ�� ���� �ƴϸ�
        {
            int time = PlayerStats.GetDiffy() * 2 + Random.Range(4, 19);
            StatusManager.instance.MoodleChange(sickNum - 1, true, time * 60); //�����̻� ����
        }

    }

    protected void PanelOn() //ī�� �г� ������ �Լ�
    {
        cardPanel.SetActive(true); //ī�� �г� ������
        Invoke(nameof(ButtonOn), 3f); //3�� �ڿ� ��ư ������ ����
    }

    public void PanelOff() //ī�� �г� ������ �Լ�, Ȯ�� ��ư�̶� �����Ұ�
    {
        cardPanel.SetActive(false); //ī�� �г� ������
        resultPanelConfirm.SetActive(true); //��� Ȯ�ι�ư Ű��
    }
    protected void ButtonOn() //��ư ������ �Լ�
    {
        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = true;  //ī�� ��ư Ȱ��ȭ
        }

    }

    public int BadCard() //������ ī�� ���� ���ϱ�
    {
        int a = 1;
        if(stats.currentHungry <= 0)
        {
            a++;
        }
        if (stats.currentFatigue <= 0)
        {
            a++;
        }
        if (stats.currentAngry >= 1000)
        {
            a++;
        }
        if (stats.currentBoredom >= 1000)
        {
            a++;
        }
        if (stats.currentSadness >= 1000)
        {
            a++;
        }

        return a;
    }
}
