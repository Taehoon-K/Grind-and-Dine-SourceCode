using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcAnimator : MonoBehaviour
{
    // 길을 찾아서 이동할 에이전트
    NavMeshAgent agent;
    Animator ani;

    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    private void Awake()
    {
        // 게임이 시작되면 게임 오브젝트에 부착된 NavMeshAgent 컴포넌트를 가져와서 저장
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
    }
    private void Start()
    {
        

        agent.updateRotation = true;
        agent.updatePosition = false;
        ani.applyRootMotion = true;
    }

    private void OnAnimatorMove()
    {
        Vector3 rootposition = ani.rootPosition;
        rootposition.y = agent.nextPosition.y;
        transform.position = rootposition;
        agent.nextPosition = rootposition;
        //transform.position = agent.nextPosition;
    }

    void Update()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        //ani.SetBool("move", shouldMove);
        ani.SetFloat("speedX", velocity.x);
        ani.SetFloat("speedY", velocity.y);

        //GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;
    }
}
