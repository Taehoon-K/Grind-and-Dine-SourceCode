using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FLook;
using static UIManager;
using UnityEngine.Localization;
using Yarn.Unity;
using Unity.VisualScripting;

//using System;

public class StatusManager : MonoBehaviour,ITimeTracker
{
    private int[] experienceRequired = { 100, 280, 390, 530, 850, 1150, 1500, 2100, 3100, 5000 };
    private int[] totalExperience = { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 }; //경험치 양

    Status status;
    private Moodle[] moodle = new Moodle[14];

    [SerializeField]
    private int total = 1000;

    // 스태미나 증가량
    [SerializeField]
    private int spIncreaseSpeed;

    // 스태미나 재회복 딜레이 시간
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부
    public bool isRun = false;

    //관찰자 패턴
    List<IStatusTracker> listeners = new List<IStatusTracker>();

    [Header("Character")]
    [SerializeField] private GameObject woman,man;

    [Header("Perk")]
    [SerializeField] private List<PerkOption> perkOptions; // PerkOption 데이터 리스트

    [Header("SimulationStat")]
    [SerializeField] private List<SimulationStat> simulationStats; //시뮬레이션 시 스탯 증감 모아둔 구조체
    [SerializeField] private List<SimulationStat> riskStat; //시뮬레이션 시 스탯 증감 모아둔 구조체
    [SerializeField] private List<SimulationStat> eatStats; //그냥 먹을 시 스탯 증감 모아둔 구조체

    public static StatusManager instance = null;

    private int moodUpdateCounter = 0; // 호출 횟수를 추적하는 변수

    private bool hasloaded = false;  // 게임이 로드된 후에 시간에 따라 함수 호출되도록 하기

