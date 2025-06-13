using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UIManager;

public class CanvasOnOff : MonoBehaviour
{
    bool tabDown,rDown,kDown,mDown;
    [SerializeField]
    private GameObject[] panels; //인벤, 스탯 등 여러개들
    [SerializeField]
    private RelaionshipListingManager relationM;
   /* [SerializeField]
    private CollectionPanel collecPanel;*/

    [SerializeField]
    private float watchInvoke;
    //private Canvas canva;
    private Animator animatorFirst;
    private bool uion;
    private bool playingAni=false;
    private bool tabOn; //탭 켜져있는지
    [SerializeField] private Quaternion savedRotation;

    public UnityEvent onInventory;
    public UnityEvent offInventory;
    public UnityEvent render; //팔 들때 렌더링할것들

    private int panelNum;

    [SerializeField] private bool isTutorial;
    private void Awake()
    {
        animatorFirst = GameObject.Find("FirstPArms").GetComponent<Animator>();
    }
    private void Start()
    {
        //relationM = panels[1].GetComponent<RelaionshipListingManager>();
        //canva = canvas.GetComponent<Canvas>();
        uion = false;
        foreach(GameObject a in panels)
        {
            a.SetActive(false);
        }

        if (!isTutorial)
        {
            savedRotation = transform.rotation;
        }
    }
    void Update()
    {
        if(TutorialManager.instance != null && !TutorialManager.instance.eatWatch)
        {
            return;
        }
        if(JailManager.instance != null) //만약 감옥이면 못킴
        {
            return;
        }

        if (tabDown&&!playingAni)
        {
            tabDown = false;
            
            if (!uion)  //ui안켜져있을시 애니메이션실행
            {
                playingAni = true;  //키는중
                panelNum = 0;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                //StartCoroutine(OnInvenReady());
                uion = true;
            }
            else  //ui켜져있을시
            {
                if (panelNum!=0) //인벤 패널 아닐시
                {
                    
                    OnPanel(0);
                }
                else  //인벤 패널일시, 끄기
                {
                    OffPanel();
                }
                
            }

        }
        if (rDown && !playingAni)
        {
            rDown = false;

            if (!uion)  //상태창 열기
            {
                playingAni = true;  //키는중
                panelNum = 1;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                //StartCoroutine(OnInvenReady());
                uion = true;
            }
            else  //ui켜져있을시
            {
                if (panelNum != 1) //관계도 패널 아닐시
                {
                    
                    OnPanel(1);
                }
                else  //관계도 패널일시, 끄기
                {
                    OffPanel();
                }

            }

        }
        if (kDown && !playingAni)
        {
            kDown = false;
            if (!uion)  //상태창 열기
            {
                playingAni = true;  //키는중
                panelNum = 2;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                //StartCoroutine(OnInvenReady());
                uion = true;
            }
            else  //ui켜져있을시
            {
                if (panelNum != 2) //관계도 패널 아닐시
                {

                    OnPanel(2);
                }
                else  //관계도 패널일시, 끄기
                {
                    OffPanel();
                }

            }

        }
        if (mDown && !playingAni)
        {
            mDown = false;
            if (!uion)  //상태창 열기
            {
                playingAni = true;  //키는중
                panelNum = 3;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                uion = true;
            }
            else  //ui켜져있을시
            {
                if (panelNum != 3) //맵 패널 아닐시
                {

                    OnPanel(3);
                }
                else  //맵 패널일시, 끄기
                {
                    OffPanel();
                }
            }
        }

        tabDown = false;
        kDown = false;
        rDown = false;
        mDown = false;
    }
    public void OnPanel(int i)
    {
        panelNum = i;
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false); //패널 다 끄기
        }
        panels[i].SetActive(true);

        SoundManager.instance.PlaySound2D("texting");
    }

    private void OffPanel()
    {
        Debug.Log("팧 내림ㅁㅁㅁㅁ");
        UIManager.instance.CurrentUIState = UIState.None;
        playingAni = true;
        uion = false;
        Invoke(nameof(OffInven), watchInvoke); //팔내리기 실행
    }

    private void OffInven()
    {
        offInventory.Invoke();
        playingAni = false;

        if(TutorialManager.instance != null)
        {
            TutorialManager.instance.offInven = true;
        }
        transform.localRotation = savedRotation;
    }
    public void WatchOn() //애니메이션에서 호출 함수
    {
        Debug.Log("WatchOn");
        OnPanel(panelNum);
        relationM.RenderRelation(); //임시
        /*if (panels[4].activeSelf) //만약 켰는데 콜렉션패널이면
        {
            collecPanel.Render(); //콜렉션 패널 업뎃
        }*/
        UIManager.instance.CurrentUIState = UIState.InventoryUI;

        render.Invoke();
        playingAni = false; //동작 완료
    }

    public void OnTab(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                tabDown = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                tabDown = false;
                break;
        }
    }

    public void OnRelation(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                rDown = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                rDown = false;
                break;
        }
    }
    public void OnSKill(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                kDown = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                kDown = false;
                break;
        }
    }
    public void OnMap(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                mDown = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                mDown = false;
                break;
        }
    }
    public bool IsPlayingAni()
    {
        return playingAni;
    }
    public bool IsInvenOn()
    {
        return uion;
    }
}
