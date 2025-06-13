using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class HumanoidControllerCafe : MonoBehaviour
{

    public bool subOn = false; //���� ���� ����
    public int mMenu, sMenu;

    bool isTurning;
    public bool isDestination, isFirst; //ù�մ�����
    protected Animator animator;
    protected NavMeshAgent agent;
    [SerializeField]
    protected GameObject character; //ĳ���� �����ؼ� ���� ������ �Լ� ȣ���� �뵵

    [SerializeField]
    protected GameObject chatBubble_main;
    [SerializeField]
    protected GameObject chatBubble_sub;

    [SerializeField]
    protected GameObject[] coffeeMenu;  //�տ� �� Ŀ�Ƕ� ���̵� �޽���
    [SerializeField]
    protected GameObject[] sideMenu;

    [SerializeField]
    private bool isWoman;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        subOn = CoffeeTableManager.instance.subChatOn; //����޴� ���� �޾ƿ�
        mMenu = CoffeeTableManager.instance.mainMenu; //�޴� ��ȣ �޾ƿ���
        sMenu = CoffeeTableManager.instance.subMenu;

        int a = Random.Range(0, 3);
        animator.SetFloat("IdleState",a); //��� ��� ���� ���
    }

    void Update()
    {
        // NPC�� ��ǥ ��ġ�� �����ߴ��� Ȯ��
        if (isDestination &&!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // �������� �����߰�, NPC�� ������ ��
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                // LookAtPlayer ����
                LookAtPlayer();
                isDestination = false;

                if (isFirst)
                {
                    Hello(); //ù�մ��̸� ��ǳ�� ����
                    isFirst = false;
                }
            }
        }
    }

    public void Hello() //���̺� �Ŵ������� ȣ��, ��ǳ�� ����
    {
        chatBubble_main.SetActive(true); //��ǳ�� Ű��
        chatBubble_main.transform.GetChild(mMenu).gameObject.SetActive(true); //���θ޴� ��ȣ���� �ڽ� ������Ʈ Ű��
        if (subOn)
        {
            chatBubble_sub.SetActive(true);
            chatBubble_sub.transform.GetChild(sMenu).gameObject.SetActive(true); //����޴� ��ȣ���� �ڽ� ������Ʈ Ű��
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

    public void Angry() //ȭ���� �� ȣ��
    {
        chatBubble_main.SetActive(false);//��ǳ�� ����
        chatBubble_sub.SetActive(false);
        if (isWoman)
        {
            SoundManager.instance.PlaySound3D("angerWoman" + SoundManager.Range(1, 2), transform);
        }
        else
        {
            SoundManager.instance.PlaySound3D("angerMan" + SoundManager.Range(1, 2), transform);
        }       
        //�Ҹ� Ȱ��ȭ,���� ����
    }
    public void Good() //�ֹ� ����� ����
    {
        chatBubble_main.SetActive(false);//��ǳ�� ����
        chatBubble_sub.SetActive(false);
        coffeeMenu[mMenu].SetActive(true); //�տ� Ŀ�ǵ���ϱ�
        if (subOn)
        {
            sideMenu[sMenu].SetActive(true); //�տ� ���̵�޴�����ϱ�
        }
        if (isWoman)
        {
            SoundManager.instance.PlaySound3D("happyWoman" + SoundManager.Range(1, 5), transform);
        }
        else
        {
            SoundManager.instance.PlaySound3D("happyMan" + SoundManager.Range(1, 4), transform);
        }
        //�Ҹ� Ȱ��ȭ,���� ����
    }
    public void Appearance()
    {
        character.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize(); //���� ����
    }

    #region Rotation
    public void LookAtPlayer()
    {
        // �ٶ� ���� ���� (Y�� 90��)
        Vector3 dir = new Vector3(1, 0, 0); // Y���� �����ϰ� Z�� ���� ȸ��
        Quaternion lookRot = Quaternion.LookRotation(dir);

        StartCoroutine(LookAt(lookRot));
    }

    IEnumerator LookAt(Quaternion lookRot) //�ڿ������� ȸ�� ���� �ڷ�ƾ
    {
        //�ڷ�ƾ ���������� Ȯ��
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
                yield break; //�ڷ�ƾ ����
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 720 * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
        isTurning = false;
    }
    #endregion
}
