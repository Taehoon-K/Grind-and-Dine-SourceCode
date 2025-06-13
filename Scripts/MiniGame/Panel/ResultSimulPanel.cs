using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class ResultSimulPanel : ResultPanel
{
    [SerializeField] private SimulationJobStat[] simulationStats; //각 미니게임 시뮬시 증감할 스탯 양

    [Header("Skill")]
    [SerializeField]
    private GameObject[] skillImage;
    [SerializeField]
    private GameObject[] skillText;

    /*[Header("IsSimulOnly")]
    [SerializeField] private bool[] isOnlySimul; //시뮬레이션만 되는건지 체크, 트루면 오직 시뮬만 되는것*/

    private int gameIndex; //이 시뮬 게임이 뭔지
    private void OnDisable()
    {
        for (int i = 0; i < skillImage.Length; i++)
        {
            if (skillImage[i] != null)
            {
                skillImage[i].SetActive(false); //이미지 다 끄기
            }
            if (skillText[i] != null)
            {
                skillText[i].SetActive(false); //이미지 다 끄기
            }
        }
        for (int i = 0; i < moodleImage.Length; i++)
        {
            moodleImage[i].SetActive(false); //해당 무들 아이콘 띄우기
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
        /*if (!isOnlySimul[gameIndex]) //만약 오직 시뮬만이 아니라 직접 플레이가능한 작업이면
        {
            int c = Random.Range((int)(wage * 0.7), (int)(wage * 0.9) + 1); // a - b 이상, a + b 이하의 랜덤한 정수
            int roundedC = Mathf.RoundToInt(c / 100.0f) * 100; // 십의 자리에서 반올림
            money = roundedC;
        }
        else //오직 시뮬만 되는 작업이면
        {
            money = wage; //돈 안깎고 그대로
        }*/
        
        tip = 0;
        total = money;
        titlePanel[0].SetActive(true); //작업 완료 이미지 띄우기


        if (StatusManager.instance.GetSelectedPerk(2, 0) == 1) //만약 보너스 퍽 찍었다면
        {
            BonusGameObject.SetActive(true);
            bonus = money / 10;
            total += bonus;
            bonusMoney.text = "\u20A9" + bonus.ToString();
        }
        if (StatusManager.instance.GetMoodle()[13].isActive) //만약 녹초 활성화되있다면
        {
            PenaltyGameObject.SetActive(true);
            penalty = (money / 10) * 3;
            total -= penalty;
            penaltyMoney.text = "\u20A9" + penalty.ToString();
        }

        moneyText.text = "\u20A9" + money.ToString();
        tipText.text = "\u20A9" + tip.ToString();
        totalText.text = "\u20A9" + total.ToString();
        PlayerStats.Earn(total); //총 돈만큼 증가

        stats = StatusManager.instance.GetStatus();


        previousStatFatigue.text = stats.currentFatigue.ToString();
        previousStatSkill.text = stats.level[simulationStats[gameIndex].skillNumber].ToString(); //스킬 인덱스 넣어서 해당 스킬 가져오기

        if (StatusManager.instance.GetMoodle()[3].isActive) //만약 두통 활성화 상태라면
        {
            //나중에 필요할때 넣기
            notGetSkill.gameObject.SetActive(true);
        }
        else
        {
            if (StatusManager.instance.GetSelectedPerk(6, 0) == 0) //만약 기술 퍽 찍었다면
            {
                AddExperience(simulationStats[gameIndex].skillNumber, Mathf.RoundToInt(simulationStats[gameIndex].skillAmount * 1.2f));
            }
            else
            {
                AddExperience(simulationStats[gameIndex].skillNumber, simulationStats[gameIndex].skillAmount);
            }
        }
        currentStatSkill.text = stats.level[simulationStats[gameIndex].skillNumber].ToString(); //경험치 계산 후 스킬 레벨 가져오기

        Mood();
        Hunger();
        Fatigue();  //수치들 증감하기

        //부정 카드 개수 구하기
        int cardNum = BadCard();
        // cardNum의 값만큼 1~6 중 무작위 값을 배열에 넣기
        for (int i = 0; i < cardNum; i++)
        {
            card[i] = Random.Range(1, 7); // 1부터 6까지의 무작위 숫자
        }

        // 나머지 인덱스는 0으로 남아있음. 배열을 무작위로 섞기
        card = card.OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Card>().frontCard = CardImage[card[i]];  //카드 앞면 채워넣기
        }

        // Time.timeScale = 0; //시간 멈추기
        Invoke(nameof(PanelOn), 3f); //3초 뒤에 패널 켜지기 실행
    }
    public override void Confirm() //확인 버튼 누를시 호출될 함수
    {
        StatusManager.instance.LoadStatus(stats);
        //  Time.timeScale = 1;
        TimeManager.instance.TimeTicking = true; //시간 다시 돌게
        StatusManager.instance.MoodleChange(13, true, 6 * 60, true); //녹초 무들 키기
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


        /*previousStatHungry.text = ((int)stats.currentHungry / 10).ToString();              //이전 배고픔 수치 나타내기
        previousGraphHunger.fillAmount = (float)stats.currentHungry / 1000;
        stats.currentHungry -= Calculator((int)simulationStats[gameIndex].hungry);                             //배고픔 추가
        stats.currentHungry = Mathf.Max(stats.currentHungry, 0);                     //0아래로 안떨어지게
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


        skillImage[simulationStats[gameIndex].skillNumber].SetActive(true); //각 인덱스에 맞는 스킬 이름이랑 이미지 띄우기
        skillText[simulationStats[gameIndex].skillNumber].SetActive(true);
    }
}

