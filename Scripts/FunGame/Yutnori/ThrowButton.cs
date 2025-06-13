using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowButton : MonoBehaviour
{
    [SerializeField]
    private Image gaugeSlider; // UI 슬라이더 (게이지 바)
    [SerializeField]
    private float gaugeSpeed = 2f; // 게이지가 변화하는 속도
    private bool isPressed = false; // 버튼이 눌렸는지 여부
    private float currentGaugeValue = 0f; // 현재 게이지 값
    private bool isIncreasing = true; // 게이지가 증가 중인지 여부

    private void OnEnable()
    {
        currentGaugeValue = 0f;
        gaugeSlider.fillAmount = 0;
    }

    void Update()
    {
        if (isPressed)
        {
            UpdateGauge();
        }
    }

    // 게이지 업데이트
    private void UpdateGauge()
    {
        if (isIncreasing)
        {
            currentGaugeValue += Time.deltaTime * gaugeSpeed;

            if (currentGaugeValue >= 1f) // 게이지가 최대치에 도달하면 감소로 전환
            {
                currentGaugeValue = 1f;
                isIncreasing = false;
            }
        }
        else
        {
            currentGaugeValue -= Time.deltaTime * gaugeSpeed;

            if (currentGaugeValue <= 0f) // 게이지가 최소치에 도달하면 증가로 전환
            {
                currentGaugeValue = 0f;
                isIncreasing = true;
            }
        }

        // 슬라이더에 게이지 값을 반영
        gaugeSlider.fillAmount = currentGaugeValue;
    }

    // 버튼 누르기
    public void OnPress()
    {
        isPressed = true;
    }

    // 버튼 떼기
    public void OnRelease()
    {
        isPressed = false;
       // Debug.Log($"게이지 값: {currentGaugeValue}");

        // 윷 던지기 함수에 게이지 값 전달 (예: YutThrower 스크립트 호출)
        FindObjectOfType<YutnoriManager>().ThrowYut(currentGaugeValue);
    }
}
