using Michsky.UI.Dark;
using PP.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using Yarn.Unity;


public class YarnCommand : MonoBehaviour
{
    public List<ItemData> cuItems; //편의점 아이템리스트
    public List<ItemData> bookItems; //도서관 아이템리스트
    public List<ItemData> trashItems; //도서관 아이템리스트
    [SerializeField] private ItemData wallets;

    public Simulscene[] simulscenes;
    public Simulscene[] simulsceneRisk;

    public UnityEvent giveWatch = new UnityEvent();
    public UnityEvent giveBread = new UnityEvent();
    public UnityEvent completeSean = new UnityEvent();

    public UnityEvent startBbq;
    public UnityEvent startChicken;
    public UnityEvent startCoffee;
    public UnityEvent startBiotest;
    public UnityEvent startBrick;
    public UnityEvent startGas;

    [Header("Dialogue")]
    public DialogueRunner dialogueRunner;
    public OptionsListView optionsListView;
    public Yarn.Unity.Addons.DialogueWheel.WheelDialogueView wheelDialogueView; //휠 다이얼로그, 설득 시 띄울 용도
    public Yarn.Unity.TextLineProvider textLineProvider;


    public List<ItemData> recieveItems; //대화 중 받을 아이템리스트

    private void Start()
    {
        textLineProvider.textLanguageCode = LocalizationSettings.SelectedLocale.Identifier.Code;
    }


    [YarnCommand("giveWatch")]
    public void GiveWatch()
    {
        giveWatch.Invoke(); //튜토리얼에서 시계 주기
    }
    [YarnCommand("giveBread")]
    public void GiveBread()
    {
        giveBread.Invoke(); //튜토리얼에서 시계 주기
    }
    [YarnCommand("completeSean")]
    public void CompleteSean()
    {
        completeSean.Invoke(); //튜토리얼에서 시계 주기
    }

    [YarnCommand("startBBQ")]
    public void StartBBQ()
    {
        startBbq.Invoke(); //ui매니저에서 bbq 패널 띄우기
    }

    [YarnCommand("startChicken")]
    public void StartChicken()
    {
        startChicken.Invoke(); //ui매니저에서 chicken 패널 띄우기
    }

    [YarnCommand("startCoffee")]
    public void StartCoffee()
    {
        startCoffee.Invoke(); //ui매니저에서 chicken 패널 띄우기
    }
    [YarnCommand("startBrick")]
    public void StartBrick()
    {
        startBrick.Invoke(); //ui매니저에서 chicken 패널 띄우기
    }

    [YarnCommand("startBiotest")]
    public void StartBiotest()
    {
        startBiotest.Invoke(); //ui매니저에서 생체실험 패널 띄우기
    }
    [YarnCommand("startGas")]
    public void StartGas()
    {
        startGas.Invoke(); //ui매니저에서 주유소 패널 띄우기
    }
    


    [YarnCommand("shopBook")]
    public void ShopBook()
    {
        UIManager.instance.OpenShop(bookItems, "None");
    }
    [YarnCommand("shopTrash")]
    public void ShopTrash()
    {
        UIManager.instance.OpenShop(trashItems, "Trash");
    }
    [YarnCommand("shopCu")]
    public void ShopCu()
    {
        UIManager.instance.OpenShop(cuItems, "Drink");
    }


    [YarnCommand("buyLottery")]
    public void BuyLottery()
    {
        PlayerStats.Spend(2000); //2000원 소비
        UIManager.instance.TriggerLotteryPanel();
    }

    [YarnCommand("playBasket")]
    public void PlayBasket()
    {
        UIManager.instance.OpenGame(1);
    }
    [YarnCommand("playYut")]
    public void PlayYut()
    {
        UIManager.instance.OpenGame(0);
    }



    [YarnCommand("buyLucky")]
    public void BuyLucky()
    {
        PlayerStats.Spend(5000); //5000원 소비
        StatusManager.instance.MoodleChange(11, true, 180); //3시간 행운 증가
    }

