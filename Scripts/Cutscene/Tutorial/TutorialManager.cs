using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance = null;

    [SerializeField]
    private GameObject bed; //ħ�� ĳ����

    [Header("Sunny")]
    [SerializeField]
    private Animator sunnyAni; //���� �ִϸ��̼� ����
    [SerializeField]
    private GameObject watch;
    public bool eatWatch,eatBread, offInven; //�ð�Ծ�����
    [SerializeField] NPC sunny;

    [Header("Others")]
    /*[SerializeField]
    private GameObject statCanvas;*/
    [SerializeField]
    private GameObject realWatch;
    [SerializeField]
    private GameObject bread;
    [SerializeField]
    private GameObject wall;

    [SerializeField] private Transform house;

    PlayerYarn playeryarn;
    YarnCommand yarnCommand;

    private void Awake()
    {
        if (instance != null && instance != this) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    public void Start()
    {
        Debug.Log("Aterloaddd");
        StatusManager.instance.LoadGameStart(-1); //ĳ���� �ҷ�����
        TimeManager.instance.TimeTicking = false;
        realWatch = GameObject.FindWithTag("Watch");
        realWatch.SetActive(false);
        //statCanvas.SetActive(false);
        playeryarn = FindObjectOfType<PlayerYarn>();

        UIManager.instance.NoticeCreateEver("tutorialwasd_key");

        yarnCommand = FindObjectOfType<YarnCommand>();
        yarnCommand.giveWatch.AddListener(StartBehavior);
        yarnCommand.giveBread.AddListener(Bread);
        yarnCommand.completeSean.AddListener(CompleteSean);

        StartCoroutine(DelayedCheck());
    }

    private IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(0.5f);
        playeryarn.CheckForNearbyNPC("tutorial0", house);
        TimeManager.instance.GetGameTimestamp().hour = 21; //21�÷� ����
        TimeManager.instance.TimeTicking = false;
    }

    public void StartBehavior() //���̾�α� �Ϸ�� ȣ��
    {
        gameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
    }

    public void CompleteDialogue()
    {
        RelationshipStats.UnlockCharacter(sunny); //���� ���
        UIManager.instance.NoticeDelete();
        //ù ���̾� ������ �ð� �� �ϰ� �ִ� ����
        watch.SetActive(true);
        sunnyAni.SetBool("Hand", true);
    }

    public void OnCanvas() //����ĵ���� Ȱ��ȭ
    {
        sunnyAni.SetBool("Hand", false);
        Destroy(watch); //�ð� ����
        realWatch.SetActive(true);
        //statCanvas.SetActive(true);

        UIManager.instance.NoticeCreateEver("tutorialWatch_key");
    }
    public void PlaySecondDial() //�ι�° ���̾� ����, ���غ�� Ʈ������ �����Ұ�
    {
        playeryarn.CheckForNearbyNPC("Tutorial2", sunnyAni.transform);

        UIManager.instance.NoticeDelete();
    }

    public void Bread() //����ĵ���� Ȱ��ȭ
    {
        if (offInven && !eatBread)
        {
            sunnyAni.SetBool("Hand", true);
            bread.SetActive(true);
        }
    }
    public void OffBread()
    {
        sunnyAni.SetBool("Hand", false);
        bread.SetActive(false);

        UIManager.instance.NoticeCreateEver("tutorialtab_key");
    }

    public void DestroyWall() //�� �μ���
    {
        StartCoroutine(PlayThirdDialCoroutine());
    }
    private IEnumerator PlayThirdDialCoroutine()
    {
        // 0.5�� ���
        yield return new WaitForSeconds(0.5f);
        UIManager.instance.NoticeDelete();
        playeryarn.CheckForNearbyNPC("Tutorial3", sunnyAni.transform);
        bed.tag = "Sleep";
    }
    public void CompleteSean()
    {
        wall.SetActive(false);
        TimeManager.instance.TimeTicking = true;
    }

    public void SleepBed()
    {
        UIManager.instance.StartScreenFadeTutorial();
    }

    #region Scene
    private void LoadGame() //�� �ε�ǰ� ȣ��
    {
        if (GameTimeStateManager.instance == null)
        {
            Debug.LogError("Cannot Find gametimestate Manager");
        }
        // StatusManager.instance.LoadGameStart(0);
        // StatusManager.instance.FirstGameStart();

        //RelationshipStats.UnlockCharacter(sunny); //���� ���
        SceneTransitionManager.Instance.StartGenSean();

        RelationshipStats.UnlockCharacter(sunny); //���� ���
        StatusManager.instance.CollectionOpen(0, true); //�ﰢ��� �ٽ� ���
    }

    public void Continue()
    {
        StartCoroutine(LoadGameAsync(LoadGame));
    }

    IEnumerator LoadGameAsync(System.Action onFirstFrameLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("HomeGround");
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

        Destroy(gameObject);
    }
    #endregion
}
