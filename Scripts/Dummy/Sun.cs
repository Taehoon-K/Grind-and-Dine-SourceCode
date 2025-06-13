using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    //Light lights;
    private float time;
    void Start()
    {
        //time = TimeManager.instance.gameTimer;
        //lights = GetComponent<Light>();
    }


    void Update()
    {
        time += Time.deltaTime * 0.5f;
        if (time >= 1440f * 7)
        {
            time = 0f; // �������� ������ ���� �� �ð��� ����
        }
        float clock = (time % 1440f) / 4 - 90;
        transform.rotation = Quaternion.Euler(new Vector3(clock, 30, 0));
        /*if (clock >= -10 || clock <= 190)
        {
            lights.intensity = 1;
        }
        else
        {
            lights.intensity = 0;
        }*/
    }
}
