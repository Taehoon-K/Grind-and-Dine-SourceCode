using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using PP.InventorySystem;
using UnityEngine.InputSystem;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class CameraRayTutorial : MonoBehaviour
{
    GameObject maincamera;
    Camera camera;
    Kupa.Player player;
    PlayerYarn playeryarn;
    TextMeshProUGUI text, intercationText;//,takeText;

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

    private bool firstConver;
    void Start()
    {
        obj = GameObject.FindGameObjectWithTag("Interaction");
        player = FindObjectOfType<Kupa.Player>();
        playeryarn = FindObjectOfType<PlayerYarn>();

        text = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
        float distan;

        if (player.threePview == true) //3인칭이면 5 아니면 2f
        {
            distan = 5f;
        }
        else
        {
            distan = 2f;
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
                    text.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }
                else if (nameLocalizeString.gameObject != hit.collider.gameObject)
                {
                    text.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }
                //nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                /*text.enabled = true;
                intercationText.enabled = true;*/
                table.StringReference.TableEntryReference = "talk_key";

                //takeText.enabled = false;           
                if (interactionButton && player.isRun == false)
                {
                    interactionButton = false;
                    if (!firstConver)
                    {
                        UIManager.instance.NoticeDelete(); //wasd 지우기
                        playeryarn.CheckForNearbyNPC("Tutorial", hit.collider.transform); //얀 대화문 실행
                        hit.collider.tag = "Untagged"; //태그변경
                        firstConver = true;
                    }
                    else
                    {
                        playeryarn.CheckForNearbyNPC("Tutorial4", hit.collider.transform); //얀 대화문 실행
                    }
                    
                }
            }
            else if (tag == "Watch" || tag == "Item")
            {
                if (nameLocalizeString.StringReference == null) //아무 정보도 없으면
                {
                    text.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }
                else if (nameLocalizeString.gameObject != hit.collider.gameObject)
                {
                    text.enabled = true;
                    nameLocalizeString.StringReference = hit.collider.gameObject.GetComponent<NpcName>().Mini_name_key;
                }

                table.StringReference.TableEntryReference = "item_key";
                if (interactionButton && player.isRun == false)
                {  //주우면 인벤토리 add 추가
                    interactionButton = false;
                    if(tag == "Watch")
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
            else if (tag == "SceneTrans") //씬 이동 태그시
            {

                table.StringReference.TableEntryReference = "move_key";

                if (interactionButton)
                {
                    interactionButton = false;
                    Debug.Log("tutorial End");
                    TutorialManager.instance.Continue();
                }

            }

            else      //다 끄기
            {
                text.enabled = false;
                intercationText.enabled = false;
            }
            
        }
        else //충돌 안했을때 끄기
        {
            text.enabled = false;
            intercationText.enabled = false;
        }
        interactionButton = false;
        rightButton = false;
        leftButton = false;
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
}
