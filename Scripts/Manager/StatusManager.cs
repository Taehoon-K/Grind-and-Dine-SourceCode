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
    private int[] totalExperience = { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 }; //����ġ ��

    Status status;
    private Moodle[] moodle = new Moodle[14];

    [SerializeField]
    private int total = 1000;

    // ���¹̳� ������
    [SerializeField]
    private int spIncreaseSpeed;

    // ���¹̳� ��ȸ�� ������ �ð�
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // ���¹̳� ���� ����
    public bool isRun = false;

    //������ ����
    List<IStatusTracker> listeners = new List<IStatusTracker>();

    [Header("Character")]
    [SerializeField] private GameObject woman,man;

    [Header("Perk")]
    [SerializeField] private List<PerkOption> perkOptions; // PerkOption ������ ����Ʈ

    [Header("SimulationStat")]
    [SerializeField] private List<SimulationStat> simulationStats; //�ùķ��̼� �� ���� ���� ��Ƶ� ����ü
    [SerializeField] private List<SimulationStat> riskStat; //�ùķ��̼� �� ���� ���� ��Ƶ� ����ü
    [SerializeField] private List<SimulationStat> eatStats; //�׳� ���� �� ���� ���� ��Ƶ� ����ü

    public static StatusManager instance = null;

    private int moodUpdateCounter = 0; // ȣ�� Ƚ���� �����ϴ� ����

    private bool hasloaded = false;  // ������ �ε�� �Ŀ� �ð��� ���� �Լ� ȣ��ǵ��� �ϱ�

    private InMemoryVariableStorage variableStorage;
    private int currentHour; //���� �������. yarn�� ����
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
        status = new Status(); // ��ü �ʱ�ȭ�� ���⼭ ����

        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        // InitializePerkStates();
    }
    public enum SleepState
    {
        Awake,        // �⺻ ����
        Sleep,
        Simulate
    }
    public SleepState currentSleepState = SleepState.Awake; // ���� UI ����
    void Start()
    {
        TimeManager.instance.RegisterTracker(this);

        /*status.currentHp = total;
        status.currentFatigue = total;
        status.currentHungry = total;
        status.currentSp = total;*/

       /* status.level[2] = 5;
        status.skillAmount[2] = 100;
        status.skillAmount[3] = 30; //�׽�Ʈ*/

        if(TutorialManager.instance != null) //���� Ʃ�丮���̸�
        {
            status.currentHp = total;
            status.currentFatigue = total;
            status.currentHungry = 560;
            status.currentSp = total; //ä��� ����
        }

        //variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        //Debug.Log("Find Viraviole storage");
    }

    public void LoadGameStart(int index) //�ε��ؼ� �� �����Ҷ� ȣ��
    {
        var player = FindObjectOfType<Kupa.Player>();
        GameObject prefab;
        if (PlayerStats.IsWoman)
        {
            //  cc.SwitchCharacterSettings(1); //0�̸� ���� 1�̸� ����
            prefab = Instantiate(woman, player.transform);

        }
        else
        {
            //  cc.SwitchCharacterSettings(0); //0�̸� ���� 1�̸� ����
            prefab = Instantiate(man, player.transform);
        }
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        //prefab.gameObject.transform = player.modelTransform;
        player.GetTransform();

        Debug.Log("�÷��̾� ����ŷ �������������");
        //������ ���̰� �ٷ� player���� �ݶ��̴� ��Ȱ��ȭ ����
        player.GetComponent<Kupa.Player>().SetupRagdoll();

        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();

        var saved = cc.GetSavedCharacterDatas();

        // �ش� �̸��� ���� �����Ͱ� �ִ��� Ȯ��
        for (int i = 0; i < saved.Count; i++)
        {
            //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
            if (saved[i].name == (index+1).ToString())
            {
                cc.ApplySavedCharacterData(saved[i]); // ���ϴ� �̸��� ĳ���� ������ ����
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // ã���� �� �̻� �ݺ����� �ʰ� ����
            }
        }

        

        player.eye = FindChildByName(player.gameObject, "Head");

        ChangeLayerRecursively(player.gameObject, 6); //���̾� �ٲٱ�
        Transform thirdChild = player.gameObject.transform.GetChild(2);
        // �� ��° �ڽ��� Animator ������Ʈ ��������
        Animator animator = thirdChild.GetComponent<Animator>();
        // Animator ������Ʈ�� ������ ����
        if (animator != null)
        {
            Destroy(animator);
        }

        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Animator>().enabled = true;
        // Animator ���¸� �����Ͽ� �ٽ� ����� �� �ֵ��� ��
        player.GetComponent<Animator>().Rebind();
        player.GetComponent<Animator>().Update(0); // ������Ʈ�� ������ ȣ���� ��� �ݿ�

        hasloaded = true;

        RenderDice(); //��������� ����
        //RenderDept();

        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 5); //5�� ���Կ� ����
    }

    public void FirstGameStart()  //���� ó�� �����Ҷ� ȣ��� �Լ�
    {
        status.currentHp = total;
        status.currentFatigue = total;
        status.currentHungry = total;
        status.currentSp = total;

        for (int i = 0; i < moodle.Length; i++)
        {
            moodle[i] = new Moodle { isActive = false, timeLeft = 0 }; // �⺻������ �ʱ�ȭ
        }
        InitializePerkStates(); //�� �ʱ�ȭ

        var player = FindObjectOfType<Kupa.Player>();
        GameObject prefab;
        
        if (PlayerStats.IsWoman)
        {
            // cc.SwitchCharacterSettings(1); //0�̸� ���� 1�̸� ����
            prefab = Instantiate(woman, player.transform.localPosition, player.transform.localRotation, player.transform);

        }
        else
        {
            // cc.SwitchCharacterSettings(0); //0�̸� ���� 1�̸� ����
            prefab = Instantiate(man, player.transform.localPosition, player.transform.localRotation, player.transform);
        }
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        player.GetTransform();

        Debug.Log("�÷��̾� ����ŷ �������������");
        //������ ���̰� �ٷ� player���� �ݶ��̴� ��Ȱ��ȭ ����
        player.GetComponent<Kupa.Player>().SetupRagdoll();

        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponentInChildren<AdvancedPeopleSystem.CharacterCustomization>();
        var saved = cc.GetSavedCharacterDatas();

        // �ش� �̸��� ���� �����Ͱ� �ִ��� Ȯ��
        for (int i = 0; i < saved.Count; i++)
        {
            if (saved[i].name == "0")
            {
                cc.ApplySavedCharacterData(saved[i]); // ���ϴ� �̸��� ĳ���� ������ ����
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // ã���� �� �̻� �ݺ����� �ʰ� ����
            }
        }


        //  FLookAnimator lookAni = player.gameObject.GetComponent<FLookAnimator>();
        //  lookAni.LeadBone = FindChildByName(player.gameObject, "Head");
        //  player.eye = lookAni.LeadBone;

        player.eye = FindChildByName(player.gameObject, "Head");

        ChangeLayerRecursively(player.gameObject, 6); //���̾� �ٲٱ�
        Transform thirdChild = player.gameObject.transform.GetChild(2);
        // �� ��° �ڽ��� Animator ������Ʈ ��������
        Animator animator = thirdChild.GetComponent<Animator>();
        // Animator ������Ʈ�� ������ ����
        if (animator != null)
        {
            Destroy(animator);
        }
        
        //lookAni.enabled = false;
        //lookAni.enabled = true;
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Animator>().enabled = true;
        // Animator ���¸� �����Ͽ� �ٽ� ����� �� �ֵ��� ��
        player.GetComponent<Animator>().Rebind();
        player.GetComponent<Animator>().Update(0); // ������Ʈ�� ������ ȣ���� ��� �ݿ�

        hasloaded = true;

        //RenderDept();

        cc.SaveCharacterToFileReal(AdvancedPeopleSystem.CharacterCustomizationSetup.CharacterFileSaveFormat.Json, "", 5); //5�� ���Կ� ����
    }

    Transform FindChildByName(GameObject parent, string targetName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true)) // true: ��Ȱ��ȭ�� �ڽ� ����
        {
            if (child.name == targetName)
            {
                return child; // �̸��� ��ġ�ϴ� �ڽ�(Ȥ�� ����) ������Ʈ ��ȯ
            }
        }
        return null; // ã�� ���ϸ� null ��ȯ
    }
    void ChangeLayerRecursively(GameObject obj, int newLayer)
    {
        // �ش� ������Ʈ�� ���̾� ����
        //obj.layer = newLayer;

        // ��� �ڽĵ��� ���̾� ����
        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true)) // true: ��Ȱ��ȭ�� �ڽĵ鵵 ����
        {
            child.gameObject.layer = newLayer;
        }
    }

    public void ClockUpdate(GameTimestamp timestamp) //�� �ʸ��� ȣ��� �Լ�
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
        else if(currentSleepState == SleepState.Sleep) //�ڴ� ���¸�
        {
            if (moodle[1].isActive) //���� �Ҹ��� Ȱ��ȭ��������
            {
                AddFatigue(0.85f, true); //�ݸ� ȸ��
            }
            else
            {
                AddFatigue(1.7f, true);
            }

            if (GetSelectedPerk(6, 1) == 1) //���� 10-2�� Ȱ��ȭ��������
            {
                AddAngry(-1);
                AddSadness(-1);
                AddBoredom(-1);
            }
            else
            {
                if(PlayerStats.Difficulty == 0) //���� ���� ���̵��϶��� �ڸ� ���� ȸ��
                {
                    moodUpdateCounter++;
                    if (moodUpdateCounter >= 20) // �� �� ȣ��Ǿ��� ��
                    {
                        AddAngry(-1);
                        AddSadness(-1);
                        AddBoredom(-1);
                        moodUpdateCounter = 0; // ī���� �ʱ�ȭ
                    }
                }
                
            }
        }
        MoodleDecrease(); //���� ���� �ð� ���̱�

        UIManager.instance.canRunOnOff(moodle[5].isActive); //������ ���� ���� �̹��� �����״��ϱ�

        if (timestamp.hour != currentHour)  //���� �ð� �ٲ��
        {
            currentHour = timestamp.hour;
            variableStorage.SetValue("$hour", currentHour);  //yarn�� �ð� ������Ʈ
        }

        RenderStat();//��������ҿ� ���� ��ġ ����
    }
    private void FixedUpdate() //���� �����Ӹ��� ȣ��Ǵ� �Լ�
    {
        Stamina();
  

        foreach (IStatusTracker listener in listeners) //ui���� ��ġ�� ����
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

        RenderPerk(); //��������� ����
        RenderCharmLevel(); //��������� ����
        RenderStat();//��������� ����
    }

    public void DestroyMoodleOne() //���� �ϳ� �������� �����, �� �Լ�
    {
        // 1. isActive�� true�� �ε����� ����Ʈ�� ����
        List<int> activeIndexes = new List<int>();
        for (int i = 0; i < moodle.Length; i++)
        {
            if (moodle[i].isActive)
            {
                activeIndexes.Add(i);
            }
        }

        // 2. �ƹ��͵� Ȱ��ȭ�� �� ���ٸ� ����
        if (activeIndexes.Count == 0)
            return;

        // 3. �������� �ϳ� �����ؼ� ��Ȱ��ȭ
        int randomIndex = activeIndexes[Random.Range(0, activeIndexes.Count)];

        MoodleChange(randomIndex,false); //���� Ȱ��ȭ ���� �ϳ� �����
    }
    public void MoodleChange(int index, bool bol, int time = 0, bool isSet = false) //�����̻� �ٲٴ� �Լ�, isSet = �ð��� ���ϴ� ���� �ƴ϶� ����
    {
        //moodle[index].isActive = bol;
        if (bol)
        {
            if(index == 10 && GetSelectedPerk(0, 1) == 1) //���� ���� �鿪 �� �����ִٸ�
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
            moodle[index].timeLeft = Mathf.Min(moodle[index].timeLeft, 72*60); //72�ð� ���� �ʰ� ����

            if (moodle[index].timeLeft <= 0)
            {
                if (moodle[index].isActive) //���� �����־��ٸ�
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
            if (moodle[index].isActive) //���� �����־��ٸ�
            {
                moodle[index].isActive = false;
                UIManager.instance.NoticeItemCreate(7, index);
            }
            moodle[index].timeLeft = 0; //���� �����ð� 0���� �ٲ�
        }
        UIManager.instance.RenderPlayerStats();
        RenderMoodle(); //���̾�α� ��������� ����
    }
    public void AddMoodle(MoodleImform[] moodleImforms) //���� ������ ���� ���� �� ȣ���� �Լ�
    {
        if (moodleImforms == null || moodleImforms.Length == 0)
            return; // �迭�� ��������� ó������ ����

        foreach (MoodleImform moodleImform in moodleImforms)
        {
            if (moodleImform != null && moodleImform.timeLeft != 0) // //���� ���� Ÿ���� 0�� �ƴ� ��쿡��, 0�϶��� isacitve�� �ƴϵ� �� ����
            {
                if (Random.value < moodleImform.probability) // Ȯ�� üũ
                {
                    MoodleChange(moodleImform.index, moodleImform.isActive, moodleImform.timeLeft * 60);
                }
            }
        }
    }
    public Moodle[] GetMoodle()
    {
        return moodle; //�����̻� �迭 ����
    }
    public void LoadMoodle(Moodle[] mood) //���̺� �ε�� �����̻� �ε�
    {
        //moodle = mood;

        // ���� mood �迭�� ������ �� �迭�� ���� (�ִ� 10�������� ����)
        for (int i = 0; i < mood.Length; i++)
        {
            moodle[i] = mood[i];
        }
        UIManager.instance.RenderPlayerStats();
        RenderMoodle(); //���ȸŴ��� ���� ����
    }
    
    public int GetHp() //�Ƿε� ��� �ڵ�
    {
        return (int)status.currentFatigue;
    }

    public void AddHungry(float point, bool isNatural = false)
    {
        if (moodle[2].isActive && point < 0) //���� �������̸�, �׸��� ���� ��
        {
            status.currentHungry += point * 1.3f; //1.5�� ���� ���

            if (!isNatural) //���� �ڿ������� ���� �ƴ϶��
            {
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
                {
                    UIManager.instance.NoticeItemCreate(2, (int)(point * 1.3f / 10));
                }
                
            }
        }
        else if (point < 0)
        {
            status.currentHungry += point;

            if (!isNatural) //���� �ڿ������� ���� �ƴ϶��
            {
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
                {
                    UIManager.instance.NoticeItemCreate(2, (int)point / 10);
                }
                
            }
        }
        else if(point > 0)
        {
            if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
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
        if(point < 0 && GetSelectedPerk(0,0) == 0) //���� �Ƿε� ������ �Ƿΰ��� �� �����ִٸ�
        {
            //Debug.Log("PiroPErkkkkkkkkkkk");
            point *= 0.8f;
        }

        if (moodle[0].isActive && point < 0) //���� �����Ƿθ�
        {
            status.currentFatigue += point * 1.3f; //1.5�� ���� ���

            if (!isNatural) //���� �ڿ������� ���� �ƴ϶��
            {
                
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
                {
                    UIManager.instance.NoticeItemCreate(1, (int)(point * 1.3f / 10));
                }
            }
        }
        else if (point < 0)
        {
            status.currentFatigue += point;
            if (!isNatural) //���� �ڿ������� ���� �ƴ϶��
            {
                
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
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
                
                if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
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
        if(point != 0 && point != -1) //���� �߶� 1�� �����ϴ°͵� �ƴϸ�
        {
            if(UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
            {
                UIManager.instance.NoticeItemCreate(3, point / 10);
            }
            
        }

        if (point > 0 && GetSelectedPerk(6, 0) == 1 && !isBonus) //���� ���� 5-2�� �����ִٸ�
        {
            point = (int)(point * 0.8f);
        }
        if (moodle[6].isActive && point > 0 && !isBonus) //���� ������� ������ ���� Ư�� �����ִٸ�
        {
            point = (int)(point * 1.5f);
        }
        if (moodle[7].isActive && point > 0 && !isBonus) //���� ������� ������ ���ڿ��ߵ� Ư�� �����ִٸ�
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
        if (point != 0 && point != -1) //���� �߶� 1�� �����ϴ°͵� �ƴϸ�
        {
            if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
            {
                UIManager.instance.NoticeItemCreate(4, point / 10);
            }     
        }

        if (point > 0 && GetSelectedPerk(6, 0) == 1 && !isBonus) //���� ���� 5-2�� �����ִٸ�
        {
            point = (int)(point * 0.8f);
        }
        if (moodle[6].isActive && point > 0 && !isBonus) //���� ������� ������ ���� Ư�� �����ִٸ�
        {
            point = (int)(point * 1.5f);
        }
        if (moodle[7].isActive && point > 0 && !isBonus) //���� ������� ������ ���ڿ��ߵ� Ư�� �����ִٸ�
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
    public void AddBoredom(int point, bool isBonus = false) //isBonus Ʈ��� ���۽� ���鶫�� ���ʽ��� �ö󰡴°�
    {
        if (point != 0 && point != -1) //���� �߶� 1�� �����ϴ°͵� �ƴϸ�
        {
            
            if (UIManager.instance != null && UIManager.instance.isActiveAndEnabled) //ui�Ŵ��� Ȱ��ȭ�Ͻø�
            {
                UIManager.instance.NoticeItemCreate(5, point / 10);
            }
        }

        if (point > 0 && GetSelectedPerk(6, 0) == 1 && !isBonus) //���� ���� 5-2�� �����ִٸ�
        {
            point = (int)(point * 0.8f);
        }
        if (moodle[6].isActive && point > 0 && !isBonus) //���� ������� ������ ���� Ư�� �����ִٸ�
        {
            point = (int)(point * 1.5f);
        }
        if (moodle[7].isActive && point > 0 && !isBonus) //���� ������� ������ ���ڿ��ߵ� Ư�� �����ִٸ�
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
        //������ �ε巴�� �ø��� �뵵
    {
        float duration = 1.0f; // �������� �����ϴ� �� �ɸ��� �ð� (��)
        float startHungry = current;
        float targetHungry = Mathf.Clamp(startHungry + point, 0, total); // �ּ� 0, �ִ� total�� ����

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startHungry, targetHungry, elapsedTime / duration);
            updateValueCallback(newValue); // ���� ������Ʈ�ϴ� �ݹ� ȣ��
            yield return null;
        }

        // ���� �� ����
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
                if(currentSleepState == SleepState.Awake) //�������� ���� ����
                {
                    status.currentHp -= zeroCount * 3;
                }
                
            }
            else
            {
                if (currentSleepState != SleepState.Simulate) //�ùķ��̼Ƕ��� ���� ���ϰ�
                {
                    status.currentHp += 3;
                }
                
            }
            
        }
        else
        {
            Debug.Log("HP ��ġ�� 0 �� �Ǿ����ϴ�.");

            var player = FindObjectOfType<Kupa.Player>();
            player.GetComponent<Kupa.Player>().OnDead(); //�÷��̾� ���� ����

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
                // timeLeft ����
                moodle[i].timeLeft--;

                // timeLeft�� 0�� �Ǹ� isActive�� false�� ��ȯ
                if (moodle[i].timeLeft <= 0)
                {
                    UIManager.instance.NoticeItemCreate(7, i);
                    moodle[i].isActive = false;
                    moodle[i].timeLeft = 0; // timeLeft�� ������ �������� �ʰ� ��
                }
            }
        }
    }

    private void Stamina()
    {
        if (isRun == true)//�޸�����
        {
            currentSpRechargeTime = 0;

            if (GetSelectedPerk(0, 0) == 1)
            {
                //Debug.Log("Stamian Perkkkkkkkk");
                status.currentSp -= 4; //���ݸ� ����

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

    public float GetExperienceProgress(int index) //����ġ �� ����ϴ� �뵵
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
        if(status.level[skillIndex] == 10) //�����̸� ����
        {
            return;
        }
        int currentExperience = status.skillAmount[skillIndex];
        int newExperience = currentExperience + experience; //������ ��
        int nextLevelExperience = experienceRequired[status.level[skillIndex]]; //�������� ���� ���� ��

        float currentAmount = (float)currentExperience / nextLevelExperience;
        float plusAmount = (float)newExperience / nextLevelExperience; //������ ��ü ����ġ����
        UIManager.instance.NoticeItemCreate(9, currentAmount, plusAmount, skillIndex); //notice ȣ��

        if(nextLevelExperience <= newExperience)
        {
            status.level[skillIndex]++;
            status.skillAmount[skillIndex] = newExperience - nextLevelExperience;

            RenderCharmLevel(); //�ɷ�ġ ���� ��������� ������Ʈ
        }
        else
        {
            status.skillAmount[skillIndex] = newExperience;
        }
    }
    public void AddStat(Status statusLoad) //���������� ���� ���� �� ȣ��� �Լ�
    {
        if (statusLoad == null)
        {
            return;
        }
        if (!GetMoodle()[12].isActive || Random.value > 0.5f) //���� ���ߵ� �ɷ��ִ°� �ƴϸ�
        {
            AddHungry(statusLoad.currentHungry);
        }
        else
        {
            if(statusLoad.currentHungry < 0) //���� ���ߵ��̶� �����ϴ°Ÿ�
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

    // �ʱ�ȭ: ��� �� ���¸� ��� ���·� ����
    private void InitializePerkStates()
    {
        for (int i = 0; i < status.perkStates.GetLength(0); i++)
        {
            for (int j = 0; j < status.perkStates.GetLength(1); j++)
            {
                status.perkStates[i, j] = 0; // ��� ����
                status.selectedPerks[i, j] = -1; // ���õ��� ����
            }
        }
    }

    // Ư�� ��ų�� ������ �ش��ϴ� PerkOption ��ȯ
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

    // ���� �� �� �� ���� ������Ʈ
    public void UpdatePerkStates()
    {
        for (int i = 0; i < status.level.Length; i++)
        {
            if (status.level[i] >= 5 && status.perkStates[i, 0] == 0)
            {
                status.perkStates[i, 0] = 1; // 5���� �� ���� ����
            }

            if (status.level[i] >= 10 && status.perkStates[i, 1] == 0)
            {
                status.perkStates[i, 1] = 1; // 10���� �� ���� ����
            }
        }
    }

    // �� ����
    public bool SelectPerk(int skillIndex, int perkLevelIndex, int perkId)
    {
        if (status.perkStates[skillIndex, perkLevelIndex] == 1)
        {
            status.selectedPerks[skillIndex, perkLevelIndex] = perkId; // ���õ� �� ID ����
            status.perkStates[skillIndex, perkLevelIndex] = 2; // ���� �Ϸ� ����

            RenderPerk(); //��������� ����
            return true;
        }

        Debug.LogError("Perk selection failed: Invalid state.");
        return false;
    }

    // ���õ� �� ���� ��ȯ, � �� �����ߴ��� 
    public int GetSelectedPerk(int skillIndex, int perkLevelIndex)
    {
        return status.selectedPerks[skillIndex, perkLevelIndex];
    }

    /// <summary>
    /// �־��� ��ų �ε����� �� ���� �ε����� �ش��ϴ� ���¸� ��ȯ�մϴ�. �� ���� �ߴ��� ���ߴ��� ����
    /// </summary>
    public int GetPerkState(int skillIndex, int perkLevelIndex)
    {
        return status.perkStates[skillIndex, perkLevelIndex];
    }

    /// <summary>
    /// ���õ� ���� �̹����� ��ȯ�մϴ�.
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
    public void SetTodayDice() //�Ϸ� ���� ��, �ƴϸ� ���� ��ȸ �ѹ� �� �� 1~100 ���� ���� ����
    {
        status.diceLevel = Random.Range(1, 101);
        Debug.Log("������ ���� ������ " + status.diceLevel);
        RenderDice(); //��������� ����
    }

    public void SetTodayLuck() //TimeManager���� �Ϸ� ���� �� �� ���� �ο�
    {
        status.luckyLevel = Random.Range(-10, 11);
        Debug.Log("������ ���� " + status.luckyLevel);
    }
    public int GetCrimianl() //���� ��ġ �޾ƿ���
    {
        return status.crimianalCount;
    }
    public void SetCrimianl(int count) //���� ��ġ ����
    {
        status.crimianalCount += count;
        status.crimianalCount = Mathf.Clamp(status.crimianalCount, 0, 20);
    }
    public int GetLuckLevel() //�� ���� �Լ� -10~10����. ex) -10�̸�  10%����
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

        if (GetSelectedPerk(6, 1) == 0) //��� �� ����ٸ�
        {
            luck += 3;
        }
        if (moodle[11].isActive) //���� û���� ��� ���� ����������
        {
            luck += 1;
        }

        luck = Mathf.Max(-10, Mathf.Min(luck, 10));

        return luck;
    }

    public void UpdateStorageMoney() //�� �����Ҷ����� ��������ҿ� ������
    {
        variableStorage.SetValue("$gold", PlayerStats.GetMoney());
    }

    public void SimulationCalcul(int index) //�ùķ��̼� �� ���� ���
    {
        AddFatigue(simulationStats[index].fatigue * -1);
        AddHungry(simulationStats[index].hungry * -1);
        AddAngry(simulationStats[index].angry);
        AddBoredom(simulationStats[index].boredom);
        AddSadness(simulationStats[index].sadness);
        //hp�� ���߿� �ʿ��ϸ� �߰��Ұ�, ���ȸŴ����� ���� ���� �ȵǾ��־ ����

        if (simulationStats[index].skillAmount != 0)
        {
            AddExperience(simulationStats[index].skillNumber, simulationStats[index].skillAmount);
        }
    }
    public void SimulationRiskCalcul(int index) //����ũ �ùķ��̼� �� ���� ���
    {
        AddFatigue(riskStat[index].fatigue * -1);
        AddHungry(riskStat[index].hungry * -1);
        AddAngry(riskStat[index].angry);
        AddBoredom(riskStat[index].boredom);
        AddSadness(riskStat[index].sadness);
        //hp�� ���߿� �ʿ��ϸ� �߰��Ұ�, ���ȸŴ����� ���� ���� �ȵǾ��־ ����

        if (riskStat[index].skillAmount != 0)
        {
            AddExperience(riskStat[index].skillNumber, riskStat[index].skillAmount);
        }
    }
    public void EatItemCalcul(int index) //�κ� ���� �׳� �� ���� �� ���� ����
    {
        AddFatigue(eatStats[index].fatigue * -1);
        if (!GetMoodle()[12].isActive || Random.value > 0.5f) //���� ���ߵ� �ɷ��ִٸ�
        {
            AddHungry(eatStats[index].hungry * -1);
        }
        AddAngry(eatStats[index].angry);
        AddBoredom(eatStats[index].boredom);
        AddSadness(eatStats[index].sadness);
        //hp�� ���߿� �ʿ��ϸ� �߰��Ұ�, ���ȸŴ����� ���� ���� �ȵǾ��־ ����

        CollectionOpen(index, false);
    }
    public void CollectionOpen(int index, bool isInvenItem) //�ݷ��� ����
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
    public void ActJobOpen(int index, bool isAct) //�ݷ��� ����
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

    public void RenderMoodle() //���� ���� ���� ����ҿ� ������Ʈ
    {
        variableStorage.SetValue("$bodyache", GetMoodle()[4].isActive);
    }

    public void RenderPerk() //���� �ѹ� �� �� ���� ����ҿ� ������Ʈ
    {
        bool isSelect = false;
        if(GetSelectedPerk(4, 1) == 0)
        {
            isSelect = true;
        }
        variableStorage.SetValue("$oneMorePerk", isSelect);
    }
    public void RenderDice() //��������� ���� ����
    {
        variableStorage.SetValue("$todayNum", status.diceLevel);
    }
    public void RenderCharmLevel() //��������ҿ� �ŷ� ���� ����, �̴ϰ��� ������ ���� ��
    {
        variableStorage.SetValue("$strengthLevel", status.level[0]);
        variableStorage.SetValue("$handyLevel", status.level[2]);
        variableStorage.SetValue("$charmLevel", status.level[4]);
        variableStorage.SetValue("$intelLevel", status.level[6]);
    }
    public void RenderDept() //��������ҿ� �̹� �� �� ����
    {
        variableStorage.SetValue("$thisWeekDept", PlayerStats.GetWeekDept());
    }
    public void RenderStat() //��������ҿ� �Ƿε�,�����,ȭ�� ���� ������ ��ġ ����
    {
        variableStorage.SetValue("$fatigue", status.currentFatigue);
        variableStorage.SetValue("$hungry", status.currentHungry);
        variableStorage.SetValue("$anger", status.currentAngry);
        variableStorage.SetValue("$sadness", status.currentSadness);
        variableStorage.SetValue("$boredom", status.currentBoredom);
    }
}
