using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance = null;
    [SerializeField]
    private GameTimestamp timestamp;



    public float timeScale = 1.0f;

    //[SerializeField] private Transform sunTransform;

    //관찰자 패턴
    List<ITimeTracker> listeners = new List<ITimeTracker>();

    [Header("Skybox")]
    [SerializeField]
    private Material dawnSkybox;  // 새벽/저녁 스카이박스 (06-09, 18-21)
    [SerializeField]
    private Material daySkybox;   // 낮 스카이박스 (09-18)
    [SerializeField]
    private Material nightSkybox; // 밤 스카이박스 (21-06)

    private Material currentSkybox; // 현재 적용된 스카이박스
    private Material targetSkybox;  // 목표 스카이박스
    private float transitionProgress = 1f; // 전환 진행도 (0~1)
    private int lastHour = -1; // 마지막으로 업데이트된 시간

    private Light sunLight;

    public bool TimeTicking { get; set; } //시간 흐르고 멈추는것 관리
    private void Awake()
    {
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
           // DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }


    private void Start()
    {
        // 초기 스카이박스 설정
        currentSkybox = dawnSkybox; // 기본값 설정 (낮 스카이박스)
        RenderSettings.skybox = currentSkybox;

        timestamp = new GameTimestamp(1,GameTimestamp.Season.Spring, 1,7,0);
        TimeTicking = true;
        StartCoroutine(TimeUpdate());
    }

    public void LoadTime(GameTimestamp timestamp)
    {
        this.timestamp = timestamp;
    }

    IEnumerator TimeUpdate()
    {
        while (true)
        {
            if (TimeTicking)
            {
                Tick();
            }
            yield return new WaitForSeconds(2/timeScale);
            
        }
    }
    public void Tick() //매 초마다 호출될 함수
    {
        timestamp.UpdateClock();

        //리스너 호출
        foreach(ITimeTracker listener in listeners)
        {
            listener.ClockUpdate(timestamp);
        }
        UpdateSunMovement();

        UpdateSkyboxTransition();
    }


    private void UpdateSunMovement()
    {
        // 현재 시간을 분으로 변환
        int timeInMinutes = GameTimestamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        // 현재 각도와 목표 각도 계산
        float currentSunAngle = .25f * timeInMinutes - 90;
        int nextMinute = (timeInMinutes + 1) % (24 * 60); // 다음 분 계산
        float nextSunAngle = .25f * nextMinute - 90;

        // 초 단위 시간 계산
        float secondsInMinute = (float)timestamp.minute / 60f;

        // 부드러운 이동을 위한 보간
        float smoothTime = 60f; // 1분 동안의 이동을 고려하여 조절 가능한 값
        float t = secondsInMinute;
        float smoothSunAngle = Mathf.Lerp(currentSunAngle, nextSunAngle, t);

        // 태양의 각도를 업데이트
        //sunTransform.eulerAngles = new Vector3(smoothSunAngle, 0, 0);

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentSkybox != null)
        {
            RenderSettings.skybox = currentSkybox;
        }

        sunLight = GameObject.FindGameObjectWithTag("SunLight")?.GetComponent<Light>(); //태양 찾기
        if (sunLight != null)
        {
            Debug.Log("Sun Light found: " + sunLight.name);
        }
        else
        {
            Debug.Log("No Light component found on object with tag 'Sun'");
        }
    }
    private void UpdateSkyboxTransition()
    {
        // 시간대에 따라 목표 스카이박스 설정
        if (timestamp.hour >= 9 && timestamp.hour < 18) // 낮
        {
            RenderSettings.skybox = daySkybox;

            if (sunLight != null) 
            {
                sunLight.intensity = 1.0f; //태양 강도 최대로
            }
        }
        else if ((timestamp.hour >= 18 && timestamp.hour < 21) || (timestamp.hour >= 6 && timestamp.hour < 9)) // 새벽/저녁
        {
            RenderSettings.skybox = dawnSkybox;

            // 현재 시간의 진행 정도 계산 (0~1)
            float hourProgress = (timestamp.minute + (timestamp.hour % 3) * 60) / 180f;

            if (timestamp.hour >= 18 && timestamp.hour < 21) // 저녁 (낮 -> 밤)
            {
                RenderSettings.skybox.SetFloat("_CubemapTransition", Mathf.Clamp01(hourProgress));
                if (sunLight != null)
                {
                    sunLight.intensity = Mathf.Clamp01(1 - hourProgress); //태양 강도 조정
                }
            }
            else // 새벽 (밤 -> 낮)
            {
                RenderSettings.skybox.SetFloat("_CubemapTransition", Mathf.Clamp01(1 - hourProgress));
                if (sunLight != null)
                {
                    sunLight.intensity = Mathf.Clamp01(hourProgress); //태양 강도 조정
                }
            }
        }
        else // 밤
        {
            RenderSettings.skybox = nightSkybox;

            if (sunLight != null)
            {
                sunLight.intensity = 0f; //태양 강도 최하로
            }
        }

    }


    public void SkipTime(int sleepTime, bool isSleep = false)
    {
        if (isSleep)
        {
            StatusManager.instance.currentSleepState = StatusManager.SleepState.Sleep; //자는 상태로 바꿔서 피로도 올라가게
        }
        else
        {
            StatusManager.instance.currentSleepState = StatusManager.SleepState.Simulate; //hp 감소 안하게
        }
        int skipMinute = sleepTime * 60;

        if(skipMinute <= 0)
        {
            return;
        }

        for(int i = 0; i < skipMinute; i++)
        {
            Tick();
        }
        StatusManager.instance.currentSleepState = StatusManager.SleepState.Awake;
    }
    public void SkipTimeToSix() //6시까지 취침
    {
        StatusManager.instance.currentSleepState = StatusManager.SleepState.Sleep;
        // Calculate the current time in minutes
        int currentTimeInMinutes = GameTimestamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        // Calculate the target time (6 AM in minutes)
        int targetTimeInMinutes = GameTimestamp.HoursToMinutes(7); // 6 AM

        // Calculate how many minutes to skip
        int minutesToSkip = targetTimeInMinutes - currentTimeInMinutes;

        // If it's already after 6 AM, skip to 6 AM the next day
        if (minutesToSkip <= 0)
        {
            minutesToSkip += 24 * 60; // Add 24 hours in minutes to jump to the next day's 6 AM
        }

        Debug.Log(minutesToSkip+"                aaaaaaa")
            ;

        // Execute Tick() for the calculated minutes
        for (int i = 0; i < minutesToSkip; i++)
        {
            Tick();
        }

        StatusManager.instance.currentSleepState = StatusManager.SleepState.Awake;
    }

    //handling listeners
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }
    public void UnregisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }
    public GameTimestamp GetGameTimestamp()
    {
        return timestamp;
    }

    public bool isSiesta() //만약 낮잠잘수 있는 시간인지 확인
    {
        if(timestamp.hour >=6 && timestamp.hour <= 12)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private int activeCount = 0;

    public void AddActiveSystem() //페이드아웃과 로딩 때의 타임스케일 관리
    {
        activeCount++;
        Time.timeScale = 0f;
    }

    public void RemoveActiveSystem()
    {
        activeCount = Mathf.Max(0, activeCount - 1);
        if (activeCount == 0)
        {
            Time.timeScale = 1f;
        }
    }
}