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
                agent.velocity *= 0.5f; //�ӵ� ���̰�
                agent.isStopped = true; // ��ȣ�� ���� ���̸� ����
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Table"))
        {
            // ���� ����
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Car")) //���� ���� �����ϸ� ����
        {
            /*agent.velocity = Vector3.zero;
            agent.isStopped = true; // ����*/
            // �� ������ Ȯ��: �ڱ⺸�� �տ� ���� ���� ���߱�
            Vector3 toOther = other.transform.position - transform.position;

            // ���� 30�� �̳��� �ִ� ���� ������� �Ǵ� (forward ����)
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
            agent.isStopped = false; // Ⱦ�ܺ��� ����� �̵�
            trafficLight = null;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Car")) //���� ���� �����ϸ�
        {

            agent.isStopped = false; // �ٽ� ���
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
