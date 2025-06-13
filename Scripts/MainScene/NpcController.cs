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
    private GameObject character; //ĳ���� �����ؼ� ���� ������ �Լ� ȣ���� �뵵

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private GameObject[] actionItem; //�� ��ü �ൿ �� ��Ÿ�� ������
    [SerializeField]
    private GameObject[] subActionItem; //���� ������

    [SerializeField]
    private int animationIndex; //��ü�ൿ �ִϸ��̼� ����

    [SerializeField]
    private int actionNum; //�� �ൿ �ѹ�

    private Coroutine upperBodyCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        character.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize(); //���� ����

        
    }
    public void Gen() //npc�� �ɶ� ȣ��
    {
        actionNum = Random.Range(0, animationIndex);
        if (actionItem.Length > actionNum) //�׼Ǿ����� �迭 ���� �����
        {
            actionItem[actionNum].SetActive(true); //�ش� ������ Ȱ��ȭ
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
        StopAllUpper(); //��ü�ൿ ��� ����
        onGreet.Invoke(); //changeNpcȣ��
        animator.SetFloat("ByeNum", byeNum);
        animator.SetTrigger("isBye");
    }
    public void Transport() //��ġ �̵�
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
    private void UpperPlay() //��ü�ൿ ����
    {
        /*if (animationIndex >= actionNum) //�׼Ǿ����� �迭 ���� �����
        {
            animator.SetFloat("PlayingNum", actionNum);
            animator.SetBool("isUpper", true);
            int rand = Random.Range(30, 50);
            Invoke(nameof(UpperStop), rand);
        }*/
        animator.SetFloat("PlayingNum", actionNum);
        animator.SetBool("isUpper", true);
    }
    private void UpperStop() //��ü�ൿ ����
    {
        animator.SetBool("isUpper", false);
    }
    IEnumerator UpperBodyRoutine()
    {
        while (true)
        {
            UpperPlay();
            yield return new WaitForSeconds(Random.Range(15, 20)); // UpperPlay�� ���� ��� �ð�
            UpperStop();
            yield return new WaitForSeconds(Random.Range(10, 15)); // UpperStop �� ��� ���
        }
    }

    private void StopAllUpper() // ��� �ִϸ��̼� ���߱�
    {
        if (upperBodyCoroutine != null)
        {
            StopCoroutine(upperBodyCoroutine);
            upperBodyCoroutine = null;
        }
        UpperStop();
    }
    void StartUpperBodyRoutine() //�ڷ�ƾ ����
    {
        upperBodyCoroutine = StartCoroutine(UpperBodyRoutine());
    }
}
