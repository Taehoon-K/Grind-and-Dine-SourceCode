using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UIManager;

public class SleepPrompt : UiPrompt //낮잠용
{
    private int sleepTime;
    [SerializeField]
    private GameObject screen;
    [SerializeField]
    private GameObject saveScreen;
    [SerializeField]
    TextMeshProUGUI sleepTimeText;
    [SerializeField]
    TextMeshProUGUI afterHpText; //예상 피로도
    private int afterHp;
    private bool isJails; //감옥인지
    [SerializeField] private GameObject saveButton;

    public void OpenScreen(bool isJail) //화면 킬 때 호출하는 함수
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
    public void RenderScreen() //버튼 누를때마다 업뎃하는 함수
    {
        screen.SetActive(true);
        sleepTimeText.text = sleepTime.ToString();
        afterHp = Mathf.Clamp((int)(StatusManager.instance.GetHp() + sleepTime * 60 * 1.7f) / 10,0,100);
        afterHpText.text = afterHp + "%";

    }
    public void AddTime() //시간 더하기 버튼
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
    public void SaveButton() //저장 버튼 누를시
    {
        saveScreen.SetActive(true);
        screen.SetActive(false);
    }
    public void SleepButton() //자기 버튼 누를시
    {
        UIManager.instance.StartScreenFade(false, sleepTime);
        if (!isJails)
        {
            GameTimeStateManager.instance.Sleep(sleepTime); //감옥 아닐때만 저장되게
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
