using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class CharacterMovement : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;
    public bool isSit;
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void ToggleMovement(bool enabled)
    {
        agent.enabled = enabled;
    }

    public void MoveTo(NPCLocationState locationState)
    {
        SceneTransitionManager.Location locationToMoveTo = locationState.location;

        SceneTransitionManager.Location currentLocation = SceneTransitionManager.Instance.currentLocation;

        if(locationToMoveTo == currentLocation)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(locationState.coord, out hit, 10f, NavMesh.AllAreas);

            //이동할 목적지 설정
            if (Vector3.Distance(transform.position, hit.position) < 1f) //목적지 도착했다면 
            {
                if (locationState.isSit) //만약 앉는 위치면
                {
                    animator.SetBool("Sit", true);
                    isSit = true;
                    //Debug.Log("isSittttttttttttttttt");
                }
                return; 
            
            }

            animator.SetBool("Sit", false);
            isSit = false;
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                Debug.LogError("NavMeshAgent가 꺼져있거나 NavMesh 위에 없음");
                return;
            }
            agent.SetDestination(hit.position);
            return;
        }

        SceneTransitionManager.Location nextLocation = LocationManager.GetNextLocation(currentLocation, locationToMoveTo);

        //exit point 찾기
        Vector3 destination = LocationManager.Instance.GetExitPosition(nextLocation).position;
        agent.SetDestination(destination);
    }
    public void MoveTo(Vector3 pos) //컷신에서 움직이는용도, 지금은 안씀
    {
        agent.SetDestination(pos);
    }

    public bool IsMoving() //대화 걸때 멈추게
    {
        //에이전트 멈추면 자동 폴스
        if (!agent.enabled) return false;
        float v = agent.velocity.sqrMagnitude;
        return v > 0;
    }
}
