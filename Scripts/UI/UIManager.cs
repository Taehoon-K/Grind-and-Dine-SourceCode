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
    private TextMeshProUGUI Timebox; //시간 ui
    [SerializeField]
    private TextMeshProUGUI dateBox;
    [SerializeField] private GameObject minimap; //미니맵

    // 필요한 이미지
    [SerializeField]
    private Image[] images_Gauge;
    [SerializeField]
    private Sprite[] images_Mood; //감정 이미지 바꾸는 용도
    [SerializeField]
    private Image imageCurrentMood; //감정 이미지 바꾸는 용도

    

    [Header("Plater Stats")]
    [SerializeField]
    private TextMeshProUGUI moneyText;

    [SerializeField]
    protected GameObject moneyPlus; //돈 효과
    [SerializeField]
    protected GameObject moneyMinus; //돈 효과
    
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
    [SerializeField] private GameObject noticePersuade; //설득 성공시 나올 GIF

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
    [SerializeField] private CardPanel cardPanel; //리스크행동시 카드 뽑을 카드패널

    [Header("Item & Stat Notice")]
    [SerializeField] private GameObject noticeStat; // 알림 Prefab
    [SerializeField] private GameObject noticeMoodle; // 무들 알림 프리팹
    [SerializeField] private GameObject noticePerk; // 무들 알림 프리팹
    [SerializeField] private GameObject noticeSkill; // 스킬 경험치 얻을시 알림 프리팹
    [SerializeField] private GameObject noticeQuest; // 퀘스트 노티스

    private struct NoticeData
    {
        public int messageType;
        public int number; // 기본적으로는 여기에 기존 숫자들 (ex: 증가량)을 넣음
        public float oldValue;
        public float newValue;
        public LocalizedString additionalText;
    }
    private Queue<NoticeData> noticeQueue = new Queue<NoticeData>(); // 알림 데이터를 저장하는 큐
    private bool isDisplaying = false; // 현재 알림 표시 여부

    private bool isSleepSix;
    private int sleepTimes;

    [Header("Screen Fade & GameOver")]
    [SerializeField] private ScreenFadeOff screenFade;
    [SerializeField] private ScreenFadeOff gameOverN, gameOverDept,demoOver,moveScooter;

    [SerializeField] private CanvasGroup canvasPrompt,canvasNotice; //인벤 띄울 시 사라질 ui들

    [SerializeField] private GameObject cantRun; //못달릴때 띄우는 옵젝

    [SerializeField] private GameObject simulGauge;

    [SerializeField] private GameObject eatWaiting; //뭐 먹을 때 timedUI로 바꾸려고 띄우는 가짜 ui

    [Header("각 UI 상태에 포함될 UI 프롬프트 그룹")]
    [SerializeField] private GameObject[] generalUIPrompts;  // GeneralUI 상태에 해당하는 프롬프트들
    //[SerializeField] private GameObject[] inventoryUIPrompts; // InventoryUI 상태에 해당하는 프롬프트들
    [SerializeField] private GameObject[] timedUIPrompts;     // TimedUI 상태에 해당하는 프롬프트들
    GameObject copy;

    // 상황에 맞는 로컬라이즈 키를 저장하는 딕셔너리
    private Dictionary<int, string> messageKeyMap = new Dictionary<int, string>()
    {
        { 0, "AddItem_key" },
        { 1, "AddVigor_key" }, //피로도 증감
        { 2, "AddSatiety_key" }, //배고픔 증감
        { 3, "AddAnger_key" }, //화남
        { 4, "AddSadness_key" }, //슬픔
        { 5, "AddBoredom_key" }, //지루함
        { 6, null }, //무들 생성
        { 7, null }, //무들 제거
        { 8, null }, //퍽 발동
    };

    private void Awake()
    {
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
            DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }

        //_uiStateStack.Push(UIState.None);
    }

    private void Start()
    {
        //트레이드 인벤 오류 방지용으로 한번 켰다 끄는 용도
        shopListingManager.gameObject.SetActive(false);

        //시간 업뎃 될때마다 등록
        TimeManager.instance.RegisterTracker(this);
        StatusManager.instance.RegisterTracker(this);
        RenderPlayerStats();
        //PlayerStats.Earn(500000); //테스트용

        

        Cursor.visible = false; //커서 끄기
    }
    private void OnEnable()
    {
        // 남아있는 알림을 삭제
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
        None,        // 기본 상태
        GeneralUI,   // 일반 UI (시간 멈춤)
        InventoryUI, // 인벤토리 UI (시간 멈춤)
        TimedUI      // 시간 진행 UI (미니게임 결과창 등)
    }

    private UIState _currentUIState = UIState.None; // 현재 UI 상태
   //private UIState previousUIState = UIState.None; // 현재 UI 상태

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
                    Time.timeScale = 1; // 시간 정상 진행
                    Cursor.visible = false;
                    if(m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("Player");
                        InputSystem.ResetHaptics(); // 모든 입력 상태를 초기화
                    }
                    canvasNotice.alpha = 1;
                    break;

                case UIState.GeneralUI:
                    Time.timeScale = 0; // 시간 멈춤
                    if (m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("UIOn");
                        InputSystem.ResetHaptics(); // 모든 입력 상태를 초기화
                    }                  
                    Cursor.visible = true;
                    canvasNotice.alpha = 0;
                    break;

                case UIState.InventoryUI:
                    Time.timeScale = 0; // 시간 멈춤
                    Cursor.visible = true;
                    if (m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("Player");
                        InputSystem.ResetHaptics(); // 모든 입력 상태를 초기화
                    }
                    canvasNotice.alpha = 0;
                    break;

                case UIState.TimedUI:
                    Time.timeScale = 1; // 시간 진행
                    if (m_Action != null)
                    {
                        m_Action.SwitchCurrentActionMap("UIOn");
                        InputSystem.ResetHaptics(); // 모든 입력 상태를 초기화
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
    /// 특정 UI 프롬프트가 활성화될 때 UI 상태를 변경
    /// </summary>
    public void OnUIPromptOpened(GameObject openedPrompt)
    {
        /*
        //Debug.Log("Ui열림");
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
        /* yield return null; // 한 프레임 대기

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
        yield return null; // 한 프레임 대기

        CanvasOnOff inven = FindObjectOfType<CanvasOnOff>();
        if (inven != null && inven.IsInvenOn()) //만약 인벤 켜져있다면
        {
            CurrentUIState = UIState.InventoryUI;
        }
        // 모든 프롬프트가 꺼져 있으면 None
        else if (AllClosed(generalUIPrompts) && AllClosed(timedUIPrompts))
        {
            CurrentUIState = UIState.None;
        }
        // 일반 UI만 열려 있으면 GeneralUI
        else if (!AllClosed(generalUIPrompts) && AllClosed(timedUIPrompts))
        {
            CurrentUIState = UIState.GeneralUI;
        }
        // 타임드 UI만 열려 있으면 TimedUI
        else if (AllClosed(generalUIPrompts) && !AllClosed(timedUIPrompts))
        {
            CurrentUIState = UIState.TimedUI;
        }
        // 둘 다 열려 있으면 우선순위 고려해서 TimedUI 선택 (혹은 GeneralUI로)
        else
        {
            CurrentUIState = UIState.TimedUI;
        }
    }

    /// <summary>
    /// 특정 배열에 게임 오브젝트가 포함되어 있는지 확인
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
    /// 특정 UI 그룹이 전부 비활성화되었는지 확인
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

        // 기본값은 AM
        string prefix = "AM";

        // 12시간마다 전환
        if (hours >= 12)
        {
            prefix = "PM"; // 12시부터 오후
        }

        // 12시간 형식으로 변경
        if (hours > 12)
        {
            hours -= 12; // 13~23시는 12를 뺌
        }
        else if (hours == 0)
        {
            hours = 12; // 0시는 12시(자정)로 표시
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
        images_Gauge[3].fillAmount = (float)status.currentHp / 1000;  //hp 수치
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
            // 500 이상인 값 중 가장 큰 값을 찾고 그에 해당하는 인덱스 활성화
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
    public void RenderPlayerStats() //돈 등 스탯 관리
    {
        moneyText.text = PlayerStats.Money.ToString("N0") + PlayerStats.CURRENCY;
        shopListingManager.RenderMoney(); //상점 돈 업뎃
    }

    public void OpenShop(List<ItemData> shopItems, string filter) //상점 열때 함수
    {
        //CurrentUIState = UIState.GeneralUI;
        shopListingManager.gameObject.SetActive(true);
        shopListingManager.RenderShop(shopItems,filter);
    }
    
    //yesno 프롬프트 출력 함수
    public void TriggerYesNoPrompt(LocalizedString message, System.Action onYesCallback)
    {
        yesNoPrompt.gameObject.SetActive(true);
        yesNoPrompt.CreatePrompt(message, onYesCallback);
    }

    //슬립 프롬프트 호출 함수
    public void TriggerSleepPrompt(bool isJail)
    {
        //CurrentUIState = UIState.GeneralUI;
        if (TimeManager.instance.isSiesta()) //만약 낮잠자는 시간이면
        {
            sleepPrompt.OpenScreen(isJail);
        }
        else
        {
            sleepPromptSix.OpenScreen(isJail);
        }
    }

    //엘베 프롬프트 호출 함수
    public void TriggerElevatorPrompt(int floor)
    {
       // CurrentUIState = UIState.GeneralUI;
        elevatorPrompt.OpenScreen(floor);
    }

    //스킬 프롬프트 호출 함수
    public void TriggerSkillPrompt(int skillN,int levelN)
    {
        //CurrentUIState = UIState.GeneralUI;
        skillSelectPrompt.OpenScreen(skillN,levelN);
    }
    //스킬 프롬프트 나가기
    public void ExitSkillPrompt()
    {
        //CurrentUIState = UIState.InventoryUI;
    }

    //엘베 프롬프트 호출 함수
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

    public void NoticeCreate(string key) //알림 띄우기
    {
        Transform parentCanvas = notice.transform.parent;

        // 비활성화된 상태에서 복제합니다. 복제된 오브젝트는 활성화된 상태로 생성됩니다.
        GameObject copy = Instantiate(notice, notice.transform.position, notice.transform.rotation, parentCanvas);
        copy.SetActive(true);
        copy.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = key;

        // 2초 후에 copy 오브젝트를 삭제
        Destroy(copy, 2f);
    }

    public void NoticeCreatePersuade() //설득 성공 GIF 띄우기
    {
        Transform parentCanvas = noticePersuade.transform.parent;

        // 비활성화된 상태에서 복제합니다. 복제된 오브젝트는 활성화된 상태로 생성됩니다.
        GameObject copy = Instantiate(noticePersuade, noticePersuade.transform.position, noticePersuade.transform.rotation, parentCanvas);
        copy.SetActive(true);
        // 2초 후에 copy 오브젝트를 삭제
        Destroy(copy, 2f);
    }

    public void NoticeCreateEver(string key) //알림 띄우기, 계속
    {
        Transform parentCanvas = notice.transform.parent;

        // 비활성화된 상태에서 복제합니다. 복제된 오브젝트는 활성화된 상태로 생성됩니다.
        noticeEver = Instantiate(notice, notice.transform.position, notice.transform.rotation, parentCanvas);
        noticeEver.SetActive(true);
        noticeEver.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = key;

    }
    public void NoticeDelete() //알림 지우기
    {
        Destroy(noticeEver);
    }

    public void OpenGame(int num) //게임 패널 열때 함수
    {
        gamePanel[num].SetActive(true);
    }


    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            escButton = true; // 키를 뗄 때만 true
        }
        else
        {
            escButton = false;
        }
    }
    private IEnumerator ResetEscButton()
    {
        yield return null; // 한 프레임 대기
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
    // 알림 요청 (int: 상황 코드, string: additionalText, int: number)
    public void NoticeItemCreate(int messageType, int number, LocalizedString additionalText = null) //기존 알림용
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
    public void NoticeItemCreate(int messageType, float oldValue, float newValue, int index) //스킬레벨업용 float넘기는 함수
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


    // 알림을 순차적으로 표시하는 코루틴
    private IEnumerator DisplayNotices()
    {
        isDisplaying = true;

        while (noticeQueue.Count > 0)
        {

            yield return new WaitUntil(() => TimeManager.instance.TimeTicking && _currentUIState == UIState.None); //시간 흐르고 ui상태 None일때만 실행되게

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

                // 알림 오브젝트 생성
                Transform parentCanvas = noticeStat.transform.parent;
                copy = Instantiate(noticeStat, noticeStat.transform.position, noticeStat.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = localizedKey;

                // 추가 텍스트 처리: messageType이 0이면 additionalText도 로컬라이즈 키로 해석
                string appendedText = "";
                if (messageType == 0)
                {
                    // LocalizeStringEvent 컴포넌트 가져오기
                    copy.transform.GetChild(2).GetComponent<LocalizeStringEvent>().StringReference = additionalText;
                    string appendedTextReal = copy.transform.GetChild(2).GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString();
                    // string appendedTextReal = copy.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
                    Debug.Log(appendedTextReal + "          aaaa");
                    appendedText = $"{appendedTextReal} ({number})"; // 숫자를 괄호로 추가
                }
                else
                {
                    Image backgroundImage = copy.GetComponent<Image>(); // 복제된 알림의 배경 이미지 컴포넌트
                    if (backgroundImage != null)
                    {
                        if (((messageType == 1 || messageType == 2) && number > 0) ||
                              ((messageType == 3 || messageType == 4 || messageType == 5) && number < 0)) //만약 배고픔이나 피로도 증가는 초록색, 스트레스는 감소가 초록색
                        {
                            backgroundImage.color = new Color(0f, 0.5f, 0f, 0f); // 양수일 경우 초록색
                            copy.GetComponent<UIParallelAnimation>().startColor = new Color(0f, 0.5f, 0f, 0f);
                            copy.GetComponent<UIParallelAnimation>().finalColor = new Color(0f, 0.5f, 0f, 0.8f);
                        }
                        else if (((messageType == 1 || messageType == 2) && number < 0) ||
                                ((messageType == 3 || messageType == 4 || messageType == 5) && number > 0)) //반대로
                        {
                            backgroundImage.color = new Color(0.5f, 0f, 0f, 0f); // 음수일 경우 빨간색
                            copy.GetComponent<UIParallelAnimation>().startColor = new Color(0.5f, 0f, 0f, 0f);
                            copy.GetComponent<UIParallelAnimation>().finalColor = new Color(0.5f, 0f, 0f, 0.8f);
                        }
                    }


                    appendedText = number > 0 ? $"+{number}" : number.ToString();
                }

                copy.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = appendedText;

                // 0.5초 동안 표시 후 삭제
                yield return new WaitForSeconds(2f);
                Destroy(copy);
            }
            else if(messageType >= 6 && messageType <= 7) //만약 무들 생성 및 삭제라면
            {
                // 알림 오브젝트 생성
                Transform parentCanvas = noticeMoodle.transform.parent;
                copy = Instantiate(noticeMoodle, noticeMoodle.transform.position, noticeMoodle.transform.rotation, parentCanvas);
                copy.SetActive(true);

                if (messageType == 6) //무들 획득 시
                {
                    copy.GetComponent<MoodleNotice>().Render(number, true);
                }
                else //무들 제거 시
                {
                    copy.GetComponent<MoodleNotice>().Render(number, false);
                }
                // 4초 동안 표시 후 삭제
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
            else if (messageType == 8) //퍽 발동이라면
            {
                // 알림 오브젝트 생성
                Transform parentCanvas = noticePerk.transform.parent;
                copy = Instantiate(noticePerk, noticePerk.transform.position, noticePerk.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.GetComponent<PerkNotice>().Render(number);

                // 4초 동안 표시 후 삭제
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
            else if (messageType == 9) //만약 스킬 레벨업이라면
            {
                // 알림 오브젝트 생성
                Transform parentCanvas = noticeSkill.transform.parent;
                copy = Instantiate(noticeSkill, noticeSkill.transform.position, noticeSkill.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.GetComponent<SkillNotice>().Render(oldValue, newValue, number);

                // 4초 동안 표시 후 삭제
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
            else if (messageType == 10) //만약 퀘스트 시작, 완료 알림이면
            {
                // 알림 오브젝트 생성
                Transform parentCanvas = noticeSkill.transform.parent;
                copy = Instantiate(noticeQuest, noticeQuest.transform.position, noticeQuest.transform.rotation, parentCanvas);
                copy.SetActive(true);

                copy.GetComponent<QuestNotice>().Render(number, additionalText);

                // 4초 동안 표시 후 삭제
                yield return new WaitForSeconds(4f);
                Destroy(copy);
            }
        }

        isDisplaying = false;
    }

    private void DestroyCurrentNotice()
    {
        // 현재 실행 중인 알림을 삭제하는 메소드
        if (copy != null)
        {
            copy.GetComponent<UIParallelAnimation>().Pause();
            copy.GetComponent<UIParallelAnimation>().enabled = false;
            Destroy(copy);
            StartCoroutine(DisplayNotices()); //이어서 출력
        }
    }
    public void StartPerkNow(int index)
    {
        StartCoroutine(NoticePerkNow(index));
    }
    private IEnumerator NoticePerkNow(int index) //UI 상태 뭐든 시간 뭐든 강제로 띄우기
    {
        Transform parentCanvas = noticePerk.transform.parent;
        GameObject copy = Instantiate(noticePerk, noticePerk.transform.position, noticePerk.transform.rotation, parentCanvas);
        copy.SetActive(true);

        copy.GetComponent<PerkNotice>().Render(index);

        // 2초 동안 표시 후 삭제
        yield return new WaitForSeconds(2f);
        Destroy(copy);
    }
    #endregion

    public void StartScreenFade(bool isSix, int sleepTime = 0)  //화면 끄기, 잘때나 시간 지나갈때용도
    {
        TimeManager.instance.TimeTicking = false;
        isSleepSix = isSix;
        sleepTimes = sleepTime;
        screenFade.StartFadeOut(AfterFadeOut); //페이드아웃 실행 후 콜백 받음
    }
    public void StartScreenFadeTutorial()  //화면 끄기, 잘때나 시간 지나갈때용도
    {
        screenFade.StartFadeOut(AfterFadeOutTuto); //페이드아웃 실행 후 콜백 받음
    }
    private void AfterFadeOutTuto()  //화면 꺼지고 실행할 함수
    {
        //SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.HomeGround);
        TutorialManager.instance.Continue();
        SceneTransitionManager.Instance.DestroyManager();
        screenFade.StartFadeIn(AfterFadeIn); //화면 키기
    }

    private void AfterFadeOut()  //화면 꺼지고 실행할 함수
    {
        //실행할거 쓰고
        if (isSleepSix)
        {
            TimeManager.instance.SkipTimeToSix();
            GameTimeStateManager.instance.OnIsSleep(false);
        }
        else
        {
            TimeManager.instance.SkipTime(sleepTimes,true); //시간 보내는 함수 호출
        }
        //TimeManager.instance.SkipTime(sleepTime); //시간 보내는 함수 호출
        //TimeManager.instance.TimeTicking = true;
        screenFade.StartFadeIn(AfterFadeIn); //화면 키기

    }

    private void AfterFadeIn()  //화면 다 켜진 후 실행할 것들
    {
        TimeManager.instance.TimeTicking = true;
    }

    public void canRunOnOff(bool isOn) //달리는 이미지 띄우고 없앨때
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
    public void DemoOver() //데모 끝날 시
    {
        demoOver.StartFadeOut();
    }

    public void isConversation(bool isConver) //대화문 실행중이면 ui바꾸기
    {
        if (isConver)  //만약 대화 시작하면
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

    public void OnSimulGauge(LocalizedString localized) //시뮬 시 켜지는 게이지
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

        // 서서히 채워짐
        while (elapsedTime < 8f)
        {
            if (CurrentUIState != UIState.TimedUI) 
            {
                CurrentUIState = UIState.TimedUI;
            }
            gauge.fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / 8f); // fillAmount 변경
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 채우기 완료
        gauge.fillAmount = 1f;
        simulGauge.SetActive(false);
       // CurrentUIState = UIState.None;
    }

    public void OnEatWaiting(bool isActive) //뭐 먹을때 ui상태 바꾸기 위한 땜빵
    {
        eatWaiting.SetActive(isActive);
    }

    public void AddMoney(int amount) //돈 오를때 호출될 함수
    {
        moneyPlus.GetComponent<TextMeshProUGUI>().text = "+\u20A9" + amount.ToString();
        moneyPlus.SetActive(true); //돈 오르는 효과 키기
    }
    public void MinusMoney(int amount) //돈 내려갈때 호출될 함수
    {
        moneyMinus.GetComponent<TextMeshProUGUI>().text = "-\u20A9" + amount.ToString();
        moneyMinus.SetActive(true); //돈 오르는 효과 키기
    }


    private void OnDestroy() //메인메뉴 가서 파괴될때
    {
        Time.timeScale = 1f;
    }

    public void StartFadeoutScooter(Action move)  //화면 끄기, 잘때나 시간 지나갈때용도
    {
        moveScooter.StartFadeOut(move); //페이드아웃 실행 후 콜백 받음
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