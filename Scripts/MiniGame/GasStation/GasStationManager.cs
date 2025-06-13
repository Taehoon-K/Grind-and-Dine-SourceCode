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
    [SerializeField] private int bricksCount; //���� ���� ����
    private bool isNearDigel, isNearGasolin, isNearDigel2, isNearGasolin2, isNearWater, isNearPaper,
        isNearFuel1,isNearFuel2, isNearInsert1, isNearInsert2;
    [SerializeField] private int currentHand = -1; //���� �տ� ����ִ°� 0: ����, 1: ����, 2: ��, 3: ����

    [Header("Speed")]
    [SerializeField] private float carrySpeed = 1.2f; //�ʱ� �ȴ¼ӵ�
    [SerializeField] private float runSpeed;

    [Header("Audio")]
    [SerializeField] private AudioSource brickFallSound;
    [SerializeField] private AudioSource[] claction;

    [Header("Car")]
    [SerializeField] private GameObject[] car; //�ڵ��� �����յ�
    [SerializeField] private Transform[] genPo; //���� �� ����Ʈ �� 2��
    [SerializeField] private Transform[] targetPo; //���� �߰� ����Ʈ �� 4��
    [SerializeField] private Transform[] endPo; //���� �ı� ����Ʈ �� 2��
    private bool car1AtTarget = false;
    private bool car2AtTarget = false;
    [SerializeField] private int car1OilType,car2OilType; //���� ���� �⸧ ���� 1:���� 2:���ָ�
    [SerializeField] private int car1SubType,car2SubType; //���� �� ���� ���� 1: �� 2: ���� 0: ����


    [Header("FuelGauge")]
    [SerializeField] private GameObject fuelObject;
    [SerializeField] private float[] fuelWantGauge; //���� ���� ���ϴ� ������ ����
    [SerializeField] private Image gaugeFront, gaugeBack; //������ �յ� ����
    public float currentFuelGauge; //������ ��
    public float maxFuel = 1f;
    public float fillSpeed = 0.2f; // �ʴ� ������
    public bool isFilling = false; //���� ������ ����

    [Header("Fuel Gun")]
    [SerializeField] private GameObject[] fuelGuns; //������ �޷��ִ� ����
    [SerializeField] private GameObject[] ropes;
    [SerializeField] private GameObject ropeParent; //������ ��
    //[SerializeField] private Transform handsPosition; //ĳ���� �� ��ġ


    #region privateField
    private GameObject[] handObject = new GameObject[4]; //0: ����, 1: ����, 2: ��, 3: ����
    private int strength;
    private GameObject car1, car2;
    #endregion

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
           // strength = stat.level[0]; //�� ���� ��������          
        }

        FindObjects(); //������� �� ���� ã�Ƽ� �ֱ�
        Money = jobStat.BaseSalary; //baseSalary ����

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
        if (other.CompareTag("SoyHot")) //���� ���� ��ġ�� ���ٸ�
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
        if (other.CompareTag("SoyHot")) //���� ���� ��ġ�� ���ٸ�
        {
            isNearDigel = false;
        }
        else if (other.CompareTag("Tray"))
        {
            isNearGasolin = false;
        }
        else if(other.CompareTag("Bell")) //���� ���� ��ġ�� ���ٸ�
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

    private void CameraRay() //ĵ���� ���� ������Ʈ
    {
        if (isNearWater)
        {
            if (currentHand == 2) //�տ� �� �ִٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //�������� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryHandoffTrigger"); //������ ���ٳ��� �ִϸ��̼� ����

                    StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                    StartCoroutine(UpdateObjectsVisibility(-1));
                }
            }
            else if (currentHand == -1) //�տ� �ϰ͵� ���ٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "PickUrea_key"; //���� �ݱ� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryRecieveTrigger");

                    StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                    StartCoroutine(UpdateObjectsVisibility(2));
                }
            }
        }
        else if (isNearPaper)
        {
            if (currentHand == 3) //�տ� ���� �ִٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //�������� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryHandoffTrigger"); //������ ���ٳ��� �ִϸ��̼� ����

                    StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                    StartCoroutine(UpdateObjectsVisibility(-1));
                }
            }
            else if (currentHand == -1) //�տ� �ϰ͵� ���ٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "PickTissue_key"; //���� �ݱ� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("CarryRecieveTrigger");

                    StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                    StartCoroutine(UpdateObjectsVisibility(3));
                }
            }
        }
        else if (isNearDigel) //���� 1�� ���� ������ �ֺ��̸�
        {
            if(currentHand == 0) //�տ� ������ �ִٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //�������� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //������ ���ٳ��� �ִϸ��̼� ����

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[0].SetActive(true);
                    ropes[0].SetActive(false);
                }
            }
            else if(currentHand == -1) //�տ� �ϰ͵� ���ٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "DieselSelect_key"; //���� �ݱ� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(0,0));
                    
                }
            }
        }
        else if (isNearDigel2) //���� 2�� ���� ������ �ֺ��̸�
        {
            if (currentHand == 0) //�տ� ������ �ִٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //�������� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //������ ���ٳ��� �ִϸ��̼� ����

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[2].SetActive(true);
                    ropes[2].SetActive(false);
                }
            }
            else if (currentHand == -1) //�տ� �ϰ͵� ���ٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "DieselSelect_key"; //���� �ݱ� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(0, 2));

                }
            }
        }
        else if (isNearGasolin) //���� 1�� ���ָ� ������ �ֺ��̸�
        {
            if (currentHand == 1) //�տ� ������ �ִٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //�������� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //������ ���ٳ��� �ִϸ��̼� ����

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[1].SetActive(true);
                    ropes[1].SetActive(false);
                }
            }
            else if (currentHand == -1) //�տ� �ϰ͵� ���ٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "GasolineSelect_key"; //���� �ݱ� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(1, 1));

                }
            }
        }
        else if (isNearGasolin2) //���� 2�� ���ָ� ������ �ֺ��̸�
        {
            if (currentHand == 1) //�տ� ������ �ִٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "put_key"; //�������� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp"); //������ ���ٳ��� �ִϸ��̼� ����

                    StartCoroutine(UpdateObjectsVisibility(-1));
                    fuelGuns[3].SetActive(true);
                    ropes[3].SetActive(false);
                }
            }
            else if (currentHand == -1) //�տ� �ϰ͵� ���ٸ�
            {
                text.enabled = true;
                nameString.StringReference.TableEntryReference = "GasolineSelect_key"; //���� �ݱ� �ؽ�Ʈ ����
                if (interactionButton && !controller.isPaused)   //car1AtTarget) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
                {
                    interactionButton = false;
                    animator.SetTrigger("FuelUp");

                    StartCoroutine(UpdateObjectsVisibility(1, 3));

                }
            }
        }

        else if(isNearFuel1 && car1AtTarget && (currentHand == 0 || currentHand == 1) && car1OilType != 0) //������ ��� ������ ������ ������
        {
            text.enabled = true;
            if (!isFilling) //������ �ƴ϶��
            {
                nameString.StringReference.TableEntryReference = "StartFueling_key"; //���� ���� �ؽ�Ʈ ����
            }
            else
            {
                nameString.StringReference.TableEntryReference = "StopFueling_key"; //���� ���� �ؽ�Ʈ ����
            }
           
            if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
            {
                interactionButton = false;
                animator.SetTrigger("OilStart"); //������ ���ٳ��� �ִϸ��̼� ����

                if (currentHand == 0)  //���� ������
                {
                    StartCoroutine(HandleFueling(0, true));
                }
                else
                {
                    StartCoroutine(HandleFueling(0, false));
                }   
            }
        }
        else if (isNearFuel2 && car2AtTarget && (currentHand == 0 || currentHand == 1) && car2OilType != 0) //������ ��� ������ ������ ������
        {
            text.enabled = true;
            if (!isFilling) //������ �ƴ϶��
            {
                nameString.StringReference.TableEntryReference = "StartFueling_key"; //���� ���� �ؽ�Ʈ ����
            }
            else
            {
                nameString.StringReference.TableEntryReference = "StopFueling_key"; //���� ���� �ؽ�Ʈ ����
            }
            if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
            {
                interactionButton = false;
                animator.SetTrigger("OilStart"); //������ ���ٳ��� �ִϸ��̼� ����

                if (currentHand == 0)  //���� ������
                {
                    StartCoroutine(HandleFueling(1, true));
                }
                else
                {
                    StartCoroutine(HandleFueling(1, false));
                }
            }
        }

        else if (isNearInsert1 && car1AtTarget && (currentHand == 2 || currentHand == 3)) //���� ��� â�� ���� ��
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "giveItem_key"; //������ �ֱ� �ؽ�Ʈ ����
            if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
            {
                interactionButton = false;
                animator.SetTrigger("CarryHandoffTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                StartCoroutine(UpdateObjectsVisibility(-1));

                if (currentHand == 2)  //��
                {
                    WaterComplete(0, true);
                }
                else //����
                {
                    WaterComplete(0, false);
                }
            }

        }
        else if (isNearInsert2 && car2AtTarget && (currentHand == 2 || currentHand == 3)) //���� ��� â�� ���� ��
        {
            text.enabled = true;
            nameString.StringReference.TableEntryReference = "giveItem_key"; 
            if (interactionButton && !controller.isPaused) //�ִϸ��̼� ���� �ƴϰ� �ȿ����϶�
            {
                interactionButton = false;
                animator.SetTrigger("CarryHandoffTrigger");
                StartCoroutine(controller.COMovePause(1.2f)); //�ִϸ��̼� ����
                StartCoroutine(UpdateObjectsVisibility(-1));

                if (currentHand == 2)  //��
                {
                    WaterComplete(1, true);
                }
                else //����
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
    private IEnumerator HandleFueling(int carIndex, bool isDigel) //���� �� �ڷ�ƾ ����
    {
        Debug.Log("���� �ڷ�ƾ ����");
        
        isFilling = true;
        SoundManager.instance.PlaySound2D("fueling-a-car1",0,true);
        // �÷��̾� ���� + ������ ON
        controller.isPaused = true; //�÷��̾� �̵� ����
        FuelImage(true, carIndex);

        /*// ���: �����̽��� �Է� ������ ��ٸ�
        yield return StartCoroutine(WaitForStartInput());*/

        // �����̽��� �ߺ� �Է� ������ ���� ª�� ���
        yield return new WaitForSeconds(0.2f);

        // ���� ����
        while (currentFuelGauge < 1f)
        {
            currentFuelGauge += fillSpeed * Time.deltaTime;

            // �ߴ� �Է� �ޱ� (���߿� �����̽��� ������ �ߴ�)
            if (interactionButton)
            {
                interactionButton = false;
                isFilling = false; //���� �ߴ�
                //nameString.StringReference.TableEntryReference = "StopFueling_key";
                Debug.Log("���� �ߴܵ�");
                FuelImage(false);
                //isFueling = false;
                RefuelComplete(carIndex, isDigel); //���� ��
                animator.SetTrigger("OilEnd");
                controller.isPaused = false;
                yield break; // �ڷ�ƾ ����
            }

            // ������ UI ������Ʈ ���⿡ �߰� ����
            yield return null;
        }
        //���� �Ϸ� ó��
        FuelImage(false);
        Debug.Log("���� �� ä��");
        RefuelComplete(carIndex,isDigel); //���� ��
        //isFueling = false;
        animator.SetTrigger("OilEnd");
        controller.isPaused = false;
    }
    private void FuelImage(bool on, int index = -1) //�ʷϻ� ������ ��ġ ����
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
        ActivateOnly(obj); //��� ��Ȱ��ȭ

        if(obj == 0 || obj == 1) //���� ������ ��°Ŷ��
        {
            fuelGuns[rope].SetActive(false);
            ropes[rope].SetActive(true);
        }
    }
    private void ActivateOnly(int index) //�ش� ������Ʈ�� Ȱ��ȭ �Լ�
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
        handObject[2] = GameObject.FindGameObjectWithTag("Bottle"); //������Ʈ ã��
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
        SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //���Ҹ� �߰�
        AddTip(rand * 100); //1000������ 2000�� ���� ��
    }


    private IEnumerator SpawnCar(int index)
    {
        int randd = UnityEngine.Random.Range(3, 10);
        yield return new WaitForSeconds(randd);

        GameObject selectedCar = Instantiate(car[UnityEngine.Random.Range(0, car.Length)], genPo[index].position, genPo[index].rotation);
        NavMeshAgent agent = selectedCar.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("���� �����տ� NavMeshAgent�� �����ϴ�.");
            yield break;
        }

        if (index == 0)
        {
            int rand = UnityEngine.Random.Range(30, 86);  //���� ������ ����
            fuelWantGauge[0] = rand * 0.01f;
            car1OilType = UnityEngine.Random.Range(1, 3); //1,2�� ���� ����
            car1SubType = UnityEngine.Random.Range(0, 3); //0,1,2 �� ���� ����
            selectedCar.GetComponent<CarCanvas>().Render(car1OilType, car1SubType);

            car1 = selectedCar;
            agent.SetDestination(targetPo[0].position);
            StartCoroutine(WaitUntilArrival(agent, 0));
        }
        else
        {
            int rand = UnityEngine.Random.Range(30, 86); //���� ������ ����
            fuelWantGauge[1] = rand * 0.01f;
            car2OilType = UnityEngine.Random.Range(1, 3); //1,2�� ���� ����
            car2SubType = UnityEngine.Random.Range(0, 3); //0,1,2 �� ���� ����
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

        Debug.Log($"���� {index + 1} ���� ��ġ ����");
        claction[index].Play();
    }

    public void RefuelComplete(int index, bool isDigel) //Ʈ��� ����, ������ ���ָ�
    {
        SoundManager.instance.StopLoopSound("fueling-a-car1");
        SoundManager.instance.PlaySound2D("fueling-a-carEnd");
        currentFuelGauge = Mathf.Lerp(0, 1f, currentFuelGauge);
        Debug.Log("���� ������: " + currentFuelGauge);

        if (currentFuelGauge >= fuelWantGauge[index] && currentFuelGauge <= fuelWantGauge[index] + 0.15f)
        {
            Debug.Log("���� ������ ������������");

            if (index == 0 && car1AtTarget)
            {
                if ((isDigel && car1OilType == 1) || (!isDigel&& car1OilType == 2)) //���� �⸧ ������ �°� �־��ٸ�
                {
                    car1OilType = 0;
                    car1.GetComponent<CarCanvas>().CorrectItem(true);
                    if (car1SubType == 0) //���� ���̵嵵 �̹� ó���ߴٸ�
                    {
                       // Debug.Log("���� ���� ����, �� �߰�");
                        CalculateMoney(); //�� �ö󰡰� �߰�
                        car1AtTarget = false;
                        StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
                    }
                }
                else //���� Ʋ���� �־�����
                {
                    Debug.Log("���� ���� ����");
                    //ȭ���� ������
                    car1AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
                }
            }
            else if (index == 1 && car2AtTarget)
            {
                if ((isDigel && car2OilType == 1) || (!isDigel && car2OilType == 2)) //���� �⸧ ������ �°� �־��ٸ�
                {
                    car2OilType = 0;
                    car2.GetComponent<CarCanvas>().CorrectItem(true);
                    if (car2SubType == 0) //���� ���̵嵵 �̹� ó���ߴٸ�
                    {
                        CalculateMoney(); //�� �ö󰡰� �߰�
                        car2AtTarget = false;
                        StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
                    }
                }
                else //���� Ʋ���� �־�����
                {
                    //ȭ���� ������
                    car2AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
                }
            }
        }
        else //���� ������ �������� ��
        {
            Debug.Log("���� ������ ����");
            //ȭ���� ������
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
    private void WaterComplete(int index, bool isWater) //���̶� ���� ���� �� �˻�
    {
        if (index == 0 && car1AtTarget)
        {
            if ((isWater && car1SubType == 1) || (!isWater && car1SubType == 2)) //���� �⸧ ������ �°� �־��ٸ�
            {
                car1SubType = 0;
                car1.GetComponent<CarCanvas>().CorrectItem(false);
                if (car1OilType == 0) //���� ������ �̹� ó���ߴٸ�
                {
                   // Debug.Log("���̵嵵 ����, �� �߰�");
                    CalculateMoney(); //�� �ö󰡰� �߰�
                    car1AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
                }
            }
            else //���� Ʋ���� �־�����
            {
                Debug.Log("���̵� ���� ����");
                //ȭ���� ������
                car1AtTarget = false;
                StartCoroutine(MoveToEndAndDestroy(car1, endPo[0], index));
            }
        }
        else if (index == 1 && car2AtTarget)
        {
            if ((isWater && car2SubType == 1) || (!isWater && car2SubType == 2)) //���� �⸧ ������ �°� �־��ٸ�
            {
                car2SubType = 0;
                car2.GetComponent<CarCanvas>().CorrectItem(false);
                if (car2OilType == 0) //���� ������ �̹� ó���ߴٸ�
                {
                    CalculateMoney(); //�� �ö󰡰� �߰�
                    car2AtTarget = false;
                    StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
                }
            }
            else //���� Ʋ���� �־�����
            {
                //ȭ���� ������
                car2AtTarget = false;
                StartCoroutine(MoveToEndAndDestroy(car2, endPo[1], index));
            }
        }
    }

    private IEnumerator MoveToEndAndDestroy(GameObject carObj, Transform endPoint, int index)
    {
        //�ʱ�ȭ
        if (index == 0) //1����
        {
            car1OilType = 0;
            car1SubType = 0;
        }
        else  //2����
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

    private IEnumerator WaitForStartInput() //���� ���� ��� �ڷ�ƾ
    {
        Debug.Log("�����̽��ٸ� ���� ���� ����");

        while (interactionButton)
        {
            interactionButton = false;
            yield return null;
        }

        Debug.Log("���� ����!");
    }
}
