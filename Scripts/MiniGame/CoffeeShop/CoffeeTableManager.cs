using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CoffeeTableManager : MonoBehaviour, HelpPanel
{

    // TableState 객체를 포함하는 리스트 초기화
    private List<TableState> coffeeMenu = new List<TableState>
    {                              //언더샷 오버샷 우유   얼음  딸시   초시   카시    크림   블렌딩
        new TableState(new bool[8] { true, false, false, true, false, false, false, false}, false), //아메리카노
        new TableState(new bool[8] { false, false, true, true, true, false, false, true}, true),
        new TableState(new bool[8] { false, false, true, true, false, true, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, true, true}, true), //카라멜 프랍
        new TableState(new bool[8] { false, false, true, true, true, false, false, false}, false),
        new TableState(new bool[8] { false, false, true, true, false, true, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, true, false}, false),  //카라멜 마끼
    };
    [SerializeField] protected SimulationJobStat jobStat;

    [SerializeField]
    protected TextMeshProUGUI Timebox; //시간 ui
    [SerializeField]
    protected Image TimeImage; //시간 그래프
    [SerializeField]
    protected LocalizeStringEvent timeString; //시간 텍스트랑 같은 옵젝
    [SerializeField]
    protected GameObject resultPanel; //시간 텍스트랑 같은 옵젝

    [SerializeField]
    protected TextMeshProUGUI moneyText;
    [SerializeField]
    protected TextMeshProUGUI tipText;
    /*[SerializeField]
    protected GameObject moneyPlus;*/
    [SerializeField]
    protected GameObject tipPlus;
    protected int money, tip;

    [SerializeField]
    protected GameObject notice;

    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //도움말 버튼

    public int Money
    {
        get
        {
            return money; // 속성 값을 반환
        }
        set
        {
            money = value;

        }
    }
    /*public void AddMoney(int amount)
    {
        Money += amount; // 속성의 set 접근자가 호출되면서 로그가 출력됨
        moneyPlus.GetComponent<TextMeshProUGUI>().text = "+\u20A9" + amount.ToString();
        moneyText.text = "\u20A9" + money.ToString();
        moneyPlus.SetActive(true); //돈 오르는 효과 키기
    }*/
    public int Tip
    {
        get
        {
            return tip; // 속성 값을 반환
        }
        set
        {
            tip = value;
        }
    }
    public void AddTip(int amount)
    {
        Tip += amount; // 속성의 set 접근자가 호출되면서 로그가 출력됨
        tipPlus.GetComponent<TextMeshProUGUI>().text = "+\u20A9" + amount.ToString();
        tipText.text = "\u20A9" + tip.ToString();
        tipPlus.SetActive(true); //돈 오르는 효과 키기    
    }

    [SerializeField]
    protected GameObject npc,npcFemale; //복제할 npc
    [SerializeField]
    protected Transform npcGenPo; //npc 생성 위치
    [SerializeField]
    protected Transform npcExitPo; //npc 나가는 위치
    public bool subChatOn = false; //두번째 메뉴 시킬지 여부

    protected int difficult; //난이도 임시 변수, 나중에 난이도 매니저에서 받아올거

    public float gameTime; //게임 시간
    protected float currentTime; //게임 시간
    [SerializeField]
    protected float baseTimeBetweenCustomers;  // 기본 손님 대기 시간 (초)

    public static CoffeeTableManager instance = null;
    public Status stat; //원래 가지고 있던 스탯 수치 저장

    public int mainMenu, subMenu; //각 메뉴 숫자

    [SerializeField]
    protected Transform[] points; // NPC 대기 위치가 저장된 배열
    [SerializeField]
    protected int maxNPCs = 8; // 최대 NPC 수
    protected List<GameObject> npcList = new List<GameObject>();
    protected int currentNPCIndex = 0; // 현재 NPC가 들어갈 위치 인덱스

    protected void Awake()
    {
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }
    protected void Start()
    {
        helpPanel.SetActive(false);
        Money = jobStat.BaseSalary; //baseSalary 설정
        moneyText.text = "\u20A9" + Money.ToString();

        TimeManager.instance.TimeTicking = false; //시간 흐르는거 멈추게하기
        InvokeRepeating(nameof(ClockUpdate), 1.0f, 1.0f);
        //TimeManager.instance.RegisterTracker(this);

        currentTime = gameTime;
        StartCoroutine(SpawnCustomers());
        stat = StatusManager.instance.GetStatus().Clone();
    }
    private void Update()
    {
        if (helpButton)
        {
            helpButton = false;
            if (Time.timeScale != 0) //시간 안멈춰있을때만
            {
                helpPanel.SetActive(true);
            }
        }
    }

    protected virtual void CreateOrder() //손님 생성 함수
    {

        subChatOn = false; //초기화
        mainMenu = Random.Range(0, 9);
        int peopleNum = Random.Range(0, 2); //0~1 사이 난수 생성, 서브메뉴 결정용

        if (peopleNum == 0)
        {
            // tableObject[i1].GetComponent<TableObject>().SubOn = true;
            subChatOn = true; //서브메뉴 말풍선도 생김
            subMenu = Random.Range(0, 5); //0에서 4번 메뉴 중 랜덤
        }

        GameObject npc1;
        int rando1 = Random.Range(0, 2); //50퍼, 성별 여부
        if (rando1 == 0)
        {
            npc1 = Instantiate(npc, npcGenPo); //npc 생성
        }
        else
        {
            npc1 = Instantiate(npcFemale, npcGenPo);
        }
        npc1.GetComponent<HumanoidControllerCafe>().Appearance(); //npc 외형 결정하는 함수 호출
                                                              // NPC가 npcExit로 이동하게 함
        NavMeshAgent agent = npc1.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(points[currentNPCIndex].position); // NavMesh를 사용해 원하는 위치로 이동
            npc1.GetComponent<HumanoidControllerCafe>().isDestination = true;
        }
        /*int a = Random.Range(0, 3);
        npc1.GetComponent<Animator>().SetFloat("IdleState", a);*/ //대기 모션 랜덤 재생

        npcList.Add(npc1); // NPC를 리스트에 추가
        if(npcList[0] == npc1)
        {
            npc1.GetComponent<HumanoidControllerCafe>().isFirst = true;
        }
        currentNPCIndex++; // 다음 NPC가 들어갈 위치를 가리킴

        
    }

    //맨앞 npc 말풍선 띄우고 주문받기
    protected void OrderUp()
    {
        // frontNPC.GetComponent<HumanoidControllerCafe>().Hello(); //말풍선띄우기
        if(npcList.Count > 0)
        {
            npcList[0].GetComponent<HumanoidControllerCafe>().isFirst = true;
        }

    }

    public void Angry() //잘못된 음식 들어갈 시 호출
    {
        GameObject frontNPC = npcList[0];
        frontNPC.GetComponent<HumanoidControllerCafe>().Angry();
        RemoveFrontNPC();
    }
    public void EatGood() //주문 제대로 들어갈시
    {
        GameObject frontNPC = npcList[0];
        frontNPC.GetComponent<HumanoidControllerCafe>().Good();
        RemoveFrontNPC();
    }

    public bool CheckCoffee(bool[] ingre, bool mix, bool sideOn, int sideId)
    {
        if (npcList[0] == null) //만약 아무도 없다면
        {
            return false;
        }
        bool subOn = npcList[0].GetComponent<HumanoidControllerCafe>().subOn;
        int sMenu = npcList[0].GetComponent<HumanoidControllerCafe>().sMenu;
        int mMenu = npcList[0].GetComponent<HumanoidControllerCafe>().mMenu;
        GameObject frontNPC = npcList[0];
        if(npcList[0].GetComponent<HumanoidControllerCafe>().isFirst) //만약 아직 첫사람 도착 못했을시
        {
            return false;
        }

        if (subOn) //사이드 주문 있음
        {
            if (!sideOn || sideId != sMenu) //만약 사이드 없다면, 사이드메뉴 불일치 시
            {
                Angry();
                return false;
            }
        }
        else
        {
            if (sideOn)  //만약 주문없는데 뭔가 올라가져있다면
            {
                //돈 깎고 리턴시키기
            }
        }
        if (mix != coffeeMenu[mMenu].isCook) //만약 믹스 상태 다르면
        {
            Angry();
            return false;
        }
        // ingre 배열의 각 요소를 기준 상태와 비교
        for (int i = 0; i < ingre.Length; i++)
        {
            if (ingre[i] != coffeeMenu[mMenu].ingre[i]) //재료 불일치 시
            {
                Angry();
                return false;
            }
        }
        EatGood();
        return true;
    }

    // NPC 제거 (맨 앞 NPC를 npcExit 위치로 이동시킨 후, 뒤 NPC들 이동)
    private void RemoveFrontNPC()
    {
        if (npcList.Count > 0)
        {
            GameObject frontNPC = npcList[0]; // 맨 앞 NPC

            // NPC가 npcExit로 이동하게 함
            NavMeshAgent agent = frontNPC.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(npcExitPo.position); // NavMesh를 사용해 npcExit으로 이동
            }

            // 일정 시간 후 리스트에서 제거 (NPC가 완전히 나갔다고 가정)
            StartCoroutine(RemoveNPCAfterExit(frontNPC));

            npcList.RemoveAt(0); // 리스트에서 제거

            // 나머지 NPC들 이동 (NavMesh를 사용해 포인트로 이동)
            for (int i = 0; i < npcList.Count; i++)
            {
                GameObject npc = npcList[i];
                NavMeshAgent npcAgent = npc.GetComponent<NavMeshAgent>();
                if (npcAgent != null)
                {
                    npcAgent.SetDestination(points[i].position); // 다음 위치로 이동
                    npc.GetComponent<HumanoidControllerCafe>().isDestination = true;
                }
            }

            currentNPCIndex--; // 다음 생성될 NPC의 인덱스를 조정
            OrderUp(); //다음손님 호출
        }
    }

    //손님 생성 함수 관련
    protected IEnumerator SpawnCustomers()
    {
        while (currentTime > 60) //종료 60초 전부터 생성 금지
        {

            // 현재 비어있는 테이블 수에 따라 손님 대기 시간 조절
            float timeBetweenCustomers = Mathf.Lerp(1f, baseTimeBetweenCustomers, (float) currentNPCIndex / maxNPCs);

            // 사용 가능한 테이블이 있을 때만 손님 생성
            if (currentNPCIndex < maxNPCs)
            {
                yield return new WaitForSeconds(timeBetweenCustomers);

                if (currentTime >= 0) //만약 0초 이상 남았다면
                {
                    CreateOrder(); //손님 생성
                }
            }
            else
            {
                yield return null; // 사용 가능한 테이블이 없을 경우 대기
            }
        }

        while (currentTime <= 60) // 0초부터 폴스카운트 검사
        {

            // npc가 0일 경우 함수 호출 후 코루틴 종료
            if (currentNPCIndex == 0 && currentTime <= 0)
            {
                resultPanel.SetActive(true); // falseCount가 0일 때 결과 화면 함수 호출
                resultPanel.GetComponent<ResultPanel>().Render();
                yield break; // 코루틴 종료
            }

            // 매 프레임마다 falseCount를 계속 검사
            yield return null;
        }

        yield return null;
    }

    // NPC가 npcExit로 완전히 이동한 후 리스트에서 제거
    IEnumerator RemoveNPCAfterExit(GameObject npc)
    {
        yield return new WaitForSeconds(10f); // NPC가 나갈 시간 대기 (시간은 상황에 맞게 조정)
        Destroy(npc); // NPC 오브젝트 삭제
    }



    public void ClockUpdate()
    {
        if (currentTime <= 0) //시간 끝나면
        {
            timeString.StringReference.TableEntryReference = "extratime_key";  //오버타임 텍스트 띄우기
            //결과 화면 띄우기

        }
        else
        {
            currentTime--;
            int minutes = Mathf.FloorToInt(currentTime / 60); // 분
            int seconds = Mathf.FloorToInt(currentTime % 60); // 초
                                                              // 분은 한 자리, 초는 두 자리로 표시
            Timebox.text = string.Format("{0}:{1:00}", minutes, seconds);
            TimeImage.fillAmount = (float)currentTime / gameTime;
        }
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

    #region HelpPanel
    public void HelpOn()
    {
        helpPanel.SetActive(true);
    }
    public void HelpOff()
    {
        helpPanel.SetActive(false);
    }

    public void OnHelpButton(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                helpButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                helpButton = false;
                break;
        }
    }
    #endregion
}
