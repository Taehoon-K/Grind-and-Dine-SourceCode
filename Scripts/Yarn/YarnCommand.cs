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
    public List<ItemData> cuItems; //������ �����۸���Ʈ
    public List<ItemData> bookItems; //������ �����۸���Ʈ
    public List<ItemData> trashItems; //������ �����۸���Ʈ
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
    public Yarn.Unity.Addons.DialogueWheel.WheelDialogueView wheelDialogueView; //�� ���̾�α�, ���� �� ��� �뵵
    public Yarn.Unity.TextLineProvider textLineProvider;


    public List<ItemData> recieveItems; //��ȭ �� ���� �����۸���Ʈ

    private void Start()
    {
        textLineProvider.textLanguageCode = LocalizationSettings.SelectedLocale.Identifier.Code;
    }


    [YarnCommand("giveWatch")]
    public void GiveWatch()
    {
        giveWatch.Invoke(); //Ʃ�丮�󿡼� �ð� �ֱ�
    }
    [YarnCommand("giveBread")]
    public void GiveBread()
    {
        giveBread.Invoke(); //Ʃ�丮�󿡼� �ð� �ֱ�
    }
    [YarnCommand("completeSean")]
    public void CompleteSean()
    {
        completeSean.Invoke(); //Ʃ�丮�󿡼� �ð� �ֱ�
    }

    [YarnCommand("startBBQ")]
    public void StartBBQ()
    {
        startBbq.Invoke(); //ui�Ŵ������� bbq �г� ����
    }

    [YarnCommand("startChicken")]
    public void StartChicken()
    {
        startChicken.Invoke(); //ui�Ŵ������� chicken �г� ����
    }

    [YarnCommand("startCoffee")]
    public void StartCoffee()
    {
        startCoffee.Invoke(); //ui�Ŵ������� chicken �г� ����
    }
    [YarnCommand("startBrick")]
    public void StartBrick()
    {
        startBrick.Invoke(); //ui�Ŵ������� chicken �г� ����
    }

    [YarnCommand("startBiotest")]
    public void StartBiotest()
    {
        startBiotest.Invoke(); //ui�Ŵ������� ��ü���� �г� ����
    }
    [YarnCommand("startGas")]
    public void StartGas()
    {
        startGas.Invoke(); //ui�Ŵ������� ������ �г� ����
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
        PlayerStats.Spend(2000); //2000�� �Һ�
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
        PlayerStats.Spend(5000); //5000�� �Һ�
        StatusManager.instance.MoodleChange(11, true, 180); //3�ð� ��� ����
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
        dialogueRunner.dialogueViews[1] = wheelDialogueView; //�ٴ��̾�α׷� ����
    }

    [YarnCommand("changeBasic")]
    public void ChangeBasic()
    {
        dialogueRunner.dialogueViews[1] = optionsListView; //�����ɷ� �ٽ� ����
        StatusManager.instance.SetTodayDice(); //�ٽ� ���� ���� ����
    }

    #region Dept
    [YarnCommand("payDept")]
    public void PayDept() //�� ������ ���� �Լ�
    {
        int cost = PlayerStats.GetWeekDept();
        PlayerStats.Spend(cost);
        PlayerStats.SpendLastDept(false); //�� �� ����
        //HomeManager.instance?.DegenSean(); //������ ������
    }

    [YarnCommand("dontPayWeekDept")]
    public void DontPayWeekDept() //���� �����ؼ� �̹��� �� �ȳ�
    {
        PlayerStats.SpendLastDept(true); //�̹��� �� ����, ��� �Ѿ��� �ȱ���
        //HomeManager.instance?.DegenSean(); //������ ������
    }

    [YarnCommand("payNextWeek")]
    public void PayNextWeek() //���� �����ؼ� �̹��� �� �̿�
    {
        PlayerStats.TurnoverLastDept(); //�� �̿�
        //HomeManager.instance?.DegenSean(); //������ ������
    }

    [YarnCommand("cantPayGameOver")]
    public void CantPayGameOver() //���� ���� ȭ�� ����
    {
        UIManager.instance.GameOver(false);
    }

    [YarnCommand("degenSean")]
    public void DegenSean() //�� ������ �Լ�
    {
        HomeManager.instance?.DegenSean(); //������ ������
    }
    

    [YarnCommand("secondWind")]
    public void SecondWind() //���� �ѹ� �� �� �ߵ� �� ȣ��� �Լ�
    {
        StatusManager.instance.SetTodayDice(); //�ٽ� ���� ���� ����
        UIManager.instance.StartPerkNow(0);  //timeUi�� �߰�
    }
    [YarnCommand("gifActive")]
    public void GifActive() //���� ���� �� gif ����
    {
        UIManager.instance.NoticeCreatePersuade(); //���� GIF ����
    }
    #endregion

    //�ٷ� �Դ� ���ĵ�
    [YarnCommand("buyFishBread")]
    public void BuyFishBread() //�ؾ �������
    {
        PlayerStats.Spend(1000); //1000�� �Һ�
        FindObjectOfType<Throwing>().EatReadyCutscene(100); //�ؾ �Ա� ����
    }
    [YarnCommand("buyTanhulu")]
    public void BuyTanhulu() //�ؾ �������
    {
        PlayerStats.Spend(4000); //4000�� �Һ�
        FindObjectOfType<Throwing>().EatReadyCutscene(104); //���ķ� �Ա� ����
    }
    [YarnCommand("buyAmericano")]
    public void BuyAmericano() //�ؾ �������
    {
        PlayerStats.Spend(5000); //5000�� �Һ�
        FindObjectOfType<Throwing>().DrinkReadyCutscene(103); //Ŀ�� �Ա� ����
    }
    [YarnCommand("buyGimbab")]
    public void BuyGimbab() //��� �������
    {
        PlayerStats.Spend(4000); //5000�� �Һ�
        FindObjectOfType<Throwing>().EatReadyCutscene(105); //��� �Ա� ����
    }


    #region Police
    [YarnCommand("policePass")]
    public void PolicePass() //���� �����ؼ� ���� ������
    {
        ExtraManager.instance?.DegenPolice(); //���� ���ֱ�
    }
    [YarnCommand("policeMoney")]
    public void PoliceMoney() //���� �����ؼ� ���� ������
    {
        PlayerStats.Spend(50000); //50000�� �Һ�
        ExtraManager.instance?.DegenPolice(); //���� ���ֱ�
    }
    [YarnCommand("policeFail")]
    public void PoliceFail() //���� ���� �� ��ġ��
    {
        SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.Jail); //��ġ�� ����
    }
    [YarnCommand("getoutJail")]
    public void GetoutJail() //��¥ �� ä��� ��ġ�� ������
    {
        SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.Proto); //������
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
    [YarnCommand("tutoQue")]  //����Ʈ ���� �Լ�
    public void TutoQue(string name)
    {
        var questData = Resources.Load<QuestData>("Quests/" + name);
        if (questData != null)
        {
            QuestManager.instance.StartQuest(questData);
        }
        else
        {
            Debug.Log($"����Ʈ �����͸� ã�� �� �����ϴ�: Quests/{name}");
        }
    }


    [YarnCommand("questComplete")]
    public void QuestComplete(int id, int index)
    { 
        QuestManager.instance.CompleteObjective(id,index); //�ش� ����Ʈ �Ϸ�
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
            actionRisk: () => { StatusManager.instance.MoodleChange(12, true, 1440);  ExtraManager.instance.interObjects[0].SetActive(false); },  // ������ �� ����Ǵ� �׼�
            actionNone: () => {  ExtraManager.instance.interObjects[0].SetActive(false); }  // ������ �ƴ� �� ����Ǵ� �׼�
        ,1);
    }
    [YarnCommand("stealWallet")]
    public void StealWallet()
    {
        CutsceneManager.Instance.StartSimulateScene(simulsceneRisk[1], true,
            actionRisk: () => { ExtraManager.instance.GenPolice(); ExtraManager.instance.interObjects[1].SetActive(false); },  // ������ �� ����Ǵ� �׼�
            actionNone: () => { LootingManager.instance.GiveReward(wallets, 1); ExtraManager.instance.interObjects[1].SetActive(false); }  // ������ �ƴ� �� ����Ǵ� �׼�
        ,0,true);
    }
    [YarnCommand("poorMoney")]
    public void PoorMoney()
    {
        int money = Random.Range(5, 31) * 1000;
        CutsceneManager.Instance.StartSimulateScene(simulsceneRisk[2], true,
            actionRisk: () => { RelationshipStats.AddAllPoint(-250); ExtraManager.instance.GenSean(); PlayerStats.Earn(money); },  // ������ �� ����Ǵ� �׼�
            actionNone: () => { PlayerStats.Earn(money); }  // ������ �ƴ� �� ����Ǵ� �׼�
        , 2, true);
    }

    #endregion
}
