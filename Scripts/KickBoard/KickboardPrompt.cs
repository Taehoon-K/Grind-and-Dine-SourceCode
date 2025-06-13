using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KickboardPrompt : UiPrompt
{
    [SerializeField] private List<Button> stationButtons;
    [SerializeField] private GameObject noMoneyNotice, confirmNotice;

    private int indexs;
    public void Render(int currentIndex)
    {

        for (int i = 0; i < stationButtons.Count; i++)
        {
            Button btn = stationButtons[i];

            if (i == currentIndex)
                btn.interactable = false;
            else
                btn.interactable = true;

        }
    }

    public void ClickButton(int index) //만약 버튼에서 눌렀을 시 호출
    {
        if (PlayerStats.Money >= 2000) //플레이어 돈이 2000원보다 많다면
        {
            confirmNotice.SetActive(true);
            indexs = index;
        }
        else
        {
            //돈없습니다 알람
            noMoneyNotice.SetActive(true);
        }
    }

    public void ClickConfirm() //만약 버튼에서 눌렀을 시 호출
    {
        PlayerStats.Spend(2000);
        ExtraManager.instance.MovePlayer(indexs);

        confirmNotice.SetActive(false);
        gameObject.SetActive(false);
    }
}
