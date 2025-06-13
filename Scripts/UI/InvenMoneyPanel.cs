using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InvenMoneyPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentMoney,totalEarn,totalDept, thisWeekToPay;

    private void Start()
    {
        Render();
    }
    private void OnEnable()
    {
        Render();
    }

    private void Render()
    {
        currentMoney.text = PlayerStats.GetMoney().ToString("N0") + "\u20A9";
        totalEarn.text = PlayerStats.GetTotalMoney().ToString("N0") + "\u20A9";
        totalDept.text = PlayerStats.GetTotalDept().ToString("N0") + "\u20A9";
        thisWeekToPay.text = PlayerStats.GetWeekDept().ToString("N0") + "\u20A9";
    }

}
