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
    private bool isJails; //감옥인지 여부
    [SerializeField] private GameObject saveButton;
    public void OpenScreen(bool isJail) //화면 킬 때 호출하는 함수
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
    public void SaveButton() //저장 버튼 누를시, 여기선 안씀
    {
        saveScreen.SetActive(true);
        screen.SetActive(false);
    }
    public void SleepButton() //자기 버튼 누를시
    {
        UIManager.instance.StartScreenFade(true);
        GameTimeStateManager.instance.SleepUntilSix(isJails);

       // StatusManager.instance.GetStatus().currentFatigue += sleepTime * 100;

        screen.SetActive(false);
        //UIManager.instance.CurrentUIState = UIState.None;
        if(TimeManager.instance.GetGameTimestamp().hour < 21 && StatusManager.instance.GetSelectedPerk(0, 1) == 0) //만약 9시 전 취침이고 퍽 찍었다면
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
