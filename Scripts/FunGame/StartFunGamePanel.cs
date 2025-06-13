using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UIManager;

public class StartFunGamePanel : UiPrompt
{
    [Header("Scene")]
    [SerializeField]
    private SceneTransitionManager.Location locationToSwitch;

    /*[Header("Others")]
    [SerializeField] private Button leftButton;  // ���� ��ư
    [SerializeField] private Button rightButton; // ������ ��ư
    [SerializeField]
    private TextMeshProUGUI wageText;   // �� ���� ǥ���� Text UI*/

    [SerializeField] private Simulscene simulscene; //�ùķ��̼� �� ��� �ùľ�
    [SerializeField] private GameObject simulButton; //�ùķ��̼� ��ư, ���� �� 0�϶��� Ȱ��ȭ

    private readonly int[] moneyValues = { 0, 5000, 10000, 20000, 30000, 40000, 50000, 100000 }; // ������ �� ��
    private int currentIndex = 0; // ���� ���õ� �� ���� �ε���
    

    public void Confirm() //Ȯ�� ��ư ������ ȣ��� �Լ�
    {
      /*  int selectedMoney = moneyValues[currentIndex];
        PlayerPrefs.SetInt("SelectedMoney", selectedMoney); //�� �� ����*/
        //�� �̵�
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
    }
    public void Simulate() //�ù� ��ư ������ ȣ��� �Լ�
    {
        CutsceneManager.Instance.StartSimulateScene(simulscene); //�ùľ� ����

    }
    private void Start()
    {
        // �ʱ� �� ����
        //UpdateMoneyText();

       /* // ��ư�� �̺�Ʈ �߰�
        leftButton.onClick.AddListener(DecreaseMoney);
        rightButton.onClick.AddListener(IncreaseMoney);*/
    }
    protected override void OnEnable()
    {
        base.OnEnable();
       /* currentIndex = 0;
        UpdateMoneyText();*/
    }

    /*private void UpdateMoneyText()
    {
        // �� ���� ������Ʈ
        wageText.text = moneyValues[currentIndex].ToString("N0") + "\u20A9"; // "1,000��" �������� ǥ��
        
        if (currentIndex == 0) //�� �Ȱɾ�����
        {
            simulButton.SetActive(true);
        }
        else
        {
            simulButton.SetActive(false);
        }
    }
    
    private void DecreaseMoney()
    {
        // ���� ��ư Ŭ�� �� �� �� ����
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateMoneyText();
        }
    }

    private void IncreaseMoney()
    {
        // ������ ��ư Ŭ�� �� �� �� ����
        if (currentIndex < moneyValues.Length - 1)
        {
            if (moneyValues[currentIndex + 1] <= PlayerStats.GetMoney()) //���� ������ ���� ���� ���� ����
            {
                currentIndex++;
                UpdateMoneyText();
            }
        }
    }*/
}
