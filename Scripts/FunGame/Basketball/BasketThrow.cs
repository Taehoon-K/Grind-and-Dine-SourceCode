using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BasketThrow : MonoBehaviour
{
    [SerializeField]
    private Image gaugeSlider; // UI �����̴� (������ ��)
    [SerializeField]
    private float gaugeSpeed = 2f; // �������� ��ȭ�ϴ� �ӵ�
    private bool isPressed = false; // ��ư�� ���ȴ��� ����
    private float currentGaugeValue = 0f; // ���� ������ ��
    private bool isIncreasing = true; // �������� ���� ������ ����

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

    // ������ ������Ʈ
    private void UpdateGauge()
    {
        if (isIncreasing)
        {
            currentGaugeValue += Time.deltaTime * gaugeSpeed;

            if (currentGaugeValue >= 1f) // �������� �ִ�ġ�� �����ϸ� ���ҷ� ��ȯ
            {
                currentGaugeValue = 1f;
                isIncreasing = false;
            }
        }
        else
        {
            currentGaugeValue -= Time.deltaTime * gaugeSpeed;

            if (currentGaugeValue <= 0f) // �������� �ּ�ġ�� �����ϸ� ������ ��ȯ
            {
                currentGaugeValue = 0f;
                isIncreasing = true;
            }
        }

        // UI �����̴� ������Ʈ
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
                // ��ư�� ���� ��
                isPressed = false;

                if(BasketBallManager.instance.IsPlayerTurn && !BasketBallManager.instance.isShoot)
                {
                    BasketBallManager.instance.isShoot = true;
                    // �� ����
                    BasketBallManager.instance.ShootBall(currentGaugeValue);
                }
                //BasketBallManager.instance.ShootNpc(true, currentGaugeValue);

                // ������ �ʱ�ȭ
                currentGaugeValue = 0f;
                isIncreasing = true;

                // UI �����̴� �ʱ�ȭ
                if (gaugeSlider != null)
                {
                    gaugeSlider.fillAmount = currentGaugeValue;
                }
                break;
        }
    }
}
