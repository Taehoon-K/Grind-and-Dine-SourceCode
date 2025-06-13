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
    TextMeshProUGUI text,interactionText; //������ �̸� �ؽ�Ʈ
    [SerializeField]
    private LocalizeStringEvent nameString;
    [SerializeField]
    private LocalizeStringEvent interationString;

    private TableObject lastHitTargetT;
    private ColliderSauce lastHitTargetSauce;
    private GameObject plateObj; //���� �׸�

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
    public bool HoldSomething { get { return isHold; } set { isHold = value; animatorFirst.SetBool("isHold", value); } } //�տ� ����ִ��� Ȯ�ο�
    private int itemState; //�տ� ����ִ� ������ ���� �������� 

    [SerializeField]
    private GameObject animator;
    [SerializeField] private LayerMask layermaskItem;
    [SerializeField] private LayerMask layermaskPickup;
    [SerializeField] private LayerMask layermaskCookingPot;
    [SerializeField] private LayerMask layermaskTable;
    [SerializeField] private LayerMask layermaskDirtyTable;

    [Header("Desk")]
    [SerializeField] private Transform HandPoint; //���� ��� �� ��ġ

    [SerializeField] private GameObject mixDesk; //��� ���� ����ũ
    private bool mixing;

    [Header("Tools")]
    [SerializeField] private GameObject extingushFx; //��ȭ�� fx
    private bool isExtingush; //��ȭ�� �Ѹ��� ��

    private Transform highlight; //�ƿ����� ���� �뵵

    private int id;
    bool throwButton, leftButton, interactionButton;
    bool isPlayingExtingushSound = false;
    FixedJoint joint;
    GameObject parent;

    [Header("Panel")]
    [SerializeField] private GameObject warningPanel; //���� �����ųĴ� ����
    [SerializeField] private GameObject recipePanel; //������ �����ִ� �г�

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
    /*private void LateUpdate() //�ڿ� ���������
    {
        if (itemState == 6) //��ü ���������
        {
            fi
            HandPoint.GetChild(0).
        }
    }*/
    void Update()
    {

        if (itemState == 6) //��ü ���������
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
        if(leftButton && itemState == 3) //���� �տ� ��ȭ�� �ְ� ��Ŭ�� ������
        {
            isExtingush = true;
            extingushFx.SetActive(true);
            if (!isPlayingExtingushSound)
            {
                SoundManager.instance.PlaySound2D("extinguisher", 0f, true);
                isPlayingExtingushSound = true; // �Ҹ� ��� ���¸� true�� ����
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

            if (tag == "Npc") //���׵� npc
            {
                // nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "item_key";

                if (interactionButton && !HoldSomething) //�տ� ���������
                {
                    interactionButton = false;
                    itemState = 6;
                    joint = hit.collider.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = HandPoint.GetComponent<Rigidbody>();


                    parent = hit.collider.gameObject.GetComponentInParent<HumanoidControllerSub>().gameObject;
                    // ������Ʈ�� ��ġ�� ȸ���� �����մϴ�.
                    /*parent.transform.SetParent(HandPoint);
                    FixedJoint joint = GetComponentInChildren<FixedJoint>();
                    joint.connectedBody = HandPoint.GetComponent<Rigidbody>();*/
                    foreach (Collider col in parent.GetComponent<HumanoidControllerSub>().cols)
                    {
                        col.enabled = false;
                    }
                }
            }
            else if (tag == "TrashCan") //��������
            {
                nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "throwaway_key";

                if (interactionButton && HoldSomething && itemState == 1) //�տ� ���������
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("trashcan");
                    Ground(); //Ȧ���� ���� �Լ�
                    itemState = 0;
                }
                else if (interactionButton && HoldSomething && itemState == 2) // �տ� �ҹ�� ���� ��
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("trashcan");
                    plateObj.GetComponent<Bowl>().Pickup();
                    PlateGround();
                    PlateObjChange();
                    itemState = 0;
                }
                else if (interactionButton && HoldSomething && (itemState == 4 || itemState == 5)) //��� ���ðų� ���Ͻ�
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
                {  //������ �ݱ�
                    interactionButton = false;
                    if (tag == "ItemActive")
                    {
                        Destroy(hit.collider.gameObject);
                        id = hit.collider.gameObject.GetComponent<ItemId>().id;
                        itemState = 1;
                    }
                    else if (tag == "ItemAnother") //��ȭ��
                    {

                        hit.collider.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        id = hit.collider.gameObject.GetComponent<ItemId>().id;
                        itemState = 3;

                    }
                    else if (tag == "ItemPlate") //�ҹ��� ���
                    {
                        // �浹�� ������Ʈ�� ItemPrefab ��ũ��Ʈ���� prefab ��������
                        GameObject prefab = hit.collider.gameObject.GetComponent<ItemPrefab>().prefab;

                        GameObject newObject = Instantiate(prefab);
                        plateObj = newObject;
                        newObject.transform.SetParent(HandPoint);

                        SoundManager.instance.PlaySound2D("dinner-plate"); //���� �Ҹ� ���

                        if (hit.collider.gameObject.GetComponent<ItemId>().id == 10)//���� ����̸�
                        {
                            newObject.transform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                            newObject.transform.localRotation = Quaternion.Euler(240, 0, 20);
                            newObject.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
                            itemState = 2;
                        }
                        else if (hit.collider.gameObject.GetComponent<ItemId>().id == 11)//�ҹ���
                        {
                            // ������Ʈ�� ��ġ�� ȸ���� �����մϴ�.
                            newObject.transform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                            newObject.transform.localRotation = Quaternion.Euler(240, 0, 20);
                            newObject.transform.localScale = new Vector3(0.5f, 0.7f, 0.5f);
                            itemState = 2;
                        }
                        else if (hit.collider.gameObject.GetComponent<ItemId>().id == 20)//Ŀ�Ǹ�
                        {
                            // ������Ʈ�� ��ġ�� ȸ���� �����մϴ�.
                            newObject.transform.localPosition = new Vector3(0.1f, 0.12f, 0f);
                            newObject.transform.localRotation = Quaternion.Euler(-211, 128, -38);
                            newObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            itemState = 2;
                        }
                        else                                                        //�����̸�
                        {
                            newObject.transform.localPosition = new Vector3(0.35f, 0.5f, -0.15f);
                            newObject.transform.localRotation = Quaternion.Euler(-211, 128, -38);
                            newObject.transform.localScale = new Vector3(1, 0.5f, 1);
                            itemState = 5;
                        }


                    }
                    else if (tag == "Tray") //���ó�� ������ �ҷ����� ������ ���ϴ°͵�
                    {
                        /*interTray.Invoke();
                        //Invoke(nameof(PlateBool), 0.1f);*/

                        // �浹�� ������Ʈ�� ItemPrefab ��ũ��Ʈ���� prefab ��������
                        GameObject prefab = hit.collider.gameObject.GetComponent<ItemPrefab>().prefab;
                        id = hit.collider.gameObject.GetComponent<ItemPrefab>().id;
                        GameObject newObject = Instantiate(prefab);
                        newObject.transform.SetParent(HandPoint);

                        // ������Ʈ�� ��ġ�� ȸ���� �����մϴ�.
                        newObject.transform.localPosition = new Vector3(-0.15f, 0, -0.12f);
                        newObject.transform.localRotation = Quaternion.Euler(220, 0, 0);
                        itemState = 4;

                        SoundManager.instance.PlaySound2D("freezer");
                    }
                    else if (tag == "Item") //���� ���� ���
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
                    Equips(id); //�����̶� ��ġ �̺�Ʈ ����
                }

                else if (interactionButton && HoldSomething && itemState == 2) //�տ� �ҹ�� ��� ������
                {
                    interactionButton = false;
                    if (tag == "SoyHot") //���� ��� ������
                    {

                        plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);
                    }
                }
                else if (interactionButton && mixing) //����ũ�� ���� ������
                {
                    interactionButton = false;
                    if (tag == "SoyHot") //���� ��� ������
                    {

                        plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);
                    }
                }
                else if (interactionButton && HoldSomething && tag == "ItemAnother") //���� ���������
                {
                    interactionButton = false;
                    hit.collider.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    itemState = 0;
                    Ground();

                }


                highlight = hit.transform;                                            //���⼭���� �ƿ����� �ڵ�
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
                }                                                                                  //�������
            }
            else if (tag == "SoyHot")
            {
                nameString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                interationString.StringReference.TableEntryReference = "item_key";
                if (interactionButton && HoldSomething && itemState == 2) //�տ� �ҹ�� ��� ������
                {
                    interactionButton = false;
                    plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);

                }
                else if (interactionButton && mixing) //����ũ�� ���� ������
                {
                    interactionButton = false;
                    plateObj.GetComponent<Bowl>().Pickup(hit.collider.gameObject.GetComponent<ItemId>().id);

                }

            }
            else if (tag == "SceneTrans") //�� �̵� �±׽�
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "quit_key";


                if (interactionButton)
                {
                    interactionButton = false;
                    warningPanel.SetActive(true);
                    Time.timeScale = 0; //�ð� ���߱�
                    //��� ����
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
            else      //�� ����
            {
                highlight = null;
                nameString.StringReference = null;
                text.text = ""; //�̸� �ؽ�Ʈ ����
                text.enabled = false;
                interactionText.enabled = false;
            }
        }        
        /*else //�浹 �������� ����
        {
            nameString.StringReference = null;
            text.text = "";
            text.enabled = false;
            interactionText.enabled = false;
        }*/



        else if (Physics.Raycast(ray, out hit, 2f, layermaskPickup))  //�ҹ�� ���� �ݱ��
        {
            text.enabled = true;
            interactionText.enabled = true;
            if (itemState == 2 && HoldSomething)
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.transform.childCount == 0) //����ũ�� �ڽ� ������, �� ���� �ȿö���������
                {
                    interactionButton = false;//��������
                    Transform childTransform = HandPoint.GetChild(0);
                    FireBowl firebowl = childTransform.GetComponent<FireBowl>();
                    if (hit.collider.tag == "Bottle" && firebowl == null) //���� ����̰� �������������
                    {
                        return;
                    }
                    SoundManager.instance.PlaySound2D("plateDown"); //���� �Ҹ� ���

                    itemState = 0;
                    HoldSomething = false;
                    childTransform.transform.SetParent(hit.collider.gameObject.transform);
                    childTransform.transform.localPosition = childTransform.GetComponent<Bowl>().savedPosition;
                    childTransform.localRotation = childTransform.GetComponent<Bowl>().savedRotation;
                    childTransform.localScale = childTransform.GetComponent<Bowl>().savedScale;

                    if (hit.collider.gameObject == mixDesk)                                                 //���� ����ũ���
                    {
                        mixing = true;
                    }
                    else
                    {
                        PlateObjChange();
                    }


                    if (hit.collider.tag == "Bottle" && firebowl != null) //���� �ҹ����
                    {
                        firebowl.IsCook = true;
                    }
                }
            }
            else if (itemState == 1 && HoldSomething && hit.collider.gameObject.transform.childCount == 1)
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //����ũ�� �ڽ� ������, �� ���� �ȿö���������
                {
                    interactionButton = false;//��������
                    GameObject child = hit.collider.gameObject.transform.GetChild(0).gameObject;
                    Bowl bowl = child.GetComponent<Bowl>();
                    if (bowl != null && (id == 2 || id == 3)) //���� ����̰� �������������
                    {
                        bowl.Pickup(id-2); //�� �ֱ�
                        Ground(); //equip false �����
                        itemState = 0;
                    }
                }
            }
            else if (itemState == 5 && HoldSomething && hit.collider.tag == "Shop") //�� �̶��
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.transform.childCount == 0) //����ũ�� �ڽ� ������, �� ���� �ȿö���������
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
            else if (itemState == 0 && !HoldSomething && hit.collider.gameObject.transform.childCount != 0) //�տ� �ϰ͵� ���� �� �ö�������
            {                                                                                               //�ٽ� �ݱ�
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //��ư ������
                {
                    interactionButton = false;
                    Transform childTransform = hit.collider.gameObject.transform.GetChild(0);

                    if (childTransform.gameObject.GetComponent<ItemId>() != null &&
                        childTransform.gameObject.GetComponent<ItemId>().id == 11)//�ҹ���
                    {
                        if (childTransform.gameObject.GetComponent<FireBowl>().isBurning) //���� �ҳ���������
                        {
                            return;
                        }
                        // ������Ʈ�� ��ġ�� ȸ���� �����մϴ�.
                        childTransform.SetParent(HandPoint);
                        childTransform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                        childTransform.localRotation = Quaternion.Euler(240, 0, 20);
                        childTransform.localScale = new Vector3(0.5f, 0.7f, 0.5f);
                        childTransform.gameObject.GetComponent<FireBowl>().IsCook = false; //�丮 ���߱�
                        plateObj = childTransform.gameObject;
                        itemState = 2;
                    }
                    else if(childTransform.gameObject.GetComponent<ItemId>() != null &&
                        childTransform.gameObject.GetComponent<ItemId>().id == 10) //����̸�
                    {
                        childTransform.SetParent(HandPoint);
                        childTransform.localPosition = new Vector3(-0.06f, 0.04f, -0.128f);
                        childTransform.localRotation = Quaternion.Euler(240, 0, 20);
                        childTransform.localScale = new Vector3(0.7f, 1f, 0.7f);
                        plateObj = childTransform.gameObject;
                        itemState = 2;
                    }
                    else if (hit.collider.gameObject.GetComponent<ItemId>() != null && 
                        hit.collider.gameObject.GetComponent<ItemId>().id == 20)//Ŀ�Ǹ�
                    {
                        childTransform.SetParent(HandPoint);
                        // ������Ʈ�� ��ġ�� ȸ���� �����մϴ�.
                        childTransform.transform.localPosition = new Vector3(0.1f, 0.12f, 0f);
                        childTransform.transform.localRotation = Quaternion.Euler(-211, 128, -38);
                        childTransform.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        plateObj = childTransform.gameObject;
                        itemState = 2;

                        hit.collider.gameObject.GetComponentInParent<TableCoffee>().OffCoffee();
                    }
                    else                                                        //�����̸�
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

                // �ڽ��� �ִ��� Ȯ��
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
        else if (Physics.Raycast(ray, out hit, 2f, layermaskCookingPot))  //����뵵
        {
            text.enabled = true;
            interactionText.enabled = true;
            if (itemState == 1 && HoldSomething && (id == 0 || id == 1))
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //����ũ�� �ڽ� ������, �� ���� �ȿö���������
                {
                    interactionButton = false;
                    //��������
                    itemState = 0;
                    HoldSomething = false;
                    hit.collider.gameObject.transform.GetChild(0).GetComponent<CookingPot>().Pickup(id);
                    Ground(); //������ ��������
                }
            }

            else if (itemState == 0 && !HoldSomething) //�տ� �ϰ͵� ���� �� �ö�������
            {                                                                                               //�ٽ� �ݱ�
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton) //��ư ������
                {
                    interactionButton = false;
                    Transform childTransform = hit.collider.gameObject.transform.GetChild(0);
                    if (childTransform.gameObject.GetComponent<CookingPot>().isBurning || childTransform.gameObject.GetComponent<CookingPot>().IsCook == false) 
                    //���� �ҳ����ְų� �丮���� �ƴϸ�
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

        else if (Physics.Raycast(ray, out hit, 2f, layermaskTable))  //���̺�뵵
        {
            text.enabled = true;
            interactionText.enabled = true;

            if (isExtingush) //���� ��ȭ�� ���̺� �Ѹ���
            {
                hit.collider.gameObject.GetComponent<TableObjectBbq>().Angry(); //ȭ��
                return;
            }

            if (itemState == 5 && HandPoint.GetChild(0).gameObject.GetComponent<CharcoalBowl>().BeCooked) //�� ����ְ� �����Ÿ�
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableCharcoal())
                //üũ���̺� Ʈ���϶�
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("dinner-plate"); //���� �Ҹ� ���
                    Destroy(HandPoint.GetChild(0).gameObject);
                    itemState = 0;
                    PlateGround(); //Ȧ����� ����
                }
            }
            else if (itemState == 2 && HoldSomething) //��� �����Ͻ�
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.GetComponent<TableObjectBbq>() != null &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().isCharcoal &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().SubOn)
                //�� �ö���������, �׸��� ����޴� ����������
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //���� �Ҹ� ���
                    bool isCooked = false, isFirebowl = false, isBurnt = false; //��������� ����, �ҹ������ ����, ������ ����
                    GameObject bowl = HandPoint.GetChild(0).gameObject;
                    if (bowl.GetComponent<FireBowl>() != null) //���� �ҹ����
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
                    if (hit.collider.gameObject.GetComponent<TableObjectBbq>() != null) //���̺������Ʈbbq���
                    {
                        hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableSub(a, isCooked, isFirebowl, isBurnt);
                    }

                    /*  if (hit.collider.gameObject.GetComponent<TableCoffee>() != null) //���̺�Ŀ�Ƕ��
                      {
                         bool mix = bowl.GetComponent<CoffeeBowl>().GetMix();
                         hit.collider.gameObject.GetComponent<TableCoffee>().SetCoffee(a, mix, true);
                      }*/

                    bowl.transform.SetParent(hit.collider.gameObject.transform.GetChild(0));
                    bowl.transform.localPosition = bowl.GetComponent<Bowl>().savedPosition;
                    bowl.transform.localRotation = bowl.GetComponent<Bowl>().savedRotation;
                    bowl.transform.localScale = bowl.GetComponent<Bowl>().savedScale;

                    itemState = 0;
                    PlateGround(); //Ȧ����� ����
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
                    PlateGround(); //Ȧ����� ����
                }
            }
            else if (itemState == 4 && HoldSomething) //��� �����϶�
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().isCharcoal &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableMain(id))
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //���� �Ҹ� ���

                    hit.collider.gameObject.GetComponent<TableObjectBbq>().meatObject[id].SetActive(true);
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().fxOther.SetActive(true);
                    Destroy(HandPoint.GetChild(0).gameObject);
                    PlateGround();
                    itemState = 0;
                }
            }
            else if (itemState == 1 && HoldSomething && hit.collider.gameObject.GetComponent<TableObjectBbq>() != null) //������Ͻ�
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton &&
                    hit.collider.gameObject.GetComponent<TableObjectBbq>().CheckTableDrink(id - 5))
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //���� �Ҹ� ���

                    hit.collider.gameObject.GetComponent<TableObjectBbq>().drinkObject[id - 5].SetActive(true);
                    Ground();
                    itemState = 0;
                }
            }
            else if (itemState == 1 && HoldSomething && hit.collider.gameObject.GetComponent<TableCoffee>() != null) //ī�信�� ���̵���������
            {
                nameString.StringReference = null;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton)
                {
                    interactionButton = false;
                    SoundManager.instance.PlaySound2D("plateDown"); //���� �Ҹ� ���

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
                if (!HoldSomething) //�տ� ���� ������
                {
                    hit.collider.gameObject.GetComponent<TableObject>().IsClean = true;
                    lastHitTargetT = hit.collider.gameObject.GetComponent<TableObject>();

                    animatorFirst.SetBool("isMix", true);
                    objectToEquipF[9].SetActive(true); //�ɷ� Ȱ��ȭ
                } //���� ������
                else
                {
                    //�޼��� ����
                }

            }
            // ��ư ������ �� ������ ä��� ����
            else if (!interactionButton)
            {
                interactionButton = false;
                hit.collider.gameObject.GetComponent<TableObject>().IsClean = false;

                animatorFirst.SetBool("isMix", false);
                objectToEquipF[9].SetActive(false); //�ɷ� ��Ȱ��ȭ
            }

        }
        else
        {
            if (lastHitTargetT != null) //������ ������Ʈ �� �ƴϸ�
            {
                lastHitTargetT.IsClean = false;

                animatorFirst.SetBool("isMix", false);
                objectToEquipF[9].SetActive(false); //�ɷ� ��Ȱ��ȭ
            }
            text.text = "";
            text.enabled = false;
            interactionText.enabled = false;
        }
 
    }
    private void PlateObjChange() //���� �ҹ�� �׸� ���� �Լ�
    {
        if (mixing) //���� ����ũ�� �ҹ�� ������
        {
            plateObj = mixDesk.transform.GetChild(0).gameObject; //�� �ҹ��� ���� �ٲٱ�
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
    public void Ground()  //��� ������ ���������� ���� �Լ�
    {
        objectToEquipF[id].SetActive(false);
        
        Invoke(nameof(RTF), 0.05f);
    }
    public void PlateGround()  //���� ���������� �� ���� �Լ�
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
            if(itemState ==1 || itemState == 3) //���� ������ó�� ����ΰ͵�
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
