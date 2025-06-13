using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PP.InventorySystem;
using System;
using Newtonsoft.Json;
using Kupa;

public class GameTimeStateManager : MonoBehaviour,ITimeTracker
    //�ð� ���� ���� ���� ������Ʈ�ϴ� Ŭ����

{

    private int minutesElapsed = 0; //���͹� ������Ʈ
    //15�и��� ȣ��� �̺�Ʈ
    public UnityEvent onIntervalUpdate;

    [SerializeField]
    GameBlackboard blackboard = new GameBlackboard();

    const string TIMESTAMP = "Timestamp";
    public GameBlackboard GetBlackboard()
    {
       // Debug.Log(blackboard.entries.Values);
        return blackboard;
    }

    [SerializeField]
    private Player player;

    private bool isSleep;

    public bool isDayChange; //������ �Ŵ������� �Ϸ� �ٲ� �� �����ϴ� �뵵

    public static GameTimeStateManager instance = null;
    private void Awake()
    {
        if (instance != null && instance != this) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        //questEvents = new QuestEvents();
    }
    void Start()
    {
        TimeManager.instance.RegisterTracker(this);
    }

    public void ClockUpdate(GameTimestamp timestamp)
    {
        //�����忡 �ð� ���� ����
        blackboard.SetValue(TIMESTAMP, timestamp);


        if(timestamp.hour == 7 && timestamp.minute == 0)
        {
            StartCoroutine(OnDayReset());
        }

        if (timestamp.hour == 0 && timestamp.minute == 0 && !isSleep) //���� 0���ε� ���ڰ� ������
        {
            OnStun();
        }

        if (timestamp.GetDayOfTheWeek() == GameTimestamp.DayOfTheWeek.Monday &&
            timestamp.hour == 7 && timestamp.minute == 0)
        {
            OnWeekReset();
        }

        if(timestamp.season == GameTimestamp.Season.Spring && 
            timestamp.GetDayOfTheWeek() == GameTimestamp.DayOfTheWeek.Monday &&
            timestamp.hour == 7 && 
            timestamp.minute == 0) //���� ���Ͻ�
        {
            OnSpring();
        }

        if (timestamp.season == GameTimestamp.Season.Summer &&
            timestamp.GetDayOfTheWeek() == GameTimestamp.DayOfTheWeek.Monday &&
            timestamp.hour >= 7 &&
            timestamp.minute == 0) //���� ����
        {
            DemoEnd();
        }

        if (minutesElapsed >= 15)
        {
            minutesElapsed = 0;
            onIntervalUpdate?.Invoke();
        }
        else
        {
            minutesElapsed++;
        }
    }
    private IEnumerator OnDayReset() //���̵��� ������ �ð� �귯�� ����ǰ� �ڷ�ƾ���� ����
    {
        yield return new WaitUntil(() => TimeManager.instance.TimeTicking);

        Debug.Log("Day Reset");
        foreach(NPCRelationshipState npc in RelationshipStats.relationships)
        {
            npc.hasTalkedToday = false;
            npc.giftGivenToday = false;
        }

        StatusManager.instance.SetTodayLuck();
        StatusManager.instance.SetTodayDice(); //����� ���� �ֻ��� ����
        StatusManager.instance.SetCrimianl(-1); //���� ��ġ 1 ����

        if (PlayerStats.needToSpawnSean)
        {
            HomeManager.instance.GenSean(); //�з����� ������ ����
            PlayerStats.needToSpawnSean = false;
        }


        if (JailManager.instance != null)
        {
            JailManager.instance.SkipDay();//���� �������ִٸ� �Ϸ� ī��Ʈ
        }
        
    }

    private void OnWeekReset()
    {
        Debug.Log("Week Reset");
        foreach (NPCRelationshipState npc in RelationshipStats.relationships)
        {
            npc.giftGivenTwiceWeek = 0; //���� Ƚ�� �ʱ�ȭ
        }

        if (PlayerStats.Initilaze()) //���� �� �� ���Ҵٸ�
        {

        }
        else
        {
            //������ ���� �� ���ɱ�
            if (HomeManager.instance != null)
            {
                HomeManager.instance.GenSean();//
            }
            else //���� �����̸�
            {
                PlayerStats.needToSpawnSean = true;
            }
        }
    }

    public void Sleep(int sleepTime) //���� �ڰ� ����
    {
       // TimeManager.instance.TimeTicking = false;
        //TimeManager.instance.SkipTime(sleepTime); //�ð� ������ �Լ� ȣ��
        SaveManager.Save(ExportSaveState(),0); //���似�̺꿡 ����

        AdvancedPeopleSystem.CharacterCustomization cc = gameObject.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 1); //1�� ���Կ� ����
    }
    public void SleepUntilSix(bool isJail) //7�ñ��� ��ħ
    {
        isSleep = true;
        //TimeManager.instance.SkipTimeToSix(); //�ð� ������ �Լ� ȣ��
        //isSleep = false;

        SaveManager.Save(ExportSaveState(), 0); //���似�̺꿡 ����

        if (!isJail)
        {
            AdvancedPeopleSystem.CharacterCustomization cc = gameObject.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
            cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 1); //1�� ���Կ� ����
        }
    }
    public void OnIsSleep(bool on)
    {
        isSleep = on;
    }
    public void JustSave(int index) //�׳� ����
    {
        SaveManager.Save(ExportSaveState(),index);
        AdvancedPeopleSystem.CharacterCustomization cc = gameObject.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", index+1); //2,3,4�� ���Կ� ����
    }

    public GameSaveState ExportSaveState() //���̺� ������ ���� �Լ�
    {      
        GameTimestamp timestamp = TimeManager.instance.GetGameTimestamp();
        int money = PlayerStats.GetMoney();
        string inven = Inventory.instance.SaveInventory();
        string name = PlayerStats.GetName();
        int diffy = PlayerStats.GetDiffy();
        //List<NPCRelationshipState> relationships = RelationshipStats.GetRelationShip();
        List<bool> secrets = IKnowAbout.GetSecret();
        QuestSaveWrapper questData = QuestManager.instance.SaveQuests();
        int totalMoney = PlayerStats.GetTotalMoney();
        int totalDept = PlayerStats.GetTotalDept();
        int weekDept = PlayerStats.GetWeekDept();
        int failDept = PlayerStats.GetFailDept();
        GameBlackboard sd = blackboard;
        Moodle[] moodle = StatusManager.instance.GetMoodle();
        Status stat = StatusManager.instance.GetStatus();
        bool sex = PlayerStats.GetSex();
        Dictionary<string, GameTimestamp> lootingObjects = LootingManager.instance.GetLootingObjects();
        return new GameSaveState(sd,timestamp,money,inven,name,diffy, secrets, questData, totalMoney,
            totalDept,weekDept,failDept,moodle,stat, sex, lootingObjects);
    }

    public void LoadSave(int index)
    {
        Debug.Log("Load Saveeeeeeeeeeeeeeeeeeee");
        GameSaveState save = SaveManager.Load(index);
        //RestoreFromSerializableDictionary(save.blackboard);
        blackboard = save.blackboard;
        //time
        TimeManager.instance.LoadTime(save.timestamp);
        PlayerStats.LoadMoney(save.money);
        PlayerStats.SetDiffy(save.difficulty);
        PlayerStats.SetName(save.name);
        PlayerStats.LoadTotalMoney(save.totalMoney);
        PlayerStats.LoadTotalDept(save.totalDept);
        PlayerStats.LoadWeekDept(save.weekDept);
        PlayerStats.LoadFailDept(save.failDept);
        StatusManager.instance.LoadMoodle(save.moodle);
        StatusManager.instance.LoadStatus(save.stat);
        PlayerStats.SetSex(save.sex);


        //�κ��丮 �ҷ�����
        string json = save.inven;;
        InventorySaveData saveInven = JsonUtility.FromJson<InventorySaveData>(json);
        Inventory.instance.LoadInventory(saveInven);

        //npc ���� �ε�
        RelationshipStats.LoadStats();
       // RelationshipStats.LoadStats(save.relationships);
        //��� �ε�
        IKnowAbout.LoadSecret(save.secrets);
        QuestManager.instance.LoadQuests(save.questData);

        LootingManager.instance.LoadLootingObjects(save.lootingObjects);
    }

    private void OnSpring() //���Ͻ� ȣ��
    {

    }
    private void OnSummer() //������ ȣ��
    {

    }
    private void OnFall() //������ ȣ��
    {

    }
    private void OnWinter() //�ܿ￡ ȣ��
    {

    }
    private void DemoEnd() //���� ����
    {
        UIManager.instance.DemoOver();
    }
    
    /*private SerializableDictionary SaveToPlayerPrefs(GameBlackboard blackboard)
    {
        SerializableDictionary sd = new SerializableDictionary();

        foreach (KeyValuePair<string, object> kvp in blackboard.entries)
        {
            sd.keys.Add(kvp.Key);
            sd.values.Add(JsonUtility.ToJson(kvp.Value));
        }

        return sd;
    }
    public void RestoreFromSerializableDictionary(SerializableDictionary sd)
    {
        //blackboard.entries = new Dictionary<string, object>();

        for (int i = 0; i < sd.keys.Count; i++)
        {
            // blackboard.entries[sd.keys[i]] = JsonUtility.FromJson(sd.values[i], typeof(object));
            blackboard.SetValue(sd.keys[i], JsonUtility.FromJson(sd.values[i], typeof(object)));
        }
        
    }*/

    private void OnStun() //�÷��̾� ����
    {
        player.OnFaint();
    }
}
