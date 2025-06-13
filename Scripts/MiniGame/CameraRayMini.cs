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
    [SerializeField] private GameObject platePrefab; //접시 프리팹

    ThrowingMini throwmini;

    public bool interactionButton,useButton;

    [SerializeField] private LayerMask layermask;
    [SerializeField] private LayerMask layermask2;
    [SerializeField] private LayerMask layermask3; //그냥 플레이어 빼고 다 전체용

    [SerializeField] private LayerMask layermaskTable; //테이블용
    [SerializeField] private LayerMask layermaskDirtyTable; //더러운테이블용
    [SerializeField] private Transform HandPoint; //접시 쥐는 손 위치


    private Transform highlight; //아웃라인 강조 용도

    private int id;

    public UnityEvent <int> useSauce;


    [SerializeField]
    private int[] requireId; //접시에 올라갈 수 있는 아이템 배열


    private float lastNotificationTime = -1f; // 마지막 알림이 뜬 시간
    public float notificationCooldown = 1f; // 알림 쿨타임 (1초)

    [Header("Panel")]
    [SerializeField] private GameObject warningPanel; //게임 나갈거냐는 문구

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

                if (interactionButton && throwmini.readyToThrow) //손에 뭐 있을 때
                {
                    interactionButton = false;
                    throwmini.Ground();
                    SoundManager.instance.PlaySound2D("trashcan");
                    if (plateObj != null)
                    {
                        DestroyPlate(); //접시 삭제
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
                {  //누르면 줍기
                    interactionButton = false;
                    if (tag == "SoyHot")
                    {
                        useSauce.Invoke(id);
                    }
                    else if (tag == "Item")
                    {
                        if(id < 50)//만약 음료수 종류라면)
                        {
                            GameObject projectile = Instantiate(platePrefab);
                            projectile.GetComponent<MeshCollider>().enabled = false; //콜라이더 플레이어 충돌 못하게 끄기
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

                        if (tag == "ItemAnother") //튀김기에서 꺼낼때만
                        {
                            
                            Collider[] colliders = hit.collider.gameObject.GetComponentsInChildren<Collider>(); // 모든 콜라이더 컴포넌트를 찾음
                            foreach (Collider collider in colliders)
                            {
                                collider.enabled = false;  // 각 콜라이더의 활성화 상태를 false로 설정
                            }

                        }
                        

                        GameObject projectile = Instantiate(platePrefab);
                        projectile.GetComponent<MeshCollider>().enabled = false; //콜라이더 플레이어 충돌 못하게 끄기
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
                        hit.collider.gameObject.GetComponent<MeshCollider>().enabled = false; //콜라이더 플레이어 충돌 못하게 끄기
                        hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                        hit.collider.gameObject.transform.SetParent(HandPoint);
                        hit.collider.gameObject.transform.localPosition = new Vector3(-0.15f, 0, -0.12f);
                        hit.collider.gameObject.transform.localRotation = Quaternion.Euler(220, 0, 0);
                        plateObj = hit.collider.gameObject;

                        plateObj.GetComponent<ItemId>().id = id;
                        throwmini.Equips(id);
                    }

                }
                

                highlight = hit.transform;    //여기서부터 아웃라인 코드
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
            else if (tag == "SceneTrans") //씬 이동 태그시
            {
                text.enabled = false;
                interationString.StringReference.TableEntryReference = "quit_key";

                if (interactionButton)
                {
                    interactionButton = false;
                    warningPanel.SetActive(true);
                    Time.timeScale = 0; //시간 멈추기
                    //경고문 띄우기
                }

            }
            else      //다 끄기
            {
                highlight = null;
                text.enabled = false;
            }
        }        
        
        else if (Physics.Raycast(ray, out hit, 2f, layermask))  //튀김기 사용
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


        else if (Physics.Raycast(ray, out hit, 2f, layermask2)) //주방 책상에 접시 내려놓는거
        {
            if (throwmini.readyToThrow && throwmini.id_ < 15)
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "put_key";
                if (interactionButton && hit.collider.gameObject.transform.childCount == 0) //데스크에 자식 없을때, 즉 접시 안올라져있을때
                {
                    interactionButton = false;
                    throwmini.readyToThrow = false;
                    Transform childTransform = HandPoint.GetChild(0);
                    childTransform.transform.SetParent(hit.collider.gameObject.transform);
                    childTransform.transform.localPosition = new Vector3(0,0.1f,0);
                    childTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    childTransform.transform.localScale = new Vector3(1.5f, 100, 2f);
                    childTransform.gameObject.GetComponent<MeshCollider>().enabled = true; //콜라이더 다시 키기
                    childTransform.gameObject.GetComponent<BoxCollider>().enabled = true;
                    throwmini.PlateGround(); //readytothrow false 실행
                }
            }
            else
            {
                interactionText.enabled = false;
            }

        }
        else if (Physics.Raycast(ray, out hit, 2f, layermaskTable)) //손님 테이블에 음식 내려놓을때
        {
            if (throwmini.readyToThrow && throwmini.id_ < 15) //접시 들고있으면
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "put_key";

                if (interactionButton) 
                {
                    interactionButton = false;
                    if (hit.collider.gameObject.GetComponent<TableObject>().CheckTablePlate(HandPoint.GetChild(0).GetComponent<ItemId>().id))
                    {
                        SoundManager.instance.PlaySound2D("plateDown"); //딸깍 소리 재생
                        Transform childTransform = HandPoint.GetChild(0);
                        if (hit.collider.gameObject.transform.GetChild(0).childCount == 0) //첫번째 접시 칸에 비었으면
                        {
                            childTransform.transform.SetParent(hit.collider.gameObject.transform.GetChild(0));
                        }
                        else //첫번째 접시칸 차있으면
                        {
                            childTransform.transform.SetParent(hit.collider.gameObject.transform.GetChild(1));
                        }
                        childTransform.transform.localPosition = new Vector3(0, 0.1f, 0);
                        childTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        childTransform.transform.localScale = new Vector3(1.5f, 100, 2f);
                        throwmini.PlateGround(); //readytothrow false 실행
                        Debug.Log("plate good");
                    }
                    else
                    {
                        //노티스 생성
                        ChiTableManager.instance.NoticeCreate("NotOrderThis");
                    }
                }   
            }
            else if (throwmini.readyToThrow && throwmini.id_ >= 50) //음료수 들고있을때
            {
                interactionText.enabled = true;
                interationString.StringReference.TableEntryReference = "put_key";

                if (interactionButton && hit.collider.gameObject.GetComponent<TableObject>().CheckTableDrink(throwmini.id_)) //데스크에 자식 없을때, 즉 접시 안올라져있을때
                {
                    SoundManager.instance.PlaySound2D("plateDown"); //딸깍 소리 재생
                    hit.collider.gameObject.GetComponent<TableObject>().drinkObject[throwmini.id_ - 50].SetActive(true);
                    interactionButton = false;
                    throwmini.Ground(); //readytothrow false 실행
                    Debug.Log("drink good");
                }
            }
            else
            {
                interactionText.enabled = false;
            }
        }
        else if (Physics.Raycast(ray, out hit, 2f, layermaskDirtyTable)) //책상 치우기용
        {
            interactionText.enabled = true;
            interationString.StringReference.TableEntryReference = "clean_key";

            if (interactionButton)
            {
                if (!throwmini.readyToThrow) //손에 물건 없으면
                {
                    hit.collider.gameObject.GetComponent<TableObject>().IsClean = true;
                    //throwmini.WashDishOn(); //치우는 애니메이션용
                    lastHitTargetT = hit.collider.gameObject.GetComponent<TableObject>();
                } //물건 있으면
                else
                {
                    if (ShouldShowNotification()) // 알림 조건이 맞으면
                    {
                        //메세지 띄우기
                        ChiTableManager.instance.NoticeCreate("PutDownThings");
                        lastNotificationTime = Time.time; // 마지막 알림 시간 업데이트
                    }
                }

            }

            // 버튼 떼졌을 때 게이지 채우기 중지
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

            if (lastHitTargetT != null) //설거지 컴포넌트 눌 아니면
            {
                lastHitTargetT.IsClean = false;
            }      
           
        }
    }

    public void DestroyPlate()
    {
       // Debug.Log("접시 버리미미미");
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
