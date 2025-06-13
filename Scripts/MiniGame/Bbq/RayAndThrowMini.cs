using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.InputSystem;
using UnityEditor.Rendering;

public class RayAndThrowMini : MonoBehaviour
{
    [SerializeField]
    GameObject maincamera;
    Camera camera;
    [Header("Text")]
    [SerializeField]
    TextMeshProUGUI text,interactionText; //아이템 이름 텍스트
    [SerializeField]
    private LocalizeStringEvent nameString;
    [SerializeField]
    private LocalizeStringEvent interationString;

    private TableObject lastHitTargetT;
    private ColliderSauce lastHitTargetSauce;
    private GameObject plateObj; //음식 그릇

    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public Transform trashPoint;
    public GameObject[] objectPrefab;

    [SerializeField]
    private GameObject[] objectToEquipF;
    private Animator animatorFirst;

    [Header("Settings")]
    public float throwCooldown;
    [SerializeField]
    private float throwInvoke;

    [Header("Throwing")]
    public float throwForce;
    public float throwUpwardForce;
    //public float throwAngle = 45f;

    private bool isHold;
    public bool HoldSomething { get { return isHold; } set { isHold = value; animatorFirst.SetBool("isHold", value); } } //손에 들고있는지 확인용
    private int itemState; //손에 들고있는 아이템 종류 무엇인지 

    [SerializeField]
    private GameObject animator;
    [SerializeField] private LayerMask layermaskItem;
    [SerializeField] private LayerMask layermaskPickup;
    [SerializeField] private LayerMask layermaskCookingPot;
    [SerializeField] private LayerMask layermaskTable;
    [SerializeField] private LayerMask layermaskDirtyTable;

    [Header("Desk")]
    [SerializeField] private Transform HandPoint; //접시 쥐는 손 위치

    [SerializeField] private GameObject mixDesk; //재료 섞는 데스크
    private bool mixing;

    [Header("Tools")]
    [SerializeField] private GameObject extingushFx; //소화기 fx
    private bool isExtingush; //소화기 뿌리는 중

    private Transform highlight; //아웃라인 강조 용도

    private int id;
    bool throwButton, leftButton, interactionButton;
    bool isPlayingExtingushSound = false;
    FixedJoint joint;
    GameObject parent;

    [Header("Panel")]
    [SerializeField] private GameObject warningPanel; //게임 나갈거냐는 문구
    [SerializeField] private GameObject recipePanel; //레시피 적혀있는 패널

    private void Awake()
    {
        animatorFirst = animator.GetComponent<Animator>();
    }

    void Start()
    {
        recipePanel.SetActive(false);

        camera = maincamera.GetComponent<Camera>();
        HoldSomething = false;
    }
    /*private void LateUpdate() //뒤에 물리연산용
    {
        if (itemState == 6) //시체 들고있을때
        {
            fi
            HandPoint.GetChild(0).
        }
    }*/
    void Update()
    {

        if (itemState == 6) //시체 들고있을때
        {
            interactionText.enabled = true;
            interationString.StringReference.TableEntryReference = "put_key";
            if (interactionButton)
            {
                interactionButton = false;
                if (joint != null)
                {
                    Destroy(joint);
                }
                foreach (Collider col in parent.GetComponent<HumanoidControllerSub>().cols)
                {
                    col.enabled = true;
                }
                RTF();
                itemState = 0;
            }
            return;
        }

        if (throwButton && HoldSomething && itemState ==1)
        {
            throwButton = false;
            ThrowReady();
        }
        if(leftButton && itemState == 3) //만약 손에 소화기 있고 좌클릭 누르면
        {
            isExtingush = true;
            extingushFx.SetActive(true);
            if (!isPlayingExtingushSound)
            {
                SoundManager.instance.PlaySound2D("extinguisher", 0f, true);
                isPlayingExtingushSound = true; // 소리 재생 상태를 true로 설정
            }           
        }
        else
        {
            extingushFx.SetActive(false);
            isExtingush = false;
            if (isPlayingExtingushSound)
            {
                isPlayingExtingushSound = false;
                SoundManager.instance.StopLoopSound("extinguisher");
            } 
        }

        Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<OutlineReal>().enabled = false;
            highlight = null;
        }


