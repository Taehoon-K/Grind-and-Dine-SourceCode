using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowButton : MonoBehaviour
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
        if (isPressed)
        {
            UpdateGauge();
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

        // �����̴��� ������ ���� �ݿ�
        gaugeSlider.fillAmount = currentGaugeValue;
    }

    // ��ư ������
    public void OnPress()
    {
        isPressed = true;
    }

    // ��ư ����
    public void OnRelease()
    {
        isPressed = false;
       // Debug.Log($"������ ��: {currentGaugeValue}");

        // �� ������ �Լ��� ������ �� ���� (��: YutThrower ��ũ��Ʈ ȣ��)
        FindObjectOfType<YutnoriManager>().ThrowYut(currentGaugeValue);
    }
}
