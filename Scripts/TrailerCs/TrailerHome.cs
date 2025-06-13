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


        // �ʱ� ��ġ���� �������� ���� ��� (���� �Ÿ�)
        Vector3 offset = cameras.position - target.position;
        float height = cameras.position.y; // �ʱ� ī�޶� ���� ����

        // xz ������ �Ÿ� ��� (y �� ����)
        float horizontalDistance = new Vector2(offset.x, offset.z).magnitude;
        float startAngle = Mathf.Atan2(offset.z, offset.x); // �ʱ� ���� ���

        float angle = startAngle; // ���� ����

        while (true)
        {
            if (target == null) yield break;

            angle += speed * Time.deltaTime; // �ݽð� ���� ȸ��
            Vector3 newOffset = new Vector3(Mathf.Cos(angle) * horizontalDistance, 0, Mathf.Sin(angle) * horizontalDistance);

            // ���ο� ī�޶� ��ġ ���� (���̴� ����)
            cameras.position = new Vector3(target.position.x + newOffset.x, height, target.position.z + newOffset.z);
            cameras.LookAt(target); // ��� �ٶ󺸱�

            yield return null;
        }
    }

    IEnumerator ActivateOneByOne()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            // ��� ����
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(false);
            }

            // ���� �ε����� �ѱ�
            objectsToActivate[i].SetActive(true);

            yield return new WaitForSeconds(3f);
        }
    }
}
