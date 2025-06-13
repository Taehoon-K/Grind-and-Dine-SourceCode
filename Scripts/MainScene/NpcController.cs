using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NpcController : MonoBehaviour
{
    Animator animator;

    public bool isFirstNpc = false;
    public int helloNum, byeNum;

    public UnityEvent onGreet,reset;

    [SerializeField]
    private GameObject character; //캐릭터 지정해서 랜덤 돌리기 함수 호출할 용도

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private GameObject[] actionItem; //각 상체 행동 시 나타날 아이템
    [SerializeField]
    private GameObject[] subActionItem; //서브 아이템

    [SerializeField]
    private int animationIndex; //상체행동 애니메이션 개수

    [SerializeField]
    private int actionNum; //각 행동 넘버

    private Coroutine upperBodyCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        character.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize(); //랜덤 실행

        
    }
    public void Gen() //npc젠 될때 호출
    {
        actionNum = Random.Range(0, animationIndex);
        if (actionItem.Length > actionNum) //액션아이템 배열 안의 수라면
        {
            actionItem[actionNum].SetActive(true); //해당 아이템 활성화
            if(actionNum == 4)
            {
                subActionItem[0].SetActive(true);
            }else if(actionNum == 2)
            {
                subActionItem[1].SetActive(true);
            }
        }
    }
    public void GreetingBye()
    {
        StopAllUpper(); //상체행동 모두 중지
        onGreet.Invoke(); //changeNpc호출
        animator.SetFloat("ByeNum", byeNum);
        animator.SetTrigger("isBye");
    }
    public void Transport() //위치 이동
    {
        gameObject.transform.position = startPoint.position;
        reset.Invoke();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void GreetingHello()
    {
        animator.SetFloat("GreetingNum", helloNum);
        animator.SetTrigger("isGreeting");
        Invoke(nameof(StartUpperBodyRoutine), 7f);
    }
    private void UpperPlay() //상체행동 시작
    {
        /*if (animationIndex >= actionNum) //액션아이템 배열 안의 수라면
        {
            animator.SetFloat("PlayingNum", actionNum);
            animator.SetBool("isUpper", true);
            int rand = Random.Range(30, 50);
            Invoke(nameof(UpperStop), rand);
        }*/
        animator.SetFloat("PlayingNum", actionNum);
        animator.SetBool("isUpper", true);
    }
    private void UpperStop() //상체행동 중지
    {
        animator.SetBool("isUpper", false);
    }
    IEnumerator UpperBodyRoutine()
    {
        while (true)
        {
            UpperPlay();
            yield return new WaitForSeconds(Random.Range(15, 20)); // UpperPlay의 랜덤 대기 시간
            UpperStop();
            yield return new WaitForSeconds(Random.Range(10, 15)); // UpperStop 후 잠시 대기
        }
    }

    private void StopAllUpper() // 즉시 애니메이션 멈추기
    {
        if (upperBodyCoroutine != null)
        {
            StopCoroutine(upperBodyCoroutine);
            upperBodyCoroutine = null;
        }
        UpperStop();
    }
    void StartUpperBodyRoutine() //코루틴 시작
    {
        upperBodyCoroutine = StartCoroutine(UpperBodyRoutine());
    }
}