    private InMemoryVariableStorage variableStorage;
    private int currentHour; //현재 몇시인지. yarn에 쓸것
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
        status = new Status(); // 객체 초기화는 여기서 수행

        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        // InitializePerkStates();
    }
    public enum SleepState
    {
        Awake,        // 기본 상태
        Sleep,
        Simulate
    }
    public SleepState currentSleepState = SleepState.Awake; // 현재 UI 상태
    void Start()
    {
        TimeManager.instance.RegisterTracker(this);

        /*status.currentHp = total;
        status.currentFatigue = total;
        status.currentHungry = total;
        status.currentSp = total;*/

       /* status.level[2] = 5;
        status.skillAmount[2] = 100;
        status.skillAmount[3] = 30; //테스트*/

        if(TutorialManager.instance != null) //만약 튜토리얼이면
        {
            status.currentHp = total;
            status.currentFatigue = total;
            status.currentHungry = 560;
            status.currentSp = total; //채우고 시작
        }

        //variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        //Debug.Log("Find Viraviole storage");
    }

    public void LoadGameStart(int index) //로드해서 겜 시작할때 호출
    {
        var player = FindObjectOfType<Kupa.Player>();
        GameObject prefab;
        if (PlayerStats.IsWoman)
        {
            //  cc.SwitchCharacterSettings(1); //0이면 남자 1이면 여자
            prefab = Instantiate(woman, player.transform);

        }
        else
        {
            //  cc.SwitchCharacterSettings(0); //0이면 남자 1이면 여자
            prefab = Instantiate(man, player.transform);
        }
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        //prefab.gameObject.transform = player.modelTransform;
        player.GetTransform();

        Debug.Log("플레이어 마네킹 생성어어어어어어어엉");
        //프리팹 붙이고 바로 player에서 콜라이더 비활성화 실행
        player.GetComponent<Kupa.Player>().SetupRagdoll();

        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();

        var saved = cc.GetSavedCharacterDatas();

        // 해당 이름을 가진 데이터가 있는지 확인
        for (int i = 0; i < saved.Count; i++)
        {
            //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
            if (saved[i].name == (index+1).ToString())
            {
                cc.ApplySavedCharacterData(saved[i]); // 원하는 이름의 캐릭터 데이터 적용
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // 찾으면 더 이상 반복하지 않고 종료
            }
        }

        

        player.eye = FindChildByName(player.gameObject, "Head");

        ChangeLayerRecursively(player.gameObject, 6); //레이어 바꾸기
        Transform thirdChild = player.gameObject.transform.GetChild(2);
        // 세 번째 자식의 Animator 컴포넌트 가져오기
        Animator animator = thirdChild.GetComponent<Animator>();
        // Animator 컴포넌트가 있으면 삭제
        if (animator != null)
        {
            Destroy(animator);
        }

        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Animator>().enabled = true;
        // Animator 상태를 리셋하여 다시 재생할 수 있도록 함
        player.GetComponent<Animator>().Rebind();
        player.GetComponent<Animator>().Update(0); // 업데이트를 강제로 호출해 즉시 반영

        hasloaded = true;

        RenderDice(); //가변저장소 업뎃
        //RenderDept();

        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 5); //5번 슬롯에 저장
    }

    public void FirstGameStart()  //게임 처음 시작할때 호출될 함수
    {
        status.currentHp = total;
        status.currentFatigue = total;
        status.currentHungry = total;
        status.currentSp = total;

        for (int i = 0; i < moodle.Length; i++)
        {
            moodle[i] = new Moodle { isActive = false, timeLeft = 0 }; // 기본값으로 초기화
        }
        InitializePerkStates(); //퍽 초기화

        var player = FindObjectOfType<Kupa.Player>();
        GameObject prefab;
        
        if (PlayerStats.IsWoman)
        {
            // cc.SwitchCharacterSettings(1); //0이면 남자 1이면 여자
            prefab = Instantiate(woman, player.transform.localPosition, player.transform.localRotation, player.transform);

        }
        else
        {
            // cc.SwitchCharacterSettings(0); //0이면 남자 1이면 여자
            prefab = Instantiate(man, player.transform.localPosition, player.transform.localRotation, player.transform);
        }
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        player.GetTransform();

        Debug.Log("플레이어 마네킹 생성어어어어어어어엉");
        //프리팹 붙이고 바로 player에서 콜라이더 비활성화 실행
        player.GetComponent<Kupa.Player>().SetupRagdoll();

        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
        var saved = cc.GetSavedCharacterDatas();

        // 해당 이름을 가진 데이터가 있는지 확인
        for (int i = 0; i < saved.Count; i++)
        {
            if (saved[i].name == "0")
            {
                cc.ApplySavedCharacterData(saved[i]); // 원하는 이름의 캐릭터 데이터 적용
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // 찾으면 더 이상 반복하지 않고 종료
            }
        }


        //  FLookAnimator lookAni = player.gameObject.GetComponent<FLookAnimator>();
        //  lookAni.LeadBone = FindChildByName(player.gameObject, "Head");
        //  player.eye = lookAni.LeadBone;

        player.eye = FindChildByName(player.gameObject, "Head");

        ChangeLayerRecursively(player.gameObject, 6); //레이어 바꾸기
        Transform thirdChild = player.gameObject.transform.GetChild(2);
        // 세 번째 자식의 Animator 컴포넌트 가져오기
        Animator animator = thirdChild.GetComponent<Animator>();
        // Animator 컴포넌트가 있으면 삭제
        if (animator != null)
        {
            Destroy(animator);
        }
        
        //lookAni.enabled = false;
        //lookAni.enabled = true;
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Animator>().enabled = true;
        // Animator 상태를 리셋하여 다시 재생할 수 있도록 함
        player.GetComponent<Animator>().Rebind();
        player.GetComponent<Animator>().Update(0); // 업데이트를 강제로 호출해 즉시 반영

        hasloaded = true;

        //RenderDept();

        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 5); //5번 슬롯에 저장
    }

    Transform FindChildByName(GameObject parent, string targetName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true)) // true: 비활성화된 자식 포함
        {
            if (child.name == targetName)
            {
                return child; // 이름이 일치하는 자식(혹은 손자) 오브젝트 반환
            }
        }
        return null; // 찾지 못하면 null 반환
    }
    void ChangeLayerRecursively(GameObject obj, int newLayer)
    {
        // 해당 오브젝트의 레이어 변경
        //obj.layer = newLayer;

        // 모든 자식들의 레이어 변경
        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true)) // true: 비활성화된 자식들도 포함
        {
            child.gameObject.layer = newLayer;
        }
    }

    public void ClockUpdate(GameTimestamp timestamp) //매 초마다 호출될 함수
    {
        if (!hasloaded)
        {
            return;
        }

        Hp();
        AddHungry(-0.2f,true);
        if (currentSleepState == SleepState.Awake) 
        {
            AddFatigue(-0.2f, true);
        }
        else if(currentSleepState == SleepState.Sleep) //자는 상태면
        {
            if (moodle[1].isActive) //만약 불면증 활성화되있으면
            {
                AddFatigue(0.85f, true); //반만 회복
            }
            else
            {
                AddFatigue(1.7f, true);
            }

            if (GetSelectedPerk(6, 1) == 1) //만약 10-2퍽 활성화되있으면
            {
                AddAngry(-1);
                AddSadness(-1);
                AddBoredom(-1);
            }
            else
            {
                if(PlayerStats.Difficulty == 0) //만약 쉬움 난이도일때만 자면 감정 회복
                {
                    moodUpdateCounter++;
                    if (moodUpdateCounter >= 20) // 두 번 호출되었을 때
                    {
                        AddAngry(-1);
                        AddSadness(-1);
                        AddBoredom(-1);
                        moodUpdateCounter = 0; // 카운터 초기화
                    }
                }
                
            }
        }
        MoodleDecrease(); //무들 남은 시간 줄이기

        UIManager.instance.canRunOnOff(moodle[5].isActive); //근육통 무들 따라 이미지 껐다켰다하기

        if (timestamp.hour != currentHour)  //만약 시간 바뀌면
        {
            currentHour = timestamp.hour;
            variableStorage.SetValue("$hour", currentHour);  //yarn에 시간 업데이트
        }

        RenderStat();//가변저장소에 현재 수치 업뎃
    }
    private void FixedUpdate() //고정 프레임마다 호출되는 함수
    {
        Stamina();
  

        foreach (IStatusTracker listener in listeners) //ui에게 수치들 전송
        {
            listener.StatusUpdate(status);
        }
    }

    public Status GetStatus()
    {
        return status;
    }
    public void LoadStatus(Status statusLoad)
    {
        if (statusLoad == null)
        {
            status = new Status();
            InitializePerkStates();
            return;
        }
        status = statusLoad;

        RenderPerk(); //가변저장소 업뎃
        RenderCharmLevel(); //가변저장소 업뎃
        RenderStat();//가변저장소 업뎃
    }

    public void DestroyMoodleOne() //무들 하나 랜덤으로 지우기, 퍽 함수
    {
        // 1. isActive가 true인 인덱스를 리스트로 수집
        List<int> activeIndexes = new List<int>();
        for (int i = 0; i < moodle.Length; i++)
        {
            if (moodle[i].isActive)
            {
                activeIndexes.Add(i);
            }
        }

        // 2. 아무것도 활성화된 게 없다면 종료
        if (activeIndexes.Count == 0)
            return;

        // 3. 랜덤으로 하나 선택해서 비활성화
        int randomIndex = activeIndexes[Random.Range(0, activeIndexes.Count)];

        MoodleChange(randomIndex,false); //랜덤 활성화 무들 하나 지우기
    }
    public void MoodleChange(int index, bool bol, int time = 0, bool isSet = false) //상태이상 바꾸는 함수, isSet = 시간을 더하는 것이 아니라 지정
    {
        //moodle[index].isActive = bol;
        if (bol)
        {
            if(index == 10 && GetSelectedPerk(0, 1) == 1) //만약 취함 면역 퍽 찍혀있다면
            {
                return;
            }

            if (!moodle[index].isActive)
            {
                moodle[index].isActive = true;
                UIManager.instance.NoticeItemCreate(6, index);
            }
            if (!isSet)
            {
                moodle[index].timeLeft += time;
            }
            else
            {
                moodle[index].timeLeft = time;
            }
            moodle[index].timeLeft = Mathf.Min(moodle[index].timeLeft, 72*60); //72시간 넘지 않게 고정

            if (moodle[index].timeLeft <= 0)
            {
                if (moodle[index].isActive) //만약 켜져있었다면
                {
                    moodle[index].isActive = false;
                    UIManager.instance.NoticeItemCreate(7, index);
                }
                moodle[index].isActive = false;
                moodle[index].timeLeft = 0; 
            }
        }
        else
        {
            if (moodle[index].isActive) //만약 켜져있었다면
            {
                moodle[index].isActive = false;
                UIManager.instance.NoticeItemCreate(7, index);
            }
            moodle[index].timeLeft = 0; //무들 남은시간 0으로 바꿈
        }
        UIManager.instance.RenderPlayerStats();
        RenderMoodle(); //다이얼로그 가변저장소 업뎃
    }
    public void AddMoodle(MoodleImform[] moodleImforms) //음식 등으로 무들 변동 시 호출할 함수
    {
        if (moodleImforms == null || moodleImforms.Length == 0)
            return; // 배열이 비어있으면 처리하지 않음

        foreach (MoodleImform moodleImform in moodleImforms)
        {
            if (moodleImform != null && moodleImform.timeLeft != 0) // //만약 무들 타임이 0이 아닌 경우에만, 0일때는 isacitve든 아니든 걍 무시
            {
                if (Random.value < moodleImform.probability) // 확률 체크
                {
                    MoodleChange(moodleImform.index, moodleImform.isActive, moodleImform.timeLeft * 60);
                }
            }
        }
    }
    public Moodle[] GetMoodle()
    {
        return moodle; //상태이상 배열 리턴
    }
    public void LoadMoodle(Moodle[] mood) //세이브 로드시 상태이상 로드
    {
        //moodle = mood;

        // 기존 mood 배열의 내용을 새 배열로 복사 (최대 10개까지만 복사)
        for (int i = 0; i < mood.Length; i++)
        {
            moodle[i] = mood[i];
        }
        UIManager.instance.RenderPlayerStats();
        RenderMoodle(); //스탯매니저 무들 업뎃
    }
    
    public int GetHp() //피로도 얻는 코드
    {
        return (int)status.currentFatigue;
    }

    public void AddHungry(float point, bool isNatural = false)
    {
        if (moodle[2].isActive && point < 0) //만약 폭식증이면, 그리고 감소 시
        {
            status.currentHungry += point * 1.3f; //1.5배 빨리 상승

            if (!isNatural) //만약 자연스러운 증감 아니라면
            {
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
                {
                    UIManager.instance.NoticeItemCreate(2, (int)(point * 1.3f / 10));
                }
                
            }
        }
        else if (point < 0)
        {
            status.currentHungry += point;

            if (!isNatural) //만약 자연스러운 증감 아니라면
            {
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
                {
                    UIManager.instance.NoticeItemCreate(2, (int)point / 10);
                }
                
            }
        }
        else if(point > 0)
        {
            if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
            {
                UIManager.instance.NoticeItemCreate(2, (int)point / 10);
            }
            StartCoroutine(SmoothIncrease(status.currentHungry, point, total, newValue => status.currentHungry = newValue));
        }
        status.currentHungry = Mathf.Max(0, Mathf.Min(status.currentHungry, total));
    }

    public void AddFatigue(float point, bool isNatural = false)
    {
        //Debug.Log(point);
        if(point < 0 && GetSelectedPerk(0,0) == 0) //만약 피로도 증가에 피로감소 퍽 찍혀있다면
        {
            //Debug.Log("PiroPErkkkkkkkkkkk");
            point *= 0.8f;
        }

        if (moodle[0].isActive && point < 0) //만약 만성피로면
        {
            status.currentFatigue += point * 1.3f; //1.5배 빨리 상승

            if (!isNatural) //만약 자연스러운 증감 아니라면
            {
                
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
                {
                    UIManager.instance.NoticeItemCreate(1, (int)(point * 1.3f / 10));
                }
            }
        }
        else if (point < 0)
        {
            status.currentFatigue += point;
            if (!isNatural) //만약 자연스러운 증감 아니라면
            {
                
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
                {
                    UIManager.instance.NoticeItemCreate(1, (int)point / 10);
                }
            }
        }
        else if (point > 0)
        {
            if (isNatural)
            {
                status.currentFatigue += point;
            }
            else
            {
                
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
                {
                    UIManager.instance.NoticeItemCreate(1, (int)point / 10);
                }
                StartCoroutine(SmoothIncrease(status.currentFatigue, point, total, newValue => status.currentFatigue = newValue));
            }
        }
        //Debug.Log(status.currentFatigue+ " aaasdf");
        status.currentFatigue = Mathf.Max(0, Mathf.Min(status.currentFatigue, total));
        //Debug.Log(status.currentFatigue+" bbbbb");
    }
    public void AddAngry(int point, bool isBonus = false)
    {
        if(point != 0 && point != -1) //만약 잘때 1씩 감소하는것도 아니면
        {
            if(UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
            {
                UIManager.instance.NoticeItemCreate(3, point / 10);
            }
            
        }

        if (point > 0 && GetSelectedPerk(6, 0) == 1 && !isBonus) //만약 지능 5-2퍽 찍혀있다면
        {
            point = (int)(point * 0.8f);
        }
        if (moodle[6].isActive && point > 0 && !isBonus) //만약 부정기분 증가고 골초 특성 켜져있다면
        {
            point = (int)(point * 1.5f);
        }
        if (moodle[7].isActive && point > 0 && !isBonus) //만약 부정기분 증가고 알코올중독 특성 켜져있다면
        {
            point = (int)(point * 1.5f);
        }
        status.currentAngry += point;
        status.currentAngry = Mathf.Max(0, Mathf.Min(status.currentAngry, total));

        if (moodle[8].isActive && point > 0 && !isBonus)
        {
            int ran = Random.Range(0, 2);
            if (ran == 0)
            {
                AddSadness(point, true);
            }
            else
            {
                AddBoredom(point, true);
            }
        }
    }
    public void AddSadness(int point, bool isBonus = false)
    {
        if (point != 0 && point != -1) //만약 잘때 1씩 감소하는것도 아니면
        {
            if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
            {
                UIManager.instance.NoticeItemCreate(4, point / 10);
            }     
        }

        if (point > 0 && GetSelectedPerk(6, 0) == 1 && !isBonus) //만약 지능 5-2퍽 찍혀있다면
        {
            point = (int)(point * 0.8f);
        }
        if (moodle[6].isActive && point > 0 && !isBonus) //만약 부정기분 증가고 골초 특성 켜져있다면
        {
            point = (int)(point * 1.5f);
        }
        if (moodle[7].isActive && point > 0 && !isBonus) //만약 부정기분 증가고 알코올중독 특성 켜져있다면
        {
            point = (int)(point * 1.5f);
        }
        status.currentSadness += point;
        status.currentSadness = Mathf.Max(0, Mathf.Min(status.currentSadness, total));

        if (moodle[8].isActive && point > 0 && !isBonus)
        {
            int ran = Random.Range(0, 2);
            if (ran == 0)
            {
                AddAngry(point, true);
            }
            else
            {
                AddBoredom(point, true);
            }
        }
    }
    public void AddBoredom(int point, bool isBonus = false) //isBonus 트루로 전송시 무들땜에 보너스로 올라가는것
    {
        if (point != 0 && point != -1) //만약 잘때 1씩 감소하는것도 아니면
        {
            
            if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui매니저 활성화일시만
            {
                UIManager.instance.NoticeItemCreate(5, point / 10);
            }
        }

        if (point > 0 && GetSelectedPerk(6, 0) == 1 && !isBonus) //만약 지능 5-2퍽 찍혀있다면
        {
            point = (int)(point * 0.8f);
        }
        if (moodle[6].isActive && point > 0 && !isBonus) //만약 부정기분 증가고 골초 특성 켜져있다면
        {
            point = (int)(point * 1.5f);
        }
        if (moodle[7].isActive && point > 0 && !isBonus) //만약 부정기분 증가고 알코올중독 특성 켜져있다면
        {
            point = (int)(point * 1.5f);
        }
        status.currentBoredom += point;
        status.currentBoredom = Mathf.Max(0, Mathf.Min(status.currentBoredom, total));

        if (moodle[8].isActive && point > 0 && !isBonus)
        {
            int ran = Random.Range(0, 2);
            if (ran == 0)
            {
                AddAngry(point, true);
            }
            else
            {
                AddSadness(point, true);
            }
        }
    }

    private IEnumerator SmoothIncrease(float current, float point, float total, System.Action<float> updateValueCallback)
        //게이지 부드럽게 올리는 용도
    {
        float duration = 1.0f; // 게이지가 증가하는 데 걸리는 시간 (초)
        float startHungry = current;
        float targetHungry = Mathf.Clamp(startHungry + point, 0, total); // 최소 0, 최대 total로 제한

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startHungry, targetHungry, elapsedTime / duration);
            updateValueCallback(newValue); // 값을 업데이트하는 콜백 호출
            yield return null;
        }

        // 최종 값 보정
        updateValueCallback(targetHungry);
    }

    private void Hp()
    {
        //Debug.Log(status.currentHp + "aadaasd" + status.currentFatigue + "     " + status.currentHungry + "     " + status.currentAngry + "     " + status.currentSadness + "     " + status.currentBoredom);
        if (status.currentHp > 0)
        {
            int zeroCount = 0;
            if (status.currentFatigue == 0) zeroCount++;
            if (status.currentHungry == 0) zeroCount++;
            if (status.currentAngry == total) zeroCount++;
            if (status.currentSadness == total) zeroCount++;
            if (status.currentBoredom == total) zeroCount++;

            if (zeroCount >= 1)
            {
                if(currentSleepState == SleepState.Awake) //깨어있을 때만 감소
                {
                    status.currentHp -= zeroCount * 3;
                }
                
            }
            else
            {
                if (currentSleepState != SleepState.Simulate) //시뮬레이션때는 증가 안하게
                {
                    status.currentHp += 3;
                }
                
            }
            
        }
        else
        {
            Debug.Log("HP 수치가 0 이 되었습니다.");

            var player = FindObjectOfType<Kupa.Player>();
            player.GetComponent<Kupa.Player>().OnDead(); //플레이어 뒤짐 실행

            status.currentHp = 500;
            if (status.currentFatigue < 500) status.currentFatigue = 500;
            if (status.currentHungry < 500) status.currentHungry = 500;
            if (status.currentAngry > 500) status.currentAngry = 500;
            if (status.currentSadness > 500) status.currentSadness = 500;
            if (status.currentBoredom > 500) status.currentBoredom = 500;
        }
           

        status.currentHp = Mathf.Max(0, Mathf.Min(status.currentHp, total));
    }

    private void MoodleDecrease()
    {
        for (int i = 0; i < moodle.Length; i++)
        {
            if (moodle[i].isActive)
            {
                // timeLeft 감소
                moodle[i].timeLeft--;

                // timeLeft가 0이 되면 isActive를 false로 전환
                if (moodle[i].timeLeft <= 0)
                {
                    UIManager.instance.NoticeItemCreate(7, i);
                    moodle[i].isActive = false;
                    moodle[i].timeLeft = 0; // timeLeft를 음수로 내려가지 않게 함
                }
            }
        }
    }

    private void Stamina()
    {
        if (isRun == true)//달리는중
        {
            currentSpRechargeTime = 0;

            if (GetSelectedPerk(0, 0) == 1)
            {
                //Debug.Log("Stamian Perkkkkkkkk");
                status.currentSp -= 4; //절반만 감소

            }
            else
            {
                status.currentSp -= 8;
            }
        }
        else
        {
            
            if (currentSpRechargeTime >= spRechargeTime)
            {
                status.currentSp += spIncreaseSpeed;
                status.currentSp = Mathf.Max(0, Mathf.Min(status.currentSp, total));
            }
            else
            {
                currentSpRechargeTime++;
            }
        }
    }

    public int GetCurrentSP()
    {
        return status.currentSp;
    }
    public void RegisterTracker(IStatusTracker listener)
    {
        listeners.Add(listener);
    }
    public void UnregisterTracker(IStatusTracker listener)
    {
        listeners.Remove(listener);
    }

    public float GetExperienceProgress(int index) //경험치 바 계산하는 용도
    {
        int currentExperience = status.skillAmount[index];

        if (status.level[index] == 10)
        {
            return 1f;
        }
        else if (status.level[index] > 0)
        {
            //int previousTotalExperience = totalExperience[status.level[index] - 1];
            int nextLevelExperience = experienceRequired[status.level[index]];
            //int experienceInCurrentLevel = currentExperience - previousTotalExperience;
            
            return (float)currentExperience / nextLevelExperience;
        }
        else
        {
            return (float)currentExperience / experienceRequired[0];
        }
    }

    public void AddExperience(int skillIndex, int experience)
    {
        if(status.level[skillIndex] == 10) //만렙이면 리턴
        {
            return;
        }
        int currentExperience = status.skillAmount[skillIndex];
        int newExperience = currentExperience + experience; //합쳐진 양
        int nextLevelExperience = experienceRequired[status.level[skillIndex]]; //다음레벨 가기 위한 양

        float currentAmount = (float)currentExperience / nextLevelExperience;
        float plusAmount = (float)newExperience / nextLevelExperience; //합쳐진 전체 경험치비율
        UIManager.instance.NoticeItemCreate(9, currentAmount, plusAmount, skillIndex); //notice 호출

        if(nextLevelExperience <= newExperience)
        {
            status.level[skillIndex]++;
            status.skillAmount[skillIndex] = newExperience - nextLevelExperience;

            RenderCharmLevel(); //능력치 레벨 가변저장소 업데이트
        }
        else
        {
            status.skillAmount[skillIndex] = newExperience;
        }
    }
    public void AddStat(Status statusLoad) //아이템으로 스탯 증감 시 호출될 함수
    {
        if (statusLoad == null)
        {
            return;
        }
        if (!GetMoodle()[12].isActive || Random.value > 0.5f) //만약 식중독 걸려있는거 아니면
        {
            AddHungry(statusLoad.currentHungry);
        }
        else
        {
            if(statusLoad.currentHungry < 0) //만약 식중독이라도 감소하는거면
            {
                AddHungry(statusLoad.currentHungry);
            }
        }
        AddFatigue(statusLoad.currentFatigue);
        AddSadness(statusLoad.currentSadness);
        AddBoredom(statusLoad.currentBoredom);
        AddAngry(statusLoad.currentAngry);
    }


    #region Perk

    // 초기화: 모든 퍽 상태를 잠김 상태로 설정
    private void InitializePerkStates()
    {
        for (int i = 0; i < status.perkStates.GetLength(0); i++)
        {
            for (int j = 0; j < status.perkStates.GetLength(1); j++)
            {
                status.perkStates[i, j] = 0; // 잠김 상태
                status.selectedPerks[i, j] = -1; // 선택되지 않음
            }
        }
    }

    // 특정 스킬과 레벨에 해당하는 PerkOption 반환
    public PerkOption GetPerkOption(int skillIndex, int perkLevelIndex)
    {
        int index = skillIndex * 2 + perkLevelIndex;

        if (index >= 0 && index < perkOptions.Count)
        {
            return perkOptions[index];
        }

        Debug.LogError("Invalid perk option request.");
        return null;
    }

    // 레벨 업 후 퍽 상태 업데이트
    public void UpdatePerkStates()
    {
        for (int i = 0; i < status.level.Length; i++)
        {
            if (status.level[i] >= 5 && status.perkStates[i, 0] == 0)
            {
                status.perkStates[i, 0] = 1; // 5레벨 퍽 선택 가능
            }

            if (status.level[i] >= 10 && status.perkStates[i, 1] == 0)
            {
                status.perkStates[i, 1] = 1; // 10레벨 퍽 선택 가능
            }
        }
    }

    // 퍽 선택
    public bool SelectPerk(int skillIndex, int perkLevelIndex, int perkId)
    {
        if (status.perkStates[skillIndex, perkLevelIndex] == 1)
        {
            status.selectedPerks[skillIndex, perkLevelIndex] = perkId; // 선택된 퍽 ID 저장
            status.perkStates[skillIndex, perkLevelIndex] = 2; // 선택 완료 상태

            RenderPerk(); //가변저장소 업뎃
            return true;
        }

        Debug.LogError("Perk selection failed: Invalid state.");
        return false;
    }

    // 선택된 퍽 상태 반환, 어떤 퍽 선택했는지 
    public int GetSelectedPerk(int skillIndex, int perkLevelIndex)
    {
        return status.selectedPerks[skillIndex, perkLevelIndex];
    }

    /// <summary>
    /// 주어진 스킬 인덱스와 퍽 레벨 인덱스에 해당하는 상태를 반환합니다. 퍽 선택 했는지 안했는지 여부
    /// </summary>
    public int GetPerkState(int skillIndex, int perkLevelIndex)
    {
        return status.perkStates[skillIndex, perkLevelIndex];
    }

    /// <summary>
    /// 선택된 퍽의 이미지를 반환합니다.
    /// </summary>
    public Sprite GetSelectedPerkImage(int skillIndex, int perkLevelIndex)
    {
        int selectedPerkId = status.selectedPerks[skillIndex, perkLevelIndex];
        if (selectedPerkId >= 0 && selectedPerkId < perkOptions.Count)
        {
            return perkOptions[skillIndex * 2 + perkLevelIndex].skillImage[selectedPerkId];
        }

        Debug.LogError("Selected perk image not found.");
        return null;
    }
    public LocalizedString GetSelectedPerkString(int skillIndex, int perkLevelIndex)
    {
        int selectedPerkId = status.selectedPerks[skillIndex, perkLevelIndex];
        if (selectedPerkId >= 0 && selectedPerkId < perkOptions.Count)
        {
            return perkOptions[skillIndex * 2 + perkLevelIndex].skillName[selectedPerkId];
        }

        Debug.LogError("Selected perk skillname not found.");
        return null;
    }
    public LocalizedString GetSelectedPerkStringDes(int skillIndex, int perkLevelIndex)
    {
        int selectedPerkId = status.selectedPerks[skillIndex, perkLevelIndex];
        if (selectedPerkId >= 0 && selectedPerkId < perkOptions.Count)
        {
            return perkOptions[skillIndex * 2 + perkLevelIndex].skillDescreption[selectedPerkId];
        }

        Debug.LogError("Selected perk skillDes not found.");
        return null;
    }

    #endregion
    public void SetTodayDice() //하루 시작 시, 아니면 설득 기회 한번 더 시 1~100 설득 난수 생성
    {
        status.diceLevel = Random.Range(1, 101);
        Debug.Log("오늘의 설득 난수는 " + status.diceLevel);
        RenderDice(); //가변저장소 업뎃
    }

    public void SetTodayLuck() //TimeManager에서 하루 시작 시 운 랜덤 부여
    {
        status.luckyLevel = Random.Range(-10, 11);
        Debug.Log("오늘의 운은 " + status.luckyLevel);
    }
    public int GetCrimianl() //범죄 수치 받아오기
    {
        return status.crimianalCount;
    }
    public void SetCrimianl(int count) //범죄 수치 관리
    {
        status.crimianalCount += count;
        status.crimianalCount = Mathf.Clamp(status.crimianalCount, 0, 20);
    }
    public int GetLuckLevel() //운 보는 함수 -10~10까지. ex) -10이면  10%감소
    {
        int luck = status.luckyLevel;

        switch(PlayerStats.GetDiffy())
        {
            case 0:
                luck += 1;
                break;
 
            case 2:
                luck -= 1;
                break;

            case 3:
                luck -= 3;
                break;

            default:
                break;
        }

        if (GetSelectedPerk(6, 1) == 0) //행운 퍽 찍었다면
        {
            luck += 3;
        }
        if (moodle[11].isActive) //만약 청중의 행운 무들 켜져있으면
        {
            luck += 1;
        }

        luck = Mathf.Max(-10, Mathf.Min(luck, 10));

        return luck;
    }

    public void UpdateStorageMoney() //돈 증감할때마다 가변저장소에 보내기
    {
        variableStorage.SetValue("$gold", PlayerStats.GetMoney());
    }

    public void SimulationCalcul(int index) //시뮬레이션 시 스탯 계산
    {
        AddFatigue(simulationStats[index].fatigue * -1);
        AddHungry(simulationStats[index].hungry * -1);
        AddAngry(simulationStats[index].angry);
        AddBoredom(simulationStats[index].boredom);
        AddSadness(simulationStats[index].sadness);
        //hp는 나중에 필요하면 추가할것, 스탯매니저에 아직 구현 안되어있어서 귀찮

        if (simulationStats[index].skillAmount != 0)
        {
            AddExperience(simulationStats[index].skillNumber, simulationStats[index].skillAmount);
        }
    }
    public void SimulationRiskCalcul(int index) //리스크 시뮬레이션 시 스탯 계산
    {
        AddFatigue(riskStat[index].fatigue * -1);
        AddHungry(riskStat[index].hungry * -1);
        AddAngry(riskStat[index].angry);
        AddBoredom(riskStat[index].boredom);
        AddSadness(riskStat[index].sadness);
        //hp는 나중에 필요하면 추가할것, 스탯매니저에 아직 구현 안되어있어서 귀찮

        if (riskStat[index].skillAmount != 0)
        {
            AddExperience(riskStat[index].skillNumber, riskStat[index].skillAmount);
        }
    }
    public void EatItemCalcul(int index) //인벤 말고 그냥 뭐 먹을 시 스탯 계산용
    {
        AddFatigue(eatStats[index].fatigue * -1);
        if (!GetMoodle()[12].isActive || Random.value > 0.5f) //만약 식중독 걸려있다면
        {
            AddHungry(eatStats[index].hungry * -1);
        }
        AddAngry(eatStats[index].angry);
        AddBoredom(eatStats[index].boredom);
        AddSadness(eatStats[index].sadness);
        //hp는 나중에 필요하면 추가할것, 스탯매니저에 아직 구현 안되어있어서 귀찮

        CollectionOpen(index, false);
    }
    public void CollectionOpen(int index, bool isInvenItem) //콜렉션 개방
    {

        if (isInvenItem) 
        {
            Debug.Log(index + "fdasdfsa");
            GetStatus().itemOpen[index] = true;
        }
        else
        {
            GetStatus().notInvenItemOpen[index] = true;
        }      
    }
    public void ActJobOpen(int index, bool isAct) //콜렉션 개방
    {
        if (isAct)
        {
            GetStatus().actOpen[index] = true;
        }
        else
        {
            GetStatus().jobOpen[index] = true;
        }
    }

    public void RenderMoodle() //무들 상태 가변 저장소에 업데이트
    {
        variableStorage.SetValue("$bodyache", GetMoodle()[4].isActive);
    }

    public void RenderPerk() //설득 한번 더 퍽 가변 저장소에 업데이트
    {
        bool isSelect = false;
        if(GetSelectedPerk(4, 1) == 0)
        {
            isSelect = true;
        }
        variableStorage.SetValue("$oneMorePerk", isSelect);
    }
    public void RenderDice() //가변저장소 난수 업뎃
    {
        variableStorage.SetValue("$todayNum", status.diceLevel);
    }
    public void RenderCharmLevel() //가변저장소에 매력 레벨 업뎃, 미니게임 끝나고도 업뎃 됨
    {
        variableStorage.SetValue("$strengthLevel", status.level[0]);
        variableStorage.SetValue("$handyLevel", status.level[2]);
        variableStorage.SetValue("$charmLevel", status.level[4]);
        variableStorage.SetValue("$intelLevel", status.level[6]);
    }
    public void RenderDept() //가변저장소에 이번 주 빚 업뎃
    {
        variableStorage.SetValue("$thisWeekDept", PlayerStats.GetWeekDept());
    }
    public void RenderStat() //가변저장소에 피로도,배고픔,화남 슬픔 지루함 수치 업뎃
    {
        variableStorage.SetValue("$fatigue", status.currentFatigue);
        variableStorage.SetValue("$hungry", status.currentHungry);
        variableStorage.SetValue("$anger", status.currentAngry);
        variableStorage.SetValue("$sadness", status.currentSadness);
        variableStorage.SetValue("$boredom", status.currentBoredom);
    }
}
