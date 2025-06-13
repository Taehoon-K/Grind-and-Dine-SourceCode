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

            //�̵��� ������ ����
            if (Vector3.Distance(transform.position, hit.position) < 1f) //������ �����ߴٸ� 
            {
                if (locationState.isSit) //���� �ɴ� ��ġ��
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
                Debug.LogError("NavMeshAgent�� �����ְų� NavMesh ���� ����");
                return;
            }
            agent.SetDestination(hit.position);
            return;
        }

        SceneTransitionManager.Location nextLocation = LocationManager.GetNextLocation(currentLocation, locationToMoveTo);

        //exit point ã��
        Vector3 destination = LocationManager.Instance.GetExitPosition(nextLocation).position;
        agent.SetDestination(destination);
    }
    public void MoveTo(Vector3 pos) //�ƽſ��� �����̴¿뵵, ������ �Ⱦ�
    {
        agent.SetDestination(pos);
    }

    public bool IsMoving() //��ȭ �ɶ� ���߰�
    {
        //������Ʈ ���߸� �ڵ� ����
        if (!agent.enabled) return false;
        float v = agent.velocity.sqrMagnitude;
        return v > 0;
    }
}
