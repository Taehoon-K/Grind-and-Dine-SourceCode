using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class HumanoidControllerCafe : MonoBehaviour
{

    public bool subOn = false; //서브 생성 여부
    public int mMenu, sMenu;

    bool isTurning;
    public bool isDestination, isFirst; //첫손님인지
    protected Animator animator;
    protected NavMeshAgent agent;
    [SerializeField]
    protected GameObject character; //캐릭터 지정해서 랜덤 돌리기 함수 호출할 용도

    [SerializeField]
    protected GameObject chatBubble_main;
    [SerializeField]
    protected GameObject chatBubble_sub;

    [SerializeField]
    protected GameObject[] coffeeMenu;  //손에 들 커피랑 사이드 메쉬들
    [SerializeField]
    protected GameObject[] sideMenu;

    [SerializeField]
    private bool isWoman;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        subOn = CoffeeTableManager.instance.subChatOn; //서브메뉴 여부 받아옴
        mMenu = CoffeeTableManager.instance.mainMenu; //메뉴 번호 받아오기
        sMenu = CoffeeTableManager.instance.subMenu;

        int a = Random.Range(0, 3);
        animator.SetFloat("IdleState",a); //대기 모션 랜덤 재생
    }

    void Update()
    {
        // NPC가 목표 위치에 도달했는지 확인
        if (isDestination &&!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 목적지에 도달했고, NPC가 멈췄을 때
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                // LookAtPlayer 실행
                LookAtPlayer();
                isDestination = false;

                if (isFirst)
                {
                    Hello(); //첫손님이면 말풍선 띄우기
                    isFirst = false;
                }
            }
        }
    }

    public void Hello() //테이블 매니저에서 호출, 말풍선 띄우기
    {
        chatBubble_main.SetActive(true); //말풍선 키기
        chatBubble_main.transform.GetChild(mMenu).gameObject.SetActive(true); //메인메뉴 번호따라 자식 오브젝트 키기
        if (subOn)
        {
            chatBubble_sub.SetActive(true);
            chatBubble_sub.transform.GetChild(sMenu).gameObject.SetActive(true); //서브메뉴 번호따라 자식 오브젝트 키기
        }
        if (isWoman)
        {
            SoundManager.instance.PlaySound3D("questionWoman" + SoundManager.Range(1, 2), transform);
        }
        else
        {
            SoundManager.instance.PlaySound3D("questionMan" + SoundManager.Range(1, 2), transform);
        }
    }

    public void Angry() //화났을 시 호출
    {
        chatBubble_main.SetActive(false);//말풍선 끄기
        chatBubble_sub.SetActive(false);
        if (isWoman)
        {
            SoundManager.instance.PlaySound3D("angerWoman" + SoundManager.Range(1, 2), transform);
        }
        else
        {
            SoundManager.instance.PlaySound3D("angerMan" + SoundManager.Range(1, 2), transform);
        }       
        //소리 활성화,남녀 따로
    }
    public void Good() //주문 제대로 들어갈시
    {
        chatBubble_main.SetActive(false);//말풍선 끄기
        chatBubble_sub.SetActive(false);
        coffeeMenu[mMenu].SetActive(true); //손에 커피들게하기
        if (subOn)
        {
            sideMenu[sMenu].SetActive(true); //손에 사이드메뉴들게하기
        }
        if (isWoman)
        {
            SoundManager.instance.PlaySound3D("happyWoman" + SoundManager.Range(1, 5), transform);
        }
        else
        {
            SoundManager.instance.PlaySound3D("happyMan" + SoundManager.Range(1, 4), transform);
        }
        //소리 활성화,남녀 따로
    }
    public void Appearance()
    {
        character.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize(); //랜덤 실행
    }

    #region Rotation
    public void LookAtPlayer()
    {
        // 바라볼 방향 설정 (Y축 90도)
        Vector3 dir = new Vector3(1, 0, 0); // Y축을 고정하고 Z축 기준 회전
        Quaternion lookRot = Quaternion.LookRotation(dir);

        StartCoroutine(LookAt(lookRot));
    }

    IEnumerator LookAt(Quaternion lookRot) //자연스럽게 회전 위한 코루틴
    {
        //코루틴 실행중인지 확인
        if (isTurning)
        {
            isTurning = false;
        }
        else
        {
            isTurning = true;
        }
        while (transform.rotation != lookRot)
        {
            if (!isTurning)
            {
                yield break; //코루틴 종료
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 720 * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
        isTurning = false;
    }
    #endregion
}
