using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DeptPrompt : UiPrompt
{
    [SerializeField]
    private TextMeshProUGUI totalDept, thisWeekToPay, remainDept;
    [SerializeField] TMP_InputField payAmount;

    private int currentMoney,amount;
    protected override void OnEnable()
    {
        base.OnEnable();
        Render(0);
        currentMoney = PlayerStats.GetMoney();
    }
    public void OnInputValueChanged(string value)
    {
        int number;
        if (int.TryParse(value, out number))  // ��ȯ �õ�
        {
            if (number > currentMoney) 
            { 
                number = currentMoney;
                payAmount.text = number.ToString();
            }
            else if(number < 0) 
            {
                number = 0;
                payAmount.text = number.ToString();
            }
            Render(number);
        }
        else
        {
            Render(0);
        }
        
    }

    private void Render(int amount)
    {
        totalDept.text = PlayerStats.GetTotalDept().ToString("N0") + "\u20A9";
        thisWeekToPay.text = PlayerStats.GetWeekDept().ToString("N0") + "\u20A9";
        int remainM = PlayerStats.GetWeekDept() - amount;
        remainDept.text = Mathf.Max(0,remainM).ToString("N0") + "\u20A9";

        this.amount = amount;
    }

    public void Confirm()  //Ȯ�ι�ư ������
    {
        if(amount <= 0)
        {
            return;
        }

        // ���� ���� �κ�
        PlayerStats.SpendDept(amount);

        // �� ���� �� ���� �Һ� ó��
        PlayerStats.Spend(amount);  // �Һ��ϴ� �κ��� ������ ó��

        //StatusManager.instance.RenderDept();
        gameObject.SetActive(false);
    }
}