    [YarnCommand("playBusking")]
    public void PlayBusking()
    {
        //UIManager.instance.OpenGame(1);
    }


    #region Simul
    [YarnCommand("watchBusking")]
    public void WatchBusking()
    {
        CutsceneManager.Instance.StartSimulateScene(simulscenes[0]);

    }
    [YarnCommand("studying")]
    public void Studying()
    {
        CutsceneManager.Instance.StartSimulateScene(simulscenes[1]);

    }
    [YarnCommand("gymSimul")]
    public void GymSimul()
    {
        CutsceneManager.Instance.StartSimulateScene(simulscenes[2]);
        PlayerStats.Spend(15000);
    }
    [YarnCommand("arcadeSimul")]
    public void ArcadeSimul()
    {
        CutsceneManager.Instance.StartSimulateScene(simulscenes[3]);
        PlayerStats.Spend(4000);
    }
    #endregion





    [YarnCommand("changeToWheel")]
    public void ChangeToWheel()
    {
        dialogueRunner.dialogueViews[1] = wheelDialogueView; //휠다이얼로그로 변경
    }

    [YarnCommand("changeBasic")]
    public void ChangeBasic()
    {
        dialogueRunner.dialogueViews[1] = optionsListView; //원래걸로 다시 변경
        StatusManager.instance.SetTodayDice(); //다시 설득 난수 리셋
    }

    #region Dept
    [YarnCommand("payDept")]
    public void PayDept() //빛 강제로 내는 함수
    {
        int cost = PlayerStats.GetWeekDept();
        PlayerStats.Spend(cost);
        PlayerStats.SpendLastDept(false); //빚 다 냈음
        //HomeManager.instance?.DegenSean(); //빚쟁이 나가기
    }

    [YarnCommand("dontPayWeekDept")]
    public void DontPayWeekDept() //설득 성공해서 이번주 빚 안냄
    {
        PlayerStats.SpendLastDept(true); //이번주 빚 없앰, 대신 총액은 안깎임
        //HomeManager.instance?.DegenSean(); //빚쟁이 나가기
    }

    [YarnCommand("payNextWeek")]
    public void PayNextWeek() //설득 성공해서 이번주 빚 이월
    {
        PlayerStats.TurnoverLastDept(); //빚 이월
        //HomeManager.instance?.DegenSean(); //빚쟁이 나가기
    }

    [YarnCommand("cantPayGameOver")]
    public void CantPayGameOver() //게임 오버 화면 띄우기
    {
        UIManager.instance.GameOver(false);
    }

    [YarnCommand("degenSean")]
    public void DegenSean() //션 나가는 함수
    {
        HomeManager.instance?.DegenSean(); //빚쟁이 나가기
    }
    

    [YarnCommand("secondWind")]
    public void SecondWind() //설득 한번 더 퍽 발동 시 호출될 함수
    {
        StatusManager.instance.SetTodayDice(); //다시 설득 난수 리셋
        UIManager.instance.StartPerkNow(0);  //timeUi라도 뜨게
    }
    [YarnCommand("gifActive")]
    public void GifActive() //설득 성공 시 gif 생성
    {
        UIManager.instance.NoticeCreatePersuade(); //설득 GIF 띄우기
    }
    #endregion

    //바로 먹는 음식들
    [YarnCommand("buyFishBread")]
    public void BuyFishBread() //붕어빵 사먹을시
    {
        PlayerStats.Spend(1000); //1000원 소비
        FindObjectOfType<Throwing>().EatReadyCutscene(100); //붕어빵 먹기 실행
    }
    [YarnCommand("buyTanhulu")]
    public void BuyTanhulu() //붕어빵 사먹을시
    {
        PlayerStats.Spend(4000); //4000원 소비
        FindObjectOfType<Throwing>().EatReadyCutscene(104); //탕후루 먹기 실행
    }
    [YarnCommand("buyAmericano")]
    public void BuyAmericano() //붕어빵 사먹을시
    {
        PlayerStats.Spend(5000); //5000원 소비
        FindObjectOfType<Throwing>().DrinkReadyCutscene(103); //커피 먹기 실행
    }
    [YarnCommand("buyGimbab")]
    public void BuyGimbab() //김밥 사먹을시
    {
        PlayerStats.Spend(4000); //5000원 소비
        FindObjectOfType<Throwing>().EatReadyCutscene(105); //김밥 먹기 실행
    }


