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
    }// 신호등 상태 (초록 불인지 여부)

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    private IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // 빨간 불 (대기 시간 a)
            IsGreenLight = false;
            yield return new WaitForSeconds(redLight);

            // 초록 불 (지속 시간 b)
            IsGreenLight = true;
            yield return new WaitForSeconds(blueLight);
        }
    }
}
