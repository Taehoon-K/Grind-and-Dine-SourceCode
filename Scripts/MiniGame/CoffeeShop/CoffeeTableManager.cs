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

    // TableState ��ü�� �����ϴ� ����Ʈ �ʱ�ȭ
    private List<TableState> coffeeMenu = new List<TableState>
    {                              //����� ������ ����   ����  ����   �ʽ�   ī��    ũ��   ����
        new TableState(new bool[8] { true, false, false, true, false, false, false, false}, false), //�Ƹ޸�ī��
        new TableState(new bool[8] { false, false, true, true, true, false, false, true}, true),
        new TableState(new bool[8] { false, false, true, true, false, true, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, true, true}, true), //ī��� ����
        new TableState(new bool[8] { false, false, true, true, true, false, false, false}, false),
        new TableState(new bool[8] { false, false, true, true, false, true, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, true, false}, false),  //ī��� ����
    };
    [SerializeField] protected SimulationJobStat jobStat;

    [SerializeField]
    protected TextMeshProUGUI Timebox; //�ð� ui
    [SerializeField]
    protected Image TimeImage; //�ð� �׷���
    [SerializeField]
    protected LocalizeStringEvent timeString; //�ð� �ؽ�Ʈ�� ���� ����
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
    protected int money, tip;

    [SerializeField]
    protected GameObject notice;

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

    [SerializeField]
    protected GameObject npc,npcFemale; //������ npc
    [SerializeField]
    protected Transform npcGenPo; //npc ���� ��ġ
    [SerializeField]
    protected Transform npcExitPo; //npc ������ ��ġ
    public bool subChatOn = false; //�ι�° �޴� ��ų�� ����

    protected int difficult; //���̵� �ӽ� ����, ���߿� ���̵� �Ŵ������� �޾ƿð�

    public float gameTime; //���� �ð�
    protected float currentTime; //���� �ð�
    [SerializeField]
    protected float baseTimeBetweenCustomers;  // �⺻ �մ� ��� �ð� (��)

    public static CoffeeTableManager instance = null;
    public Status stat; //���� ������ �ִ� ���� ��ġ ����

    public int mainMenu, subMenu; //�� �޴� ����

    [SerializeField]
    protected Transform[] points; // NPC ��� ��ġ�� ����� �迭
    [SerializeField]
    protected int maxNPCs = 8; // �ִ� NPC ��
    protected List<GameObject> npcList = new List<GameObject>();
    protected int currentNPCIndex = 0; // ���� NPC�� �� ��ġ �ε���

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
    private void Update()
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

    protected virtual void CreateOrder() //�մ� ���� �Լ�
    {

        subChatOn = false; //�ʱ�ȭ
        mainMenu = Random.Range(0, 9);
        int peopleNum = Random.Range(0, 2); //0~1 ���� ���� ����, ����޴� ������

        if (peopleNum == 0)
        {
            // tableObject[i1].GetComponent<TableObject>().SubOn = true;
            subChatOn = true; //����޴� ��ǳ���� ����
            subMenu = Random.Range(0, 5); //0���� 4�� �޴� �� ����
        }

        GameObject npc1;
        int rando1 = Random.Range(0, 2); //50��, ���� ����
        if (rando1 == 0)
        {
            npc1 = Instantiate(npc, npcGenPo); //npc ����
        }
        else
        {
            npc1 = Instantiate(npcFemale, npcGenPo);
        }
        npc1.GetComponent<HumanoidControllerCafe>().Appearance(); //npc ���� �����ϴ� �Լ� ȣ��
                                                              // NPC�� npcExit�� �̵��ϰ� ��
        NavMeshAgent agent = npc1.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(points[currentNPCIndex].position); // NavMesh�� ����� ���ϴ� ��ġ�� �̵�
            npc1.GetComponent<HumanoidControllerCafe>().isDestination = true;
        }
        /*int a = Random.Range(0, 3);
        npc1.GetComponent<Animator>().SetFloat("IdleState", a);*/ //��� ��� ���� ���

        npcList.Add(npc1); // NPC�� ����Ʈ�� �߰�
        if(npcList[0] == npc1)
        {
            npc1.GetComponent<HumanoidControllerCafe>().isFirst = true;
        }
        currentNPCIndex++; // ���� NPC�� �� ��ġ�� ����Ŵ

        
    }

    //�Ǿ� npc ��ǳ�� ���� �ֹ��ޱ�
    protected void OrderUp()
    {
        // frontNPC.GetComponent<HumanoidControllerCafe>().Hello(); //��ǳ������
        if(npcList.Count > 0)
        {
            npcList[0].GetComponent<HumanoidControllerCafe>().isFirst = true;
        }

    }

    public void Angry() //�߸��� ���� �� �� ȣ��
    {
        GameObject frontNPC = npcList[0];
        frontNPC.GetComponent<HumanoidControllerCafe>().Angry();
        RemoveFrontNPC();
    }
    public void EatGood() //�ֹ� ����� ����
    {
        GameObject frontNPC = npcList[0];
        frontNPC.GetComponent<HumanoidControllerCafe>().Good();
        RemoveFrontNPC();
    }

    public bool CheckCoffee(bool[] ingre, bool mix, bool sideOn, int sideId)
    {
        if (npcList[0] == null) //���� �ƹ��� ���ٸ�
        {
            return false;
        }
        bool subOn = npcList[0].GetComponent<HumanoidControllerCafe>().subOn;
        int sMenu = npcList[0].GetComponent<HumanoidControllerCafe>().sMenu;
        int mMenu = npcList[0].GetComponent<HumanoidControllerCafe>().mMenu;
        GameObject frontNPC = npcList[0];
        if(npcList[0].GetComponent<HumanoidControllerCafe>().isFirst) //���� ���� ù��� ���� ��������
        {
            return false;
        }

        if (subOn) //���̵� �ֹ� ����
        {
            if (!sideOn || sideId != sMenu) //���� ���̵� ���ٸ�, ���̵�޴� ����ġ ��
            {
                Angry();
                return false;
            }
        }
        else
        {
            if (sideOn)  //���� �ֹ����µ� ���� �ö����ִٸ�
            {
                //�� ��� ���Ͻ�Ű��
            }
        }
        if (mix != coffeeMenu[mMenu].isCook) //���� �ͽ� ���� �ٸ���
        {
            Angry();
            return false;
        }
        // ingre �迭�� �� ��Ҹ� ���� ���¿� ��
        for (int i = 0; i < ingre.Length; i++)
        {
            if (ingre[i] != coffeeMenu[mMenu].ingre[i]) //��� ����ġ ��
            {
                Angry();
                return false;
            }
        }
        EatGood();
        return true;
    }

    // NPC ���� (�� �� NPC�� npcExit ��ġ�� �̵���Ų ��, �� NPC�� �̵�)
    private void RemoveFrontNPC()
    {
        if (npcList.Count > 0)
        {
            GameObject frontNPC = npcList[0]; // �� �� NPC

            // NPC�� npcExit�� �̵��ϰ� ��
            NavMeshAgent agent = frontNPC.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(npcExitPo.position); // NavMesh�� ����� npcExit���� �̵�
            }

            // ���� �ð� �� ����Ʈ���� ���� (NPC�� ������ �����ٰ� ����)
            StartCoroutine(RemoveNPCAfterExit(frontNPC));

            npcList.RemoveAt(0); // ����Ʈ���� ����

            // ������ NPC�� �̵� (NavMesh�� ����� ����Ʈ�� �̵�)
            for (int i = 0; i < npcList.Count; i++)
            {
                GameObject npc = npcList[i];
                NavMeshAgent npcAgent = npc.GetComponent<NavMeshAgent>();
                if (npcAgent != null)
                {
                    npcAgent.SetDestination(points[i].position); // ���� ��ġ�� �̵�
                    npc.GetComponent<HumanoidControllerCafe>().isDestination = true;
                }
            }

            currentNPCIndex--; // ���� ������ NPC�� �ε����� ����
            OrderUp(); //�����մ� ȣ��
        }
    }

    //�մ� ���� �Լ� ����
    protected IEnumerator SpawnCustomers()
    {
        while (currentTime > 60) //���� 60�� ������ ���� ����
        {

            // ���� ����ִ� ���̺� ���� ���� �մ� ��� �ð� ����
            float timeBetweenCustomers = Mathf.Lerp(1f, baseTimeBetweenCustomers, (float) currentNPCIndex / maxNPCs);

            // ��� ������ ���̺��� ���� ���� �մ� ����
            if (currentNPCIndex < maxNPCs)
            {
                yield return new WaitForSeconds(timeBetweenCustomers);

                if (currentTime >= 0) //���� 0�� �̻� ���Ҵٸ�
                {
                    CreateOrder(); //�մ� ����
                }
            }
            else
            {
                yield return null; // ��� ������ ���̺��� ���� ��� ���
            }
        }

        while (currentTime <= 60) // 0�ʺ��� ����ī��Ʈ �˻�
        {

            // npc�� 0�� ��� �Լ� ȣ�� �� �ڷ�ƾ ����
            if (currentNPCIndex == 0 && currentTime <= 0)
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

    // NPC�� npcExit�� ������ �̵��� �� ����Ʈ���� ����
    IEnumerator RemoveNPCAfterExit(GameObject npc)
    {
        yield return new WaitForSeconds(10f); // NPC�� ���� �ð� ��� (�ð��� ��Ȳ�� �°� ����)
        Destroy(npc); // NPC ������Ʈ ����
    }



    public void ClockUpdate()
    {
        if (currentTime <= 0) //�ð� ������
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
