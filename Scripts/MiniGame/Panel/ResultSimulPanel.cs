using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class ResultSimulPanel : ResultPanel
{
    [SerializeField] private SimulationJobStat[] simulationStats; //�� �̴ϰ��� �ùĽ� ������ ���� ��

    [Header("Skill")]
    [SerializeField]
    private GameObject[] skillImage;
    [SerializeField]
    private GameObject[] skillText;

    /*[Header("IsSimulOnly")]
    [SerializeField] private bool[] isOnlySimul; //�ùķ��̼Ǹ� �Ǵ°��� üũ, Ʈ��� ���� �ùĸ� �Ǵ°�*/

    private int gameIndex; //�� �ù� ������ ����
    private void OnDisable()
    {
        for (int i = 0; i < skillImage.Length; i++)
        {
            if (skillImage[i] != null)
            {
                skillImage[i].SetActive(false); //�̹��� �� ����
            }
            if (skillText[i] != null)
            {
                skillText[i].SetActive(false); //�̹��� �� ����
            }
        }
        for (int i = 0; i < moodleImage.Length; i++)
        {
            moodleImage[i].SetActive(false); //�ش� ���� ������ ����
            moodleText[i].SetActive(false);
        }
        cardPanelConfirm.SetActive(false);
        resultPanelConfirm.SetActive(false);
    }

    public void Render2(int gameIndex)
    {
        UIManager.instance.CurrentUIState = UIManager.UIState.TimedUI;

        this.gameIndex = gameIndex;
        int wage = simulationStats[gameIndex].BaseSalary;
        money = wage;
        /*if (!isOnlySimul[gameIndex]) //���� ���� �ùĸ��� �ƴ϶� ���� �÷��̰����� �۾��̸�
        {
            int c = Random.Range((int)(wage * 0.7), (int)(wage * 0.9) + 1); // a - b �̻�, a + b ������ ������ ����
            int roundedC = Mathf.RoundToInt(c / 100.0f) * 100; // ���� �ڸ����� �ݿø�
            money = roundedC;
        }
        else //���� �ùĸ� �Ǵ� �۾��̸�
        {
            money = wage; //�� �ȱ�� �״��
        }*/
        
        tip = 0;
        total = money;
        titlePanel[0].SetActive(true); //�۾� �Ϸ� �̹��� ����


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
            penalty = (money / 10) * 3;
            total -= penalty;
            penaltyMoney.text = "\u20A9" + penalty.ToString();
        }

        moneyText.text = "\u20A9" + money.ToString();
        tipText.text = "\u20A9" + tip.ToString();
        totalText.text = "\u20A9" + total.ToString();
        PlayerStats.Earn(total); //�� ����ŭ ����

        stats = StatusManager.instance.GetStatus();


        previousStatFatigue.text = stats.currentFatigue.ToString();
        previousStatSkill.text = stats.level[simulationStats[gameIndex].skillNumber].ToString(); //��ų �ε��� �־ �ش� ��ų ��������

        if (StatusManager.instance.GetMoodle()[3].isActive) //���� ���� Ȱ��ȭ ���¶��
        {
            //���߿� �ʿ��Ҷ� �ֱ�
            notGetSkill.gameObject.SetActive(true);
        }
        else
        {
            if (StatusManager.instance.GetSelectedPerk(6, 0) == 0) //���� ��� �� ����ٸ�
            {
                AddExperience(simulationStats[gameIndex].skillNumber, Mathf.RoundToInt(simulationStats[gameIndex].skillAmount * 1.2f));
            }
            else
            {
                AddExperience(simulationStats[gameIndex].skillNumber, simulationStats[gameIndex].skillAmount);
            }
        }
        currentStatSkill.text = stats.level[simulationStats[gameIndex].skillNumber].ToString(); //����ġ ��� �� ��ų ���� ��������

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
    public override void Confirm() //Ȯ�� ��ư ������ ȣ��� �Լ�
    {
        StatusManager.instance.LoadStatus(stats);
        //  Time.timeScale = 1;
        TimeManager.instance.TimeTicking = true; //�ð� �ٽ� ����
        StatusManager.instance.MoodleChange(13, true, 6 * 60, true); //���� ���� Ű��
        UIManager.instance.CurrentUIState = UIManager.UIState.None;

        QuestManager.instance.CompleteObjective(0, 1);
    }


    protected override void Hunger()
    {
        int hungry = (int)stats.currentHungry;
        previousStatHungry.text = (hungry / 10).ToString();
        previousGraphHunger.fillAmount = (float)hungry / 1000;

        int calcul = Calculator((int)simulationStats[gameIndex].hungry);
        StatusManager.instance?.AddHungry(calcul * -1, true);

        hungry -= calcul;
        hungry = Mathf.Max(hungry, 0);
        currentStatHungry.text = ((int)hungry / 10).ToString();
        currentGraphHunger.fillAmount = (float)hungry / 1000;


        /*previousStatHungry.text = ((int)stats.currentHungry / 10).ToString();              //���� ����� ��ġ ��Ÿ����
        previousGraphHunger.fillAmount = (float)stats.currentHungry / 1000;
        stats.currentHungry -= Calculator((int)simulationStats[gameIndex].hungry);                             //����� �߰�
        stats.currentHungry = Mathf.Max(stats.currentHungry, 0);                     //0�Ʒ��� �ȶ�������
        currentStatHungry.text = ((int)stats.currentHungry / 10).ToString();
        currentGraphHunger.fillAmount = (float)stats.currentHungry / 1000;*/
    }
    protected override void Fatigue()
    {
        int fatigue = (int)stats.currentFatigue;
        previousStatFatigue.text = (fatigue / 10).ToString();
        previousGraphFatigue.fillAmount = (float)fatigue / 1000;

        int calcul = Calculator((int)simulationStats[gameIndex].fatigue);
        //Debug.Log(calcul + "aaaaaaaaaaaaaaaaaaaaaa");
        StatusManager.instance?.AddFatigue(calcul * -1,true);

        fatigue -= calcul;
        fatigue = Mathf.Max(fatigue, 0);
        currentStatFatigue.text = ((int)fatigue / 10).ToString();
        currentGraphFatigue.fillAmount = (float)fatigue / 1000;


       /* previousStatFatigue.text = ((int)stats.currentFatigue / 10).ToString();
        previousGraphFatigue.fillAmount = (float)stats.currentFatigue / 1000;
        stats.currentFatigue -= Calculator((int)simulationStats[gameIndex].fatigue);
        stats.currentFatigue = Mathf.Max(stats.currentFatigue, 0);
        currentStatFatigue.text = ((int)stats.currentFatigue / 10).ToString();
        currentGraphFatigue.fillAmount = (float)stats.currentFatigue / 1000;*/
    }
    protected override void Mood()
    {
        int angry = stats.currentAngry;
        previousStatAnger.text = (angry / 10).ToString();
        StatusManager.instance?.AddAngry(simulationStats[gameIndex].angry, true);
        angry += simulationStats[gameIndex].angry;
        angry = Mathf.Min(angry, 1000);
        currentStatAnger.text = (angry / 10).ToString();

        int sadness = stats.currentSadness;
        previousStatSad.text = (sadness / 10).ToString();
        StatusManager.instance?.AddSadness(simulationStats[gameIndex].sadness, true);
        sadness += simulationStats[gameIndex].sadness;
        sadness = Mathf.Min(sadness, 1000);
        currentStatSad.text = (sadness / 10).ToString();

        int boredom = stats.currentBoredom;
        previousStatBoredom.text = (boredom / 10).ToString();
        StatusManager.instance?.AddBoredom(simulationStats[gameIndex].boredom, true);
        boredom += simulationStats[gameIndex].boredom;
        boredom = Mathf.Min(boredom, 1000);
        currentStatBoredom.text = (boredom / 10).ToString();


        skillImage[simulationStats[gameIndex].skillNumber].SetActive(true); //�� �ε����� �´� ��ų �̸��̶� �̹��� ����
        skillText[simulationStats[gameIndex].skillNumber].SetActive(true);
    }
}

