using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyListingManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentMoney;
    [SerializeField]
    private TextMeshProUGUI totalMoney;
    [SerializeField]
    private TextMeshProUGUI remainDept;
    [SerializeField]
    private TextMeshProUGUI thisWeekDept;

    public void RenderMoney() //È­¸é ÄÑÁú¶§ µ·ÀÌ¶û ºú Á¤º¸ °¡Á®¿À±â
    {
        currentMoney.text = PlayerStats.GetMoney().ToString("N0") + "\u20A9";
        totalMoney.text = PlayerStats.GetTotalMoney().ToString("N0") + "\u20A9";
        remainDept.text = PlayerStats.GetTotalDept().ToString("N0") + "\u20A9";
        thisWeekDept.text = PlayerStats.GetWeekDept().ToString("N0") + "\u20A9";
    }
}
