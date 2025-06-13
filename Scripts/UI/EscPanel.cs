using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIManager;

public class EscPanel : UiPrompt
{
    [SerializeField]
    GameObject settingPanel;

    public void SettingOn()
    {
        settingPanel.SetActive(true);
    }
    public void BackToTitle()
    {
        SceneTransitionManager.Instance.DestroyManager();
        //SceneManager.LoadScene("Main Menu"); //메인메뉴로 이동
        LoadingSceneController.Instance.LoadScene("Main Menu");
    }
}