        if (Physics.Raycast(ray, out hit,2f, layermaskItem))
        {
            string tag = hit.collider.tag;

            text.enabled = true;
            interactionText.enabled = true;           

            if (tag == "Npc") //래그돌 npc
            {
                // nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "item_key";

                if (interactionButton && !HoldSomething) //손에 재료있을시
                {
                    interactionButton = false;
                    itemState = 6;
                    joint = hit.collider.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = HandPoint.GetComponent<Rigidbody>();


                    parent = hit.collider.gameObject.GetComponentInParent<HumanoidControllerSub>().gameObject;
                    // 오브젝트의 위치와 회전을 설정합니다.
                    /*parent.transform.SetParent(HandPoint);
                    FixedJoint joint = GetComponentInChildren<FixedJoint>();
                    joint.connectedBody = HandPoint.GetComponent<Rigidbody>();*/
                    foreach (Collider col in parent.GetComponent<HumanoidControllerSub>().cols)
                    {
                        col.enabled = false;
                    }
                }
            }
            else if (tag == "TrashCan") //쓰레기통
            {
                nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "throwaway_key";

                if (interactionButton && HoldSomething && itemState == 1) //손에 재료있을시
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("trashcan");
                    Ground(); //홀드썸띵 폴스 함수
                    itemState = 0;
                }
                else if (interactionButton && HoldSomething && itemState == 2) // 손에 뚝배기 있을 때
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("trashcan");
                    plateObj.GetComponent<Bowl>().Pickup();
                    PlateGround();
                    PlateObjChange();
                    itemState = 0;
                }
                else if (interactionButton && HoldSomething && (itemState == 4 || itemState == 5)) //고기 접시거나 숯일시
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("trashcan");
                    Destroy(HandPoint.GetChild(0).gameObject);
                    PlateGround();
                    itemState = 0;
                }
                highlight = null;
            }

            else if (tag == "ItemActive" || tag == "Item" || tag == "ItemAnother" || tag == "ItemPlate" || tag == "Tray")
            {
                nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "item_key";


                if (interactionButton && !HoldSomething)
                {  //누르면 줍기
                    interactionButton = false;
                    if (tag == "ItemActive")
                    {
                        Destroy(hit.collider.gameObject);
                        id = hit.collider.gameObject.GetComponent<ItemId>().id;
                        itemState = 1;
                    }
                    else if (tag == "ItemAnother") //소화기
                    {

                        hit.collider.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        id = hit.collider.gameObject.GetComponent<ItemId>().id;
                        itemState = 3;

                    }
                    else if (tag == "ItemPlate") //뚝배기랑 사발
                    {
                        // 충돌한 오브젝트의 ItemPrefab 스크립트에서 prefab 가져오기
                        GameObject prefab = hit.collider.gameObject.GetComponent<ItemPrefab>().prefab;

                        GameObject newObject = Instantiate(prefab);
                        plateObj = newObject;
                        newObject.transform.SetParent(HandPoint);

                        SoundManager.instance.PlaySound2D("dinner-plate"); //딸깍 소리 재생

                        if (hit.collider.gameObject.GetComponent<ItemId>().id == 10)//만약 사발이면
                        {
                            newObject.transform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                            newObject.transform.localRotation = Quaternion.Euler(240, 0, 20);
                            newObject.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
                            itemState = 2;
                        }
                        else if (hit.collider.gameObject.GetComponent<ItemId>().id == 11)//뚝배기면
                        {
                            // 오브젝트의 위치와 회전을 설정합니다.
                            newObject.transform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                            newObject.transform.localRotation = Quaternion.Euler(240, 0, 20);
                            newObject.transform.localScale = new Vector3(0.5f, 0.7f, 0.5f);
                            itemState = 2;
                        }
                        else if (hit.collider.gameObject.GetComponent<ItemId>().id == 20)//커피면
                        {
                            // 오브젝트의 위치와 회전을 설정합니다.
                            newObject.transform.localPosition = new Vector3(0.1f, 0.12f, 0f);
                            newObject.transform.localRotation = Quaternion.Euler(-211, 128, -38);
                            newObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            itemState = 2;
                        }
                        else                                                        //숯불이면
                        {
                            newObject.transform.localPosition = new Vector3(0.35f, 0.5f, -0.15f);
                            newObject.transform.localRotation = Quaternion.Euler(-211, 128, -38);
                            newObject.transform.localScale = new Vector3(1, 0.5f, 1);
                            itemState = 5;
                        }


                    }
                    else if (tag == "Tray") //고기처럼 프리팹 불러오고 던지지 못하는것들
                    {
                        /*interTray.Invoke();
                        //Invoke(nameof(PlateBool), 0.1f);*/

                        // 충돌한 오브젝트의 ItemPrefab 스크립트에서 prefab 가져오기
                        GameObject prefab = hit.collider.gameObject.GetComponent<ItemPrefab>().prefab;
                        id = hit.collider.gameObject.GetComponent<ItemPrefab>().id;
                        GameObject newObject = Instantiate(prefab);
                        newObject.transform.SetParent(HandPoint);

                        // 오브젝트의 위치와 회전을 설정합니다.
                        newObject.transform.localPosition = new Vector3(-0.15f, 0, -0.12f);
                        newObject.transform.localRotation = Quaternion.Euler(220, 0, 0);
                        itemState = 4;

                        SoundManager.instance.PlaySound2D("freezer");
                    }
                    else if (tag == "Item") //소주 맥주 등등
                    {
                        if (hit.collider.gameObject.GetComponentInParent<TableCoffee>() != null)
                        {
                            id = hit.collider.gameObject.GetComponentInParent<TableCoffee>().sideId;
                            hit.collider.gameObject.GetComponentInParent<TableCoffee>().SetSide(0, false);    
                        }
                        else
                        {
                            id = hit.collider.gameObject.GetComponent<ItemId>().id;
                            SoundManager.instance.PlaySound2D("freezer");
                        }              
                        itemState = 1;         
                    }
                    Equips(id); //장착이랑 배치 이벤트 실행
                }

                else if (interactionButton && HoldSomething && itemState == 2) //손에 뚝배기 들고 있을시
                {
                    interactionButton = false;
                    if (tag == "SoyHot") //만약 재료 누를시
                    {

                        plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);
                    }
                }
                else if (interactionButton && mixing) //데스크에 음식 있을시
                {
                    interactionButton = false;
                    if (tag == "SoyHot") //만약 재료 누를시
                    {

                        plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);
                    }
                }
                else if (interactionButton && HoldSomething && tag == "ItemAnother") //도구 들고있을시
                {
                    interactionButton = false;
                    hit.collider.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    itemState = 0;
                    Ground();

                }


                highlight = hit.transform;                                            //여기서부터 아웃라인 코드
                if (highlight.gameObject.GetComponent<OutlineReal>() != null)
                {
                    highlight.gameObject.GetComponent<OutlineReal>().enabled = true;
                }
                else
                {
                    OutlineReal outline = highlight.gameObject.AddComponent<OutlineReal>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<OutlineReal>().OutlineColor = Color.magenta;
                    highlight.gameObject.GetComponent<OutlineReal>().OutlineWidth = 7.0f;
                }                                                                                  //여기까지
            }
            else if (tag == "SoyHot")
            {
                nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "item_key";
                if (interactionButton && HoldSomething && itemState == 2) //손에 뚝배기 들고 있을시
                {
                    interactionButton = false;
                    plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);

                }
                else if (interactionButton && mixing) //데스크에 음식 있을시
                {
                    interactionButton = false;
                    plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);

                }

            }
            else if (tag == "SceneTrans") //씬 이동 태그시
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "quit_key";


                if (interactionButton)
                {
                    interactionButton = false;
                    warningPanel.SetActive(true);
                    Time.timeScale = 0; //시간 멈추기
                    //경고문 띄우기
                }

            }
            else if (tag == "Bell")
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "bellUse_key";
                if (interactionButton)
                {
                    interactionButton = false;

                    hit.collider.gameObject.GetComponentInParent<TableCoffee>().RingBell();
                }
            }
            else if (tag == "Lootable")
            {
                nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "look_key";
                if (interactionButton)
                {
                    interactionButton = false;

                    recipePanel.SetActive(true);
                }
            }
            else      //다 끄기
            {
                highlight = null;
                nameString.StringReference = null;
                text.text = ""; //이름 텍스트 비우기
                text.enabled = false;
                interactionText.enabled = false;
            }
        }        
        /*else //충돌 안했을때 끄기
        {
            nameString.StringReference = null;
            text.text = "";
            text.enabled = false;
            interactionText.enabled = false;
        }*/



        else if (Physics.Raycast(ray, out hit, 2f, layermaskPickup))  //뚝배기 놓고 줍기용
        {
            text.enabled = true;
            interactionText.enabled = true;
            if (itemState == 2 && HoldSomething)
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.transform.childCount == 0) //데스크에 자식 없을때, 즉 접시 안올라져있을때
                {
                    interactionButton = false;//내려놓기
                    Transform childTransform = HandPoint.GetChild(0);
                    FireBowl firebowl = childTransform.GetComponent<FireBowl>();
                    if (hit.collider.tag == "Bottle" && firebowl == null) //만약 사발이고 가스레인지라면
                    {
                        return;
                    }
                    SoundManager.instance.PlaySound2D("plateDown"); //딸깍 소리 재생

                    itemState = 0;
                    HoldSomething = false;
                    childTransform.transform.SetParent(hit.collider.gameObject.transform);
                    childTransform.transform.localPosition = childTransform.GetComponent<Bowl>().savedPosition;
                    childTransform.localRotation = childTransform.GetComponent<Bowl>().savedRotation;
                    childTransform.localScale = childTransform.GetComponent<Bowl>().savedScale;

                    if (hit.collider.gameObject == mixDesk)                                                 //섞는 데스크라면
                    {
                        mixing = true;
                    }
                    else
                    {
                        PlateObjChange();
                    }


                    if (hit.collider.tag == "Bottle" && firebowl != null) //만약 뚝배기라면
                    {
                        firebowl.IsCook = true;
                    }
                }
            }
            else if (itemState == 1 && HoldSomething && hit.collider.gameObject.transform.childCount == 1)
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //데스크에 자식 없을때, 즉 접시 안올라져있을때
                {
                    interactionButton = false;//내려놓기
                    GameObject child = hit.collider.gameObject.transform.GetChild(0).gameObject;
                    Bowl bowl = child.GetComponent<Bowl>();
                    if (bowl != null && (id == 2 || id == 3)) //만약 사발이고 가스레인지라면
                    {
                        bowl.Pickup(id-2); //면 넣기
                        Ground(); //equip false 만들기
                        itemState = 0;
                    }
                }
            }
            else if (itemState == 5 && HoldSomething && hit.collider.tag == "Shop") //숯 이라면
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.transform.childCount == 0) //데스크에 자식 없을때, 즉 접시 안올라져있을때
                {
                    interactionButton = false;
                    Transform childTransform = HandPoint.GetChild(0);
                    CharcoalBowl charbowl = childTransform.GetComponent<CharcoalBowl>();

                    itemState = 0;
                    HoldSomething = false;
                    childTransform.transform.SetParent(hit.collider.gameObject.transform);
                    childTransform.transform.localPosition = childTransform.GetComponent<Bowl>().savedPosition;
                    childTransform.localRotation = childTransform.GetComponent<Bowl>().savedRotation;
                    childTransform.localScale = childTransform.GetComponent<Bowl>().savedScale;
                    charbowl.IsCook = true;
                }
            }
            else if (itemState == 0 && !HoldSomething && hit.collider.gameObject.transform.childCount != 0) //손에 암것도 없고 뭐 올라가있으면
            {                                                                                               //다시 줍기
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //버튼 누를시
                {
                    interactionButton = false;
                    Transform childTransform = hit.collider.gameObject.transform.GetChild(0);

                    if (childTransform.gameObject.GetComponent<ItemId>() != null &&
                        childTransform.gameObject.GetComponent<ItemId>().id == 11)//뚝배기면
                    {
                        if (childTransform.gameObject.GetComponent<FireBowl>().isBurning) //만약 불나고있으면
                        {
                            return;
                        }
                        // 오브젝트의 위치와 회전을 설정합니다.
                        childTransform.SetParent(HandPoint);
                        childTransform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                        childTransform.localRotation = Quaternion.Euler(240, 0, 20);
                        childTransform.localScale = new Vector3(0.5f, 0.7f, 0.5f);
                        childTransform.gameObject.GetComponent<FireBowl>().IsCook = false; //요리 멈추기
                        plateObj = childTransform.gameObject;
                        itemState = 2;
                    }
                    else if(childTransform.gameObject.GetComponent<ItemId>() != null &&
                        childTransform.gameObject.GetComponent<ItemId>().id == 10) //사발이면
                    {
                        childTransform.SetParent(HandPoint);
                        childTransform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                        childTransform.localRotation = Quaternion.Euler(240, 0, 20);
                        childTransform.localScale = new Vector3(0.7f, 1f, 0.7f);
                        plateObj = childTransform.gameObject;
                        itemState = 2;
                    }
                    else if (hit.collider.gameObject.GetComponent<ItemId>() != null && 
                        hit.collider.gameObject.GetComponent<ItemId>().id == 20)//커피면
                    {
                        childTransform.SetParent(HandPoint);
                        // 오브젝트의 위치와 회전을 설정합니다.
                        childTransform.transform.localPosition = new Vector3(0.1f, 0.12f, 0f);
                        childTransform.transform.localRotation = Quaternion.Euler(-211, 128, -38);
                        childTransform.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        plateObj = childTransform.gameObject;
                        itemState = 2;

                        hit.collider.gameObject.GetComponentInParent<TableCoffee>().OffCoffee();
                    }
                    else                                                        //숯불이면
                    {
                        
                        childTransform.SetParent(HandPoint);
                        childTransform.localPosition = new Vector3(0.35f, 0.5f, -0.15f);
                        childTransform.localRotation = Quaternion.Euler(-211, 128, -38);
                        childTransform.localScale = new Vector3(1, 0.5f, 1);
                        childTransform.gameObject.GetComponent<CharcoalBowl>().IsCook = false;
                        itemState = 5;
                    }


                    HoldSomething = true;
                    if (hit.collider.gameObject == mixDesk)
                    {
                        mixing = false;
                    }
                }
            }
            else
            {
                text.text = "";
                text.enabled = false;
                interactionText.enabled = false;
            }


            if (isExtingush)
            {
                Transform parentTransform = hit.collider.gameObject.transform;

                // 자식이 있는지 확인
                if (parentTransform.childCount > 0)
                {
                    Transform childTransform = parentTransform.GetChild(0);
                    FireBowl f = childTransform.gameObject.GetComponent<FireBowl>();

                    if (f != null)
                    {
                        f.PutOutFire();
                    }
                }
            }
        }
        else if (Physics.Raycast(ray, out hit, 2f, layermaskCookingPot))  //냄비용도
        {
            text.enabled = true;
            interactionText.enabled = true;
            if (itemState == 1 && HoldSomething && (id == 0 || id == 1))
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //데스크에 자식 없을때, 즉 접시 안올라져있을때
                {
                    interactionButton = false;
                    //내려놓기
                    itemState = 0;
                    HoldSomething = false;
                    hit.collider.gameObject.transform.GetChild(0).GetComponent<CookingPot>().Pickup(id);
                    Ground(); //아이템 내려놓기
                }
            }

            else if (itemState == 0 && !HoldSomething) //손에 암것도 없고 뭐 올라가있으면
            {                                                                                               //다시 줍기
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //버튼 누를시
                {
                    interactionButton = false;
                    Transform childTransform = hit.collider.gameObject.transform.GetChild(0);
                    if (childTransform.gameObject.GetComponent<CookingPot>().isBurning || childTransform.gameObject.GetComponent<CookingPot>().IsCook == false) 
                    //만약 불나고있거나 요리중이 아니면
                    {
                        return;
                    }
                    id = childTransform.gameObject.GetComponent<CookingPot>().Pickout();
                    itemState = 1;
                    Equips(id);
                }
            }
            else
            {
                text.text = "";
                text.enabled = false;
                interactionText.enabled = false;
            }


            if (isExtingush)
            {
                Transform childTransform = hit.collider.gameObject.transform.GetChild(0);
                FireBowl f = childTransform.gameObject.GetComponent<FireBowl>();
                CookingPot cookP = childTransform.gameObject.GetComponent<CookingPot>();
                if (f != null)
                {
                    f.PutOutFire();
                }
                else if (cookP != null)
                {
                    cookP.PutOutFire();
                }

            }
        }

        else if (Physics.Raycast(ray, out hit, 2f, layermaskTable))  //테이블용도
        {
            text.enabled = true;
            interactionText.enabled = true;

            if (isExtingush) //만약 소화기 테이블에 뿌릴시
            {
                hit.collider.gameObject.GetComponent<TableObjectBbq>().Angry(); //화남
                return;
            }

            if (itemState == 5 && HandPoint.GetChild(0).gameObject.GetComponent<CharcoalBowl>().BeCooked) //숯 들고있고 익은거면
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableCharcoal())
                //체크테이블 트루일때
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("dinner-plate"); //딸깍 소리 재생
                    Destroy(HandPoint.GetChild(0).gameObject);
                    itemState = 0;
                    PlateGround(); //홀스썸띵 폴스
                }
            }
            else if (itemState == 2 && HoldSomething) //면류 음식일시
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.GetComponent<TableObjectBbq>() != null &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().isCharcoal &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().SubOn)
                //숯 올라가져있을시, 그리고 서브메뉴 켜져있을시
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //딸깍 소리 재생
                    bool isCooked = false, isFirebowl = false, isBurnt = false; //가열됬는지 여부, 뚝배기인지 여부, 탔는지 여부
                    GameObject bowl = HandPoint.GetChild(0).gameObject;
                    if (bowl.GetComponent<FireBowl>() != null) //만약 뚝배기라면
                    {
                        isFirebowl = true;
                        if (bowl.GetComponent<FireBowl>().beCooked)
                        {
                            isCooked = true;
                        }
                        if (bowl.GetComponent<FireBowl>().beBurnt)
                        {
                            isBurnt = true;
                        }
                    }
                    bool[] a = bowl.GetComponent<Bowl>().ingredient;
                    if (hit.collider.gameObject.GetComponent<TableObjectBbq>() != null) //테이블오브젝트bbq라면
                    {
                        hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableSub(a, isCooked, isFirebowl, isBurnt);
                    }

                    /*  if (hit.collider.gameObject.GetComponent<TableCoffee>() != null) //테이블커피라면
                      {
                         bool mix = bowl.GetComponent<CoffeeBowl>().GetMix();
                         hit.collider.gameObject.GetComponent<TableCoffee>().SetCoffee(a, mix, true);
                      }*/

                    bowl.transform.SetParent(hit.collider.gameObject.transform.GetChild(0));
                    bowl.transform.localPosition = bowl.GetComponent<Bowl>().savedPosition;
                    bowl.transform.localRotation = bowl.GetComponent<Bowl>().savedRotation;
                    bowl.transform.localScale = bowl.GetComponent<Bowl>().savedScale;

                    itemState = 0;
                    PlateGround(); //홀스썸띵 폴스
                }
                else if (interactionButton && hit.collider.gameObject.GetComponent<TableCoffee>() != null && !hit.collider.gameObject.GetComponent<TableCoffee>().GetCoffee())
                {
                    interactionButton = false;
                    GameObject bowl = HandPoint.GetChild(0).gameObject;
                    bool[] a = bowl.GetComponent<Bowl>().ingredient;
                    bool mix = bowl.GetComponent<CoffeeBowl>().GetMix();
                    hit.collider.gameObject.GetComponent<TableCoffee>().SetCoffee(a, mix, true);

                    bowl.transform.SetParent(hit.collider.gameObject.transform.GetChild(0));
                    bowl.transform.localPosition = bowl.GetComponent<Bowl>().savedPosition;
                    bowl.transform.localRotation = bowl.GetComponent<Bowl>().savedRotation;
                    bowl.transform.localScale = bowl.GetComponent<Bowl>().savedScale;
                    itemState = 0;
                    PlateGround(); //홀스썸띵 폴스
                }
            }
            else if (itemState == 4 && HoldSomething) //고기 접시일때
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().isCharcoal &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableMain(id))
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //딸깍 소리 재생

                    hit.collider.gameObject.GetComponent<TableObjectBbq>().meatObject[id].SetActive(true);
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().fxOther.SetActive(true);
                    Destroy(HandPoint.GetChild(0).gameObject);
                    PlateGround();
                    itemState = 0;
                }
            }
            else if (itemState == 1 && HoldSomething && hit.collider.gameObject.GetComponent<TableObjectBbq>() != null) //음료수일시
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableDrink(id - 5))
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //딸깍 소리 재생

                    hit.collider.gameObject.GetComponent<TableObjectBbq>().drinkObject[id - 5].SetActive(true);
                    Ground();
                    itemState = 0;
                }
            }
            else if (itemState == 1 && HoldSomething && hit.collider.gameObject.GetComponent<TableCoffee>() != null) //카페에서 사이드들고있을시
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //딸깍 소리 재생

                    hit.collider.gameObject.GetComponent<TableCoffee>().sideObject[id].SetActive(true);
                    hit.collider.gameObject.GetComponent<TableCoffee>().SetSide(id, true);
                    Ground();
                    itemState = 0;
                }
            }
            else
            {
                text.text = "";
                text.enabled = false;
                interactionText.enabled = false;
            }
        }
        else if (Physics.Raycast(ray, out hit, 2f, layermaskDirtyTable))
        {
            text.enabled = true;
            interationString.StringReference.TableEntryReference = "clean_key";
            interactionText.enabled = true;
            if (interactionButton)
            {
                if (!HoldSomething) //손에 물건 없으면
                {
                    hit.collider.gameObject.GetComponent<TableObject>().IsClean = true;
                    lastHitTargetT = hit.collider.gameObject.GetComponent<TableObject>();

                    animatorFirst.SetBool("isMix", true);
                    objectToEquipF[9].SetActive(true); //걸레 활성화
                } //물건 있으면
                else
                {
                    //메세지 띄우기
                }

            }
            // 버튼 떼졌을 때 게이지 채우기 중지
            else if (!interactionButton)
            {
                interactionButton = false;
                hit.collider.gameObject.GetComponent<TableObject>().IsClean = false;

                animatorFirst.SetBool("isMix", false);
                objectToEquipF[9].SetActive(false); //걸레 비활성화
            }

        }
        else
        {
            if (lastHitTargetT != null) //설거지 컴포넌트 눌 아니면
            {
                lastHitTargetT.IsClean = false;

                animatorFirst.SetBool("isMix", false);
                objectToEquipF[9].SetActive(false); //걸레 비활성화
            }
            text.text = "";
            text.enabled = false;
            interactionText.enabled = false;
        }
 
    }
    private void PlateObjChange() //지정 뚝배기 그릇 변경 함수
    {
        if (mixing) //만약 데스크에 뚝배기 있으면
        {
            plateObj = mixDesk.transform.GetChild(0).gameObject; //그 뚝배기로 지정 바꾸기
        }
        else
        {
            plateObj = null;
        }
    }


    private void ThrowReady()
    {
        //animator.SetTrigger("isThrow");
        animatorFirst.SetTrigger("isThrow");
        itemState = 0;
        Invoke(nameof(Throw), throwInvoke);
    }
    private void Throw()
    {
        objectToEquipF[id].SetActive(false);
        HoldSomething = false;

        GameObject projectile = null;
        // instantiate object to throw
        projectile = Instantiate(objectPrefab[id], attackPoint.position, cam.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 20f, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            // Calculate direction towards hit point
            forceDirection = (hit.point - attackPoint.position).normalized;

            // Calculate the distance to the target point
            float distanceToTarget = Vector3.Distance(hit.point, attackPoint.position);

            // Set the throw angle in degrees (try 45 degrees as a starting point)
            float throwAngle = 20f;

            // Calculate the required velocity magnitude (using simplified projectile motion equation)
            float gravity = Physics.gravity.magnitude;
            float throwAngleRad = throwAngle * Mathf.Deg2Rad; // Convert to radians

            // Calculate the velocity required to reach the target distance at the specified angle
            float projectileVelocity = Mathf.Sqrt(distanceToTarget * gravity / Mathf.Sin(2 * throwAngleRad));

            // Break the velocity into x and y components based on the angle
            Vector3 velocity = forceDirection * Mathf.Cos(throwAngleRad) * projectileVelocity + Vector3.up * Mathf.Sin(throwAngleRad) * projectileVelocity;

            // Set the projectile's velocity directly
            projectileRb.velocity = velocity;
        }
        else
        {
            // Default throw direction
            Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        }

    }
    public void Ground()  //허상 아이템 내려놓을때 실행 함수
    {
        objectToEquipF[id].SetActive(false);
        
        Invoke(nameof(RTF), 0.05f);
    }
    public void PlateGround()  //접시 내려놓았을 때 실행 함수
    {
        Invoke(nameof(RTF), 0.05f);
    }
    private void RTT()
    {
        HoldSomething = true;
    }
    private void RTF()
    {
        HoldSomething = false;
    }
    private void Equips(int index)
    {

        if (HoldSomething)
        {
            return;
        }
        else
        {
            if(itemState ==1 || itemState == 3) //재료랑 도구들처럼 허상인것들
            {
                objectToEquipF[index].SetActive(true);
            }
            
            id = index;
            Invoke(nameof(RTT), 0.05f);
            //onPlace.Invoke(index);
        }
    }

    public void OnTryLeft(InputAction.CallbackContext context)
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
}
