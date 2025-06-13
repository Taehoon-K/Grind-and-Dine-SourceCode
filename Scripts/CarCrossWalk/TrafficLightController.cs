using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [SerializeField]
    private float redLight, blueLight;
    [SerializeField]
    private GameObject redLightO, blueLightO;

    private bool isGreenLight;
    public bool IsGreenLight
    {
        get { return isGreenLight; }
        set {
            isGreenLight = value;
            redLightO.SetActive(!value); 
            blueLightO.SetActive(value); 
        }
    }// ��ȣ�� ���� (�ʷ� ������ ����)

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    private IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // ���� �� (��� �ð� a)
            IsGreenLight = false;
            yield return new WaitForSeconds(redLight);

            // �ʷ� �� (���� �ð� b)
            IsGreenLight = true;
            yield return new WaitForSeconds(blueLight);
        }
    }
}
