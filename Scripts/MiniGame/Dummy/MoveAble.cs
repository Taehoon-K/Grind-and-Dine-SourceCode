using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAble : MonoBehaviour
{
    // 길을 찾아서 이동할 에이전트
    NavMeshAgent agent;
    Animator ani;

    // 에이전트의 목적지
    [SerializeField]
    private Transform target;

    private void Awake()
    {
        // 게임이 시작되면 게임 오브젝트에 부착된 NavMeshAgent 컴포넌트를 가져와서 저장
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {

    }
}

