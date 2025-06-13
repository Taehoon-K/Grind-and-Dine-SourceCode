using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using PP.InventorySystem;
using UnityEngine.InputSystem;

public class CameraRay : MonoBehaviour
{
    GameObject maincamera;
    Camera camera;
    Kupa.Player player;
    PlayerYarn playeryarn;
    TextMeshProUGUI nametext, intercationText;//,takeText;
    
    //[SerializeField]
    private GameObject obj;
    //private GameObject namae,talk,take;

    [SerializeField]
    private LocalizeStringEvent nameLocalizeString;
    private LocalizeStringEvent table;

    public Inventory _inventory;

    private bool interactionButton;
    private bool leftButton;
    private bool rightButton;

    public bool fishingOn { private get; set; } = false; //낚싯대 들고있나 검사

    [Header("Loot")]
    [SerializeField] private float lootTime = 4.0f; // 루팅에 걸리는 시간
    private float currentLootProgress = 0.0f;
    private bool isLooting = false;
    private LootableObject currentLootable;

    [Header("Crosshair")]
    [SerializeField] private float crosshairDistance = 10f; // 감지 전용 거리
    [SerializeField] private GameObject normalCross,findCross;
    bool isFindCrossActive = false; // 클래스 내부에 캐시 변수 선언

    void Start()
    {
        obj = GameObject.FindGameObjectWithTag("Interaction");
        player = FindObjectOfType<Kupa.Player>();
        playeryarn = FindObjectOfType<PlayerYarn>();

        nametext = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        intercationText = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        //takeText = obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>();*/

        table = obj.transform.GetChild(1).GetComponent<LocalizeStringEvent>();


        maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        camera = maincamera.GetComponent<Camera>();
        
    }

    void Update()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        float distan, fishDistan;

