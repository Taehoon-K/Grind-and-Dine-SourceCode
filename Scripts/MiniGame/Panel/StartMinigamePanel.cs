using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UIManager;

public class StartMinigamePanel : UiPrompt
{
    /*
    [Header("Numeric")]
    [SerializeField]
    private int rangeMoney; //플마 범위
    [SerializeField]
    private int skillNum; //스킬 선택
    [SerializeField]
    private int hungerNumeric; //배고픔 감소 수치
    [SerializeField]
    private int fatigueNumeric; //피로도 감소 수치
    [SerializeField]
    private int stressNumeric; //스트레스 감정 증가 수치*/


    [Header("Scene")]
    [SerializeField]
    private SceneTransitionManager.Location locationToSwitch;

    //[Header("Simul")]
    //[SerializeField] private int whatMini; //무슨 미니게임인지, 
    //[SerializeField]private GameObject simulButton; //시뮬레이션 버튼, 시뮬 돈 0 이상일 시 활성화
    [SerializeField] private TextMeshProUGUI wageText;
    [SerializeField] private SimulationJobStat baseSalary;

    [SerializeField] private Simulscene simulscene; //시뮬레이션 시 띄울 시뮬씬

    [Header("IsOnlySimulation")]
    [SerializeField] private bool isOnlySimul; //시뮬만 있는 함수는 트루로 체크
    [SerializeField] private int simulMoney;  //시뮬시 받을 돈 텍스트로 띄울 값

    public void Confirm() //확인 버튼 누를시 호출될 함수
    {
        //씬 이동
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
    }

    public void Simulate() //시뮬 버튼 누를시 호출될 함수
    {
       // UIManager.instance.TriggerSimulPrompt(whatMini);
       CutsceneManager.Instance.StartSimulateScene(simulscene); //시뮬씬 실행

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //simulButton.SetActive(true);
        //IManager.instance.CurrentUIState = UIState.GeneralUI;
        /*if (StatusManager.instance.GetStatus().miniPrice[whatMini] > 0 || isOnlySimul) //만약 일해본적이 있다면, 아님 시뮬만 되는 작업이라면
        {
            

            if (isOnlySimul)
            {
                StatusManager.instance.GetStatus().miniPrice[whatMini] = simulMoney; //시뮬 시 받을 돈에 저장
                wageText.text = simulMoney + "\u20A9";
            }
            else
            {
                wage = StatusManager.instance.GetStatus().miniPrice[whatMini];
                wageText.text = (Mathf.Round(wage * 0.7f / 10) * 10).ToString("N0") + "\u20A9 ~ " + (Mathf.Round(wage * 0.9f / 10) * 10).ToString("N0") + "\u20A9";
            }
        }
        else
        {
            wageText.text = "? \u20A9";
        }*/
        wageText.text = baseSalary.BaseSalary +"\u20A9";
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //UIManager.instance.CurrentUIState = UIState.None;
    }
}
