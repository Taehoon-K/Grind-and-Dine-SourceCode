using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerHome : MonoBehaviour
{
    [SerializeField] private Animator ani;
    [SerializeField] private Transform cameras;
    [SerializeField] private Transform target;
    [SerializeField] private float speed;

    [SerializeField] private GameObject[] objectsToActivate;


    private void Start()
    {
        Invoke(nameof(starrr),3f);
        OrbitAroundObject(speed);

        StartCoroutine(ActivateOneByOne());
    }
    private void starrr()
    {
        ani.SetTrigger("Idle");
    }

    public void OrbitAroundObject(float speed)
    {
        StartCoroutine(OrbitCoroutine(speed));
    }
    private IEnumerator OrbitCoroutine(float speed)
    {
        if (target == null) yield break;


        // 초기 위치에서 대상까지의 벡터 계산 (수평 거리)
        Vector3 offset = cameras.position - target.position;
        float height = cameras.position.y; // 초기 카메라 높이 유지

        // xz 평면상의 거리 계산 (y 값 제외)
        float horizontalDistance = new Vector2(offset.x, offset.z).magnitude;
        float startAngle = Mathf.Atan2(offset.z, offset.x); // 초기 각도 계산

        float angle = startAngle; // 시작 각도

        while (true)
        {
            if (target == null) yield break;

            angle += speed * Time.deltaTime; // 반시계 방향 회전
            Vector3 newOffset = new Vector3(Mathf.Cos(angle) * horizontalDistance, 0, Mathf.Sin(angle) * horizontalDistance);

            // 새로운 카메라 위치 설정 (높이는 유지)
            cameras.position = new Vector3(target.position.x + newOffset.x, height, target.position.z + newOffset.z);
            cameras.LookAt(target); // 대상 바라보기

            yield return null;
        }
    }

    IEnumerator ActivateOneByOne()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            // 모두 끄기
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(false);
            }

            // 현재 인덱스만 켜기
            objectsToActivate[i].SetActive(true);

            yield return new WaitForSeconds(3f);
        }
    }
}
