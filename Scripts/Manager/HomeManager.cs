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
        if (instance != null && instance != this) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
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

    public void GenSean() //빚쟁이 활성화
    {
        sean.SetActive(true);
        playeryarn.CheckForNearbyNPC("PayDeptStart",sean.transform);
    }

    public void GenTutoSean() //빚쟁이 활성화
    {
        sean.SetActive(true);
        playeryarn.CheckForNearbyNPC("TutorialSean", sean.transform);
    }

    public void DegenSean() //빚 갚거나 넘어갈 시 빚쟁이 없애기
    {
        StartCoroutine(MoveNpc());
    }

    private IEnumerator MoveNpc()
    {
        Vector3 targetP = seanOutPoint.position;
        //npc 해당 위치로 이동
        // 목적지 설정
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetP, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            animator.SetBool("Walk", true);
        }

        // 이동 완료 대기
        while (agent.remainingDistance > 0.1f || agent.pathPending)
        {
            // Debug.Log(agent.remainingDistance + "길 가는중" + agent.pathPending);
            yield return null;
        }
        sean.SetActive(false);
    }
}
