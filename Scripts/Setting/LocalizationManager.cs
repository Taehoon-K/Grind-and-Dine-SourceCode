using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private void Start()
    {
        if (!PlayerPrefs.HasKey("Language"))  //��� ���� �ҷ�����
        {
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                StartCoroutine(LocaleChange(1));
            }
            else
            {
                StartCoroutine(LocaleChange(0));
            }
        }

        Cursor.lockState = CursorLockMode.Confined;
    }



    IEnumerator LocaleChange(int index)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
