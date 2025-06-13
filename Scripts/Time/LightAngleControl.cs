using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAngleControl : MonoBehaviour
{
    // Directional Light의 강도를 조절할 때 사용할 변수입니다.
    public Light directionalLight;

    // Update 함수는 매 프레임마다 호출됩니다.
    void Update()
    {
        // Directional Light의 현재 회전 각도를 가져옵니다.
        float rotationX = transform.rotation.eulerAngles.x;
        // 만약 회전 각도가 180도 이상이라면 (즉, 빛이 아래로 향하는 경우)
        if (rotationX > 270f && rotationX <360f)
        {
            // 강도를 0으로 설정합니다.
            directionalLight.intensity = 0f;
        }
        else
        {
            // 그렇지 않다면 기본 강도로 설정합니다. (여기서는 1.0f로 설정)
            directionalLight.intensity = 1.0f;
        }
    }
}
