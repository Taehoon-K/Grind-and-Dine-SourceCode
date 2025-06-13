using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private GameObject sean;
    [SerializeField] private Transform seanOutPoint;

    PlayerYarn playeryarn;
    NavMeshAgent agent;
    Animator animator;
    public static HomeManager instance = null;
    private void Awake()
    {
        if (instance != null && instance != this) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        playeryarn = FindObjectOfType<PlayerYarn>();
        agent = sean.GetComponent<NavMeshAgent>();
        animator = sean.GetComponent<Animator>();
    }

    public void GenSean() //������ Ȱ��ȭ
    {
        sean.SetActive(true);
        playeryarn.CheckForNearbyNPC("PayDeptStart",sean.transform);
    }

    public void GenTutoSean() //������ Ȱ��ȭ
    {
        sean.SetActive(true);
        playeryarn.CheckForNearbyNPC("TutorialSean", sean.transform);
    }

    public void DegenSean() //�� ���ų� �Ѿ �� ������ ���ֱ�
    {
        StartCoroutine(MoveNpc());
    }

    private IEnumerator MoveNpc()
    {
        Vector3 targetP = seanOutPoint.position;
        //npc �ش� ��ġ�� �̵�
        // ������ ����
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetP, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            animator.SetBool("Walk", true);
        }

        // �̵� �Ϸ� ���
        while (agent.remainingDistance > 0.1f || agent.pathPending)
        {
            // Debug.Log(agent.remainingDistance + "�� ������" + agent.pathPending);
            yield return null;
        }
        sean.SetActive(false);
    }
}
