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
    private int rangeMoney; //�ø� ����
    [SerializeField]
    private int skillNum; //��ų ����
    [SerializeField]
    private int hungerNumeric; //����� ���� ��ġ
    [SerializeField]
    private int fatigueNumeric; //�Ƿε� ���� ��ġ
    [SerializeField]
    private int stressNumeric; //��Ʈ���� ���� ���� ��ġ*/


    [Header("Scene")]
    [SerializeField]
    private SceneTransitionManager.Location locationToSwitch;

    //[Header("Simul")]
    //[SerializeField] private int whatMini; //���� �̴ϰ�������, 
    //[SerializeField]private GameObject simulButton; //�ùķ��̼� ��ư, �ù� �� 0 �̻��� �� Ȱ��ȭ
    [SerializeField] private TextMeshProUGUI wageText;
    [SerializeField] private SimulationJobStat baseSalary;

    [SerializeField] private Simulscene simulscene; //�ùķ��̼� �� ��� �ùľ�

    [Header("IsOnlySimulation")]
    [SerializeField] private bool isOnlySimul; //�ùĸ� �ִ� �Լ��� Ʈ��� üũ
    [SerializeField] private int simulMoney;  //�ùĽ� ���� �� �ؽ�Ʈ�� ��� ��

    public void Confirm() //Ȯ�� ��ư ������ ȣ��� �Լ�
    {
        //�� �̵�
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
    }

    public void Simulate() //�ù� ��ư ������ ȣ��� �Լ�
    {
       // UIManager.instance.TriggerSimulPrompt(whatMini);
       CutsceneManager.Instance.StartSimulateScene(simulscene); //�ùľ� ����

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //simulButton.SetActive(true);
        //IManager.instance.CurrentUIState = UIState.GeneralUI;
        /*if (StatusManager.instance.GetStatus().miniPrice[whatMini] > 0 || isOnlySimul) //���� ���غ����� �ִٸ�, �ƴ� �ùĸ� �Ǵ� �۾��̶��
        {
            

            if (isOnlySimul)
            {
                StatusManager.instance.GetStatus().miniPrice[whatMini] = simulMoney; //�ù� �� ���� ���� ����
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
