using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    //블랙보드 키에 쓸 location
    const string LOCATION_KEY = "Location";
    const string INDOORS = "CurrentlyIndoor";

    public enum Location { Tutorial,Proto, Villar1, Villar2, Gym, Villar3, ChickenMini, SereneH, JakeH, 
        BbqMini, PoliceOffice, CoffeeShopMini, CharlesH,
        HospitalCut, Apart101, Yutnori, DeptInside, HomeGround, Lottery, RohanH, Basketball, Jail, BrickMoveMini,
        GasstaionMini
    } //플레이어가 이동할 씬
    public Location currentLocation;

    [SerializeField]
    GameObject ui;
    [SerializeField]
    GameObject halfEssential;

    private Transform playerPoint;
    //private Transform pivotCameraPoint;

    public UnityEvent onLoactionLoad;

    //[SerializeField] private GameObject tutorialManager;
    private bool tutorialOff; //맨 처음 튜토 실행할때만 트루,

    //private bool screenFadeOut;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (TutorialManager.instance != null)
            {
                // 튜토리얼일 경우: 기존 인스턴스를 파괴하고 현재 인스턴스를 유지
                Destroy(Instance.gameObject);
                Instance = this;
            }
            else
            {
                // 일반적인 경우: 현재 인스턴스를 파괴하고 기존 것을 유지
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            // 처음 생성된 경우
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        /*if (tutorialManager != null && !tutorialOff)
        {
            tutorialManager.SetActive(true);
            tutorialOff = true;
            //TutorialManager.instance.AfterLoad();
        }*/

        // 중복 방지
        SceneManager.sceneLoaded -= OnLocationLoad;
        SceneManager.sceneLoaded += OnLocationLoad;

        //플레이어 위치 찾기
        playerPoint = GameObject.FindGameObjectWithTag("Player").transform;
        //pivotCameraPoint = GameObject.FindGameObjectWithTag("FirstPCamera").transform;
    }

    //다른 씬이랑 교체
    public void SwitchLocation(Location locationToSwitch)
    {
        LoadingSceneController.Instance.LoadScene(locationToSwitch.ToString());
    }

    //씬 로드될때 호출
    public void OnLocationLoad(Scene scene,LoadSceneMode mode)
    {
        //씬 로드될때 플레이어가 오는 위치
        Location oldLocation = currentLocation;

        if (scene.name == "Main Menu")
        {
            return;
        }

        //새 위치
        Location newLocation = (Location)Enum.Parse(typeof(Location), scene.name);
        //Debug.Log(currentLocation+"aaaaaaaaaaaaaaaaaaaa" + newLocation);
        //만약 어디 새로운 곳에서 온게 아니라면

        if (newLocation == Location.HomeGround && oldLocation == Location.Tutorial) //만약 튜토리얼 시작이라면
        {
            if(TutorialManager.instance != null)
            {
                StatusManager.instance.FirstGameStart();
                playerPoint = GameObject.FindGameObjectWithTag("Player").transform;
            }
            
           
        }
        if (newLocation == Location.Proto && oldLocation == Location.HospitalCut) //만약 튜토리얼 시작이라면
        {
            StatusManager.instance.MoodleChange(9, true, 48 * 60); //48시간 만신창이
        }

        if (newLocation == Location.Proto) 
        {
            if(UIManager.instance != null)
            {
                UIManager.instance.OnMinimap(true); //미니맵 키기
            }
            
        }
        else
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.OnMinimap(false); //미니맵 끄기
            }
        }

        //스타트 포인트 찾기
        Transform startPoint = LocationManager.Instance.GetPlayerStartingPosition(oldLocation);
        Debug.Log(oldLocation.ToString() +"            "+ newLocation.ToString());

        if (startPoint == null) //만약 못찾았다면
        {
            startPoint = LocationManager.Instance.GetPlayerStartingPosition(Location.Proto); //프로토꺼 가져오기
        }


        //캐릭터 컨트롤러 비활성
        CharacterController playerCharacter = playerPoint.GetComponent<CharacterController>();
        playerCharacter.enabled = false;

        playerPoint.position = startPoint.position;
        playerPoint.rotation = startPoint.rotation;
        //pivotCameraPoint.rotation = startPoint.rotation;
        playerPoint.GetComponent<Kupa.Player>().ResetRotation(startPoint);

        //다시 활성화
        playerCharacter.enabled = true;

        Debug.Log(playerPoint.position);

        currentLocation = newLocation;


        //미니게임 진입시 ui껐다 키는 용도
        if (scene.name == "ChickenMini" || scene.name == "CoffeeShopMini" || scene.name == "BbqMini"
            || scene.name == "Yutnori" || scene.name == "Basketball" || scene.name == "BrickMoveMini" || scene.name == "GasstaionMini")
        {
            // 특정 씬으로 이동할 때 특정 오브젝트를 비활성화
            ui.SetActive(false);
            halfEssential.SetActive(false);
            //objectWasActive = objectToDisable.activeSelf; // 이전 상태 저장
        }
        else if(oldLocation == Location.ChickenMini || oldLocation == Location.CoffeeShopMini || oldLocation == Location.BbqMini
            || oldLocation == Location.Yutnori || oldLocation == Location.Basketball || oldLocation == Location.BrickMoveMini || oldLocation == Location.GasstaionMini)
        {
            // 다른 씬으로 이동할 때 다시 활성화
            ui.SetActive(true);
            halfEssential.SetActive(true);
        }

        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
        blackboard.SetValue(LOCATION_KEY, currentLocation);
        //blackboard.SetValue(INDOORS, curr)

        //SceneManager.sceneLoaded -= OnLocationLoad;
        onLoactionLoad?.Invoke();

        CutsceneManager.Instance.OnLocationLoad();
    }
    public void StartGenSean()
    {
        StartCoroutine(WaitForHomeManagerThenGenerate()); //홈매니저 인스턴스 생성까지 대기
    }
    private IEnumerator WaitForHomeManagerThenGenerate()
    {
        yield return null; // 한 프레임 기다림
        // HomeManager가 생성될 때까지 매 프레임 확인
        yield return new WaitUntil(() => HomeManager.instance != null);

        // 생성되면 함수 실행
        HomeManager.instance.GenTutoSean();
    }
    public void DestroyManager() //메인메뉴 갈시 매니저 삭제
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnLocationLoad;
        }
    }
}
