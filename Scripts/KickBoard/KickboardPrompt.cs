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

    public void ClickButton(int index) //���� ��ư���� ������ �� ȣ��
    {
        if (PlayerStats.Money >= 2000) //�÷��̾� ���� 2000������ ���ٸ�
        {
            confirmNotice.SetActive(true);
            indexs = index;
        }
        else
        {
            //�������ϴ� �˶�
            noMoneyNotice.SetActive(true);
        }
    }

    public void ClickConfirm() //���� ��ư���� ������ �� ȣ��
    {
        PlayerStats.Spend(2000);
        ExtraManager.instance.MovePlayer(indexs);

        confirmNotice.SetActive(false);
        gameObject.SetActive(false);
    }
}
