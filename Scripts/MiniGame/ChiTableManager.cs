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
    protected TextMeshProUGUI Timebox; //�ð� ui
    [SerializeField]
    protected Image TimeImage; //�ð� �׷���
    [SerializeField]
    protected LocalizeStringEvent timeString; //�ð� �ؽ�Ʈ�� ���� ����
   // protected bool timeOver; //�ð� �������� ȣ��
    [SerializeField]
    protected GameObject resultPanel; //�ð� �ؽ�Ʈ�� ���� ����

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
    protected bool[] orderUp =   new bool[8]; //Ʈ�� �Ǹ� ���� ���� �� �ְ�

    [SerializeField]
    protected GameObject notice; //�˸� ���� ����

    [SerializeField] protected int despawnTime; //�մ� ���� ���ϴ� �ð� �� ����
    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //���� ��ư

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
    public bool GetOrderUp(int index)
    {
        return orderUp[index];
    }
    public void SetOrderUp(int index, bool value)
    {
        orderUp[index] = value;
        tableObject[index].GetComponent<BoxCollider>().enabled = value; //���̺� �ݶ��̴� Ȱ,��Ȱ��ȭ
        tableObject[index].GetComponent<TableObject>().myindex = index; //���̺� �ڱ� ��ȣ ����
    }
    public void SetTableOn(int index) //���������� ���� ����������
    {
        //tableOn[index] = false;
        tableObject[index].GetComponent<BoxCollider>().enabled = true;
        tableObject[index].GetComponent<TableObject>().EatFinish();

    }
    public void AngryOn(int index) //ȭ������
    {       
        tableObject[index].GetComponent<TableObject>().EatFinish(true);

        //tableObject[index].GetComponent<TableObject>().Reset1();
        tableObject[index].GetComponent<TableObject>().TableAngry();

    }
    public GameObject[] tableChair; //���� ������Ʈ
    public GameObject[] tableSubChair; //���� ���� ������Ʈ
    [SerializeField]
    protected GameObject[] tableObject; //���̺� ������Ʈ
    [SerializeField]
    protected GameObject npc; //������ npc
    [SerializeField]
    protected GameObject npcSub; //������ ���� npc
    [SerializeField]
    protected Transform npcGenPo; //npc ���� ��ġ
    [SerializeField]
    protected Transform npcSubGenPo; //npc ���� ���� ��ġ
    public int tableId;
    public bool subChatOn = false; //�ι�° �޴� ��ų�� ����
    public bool drinkChatOn = false; //����� �޴� ��ų�� ����
    public int mainMenu,subMenu,drinkMenu; //�� �޴� ����
    protected int i1;

    protected GameObject[] projectile = new GameObject[8];
    protected GameObject[] projectileSub = new GameObject[8];

    protected int difficult; //���̵� �ӽ� ����, ���߿� ���̵� �Ŵ������� �޾ƿð�


    public float gameTime; //���� �ð�
    protected float currentTime; //���� �ð�
    [SerializeField]
    protected float baseTimeBetweenCustomers;  // �⺻ �մ� ��� �ð� (��)

    public static ChiTableManager instance = null;
    public Status stat; //���� ������ �ִ� ���� ��ġ ����

    protected void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }
    protected void Start()
    {
        helpPanel.SetActive(false);

        Money = jobStat.BaseSalary; //baseSalary ����
        moneyText.text = "\u20A9" + Money.ToString();

        TimeManager.instance.TimeTicking = false; //�ð� �帣�°� ���߰��ϱ�
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
            if (Time.timeScale != 0) //�ð� �ȸ�����������
            {
                helpPanel.SetActive(true);
            }
        }
    }

    protected virtual void createOrder(int count) //�մ� ���� �Լ�
    {
        subChatOn = false;
        drinkChatOn = false; //�ʱ�ȭ

        int falseCount = count;

        // ���� ���� �� ����
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

        int peopleNum = Random.Range(0, 3); //0~2 ���� ���� ����, �մ� �� ������

        if (peopleNum == 2)
        {
            tableObject[i1].GetComponent<TableObject>().SubOn = true;
            subChatOn = true; //����޴� ��ǳ���� ����
            subMenu = Random.Range(0, 7); //0���� 6�� �޴� �� ����
        }

        int rando1 = Random.Range(0, 2); //50��, �帵ũ ���� ����
        if (rando1 == 0)
        {
            drinkChatOn = true;
            tableObject[i1].GetComponent<TableObject>().DrinkOn = true;
            drinkMenu = Random.Range(0, 3); //0���� 3�� �޴� �� ����
        }

        tableObject[i1].GetComponent<TableObject>().MenuNumber(mainMenu, subMenu, drinkMenu);


        //GameObject projectile = null;
        projectile[i1] = Instantiate(npc, npcGenPo); //npc ����
        projectile[i1].GetComponent<HumanoidController>().Appearance(); //npc ���� �����ϴ� �Լ� ȣ��
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableChair[tableId]);
        //���� ���̺� ������Ʈ �Ҵ�       
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
        //projectile[i1].GetComponent<NpcAnimator>().enabled = true;
        if (peopleNum != 0) //2�� �̻��϶�
        {
            projectileSub[i1] = Instantiate(npcSub, npcSubGenPo);
            projectileSub[i1].GetComponent<HumanoidControllerSub>().Appearance(); //npc ���� �����ϴ� �Լ� ȣ��
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableSubChair[tableId]);
            //���� ���̺� ������Ʈ �Ҵ� 
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
        }
    }

    public virtual void HumanoidEat(int index)
    {
        projectile[index].GetComponent<HumanoidController>().Eat(); //eat �Լ� ȣ��
        if (projectileSub[index] != null) //���� ����մԵ� �ִٸ�
        {
            projectileSub[index].GetComponent<HumanoidControllerSub>().Eat();
        }
    }

    public void TableOnAgain(int index) //�� ġ������ ȣ���
    {
        tableOn[index] = false;

    }


    //�մ� ���� �Լ� ����
    protected IEnumerator SpawnCustomers()
    {
        while (currentTime > despawnTime) //���� 30�� ������ ���� ����
        {
            int falseCount = 0;
            for (int i = 0; i < tableOn.Length; i++)
            {
                if (!tableOn[i]) //tableOn �迭 ���� ����� �˻�
                {
                    falseCount++;
                }
            }

            // ���� ����ִ� ���̺� ���� ���� �մ� ��� �ð� ����
            float timeBetweenCustomers = Mathf.Lerp(1f, baseTimeBetweenCustomers, (float)(8 - falseCount) / 8);
            
            // ��� ������ ���̺��� ���� ���� �մ� ����
            if (falseCount > 0)
            {
                yield return new WaitForSeconds(timeBetweenCustomers);

                if(currentTime >= 0) //���� 0�� �̻� ���Ҵٸ�
                {
                    createOrder(falseCount); //�մ� ����
                }
            }
            else
            {
                yield return null; // ��� ������ ���̺��� ���� ��� ���
            }
        }

        while (currentTime <= despawnTime) // 0�ʺ��� ����ī��Ʈ �˻�
        {
            int falseCount = 0;

            // tableOn �迭���� ����ִ� ���̺� ���� ���
            for (int i = 0; i < tableOn.Length; i++)
            {
                if (!tableOn[i])
                {
                    falseCount++;
                }
            }

            // falseCount�� 0�� ��� �Լ� ȣ�� �� �ڷ�ƾ ����
            if (falseCount == 8 && currentTime <=0)
            {
                resultPanel.SetActive(true); // falseCount�� 0�� �� ��� ȭ�� �Լ� ȣ��
                resultPanel.GetComponent<ResultPanel>().Render();
                yield break; // �ڷ�ƾ ����
            }

            // �� �����Ӹ��� falseCount�� ��� �˻�
            yield return null;
        }

        yield return null;
    }

    public void ClockUpdate()
    {
        if(currentTime <= 0) //�ð� ������
        {
            timeString.StringReference.TableEntryReference = "extratime_key";  //����Ÿ�� �ؽ�Ʈ ����
            //��� ȭ�� ����

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
