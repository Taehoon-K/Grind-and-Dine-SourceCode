using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.InputSystem;

public class CameraRayMini : MonoBehaviour
{
    [SerializeField]
    GameObject maincamera;
    Camera camera;
    [SerializeField]
    TextMeshProUGUI text, interactionText;
    [SerializeField]
    private LocalizeStringEvent nameLocalizeString;
    [SerializeField]
    private LocalizeStringEvent interationString;

    private TableObject lastHitTargetT;
    //private ColliderSauce lastHitTargetSauce;
    public GameObject plateObj;
    [SerializeField] private GameObject platePrefab; //���� ������

    ThrowingMini throwmini;

    public bool interactionButton,useButton;

    [SerializeField] private LayerMask layermask;
    [SerializeField] private LayerMask layermask2;
    [SerializeField] private LayerMask layermask3; //�׳� �÷��̾� ���� �� ��ü��

    [SerializeField] private LayerMask layermaskTable; //���̺��
    [SerializeField] private LayerMask layermaskDirtyTable; //���������̺��
    [SerializeField] private Transform HandPoint; //���� ��� �� ��ġ


    private Transform highlight; //�ƿ����� ���� �뵵

    private int id;

    public UnityEvent <int> useSauce;


    [SerializeField]
    private int[] requireId; //���ÿ� �ö� �� �ִ� ������ �迭


    private float lastNotificationTime = -1f; // ������ �˸��� �� �ð�
    public float notificationCooldown = 1f; // �˸� ��Ÿ�� (1��)

    [Header("Panel")]
    [SerializeField] private GameObject warningPanel; //���� �����ųĴ� ����

    void Start()
    {
        //maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        camera = maincamera.GetComponent<Camera>();
        throwmini = GetComponent<ThrowingMini>();

    }

