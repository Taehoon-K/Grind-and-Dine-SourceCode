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
    private Transform startPoint;    // 버스의 시작 지점
    [SerializeField]
    private Transform endPoint;      // 버스의 도착 지점

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
            // VibrateForTime() 함수 호출
            VibrateForTime();

            // 1초에서 5초 사이의 랜덤 시간 대기
            float randomDelay = Random.Range(1f, 5f);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private void Update()
    {
        // 도착지점에 도착하면 순간 이동하여 다시 출발
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
        // 도착 지점으로 이동
        agent.SetDestination(endPoint.position);
    }

    private void TeleportToStart()
    {
        // 시작 지점으로 순간 이동
        // agent.Warp(startPoint.position);
        gameObject.transform.position = startPoint.position;
    }
}
