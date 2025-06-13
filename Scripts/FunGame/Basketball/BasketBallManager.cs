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
    private MeshCollider rimCollider; // �� �ݶ��̴�
    public GameObject ball; // �� ������Ʈ
    public Transform rimCenter; // �� ���߾� ��ġ
    public Transform cameraTransform; // ī�޶� Transform
    [SerializeField] private float shootHeightOffset = 1.5f; // ���� ���� ������
    [SerializeField] private Transform freethrowLine;
    [SerializeField] private Transform bench;
    [SerializeField] private Transform twoBoundLine;
    [SerializeField] private Transform benchEye;
    [SerializeField] private Transform freeThrowEye;
    [SerializeField] private Transform twoBoundEye;


    [Header("Shooting")]
    //[SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float throwForce = 10f; // ���� ��
    [SerializeField] private float throwUpwardForce = 10f; // ���� ��
                                                           // [SerializeField] private float angle = 45f; // �߻� ����
                                                           // [SerializeField] private int trajectoryPoints = 30; // ���� ����Ʈ ��
    [SerializeField] private float missOffset;
    [SerializeField] private float maxDistance;
    [SerializeField] private float shootNoise; //�÷��̾ �� �� �����¿� ���� Ƣ�°�


    [Header("Mouse")]
    [SerializeField] protected float sensitivity = 1f;
    protected Vector3 mouseMove;      //ī�޶� ȸ����
    protected Vector2 axisLook;

    [Header("Game")]
    [SerializeField] private bool isPlayerTurn = true; //�÷��̾� ������ ����
    public bool isShoot; //�÷��̾ �� ������ ����
    private bool isGoal;
    private int playerScore = 0; // �÷��̾� ����
    private int npcScore = 0; // NPC ����
    private bool isCamreaMove; //ī�޶� �̵�������
    public float cameraMoveSpeed = 2f; // ī�޶� �̵� �ӵ�
    public int money;
    public Status stat; //���� ������ �ִ� ���� ��ġ ����

    [Header("Npc")]
    [SerializeField] private Transform npcHand; //npc ��, �� ������
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


    private Vector3 lastMissedBallPosition; //���ٿ�� ������

    public static BasketBallManager instance = null;
    private int luck;
    private bool isTiming = false; // Ÿ�̸� ���� ����
    private float timer = 0f;     // Ÿ�̸� ��

    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //���� ��ư

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

    private void Start()
    {
        ResetBall(true);
        me.text = 0.ToString();
        oppo.text = 0.ToString();

        if(StatusManager.instance != null)
        {
            luck = StatusManager.instance.GetLuckLevel();
            TimeManager.instance.TimeTicking = false; //�ð� �帣�°� ���߰��ϱ�
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

        mouseMove = NormalizeAngles(cameraTransform.localEulerAngles); //���콺 ���� �̸� ī�޶� ��ġ�� ����
    }

    public void ShootBall(float gauge) //�÷��̾ ���� �� ���󰡴� �Լ�
    {
        // 1. �θ�-�ڽ� ���� ����
        ball.transform.parent = null;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.useGravity = true;

        // ���� ���
        Vector3 forceDirection = cameraTransform.transform.forward;

        // �¿� ���� ȸ�� �� �߰�
        float randomHorizontalAngle = UnityEngine.Random.Range(-shootNoise, shootNoise); // �¿� ȸ�� ����
        Quaternion horizontalRotation = Quaternion.Euler(0, randomHorizontalAngle, 0); // y�� ���� ȸ��
        // ȸ�� ����� ���� ���� ���
        Vector3 finalDirection = horizontalRotation * forceDirection;

        // �� ��� (������ �� ���)
        Vector3 forceToAdd = finalDirection * (throwForce * gauge) + transform.up * (throwUpwardForce * gauge);

        rb.AddForce(forceToAdd, ForceMode.Impulse); // �� ������  

        StartTimer();
    }

    public void ReceiceResult(bool isOut) //���ٿ��Ǹ� ��� �޾ƿ�
    {
        StopTimer(); //Ÿ�̸� �ʱ�ȭ

        StartCoroutine(ShootResult(isOut));  //�ڷ�ƾ ����
    }
    IEnumerator ShootResult(bool isOut)
    {
        yield return new WaitForSeconds(1f); //���ٿ�� �� 1�� ���

        if (isGoal) //���� �� ���ٸ�
        {
            shootMade.SetActive(true); //�� ���� ����
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

                isShoot = false; //�ٽ� ���� �� �ְ�
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
                ResetBall(false); //�� �ٽ� npc������
                StartCoroutine(MoveNpc(freethrowLine, ShootNpc));
            }
        }
        else  //�� �ȵ��� �� �� ü����
        {
            if (IsPlayerTurn)
            {
                IsPlayerTurn = false;
                //npc ����� �̵� ������

                yield return MoveCamera(benchEye); //��ġ�� �̵�

                if (isOut)//���� ��Ʈ ���̸�
                {
                    ResetBall(false);
                    //npc ����� ����Ʃ��������
                    StartCoroutine(MoveNpc(freethrowLine, ShootNpc));
                }
                else
                {
                    //����� 
                    ResetBall(false);
                    //npc ����� ���ٿ�� ��ġ��
                    StartCoroutine(MoveNpc(twoBoundLine, ShootNpc));
                }
                                                
            }
            else //npc �� �ȵ���
            {
                IsPlayerTurn = true;

                StartCoroutine(MoveNpc(bench, null)); //npc ��ġ�� �̵�

                if (isOut)//���� ��Ʈ ���̸�
                {
                    ResetBall(true);
                    //npc ����� ����Ʃ��������
                    yield return MoveCamera(freeThrowEye);
                }
                else
                {
                    //����� 
                    ResetBall(true);
                    //npc ����� ���ٿ�� ��ġ��
                    yield return MoveCamera(twoBoundEye);
                }

                isShoot = false; //�� ��� �ְ���
            }
        }
    }
    public void ShootNpc() 
    {
        
        float gravity = -9.81f;
        float apexHeightOffset = 2f;

        // 1. �θ�-�ڽ� ���� ����
        ball.transform.parent = null;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.useGravity = true;

        // 2. ��ǥ ���� ��� (�� �߽� + �ణ ���� ����)
        Vector3 targetPosition = rimCenter.position + Vector3.up * 0.2f; // �� �߽ɺ��� �ణ ���� ����

        Vector3 startPosition = ball.transform.GetChild(0).transform.position;

        // ���� ���� ���: ������ ���� ���� �� ���� ��ġ
        float apexHeight = targetPosition.y + apexHeightOffset;

        // ���� �� ���� �Ÿ� ���
        float horizontalDistance = Mathf.Sqrt(Mathf.Pow(targetPosition.x - startPosition.x, 2) + Mathf.Pow(targetPosition.z - startPosition.z, 2));
        float verticalDistance = targetPosition.y - startPosition.y;


        // �Ÿ� ��� ���� Ȯ�� ���
        float successProbability = Mathf.Clamp01(1f - (horizontalDistance / maxDistance)); // �Ÿ� ��� ���� Ȯ�� (0~1)

        // ���� ���� ����
        bool isSuccessful = UnityEngine.Random.value < successProbability;

        Debug.Log(horizontalDistance + "        a     " + successProbability+"   b   " + isSuccessful);
        // ���� ���ο� ���� ��ǥ ���� ����
        if (!isSuccessful)
        {
            float exclusionRange = missOffset / 2f;
            targetPosition.x += GetRandomOffsetWithExclusion(-missOffset, missOffset, exclusionRange);
            targetPosition.z += GetRandomOffsetWithExclusion(-missOffset, missOffset, exclusionRange);
            targetPosition.y += GetRandomOffsetWithExclusion(-missOffset/2f, missOffset / 2f, exclusionRange / 2f); // �ణ ��/�Ʒ��ε� ��������
        }

        // �ʱ� �ӵ� �� ���� ���
        float initialVerticalSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * (apexHeight - startPosition.y)); // �������� �����ϴ� �ӵ�
        float timeToApex = initialVerticalSpeed / Mathf.Abs(gravity); // �������� �����ϴ� �ð�
        float totalFlightTime = timeToApex + Mathf.Sqrt(2 * Mathf.Abs(gravity) * (apexHeight - targetPosition.y)) / Mathf.Abs(gravity); // ��ü ���� �ð�
        float initialHorizontalSpeed = horizontalDistance / totalFlightTime; // ���� �ӵ�

        // �ӵ� ���� ���
        Vector3 direction = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z).normalized; // ���� ����
        Vector3 velocity = direction * initialHorizontalSpeed; // ���� �ӵ� ����
        velocity.y = initialVerticalSpeed; // ���� �ӵ� ����

        // Rigidbody�� �ӵ� ����
        rb.velocity = velocity;

        Debug.DrawLine(startPosition, targetPosition, isSuccessful ? Color.green : Color.red, 2f);

        StartTimer();
    }


    void ResetBall(bool isPlayer)
    {
        isGoal = false;
        ball.GetComponent<Ball>().ResetVariable();

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero; // ���ӵ� �ʱ�ȭ
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // ���ӵ� �ʱ�ȭ
        ball.GetComponent<Rigidbody>().useGravity = false;

        if (isPlayer)
        {
            // �θ�-�ڽ� ���� ����
            ball.transform.parent = cameraTransform;
            //�� �ٽ� 0,0 ��ġ��
            ball.transform.localPosition = new Vector3(-0.22f, -0.6f, 0);
            ball.transform.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ
        }
        else
        {
            npcAni.SetTrigger("Hold");
            // �θ�-�ڽ� ���� ����
            ball.transform.parent = npcHand;
            ball.transform.localPosition = new Vector3(-0.22f, 0f, 0);
            ball.transform.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ
        }
    }
    public void ResetCameraToFree(bool isFree, Transform ThrowPoint = null)
    {
        Vector3 targetPosition;
        if (isFree || isGoal)
        {
            targetPosition = freethrowLine.position; // ��ǥ ��ġ
        }
        else
        {
            targetPosition = ThrowPoint.position;
        }
        targetPosition.y = -3.8f; // y ��ǥ�� 1.3���� ����
        cameraTransform.position = targetPosition; // ī�޶� ��ġ�� ����
        cameraTransform.LookAt(rimCenter);

        mouseMove = cameraTransform.localEulerAngles; //���콺 ���� �̸� ī�޶� ��ġ�� ����

        ResetBall(true);
    }

    #region Npc
    private IEnumerator MoveNpc(Transform point, Action onMoveComplete)
    {
        //Debug.Log("setDeeeeeeeeeeeeeeeeee");
        Vector3 targetP = point.position;
        //npc �ش� ��ġ�� �̵�
        // ������ ����
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetP, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            npcAni.SetBool("Walk", true);
        }

        // �̵� �Ϸ� ���
        while (agent.remainingDistance > 0.1f || agent.pathPending)
        {
           // Debug.Log(agent.remainingDistance + "�� ������" + agent.pathPending);
            yield return null;
        }
        npcAni.SetBool("Walk", false);
        LookAtRim();

        if (onMoveComplete != null) //�� ���� ���� �� ��� ��������
        {
            npcAni.SetTrigger("Shoot"); //�ִϸ��̼� ���� �� 1.4�� �ڿ� �� ����
            yield return new WaitForSeconds(1.4f);
        }
        // �̵� �Ϸ� �� �۾� ����
        onMoveComplete?.Invoke();
    }
    void LookAtRim()
    {

        //�Ÿ����
        Vector3 dir = rimCenter.position - agent.gameObject.transform.position;
        //npc�ȵ��������� y�� ����
        dir.y = 0;
        //vector to quaternion
        Quaternion lookRot = Quaternion.LookRotation(dir);

        StartCoroutine(LookAt(lookRot));
    }

    IEnumerator LookAt(Quaternion lookRot) //�ڿ������� ȸ�� ���� �ڷ�ƾ
    {
        bool isTurning = true;
        while (agent.gameObject.transform.rotation != lookRot)
        {
            if (!isTurning)
            {
                yield break; //�ڷ�ƾ ����
            }

           // Debug.Log("���ۺ�����");
            agent.gameObject.transform.rotation = Quaternion.RotateTowards(agent.gameObject.transform.rotation, lookRot, 720 * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
        isTurning = false;
    }
    #endregion



    private void LateUpdate()
    {
        if (Time.timeScale < 0.001f) return;         //�Ͻ����� �� �ð��� ���� ���¿��� �Է� ����

        mouseMove += new Vector3(-axisLook.y * sensitivity, axisLook.x * sensitivity, 0);

        // Clamp the vertical camera movement
        mouseMove.x = Mathf.Clamp(mouseMove.x, -85f, 85f);

        if (!isCamreaMove) //���� �����϶��� ���콺 ���� ����
        {
            // Rotate the camera based on the look input
            cameraTransform.localEulerAngles = mouseMove;
        }
    }

    public void IsGoal()
    {
        isGoal = true;
    }
    public void WhereCourt(Transform courtPoint) //�� ������ �ڸ� ����
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

        // ��Ȯ�� ��ǥ ��ġ�� ����
        cameraTransform.position = endPosition;
        cameraTransform.rotation = endRotation;

        mouseMove = NormalizeAngles(cameraTransform.localEulerAngles); //���콺 ���� �̸� ī�޶� ��ġ�� ����
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
        // Ÿ�̸Ӱ� ���� ���̸� �ð��� ���
        if (isTiming)
        {
            timer += Time.deltaTime;
            if(timer > 10)
            {
                StopTimer();
                StartCoroutine(ShootResult(true)); //������ �� �ѱ��
            }
        }

        if (helpButton)
        {
            helpButton = false;
            if(Time.timeScale != 0) //�ð� �ȸ�����������
            {
                helpPanel.SetActive(true);
            }
        }
    }

    // Ÿ�̸� ���� �Լ�
    public void StartTimer()
    {
        isTiming = true;
        timer = 0f; // Ÿ�̸� �ʱ�ȭ
    }

    // Ÿ�̸� ���� �Լ�
    public void StopTimer()
    {
        isTiming = false;
        Debug.Log($"���� ���ư� �ð�: {timer:F2}��");
    }

    float GetRandomOffsetWithExclusion(float min, float max, float exclusionRange)
    {
        float exclusionMin = -exclusionRange; // ���� ������ �ּҰ�
        float exclusionMax = exclusionRange; // ���� ������ �ִ밪
        float randomValue;

        do
        {
            randomValue = UnityEngine.Random.Range(min, max);
        } while (randomValue >= exclusionMin && randomValue <= exclusionMax); // ���� ������ ������ �ٽ� ����

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