        if (player.threePview == true) //3인칭이면 5 아니면 2f
        {
            distan = 5f;
            fishDistan = 7f;
        }
        else
        {
            distan = 2f;
            fishDistan = 4f;
        }
        if (fishingOn && Physics.Raycast(ray, out hit, fishDistan, 1 << LayerMask.NameToLayer("Water"))) //낚시대 들고있고 물 레이어 반사 시
        {
            //낚시 텍스트 띄우고
            if (leftButton && player.isRun == false) //좌클릭 시
            {
                leftButton = false;
                player.playerState = Kupa.Player.PlayerState.FISH; //상태 변경
                Debug.Log("fishfishi");
            }
        }
        else
        {
            //낚시 텍스트 지우기
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        int player3Layer = LayerMask.NameToLayer("Player3rd");
        if (Physics.Raycast(ray, out hit, distan, ~(1 << playerLayer | 1 << player3Layer)))
        {
            string tag = hit.collider.tag;
            //Debug.Log(text.text);
            intercationText.enabled = true;
            if (tag == "Npc")
            {
                if (nameLocalizeString.StringReference == null) //아무 정보도 없으면
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<InteractableCharacter>().npc.name_key;
                }
                else if (nameLocalizeString.gameObject != hit.collider.gameObject)
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<InteractableCharacter>().npc.name_key;
                }
                //nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                /*text.enabled = true;
                intercationText.enabled = true;*/
                table.StringReference.TableEntryReference = "talk_key";

                //takeText.enabled = false;           
                if (interactionButton && player.isRun == false)
                {
                    interactionButton = false;
                    playeryarn.CheckForNearbyNPC(hit.collider.gameObject.GetComponent<InteractableCharacter>()); //얀 대화문 실행
                }
                else if (rightButton && player.isRun == false && gameObject.GetComponent<Throwing>().CheckHoldGift())//손에 무언가를 들고있다면
                {
                    rightButton = false;
                    if (playeryarn.CheckForNearbyNPCGift(gameObject.GetComponent<Throwing>()._data, hit.collider.gameObject.GetComponent<InteractableCharacter>())) //얀 선물 대화문 실행
                    {
                        //만약 주는것에 성공했다면
                        gameObject.GetComponent<Throwing>().SucceedGift();
                    }
                }
            }
            else if (tag == "NpcExtra")
            {
                if (nameLocalizeString.StringReference == null) //아무 정보도 없으면
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<InteractableExtra>().nameKey;
                }
                else if (nameLocalizeString.gameObject != hit.collider.gameObject)
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<InteractableExtra>().nameKey;
                }

                table.StringReference.TableEntryReference = "talk_key";

                //takeText.enabled = false;           
                if (interactionButton && player.isRun == false)
                {
                    interactionButton = false;
                    playeryarn.CheckForNearbyNPC(hit.collider.gameObject.GetComponent<InteractableExtra>().simpleTalk, hit.collider.gameObject.transform); //얀 대화문 실행
                }
                else if (rightButton && player.isRun == false && gameObject.GetComponent<Throwing>().CheckHoldGift())//손에 무언가를 들고있다면
                {
                    rightButton = false;

                    playeryarn.CheckForNearbyNPC(hit.collider.gameObject.GetComponent<InteractableExtra>().simpleGift, hit.collider.gameObject.transform); //얀 대화문 실행
                }
            }
            else if (tag == "Bottle" || tag == "Item")
            {
                if (nameLocalizeString.StringReference == null) //아무 정보도 없으면
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }
                else if (nameLocalizeString.gameObject != hit.collider.gameObject)
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }

                table.StringReference.TableEntryReference = "item_key";
                if (interactionButton && player.isRun == false)
                {  //주우면 인벤토리 add 추가
                    interactionButton = false;
                    _inventory.Add(hit.collider.gameObject.GetComponent<NpcName>()._itemDataArray, 1);
                    Destroy(hit.collider.gameObject);
                }
            }
            else if (tag == "ItemAnother") //만약 클릭해서 바로 먹는것이면
            {
                if (nameLocalizeString.StringReference == null) //아무 정보도 없으면
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }
                else if (nameLocalizeString.gameObject != hit.collider.gameObject)
                {
                    nametext.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }

                table.StringReference.TableEntryReference = "eatInstant_key";
                if (interactionButton && player.isRun == false)
                {
                    interactionButton = false;
                    hit.collider.gameObject.GetComponent<ClickEatItem>().InteractiveThisItem();
                }
            }
            else if (tag == "SceneTrans") //씬 이동 태그시
            {

                table.StringReference.TableEntryReference = "move_key";

                if (interactionButton)
                {
                    interactionButton = false;
                    hit.collider.GetComponent<LocationEntryPoint>().SwitchScene();
                }

            }
            else if (tag == "HouseTrans") //씬 이동 태그시
            {
                table.StringReference.TableEntryReference = "move_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    hit.collider.GetComponent<ToHouseButton>().SwitchScene();
                }

            }
            else if (tag == "Shop")
            {
                table.StringReference.TableEntryReference = "trade_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    hit.collider.GetComponent<Shop>().PickUp();
                }
            }
            else if (tag == "Respawn")
            {
                table.StringReference.TableEntryReference = "look_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    UIManager.instance.TriggerWorkPrompt();

                    QuestManager.instance.CompleteObjective(0, 0);
                }
            }
            else if (tag == "Game")
            {
                table.StringReference.TableEntryReference = "search_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    playeryarn.CheckForNearbyNPC(hit.collider.gameObject.GetComponent<PlayGame>().simpleTalk, hit.collider.gameObject.transform); //얀 대화문 실행
                }
            }
            else if (tag == "Sleep")
            {
                table.StringReference.TableEntryReference = "sleep_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    hit.collider.GetComponent<SleepBed>().PickUp();
                }
            }
            else if (tag == "SoyHot")
            {
                table.StringReference.TableEntryReference = "move_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    int index = hit.collider.GetComponent<ItemId>().id;
                    UIManager.instance.TriggerKickboardPanel(index);
                    
                }
            }
            else if (tag == "ElevatorButton")
            {
                table.StringReference.TableEntryReference = "elevator_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    if (hit.collider.GetComponent<ElevatorButton>())
                    {
                        hit.collider.GetComponent<ElevatorButton>().PickUp();
                    }
                    else
                    {
                        hit.collider.GetComponent<ElevatorInternalButton>().PickUp();
                    }

                }
            }
            else if (tag == "Lootable") //물건 뒤질때
            {
                currentLootable = hit.collider.GetComponent<LootableObject>();

                if (interactionButton && !isLooting) // 공통 조건: 버튼 눌림 & 아직 루팅 중 아님
                {
                    isLooting = true;

                    GetComponent<Animator>().SetTrigger("isRummage");
                    SoundManager.instance.PlaySound2D("scavenging", 0, true);


                    currentLootProgress = 0f; // 게이지형 루팅은 누르고 있는 동안 충전되므로 초기화
                }

                // 일반 루팅일 때만 게이지 충전 처리
                if (isLooting && interactionButton)
                {
                    table.StringReference.TableEntryReference = "searching_key";
                    currentLootProgress += Time.deltaTime;
                    UIManager.instance.UpdateLootGauge(currentLootProgress / lootTime);

                    if (currentLootProgress >= lootTime)
                    {
                        currentLootable.GiveReward();
                        ResetLooting();
                    }
                }


                // 버튼 떼졌을 때 게이지 채우기 중지
                if (!interactionButton || currentLootable == null)
                {
                     table.StringReference.TableEntryReference = "Scavenge_key";
                     ResetLooting();
                }
                else
                {
                     table.StringReference.TableEntryReference = "Scavenge_key";
                }

            }
            else if (tag == "Watch" || tag == "Bell") //튜토리얼 아이템 용도
            {
                table.StringReference.TableEntryReference = "item_key";
                if (interactionButton && player.isRun == false)
                {  //주우면 인벤토리 add 추가
                    interactionButton = false;
                    if (tag == "Watch")
                    {
                        //시계 장착
                        TutorialManager.instance.eatWatch = true;

                    }
                    else
                    {
                        _inventory.Add(hit.collider.gameObject.GetComponent<NpcName>()._itemDataArray, 1);
                        //TutorialManager.instance.eatBread = true;
                        TutorialManager.instance.OffBread();
                    }

                }
            }
            else      //다 끄기
            {
                intercationText.color = Color.white; //다시 하얀색으로
                nametext.enabled = false;
                intercationText.enabled = false;
                if (currentLootable != null)
                {
                    currentLootable = null;
                    ResetLooting();
                }
            }
        }
        else //충돌 안했을때 끄기
        {
            intercationText.color = Color.white; //다시 하얀색으로
            nametext.enabled = false;
            intercationText.enabled = false;
            if (currentLootable != null)
            {
                currentLootable = null;
                ResetLooting();
            }
            interactionButton = false;
            rightButton = false;
            leftButton = false;
        }

        if (Physics.Raycast(ray, out hit, crosshairDistance, ~(1 << playerLayer | 1 << player3Layer)))
        {
            string tag = hit.collider.tag;
            bool isInteractable =
                tag == "Npc" || tag == "NpcExtra" || tag == "Bottle" || tag == "Item" || tag == "ItemAnother" ||
                tag == "SceneTrans" || tag == "HouseTrans" || tag == "Shop" || tag == "Respawn" || tag == "Game" ||
                tag == "Sleep" || tag == "ElevatorButton" || tag == "Lootable" || tag == "Watch" || tag == "Bell" || tag == "SoyHot";

            if (isInteractable != isFindCrossActive)
            {
                findCross.SetActive(isInteractable);
                normalCross.SetActive(!isInteractable);
                isFindCrossActive = isInteractable;
            }
        }
        else
        {
            if (isFindCrossActive)
            {
                findCross.SetActive(false);
                normalCross.SetActive(true);
                isFindCrossActive = false;
            }
        }
    }

    public void OnInteraction(InputAction.CallbackContext context)
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
    private void ResetLooting()
    {
        if (isLooting)
        {
            GetComponent<Animator>().SetTrigger("isRummageCancel");
            SoundManager.instance.StopLoopSound("scavenging");
            isLooting = false;
        }    
        currentLootProgress = 0.0f;
        UIManager.instance.ResetLootGauge();
    }
}
