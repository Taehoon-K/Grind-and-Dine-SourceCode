using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.IO;
using UnityEngine.Localization.Components;
using static UIManager;

public class SavePrompt : UiPrompt
{
    [SerializeField]
    private TextMeshProUGUI[] fileTime;
    [SerializeField]
    private TextMeshProUGUI[] saveInfoText;
    [SerializeField]
    private TextMeshProUGUI[] saveMoney;
    [SerializeField]
    private TextMeshProUGUI[] saveName;
    [SerializeField]
    private LocalizeStringEvent[] saveDiffy;

    [SerializeField]
    private GameObject saveConfirm;

    //private int saveIndex;
    private string info, money, name, diffy;

    private int saveSlot;


    public void SaveSlot(int slot) //원하는 저장 슬롯 눌렀을시
    {
        saveSlot = slot;
        if (SaveManager.HasSave(slot)) //만약 세이브 있을 시
        {
            saveConfirm.SetActive(true);
        }
        else
        {
            ConfirmSave();
        }
    }

    public void ConfirmSave() //저장 확정시 호출할 슬롯
    {
        GameTimeStateManager.instance.JustSave(saveSlot);
        gameObject.SetActive(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RenderScreen();
        //UIManager.instance.CurrentUIState = UIState.GeneralUI;
    }

    protected override void OnDisable()
    { 
        base.OnDisable();
       // UIManager.instance.CurrentUIState = UIState.None;
    }


    private void RenderScreen()
    {
        for (int i = 0; i < 4; i++)
        {
            bool hasSave = SaveManager.HasSave(i);
            //loadGameButton[i].interactable = SaveManager.HasSave(i);
            //loadGameButton[i].interactable = hasSave;
            if (hasSave)
            {
                fileTime[i].text = SaveManager.GetDateTime(i).ToString();
                LoadInfo(i);
                saveInfoText[i].text = info;
                saveMoney[i].text = money;
                saveName[i].text = name;
                saveDiffy[i].StringReference.TableEntryReference = diffy;
            }
        }
    }

    private void LoadInfo(int index)
    {
        GameSaveState saveInfo = SaveManager.Load(index);
        info = "Day " + saveInfo.timestamp.day + " of " + saveInfo.timestamp.season +
            ", Year " + saveInfo.timestamp.year;

        money = saveInfo.money.ToString();
        name = saveInfo.name;
        diffy = ChangeDiffy(saveInfo.difficulty);
    }

    private string ChangeDiffy(int value) //난이도 문자로 변환 함수
    {
        switch (value)
        {
            case 0:
                return "diffy0_key";
            case 1:
                return "diffy1_key";
            case 2:
                return "diffy2_key";
            case 3:
                return "diffy3_key";
            default:
                return "diffy0_key";
        }
    }
}
