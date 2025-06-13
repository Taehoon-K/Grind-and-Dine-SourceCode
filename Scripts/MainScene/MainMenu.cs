using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.IO;
using UnityEngine.Localization.Components;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button[] loadGameButton;
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

    private int saveIndex;

    private int currentDiffy; //���� ���õ� ���̵� ��, ���� ���۽� �ݿ��ɰ�
    private bool iswoman; //Ʈ�� ���� ���� ����
    private string info, money, name, diffy;
    public void NewGameStart()
    {
        //���̵� �־ �� ���� ȣ��
        //SceneManager.LoadScene(SceneTransitionManager.Location.StartHouse.ToString());
        // StartCoroutine(LoadGameAsync(SceneTransitionManager.Location.StartBus, StartFirstGame));

        // SceneManager.LoadScene("StartBus");
        PlayerStats.SetSex(iswoman);
        PlayerStats.SetDiffy(currentDiffy);
        PlayerStats.DeptInitilaze();
        LoadingSceneController.Instance.LoadScene("StartBus");

        //StatusManager.instance.RenderDept();
    }
    private void StartFirstGame() //�� �ε�ǰ� ȣ��
    {
        if (GameTimeStateManager.instance == null)
        {
            Debug.LogError("Cannot Find gametimestate Manager");
        }
        PlayerStats.SetSex(iswoman);
        PlayerStats.SetDiffy(currentDiffy);
        StatusManager.instance.FirstGameStart(); //���� hp �̷��͵� �� ä��� ����
        //GameTimeStateManager.instance.LoadSave(saveIndex);

    }

    public void SetSex(int sex)
    {
        if(sex == 0) //���ڸ�
        {
            iswoman = false;
        }
        else  //���ڸ�
        {
            iswoman = true;
        }
    }
    public void SetDiffy(int diffy)
    {
        currentDiffy = diffy;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadGame() //�� �ε�ǰ� ȣ��
    {
        if(GameTimeStateManager.instance == null)
        {
            Debug.LogError("Cannot Find gametimestate Manager");
        }
        GameTimeStateManager.instance.LoadSave(saveIndex);
        StatusManager.instance.LoadGameStart(saveIndex); 
        StatusManager.instance.RenderDept();

        Destroy(gameObject);
    }

    public void Continue(int slot)
    {
        saveIndex = slot;
        DontDestroyOnLoad(gameObject);
        LoadingSceneController.Instance.LoadScene("HomeGround",LoadGame);
        //StartCoroutine(LoadGameAsync(SceneTransitionManager.Location.HomeGround, LoadGame));
    }

    IEnumerator LoadGameAsync(SceneTransitionManager.Location scene,System.Action onFirstFrameLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.ToString());
        //�ٸ� �� �ε�ǰ� ��� �۾� �����ϰ�
        DontDestroyOnLoad(gameObject);

        //�� �ε���� ���
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        Debug.Log("first frame loaded");
        onFirstFrameLoad?.Invoke();

        //Destroy(gameObject);
    }

    private void Start()
    {
        for(int i = 0; i < loadGameButton.Length; i++)
        {
            bool hasSave = SaveManager.HasSave(i);
            //loadGameButton[i].interactable = SaveManager.HasSave(i);
            loadGameButton[i].interactable = hasSave;
            if (hasSave)
            {
                fileTime[i].text = SaveManager.GetDateTime(i).ToString();
                LoadInfo(i);
                saveInfoText[i].text = info;
                saveMoney[i].text = money + "\u20A9";
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

    private string ChangeDiffy(int value) //���̵� ���ڷ� ��ȯ �Լ�
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
