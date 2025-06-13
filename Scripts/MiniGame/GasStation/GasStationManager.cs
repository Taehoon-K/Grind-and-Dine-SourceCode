using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using FIMSpace.Basics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class GasStationManager : ThridMiniManager
{
    public static GasStationManager instance = null;

    [Header("InGameStat")]
    [SerializeField] private int bricksCount; //현재 벽돌 갯수
    private bool isNearDigel, isNearGasolin, isNearDigel2, isNearGasolin2, isNearWater, isNearPaper,
        isNearFuel1,isNearFuel2, isNearInsert1, isNearInsert2;
    [SerializeField] private int currentHand = -1; //현재 손에 들려있는것 0: 디젤, 1: 가솔, 2: 물, 3: 휴지

    [Header("Speed")]
    [SerializeField] private float carrySpeed = 1.2f; //초기 걷는속도
    [SerializeField] private float runSpeed;

    [Header("Audio")]
    [SerializeField] private AudioSource brickFallSound;
    [SerializeField] private AudioSource[] claction;

    [Header("Car")]
    [SerializeField] private GameObject[] car; //자동차 프리팹들
    [SerializeField] private Transform[] genPo; //차량 젠 포인트 총 2곳
    [SerializeField] private Transform[] targetPo; //차량 중간 포인트 총 4곳
    [SerializeField] private Transform[] endPo; //차량 파괴 포인트 총 2곳
    private bool car1AtTarget = false;
    private bool car2AtTarget = false;
    [SerializeField] private int car1OilType,car2OilType; //각자 차들 기름 종류 1:디젤 2:가솔린
    [SerializeField] private int car1SubType,car2SubType; //각자 물 휴지 종류 1: 물 2: 휴지 0: 없음


    [Header("FuelGauge")]
    [SerializeField] private GameObject fuelObject;
    [SerializeField] private float[] fuelWantGauge; //각자 차가 원하는 게이지 설정
    [SerializeField] private Image gaugeFront, gaugeBack; //게이지 앞뒤 설정
    public float currentFuelGauge; //주유한 양
    public float maxFuel = 1f;
    public float fillSpeed = 0.2f; // 초당 주유량
    public bool isFilling = false; //주유 중인지 여부

    [Header("Fuel Gun")]
    [SerializeField] private GameObject[] fuelGuns; //주유기 달려있는 옵젝
    [SerializeField] private GameObject[] ropes;
    [SerializeField] private GameObject ropeParent; //주유기 줄
    //[SerializeField] private Transform handsPosition; //캐릭터 손 위치


    #region privateField
    private GameObject[] handObject = new GameObject[4]; //0: 디젤, 1: 가솔, 2: 물, 3: 휴지
    private int strength;
    private GameObject car1, car2;
    #endregion

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
           // strength = stat.level[0]; //힘 레벨 가져오기          
        }

        FindObjects(); //주유기랑 물 휴지 찾아서 넣기
        Money = jobStat.BaseSalary; //baseSalary 설정

        StartCoroutine(SpawnCar(0));
        StartCoroutine(SpawnCar(1));

        FuelImage(false);

        ropeParent.transform.parent = handObject[0].transform.GetChild(0).transform;
        ropeParent.transform.localPosition = new Vector3(0, 0, 0);
    }
    protected override void Update()
    {
        base.Update();
        CameraRay();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoyHot")) //만약 벽돌 위치로 갔다면
        {
            isNearDigel = true;
        }
        else if (other.CompareTag("Tray"))
        {
            isNearGasolin = true;
        }
        else if (other.CompareTag("Bell")) 
        {
            isNearDigel2 = true;
        }
        else if (other.CompareTag("Watch"))
        {
            isNearGasolin2 = true;
        }
        else if (other.CompareTag("Shop"))
        {
            isNearWater = true;
        }
        else if (other.CompareTag("Sleep"))
        {
            isNearPaper = true;
        }
        else if (other.CompareTag("Sleep"))
        {
            isNearPaper = true;
        }

        else if (other.CompareTag("Eat1"))
        {
            isNearFuel1 = true;
        }
        else if (other.CompareTag("Eat2"))
        {
            isNearInsert1 = true;
        }
        else if (other.CompareTag("Drink1"))
        {
            isNearFuel2 = true;
        }
        else if (other.CompareTag("Lootable"))
        {
            isNearInsert2 = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SoyHot")) //만약 벽돌 위치로 갔다면
        {
            isNearDigel = false;
        }
        else if (other.CompareTag("Tray"))
        {
            isNearGasolin = false;
        }
        else if(other.CompareTag("Bell")) //만약 벽돌 위치로 갔다면
        {
            isNearDigel2 = false;
        }
        else if (other.CompareTag("Watch"))
        {
            isNearGasolin2 = false;
        }
        else if (other.CompareTag("Shop"))
        {
            isNearWater = false;
        }
        else if (other.CompareTag("Sleep"))
        {
            isNearPaper = false;
        }

        else if (other.CompareTag("Eat1"))
        {
            isNearFuel1 = false;
        }
        else if (other.CompareTag("Eat2"))
        {
            isNearInsert1 = false;
        }
        else if (other.CompareTag("Drink1"))
        {
            isNearFuel2 = false;
        }
        else if (other.CompareTag("Lootable"))
        {
            isNearInsert2 = false;
        }
    }

    private void CameraRay() //캔버스 관련 업데이트
    {
        if (isNearWater)
        {
            if (currentHand == 2) //손에 물 있다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //내려놓기 텍스트 띄우기
                if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryHandoffTrigger"); //주유기 갖다놓는 애니메이션 실행

                    StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                    StartCoroutine(UpdateObjectsVisibility(-1));
                }
            }
            else if (currentHand == -1) //손에 암것도 없다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "PickUrea_key"; //벽돌 줍기 텍스트 띄우기
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryRecieveTrigger");

                    StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                    StartCoroutine(UpdateObjectsVisibility(2));
                }
            }
        }
        else if (isNearPaper)
        {
            if (currentHand == 3) //손에 휴지 있다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //내려놓기 텍스트 띄우기
                if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryHandoffTrigger"); //주유기 갖다놓는 애니메이션 실행

                    StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                    StartCoroutine(UpdateObjectsVisibility(-1));
                }
            }
            else if (currentHand == -1) //손에 암것도 없다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "PickTissue_key"; //벽돌 줍기 텍스트 띄우기
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryRecieveTrigger");

                    StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                    StartCoroutine(UpdateObjectsVisibility(3));
                }
            }
        }
        else if (isNearDigel) //만약 1번 디젤 주유기 주변이면
        {
            if(currentHand == 0) //손에 주유기 있다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //내려놓기 텍스트 띄우기
                if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //주유기 갖다놓는 애니메이션 실행

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[0].SetActive(true);
                    ropes[0].SetActive(false);
                }
            }
            else if(currentHand == -1) //손에 암것도 없다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "DieselSelect_key"; //벽돌 줍기 텍스트 띄우기
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(0,0));
                    
                }
            }
        }
        else if (isNearDigel2) //만약 2번 디젤 주유기 주변이면
        {
            if (currentHand == 0) //손에 주유기 있다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //내려놓기 텍스트 띄우기
                if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //주유기 갖다놓는 애니메이션 실행

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[2].SetActive(true);
                    ropes[2].SetActive(false);
                }
            }
            else if (currentHand == -1) //손에 암것도 없다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "DieselSelect_key"; //벽돌 줍기 텍스트 띄우기
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(0, 2));

                }
            }
        }
        else if (isNearGasolin) //만약 1번 가솔린 주유기 주변이면
        {
            if (currentHand == 1) //손에 주유기 있다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //내려놓기 텍스트 띄우기
                if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //주유기 갖다놓는 애니메이션 실행

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[1].SetActive(true);
                    ropes[1].SetActive(false);
                }
            }
            else if (currentHand == -1) //손에 암것도 없다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "GasolineSelect_key"; //벽돌 줍기 텍스트 띄우기
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(1, 1));

                }
            }
        }
        else if (isNearGasolin2) //만약 2번 가솔린 주유기 주변이면
        {
            if (currentHand == 1) //손에 주유기 있다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //내려놓기 텍스트 띄우기
                if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //주유기 갖다놓는 애니메이션 실행

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[3].SetActive(true);
                    ropes[3].SetActive(false);
                }
            }
            else if (currentHand == -1) //손에 암것도 없다면
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "GasolineSelect_key"; //벽돌 줍기 텍스트 띄우기
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //애니메이션 실행 아니고 안움직일때
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(1, 3));

                }
            }
        }

        else if(isNearFuel1 && car1AtTarget && (currentHand == 0 || currentHand == 1) && car1OilType != 0) //주유기 들고 주유구 앞으로 왔을시
        {
            text.enabled = true;
            if (!isFilling) //주유중 아니라면
            {
                nameString.StringReference.TableEntryReference = "StartFueling_key"; //주유 시작 텍스트 띄우기
            }
            else
            {
                nameString.StringReference.TableEntryReference = "StopFueling_key"; //주유 시작 텍스트 띄우기
            }
           
            if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
            {
                interactionButton = false;
                animator.SetTrigger("OilStart"); //주유기 갖다놓는 애니메이션 실행

                if (currentHand == 0)  //디젤 주유시
                {
                    StartCoroutine(HandleFueling(0, true));
                }
                else
                {
                    StartCoroutine(HandleFueling(0, false));
                }   
            }
        }
        else if (isNearFuel2 && car2AtTarget && (currentHand == 0 || currentHand == 1) && car2OilType != 0) //주유기 들고 주유구 앞으로 왔을시
        {
            text.enabled = true;
            if (!isFilling) //주유중 아니라면
            {
                nameString.StringReference.TableEntryReference = "StartFueling_key"; //주유 시작 텍스트 띄우기
            }
            else
            {
                nameString.StringReference.TableEntryReference = "StopFueling_key"; //주유 시작 텍스트 띄우기
            }
            if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
            {
                interactionButton = false;
                animator.SetTrigger("OilStart"); //주유기 갖다놓는 애니메이션 실행

                if (currentHand == 0)  //디젤 주유시
                {
                    StartCoroutine(HandleFueling(1, true));
                }
                else
                {
                    StartCoroutine(HandleFueling(1, false));
                }
            }
        }

        else if (isNearInsert1 && car1AtTarget && (currentHand == 2 || currentHand == 3)) //물건 들고 창문 갔을 시
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "giveItem_key"; //아이템 주기 텍스트 띄우기
            if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
            {
                interactionButton = false;
                animator.SetTrigger("CarryHandoffTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                StartCoroutine(UpdateObjectsVisibility(-1));

                if (currentHand == 2)  //물
                {
                    WaterComplete(0, true);
                }
                else //휴지
                {
                    WaterComplete(0, false);
                }
            }

        }
        else if (isNearInsert2 && car2AtTarget && (currentHand == 2 || currentHand == 3)) //물건 들고 창문 갔을 시
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "giveItem_key"; 
            if (interactionButton && !controller.isPaused) //애니메이션 실행 아니고 안움직일때
            {
                interactionButton = false;
                animator.SetTrigger("CarryHandoffTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //애니메이션 실행
                StartCoroutine(UpdateObjectsVisibility(-1));

                if (currentHand == 2)  //물
                {
                    WaterComplete(1, true);
                }
                else //휴지
                {
                    WaterComplete(1, false);
                }
            }
        }

        else
        {
            text.enabled = false;
        }
    }
    private IEnumerator HandleFueling(int carIndex, bool isDigel) //주유 시 코루틴 시작
    {
        Debug.Log("주유 코루틴 시작");
        
        isFilling = true;
        SoundManager.instance.PlaySound2D("fueling-a-car1",0,true);
        // 플레이어 멈춤 + 게이지 ON
        controller.isPaused = true; //플레이어 이동 정지
        FuelImage(true, carIndex);

        /*// 대기: 스페이스바 입력 전까지 기다림
        yield return StartCoroutine(WaitForStartInput());*/

        // 스페이스바 중복 입력 방지를 위한 짧은 대기
        yield return new WaitForSeconds(0.2f);

        // 주유 시작
        while (currentFuelGauge < 1f)
        {
            currentFuelGauge += fillSpeed * Time.deltaTime;

            // 중단 입력 받기 (도중에 스페이스바 누르면 중단)
            if (interactionButton)
            {
                interactionButton = false;
                isFilling = false; //주유 중단
                //nameString.StringReference.TableEntryReference = "StopFueling_key";
                Debug.Log("주유 중단됨");
                FuelImage(false);
                //isFueling = false;
                RefuelComplete(carIndex, isDigel); //주유 끝
                animator.SetTrigger("OilEnd");
                controller.isPaused = false;
                yield break; // 코루틴 종료
            }

            // 게이지 UI 업데이트 여기에 추가 가능
            yield return null;
        }
        //주유 완료 처리
        FuelImage(false);
        Debug.Log("주유 꽉 채움");
        RefuelComplete(carIndex,isDigel); //주유 끝
        //isFueling = false;
        animator.SetTrigger("OilEnd");
        controller.isPaused = false;
    }
    private void FuelImage(bool on, int index = -1) //초록색 게이지 위치 설정
    {
        if (on)
        {
            fuelObject.SetActive(true);
            gaugeFront.fillAmount = fuelWantGauge[index];
            gaugeBack.fillAmount = 1 - (fuelWantGauge[index] + 0.15f);
        }
        else
        {
            fuelObject.SetActive(false);
        }
    }

    private IEnumerator UpdateObjectsVisibility(int obj, int rope = -1)
    {
        yield return new WaitForSeconds(1f);

        currentHand = obj;
        ActivateOnly(obj); //모두 비활성화

        if(obj == 0 || obj == 1) //만약 주유기 잡는거라면
        {
            fuelGuns[rope].SetActive(false);
            ropes[rope].SetActive(true);
        }
    }
    private void ActivateOnly(int index) //해당 오브젝트만 활성화 함수
    {
        for (int i = 0; i < handObject.Length; i++)
        {
            if (handObject[i] != null)
            {
                handObject[i].SetActive(i == index);
            }
        }
    }

    private void FindObjects()
    {
        handObject[2] = GameObject.FindGameObjectWithTag("Bottle"); //오브젝트 찾기
        handObject[3] = GameObject.FindGameObjectWithTag("ItemActive");
        handObject[0] = GameObject.FindGameObjectWithTag("TrashCan");
        handObject[1] = GameObject.FindGameObjectWithTag("ItemPlate");

        handObject[2].gameObject.SetActive(false);
        handObject[3].gameObject.SetActive(false);
        handObject[0].gameObject.SetActive(false);
        handObject[1].gameObject.SetActive(false);
    }
    private void CalculateMoney()
    {
        int rand =  UnityEngine.Random.Range(10, 21);
        SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //돈소리 추가
        AddTip(rand * 100); //1000원에서 2000원 사이 팁
    }


    private IEnumerator SpawnCar(int index)
    {
        int randd = UnityEngine.Random.Range(3, 10);
        yield return new WaitForSeconds(randd);

        GameObject selectedCar = Instantiate(car[UnityEngine.Random.Range(0, car.Length)], genPo[index].position, genPo[index].rotation);
        NavMeshAgent agent = selectedCar.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("차량 프리팹에 NavMeshAgent가 없습니다.");
            yield break;
        }

        if (index == 0)
        {
            int rand = UnityEngine.Random.Range(30, 86);  //주유 게이지 설정
            fuelWantGauge[0] = rand * 0.01f;
            car1OilType = UnityEngine.Random.Range(1, 3); //1,2중 랜덤 설정
            car1SubType = UnityEngine.Random.Range(0, 3); //0,1,2 중 랜덤 설정
            selectedCar.GetComponent<CarCanvas>().Render(car1OilType, car1SubType);

            car1 = selectedCar;
            agent.SetDestination(targetPo[0].position);
            StartCoroutine(WaitUntilArrival(agent, 0));
        }
        else
        {
            int rand = UnityEngine.Random.Range(30, 86); //주유 게이지 설정
            fuelWantGauge[1] = rand * 0.01f;
            car2OilType = UnityEngine.Random.Range(1, 3); //1,2중 랜덤 설정
            car2SubType = UnityEngine.Random.Range(0, 3); //0,1,2 중 랜덤 설정
            selectedCar.GetComponent<CarCanvas>().Render(car2OilType, car2SubType);

            car2 = selectedCar;
            agent.SetDestination(targetPo[1].position);
            StartCoroutine(WaitUntilArrival(agent, 1));
        }
    }

    private IEnumerator WaitUntilArrival(NavMeshAgent agent, int index)
    {
        while (agent.pathPending || agent.remainingDistance > 0.05f)
        {
            yield return null;
        }

        if (index == 0)
            car1AtTarget = true;
        else
            car2AtTarget = true;

        Debug.Log($"차량 {index + 1} 주유 위치 도착");
        claction[index].Play();
    }

    public void RefuelComplete(int index, bool isDigel) //트루면 디젤, 폴스면 가솔린
    {
        SoundManager.instance.StopLoopSound("fueling-a-car1");
        SoundManager.instance.PlaySound2D("fueling-a-carEnd");
        currentFuelGauge = Mathf.Lerp(0, 1f, currentFuelGauge);
        Debug.Log("주유 게이지: " + currentFuelGauge);

        if (currentFuelGauge >= fuelWantGauge[index] && currentFuelGauge <= fuelWantGauge[index] + 0.15f)
        {
            Debug.Log("주유 게이지 성고오고오오옹");

            if (index == 0 && car1AtTarget)
            {
                if ((isDigel && car1OilType == 1) || (!isDigel&& car1OilType == 2)) //만약 기름 종류에 맞게 넣었다면
                {
                    car1OilType = 0;
                    car1.GetComponent<CarCanvas>().CorrectItem(true);
                    if (car1SubType == 0) //만약 사이드도 이미 처리했다면
                    {
                       // Debug.Log("주유 종류 성공, 돈 추가");
                        CalculateMoney(); //돈 올라가게 추가
                        car1AtTarget = false;
                        StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
                    }
                }
                else //종류 틀리게 넣었으면
                {
                    Debug.Log("주유 종류 실패");
                    //화내고 나가기
                    car1AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
                }
            }
            else if (index == 1 && car2AtTarget)
            {
                if ((isDigel && car2OilType == 1) || (!isDigel && car2OilType == 2)) //만약 기름 종류에 맞게 넣었다면
                {
                    car2OilType = 0;
                    car2.GetComponent<CarCanvas>().CorrectItem(true);
                    if (car2SubType == 0) //만약 사이드도 이미 처리했다면
                    {
                        CalculateMoney(); //돈 올라가게 추가
                        car2AtTarget = false;
                        StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
                    }
                }
                else //종류 틀리게 넣었으면
                {
                    //화내고 나가기
                    car2AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
                }
            }
        }
        else //주유 게이지 못맞췄을 때
        {
            Debug.Log("주유 게이지 실패");
            //화내고 나가기
            if (index == 0 && car1AtTarget)
            {
                car1AtTarget = false;
                StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));

            }
            else if (index == 1 && car2AtTarget)
            {
                car2AtTarget = false;
                StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
            }
        }

        

        currentFuelGauge = 0;
    }
    private void WaterComplete(int index, bool isWater) //물이랑 휴지 줬을 시 검사
    {
        if (index == 0 && car1AtTarget)
        {
            if ((isWater && car1SubType == 1) || (!isWater && car1SubType == 2)) //만약 기름 종류에 맞게 넣었다면
            {
                car1SubType = 0;
                car1.GetComponent<CarCanvas>().CorrectItem(false);
                if (car1OilType == 0) //만약 주유도 이미 처리했다면
                {
                   // Debug.Log("사이드도 성공, 돈 추가");
                    CalculateMoney(); //돈 올라가게 추가
                    car1AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
                }
            }
            else //종류 틀리게 넣었으면
            {
                Debug.Log("사이드 종류 실패");
                //화내고 나가기
                car1AtTarget = false;
                StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
            }
        }
        else if (index == 1 && car2AtTarget)
        {
            if ((isWater && car2SubType == 1) || (!isWater && car2SubType == 2)) //만약 기름 종류에 맞게 넣었다면
            {
                car2SubType = 0;
                car2.GetComponent<CarCanvas>().CorrectItem(false);
                if (car2OilType == 0) //만약 주유도 이미 처리했다면
                {
                    CalculateMoney(); //돈 올라가게 추가
                    car2AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
                }
            }
            else //종류 틀리게 넣었으면
            {
                //화내고 나가기
                car2AtTarget = false;
                StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
            }
        }
    }

    private IEnumerator MoveToEndAndDestroy(GameObject carObj, Transform endPoint, int index)
    {
        //초기화
        if (index == 0) //1번차
        {
            car1OilType = 0;
            car1SubType = 0;
        }
        else  //2번차
        {
            car2OilType = 0;
            car2SubType = 0;
        }

        NavMeshAgent agent = carObj.GetComponent<NavMeshAgent>();
        if (agent == null) yield break;

        if(index == 0)
        {
            car1.GetComponent<CarCanvas>().StartSound();
        }
        else
        {
            car2.GetComponent<CarCanvas>().StartSound();
        }
        yield return new WaitForSeconds(1f);

        agent.SetDestination(endPoint.position);

        while (agent.pathPending || agent.remainingDistance > 0.2f)
        {
            yield return null;
        }

        Destroy(carObj);
        yield return new WaitForSeconds(1f);
        StartCoroutine(SpawnCar(index));
    }

    private IEnumerator WaitForStartInput() //주유 시작 대기 코루틴
    {
        Debug.Log("스페이스바를 눌러 주유 시작");

        while (interactionButton)
        {
            interactionButton = false;
            yield return null;
        }

        Debug.Log("주유 시작!");
    }
}
