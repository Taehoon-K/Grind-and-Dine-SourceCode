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

    //������ Ű�� �� location
    const string LOCATION_KEY = "Location";
    const string INDOORS = "CurrentlyIndoor";

    public enum Location { Tutorial,Proto, Villar1, Villar2, Gym, Villar3, ChickenMini, SereneH, JakeH, 
        BbqMini, PoliceOffice, CoffeeShopMini, CharlesH,
        HospitalCut, Apart101, Yutnori, DeptInside, HomeGround, Lottery, RohanH, Basketball, Jail, BrickMoveMini,
        GasstaionMini
    } //�÷��̾ �̵��� ��
    public Location currentLocation;

    [SerializeField]
    GameObject ui;
    [SerializeField]
    GameObject halfEssential;

    private Transform playerPoint;
    //private Transform pivotCameraPoint;

    public UnityEvent onLoactionLoad;

    //[SerializeField] private GameObject tutorialManager;
    private bool tutorialOff; //�� ó�� Ʃ�� �����Ҷ��� Ʈ��,

    //private bool screenFadeOut;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (TutorialManager.instance != null)
            {
                // Ʃ�丮���� ���: ���� �ν��Ͻ��� �ı��ϰ� ���� �ν��Ͻ��� ����
                Destroy(Instance.gameObject);
                Instance = this;
            }
            else
            {
                // �Ϲ����� ���: ���� �ν��Ͻ��� �ı��ϰ� ���� ���� ����
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            // ó�� ������ ���
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        /*if (tutorialManager != null && !tutorialOff)
        {
            tutorialManager.SetActive(true);
            tutorialOff = true;
            //TutorialManager.instance.AfterLoad();
        }*/

        // �ߺ� ����
        SceneManager.sceneLoaded -= OnLocationLoad;
        SceneManager.sceneLoaded += OnLocationLoad;

        //�÷��̾� ��ġ ã��
        playerPoint = GameObject.FindGameObjectWithTag("Player").transform;
        //pivotCameraPoint = GameObject.FindGameObjectWithTag("FirstPCamera").transform;
    }

    //�ٸ� ���̶� ��ü
    public void SwitchLocation(Location locationToSwitch)
    {
        LoadingSceneController.Instance.LoadScene(locationToSwitch.ToString());
    }

    //�� �ε�ɶ� ȣ��
    public void OnLocationLoad(Scene scene,LoadSceneMode mode)
    {
        //�� �ε�ɶ� �÷��̾ ���� ��ġ
        Location oldLocation = currentLocation;

        if (scene.name == "Main Menu")
        {
            return;
        }

        //�� ��ġ
        Location newLocation = (Location)Enum.Parse(typeof(Location), scene.name);
        //Debug.Log(currentLocation+"aaaaaaaaaaaaaaaaaaaa" + newLocation);
        //���� ��� ���ο� ������ �°� �ƴ϶��

        if (newLocation == Location.HomeGround && oldLocation == Location.Tutorial) //���� Ʃ�丮�� �����̶��
        {
            if(TutorialManager.instance != null)
            {
                StatusManager.instance.FirstGameStart();
                playerPoint = GameObject.FindGameObjectWithTag("Player").transform;
            }
            
           
        }
        if (newLocation == Location.Proto && oldLocation == Location.HospitalCut) //���� Ʃ�丮�� �����̶��
        {
            StatusManager.instance.MoodleChange(9, true, 48 * 60); //48�ð� ����â��
        }

        if (newLocation == Location.Proto) 
        {
            if(UIManager.instance != null)
            {
                UIManager.instance.OnMinimap(true); //�̴ϸ� Ű��
            }
            
        }
        else
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.OnMinimap(false); //�̴ϸ� ����
            }
        }

        //��ŸƮ ����Ʈ ã��
        Transform startPoint = LocationManager.Instance.GetPlayerStartingPosition(oldLocation);
        Debug.Log(oldLocation.ToString() +"            "+ newLocation.ToString());

        if (startPoint == null) //���� ��ã�Ҵٸ�
        {
            startPoint = LocationManager.Instance.GetPlayerStartingPosition(Location.Proto); //�����䲨 ��������
        }


        //ĳ���� ��Ʈ�ѷ� ��Ȱ��
        CharacterController playerCharacter = playerPoint.GetComponent<CharacterController>();
        playerCharacter.enabled = false;

        playerPoint.position = startPoint.position;
        playerPoint.rotation = startPoint.rotation;
        //pivotCameraPoint.rotation = startPoint.rotation;
        playerPoint.GetComponent<Kupa.Player>().ResetRotation(startPoint);

        //�ٽ� Ȱ��ȭ
        playerCharacter.enabled = true;

        Debug.Log(playerPoint.position);

        currentLocation = newLocation;


        //�̴ϰ��� ���Խ� ui���� Ű�� �뵵
        if (scene.name == "ChickenMini" || scene.name == "CoffeeShopMini" || scene.name == "BbqMini"
            || scene.name == "Yutnori" || scene.name == "Basketball" || scene.name == "BrickMoveMini" || scene.name == "GasstaionMini")
        {
            // Ư�� ������ �̵��� �� Ư�� ������Ʈ�� ��Ȱ��ȭ
            ui.SetActive(false);
            halfEssential.SetActive(false);
            //objectWasActive = objectToDisable.activeSelf; // ���� ���� ����
        }
        else if(oldLocation == Location.ChickenMini || oldLocation == Location.CoffeeShopMini || oldLocation == Location.BbqMini
            || oldLocation == Location.Yutnori || oldLocation == Location.Basketball || oldLocation == Location.BrickMoveMini || oldLocation == Location.GasstaionMini)
        {
            // �ٸ� ������ �̵��� �� �ٽ� Ȱ��ȭ
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
        StartCoroutine(WaitForHomeManagerThenGenerate()); //Ȩ�Ŵ��� �ν��Ͻ� �������� ���
    }
    private IEnumerator WaitForHomeManagerThenGenerate()
    {
        yield return null; // �� ������ ��ٸ�
        // HomeManager�� ������ ������ �� ������ Ȯ��
        yield return new WaitUntil(() => HomeManager.instance != null);

        // �����Ǹ� �Լ� ����
        HomeManager.instance.GenTutoSean();
    }
    public void DestroyManager() //���θ޴� ���� �Ŵ��� ����
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
