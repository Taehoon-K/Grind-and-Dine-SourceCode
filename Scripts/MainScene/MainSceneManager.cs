using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] npc; //복제할 npc
    [SerializeField]
    private Transform genPoint;

    [SerializeField]
    private GameObject mainNpc; //시작 시 npc
    private GameObject tempNpc;

    [SerializeField]
    private GameObject cameraMain;

    private int greetingNum;

    [SerializeField]
    private float transitionDuration; //카메라 전환 시간

    [SerializeField]
    private float greetingIdleDuration; //인사하고 비헤이비어 시작까지 대기시간
    [SerializeField]
    private float greetingIdleDuration2; //인사하고 비헤이비어 시작까지 대기시간

    private bool isTransitioning = false; // 전환 중인지 여부
    void Start()
    {
        if (mainNpc == null)
        {
            Debug.LogError("mainNpc was not assigned, generating new NPC...");
        }

        mainNpc.GetComponent<NpcController>().isFirstNpc = true; //첫번째 npc
        mainNpc.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true; //비헤이비어 트리 시작
        mainNpc.GetComponent<Animator>().SetTrigger("isWalk");
        greetingNum = 3;
        mainNpc.GetComponent<NpcController>().byeNum = greetingNum;
        //BehaviorStart();
    }

    private void Update()
    {

        if (!isTransitioning)
        {
            FollowCamera();
        }
        
    }

    public void ResetScene() //플레이어 텔포 시 화면 초기화할 함수
    {
        if (Random.Range(0, 2) == 1)
        {
            tempNpc = Instantiate(npc[1], genPoint); //npc 생성
        }
        else
        {
            tempNpc = Instantiate(npc[0], genPoint); //npc 생성
        }
        

        //배경들 랜덤으로 없앴다 생겼다가 하기
        
        tempNpc.GetComponent<NpcController>().helloNum = greetingNum;
        greetingNum = Random.Range(0, 6);
        tempNpc.GetComponent<NpcController>().byeNum = greetingNum;
        tempNpc.GetComponent<NpcController>().Gen();
    }
    public void ChangeNpc() //인사하고 npc 바꾸기
    {
        StartCoroutine(TransitionToTarget());
        mainNpc = tempNpc;
        mainNpc.GetComponent<NpcController>().GreetingHello();
        Invoke("BehaviorStart", greetingIdleDuration);
    }

    private void BehaviorStart()
    {
        //mainNpc.GetComponent<Animator>().SetTrigger("isWalk");
        mainNpc.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true; //비헤이비어 트리 시작
        Invoke("WalkStart", greetingIdleDuration2);
    }
    private void WalkStart()
    {
        mainNpc.GetComponent<Animator>().SetTrigger("isWalk");
    }

    private void FollowCamera()
    {
        if (mainNpc == null) return;

        Vector3 newPosition = cameraMain.transform.position;
        newPosition.x = mainNpc.transform.position.x-1.5f;
        cameraMain.transform.position = newPosition;
    }

    IEnumerator TransitionToTarget() //카메라 부르럽게 전환
    {
        isTransitioning = true;
        float startX = mainNpc.transform.position.x - 1.5f;
        float endX = tempNpc.transform.position.x - 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            Vector3 newPosition = cameraMain.transform.position;
            newPosition.x = Mathf.Lerp(startX, endX, elapsedTime / transitionDuration);
            cameraMain.transform.position = newPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 전환 완료 후 최종 위치 설정
        Vector3 finalPosition = cameraMain.transform.position;
        finalPosition.x = endX;
        cameraMain.transform.position = finalPosition;
        
        isTransitioning = false;
    }
}
