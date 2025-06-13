using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LotteryResult : MonoBehaviour
{
    [SerializeField] private GameObject success, fail;

    [SerializeField] private TextMeshProUGUI moneyText;

    private int moneyy;

    public void Render(int money)
    {
        moneyy = money;
        if (money == 0) 
        {
            fail.SetActive(true);
        }
        else
        {
            success.SetActive(true);
        }

        moneyText.text = "\u20A9" + money.ToString("N0");
    }

    private void OnDisable()
    {
        success.SetActive(false); 
        fail.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Confirm() //확인 버튼 누를 때 함수
    {
        PlayerStats.Earn(moneyy);
        UIManager.instance.ExitLotteryPanel();
    }
}
