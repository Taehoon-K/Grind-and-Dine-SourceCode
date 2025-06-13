using System.Collections;
using System.Collections.Generic;
using PP.InventorySystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization.Settings;

public class BusMovement : MonoBehaviour
{
    [Header("Bus")]
    [SerializeField]
    private Transform startPoint;    // ������ ���� ����
    [SerializeField]
    private Transform endPoint;      // ������ ���� ����

    private NavMeshAgent agent;

    [Header("Camera")]
    [SerializeField]
    private float shakeAmount;
    float shakeTime;
    private Vector3 intialPosition;
    [SerializeField]
    private Camera busCamera;

    public Yarn.Unity.TextLineProvider textLineProvider;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToEndPoint();

        intialPosition = busCamera.transform.localPosition;

        StartCoroutine(VibrateRoutine());

        textLineProvider.textLanguageCode = LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    private void VibrateForTime()
    {
        shakeTime = 0.1f;
    }
    IEnumerator VibrateRoutine()
    {
        while (true)
        {
            // VibrateForTime() �Լ� ȣ��
            VibrateForTime();

            // 1�ʿ��� 5�� ������ ���� �ð� ���
            float randomDelay = Random.Range(1f, 5f);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private void Update()
    {
        // ���������� �����ϸ� ���� �̵��Ͽ� �ٽ� ���
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            TeleportToStart();
            MoveToEndPoint();
        }

        if (shakeTime > 0)
        {
            //busCamera.transform.localPosition = Random.insideUnitSphere * shakeAmount + intialPosition;
            float shakeY = Random.Range(-1f, 1f) * shakeAmount;
            busCamera.transform.localPosition = new Vector3(intialPosition.x, intialPosition.y + shakeY, intialPosition.z);
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeTime = 0.0f;
            busCamera.transform.localPosition = intialPosition;
        }

    }

    private void MoveToEndPoint()
    {
        // ���� �������� �̵�
        agent.SetDestination(endPoint.position);
    }

    private void TeleportToStart()
    {
        // ���� �������� ���� �̵�
        // agent.Warp(startPoint.position);
        gameObject.transform.position = startPoint.position;
    }
}
