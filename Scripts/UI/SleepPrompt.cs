using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UIManager;

public class SleepPrompt : UiPrompt //�����
{
    private int sleepTime;
    [SerializeField]
    private GameObject screen;
    [SerializeField]
    private GameObject saveScreen;
    [SerializeField]
    TextMeshProUGUI sleepTimeText;
    [SerializeField]
    TextMeshProUGUI afterHpText; //���� �Ƿε�
    private int afterHp;
    private bool isJails; //��������
    [SerializeField] private GameObject saveButton;

    public void OpenScreen(bool isJail) //ȭ�� ų �� ȣ���ϴ� �Լ�
    {
        sleepTime = 1;
        RenderScreen();
        isJails = isJail;

        if (isJail)
        {
            saveButton.SetActive(false);
        }
        else
        {
            saveButton.SetActive(true);
        }
    }
    public void RenderScreen() //��ư ���������� �����ϴ� �Լ�
    {
        screen.SetActive(true);
        sleepTimeText.text = sleepTime.ToString();
        afterHp = Mathf.Clamp((int)(StatusManager.instance.GetHp() + sleepTime * 60 * 1.7f) / 10,0,100);
        afterHpText.text = afterHp + "%";

    }
    public void AddTime() //�ð� ���ϱ� ��ư
    {
        if(sleepTime < 6)
        {
            sleepTime++;
        }
        RenderScreen();
    }
    public void SubstractTime()
    {
        if(sleepTime > 1)
        {
            sleepTime--;
        }
        
        RenderScreen();
    }
    public void SaveButton() //���� ��ư ������
    {
        saveScreen.SetActive(true);
        screen.SetActive(false);
    }
    public void SleepButton() //�ڱ� ��ư ������
    {
        UIManager.instance.StartScreenFade(false, sleepTime);
        if (!isJails)
        {
            GameTimeStateManager.instance.Sleep(sleepTime); //���� �ƴҶ��� ����ǰ�
        }
        
        //StatusManager.instance.GetStatus().currentFatigue += sleepTime * 60 * 1.7f;

        screen.SetActive(false);
        //UIManager.instance.CurrentUIState = UIState.None;
    }

    public void Exit()
    {
        //UIManager.instance.CurrentUIState = UIState.None;
    }
}
