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
    bool readyToConsum; //소모품 아이템
    bool throwButton, putButton, rightButton, leftButton;
    //public int throwNumber { get; set; }
    private Animator animator;
    private Animator animatorFirst;
    private Status stat;
    private MoodleImform[] moodleImform;

    public ItemData _data { get; private set; } //다른 곳에선 읽기만 가능
    public CountableItem _itemData { get; private set; } //다른 곳에선 읽기만 가능

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
    private List<GameObject> objectToDrinkTrash = new List<GameObject>(); //다먹고 버리는 쓰레기 프리팹*/

    [Header("UI")]
    [SerializeField]
    private GameObject eatPrompt,drinkPrompt,trashPrompt, consumPrompt;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private CanvasOnOff watchOn;

    [Header("smoke")]
    [SerializeField]
    private GameObject smoking; //담배연기

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
            return; // 마우스가 UI 위에 있으면 입력 무시

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
        //objectToEquipF[id_].SetActive(false); //1인칭
        //objectToEquipT[id_].SetActive(false); //3인칭
        GetItemById(objectToEquipF, id_).SetActive(false);
        GetItemById(objectToEquipT, id_).SetActive(false);
        readyToThrow = false; //던지기 실행되는 동안 던지기 함수 호출 금지

        // 던지는 오브젝트 복제
        GameObject projectile = Instantiate(objectToThrow[id_], attackPoint.position, cam.rotation);

        // 중력 추가
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // 방향 계산
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        } //중앙으로 Raycast 쏴서 hit 되는 오브젝트 쪽으로 방향 보정

        // force 추가
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

    }

    private void DrinkReady()
    {
        ItemUsed();
        drinkPrompt.SetActive(false);
        UIManager.instance.OnEatWaiting(true); //ui상태 변경
       // objectToEquipF[id_].SetActive(false); //들고있던거 끄기
       // objectToEquipT[id_].SetActive(false); //들고있던거 끄기
        GetItemById(objectToEquipF, id_).SetActive(false);
        GetItemById(objectToEquipT, id_).SetActive(false);
        //drink1[id_].SetActive(true);  //첫번째 음식 키기
        GetItemById(drink1, id_).SetActive(true);

        animatorFirst.SetTrigger("isDrink");
        Invoke(nameof(Drink), drinkInvoke);
        readyToDrink = false;

        onInventory.Invoke(); //1인칭 강제 변경

        SoundManager.instance.PlaySound2D("Drinking");
    }

    private void Drink()
    {
        UIManager.instance.OnEatWaiting(false); //ui상태 변경
        //drink1[id_].SetActive(false); //들고있던거 끄기
        GetItemById(drink1, id_).SetActive(false);

        StatusManager.instance.AddStat(stat);
        StatusManager.instance.AddMoodle(moodleImform);

       // GameObject projectile = Instantiate(objectToThrow[id_], trashPoint.position, cam.rotation); //쓰레기 버리기

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1인칭 강제 변경 풀기
    }

    private void EatReady()
    {
        ItemUsed();
        eatPrompt.SetActive(false);
        UIManager.instance.OnEatWaiting(true); //ui상태 변경

        GetItemById(objectToEquipF, id_).SetActive(false);
        GetItemById(objectToEquipT, id_).SetActive(false);
        GetItemById(eat1, id_).SetActive(true);

        animatorFirst.SetTrigger("isEat");
        Invoke(nameof(EatOneBite), eatInvoke / 2);
        Invoke(nameof(Eat), eatInvoke);
        readyToEat = false;

        onInventory.Invoke(); //1인칭 강제 변경.

        if (id_ == 1) //만약 담배면
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
        //eat1[id_].SetActive(false); //들고있던거 끄기
        //eat2[id_].SetActive(true);  //두번째 음식 키기
        GetItemById(eat1, id_).SetActive(false);
        GetItemById(eat2, id_).SetActive(true);

        if(id_  == 1) //만약 담배면
        {
            smoking.SetActive(false);
            smoking.SetActive(true);
        }
    }
    private void Eat()
    {
        UIManager.instance.OnEatWaiting(false); //ui상태 변경
       // eat2[id_].SetActive(false);
        GetItemById(eat2, id_).SetActive(false);

        StatusManager.instance.AddStat(stat);
        StatusManager.instance.AddMoodle(moodleImform);
        //GameObject projectile = Instantiate(objectToThrow[id_], trashPoint.position, cam.rotation); //쓰레기 버리기
        if (TutorialManager.instance != null)  //만약 튜토리얼중이라면
        {
            TutorialManager.instance.eatBread = true;
        }
        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1인칭 강제 변경 풀기

        smoking.SetActive(false);

        QuestManager.instance.CompleteObjective(0, 2); //퀘스트 완
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

        onInventory.Invoke(); //1인칭 강제 변경

        //SoundManager.instance.PlaySound2D("Drinking");
    }

    private void Consum()
    {
        UIManager.instance.OnEatWaiting(false); //ui상태 변경
        //drink1[id_].SetActive(false); //들고있던거 끄기
        GetItemById(consum1, id_).SetActive(false); //소모아이템 목록에서 찾기

        StatusManager.instance.AddExperience(skillIndex, skillAmount); //경험치 증가

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1인칭 강제 변경 풀기
    }


    #region EatFoodNotInven
    public void EatReadyCutscene(int foodId) //인벤에서 꺼내 먹는거 아니고 그냥 먹을시
    {
        PutIn(); //뭔가 들고 있을시 집어넣고

        UIManager.instance.OnEatWaiting(true); //ui상태 변경

        GetItemById(eat1, foodId).SetActive(true);

        animatorFirst.SetTrigger("isEat");
        StartCoroutine(DelayedEat(eatInvoke / 2, foodId));
        StartCoroutine(DelayedEat2(eatInvoke, foodId));

        SoundManager.instance.PlaySound2D("Eating");
        onInventory.Invoke(); //1인칭 강제 변경
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

        StatusManager.instance.EatItemCalcul(id - 100); //스탯 증감 실행

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1인칭 강제 변경 풀기

        UIManager.instance.OnEatWaiting(false); //ui상태 변경

        QuestManager.instance.CompleteObjective(0, 2); //퀘스트 완
    }

    public void DrinkReadyCutscene(int foodId)
    {
        PutIn(); //뭔가 들고 있을시 집어넣고

        UIManager.instance.OnEatWaiting(true); //ui상태 변경

        GetItemById(drink1, foodId).SetActive(true);

        animatorFirst.SetTrigger("isDrink");
        StartCoroutine(DelayedDrink(drinkInvoke, foodId));

        SoundManager.instance.PlaySound2D("Drinking");
        onInventory.Invoke(); //1인칭 강제 변경
    }
    private IEnumerator DelayedDrink(float delay, int foodId)
    {
        yield return new WaitForSeconds(delay);
        DrinkCutscene(foodId);
    }
    private void DrinkCutscene(int id)
    {
        GetItemById(drink1, id).SetActive(false);

        StatusManager.instance.EatItemCalcul(id - 100); //스탯 증감 실행

        animator.SetTrigger("isPutIn");
        offInventory.Invoke(); //1인칭 강제 변경 풀기

        UIManager.instance.OnEatWaiting(false); //ui상태 변경
    }

    #endregion


    public bool Equips(int id,int code, Status _value=null, MoodleImform[] _moodleValue = null) //인벤토리에서 클릭했을때
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
                case 0: //선물 말고 암것도 못하는거,(쓰레기 등등)
                    readyToThrow = true;
                    trashPrompt.SetActive(true);
                    break;
                case 1: //먹는거
                    readyToEat = true;
                    eatPrompt.SetActive(true);
                    stat = _value; //스탯 배열 받아오기
                    moodleImform = _moodleValue; //무들 정보 받아오기
                    break;
                case 2: //마시는거
                    readyToDrink = true;
                    drinkPrompt.SetActive(true);
                    stat = _value;
                    moodleImform = _moodleValue; //무들 정보 받아오기
                    break;
                case 3: //사용무기
                    readyToUse = true;
                    break;
                default:
                    break;
            }
            switch (id)//도구일 경우 각 경우에 맞게 상태 바꾸기
            {
                case 1: //낚싯대 들었을때
                    gameObject.GetComponent<CameraRay>().fishingOn = true;
                    break;
                default:
                    break;

            }
        
            return true;
        } 
    }
    public bool Equips(int id, int code, int sIndex, int sAmount) //스킬북용 이큅, 나중에 소모품들용도로 더 확장할것
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
                case 0: //책
                    readyToConsum = true;
                    consumPrompt.SetActive(true);
                    break;
                default:
                    break;
            }
            return true;
        }
    }

    private void PutIn() //아이템 다시 집어넣기
    {
        if (readyToDrink || readyToEat || readyToThrow || readyToConsum) //무언가 들고있음
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
        else if (readyToUse) //도구 들고 있을때(자리 없어도 상관없음)
        {
            animator.SetTrigger("isPutIn");
            animatorFirst.SetTrigger("isPutIn");
            GetItemById(objectToEquipF, id_).SetActive(false);
            GetItemById(objectToEquipT, id_).SetActive(false);
            readyToUse = false;
        }
    }
    public void ReceiveData(ItemData data, CountableItem itemData) //아이템 사용할때 아이템 데이터 받기
    {
        Debug.Log("REcive Item  _data" + _data);
        _data = data;
        _itemData = itemData;
    }
    public bool CheckHoldGift() //선물 줄거 들고있는지 검사, 카메라레이에서 씀
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return false; // 마우스가 UI 위에 있으면 입력 무시

        if (readyToDrink || readyToEat || readyToThrow) {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SucceedGift() //선물 주는것 성공했다면
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
    private void ItemUsed() //아이템 썼을 시 수량감소
    {
        _itemData?.UseItem(); //아이템 수량감소
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
        return null; // 아이템을 찾지 못한 경우
    }
    private void InventorySetup()
    {
        // "Inventory" 태그를 가진 GameObject를 찾음
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

        // "Inventory" 태그를 가진 GameObject를 찾음
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

        // eat1과 eat2 넣기
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

        // eat1과 eat2 넣기
        GameObject inventoryDrink = GameObject.FindGameObjectWithTag("Drink1");
        if (inventoryDrink != null)
        {
            foreach (Transform child in inventoryDrink.transform)
            {
                GameObject itemObject = child.gameObject;
                drink1.Add(itemObject);
            }
        }


        // 소모품 넣기
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