    void Update()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;


        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<OutlineReal>().enabled = false;
            highlight = null;
        }


        if (Physics.Raycast(ray, out hit,2f, layermask3))
        {
            string tag = hit.collider.tag;
            //Debug.Log(tag);

            /*if (tag == "Npc")
            {
                nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                text.enabled = true;
                talkText.enabled = true;
                takeText.enabled = false;
                throwawayText.enabled = false;


                highlight = null;
            }
            else*/

            interactionText.enabled = true;
            if (tag == "TrashCan")
            {
                nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                text.enabled = true;
                interationString.StringReference.TableEntryReference = "throwaway_key";

                if (interactionButton && throwmini.readyToThrow) //�տ� �� ���� ��
                {
                    interactionButton = false;
                    throwmini.Ground();
                    SoundManager.instance.PlaySound2D("trashcan");
                    if (plateObj != null)
                    {
                        DestroyPlate(); //���� ����
                    }
                }
                highlight = null;
            }
            else if (tag == "ItemActive" || tag == "Item" || tag == "ItemAnother" || tag == "ItemPlate" || tag == "SoyHot")
            {
                nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                id = hit.collider.gameObject.GetComponent<ItemId>().id;                        
                text.enabled = true;
                interationString.StringReference.TableEntryReference = "item_key";

                if (interactionButton && !throwmini.readyToThrow)
                {  //������ �ݱ�
                    interactionButton = false;
                    if (tag == "SoyHot")
                    {
                        useSauce.Invoke(id);
                    }
                    else if (tag == "Item")
                    {
                        if(id < 50)//���� ����� �������)
                        {
                            GameObject projectile = Instantiate(platePrefab);
                            projectile.GetComponent<MeshCollider>().enabled = false; //�ݶ��̴� �÷��̾� �浹 ���ϰ� ����
                            projectile.GetComponent<BoxCollider>().enabled = false;
                            projectile.transform.SetParent(HandPoint);
                            projectile.transform.localPosition = new Vector3(-0.15f, 0, -0.12f);
                            projectile.transform.localRotation = Quaternion.Euler(220, 0, 0);

                            plateObj = projectile;
                            plateObj.GetComponent<ItemId>().id = id;
                            plateObj.GetComponent<ColliderPlate>().realItem[id].SetActive(true);
                            
                        }
                        else
                        {
                            
                        }
                        throwmini.Equips(id);
                    }
                    else if (tag == "ItemAnother" || tag == "ItemActive")
                    {
                        hit.collider.gameObject.SetActive(false);

                        if (tag == "ItemAnother") //Ƣ��⿡�� ��������
                        {
                            
                            Collider[] colliders = hit.collider.gameObject.GetComponentsInChildren<Collider>(); // ��� �ݶ��̴� ������Ʈ�� ã��
                            foreach (Collider collider in colliders)
                            {
                                collider.enabled = false;  // �� �ݶ��̴��� Ȱ��ȭ ���¸� false�� ����
                            }

                        }
                        

                        GameObject projectile = Instantiate(platePrefab);
                        projectile.GetComponent<MeshCollider>().enabled = false; //�ݶ��̴� �÷��̾� �浹 ���ϰ� ����
                        projectile.GetComponent<BoxCollider>().enabled = false;
                        projectile.transform.SetParent(HandPoint);
                        projectile.transform.localPosition = new Vector3(-0.15f, 0, -0.12f);
                        projectile.transform.localRotation = Quaternion.Euler(220, 0, 0);

                        plateObj = projectile;
                        plateObj.GetComponent<ItemId>().id = id;
                        plateObj.GetComponent<ColliderPlate>().realItem[id].SetActive(true);
                        throwmini.Equips(id);
                    }
                    else if (tag == "ItemPlate")
                    {
                        hit.collider.gameObject.GetComponent<MeshCollider>().enabled = false; //�ݶ��̴� �÷��̾� �浹 ���ϰ� ����
                        hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                        hit.collider.gameObject.transform.SetParent(HandPoint);
                        hit.collider.gameObject.transform.localPosition = new Vector3(-0.15f, 0, -0.12f);
                        hit.collider.gameObject.transform.localRotation = Quaternion.Euler(220, 0, 0);
                        plateObj = hit.collider.gameObject;

                        plateObj.GetComponent<ItemId>().id = id;
                        throwmini.Equips(id);
                    }

                }
                

                highlight = hit.transform;    //���⼭���� �ƿ����� �ڵ�
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
            else if (tag == "SceneTrans") //�� �̵� �±׽�
            {
                text.enabled = false;
                interationString.StringReference.TableEntryReference = "quit_key";

                if (interactionButton)
                {
                    interactionButton = false;
                    warningPanel.SetActive(true);
                    Time.timeScale = 0; //�ð� ���߱�
                    //��� ����
                }

            }
            else      //�� ����
            {
                highlight = null;
                text.enabled = false;
            }
        }        
        
        else if (Physics.Raycast(ray, out hit, 2f, layermask))  //Ƣ��� ���
        {

            if (hit.collider.gameObject.GetComponent<ColliderInter>().CheckActive())
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "use_key";

                if (useButton)
                {
                    useButton = false;
                    hit.collider.gameObject.GetComponent<ColliderInter>().OnMachine();
                }
            }
            else if(throwmini.readyToThrow && throwmini.id_ < 18)
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "put_key";
            }
            else
            {
                interactionText.enabled = false;
            }

        }


        else if (Physics.Raycast(ray, out hit, 2f, layermask2)) //�ֹ� å�� ���� �������°�
        {
            if (throwmini.readyToThrow && throwmini.id_ < 15)
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.transform.childCount == 0) //����ũ�� �ڽ� ������, �� ���� �ȿö���������
                {
                    interactionButton = false;
                    throwmini.readyToThrow = false;
                    Transform childTransform = HandPoint.GetChild(0);
                    childTransform.transform.SetParent(hit.collider.gameObject.transform);
                    childTransform.transform.localPosition = new Vector3(0,0.1f,0);
                    childTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    childTransform.transform.localScale = new Vector3(1.5f, 100, 2f);
                    childTransform.gameObject.GetComponent<MeshCollider>().enabled = true; //�ݶ��̴� �ٽ� Ű��
                    childTransform.gameObject.GetComponent<BoxCollider>().enabled = true;
                    throwmini.PlateGround(); //readytothrow false ����
                }
            }
            else
            {
                interactionText.enabled = false;
            }

        }
        else if (Physics.Raycast(ray, out hit, 2f, layermaskTable)) //�մ� ���̺� ���� ����������
        {
            if (throwmini.readyToThrow && throwmini.id_ < 15) //���� ���������
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "put_key";

                if (interactionButton) 
                {
                    interactionButton = false;
                    if (hit.collider.gameObject.GetComponent<TableObject>().CheckTablePlate(HandPoint.GetChild(0).GetComponent<ItemId>().id))
                    {
                        SoundManager.instance.PlaySound2D("plateDown"); //���� �Ҹ� ���
                        Transform childTransform = HandPoint.GetChild(0);
                        if (hit.collider.gameObject.transform.GetChild(0).childCount == 0) //ù��° ���� ĭ�� �������
                        {
                            childTransform.transform.SetParent(hit.collider.gameObject.transform.GetChild(0));
                        }
                        else //ù��° ����ĭ ��������
                        {
                            childTransform.transform.SetParent(hit.collider.gameObject.transform.GetChild(1));
                        }
                        childTransform.transform.localPosition = new Vector3(0, 0.1f, 0);
                        childTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        childTransform.transform.localScale = new Vector3(1.5f, 100, 2f);
                        throwmini.PlateGround(); //readytothrow false ����
                        Debug.Log("plate good");
                    }
                    else
                    {
                        //��Ƽ�� ����
                        ChiTableManager.instance.NoticeCreate("NotOrderThis");
                    }
                }   
            }
            else if (throwmini.readyToThrow && throwmini.id_ >= 50) //����� ���������
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "put_key";

                if (interactionButton && hit.collider.gameObject.GetComponent<TableObject>().CheckTableDrink(throwmini.id_)) //����ũ�� �ڽ� ������, �� ���� �ȿö���������
                {
                    SoundManager.instance.PlaySound2D("plateDown"); //���� �Ҹ� ���
                    hit.collider.gameObject.GetComponent<TableObject>().drinkObject[throwmini.id_ - 50].SetActive(true);
                    interactionButton = false;
                    throwmini.Ground(); //readytothrow false ����
                    Debug.Log("drink good");
                }
            }
            else
            {
                interactionText.enabled = false;
            }
        }
        else if (Physics.Raycast(ray, out hit, 2f, layermaskDirtyTable)) //å�� ġ����
        {
            interactionText.enabled = true;
            interationString.StringReference.TableEntryReference = "clean_key";

            if (interactionButton)
            {
                if (!throwmini.readyToThrow) //�տ� ���� ������
                {
                    hit.collider.gameObject.GetComponent<TableObject>().IsClean = true;
                    //throwmini.WashDishOn(); //ġ��� �ִϸ��̼ǿ�
                    lastHitTargetT = hit.collider.gameObject.GetComponent<TableObject>();
                } //���� ������
                else
                {
                    if (ShouldShowNotification()) // �˸� ������ ������
                    {
                        //�޼��� ����
                        ChiTableManager.instance.NoticeCreate("PutDownThings");
                        lastNotificationTime = Time.time; // ������ �˸� �ð� ������Ʈ
                    }
                }

            }

            // ��ư ������ �� ������ ä��� ����
            if (!interactionButton)
            {
                hit.collider.gameObject.GetComponent<TableObject>().IsClean = false;
               // throwmini.WashDishOff();
            }
        }
        else
        {
            interactionText.enabled = false;
            text.enabled = false;

            if (lastHitTargetT != null) //������ ������Ʈ �� �ƴϸ�
            {
                lastHitTargetT.IsClean = false;
            }      
           
        }
    }

    public void DestroyPlate()
    {
       // Debug.Log("���� �����̹̹�");
        Destroy(plateObj);
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
    public void OnUse(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                useButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                useButton = false;
                break;
        }
    }

    bool ShouldShowNotification()
    {
        return Time.time - lastNotificationTime >= notificationCooldown;
    }

    private int Transform(int n)
    {
        switch (n)
        {
            case 1:
                return 0;
            case 2:
                return 1;
            case 4:
                return 2;
            case 5:
                return 3;
            case 7:
                return 4;
            case 8:
                return 5;
            case 10:
                return 6;
            case 11:
                return 7;
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
                return n - 4;
            default:
                return 0;
        }
    }
}
