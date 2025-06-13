using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using MK.Toon;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BasketBallManager : MonoBehaviour, HelpPanel
{
    [SerializeField]
    private MeshCollider rimCollider; // 림 콜라이더
    public GameObject ball; // 공 오브젝트
    public Transform rimCenter; // 림 정중앙 위치
    public Transform cameraTransform; // 카메라 Transform
    [SerializeField] private float shootHeightOffset = 1.5f; // 슈팅 높이 오프셋
    [SerializeField] private Transform freethrowLine;
    [SerializeField] private Transform bench;
    [SerializeField] private Transform twoBoundLine;
    [SerializeField] private Transform benchEye;
    [SerializeField] private Transform freeThrowEye;
    [SerializeField] private Transform twoBoundEye;


    [Header("Shooting")]
    //[SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float throwForce = 10f; // 슈팅 힘
    [SerializeField] private float throwUpwardForce = 10f; // 슈팅 힘
                                                           // [SerializeField] private float angle = 45f; // 발사 각도
                                                           // [SerializeField] private int trajectoryPoints = 30; // 궤적 포인트 수
    [SerializeField] private float missOffset;
    [SerializeField] private float maxDistance;
    [SerializeField] private float shootNoise; //플레이어가 슛 쏠때 상하좌우 랜덤 튀는값


    [Header("Mouse")]
    [SerializeField] protected float sensitivity = 1f;
    protected Vector3 mouseMove;      //카메라 회전값
    protected Vector2 axisLook;

    [Header("Game")]
    [SerializeField] private bool isPlayerTurn = true; //플레이어 턴인지 여부
    public bool isShoot; //플레이어가 슛 쐈는지 여부
    private bool isGoal;
    private int playerScore = 0; // 플레이어 점수
    private int npcScore = 0; // NPC 점수
    private bool isCamreaMove; //카메라 이동중인지
    public float cameraMoveSpeed = 2f; // 카메라 이동 속도
    public int money;
    public Status stat; //원래 가지고 있던 스탯 수치 저장

    [Header("Npc")]
    [SerializeField] private Transform npcHand; //npc 손, 공 넣을것
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator npcAni;

    [Header("Canvas")]
    [SerializeField] private GameObject myTurn;
    [SerializeField] private GameObject npcTurn;
    [SerializeField] private GameObject ShootGage;
    [SerializeField] private FunResultPanel resultPanel;
    [SerializeField] private int winScore;
    [SerializeField] private GameObject shootMade;
    [SerializeField] private TextMeshProUGUI me,oppo;


    private Vector3 lastMissedBallPosition; //투바운드 포지션

    public static BasketBallManager instance = null;
    private int luck;
    private bool isTiming = false; // 타이머 실행 여부
    private float timer = 0f;     // 타이머 값

    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //도움말 버튼

    public bool IsPlayerTurn { get { return isPlayerTurn; } set { isPlayerTurn = value;
            myTurn.SetActive(value); npcTurn.SetActive(!value); ShootGage.SetActive(value);
        } }
    private int PlayerScore
    {
        get { return playerScore; }
        set
        {
            playerScore = value;
            me.text = value.ToString();
        }
    }
    private int NpcScore
    {
        get { return npcScore; }
        set
        {
            npcScore = value;
            oppo.text = value.ToString();
        }
    }

    private void Awake()
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

    private void Start()
    {
        ResetBall(true);
        me.text = 0.ToString();
        oppo.text = 0.ToString();

        if(StatusManager.instance != null)
        {
            luck = StatusManager.instance.GetLuckLevel();
            TimeManager.instance.TimeTicking = false; //시간 흐르는거 멈추게하기
            if (PlayerPrefs.HasKey("SelectedMoney"))
            {
                money = PlayerPrefs.GetInt("SelectedMoney");
                Debug.Log("Received Money: " + money);
            }
            stat = StatusManager.instance.GetStatus().Clone();
        }
        else
        {
            luck = 0;
        }
        sensitivity = PlayerPrefs.GetFloat("MouseSensSlider", 50f) / 20 + 0.1f;

        mouseMove = NormalizeAngles(cameraTransform.localEulerAngles); //마우스 시점 미리 카메라 위치로 고정
    }

    public void ShootBall(float gauge) //플레이어가 던진 공 날라가는 함수
    {
        // 1. 부모-자식 관계 해제
        ball.transform.parent = null;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.useGravity = true;

        // 방향 계산
        Vector3 forceDirection = cameraTransform.transform.forward;

        // 좌우 랜덤 회전 값 추가
        float randomHorizontalAngle = UnityEngine.Random.Range(-shootNoise, shootNoise); // 좌우 회전 각도
        Quaternion horizontalRotation = Quaternion.Euler(0, randomHorizontalAngle, 0); // y축 기준 회전
        // 회전 적용된 최종 방향 계산
        Vector3 finalDirection = horizontalRotation * forceDirection;

        // 힘 계산 (게이지 값 기반)
        Vector3 forceToAdd = finalDirection * (throwForce * gauge) + transform.up * (throwUpwardForce * gauge);

        rb.AddForce(forceToAdd, ForceMode.Impulse); // 슛 날리기  

        StartTimer();
    }

    public void ReceiceResult(bool isOut) //투바운드되면 결과 받아옴
    {
        StopTimer(); //타이머 초기화

        StartCoroutine(ShootResult(isOut));  //코루틴 실행
    }
    IEnumerator ShootResult(bool isOut)
    {
        yield return new WaitForSeconds(1f); //투바운드 후 1초 대기

        if (isGoal) //만약 골 들어갔다면
        {
            shootMade.SetActive(true); //슛 성공 띄우기
            yield return new WaitForSeconds(1f);
            shootMade.SetActive(false);

            if (IsPlayerTurn)
            {
                PlayerScore++;
                if (PlayerScore == winScore)
                {
                    resultPanel.gameObject.SetActive(true);
                    resultPanel.Render();
                    yield break;
                }
                yield return MoveCamera(freeThrowEye);
                ResetBall(true);

                isShoot = false; //다시 슛쏠 수 있게
            }
            else
            {
                NpcScore++;
                if(NpcScore == winScore)
                {
                    resultPanel.gameObject.SetActive(true);
                    resultPanel.Render(true);
                    yield break;
                }
                ResetBall(false); //공 다시 npc손으로
                StartCoroutine(MoveNpc(freethrowLine, ShootNpc));
            }
        }
        else  //골 안들어갔을 시 턴 체인지
        {
            if (IsPlayerTurn)
            {
                IsPlayerTurn = false;
                //npc 공들고 이동 넣을것

                yield return MoveCamera(benchEye); //벤치로 이동

                if (isOut)//만약 코트 밖이면
                {
                    ResetBall(false);
                    //npc 공들고 자유튜라인으로
                    StartCoroutine(MoveNpc(freethrowLine, ShootNpc));
                }
                else
                {
                    //공들고 
                    ResetBall(false);
                    //npc 공들고 투바운드 위치로
                    StartCoroutine(MoveNpc(twoBoundLine, ShootNpc));
                }
                                                
            }
            else //npc 슛 안들어가면
            {
                IsPlayerTurn = true;

                StartCoroutine(MoveNpc(bench, null)); //npc 벤치로 이동

                if (isOut)//만약 코트 밖이면
                {
                    ResetBall(true);
                    //npc 공들고 자유튜라인으로
                    yield return MoveCamera(freeThrowEye);
                }
                else
                {
                    //공들고 
                    ResetBall(true);
                    //npc 공들고 투바운드 위치로
                    yield return MoveCamera(twoBoundEye);
                }

                isShoot = false; //슛 쏠수 있게함
            }
        }
    }
    public void ShootNpc() 
    {
        
        float gravity = -9.81f;
        float apexHeightOffset = 2f;

        // 1. 부모-자식 관계 해제
        ball.transform.parent = null;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.useGravity = true;

        // 2. 목표 지점 계산 (림 중심 + 약간 위로 보정)
        Vector3 targetPosition = rimCenter.position + Vector3.up * 0.2f; // 림 중심보다 약간 위로 설정

        Vector3 startPosition = ball.transform.GetChild(0).transform.position;

        // 정점 높이 계산: 림보다 일정 높이 더 높은 위치
        float apexHeight = targetPosition.y + apexHeightOffset;

        // 공과 림 간의 거리 계산
        float horizontalDistance = Mathf.Sqrt(Mathf.Pow(targetPosition.x - startPosition.x, 2) + Mathf.Pow(targetPosition.z - startPosition.z, 2));
        float verticalDistance = targetPosition.y - startPosition.y;


        // 거리 기반 성공 확률 계산
        float successProbability = Mathf.Clamp01(1f - (horizontalDistance / maxDistance)); // 거리 기반 성공 확률 (0~1)

        // 성공 여부 결정
        bool isSuccessful = UnityEngine.Random.value < successProbability;

        Debug.Log(horizontalDistance + "        a     " + successProbability+"   b   " + isSuccessful);
        // 성공 여부에 따라 목표 지점 조정
        if (!isSuccessful)
        {
            float exclusionRange = missOffset / 2f;
            targetPosition.x += GetRandomOffsetWithExclusion(-missOffset, missOffset, exclusionRange);
            targetPosition.z += GetRandomOffsetWithExclusion(-missOffset, missOffset, exclusionRange);
            targetPosition.y += GetRandomOffsetWithExclusion(-missOffset/2f, missOffset / 2f, exclusionRange / 2f); // 약간 위/아래로도 빗나가게
        }

        // 초기 속도 및 각도 계산
        float initialVerticalSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * (apexHeight - startPosition.y)); // 정점까지 도달하는 속도
        float timeToApex = initialVerticalSpeed / Mathf.Abs(gravity); // 정점까지 도달하는 시간
        float totalFlightTime = timeToApex + Mathf.Sqrt(2 * Mathf.Abs(gravity) * (apexHeight - targetPosition.y)) / Mathf.Abs(gravity); // 전체 비행 시간
        float initialHorizontalSpeed = horizontalDistance / totalFlightTime; // 수평 속도

        // 속도 벡터 계산
        Vector3 direction = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z).normalized; // 수평 방향
        Vector3 velocity = direction * initialHorizontalSpeed; // 수평 속도 설정
        velocity.y = initialVerticalSpeed; // 수직 속도 설정

        // Rigidbody에 속도 설정
        rb.velocity = velocity;

        Debug.DrawLine(startPosition, targetPosition, isSuccessful ? Color.green : Color.red, 2f);

        StartTimer();
    }


    void ResetBall(bool isPlayer)
    {
        isGoal = false;
        ball.GetComponent<Ball>().ResetVariable();

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero; // 선속도 초기화
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // 선속도 초기화
        ball.GetComponent<Rigidbody>().useGravity = false;

        if (isPlayer)
        {
            // 부모-자식 관계 복원
            ball.transform.parent = cameraTransform;
            //볼 다시 0,0 위치로
            ball.transform.localPosition = new Vector3(-0.22f, -0.6f, 0);
            ball.transform.localRotation = Quaternion.identity; // 회전 초기화
        }
        else
        {
            npcAni.SetTrigger("Hold");
            // 부모-자식 관계 복원
            ball.transform.parent = npcHand;
            ball.transform.localPosition = new Vector3(-0.22f, 0f, 0);
            ball.transform.localRotation = Quaternion.identity; // 회전 초기화
        }
    }
    public void ResetCameraToFree(bool isFree, Transform ThrowPoint = null)
    {
        Vector3 targetPosition;
        if (isFree || isGoal)
        {
            targetPosition = freethrowLine.position; // 목표 위치
        }
        else
        {
            targetPosition = ThrowPoint.position;
        }
        targetPosition.y = -3.8f; // y 좌표를 1.3으로 설정
        cameraTransform.position = targetPosition; // 카메라 위치를 변경
        cameraTransform.LookAt(rimCenter);

        mouseMove = cameraTransform.localEulerAngles; //마우스 시점 미리 카메라 위치로 고정

        ResetBall(true);
    }

    #region Npc
    private IEnumerator MoveNpc(Transform point, Action onMoveComplete)
    {
        //Debug.Log("setDeeeeeeeeeeeeeeeeee");
        Vector3 targetP = point.position;
        //npc 해당 위치로 이동
        // 목적지 설정
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetP, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            npcAni.SetBool("Walk", true);
        }

        // 이동 완료 대기
        while (agent.remainingDistance > 0.1f || agent.pathPending)
        {
           // Debug.Log(agent.remainingDistance + "길 가는중" + agent.pathPending);
            yield return null;
        }
        npcAni.SetBool("Walk", false);
        LookAtRim();

        if (onMoveComplete != null) //공 던질 때만 슛 모션 나가게함
        {
            npcAni.SetTrigger("Shoot"); //애니메이션 실행 후 1.4초 뒤에 공 나감
            yield return new WaitForSeconds(1.4f);
        }
        // 이동 완료 후 작업 실행
        onMoveComplete?.Invoke();
    }
    void LookAtRim()
    {

        //거리계산
        Vector3 dir = rimCenter.position - agent.gameObject.transform.position;
        //npc안뒤집어지게 y축 고정
        dir.y = 0;
        //vector to quaternion
        Quaternion lookRot = Quaternion.LookRotation(dir);

        StartCoroutine(LookAt(lookRot));
    }

    IEnumerator LookAt(Quaternion lookRot) //자연스럽게 회전 위한 코루틴
    {
        bool isTurning = true;
        while (agent.gameObject.transform.rotation != lookRot)
        {
            if (!isTurning)
            {
                yield break; //코루틴 종료
            }

           // Debug.Log("빙글비윽ㄹ");
            agent.gameObject.transform.rotation = Quaternion.RotateTowards(agent.gameObject.transform.rotation, lookRot, 720 * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
        isTurning = false;
    }
    #endregion



    private void LateUpdate()
    {
        if (Time.timeScale < 0.001f) return;         //일시정지 등 시간을 멈춘 상태에선 입력 방지

        mouseMove += new Vector3(-axisLook.y * sensitivity, axisLook.x * sensitivity, 0);

        // Clamp the vertical camera movement
        mouseMove.x = Mathf.Clamp(mouseMove.x, -85f, 85f);

        if (!isCamreaMove) //정상 상태일때만 마우스 조작 가능
        {
            // Rotate the camera based on the look input
            cameraTransform.localEulerAngles = mouseMove;
        }
    }

    public void IsGoal()
    {
        isGoal = true;
    }
    public void WhereCourt(Transform courtPoint) //공 떨어진 자리 업뎃
    {
        twoBoundLine.position = courtPoint.position;
    }
    IEnumerator MoveCamera(Transform targetPosition)
    {
        isCamreaMove = true;
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;

        Vector3 endPosition = targetPosition.position;
        Quaternion endRotation = targetPosition.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < 1f / cameraMoveSpeed)
        {
            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime * cameraMoveSpeed);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime * cameraMoveSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 정확히 목표 위치에 도달
        cameraTransform.position = endPosition;
        cameraTransform.rotation = endRotation;

        mouseMove = NormalizeAngles(cameraTransform.localEulerAngles); //마우스 시점 미리 카메라 위치로 고정
        isCamreaMove = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Read.
        axisLook = context.ReadValue<Vector2>();
    }
    private Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;
        angles.y = (angles.y > 180) ? angles.y - 360 : angles.y;
        angles.z = (angles.z > 180) ? angles.z - 360 : angles.z;
        return angles;
    }


    private void Update()
    {
        // 타이머가 실행 중이면 시간을 계산
        if (isTiming)
        {
            timer += Time.deltaTime;
            if(timer > 10)
            {
                StopTimer();
                StartCoroutine(ShootResult(true)); //강제로 턴 넘기기
            }
        }

        if (helpButton)
        {
            helpButton = false;
            if(Time.timeScale != 0) //시간 안멈춰있을때만
            {
                helpPanel.SetActive(true);
            }
        }
    }

    // 타이머 시작 함수
    public void StartTimer()
    {
        isTiming = true;
        timer = 0f; // 타이머 초기화
    }

    // 타이머 종료 함수
    public void StopTimer()
    {
        isTiming = false;
        Debug.Log($"공이 날아간 시간: {timer:F2}초");
    }

    float GetRandomOffsetWithExclusion(float min, float max, float exclusionRange)
    {
        float exclusionMin = -exclusionRange; // 제외 범위의 최소값
        float exclusionMax = exclusionRange; // 제외 범위의 최대값
        float randomValue;

        do
        {
            randomValue = UnityEngine.Random.Range(min, max);
        } while (randomValue >= exclusionMin && randomValue <= exclusionMax); // 제외 범위에 있으면 다시 생성

        return randomValue;
    }


    public void HelpOn()
    {
        helpPanel.SetActive(true);
    }
    public void HelpOff()
    {
        helpPanel.SetActive(false);
    }

    public void OnHelpButton(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                helpButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                helpButton = false;
                break;
        }
    }
}
