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
    [SerializeField] private int bricksCount; //현재 벽돌 갯수
    [SerializeField] private TextMeshProUGUI bricksCountText; //현재 벽돌 갯수
    public bool isNearBricks, isNearEnd;
    [SerializeField] private GameObject[] brickObjects; //벽돌 오브젝트 배열

    [Header("Speed")]
    [SerializeField] private float carrySpeed = 1.2f; //초기 걷는속도
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
            return bricksCount; // 속성 값을 반환
        }
        set
        {
            bricksCount = value;
            bricksCountText.text = "X " + bricksCount.ToString();
            StartCoroutine(UpdateBricksVisibility()); //벽돌 오브젝트 활성화
        }
    }

    protected void Awake()
    {
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }
    protected override void Start()
    {
        base.Start();
        
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //시간 흐르는거 멈추게하기
            stat = StatusManager.instance.GetStatus().Clone();
            strength = stat.level[0]; //힘 레벨 가져오기
        }        
        //TimeManager.instance.RegisterTracker(this);
        FindBricks(); //벽돌 오브젝트 찾아서 배열에 넣기  
    }
    protected override void Update()
    {
        base .Update();
        CameraRay();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoyHot")) //만약 벽돌 위치로 갔다면
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
        if (other.CompareTag("SoyHot")) //만약 벽돌 위치로 갔다면
        {
            isNearBricks = false;
        }
        else if (other.CompareTag("Tray"))
        {
            isNearEnd = false;
        }
    }

    private void CameraRay() //캔버스 관련 업데이트
    {
        if (isNearBricks && BricksCount < 8)
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "bricksPick_key"; //벽돌 줍기 텍스트 띄우기
            if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
            {
                interactionButton = false;
                animator.SetTrigger("CarryPickupTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                BricksCount++;
                SoundManager.instance.PlaySound2D("brick" + SoundManager.Range(1, 3)); //오디오 재생
            }
        }
        else if (isNearEnd && bricksCount > 0) //만약 벽돌 내려놓는 위치에 왔다면
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "bricksPut_key"; //벽돌 내려놓기 텍스트 띄우기
            if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
            {
                interactionButton = false;
                animator.SetTrigger("CarryPutdownTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                CalculateMoney(BricksCount);
                BricksCount = 0;
                brickFallSound.Play();
                SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //돈소리 추가
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
            brickObjects[i].SetActive(i < bricksCount); // bricksCount보다 작은 인덱스만 활성화
        }

        carrySpeed = 1.2f - (0.007f * (10 - strength)) * BricksCount;
        animator.SetFloat("WalkSpeed", carrySpeed);
    }
    private void FindBricks()
    {
        box = GameObject.FindGameObjectWithTag("Bottle"); //박스 오브젝트 찾기

        // box의 자식 개수 확인
        int childCount = box.transform.childCount;
        // 배열 초기화
        brickObjects = new GameObject[childCount];
        // 모든 자식을 배열에 저장
        for (int i = 0; i < childCount; i++)
        {
            brickObjects[i] = box.transform.GetChild(i).gameObject;
        }

        StartCoroutine(UpdateBricksVisibility()); //벽돌 오브젝트 비활성화

    }
    private void CalculateMoney(int brickCounts)
    {
        //AddMoney(brickCounts * 1700); //돈 증가
        //int rand =  UnityEngine.Random.Range(7, 36);
        //AddTip(brickCounts * (rand * 10)); //50원에서 200원 사이 팁
        AddTip(brickCounts * 700); //돈 증가
    }
    
}
