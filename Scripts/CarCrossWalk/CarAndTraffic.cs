using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAndTraffic : MonoBehaviour
{
    [SerializeField]
    private Transform[] genPo; //���� �� ����Ʈ �� 4��
    [SerializeField] private Transform[] endPo; //���� �ı� ����Ʈ �� 4��
    [SerializeField]
    private GameObject[] cars; //������ ������
    [SerializeField]
    private Material[] materials; //������ ĥ�� ����
    [SerializeField]
    private int genTime; //���� ���� �ֱ�

    private void Start()
    {
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        while (true)
        {
            // a ��10���� ���� �ð� ���
            float randomTime = Random.Range(genTime - 1f, genTime + 1f);

            // ���� �ð� ���
            yield return new WaitForSeconds(randomTime);

            int randomCar = Random.Range(0, cars.Length);
            int randomMat = Random.Range(0, materials.Length);
            int randomPo = Random.Range(0, genPo.Length);
            // ������Ʈ ����
            GameObject car = Instantiate(cars[randomCar], genPo[randomPo].position, genPo[randomPo].rotation);
            car.GetComponent<MeshRenderer>().material = materials[randomMat];
            car.GetComponent<NavMeshAgent>().SetDestination(endPo[randomPo].position); //���� ���� ���� ����
        }
    }
}
