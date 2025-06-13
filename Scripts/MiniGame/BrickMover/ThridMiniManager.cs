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
    protected TextMeshProUGUI Timebox; //�ð� ui
    [SerializeField]
    protected Image TimeImage; //�ð� �׷���


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
    [SerializeField] protected TextMeshProUGUI text; //������ �̸� �ؽ�Ʈ
    //[SerializeField] TextMeshProUGUI interactionText; //��ȣ�ۿ� �ؽ�Ʈ
    [SerializeField]
    protected LocalizeStringEvent nameString;
    //[SerializeField]
    //private LocalizeStringEvent interationString;

    [Header("Panel")]
    [SerializeField] protected GameObject warningPanel; //���� �����ųĴ� ����
    [SerializeField] protected GameObject resultPanel; //��� �г�

    public float gameTime; //���� �ð�
    public Status stat; //���� ������ �ִ� ���� ��ġ ����

    [SerializeField] protected GameObject man;
    [SerializeField] protected GameObject woman;
    [SerializeField] protected SimulationJobStat jobStat;

    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //���� ��ư

    [Header("Input Action")]
    [SerializeField]
    private PlayerInput m_Action;

    #region privateField
    protected float currentTime; //���� �ð�
    protected bool interactionButton;
    #endregion

    #region MoneyFunction
    public int Money
    {
        get
        {
            return money; // �Ӽ� ���� ��ȯ
        }
        set
        {
            money = value;

        }
    }
    /*public void AddMoney(int amount)
    {
        Money += amount; // �Ӽ��� set �����ڰ� ȣ��Ǹ鼭 �αװ� ��µ�
        moneyPlus.GetComponent<TextMeshProUGUI>().text = "+\u20A9" + amount.ToString();
        moneyText.text = "\u20A9" + money.ToString();
        moneyPlus.SetActive(true); //�� ������ ȿ�� Ű��
    }*/
    public int Tip
    {
        get
        {
            return tip; // �Ӽ� ���� ��ȯ
        }
        set
        {
            tip = value;
        }
    }
    public void AddTip(int amount)
    {
        Tip += amount; // �Ӽ��� set �����ڰ� ȣ��Ǹ鼭 �αװ� ��µ�
        tipPlus.GetComponent<TextMeshProUGUI>().text = "+\u20A9" + amount.ToString();
        tipText.text = "\u20A9" + tip.ToString();
        tipPlus.SetActive(true); //�� ������ ȿ�� Ű��    
    }
    #endregion


    protected virtual void Start()
    {
        helpPanel.SetActive(false);

        Money = jobStat.BaseSalary; //baseSalary ����
        moneyText.text = "\u20A9" + Money.ToString();
        LoadCharacter(); //ĳ���� ���� �ε�
        currentTime = gameTime;
        InvokeRepeating(nameof(ClockUpdate), 1.0f, 1.0f);

        //stat = StatusManager.instance.GetStatus().Clone();
    }
    protected virtual void Update()
    {
        if (helpButton)
        {
            helpButton = false;
            if (Time.timeScale != 0) //�ð� �ȸ�����������
            {
                helpPanel.SetActive(true);
            }
        }
    }

    public void ClockUpdate()
    {
        if (currentTime <= 0) //�ð� ������
        {
            EndGame(); //�̵� �Ұ�
            CancelInvoke(nameof(ClockUpdate));
            resultPanel.SetActive(true); // falseCount�� 0�� �� ��� ȭ�� �Լ� ȣ��
            resultPanel.GetComponent<ResultPanel>().Render();  //��� ȭ�� ����

        }
        else
        {
            currentTime--;
            int minutes = Mathf.FloorToInt(currentTime / 60); // ��
            int seconds = Mathf.FloorToInt(currentTime % 60); // ��
                                                              // ���� �� �ڸ�, �ʴ� �� �ڸ��� ǥ��
            Timebox.text = string.Format("{0}:{1:00}", minutes, seconds);
            TimeImage.fillAmount = (float)currentTime / gameTime;
        }
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

    public void LoadCharacter() //�ε��ؼ� �� �����Ҷ� ȣ��
    {
        GameObject prefab;
        if (PlayerStats.IsWoman)
        {
            //  cc.SwitchCharacterSettings(1); //0�̸� ���� 1�̸� ����
            prefab = Instantiate(woman, transform);

        }
        else
        {
            //  cc.SwitchCharacterSettings(0); //0�̸� ���� 1�̸� ����
            prefab = Instantiate(man, transform);
        }
        prefab.transform.localPosition = new Vector3(-0.021f, 0, 0.044f);
        //prefab.transform.localRotation = Quaternion.identity;


        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponent<AdvancedPeopleSystem.CharacterCustomization>();

        var saved = cc.GetSavedCharacterDatas();

        // �ش� �̸��� ���� �����Ͱ� �ִ��� Ȯ��
        for (int i = 0; i < saved.Count; i++)
        {
            //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
            if (saved[i].name == 5.ToString())
            {
                cc.ApplySavedCharacterData(saved[i]); // ���ϴ� �̸��� ĳ���� ������ ����
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // ã���� �� �̻� �ݺ����� �ʰ� ����
            }
        }
        /*player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Animator>().enabled = true;
        // Animator ���¸� �����Ͽ� �ٽ� ����� �� �ֵ��� ��*/
        animator.Rebind();
        animator.Update(0); // ������Ʈ�� ������ ȣ���� ��� �ݿ�
    }

    public void EndGame() //���� ���� �� ȣ���� �Լ�
    {
        if (m_Action != null)
        {
            m_Action.SwitchCurrentActionMap("UIOn");
            InputSystem.ResetHaptics(); // ��� �Է� ���¸� �ʱ�ȭ
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