    #region Police
    [YarnCommand("policePass")]
    public void PolicePass() //설득 성공해서 경찰 따돌림
    {
        ExtraManager.instance?.DegenPolice(); //경찰 없애기
    }
    [YarnCommand("policeMoney")]
    public void PoliceMoney() //설득 성공해서 경찰 따돌림
    {
        PlayerStats.Spend(50000); //50000원 소비
        ExtraManager.instance?.DegenPolice(); //경찰 없애기
    }
    [YarnCommand("policeFail")]
    public void PoliceFail() //설득 실패 시 유치장
    {
        SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.Jail); //유치장 ㄱㄱ
    }
    [YarnCommand("getoutJail")]
    public void GetoutJail() //날짜 다 채우고 유치장 나가기
    {
        SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.Proto); //밖으로
    }
    #endregion

    [YarnCommand("shopStolen")]
    public void ShopStolen()
    {
        UIManager.instance.OpenShop(new List<ItemData>(), "Steal");
    }
    [YarnCommand("promptDept")]
    public void PromptDept()
    {
        UIManager.instance.TriggerDeptPrompt();
    }




    #region Quests
    [YarnCommand("tutoQue")]  //퀘스트 시작 함수
    public void TutoQue(string name)
    {
        var questData = Resources.Load<QuestData>("Quests/" + name);
        if (questData != null)
        {
            QuestManager.instance.StartQuest(questData);
        }
        else
        {
            Debug.Log($"퀘스트 데이터를 찾을 수 없습니다: Quests/{name}");
        }
    }


    [YarnCommand("questComplete")]
    public void QuestComplete(int id, int index)
    { 
        QuestManager.instance.CompleteObjective(id,index); //해당 퀘스트 완료
    }

    [YarnCommand("recieveItem")]
    public void RecieveItem(int itemId)
    {
        if (FindObjectOfType<Inventory>().Add(recieveItems[itemId], 1) == 0)
        {
            // FindObjectOfType
        }
        else
        {

        }
    }
    #endregion

    #region RiskChoice
    [YarnCommand("eatAbandon")]
    public void EatAbandon()
    {
        CutsceneManager.Instance.StartSimulateScene(simulsceneRisk[0], true, 
            actionRisk: () => { StatusManager.instance.MoodleChange(12, true, 1440);  ExtraManager.instance.interObjects[0].SetActive(false); },  // 위험일 때 실행되는 액션
            actionNone: () => {  ExtraManager.instance.interObjects[0].SetActive(false); }  // 위험이 아닐 때 실행되는 액션
        ,1);
    }
    [YarnCommand("stealWallet")]
    public void StealWallet()
    {
        CutsceneManager.Instance.StartSimulateScene(simulsceneRisk[1], true,
            actionRisk: () => { ExtraManager.instance.GenPolice(); ExtraManager.instance.interObjects[1].SetActive(false); },  // 위험일 때 실행되는 액션
            actionNone: () => { LootingManager.instance.GiveReward(wallets, 1); ExtraManager.instance.interObjects[1].SetActive(false); }  // 위험이 아닐 때 실행되는 액션
        ,0,true);
    }
    [YarnCommand("poorMoney")]
    public void PoorMoney()
    {
        int money = Random.Range(5, 31) * 1000;
        CutsceneManager.Instance.StartSimulateScene(simulsceneRisk[2], true,
            actionRisk: () => { RelationshipStats.AddAllPoint(-250); ExtraManager.instance.GenSean(); PlayerStats.Earn(money); },  // 위험일 때 실행되는 액션
            actionNone: () => { PlayerStats.Earn(money); }  // 위험이 아닐 때 실행되는 액션
        , 2, true);
    }

    #endregion
}
