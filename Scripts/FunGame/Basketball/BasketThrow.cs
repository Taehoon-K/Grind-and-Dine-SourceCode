using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BasketThrow : MonoBehaviour
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
        if (BasketBallManager.instance.IsPlayerTurn)
        {
            if (isPressed)
            {
                UpdateGauge();
            }
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

        // UI 슬라이더 업데이트
        if (gaugeSlider != null)
        {
            gaugeSlider.fillAmount = currentGaugeValue;
        }
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                isPressed = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                // 버튼을 뗐을 때
                isPressed = false;

                if(BasketBallManager.instance.IsPlayerTurn && !BasketBallManager.instance.isShoot)
                {
                    BasketBallManager.instance.isShoot = true;
                    // 슛 실행
                    BasketBallManager.instance.ShootBall(currentGaugeValue);
                }
                //BasketBallManager.instance.ShootNpc(true, currentGaugeValue);

                // 게이지 초기화
                currentGaugeValue = 0f;
                isIncreasing = true;

                // UI 슬라이더 초기화
                if (gaugeSlider != null)
                {
                    gaugeSlider.fillAmount = currentGaugeValue;
                }
                break;
        }
    }
}
