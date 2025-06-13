using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ThridMiniManager : MonoBehaviour,HelpPanel
{
    [Header("Money&Time Canvas")]
    [SerializeField]
    protected TextMeshProUGUI Timebox; //시간 ui
    [SerializeField]
    protected Image TimeImage; //시간 그래프


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
    [SerializeField] protected Animator animator;
    [SerializeField] protected CrafterController controller;

    [Header("InteractionCanvas")]
    [SerializeField] protected TextMeshProUGUI text; //아이템 이름 텍스트
    //[SerializeField] TextMeshProUGUI interactionText; //상호작용 텍스트
    [SerializeField]
    protected LocalizeStringEvent nameString;
    //[SerializeField]
    //private LocalizeStringEvent interationString;

    [Header("Panel")]
    [SerializeField] protected GameObject warningPanel; //게임 나갈거냐는 문구
    [SerializeField] protected GameObject resultPanel; //결과 패널

    public float gameTime; //게임 시간
    public Status stat; //원래 가지고 있던 스탯 수치 저장

    [SerializeField] protected GameObject man;
    [SerializeField] protected GameObject woman;
    [SerializeField] protected SimulationJobStat jobStat;

    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //도움말 버튼

    [Header("Input Action")]
    [SerializeField]
    private PlayerInput m_Action;

    #region privateField
    protected float currentTime; //게임 시간
    protected bool interactionButton;
    #endregion

    #region MoneyFunction
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
    #endregion


    protected virtual void Start()
    {
        helpPanel.SetActive(false);

        Money = jobStat.BaseSalary; //baseSalary 설정
        moneyText.text = "\u20A9" + Money.ToString();
        LoadCharacter(); //캐릭터 외형 로드
        currentTime = gameTime;
        InvokeRepeating(nameof(ClockUpdate), 1.0f, 1.0f);

        //stat = StatusManager.instance.GetStatus().Clone();
    }
    protected virtual void Update()
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

    public void ClockUpdate()
    {
        if (currentTime <= 0) //시간 끝나면
        {
            EndGame(); //이동 불가
            CancelInvoke(nameof(ClockUpdate));
            resultPanel.SetActive(true); // falseCount가 0일 때 결과 화면 함수 호출
            resultPanel.GetComponent<ResultPanel>().Render();  //결과 화면 띄우기

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

    public void LoadCharacter() //로드해서 겜 시작할때 호출
    {
        GameObject prefab;
        if (PlayerStats.IsWoman)
        {
            //  cc.SwitchCharacterSettings(1); //0이면 남자 1이면 여자
            prefab = Instantiate(woman, transform);

        }
        else
        {
            //  cc.SwitchCharacterSettings(0); //0이면 남자 1이면 여자
            prefab = Instantiate(man, transform);
        }
        prefab.transform.localPosition = new Vector3(-0.021f, 0, 0.044f);
        //prefab.transform.localRotation = Quaternion.identity;


        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponent<AdvancedPeopleSystem.CharacterCustomization>();

        var saved = cc.GetSavedCharacterDatas();

        // 해당 이름을 가진 데이터가 있는지 확인
        for (int i = 0; i < saved.Count; i++)
        {
            //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
            if (saved[i].name == 5.ToString())
            {
                cc.ApplySavedCharacterData(saved[i]); // 원하는 이름의 캐릭터 데이터 적용
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // 찾으면 더 이상 반복하지 않고 종료
            }
        }
        /*player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Animator>().enabled = true;
        // Animator 상태를 리셋하여 다시 재생할 수 있도록 함*/
        animator.Rebind();
        animator.Update(0); // 업데이트를 강제로 호출해 즉시 반영
    }

    public void EndGame() //게임 끝날 시 호출할 함수
    {
        if (m_Action != null)
        {
            m_Action.SwitchCurrentActionMap("UIOn");
            InputSystem.ResetHaptics(); // 모든 입력 상태를 초기화
        }
    }

    #region ButtonInput
    public void OnIntercation(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                interactionButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                interactionButton = false;
                break;
        }
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

    #region HelpPanel
    public void HelpOn()
    {
        helpPanel.SetActive(true);
    }
    public void HelpOff()
    {
        helpPanel.SetActive(false);
    }
    #endregion
}
