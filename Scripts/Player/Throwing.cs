using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP.InventorySystem;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public Transform trashPoint;
    public GameObject[] objectToThrow;

    [Header("Settings")]
    //public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    //public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;
    bool readyToEat;
    bool readyToDrink;
    bool readyToUse;
    bool readyToConsum; //�Ҹ�ǰ ������
    bool throwButton, putButton, rightButton, leftButton;
    //public int throwNumber { get; set; }
    private Animator animator;
    private Animator animatorFirst;
    private Status stat;
    private MoodleImform[] moodleImform;

    public ItemData _data { get; private set; } //�ٸ� ������ �б⸸ ����
    public CountableItem _itemData { get; private set; } //�ٸ� ������ �б⸸ ����

    [SerializeField]
    private float throwInvoke;    
    [SerializeField]
    private float drinkInvoke;
    [SerializeField]
    private float eatInvoke;
    [SerializeField]
    private float consumInvoke;
    [HideInInspector]
    public int equipID;
    [SerializeField]
    private List<GameObject> objectToEquipF = new List<GameObject>();
    [SerializeField]
    private List<GameObject> objectToEquipT = new List<GameObject>();

    public UnityEvent onInventory;
    public UnityEvent offInventory;

    [Header("EatObject")]
    [SerializeField]
    private List<GameObject> eat1 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> eat2 = new List<GameObject>();

    [Header("DrinkObject")]
    [SerializeField]
    private List<GameObject> drink1 = new List<GameObject>();
    [Header("ConsumObject")]
    [SerializeField]
    private List<GameObject> consum1 = new List<GameObject>();
    /*[SerializeField]
    private List<GameObject> objectToDrinkTrash = new List<GameObject>(); //�ٸ԰� ������ ������ ������*/

    [Header("UI")]
    [SerializeField]
    private GameObject eatPrompt,drinkPrompt,trashPrompt, consumPrompt;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private CanvasOnOff watchOn;

    [Header("smoke")]
    [SerializeField]
    private GameObject smoking; //��迬��

    private int id_,skillIndex,skillAmount;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animatorFirst = GameObject.Find("FirstPArms").GetComponent<Animator>();
    }

    private void Start()
    {
        readyToThrow = false;
        readyToEat = false;
        readyToUse = false;
        readyToDrink = false;

        Invoke(nameof(InventorySetup), 1f);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() ||(inventoryUI != null && inventoryUI.IsDragging) || watchOn.IsPlayingAni())
            return; // ���콺�� UI ���� ������ �Է� ����

        /*if (throwButton&& readyToThrow)
        {
            throwButton = false;
            ThrowReady();
        }else*/ if (leftButton && readyToDrink)
        {
            leftButton = false;
            DrinkReady();
        }
        else if (leftButton && readyToEat)
        {
            leftButton = false;
            EatReady();
        }
        else if (leftButton && readyToConsum)
        {
            leftButton = false;
            ConsumReady();
        }

        if (putButton)
        {
            putButton = false;
            PutIn();
        }

        throwButton = false;
        putButton = false;
        rightButton = false;
        leftButton = false;
    }
    private void ThrowReady()
    {
        ItemUsed();
        animator.SetTrigger("isThrow");
        animatorFirst.SetTrigger("isThrow");
        Invoke(nameof(Throw), throwInvoke);
    }

    private void Throw()
    {
        //objectToEquipF[id_].SetActive(false); //1��Ī
        //objectToEquipT[id_].SetActive(false); //3��Ī
        GetItemById(objectToEquipF, id_).SetActive(false);
        GetItemById(objectToEquipT, id_).SetActive(false);
        readyToThrow = false; //������ ����Ǵ� ���� ������ �Լ� ȣ�� ����

        // ������ ������Ʈ ����
        GameObject projectile = Instantiate(objectToThrow[id_], attackPoint.position, cam.rotation);

        // �߷� �߰�
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // ���� ���
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        } //�߾����� Raycast ���� hit �Ǵ� ������Ʈ ������ ���� ����

        // force �߰�
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

    }

    private void DrinkReady()
    {
        ItemUsed();
        drinkPrompt.SetActive(false);
        UIManager.instance.OnEatWaiting(true); //ui���� ����
       // objectToEquipF[id_].SetActive(false); //����ִ��� ����
       // objectToEquipT[id_].SetActive(false); //����ִ��� ����
        GetItemById(objectToEquipF, id_).SetActive(false);
        GetItemById(objectToEquipT, id_).SetActive(false);
        //drink1[id_].SetActive(true);  //ù��° ���� Ű��
        GetItemById(drink1, id_).SetActive(true);

        animatorFirst.SetTrigger("isDrink");
        Invoke(nameof(Drink), drinkInvoke);
        readyToDrink = false;

        onInventory.Invoke(); //1��Ī ���� ����

        SoundManager.instance.PlaySound2D("Drinking");
    }

    private void Drink()
    {
        UIManager.instance.OnEatWaiting(false); //ui���� ����
        //drink1[id_].SetActive(false); //����ִ��� ����
        GetItemById(drink1, id_).SetActive(false);

        StatusManager.instance.AddStat(stat);
        StatusManager.instance.AddMoodle(moodleImform);

       // GameObject projectile = Instantiate(objectToThrow[id_], trashPoint.position, cam.rotation); //������ ������

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1��Ī ���� ���� Ǯ��
    }

    private void EatReady()
    {
        ItemUsed();
        eatPrompt.SetActive(false);
        UIManager.instance.OnEatWaiting(true); //ui���� ����

        GetItemById(objectToEquipF, id_).SetActive(false);
        GetItemById(objectToEquipT, id_).SetActive(false);
        GetItemById(eat1, id_).SetActive(true);

        animatorFirst.SetTrigger("isEat");
        Invoke(nameof(EatOneBite), eatInvoke / 2);
        Invoke(nameof(Eat), eatInvoke);
        readyToEat = false;

        onInventory.Invoke(); //1��Ī ���� ����.

        if (id_ == 1) //���� ����
        {
            smoking.SetActive(true);
            SoundManager.instance.PlaySound2D("smoking");
        }
        else
        {
            SoundManager.instance.PlaySound2D("Eating");
        }
    }
    private void EatOneBite()
    {
        //eat1[id_].SetActive(false); //����ִ��� ����
        //eat2[id_].SetActive(true);  //�ι�° ���� Ű��
        GetItemById(eat1, id_).SetActive(false);
        GetItemById(eat2, id_).SetActive(true);

        if(id_  == 1) //���� ����
        {
            smoking.SetActive(false);
            smoking.SetActive(true);
        }
    }
    private void Eat()
    {
        UIManager.instance.OnEatWaiting(false); //ui���� ����
       // eat2[id_].SetActive(false);
        GetItemById(eat2, id_).SetActive(false);

        StatusManager.instance.AddStat(stat);
        StatusManager.instance.AddMoodle(moodleImform);
        //GameObject projectile = Instantiate(objectToThrow[id_], trashPoint.position, cam.rotation); //������ ������
        if (TutorialManager.instance != null)  //���� Ʃ�丮�����̶��
        {
            TutorialManager.instance.eatBread = true;
        }
        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1��Ī ���� ���� Ǯ��

        smoking.SetActive(false);

        QuestManager.instance.CompleteObjective(0, 2); //����Ʈ ��
    }
    private void ConsumReady()
    {
        ItemUsed();
        consumPrompt.SetActive(false);
        UIManager.instance.OnEatWaiting(true); 
        GetItemById(objectToEquipF, id_).SetActive(false);
        GetItemById(objectToEquipT, id_).SetActive(false);
        GetItemById(consum1, id_).SetActive(true);

        animatorFirst.SetTrigger("isDrink");
        Invoke(nameof(Consum), consumInvoke);
        readyToConsum = false;

        onInventory.Invoke(); //1��Ī ���� ����

        //SoundManager.instance.PlaySound2D("Drinking");
    }

    private void Consum()
    {
        UIManager.instance.OnEatWaiting(false); //ui���� ����
        //drink1[id_].SetActive(false); //����ִ��� ����
        GetItemById(consum1, id_).SetActive(false); //�Ҹ������ ��Ͽ��� ã��

        StatusManager.instance.AddExperience(skillIndex, skillAmount); //����ġ ����

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1��Ī ���� ���� Ǯ��
    }


    #region EatFoodNotInven
    public void EatReadyCutscene(int foodId) //�κ����� ���� �Դ°� �ƴϰ� �׳� ������
    {
        PutIn(); //���� ��� ������ ����ְ�

        UIManager.instance.OnEatWaiting(true); //ui���� ����

        GetItemById(eat1, foodId).SetActive(true);

        animatorFirst.SetTrigger("isEat");
        StartCoroutine(DelayedEat(eatInvoke / 2, foodId));
        StartCoroutine(DelayedEat2(eatInvoke, foodId));

        SoundManager.instance.PlaySound2D("Eating");
        onInventory.Invoke(); //1��Ī ���� ����
    }
    private IEnumerator DelayedEat(float delay, int foodId)
    {
        yield return new WaitForSeconds(delay);
        EatOneBiteCutscene(foodId);
    }

    private void EatOneBiteCutscene(int id)
    {
        GetItemById(eat1, id).SetActive(false);
        GetItemById(eat2, id).SetActive(true);
    }
    private IEnumerator DelayedEat2(float delay, int foodId)
    {
        yield return new WaitForSeconds(delay);
        EatCutscene(foodId);
    }
    private void EatCutscene(int id)
    {
        GetItemById(eat2, id).SetActive(false);

        StatusManager.instance.EatItemCalcul(id - 100); //���� ���� ����

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1��Ī ���� ���� Ǯ��

        UIManager.instance.OnEatWaiting(false); //ui���� ����

        QuestManager.instance.CompleteObjective(0, 2); //����Ʈ ��
    }

    public void DrinkReadyCutscene(int foodId)
    {
        PutIn(); //���� ��� ������ ����ְ�

        UIManager.instance.OnEatWaiting(true); //ui���� ����

        GetItemById(drink1, foodId).SetActive(true);

        animatorFirst.SetTrigger("isDrink");
        StartCoroutine(DelayedDrink(drinkInvoke, foodId));

        SoundManager.instance.PlaySound2D("Drinking");
        onInventory.Invoke(); //1��Ī ���� ����
    }
    private IEnumerator DelayedDrink(float delay, int foodId)
    {
        yield return new WaitForSeconds(delay);
        DrinkCutscene(foodId);
    }
    private void DrinkCutscene(int id)
    {
        GetItemById(drink1, id).SetActive(false);

        StatusManager.instance.EatItemCalcul(id - 100); //���� ���� ����

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1��Ī ���� ���� Ǯ��

        UIManager.instance.OnEatWaiting(false); //ui���� ����
    }

    #endregion


    public bool Equips(int id,int code, Status _value=null, MoodleImform[] _moodleValue = null) //�κ��丮���� Ŭ��������
    {
        if (readyToDrink || readyToEat || readyToThrow || readyToUse || readyToConsum)
        {
            return false;
        }
        else
        {
            animator.SetTrigger("isHold");
            animatorFirst.SetTrigger("isHold");
            id_ = id;
            //objectToEquipF[id_].SetActive(true);
            GetItemById(objectToEquipF, id_).SetActive(true);
           // objectToEquipT[id_].SetActive(true);
            GetItemById(objectToEquipT, id_).SetActive(true);
            switch (code)
            {
                case 0: //���� ���� �ϰ͵� ���ϴ°�,(������ ���)
                    readyToThrow = true;
                    trashPrompt.SetActive(true);
                    break;
                case 1: //�Դ°�
                    readyToEat = true;
                    eatPrompt.SetActive(true);
                    stat = _value; //���� �迭 �޾ƿ���
                    moodleImform = _moodleValue; //���� ���� �޾ƿ���
                    break;
                case 2: //���ô°�
                    readyToDrink = true;
                    drinkPrompt.SetActive(true);
                    stat = _value;
                    moodleImform = _moodleValue; //���� ���� �޾ƿ���
                    break;
                case 3: //��빫��
                    readyToUse = true;
                    break;
                default:
                    break;
            }
            switch (id)//������ ��� �� ��쿡 �°� ���� �ٲٱ�
            {
                case 1: //���˴� �������
                    gameObject.GetComponent<CameraRay>().fishingOn = true;
                    break;
                default:
                    break;

            }
        
            return true;
        } 
    }
    public bool Equips(int id, int code, int sIndex, int sAmount) //��ų�Ͽ� ��Ţ, ���߿� �Ҹ�ǰ��뵵�� �� Ȯ���Ұ�
    {
        if (readyToDrink || readyToEat || readyToThrow || readyToUse || readyToConsum)
        {
            return false;
        }
        else
        {
            animator.SetTrigger("isHold");
            animatorFirst.SetTrigger("isHold");
            id_ = id;
            skillIndex = sIndex;
            skillAmount = sAmount;
            GetItemById(objectToEquipF, id_).SetActive(true);
            GetItemById(objectToEquipT, id_).SetActive(true);
            switch (code)
            {
                case 0: //å
                    readyToConsum = true;
                    consumPrompt.SetActive(true);
                    break;
                default:
                    break;
            }
            return true;
        }
    }

    private void PutIn() //������ �ٽ� ����ֱ�
    {
        if (readyToDrink || readyToEat || readyToThrow || readyToConsum) //���� �������
        {

                animator.SetTrigger("isPutIn");
                animatorFirst.SetTrigger("isPutIn");
                //objectToEquipF[id_].SetActive(false);
                //objectToEquipT[id_].SetActive(false);
                GetItemById(objectToEquipF, id_).SetActive(false);
                GetItemById(objectToEquipT, id_).SetActive(false);
                readyToDrink = false;
                readyToEat = false;
                readyToThrow = false;
                //readyToUse = false;

                eatPrompt.SetActive(false);
                drinkPrompt.SetActive(false);
                trashPrompt.SetActive(false);
                consumPrompt.SetActive(false);

        }
        else if (readyToUse) //���� ��� ������(�ڸ� ��� �������)
        {
            animator.SetTrigger("isPutIn");
            animatorFirst.SetTrigger("isPutIn");
            GetItemById(objectToEquipF, id_).SetActive(false);
            GetItemById(objectToEquipT, id_).SetActive(false);
            readyToUse = false;
        }
    }
    public void ReceiveData(ItemData data, CountableItem itemData) //������ ����Ҷ� ������ ������ �ޱ�
    {
        Debug.Log("REcive Item  _data" + _data);
        _data = data;
        _itemData = itemData;
    }
    public bool CheckHoldGift() //���� �ٰ� ����ִ��� �˻�, ī�޶��̿��� ��
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return false; // ���콺�� UI ���� ������ �Է� ����

        if (readyToDrink || readyToEat || readyToThrow) {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SucceedGift() //���� �ִ°� �����ߴٸ�
    {
        {
            ItemUsed();
            animator.SetTrigger("isPutIn");
            animatorFirst.SetTrigger("isPutIn");
            //objectToEquipF[id_].SetActive(false);
            //objectToEquipT[id_].SetActive(false);
            GetItemById(objectToEquipF, id_).SetActive(false);
            GetItemById(objectToEquipT, id_).SetActive(false);
            readyToDrink = false;
            readyToEat = false;
            readyToThrow = false;
            readyToConsum = false;

            eatPrompt.SetActive(false);
            drinkPrompt.SetActive(false);
            trashPrompt.SetActive(false);
            consumPrompt.SetActive(false);
        }
    }
    private void ItemUsed() //������ ���� �� ��������
    {
        _itemData?.UseItem(); //������ ��������
        _itemData = null;
    }
    public void OnRightC(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                rightButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                rightButton = false;
                break;
        }
    }
    public void OnLeftC(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                leftButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                leftButton = false;
                break;
        }
    }
    public void OnThrow(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                throwButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                throwButton = false;
                break;
        }
    }
    public void OnPutIn(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                putButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                putButton = false;
                break;
        }
    }

    private GameObject GetItemById(List<GameObject> itemList, int id)
    {
        foreach (GameObject itemObject in itemList)
        {
            int itemId = itemObject.GetComponent<ItemId>().id;
            if (itemId == id)
                return itemObject;
        }
        return null; // �������� ã�� ���� ���
    }
    private void InventorySetup()
    {
        // "Inventory" �±׸� ���� GameObject�� ã��
        GameObject inventoryObject = GameObject.FindGameObjectWithTag("Equip");

        if (inventoryObject != null)
        {
            foreach (Transform child in inventoryObject.transform)
            {
                GameObject itemObject = child.gameObject;
                objectToEquipT.Add(itemObject);
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'Inventory'.");
        }

        // "Inventory" �±׸� ���� GameObject�� ã��
        GameObject inventoryObjectF = GameObject.FindGameObjectWithTag("EquipF");
        if (inventoryObjectF != null)
        {
            foreach (Transform child in inventoryObjectF.transform)
            {
                GameObject itemObject = child.gameObject;
                objectToEquipF.Add(itemObject);
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'InventoryFirst'.");
        }

        // eat1�� eat2 �ֱ�
        GameObject inventoryEat1 = GameObject.FindGameObjectWithTag("Eat1");
        GameObject inventoryEat2 = GameObject.FindGameObjectWithTag("Eat2");
        if (inventoryEat1 != null)
        {
            foreach (Transform child in inventoryEat1.transform)
            {
                GameObject itemObject = child.gameObject;
                eat1.Add(itemObject);
            }
            foreach (Transform child in inventoryEat2.transform)
            {
                GameObject itemObject = child.gameObject;
                eat2.Add(itemObject);
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'InventoryEat1'.");
        }

        // eat1�� eat2 �ֱ�
        GameObject inventoryDrink = GameObject.FindGameObjectWithTag("Drink1");
        if (inventoryDrink != null)
        {
            foreach (Transform child in inventoryDrink.transform)
            {
                GameObject itemObject = child.gameObject;
                drink1.Add(itemObject);
            }
        }


        // �Ҹ�ǰ �ֱ�
        GameObject inventoryConsum = GameObject.FindGameObjectWithTag("Consum1");
        if (inventoryConsum != null)
        {
            foreach (Transform child in inventoryConsum.transform)
            {
                GameObject itemObject = child.gameObject;
                consum1.Add(itemObject);
            }
        }
    }
}