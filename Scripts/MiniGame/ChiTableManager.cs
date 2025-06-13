using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ChiTableManager : MonoBehaviour, HelpPanel //, ITimeTracker
{
    [SerializeField] protected SimulationJobStat jobStat;

    [SerializeField]
    protected TextMeshProUGUI Timebox; //시간 ui
    [SerializeField]
    protected Image TimeImage; //시간 그래프
    [SerializeField]
    protected LocalizeStringEvent timeString; //시간 텍스트랑 같은 옵젝
   // protected bool timeOver; //시간 끝났을시 호출
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
    protected int money,tip;

    [SerializeField]
    protected bool[] tableOn;
    [SerializeField]
    protected bool[] orderUp =   new bool[8]; //트루 되면 음식 놓을 수 있게

    [SerializeField]
    protected GameObject notice; //알림 띄우는 옵젝

    [SerializeField] protected int despawnTime; //손님 스폰 안하는 시간 초 설정
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
    public bool GetOrderUp(int index)
    {
        return orderUp[index];
    }
    public void SetOrderUp(int index, bool value)
    {
        orderUp[index] = value;
        tableObject[index].GetComponent<BoxCollider>().enabled = value; //테이블 콜라이더 활,비활성화
        tableObject[index].GetComponent<TableObject>().myindex = index; //테이블에 자기 번호 저장
    }
    public void SetTableOn(int index) //성공적으로 오더 끝났을때만
    {
        //tableOn[index] = false;
        tableObject[index].GetComponent<BoxCollider>().enabled = true;
        tableObject[index].GetComponent<TableObject>().EatFinish();

    }
    public void AngryOn(int index) //화났을시
    {       
        tableObject[index].GetComponent<TableObject>().EatFinish(true);

        //tableObject[index].GetComponent<TableObject>().Reset1();
        tableObject[index].GetComponent<TableObject>().TableAngry();

    }
    public GameObject[] tableChair; //의자 오브젝트
    public GameObject[] tableSubChair; //서브 의자 오브젝트
    [SerializeField]
    protected GameObject[] tableObject; //테이블 오브젝트
    [SerializeField]
    protected GameObject npc; //복제할 npc
    [SerializeField]
    protected GameObject npcSub; //복제할 서브 npc
    [SerializeField]
    protected Transform npcGenPo; //npc 생성 위치
    [SerializeField]
    protected Transform npcSubGenPo; //npc 서브 생성 위치
    public int tableId;
    public bool subChatOn = false; //두번째 메뉴 시킬지 여부
    public bool drinkChatOn = false; //음료수 메뉴 시킬지 여부
    public int mainMenu,subMenu,drinkMenu; //각 메뉴 숫자
    protected int i1;

    protected GameObject[] projectile = new GameObject[8];
    protected GameObject[] projectileSub = new GameObject[8];

    protected int difficult; //난이도 임시 변수, 나중에 난이도 매니저에서 받아올거


    public float gameTime; //게임 시간
    protected float currentTime; //게임 시간
    [SerializeField]
    protected float baseTimeBetweenCustomers;  // 기본 손님 대기 시간 (초)

    public static ChiTableManager instance = null;
    public Status stat; //원래 가지고 있던 스탯 수치 저장

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
    protected void Update()
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

    protected virtual void createOrder(int count) //손님 생성 함수
    {
        subChatOn = false;
        drinkChatOn = false; //초기화

        int falseCount = count;

        // 폴스 갯수 중 랜덤
        int randomIndex = Random.Range(0, falseCount);
        // Find the random false element and set it to true
        int currentIndex = 0;
        for (int i = 0; i < tableOn.Length; i++)
        {
            if (!tableOn[i])
            {
                if (currentIndex == randomIndex)
                {
                    tableOn[i] = true;
                    tableId = i;
                    Debug.Log("Random false element changed to true at index: " + i);
                    i1 = i;
                    break;
                }
                currentIndex++;
            }
        }
        mainMenu = Random.Range(0, 7);

        int peopleNum = Random.Range(0, 3); //0~2 사이 난수 생성, 손님 수 결정용

        if (peopleNum == 2)
        {
            tableObject[i1].GetComponent<TableObject>().SubOn = true;
            subChatOn = true; //서브메뉴 말풍선도 생김
            subMenu = Random.Range(0, 7); //0에서 6번 메뉴 중 랜덤
        }

        int rando1 = Random.Range(0, 2); //50퍼, 드링크 생성 여부
        if (rando1 == 0)
        {
            drinkChatOn = true;
            tableObject[i1].GetComponent<TableObject>().DrinkOn = true;
            drinkMenu = Random.Range(0, 3); //0에서 3번 메뉴 중 랜덤
        }

        tableObject[i1].GetComponent<TableObject>().MenuNumber(mainMenu, subMenu, drinkMenu);


        //GameObject projectile = null;
        projectile[i1] = Instantiate(npc, npcGenPo); //npc 생성
        projectile[i1].GetComponent<HumanoidController>().Appearance(); //npc 외형 결정하는 함수 호출
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableChair[tableId]);
        //앉을 테이블 오브젝트 할당       
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
        //projectile[i1].GetComponent<NpcAnimator>().enabled = true;
        if (peopleNum != 0) //2명 이상일때
        {
            projectileSub[i1] = Instantiate(npcSub, npcSubGenPo);
            projectileSub[i1].GetComponent<HumanoidControllerSub>().Appearance(); //npc 외형 결정하는 함수 호출
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableSubChair[tableId]);
            //앉을 테이블 오브젝트 할당 
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
        }
    }

    public virtual void HumanoidEat(int index)
    {
        projectile[index].GetComponent<HumanoidController>().Eat(); //eat 함수 호출
        if (projectileSub[index] != null) //만약 서브손님도 있다면
        {
            projectileSub[index].GetComponent<HumanoidControllerSub>().Eat();
        }
    }

    public void TableOnAgain(int index) //다 치웠을때 호출됨
    {
        tableOn[index] = false;

    }


    //손님 생성 함수 관련
    protected IEnumerator SpawnCustomers()
    {
        while (currentTime > despawnTime) //종료 30초 전부터 생성 금지
        {
            int falseCount = 0;
            for (int i = 0; i < tableOn.Length; i++)
            {
                if (!tableOn[i]) //tableOn 배열 폴스 몇개인지 검사
                {
                    falseCount++;
                }
            }

            // 현재 비어있는 테이블 수에 따라 손님 대기 시간 조절
            float timeBetweenCustomers = Mathf.Lerp(1f, baseTimeBetweenCustomers, (float)(8 - falseCount) / 8);
            
            // 사용 가능한 테이블이 있을 때만 손님 생성
            if (falseCount > 0)
            {
                yield return new WaitForSeconds(timeBetweenCustomers);

                if(currentTime >= 0) //만약 0초 이상 남았다면
                {
                    createOrder(falseCount); //손님 생성
                }
            }
            else
            {
                yield return null; // 사용 가능한 테이블이 없을 경우 대기
            }
        }

        while (currentTime <= despawnTime) // 0초부터 폴스카운트 검사
        {
            int falseCount = 0;

            // tableOn 배열에서 비어있는 테이블 수를 계산
            for (int i = 0; i < tableOn.Length; i++)
            {
                if (!tableOn[i])
                {
                    falseCount++;
                }
            }

            // falseCount가 0일 경우 함수 호출 후 코루틴 종료
            if (falseCount == 8 && currentTime <=0)
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

    public void ClockUpdate()
    {
        if(currentTime <= 0) //시간 끝나면
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
