using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;


public class BrickMoveManager : ThridMiniManager
{
    public static BrickMoveManager instance = null;

    [Header("InGameStat")]
    [SerializeField] private int bricksCount; //���� ���� ����
    [SerializeField] private TextMeshProUGUI bricksCountText; //���� ���� ����
    public bool isNearBricks, isNearEnd;
    [SerializeField] private GameObject[] brickObjects; //���� ������Ʈ �迭

    [Header("Speed")]
    [SerializeField] private float carrySpeed = 1.2f; //�ʱ� �ȴ¼ӵ�
    [SerializeField] private float runSpeed;

    [Header("Audio")]
    [SerializeField] private AudioSource brickFallSound;

    #region privateField
    //bool interactionButton;
    private GameObject box;
    private int strength;
    #endregion
    public int BricksCount
    {
        get
        {
            return bricksCount; // �Ӽ� ���� ��ȯ
        }
        set
        {
            bricksCount = value;
            bricksCountText.text = "X " + bricksCount.ToString();
            StartCoroutine(UpdateBricksVisibility()); //���� ������Ʈ Ȱ��ȭ
        }
    }

    protected void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }
    protected override void Start()
    {
        base.Start();
        
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //�ð� �帣�°� ���߰��ϱ�
            stat = StatusManager.instance.GetStatus().Clone();
            strength = stat.level[0]; //�� ���� ��������
        }        
        //TimeManager.instance.RegisterTracker(this);
        FindBricks(); //���� ������Ʈ ã�Ƽ� �迭�� �ֱ�  
    }
    protected override void Update()
    {
        base .Update();
        CameraRay();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoyHot")) //���� ���� ��ġ�� ���ٸ�
        {
            isNearBricks = true;
        }
        else if (other.CompareTag("Tray"))
        {
            isNearEnd = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SoyHot")) //���� ���� ��ġ�� ���ٸ�
        {
            isNearBricks = false;
        }
        else if (other.CompareTag("Tray"))
        {
            isNearEnd = false;
        }
    }

    private void CameraRay() //ĵ���� ���� ������Ʈ
    {
        if (isNearBricks && BricksCount < 8)
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "bricksPick_key"; //���� �ݱ� �ؽ�Ʈ ����
            if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
            {
                interactionButton = false;
                animator.SetTrigger("CarryPickupTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                BricksCount++;
                SoundManager.instance.PlaySound2D("brick" + SoundManager.Range(1, 3)); //����� ���
            }
        }
        else if (isNearEnd && bricksCount > 0) //���� ���� �������� ��ġ�� �Դٸ�
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "bricksPut_key"; //���� �������� �ؽ�Ʈ ����
            if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
            {
                interactionButton = false;
                animator.SetTrigger("CarryPutdownTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                CalculateMoney(BricksCount);
                BricksCount = 0;
                brickFallSound.Play();
                SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //���Ҹ� �߰�
            }
        }
        else
        {
            text.enabled = false;
        }
    }
    private IEnumerator UpdateBricksVisibility()
    {
        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < brickObjects.Length; i++)
        {
            brickObjects[i].SetActive(i < bricksCount); // bricksCount���� ���� �ε����� Ȱ��ȭ
        }

        carrySpeed = 1.2f - (0.007f * (10 - strength)) * BricksCount;
        animator.SetFloat("WalkSpeed", carrySpeed);
    }
    private void FindBricks()
    {
        box = GameObject.FindGameObjectWithTag("Bottle"); //�ڽ� ������Ʈ ã��

        // box�� �ڽ� ���� Ȯ��
        int childCount = box.transform.childCount;
        // �迭 �ʱ�ȭ
        brickObjects = new GameObject[childCount];
        // ��� �ڽ��� �迭�� ����
        for (int i = 0; i < childCount; i++)
        {
            brickObjects[i] = box.transform.GetChild(i).gameObject;
        }

        StartCoroutine(UpdateBricksVisibility()); //���� ������Ʈ ��Ȱ��ȭ

    }
    private void CalculateMoney(int brickCounts)
    {
        //AddMoney(brickCounts * 1700); //�� ����
        //int rand =  UnityEngine.Random.Range(7, 36);
        //AddTip(brickCounts * (rand * 10)); //50������ 200�� ���� ��
        AddTip(brickCounts * 700); //�� ����
    }
    
}
