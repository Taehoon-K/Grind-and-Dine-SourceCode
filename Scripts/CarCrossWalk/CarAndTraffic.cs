using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAndTraffic : MonoBehaviour
{
    [SerializeField]
    private Transform[] genPo; //차량 젠 포인트 총 4곳
    [SerializeField] private Transform[] endPo; //차량 파괴 포인트 총 4곳
    [SerializeField]
    private GameObject[] cars; //차량들 프리팹
    [SerializeField]
    private Material[] materials; //차량에 칠할 색들
    [SerializeField]
    private int genTime; //차량 생성 주기

    private void Start()
    {
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        while (true)
        {
            // a ±10초의 랜덤 시간 계산
            float randomTime = Random.Range(genTime - 1f, genTime + 1f);

            // 랜덤 시간 대기
            yield return new WaitForSeconds(randomTime);

            int randomCar = Random.Range(0, cars.Length);
            int randomMat = Random.Range(0, materials.Length);
            int randomPo = Random.Range(0, genPo.Length);
            // 오브젝트 생성
            GameObject car = Instantiate(cars[randomCar], genPo[randomPo].position, genPo[randomPo].rotation);
            car.GetComponent<MeshRenderer>().material = materials[randomMat];
            car.GetComponent<NavMeshAgent>().SetDestination(endPo[randomPo].position); //차량 엔드 구간 설정
        }
    }
}
