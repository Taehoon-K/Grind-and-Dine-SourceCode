using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;

public class UiPrompt : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        UIManager.instance?.OnUIPromptOpened(gameObject); // UI 열릴 때 호출
    }

    protected virtual void OnDisable()
    {
        UIManager.instance?.OnUIPromptClosed(gameObject); // UI 닫힐 때 호출
    }
}
