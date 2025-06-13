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
    private int skillNum; //스킬 선택
    [SerializeField]
    private int hungerNumeric; //배고픔 감소 수치
    [SerializeField]
    private int fatigueNumeric; //피로도 감소 수치
    [SerializeField]
    private int stressNumeric; //스트레스 감정 증가 수치*/

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
    protected GameObject[] moodleText; //해당 무들 텍스트

    [Header("Card")]
    [SerializeField]
    protected GameObject[] cardObject; //6개의 카드
    [SerializeField]
    protected GameObject cardPanel; //카드 패널
    [SerializeField]
    protected GameObject cardPanelConfirm; //카드 패널 확인버튼
    [SerializeField]
    protected GameObject resultPanelConfirm; //결과창 패널 확인버튼
    [SerializeField]
    protected GameObject[] titlePanel; //작업 완료 아니면 실패 제목 띄우기

    [Header("Scene")]
    [SerializeField]
    public SceneTransitionManager.Location locationToSwitch;
    [SerializeField] private int jobTime; //스킵할 작업 시간

    /*[SerializeField]
    private int whatMini;*/

    [Header("Perk & Skill")]
    [SerializeField] protected Image notGetSkill; //무들 이상 시 띄울 x표시
    [SerializeField] protected GameObject BonusGameObject;
    [SerializeField] protected GameObject PenaltyGameObject;
    [SerializeField] protected TextMeshProUGUI bonusMoney;
    [SerializeField] protected TextMeshProUGUI penaltyMoney;

    protected int[] card = new int[6]; // 크기 6의 배열 b[]


    protected void OnEnable()
    {
        resultPanelConfirm.SetActive(false);
        PenaltyGameObject.SetActive(false);
        BonusGameObject.SetActive(false);
        notGetSkill.gameObject.SetActive(false);
        System.Array.Clear(card, 0, card.Length); //0으로 배열 초기화
       //Render(true);
    }
    public virtual void Render(bool quit = false)
    {
        /*if (UIManager.instance != null) 
        {
            UIManager.instance.CurrentUIState = UIState.TimedUI;
        }*/
        Cursor.visible = true;
        if (!quit) //만약 중간에 나간게 아니면
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

            /* if (StatusManager.instance.GetStatus().miniPrice[whatMini] < money) //만약 이번에 번 돈이 더 크다면
             {
                 StatusManager.instance.GetStatus().miniPrice[whatMini] = money; //덮어쓰기
             }*/

            total = money + tip;
            titlePanel[0].SetActive(true); //작업 완료 이미지 띄우기
        }
        else
        {
            titlePanel[1].SetActive(true); //작업 실패 이미지 띄우기
        }

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
            penalty = (money/10) * 3;
            total -= penalty;
            penaltyMoney.text = "\u20A9" + penalty.ToString();
        }


        moneyText.text = "\u20A9" + money.ToString();
        tipText.text = "\u20A9" + tip.ToString();
        totalText.text = "\u20A9" + total.ToString();
        PlayerStats.Earn(total); //총 돈만큼 증가

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
        previousStatSkill.text = stats.level[jobStat.skillNumber].ToString(); //스킬 인덱스 넣어서 해당 스킬 가져오기
        if (StatusManager.instance.GetMoodle()[3].isActive) //만약 두통 활성화 상태라면
        {
            //나중에 필요할때 넣기
            notGetSkill.gameObject.SetActive(true);
        }
        else
        {
            if(StatusManager.instance.GetSelectedPerk(6, 0) == 0) //만약 기술 퍽 찍었다면
            {
                AddExperience(jobStat.skillNumber, Mathf.RoundToInt(jobStat.skillAmount * 1.2f));
            }
            else
            {
                AddExperience(jobStat.skillNumber, jobStat.skillAmount);
            }
        }
        currentStatSkill.text = stats.level[jobStat.skillNumber].ToString(); //경험치 계산 후 스킬 레벨 가져오기

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

    protected int Calculator(int num) //난이도나 그런거 따라 수치 랜덤 부여
    {
        int random = Random.Range(-50, 60);
       // return PlayerStats.GetDiffy() * 20 + num + random
            return num + random;
    }

    public void AddExperience(int skillIndex, int experience)
    {
        if (stats.level[skillIndex] == 10) //만렙이면 리턴
        {
            previousGraphSkill.fillAmount = 1; //흰색 그래프는 꽉
            currentGraphSkill.fillAmount = 0;
            return;
        }
        int currentExperience = stats.skillAmount[skillIndex];
        int newExperience = currentExperience + experience;
        int nextLevelExperience = experienceRequired[stats.level[skillIndex]]; //다음레벨 가기 위한 양


        if (nextLevelExperience <= newExperience)
        {
            stats.level[skillIndex]++;
            stats.skillAmount[skillIndex] = newExperience - nextLevelExperience;

            if(stats.level[skillIndex] == 10) //만약 10렙 찍었다면
            {
                previousGraphSkill.fillAmount = 1; //흰색 그래프는 꽉
                currentGraphSkill.fillAmount = 0;
            }
            else
            {
                previousGraphSkill.fillAmount = 0; //레벨업됬으니 흰색 그래프는 0
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
        /*previousStatHungry.text = ((int)stats.currentHungry / 10).ToString();              //이전 배고픔 수치 나타내기
        previousGraphHunger.fillAmount = (float)stats.currentHungry / 1000;           
        stats.currentHungry -= Calculator((int)jobStat.hungry);                             //배고픔 추가
        stats.currentHungry = Mathf.Max(stats.currentHungry, 0);                     //0아래로 안떨어지게
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

    public virtual void Confirm() //확인 버튼 누를시 호출될 함수
    {

        //UIManager.instance.CurrentUIState = UIState.None; //미니게임에서는 일단 비활성화 상태라 주석으로 지워놨음
        Cursor.visible = false;
        //  Time.timeScale = 1;
        TimeManager.instance.TimeTicking = true; //시간 다시 돌게


        TimeManager.instance.SkipTime(jobTime);
        StatusManager.instance.LoadStatus(stats);  //스탯 바꾼 후 시간 보내기

        StatusManager.instance.MoodleChange(13, true, 6 * 60, true); //녹초 무들 키기
        //상태 이상 적용하기
        //씬 이동
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);

        QuestManager.instance.CompleteObjective(0, 1);
    }
    public void ClickCard(int index) //카드 선택시 호출되는 함수
    {
        int sickNum = card[index]; //상태이상 번호 받기

        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = false;  //모든 카드 비활성화
            if(i != index)
            {
                cardObject[i].GetComponent<Card>().AfterRotate(); //나머지 카드 돌리기
            }
        }

        cardPanelConfirm.SetActive(true);
        moodleImage[sickNum].SetActive(true); //해당 무들 아이콘 띄우기
        moodleText[sickNum].SetActive(true);

        if(sickNum != 0) //만약 효과 없음 아니면
        {
            int time = PlayerStats.GetDiffy() * 2 + Random.Range(4, 19);
            StatusManager.instance.MoodleChange(sickNum - 1, true, time * 60); //상태이상 적용
        }

    }

    protected void PanelOn() //카드 패널 켜지는 함수
    {
        cardPanel.SetActive(true); //카드 패널 켜지기
        Invoke(nameof(ButtonOn), 3f); //3초 뒤에 버튼 켜지기 실행
    }

    public void PanelOff() //카드 패널 꺼지는 함수, 확인 버튼이랑 연결할것
    {
        cardPanel.SetActive(false); //카드 패널 꺼지기
        resultPanelConfirm.SetActive(true); //결과 확인버튼 키기
    }
    protected void ButtonOn() //버튼 켜지는 함수
    {
        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = true;  //카드 버튼 활성화
        }

    }

    public int BadCard() //부정적 카드 개수 구하기
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
