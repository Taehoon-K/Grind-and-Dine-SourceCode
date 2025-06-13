using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PP.InventorySystem;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour,ITimeTracker,IStatusTracker
{

    public static UIManager instance = null;
    private bool escButton;

    [SerializeField]
    private TextMeshProUGUI Timebox; //�ð� ui
    [SerializeField]
    private TextMeshProUGUI dateBox;
    [SerializeField] private GameObject minimap; //�̴ϸ�

    // �ʿ��� �̹���
    [SerializeField]
    private Image[] images_Gauge;
    [SerializeField]
    private Sprite[] images_Mood; //���� �̹��� �ٲٴ� �뵵
    [SerializeField]
    private Image imageCurrentMood; //���� �̹��� �ٲٴ� �뵵

    

    [Header("Plater Stats")]
    [SerializeField]
    private TextMeshProUGUI moneyText;

    [SerializeField]
    protected GameObject moneyPlus; //�� ȿ��
    [SerializeField]
    protected GameObject moneyMinus; //�� ȿ��
    
    [Header("Shop")]
    public ShopListingManager shopListingManager;

    [Header("Yes No Prompt")]
    public YesNoPrompt yesNoPrompt;

    [Header("Sleep Prompt")]
    public SleepPrompt sleepPrompt;
    public SleepPromptSix sleepPromptSix;

    [Header("Elevator Prompt")]
    public ElevatorPrompt elevatorPrompt;

    [Header("SkillSelect Prompt")]
    public SkillSelectPrompt skillSelectPrompt;

    [Header("SkillSelect Prompt")]
    public ToHousePrompt toHousePrompt;

    [Header("Simulate Prompt")]
    public ResultSimulPanel simulPanel;

    [Header("Dept Prompt")]
    [SerializeField]
    private DeptPrompt deptPanel;

    [Header("Lottery Prompt")]
    [SerializeField]
    private ScratchCard lotteryPanel;

    [Header("kickboard Prompt")]
    [SerializeField]
    private KickboardPrompt kickboardPanel;

    [Header("WorkNotice Prompt")]
    [SerializeField]
    private WorkPanel workPanel;

    [SerializeField]
    private GameObject notice;
    private GameObject noticeEver;
    [SerializeField] private GameObject noticePersuade; //���� ������ ���� GIF

    [Header("Game Prompt")]
    public GameObject[] gamePanel;

    [Header("Input Action")]
    [SerializeField]
    private PlayerInput m_Action;

    [SerializeField]
    private GameObject escPanel;

    [Header("Looting")]
    [SerializeField]
    private Image lootGauge;
    [SerializeField] private CardPanel cardPanel; //����ũ�ൿ�� ī�� ���� ī���г�

    [Header("Item & Stat Notice")]
    [SerializeField] private GameObject noticeStat; // �˸� Prefab
    [SerializeField] private GameObject noticeMoodle; // ���� �˸� ������
    [SerializeField] private GameObject noticePerk; // ���� �˸� ������
    [SerializeField] private GameObject noticeSkill; // ��ų ����ġ ������ �˸� ������
    [SerializeField] private GameObject noticeQuest; // ����Ʈ ��Ƽ��

    private struct NoticeData
    {
        public int messageType;
        public int number; // �⺻�����δ� ���⿡ ���� ���ڵ� (ex: ������)�� ����
        public float oldValue;
        public float newValue;
        public LocalizedString additionalText;
    }
    private Queue<NoticeData> noticeQueue = new Queue<NoticeData>(); // �˸� �����͸� �����ϴ� ť
    private bool isDisplaying = false; // ���� �˸� ǥ�� ����

    private bool isSleepSix;
    private int sleepTimes;

    [Header("Screen Fade & GameOver")]
    [SerializeField] private ScreenFadeOff screenFade;
    [SerializeField] private ScreenFadeOff gameOverN, gameOverDept,demoOver,moveScooter;

    [SerializeField] private CanvasGroup canvasPrompt,canvasNotice; //�κ� ��� �� ����� ui��

    [SerializeField] private GameObject cantRun; //���޸��� ���� ����

    [SerializeField] private GameObject simulGauge;

    [SerializeField] private GameObject eatWaiting; //�� ���� �� timedUI�� �ٲٷ��� ���� ��¥ ui

    [Header("�� UI ���¿� ���Ե� UI ������Ʈ �׷�")]
    [SerializeField] private GameObject[] generalUIPrompts;  // GeneralUI ���¿� �ش��ϴ� ������Ʈ��
    //[SerializeField] private GameObject[] inventoryUIPrompts; // InventoryUI ���¿� �ش��ϴ� ������Ʈ��
    [SerializeField] private GameObject[] timedUIPrompts;     // TimedUI ���¿� �ش��ϴ� ������Ʈ��
    GameObject copy;

    // ��Ȳ�� �´� ���ö����� Ű�� �����ϴ� ��ųʸ�
    private Dictionary<int, string> messageKeyMap = new Dictionary<int, string>()
    {
        { 0, "AddItem_key" },
        { 1, "AddVigor_key" }, //�Ƿε� ����
        { 2, "AddSatiety_key" }, //����� ����
        { 3, "AddAnger_key" }, //ȭ��
        { 4, "AddSadness_key" }, //����
        { 5, "AddBoredom_key" }, //������
        { 6, null }, //���� ����
        { 7, null }, //���� ����
        { 8, null }, //�� �ߵ�
    };

    private void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
            DontDestroyOnLoad(gameObject); //OnLoad(���� �ε� �Ǿ�����) �ڽ��� �ı����� �ʰ� ����
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }

        //_uiStateStack.Push(UIState.None);
    }

    private void Start()
    {
        //Ʈ���̵� �κ� ���� ���������� �ѹ� �״� ���� �뵵
        shopListingManager.gameObject.SetActive(false);

        //�ð� ���� �ɶ����� ���
        TimeManager.instance.RegisterTracker(this);
        StatusManager.instance.RegisterTracker(this);
        RenderPlayerStats();
        //PlayerStats.Earn(500000); //�׽�Ʈ��

        

        Cursor.visible = false; //Ŀ�� ����
    }
    private void OnEnable()
    {
        // �����ִ� �˸��� ����
        DestroyCurrentNotice();
    }
    private void Update()
    {
        if (escButton)
        {
            escButton = false;
            escPanel.SetActive(true);
        }

        escButton = false;
    }

    #region UIState
    public enum UIState
    {
        None,        // �⺻ ����
        GeneralUI,   // �Ϲ� UI (�ð� ����)
        InventoryUI, // �κ��丮 UI (�ð� ����)
        TimedUI      // �ð� ���� UI (�̴ϰ��� ���â ��)
    }

    private UIState _currentUIState = UIState.None; // ���� UI ����
   //private UIState previousUIState = UIState.None; // ���� UI ����

    public UIState CurrentUIState
    {
        get => _currentUIState;
        set
        {
           //Debug.Log(value + " vv;v;v;v;vvvvv");
            _currentUIState = value;

            switch (_currentUIState)
            {
                case UIState.None:
                    Time.timeScale = 1; // �ð� ���� ����
                    Cursor.visible = false;
                    if(m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("Player");
                        InputSystem.ResetHaptics(); // ��� �Է� ���¸� �ʱ�ȭ
                    }
                    canvasNotice.alpha = 1;
                    break;

                case UIState.GeneralUI:
                    Time.timeScale = 0; // �ð� ����
                    if (m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("UIOn");
                        InputSystem.ResetHaptics(); // ��� �Է� ���¸� �ʱ�ȭ
                    }                  
                    Cursor.visible = true;
                    canvasNotice.alpha = 0;
                    break;

                case UIState.InventoryUI:
                    Time.timeScale = 0; // �ð� ����
                    Cursor.visible = true;
                    if (m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("Player");
                        InputSystem.ResetHaptics(); // ��� �Է� ���¸� �ʱ�ȭ
                    }
                    canvasNotice.alpha = 0;
                    break;

                case UIState.TimedUI:
                    Time.timeScale = 1; // �ð� ����
                    if (m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("UIOn");
                        InputSystem.ResetHaptics(); // ��� �Է� ���¸� �ʱ�ȭ
                    }
                    Cursor.visible = true;
                    canvasNotice.alpha = 0;
                    break;

                default:
                    Debug.LogWarning("Unknown UI state!");
                    break;
            }
        }
    }
    /// <summary>
    /// Ư�� UI ������Ʈ�� Ȱ��ȭ�� �� UI ���¸� ����
    /// </summary>
    public void OnUIPromptOpened(GameObject openedPrompt)
    {
        /*
        //Debug.Log("Ui����");
        if (Contains(generalUIPrompts, openedPrompt))
        {
            if(CurrentUIState == UIState.None || CurrentUIState == UIState.InventoryUI)
            {
                previousUIState = CurrentUIState;
            }
            
            CurrentUIState = UIState.GeneralUI;
        }
        else if (Contains(timedUIPrompts, openedPrompt))
        {
            if(CurrentUIState == UIState.None || CurrentUIState == UIState.InventoryUI)
            {
                previousUIState = CurrentUIState;
            }
            CurrentUIState = UIState.TimedUI;
        }*/
        if (Contains(generalUIPrompts, openedPrompt))
        {
            CurrentUIState = UIState.GeneralUI;
        }
        else if (Contains(timedUIPrompts, openedPrompt))
        {
            CurrentUIState = UIState.TimedUI;
        }
    }

    public void OnUIPromptClosed(GameObject closedPrompt)
    {
        StartCoroutine(DelayedCheckUIState());
    }

    private IEnumerator DelayedCheckUIState()
    {
        /* yield return null; // �� ������ ���

         if (AllClosed(generalUIPrompts) && AllClosed(timedUIPrompts))
         {
             CurrentUIState = previousUIState;
         }
         else if (!AllClosed(timedUIPrompts))
         {
             CurrentUIState = UIState.TimedUI;
         }
         else if (!AllClosed(generalUIPrompts))
         {
             CurrentUIState = UIState.GeneralUI;
         }*/
        yield return null; // �� ������ ���

        CanvasOnOff inven = FindObjectOfType<CanvasOnOff>();
        if (inven != null && inven.IsInvenOn()) //���� �κ� �����ִٸ�
        {
            CurrentUIState = UIState.InventoryUI;
        }
        // ��� ������Ʈ�� ���� ������ None
        else if (AllClosed(generalUIPrompts) && AllClosed(timedUIPrompts))
        {
            CurrentUIState = UIState.None;
        }
        // �Ϲ� UI�� ���� ������ GeneralUI
        else if (!AllClosed(generalUIPrompts) && AllClosed(timedUIPrompts))
        {
            CurrentUIState = UIState.GeneralUI;
        }
        // Ÿ�ӵ� UI�� ���� ������ TimedUI
        else if (AllClosed(generalUIPrompts) && !AllClosed(timedUIPrompts))
        {
            CurrentUIState = UIState.TimedUI;
        }
        // �� �� ���� ������ �켱���� ����ؼ� TimedUI ���� (Ȥ�� GeneralUI��)
        else
        {
            CurrentUIState = UIState.TimedUI;
        }
    }

    /// <summary>
    /// Ư�� �迭�� ���� ������Ʈ�� ���ԵǾ� �ִ��� Ȯ��
    /// </summary>
    private bool Contains(GameObject[] array, GameObject obj)
    {
        foreach (var item in array)
        {
            if (item == obj)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Ư�� UI �׷��� ���� ��Ȱ��ȭ�Ǿ����� Ȯ��
    /// </summary>
    private bool AllClosed(GameObject[] array)
    {
        foreach (var item in array)
        {
            if (item.activeSelf)
                return false;
        }
        return true;
    }


    #endregion

    public void ClockUpdate(GameTimestamp timestamp)
    {
        int hours = timestamp.hour;
        int minutes = timestamp.minute;

        // �⺻���� AM
        string prefix = "AM";

        // 12�ð����� ��ȯ
        if (hours >= 12)
        {
            prefix = "PM"; // 12�ú��� ����
        }

        // 12�ð� �������� ����
        if (hours > 12)
        {
            hours -= 12; // 13~23�ô� 12�� ��
        }
        else if (hours == 0)
        {
            hours = 12; // 0�ô� 12��(����)�� ǥ��
        }

        Timebox.text = prefix + hours + ":" + minutes.ToString("00");

        int day = timestamp.day;
        string season = timestamp.season.ToString();
        string dayOfTheWeek = timestamp.GetDayOfTheWeek().ToString();
        dateBox.text = season + " " + day + " ( " + dayOfTheWeek + " ) ";
       
    }

    public void StatusUpdate(Status status)
    {
        images_Gauge[0].fillAmount = (float)status.currentFatigue / 1000;
        images_Gauge[1].fillAmount = (float)status.currentSp / 1000;
        images_Gauge[2].fillAmount = (float)status.currentHungry / 1000;
        images_Gauge[3].fillAmount = (float)status.currentHp / 1000;  //hp ��ġ
        if (images_Gauge[0].fillAmount <= 0.2)
        {
            images_Gauge[0].color = Color.red;
        }
        else
        {
            images_Gauge[0].color = Color.white;
        }

        if (images_Gauge[2].fillAmount <= 0.2)
        {
            images_Gauge[2].color = Color.red;
        }
        else
        {
            images_Gauge[2].color = Color.white;
        }

        if (images_Gauge[3].fillAmount <= 0.2)
        {
            images_Gauge[3].color = Color.red;
        }
        else
        {
            images_Gauge[3].color = Color.white;
        }

        if (status.currentAngry <= 500 && status.currentSadness <= 500 && status.currentBoredom <= 500)
        {
            imageCurrentMood.sprite = images_Mood[0];
        }
        else
        {
            // 500 �̻��� �� �� ���� ū ���� ã�� �׿� �ش��ϴ� �ε��� Ȱ��ȭ
            if (status.currentAngry >= status.currentSadness && status.currentAngry >= status.currentBoredom)
            {
                if(status.currentAngry >= 750)
                {
                    imageCurrentMood.sprite = images_Mood[2];
                }
                else
                {
                    imageCurrentMood.sprite = images_Mood[1];
                }
            }
            else if (status.currentSadness >= status.currentAngry && status.currentSadness >= status.currentBoredom)
            {
                if (status.currentSadness >= 750)
                {
                    imageCurrentMood.sprite = images_Mood[4];
                }
                else
                {
                    imageCurrentMood.sprite = images_Mood[3];
                }
            }
            else if (status.currentBoredom >= status.currentAngry && status.currentBoredom >= status.currentSadness)
            {
                if (status.currentBoredom >= 750)
                {
                    imageCurrentMood.sprite = images_Mood[6];
                }
                else
                {
                    imageCurrentMood.sprite = images_Mood[5];
                }
            }
        }

    }
    public void RenderPlayerStats() //�� �� ���� ����
    {
        moneyText.text = PlayerStats.Money.ToString("N0") + PlayerStats.CURRENCY;
        shopListingManager.RenderMoney(); //���� �� ����
    }

    public void OpenShop(List<ItemData> shopItems, string filter) //���� ���� �Լ�
    {
        //CurrentUIState = UIState.GeneralUI;
        shopListingManager.gameObject.SetActive(true);
        shopListingManager.RenderShop(shopItems,filter);
    }
    
    //yesno ������Ʈ ��� �Լ�
    public void TriggerYesNoPrompt(LocalizedString message, System.Action onYesCallback)
    {
        yesNoPrompt.gameObject.SetActive(true);
        yesNoPrompt.CreatePrompt(message, onYesCallback);
    }

    //���� ������Ʈ ȣ�� �Լ�
    public void TriggerSleepPrompt(bool isJail)
    {
        //CurrentUIState = UIState.GeneralUI;
        if (TimeManager.instance.isSiesta()) //���� �����ڴ� �ð��̸�
        {
            sleepPrompt.OpenScreen(isJail);
        }
        else
        {
            sleepPromptSix.OpenScreen(isJail);
        }
    }

    //���� ������Ʈ ȣ�� �Լ�
    public void TriggerElevatorPrompt(int floor)
    {
       // CurrentUIState = UIState.GeneralUI;
        elevatorPrompt.OpenScreen(floor);
    }

    //��ų ������Ʈ ȣ�� �Լ�
    public void TriggerSkillPrompt(int skillN,int levelN)
    {
        //CurrentUIState = UIState.GeneralUI;
        skillSelectPrompt.OpenScreen(skillN,levelN);
    }
    //��ų ������Ʈ ������
    public void ExitSkillPrompt()
    {
        //CurrentUIState = UIState.InventoryUI;
    }

    //���� ������Ʈ ȣ�� �Լ�
    public void TriggerHousePrompt(List<string> npcs)
    {
        //CurrentUIState = UIState.GeneralUI;
        toHousePrompt.RenderShop(npcs);
        toHousePrompt.gameObject.SetActive(true);
    }

    public void ExitHousePrompt()
    {
        //CurrentUIState = UIState.None;
        toHousePrompt.gameObject.SetActive(false);
    }

    public void TriggerSimulPrompt(int gameIndex)
    {
        //CurrentUIState = UIState.GeneralUI;
        simulPanel.gameObject.SetActive(true);
        simulPanel.Render2(gameIndex);
        
    }
    public void TriggerDeptPrompt()
    {
        deptPanel.gameObject.SetActive(true);
    }
    public void TriggerWorkPrompt()
    {
        workPanel.gameObject.SetActive(true);
    }
    public void TriggerLotteryPanel()
    {
        //CurrentUIState = UIState.GeneralUI;
        lotteryPanel.gameObject.SetActive(true);
    }
    public void ExitLotteryPanel()
    {
        //CurrentUIState = UIState.None;
        lotteryPanel.gameObject.SetActive(false);
    }
    public void TriggerKickboardPanel(int index)
    {
        kickboardPanel.gameObject.SetActive(true);
        kickboardPanel.Render(index);
    }
    public void TriggerCardPanel(int whatSituation, Action<bool> onResult,bool isCriminal)
    {
        cardPanel.gameObject.SetActive(true);
        cardPanel.PanelOn(whatSituation,onResult, isCriminal);
    }

    public void NoticeCreate(string key) //�˸� ����
    {
        Transform parentCanvas = notice.transform.parent;

        // ��Ȱ��ȭ�� ���¿��� �����մϴ�. ������ ������Ʈ�� Ȱ��ȭ�� ���·� �����˴ϴ�.
        GameObject copy = Instantiate(notice, notice.transform.position, notice.transform.rotation, parentCanvas);
        copy.SetActive(true);
        copy.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = key;

        // 2�� �Ŀ� copy ������Ʈ�� ����
        Destroy(copy, 2f);
    }

    public void NoticeCreatePersuade() //���� ���� GIF ����
    {
        Transform parentCanvas = noticePersuade.transform.parent;

        // ��Ȱ��ȭ�� ���¿��� �����մϴ�. ������ ������Ʈ�� Ȱ��ȭ�� ���·� �����˴ϴ�.
        GameObject copy = Instantiate(noticePersuade, noticePersuade.transform.position, noticePersuade.transform.rotation, parentCanvas);
        copy.SetActive(true);
        // 2�� �Ŀ� copy ������Ʈ�� ����
        Destroy(copy, 2f);
    }

    public void NoticeCreateEver(string key) //�˸� ����, ���
    {
        Transform parentCanvas = notice.transform.parent;

        // ��Ȱ��ȭ�� ���¿��� �����մϴ�. ������ ������Ʈ�� Ȱ��ȭ�� ���·� �����˴ϴ�.
        noticeEver = Instantiate(notice, notice.transform.position, notice.transform.rotation, parentCanvas);
        noticeEver.SetActive(true);
        noticeEver.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = key;

    }
    public void NoticeDelete() //�˸� �����
    {
        Destroy(noticeEver);
    }

    public void OpenGame(int num) //���� �г� ���� �Լ�
    {
        gamePanel[num].SetActive(true);
    }


    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            escButton = true; // Ű�� �� ���� true
        }
        else
        {
            escButton = false;
        }
    }
    private IEnumerator ResetEscButton()
    {
        yield return null; // �� ������ ���
        escButton = false;
    }

    public void UpdateLootGauge(float progress)
    {
        lootGauge.transform.parent.gameObject.SetActive(true);
        //lootGauge.gameObject.SetActive(true);
        lootGauge.fillAmount = progress;
    }

    public void ResetLootGauge()
    {
        lootGauge.transform.parent.gameObject.SetActive(false);
        //lootGauge.gameObject.SetActive(false);
        lootGauge.fillAmount = 0.0f;
    }

    #region Notice Item&Stat
    // �˸� ��û (int: ��Ȳ �ڵ�, string: additionalText, int: number)
    public void NoticeItemCreate(int messageType, int number, LocalizedString additionalText = null) //���� �˸���
    {
        NoticeData data = new NoticeData
        {
            messageType = messageType,
            number = number,
            oldValue = 0f,
            newValue = 0f,
            additionalText = additionalText
        };
        noticeQueue.Enqueue(data);
        if (!isDisplaying)
            StartCoroutine(DisplayNotices());
    }
    public void NoticeItemCreate(int messageType, float oldValue, float newValue, int index) //��ų�������� float�ѱ�� �Լ�
    {
        NoticeData data = new NoticeData
        {
            messageType = messageType,
            number = index,
            oldValue = oldValue,
            newValue = newValue,
            additionalText = null
        };
        noticeQueue.Enqueue(data);

        if (!isDisplaying)
            StartCoroutine(DisplayNotices());
    }


    // �˸��� ���������� ǥ���ϴ� �ڷ�ƾ
    private IEnumerator DisplayNotices()
    {
        isDisplaying = true;

        while (noticeQueue.Count > 0)
        {

            yield return new WaitUntil(() => TimeManager.instance.TimeTicking && _currentUIState == UIState.None); //�ð� �帣�� ui���� None�϶��� ����ǰ�

            NoticeData data = noticeQueue.Dequeue();


            int messageType = data.messageType;
            int number = data.number;
            LocalizedString additionalText = data.additionalText;
            float oldValue = data.oldValue;
            float newValue = data.newValue;

            if (messageType <= 5)
            {
                if (!messageKeyMap.TryGetValue(messageType, out string localizedKey))
                {
                    Debug.LogError($"Invalid messageType: {messageType}");
                    continue;
                }

                // �˸� ������Ʈ ����
                Transform parentCanvas = noticeStat.transform.parent;
                copy = Instantiate(noticeStat, noticeStat.transform.position, noticeStat.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = localizedKey;

                // �߰� �ؽ�Ʈ ó��: messageType�� 0�̸� additionalText�� ���ö����� Ű�� �ؼ�
                string appendedText = "";
                if (messageType == 0)
                {
                    // LocalizeStringEvent ������Ʈ ��������
                    copy.transform.GetChild(2).GetComponent<LocalizeStringEvent>().StringReference = additionalText;
                    string appendedTextReal = copy.transform.GetChild(2).GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString();
                    // string appendedTextReal = copy.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
                    Debug.Log(appendedTextReal + "          aaaa");
                    appendedText = $"{appendedTextReal} ({number})"; // ���ڸ� ��ȣ�� �߰�
                }
                else
                {
                    Image backgroundImage = copy.GetComponent<Image>(); // ������ �˸��� ��� �̹��� ������Ʈ
                    if (backgroundImage != null)
                    {
                        if (((messageType == 1 || messageType == 2) && number > 0) ||
                              ((messageType == 3 || messageType == 4 || messageType == 5) && number < 0)) //���� ������̳� �Ƿε� ������ �ʷϻ�, ��Ʈ������ ���Ұ� �ʷϻ�
                        {
                            backgroundImage.color = new Color(0f, 0.5f, 0f, 0f); // ����� ��� �ʷϻ�
                            copy.GetComponent<UIParallelAnimation>().startColor = new Color(0f, 0.5f, 0f, 0f);
                            copy.GetComponent<UIParallelAnimation>().finalColor = new Color(0f, 0.5f, 0f, 0.8f);
                        }
                        else if (((messageType == 1 || messageType == 2) && number < 0) ||
                                ((messageType == 3 || messageType == 4 || messageType == 5) && number > 0)) //�ݴ��
                        {
                            backgroundImage.color = new Color(0.5f, 0f, 0f, 0f); // ������ ��� ������
                            copy.GetComponent<UIParallelAnimation>().startColor = new Color(0.5f, 0f, 0f, 0f);
                            copy.GetComponent<UIParallelAnimation>().finalColor = new Color(0.5f, 0f, 0f, 0.8f);
                        }
                    }


                    appendedText = number > 0 ? $"+{number}" : number.ToString();
                }

                copy.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = appendedText;

                // 0.5�� ���� ǥ�� �� ����
                yield return new WaitForSeconds(2f);
                Destroy(copy);
            }
            else if(messageType >= 6 && messageType <= 7) //���� ���� ���� �� �������
            {
                // �˸� ������Ʈ ����
                Transform parentCanvas = noticeMoodle.transform.parent;
                copy = Instantiate(noticeMoodle, noticeMoodle.transform.position, noticeMoodle.transform.rotation, parentCanvas);
                copy.SetActive(true);

                if (messageType == 6) //���� ȹ�� ��
                {
                    copy.GetComponent<MoodleNotice>().Render(number, true);
                }
                else //���� ���� ��
                {
                    copy.GetComponent<MoodleNotice>().Render(number, false);
                }
                // 4�� ���� ǥ�� �� ����
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
            else if (messageType == 8) //�� �ߵ��̶��
            {
                // �˸� ������Ʈ ����
                Transform parentCanvas = noticePerk.transform.parent;
                copy = Instantiate(noticePerk, noticePerk.transform.position, noticePerk.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.GetComponent<PerkNotice>().Render(number);

                // 4�� ���� ǥ�� �� ����
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
            else if (messageType == 9) //���� ��ų �������̶��
            {
                // �˸� ������Ʈ ����
                Transform parentCanvas = noticeSkill.transform.parent;
                copy = Instantiate(noticeSkill, noticeSkill.transform.position, noticeSkill.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.GetComponent<SkillNotice>().Render(oldValue, newValue, number);

                // 4�� ���� ǥ�� �� ����
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
            else if (messageType == 10) //���� ����Ʈ ����, �Ϸ� �˸��̸�
            {
                // �˸� ������Ʈ ����
                Transform parentCanvas = noticeSkill.transform.parent;
                copy = Instantiate(noticeQuest, noticeQuest.transform.position, noticeQuest.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.GetComponent<QuestNotice>().Render(number, additionalText);

                // 4�� ���� ǥ�� �� ����
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
        }

        isDisplaying = false;
    }

    private void DestroyCurrentNotice()
    {
        // ���� ���� ���� �˸��� �����ϴ� �޼ҵ�
        if (copy != null)
        {
            copy.GetComponent<UIParallelAnimation>().Pause();
            copy.GetComponent<UIParallelAnimation>().enabled = false;
            Destroy(copy);
            StartCoroutine(DisplayNotices()); //�̾ ���
        }
    }
    public void StartPerkNow(int index)
    {
        StartCoroutine(NoticePerkNow(index));
    }
    private IEnumerator NoticePerkNow(int index) //UI ���� ���� �ð� ���� ������ ����
    {
        Transform parentCanvas = noticePerk.transform.parent;
        GameObject copy = Instantiate(noticePerk, noticePerk.transform.position, noticePerk.transform.rotation, parentCanvas);
        copy.SetActive(true);

        copy.GetComponent<PerkNotice>().Render(index);

        // 2�� ���� ǥ�� �� ����
        yield return new WaitForSeconds(2f);
        Destroy(copy);
    }
    #endregion

    public void StartScreenFade(bool isSix, int sleepTime = 0)  //ȭ�� ����, �߶��� �ð� ���������뵵
    {
        TimeManager.instance.TimeTicking = false;
        isSleepSix = isSix;
        sleepTimes = sleepTime;
        screenFade.StartFadeOut(AfterFadeOut); //���̵�ƿ� ���� �� �ݹ� ����
    }
    public void StartScreenFadeTutorial()  //ȭ�� ����, �߶��� �ð� ���������뵵
    {
        screenFade.StartFadeOut(AfterFadeOutTuto); //���̵�ƿ� ���� �� �ݹ� ����
    }
    private void AfterFadeOutTuto()  //ȭ�� ������ ������ �Լ�
    {
        //SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.HomeGround);
        TutorialManager.instance.Continue();
        SceneTransitionManager.Instance.DestroyManager();
        screenFade.StartFadeIn(AfterFadeIn); //ȭ�� Ű��
    }

    private void AfterFadeOut()  //ȭ�� ������ ������ �Լ�
    {
        //�����Ұ� ����
        if (isSleepSix)
        {
            TimeManager.instance.SkipTimeToSix();
            GameTimeStateManager.instance.OnIsSleep(false);
        }
        else
        {
            TimeManager.instance.SkipTime(sleepTimes,true); //�ð� ������ �Լ� ȣ��
        }
        //TimeManager.instance.SkipTime(sleepTime); //�ð� ������ �Լ� ȣ��
        //TimeManager.instance.TimeTicking = true;
        screenFade.StartFadeIn(AfterFadeIn); //ȭ�� Ű��

    }

    private void AfterFadeIn()  //ȭ�� �� ���� �� ������ �͵�
    {
        TimeManager.instance.TimeTicking = true;
    }

    public void canRunOnOff(bool isOn) //�޸��� �̹��� ���� ���ٶ�
    {
        cantRun.SetActive(isOn);
    }

    public void GameOver(bool isNormal)
    {
        if (isNormal) 
        {
            gameOverN.StartFadeOut();
        }
        else
        {
            gameOverDept.StartFadeOut();
        }
    }
    public void DemoOver() //���� ���� ��
    {
        demoOver.StartFadeOut();
    }

    public void isConversation(bool isConver) //��ȭ�� �������̸� ui�ٲٱ�
    {
        if (isConver)  //���� ��ȭ �����ϸ�
        {
            CurrentUIState = UIState.TimedUI;
        }
        else
        {
            if (AllClosed(timedUIPrompts) && AllClosed(generalUIPrompts))
            {
                CurrentUIState = UIState.None;
            }
            else if (!AllClosed(generalUIPrompts))
            {
                CurrentUIState = UIState.GeneralUI;
            }
            else if (!AllClosed(timedUIPrompts))
            {
                CurrentUIState = UIState.TimedUI;
            }
        }
    }

    public void OnSimulGauge(LocalizedString localized) //�ù� �� ������ ������
    {
        //CurrentUIState = UIState.TimedUI;
        simulGauge.SetActive(true);
        simulGauge.transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
        simulGauge.transform.GetChild(1).GetComponent<LocalizeStringEvent>().StringReference = localized;
        StartCoroutine(FillImageCoroutine());
    }
    private IEnumerator FillImageCoroutine()
    {
        Image gauge = simulGauge.transform.GetChild(0).GetComponent<Image>();
        float elapsedTime = 0f;

        // ������ ä����
        while (elapsedTime < 8f)
        {
            if (CurrentUIState != UIState.TimedUI) 
            {
                CurrentUIState = UIState.TimedUI;
            }
            gauge.fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / 8f); // fillAmount ����
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ä��� �Ϸ�
        gauge.fillAmount = 1f;
        simulGauge.SetActive(false);
       // CurrentUIState = UIState.None;
    }

    public void OnEatWaiting(bool isActive) //�� ������ ui���� �ٲٱ� ���� ����
    {
        eatWaiting.SetActive(isActive);
    }

    public void AddMoney(int amount) //�� ������ ȣ��� �Լ�
    {
        moneyPlus.GetComponent<TextMeshProUGUI>().text = "+\u20A9" + amount.ToString();
        moneyPlus.SetActive(true); //�� ������ ȿ�� Ű��
    }
    public void MinusMoney(int amount) //�� �������� ȣ��� �Լ�
    {
        moneyMinus.GetComponent<TextMeshProUGUI>().text = "-\u20A9" + amount.ToString();
        moneyMinus.SetActive(true); //�� ������ ȿ�� Ű��
    }


    private void OnDestroy() //���θ޴� ���� �ı��ɶ�
    {
        Time.timeScale = 1f;
    }

    public void StartFadeoutScooter(Action move)  //ȭ�� ����, �߶��� �ð� ���������뵵
    {
        moveScooter.StartFadeOut(move); //���̵�ƿ� ���� �� �ݹ� ����
    }
    public void StopFadeoutScooter() 
    {
        moveScooter.OffFade();
    }

    public void OnMinimap(bool isOn)
    {
        minimap.SetActive(isOn);
    }
}