using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    private NavMeshAgent agent;
    private TrafficLightController trafficLight;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("TrafficLight"))
        {
            trafficLight = other.GetComponent<TrafficLightController>();

            if (!trafficLight.IsGreenLight)
            {
                agent.velocity *= 0.5f; //속도 줄이고
                agent.isStopped = true; // 신호가 빨간 불이면 멈춤
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Table"))
        {
            // 본인 삭제
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Car")) //만약 차랑 접촉하면 멈춤
        {
            /*agent.velocity = Vector3.zero;
            agent.isStopped = true; // 멈춤*/
            // 앞 차인지 확인: 자기보다 앞에 있을 때만 멈추기
            Vector3 toOther = other.transform.position - transform.position;

            // 앞쪽 30도 이내에 있는 차만 대상으로 판단 (forward 기준)
            float angle = Vector3.Angle(transform.forward, toOther);

            if (angle < 60f && Vector3.Dot(transform.forward, toOther) > 0)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TrafficLight"))
        {
            agent.isStopped = false; // 횡단보도 벗어나면 이동
            trafficLight = null;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Car")) //만약 차랑 접촉하면
        {

            agent.isStopped = false; // 다시 출발
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TrafficLight") && trafficLight != null)
        {
            agent.isStopped = !trafficLight.IsGreenLight;
        }
    }
}
