using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] npc; //������ npc
    [SerializeField]
    private Transform genPoint;

    [SerializeField]
    private GameObject mainNpc; //���� �� npc
    private GameObject tempNpc;

    [SerializeField]
    private GameObject cameraMain;

    private int greetingNum;

    [SerializeField]
    private float transitionDuration; //ī�޶� ��ȯ �ð�

    [SerializeField]
    private float greetingIdleDuration; //�λ��ϰ� �����̺�� ���۱��� ���ð�
    [SerializeField]
    private float greetingIdleDuration2; //�λ��ϰ� �����̺�� ���۱��� ���ð�

    private bool isTransitioning = false; // ��ȯ ������ ����
    void Start()
    {
        if (mainNpc == null)
        {
            Debug.LogError("mainNpc was not assigned, generating new NPC...");
        }

        mainNpc.GetComponent<NpcController>().isFirstNpc = true; //ù��° npc
        mainNpc.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true; //�����̺�� Ʈ�� ����
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

    public void ResetScene() //�÷��̾� ���� �� ȭ�� �ʱ�ȭ�� �Լ�
    {
        if (Random.Range(0, 2) == 1)
        {
            tempNpc = Instantiate(npc[1], genPoint); //npc ����
        }
        else
        {
            tempNpc = Instantiate(npc[0], genPoint); //npc ����
        }
        

        //���� �������� ���ݴ� ����ٰ� �ϱ�
        
        tempNpc.GetComponent<NpcController>().helloNum = greetingNum;
        greetingNum = Random.Range(0, 6);
        tempNpc.GetComponent<NpcController>().byeNum = greetingNum;
        tempNpc.GetComponent<NpcController>().Gen();
    }
    public void ChangeNpc() //�λ��ϰ� npc �ٲٱ�
    {
        StartCoroutine(TransitionToTarget());
        mainNpc = tempNpc;
        mainNpc.GetComponent<NpcController>().GreetingHello();
        Invoke("BehaviorStart", greetingIdleDuration);
    }

    private void BehaviorStart()
    {
        //mainNpc.GetComponent<Animator>().SetTrigger("isWalk");
        mainNpc.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true; //�����̺�� Ʈ�� ����
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

    IEnumerator TransitionToTarget() //ī�޶� �θ����� ��ȯ
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

        // ��ȯ �Ϸ� �� ���� ��ġ ����
        Vector3 finalPosition = cameraMain.transform.position;
        finalPosition.x = endX;
        cameraMain.transform.position = finalPosition;
        
        isTransitioning = false;
    }
}
