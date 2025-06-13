using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public class YesNoPrompt : UiPrompt
{
    [SerializeField]
    LocalizeStringEvent promptText;

    Action onYesSelected = null;

    public void CreatePrompt(LocalizedString message, Action onYesSelected)
    {
        this.onYesSelected = onYesSelected;
        promptText.StringReference = message;
    }
    public void Answer(bool yes) //��ư ������ �� ȣ��
    {
        if(yes && onYesSelected != null)
        {
            onYesSelected(); //�׼� ����
        }

        //reset action
        onYesSelected = null;

        gameObject.SetActive(false);
    }
}
