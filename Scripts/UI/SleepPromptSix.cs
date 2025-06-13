using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class SleepPromptSix : UiPrompt
{
    [SerializeField]
    private GameObject screen;
    [SerializeField]
    private GameObject saveScreen;
    private bool isJails; //�������� ����
    [SerializeField] private GameObject saveButton;
    public void OpenScreen(bool isJail) //ȭ�� ų �� ȣ���ϴ� �Լ�
    {
        screen.SetActive(true);
        isJails = isJail;
        
        if (isJail)
        {
            saveButton.SetActive(false);
        }
        else
        {
            if (saveButton != null)
            {
                saveButton.SetActive(true);
            }
        }
    }
    public void SaveButton() //���� ��ư ������, ���⼱ �Ⱦ�
    {
        saveScreen.SetActive(true);
        screen.SetActive(false);
    }
    public void SleepButton() //�ڱ� ��ư ������
    {
        UIManager.instance.StartScreenFade(true);
        GameTimeStateManager.instance.SleepUntilSix(isJails);

       // StatusManager.instance.GetStatus().currentFatigue += sleepTime * 100;

        screen.SetActive(false);
        //UIManager.instance.CurrentUIState = UIState.None;
        if(TimeManager.instance.GetGameTimestamp().hour < 21 && StatusManager.instance.GetSelectedPerk(0, 1) == 0) //���� 9�� �� ��ħ�̰� �� ����ٸ�
        {
            UIManager.instance.NoticeItemCreate(8, 2);
            StatusManager.instance.DestroyMoodleOne();
        }
    }

    public void Exit()
    {
        //UIManager.instance.CurrentUIState = UIState.None;
    }
}
