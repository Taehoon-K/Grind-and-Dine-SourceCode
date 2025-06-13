using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PP.InventorySystem;
using System;
using Newtonsoft.Json;
using Kupa;

public class GameTimeStateManager : MonoBehaviour,ITimeTracker
    //시간 따라 정적 변수 업데이트하는 클래스

{

    private int minutesElapsed = 0; //인터벌 업데이트
    //15분마다 호출될 이벤트
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

    public bool isDayChange; //프로토 매니저에서 하루 바뀔 시 업뎃하는 용도

    public static GameTimeStateManager instance = null;
    private void Awake()
    {
        if (instance != null && instance != this) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
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
        //블랙보드에 시간 정보 업뎃
        blackboard.SetValue(TIMESTAMP, timestamp);


        if(timestamp.hour == 7 && timestamp.minute == 0)
        {
            StartCoroutine(OnDayReset());
        }

        if (timestamp.hour == 0 && timestamp.minute == 0 && !isSleep) //만약 0시인데 안자고 있으면
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
            timestamp.minute == 0) //만약 봄일시
        {
            OnSpring();
        }

        if (timestamp.season == GameTimestamp.Season.Summer &&
            timestamp.GetDayOfTheWeek() == GameTimestamp.DayOfTheWeek.Monday &&
            timestamp.hour >= 7 &&
            timestamp.minute == 0) //만약 여름
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
    private IEnumerator OnDayReset() //페이드인 끝나고 시간 흘러야 실행되게 코루틴으로 수정
    {
        yield return new WaitUntil(() => TimeManager.instance.TimeTicking);

        Debug.Log("Day Reset");
        foreach(NPCRelationshipState npc in RelationshipStats.relationships)
        {
            npc.hasTalkedToday = false;
            npc.giftGivenToday = false;
        }

        StatusManager.instance.SetTodayLuck();
        StatusManager.instance.SetTodayDice(); //설득용 난수 주사위 리셋
        StatusManager.instance.SetCrimianl(-1); //범죄 수치 1 감소

        if (PlayerStats.needToSpawnSean)
        {
            HomeManager.instance.GenSean(); //밀려났던 빚쟁이 실행
            PlayerStats.needToSpawnSean = false;
        }


        if (JailManager.instance != null)
        {
            JailManager.instance.SkipDay();//만약 감옥에있다면 하루 카운트
        }
        
    }

    private void OnWeekReset()
    {
        Debug.Log("Week Reset");
        foreach (NPCRelationshipState npc in RelationshipStats.relationships)
        {
            npc.giftGivenTwiceWeek = 0; //선물 횟수 초기화
        }

        if (PlayerStats.Initilaze()) //만약 빚 다 갚았다면
        {

        }
        else
        {
            //빚쟁이 생성 후 말걸기
            if (HomeManager.instance != null)
            {
                HomeManager.instance.GenSean();//
            }
            else //만약 감옥이면
            {
                PlayerStats.needToSpawnSean = true;
            }
        }
    }

    public void Sleep(int sleepTime) //낮잠 자고 저장
    {
       // TimeManager.instance.TimeTicking = false;
        //TimeManager.instance.SkipTime(sleepTime); //시간 보내는 함수 호출
        SaveManager.Save(ExportSaveState(),0); //오토세이브에 저장

        AdvancedPeopleSystem.CharacterCustomization cc = gameObject.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 1); //1번 슬롯에 저장
    }
    public void SleepUntilSix(bool isJail) //7시까지 취침
    {
        isSleep = true;
        //TimeManager.instance.SkipTimeToSix(); //시간 보내는 함수 호출
        //isSleep = false;

        SaveManager.Save(ExportSaveState(), 0); //오토세이브에 저장

        if (!isJail)
        {
            AdvancedPeopleSystem.CharacterCustomization cc = gameObject.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
            cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 1); //1번 슬롯에 저장
        }
    }
    public void OnIsSleep(bool on)
    {
        isSleep = on;
    }
    public void JustSave(int index) //그냥 저장
    {
        SaveManager.Save(ExportSaveState(),index);
        AdvancedPeopleSystem.CharacterCustomization cc = gameObject.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", index+1); //2,3,4번 슬롯에 저장
    }

    public GameSaveState ExportSaveState() //세이브 데이터 추출 함수
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


        //인벤토리 불러오기
        string json = save.inven;;
        InventorySaveData saveInven = JsonUtility.FromJson<InventorySaveData>(json);
        Inventory.instance.LoadInventory(saveInven);

        //npc 관계 로드
        RelationshipStats.LoadStats();
       // RelationshipStats.LoadStats(save.relationships);
        //비밀 로드
        IKnowAbout.LoadSecret(save.secrets);
        QuestManager.instance.LoadQuests(save.questData);

        LootingManager.instance.LoadLootingObjects(save.lootingObjects);
    }

    private void OnSpring() //봄일시 호출
    {

    }
    private void OnSummer() //여름에 호출
    {

    }
    private void OnFall() //가을에 호출
    {

    }
    private void OnWinter() //겨울에 호출
    {

    }
    private void DemoEnd() //데모 엔딩
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

    private void OnStun() //플레이어 기절
    {
        player.OnFaint();
    }
}
