using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance = null;

    [SerializeField]
    private GameObject bed; //침대 캐릭터

    [Header("Sunny")]
    [SerializeField]
    private Animator sunnyAni; //순희 애니메이션 연결
    [SerializeField]
    private GameObject watch;
    public bool eatWatch,eatBread, offInven; //시계먹었는지
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
        if (instance != null && instance != this) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
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
        StatusManager.instance.LoadGameStart(-1); //캐릭터 불러오기
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
        TimeManager.instance.GetGameTimestamp().hour = 21; //21시로 변경
        TimeManager.instance.TimeTicking = false;
    }

    public void StartBehavior() //다이얼로그 완료시 호출
    {
        gameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
    }

    public void CompleteDialogue()
    {
        RelationshipStats.UnlockCharacter(sunny); //순희 언락
        UIManager.instance.NoticeDelete();
        //첫 다이얼 끝나면 시계 젠 하고 애니 실행
        watch.SetActive(true);
        sunnyAni.SetBool("Hand", true);
    }

    public void OnCanvas() //스탯캔버스 활성화
    {
        sunnyAni.SetBool("Hand", false);
        Destroy(watch); //시계 삭제
        realWatch.SetActive(true);
        //statCanvas.SetActive(true);

        UIManager.instance.NoticeCreateEver("tutorialWatch_key");
    }
    public void PlaySecondDial() //두번째 다이얼 실행, 비해비어 트리에서 실행할것
    {
        playeryarn.CheckForNearbyNPC("Tutorial2", sunnyAni.transform);

        UIManager.instance.NoticeDelete();
    }

    public void Bread() //스탯캔버스 활성화
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

    public void DestroyWall() //벽 부수기
    {
        StartCoroutine(PlayThirdDialCoroutine());
    }
    private IEnumerator PlayThirdDialCoroutine()
    {
        // 0.5초 대기
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
    private void LoadGame() //씬 로드되고 호출
    {
        if (GameTimeStateManager.instance == null)
        {
            Debug.LogError("Cannot Find gametimestate Manager");
        }
        // StatusManager.instance.LoadGameStart(0);
        // StatusManager.instance.FirstGameStart();

        //RelationshipStats.UnlockCharacter(sunny); //순희 언락
        SceneTransitionManager.Instance.StartGenSean();

        RelationshipStats.UnlockCharacter(sunny); //순희 언락
        StatusManager.instance.CollectionOpen(0, true); //삼각김밥 다시 언락
    }

    public void Continue()
    {
        StartCoroutine(LoadGameAsync(LoadGame));
    }

    IEnumerator LoadGameAsync(System.Action onFirstFrameLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("HomeGround");
        //다른 씬 로드되고 계속 작업 가능하게
        DontDestroyOnLoad(gameObject);

        //씬 로드까지 대기
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
