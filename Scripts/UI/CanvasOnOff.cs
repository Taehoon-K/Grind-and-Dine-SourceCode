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
    private GameObject[] panels; //�κ�, ���� �� ��������
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
    private bool tabOn; //�� �����ִ���
    [SerializeField] private Quaternion savedRotation;

    public UnityEvent onInventory;
    public UnityEvent offInventory;
    public UnityEvent render; //�� �鶧 �������Ұ͵�

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
        if(JailManager.instance != null) //���� �����̸� ��Ŵ
        {
            return;
        }

        if (tabDown&&!playingAni)
        {
            tabDown = false;
            
            if (!uion)  //ui������������ �ִϸ��̼ǽ���
            {
                playingAni = true;  //Ű����
                panelNum = 0;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                //StartCoroutine(OnInvenReady());
                uion = true;
            }
            else  //ui����������
            {
                if (panelNum!=0) //�κ� �г� �ƴҽ�
                {
                    
                    OnPanel(0);
                }
                else  //�κ� �г��Ͻ�, ����
                {
                    OffPanel();
                }
                
            }

        }
        if (rDown && !playingAni)
        {
            rDown = false;

            if (!uion)  //����â ����
            {
                playingAni = true;  //Ű����
                panelNum = 1;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                //StartCoroutine(OnInvenReady());
                uion = true;
            }
            else  //ui����������
            {
                if (panelNum != 1) //���赵 �г� �ƴҽ�
                {
                    
                    OnPanel(1);
                }
                else  //���赵 �г��Ͻ�, ����
                {
                    OffPanel();
                }

            }

        }
        if (kDown && !playingAni)
        {
            kDown = false;
            if (!uion)  //����â ����
            {
                playingAni = true;  //Ű����
                panelNum = 2;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                //StartCoroutine(OnInvenReady());
                uion = true;
            }
            else  //ui����������
            {
                if (panelNum != 2) //���赵 �г� �ƴҽ�
                {

                    OnPanel(2);
                }
                else  //���赵 �г��Ͻ�, ����
                {
                    OffPanel();
                }

            }

        }
        if (mDown && !playingAni)
        {
            mDown = false;
            if (!uion)  //����â ����
            {
                playingAni = true;  //Ű����
                panelNum = 3;
                animatorFirst.SetTrigger("isWatch");
                onInventory.Invoke();
                uion = true;
            }
            else  //ui����������
            {
                if (panelNum != 3) //�� �г� �ƴҽ�
                {

                    OnPanel(3);
                }
                else  //�� �г��Ͻ�, ����
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
            panel.SetActive(false); //�г� �� ����
        }
        panels[i].SetActive(true);

        SoundManager.instance.PlaySound2D("texting");
    }

    private void OffPanel()
    {
        Debug.Log("�c ������������");
        UIManager.instance.CurrentUIState = UIState.None;
        playingAni = true;
        uion = false;
        Invoke(nameof(OffInven), watchInvoke); //�ȳ����� ����
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
    public void WatchOn() //�ִϸ��̼ǿ��� ȣ�� �Լ�
    {
        Debug.Log("WatchOn");
        OnPanel(panelNum);
        relationM.RenderRelation(); //�ӽ�
        /*if (panels[4].activeSelf) //���� �״µ� �ݷ����г��̸�
        {
            collecPanel.Render(); //�ݷ��� �г� ����
        }*/
        UIManager.instance.CurrentUIState = UIState.InventoryUI;

        render.Invoke();
        playingAni = false; //���� �Ϸ�
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
