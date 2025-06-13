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

    //������ ����
    List<ITimeTracker> listeners = new List<ITimeTracker>();

    [Header("Skybox")]
    [SerializeField]
    private Material dawnSkybox;  // ����/���� ��ī�̹ڽ� (06-09, 18-21)
    [SerializeField]
    private Material daySkybox;   // �� ��ī�̹ڽ� (09-18)
    [SerializeField]
    private Material nightSkybox; // �� ��ī�̹ڽ� (21-06)

    private Material currentSkybox; // ���� ����� ��ī�̹ڽ�
    private Material targetSkybox;  // ��ǥ ��ī�̹ڽ�
    private float transitionProgress = 1f; // ��ȯ ���൵ (0~1)
    private int lastHour = -1; // ���������� ������Ʈ�� �ð�

    private Light sunLight;

    public bool TimeTicking { get; set; } //�ð� �帣�� ���ߴ°� ����
    private void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
           // DontDestroyOnLoad(gameObject); //OnLoad(���� �ε� �Ǿ�����) �ڽ��� �ı����� �ʰ� ����
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }


    private void Start()
    {
        // �ʱ� ��ī�̹ڽ� ����
        currentSkybox = dawnSkybox; // �⺻�� ���� (�� ��ī�̹ڽ�)
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
    public void Tick() //�� �ʸ��� ȣ��� �Լ�
    {
        timestamp.UpdateClock();

        //������ ȣ��
        foreach(ITimeTracker listener in listeners)
        {
            listener.ClockUpdate(timestamp);
        }
        UpdateSunMovement();

        UpdateSkyboxTransition();
    }


    private void UpdateSunMovement()
    {
        // ���� �ð��� ������ ��ȯ
        int timeInMinutes = GameTimestamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        // ���� ������ ��ǥ ���� ���
        float currentSunAngle = .25f * timeInMinutes - 90;
        int nextMinute = (timeInMinutes + 1) % (24 * 60); // ���� �� ���
        float nextSunAngle = .25f * nextMinute - 90;

        // �� ���� �ð� ���
        float secondsInMinute = (float)timestamp.minute / 60f;

        // �ε巯�� �̵��� ���� ����
        float smoothTime = 60f; // 1�� ������ �̵��� ����Ͽ� ���� ������ ��
        float t = secondsInMinute;
        float smoothSunAngle = Mathf.Lerp(currentSunAngle, nextSunAngle, t);

        // �¾��� ������ ������Ʈ
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

        sunLight = GameObject.FindGameObjectWithTag("SunLight")?.GetComponent<Light>(); //�¾� ã��
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
        // �ð��뿡 ���� ��ǥ ��ī�̹ڽ� ����
        if (timestamp.hour >= 9 && timestamp.hour < 18) // ��
        {
            RenderSettings.skybox = daySkybox;

            if (sunLight != null) 
            {
                sunLight.intensity = 1.0f; //�¾� ���� �ִ��
            }
        }
        else if ((timestamp.hour >= 18 && timestamp.hour < 21) || (timestamp.hour >= 6 && timestamp.hour < 9)) // ����/����
        {
            RenderSettings.skybox = dawnSkybox;

            // ���� �ð��� ���� ���� ��� (0~1)
            float hourProgress = (timestamp.minute + (timestamp.hour % 3) * 60) / 180f;

            if (timestamp.hour >= 18 && timestamp.hour < 21) // ���� (�� -> ��)
            {
                RenderSettings.skybox.SetFloat("_CubemapTransition", Mathf.Clamp01(hourProgress));
                if (sunLight != null)
                {
                    sunLight.intensity = Mathf.Clamp01(1 - hourProgress); //�¾� ���� ����
                }
            }
            else // ���� (�� -> ��)
            {
                RenderSettings.skybox.SetFloat("_CubemapTransition", Mathf.Clamp01(1 - hourProgress));
                if (sunLight != null)
                {
                    sunLight.intensity = Mathf.Clamp01(hourProgress); //�¾� ���� ����
                }
            }
        }
        else // ��
        {
            RenderSettings.skybox = nightSkybox;

            if (sunLight != null)
            {
                sunLight.intensity = 0f; //�¾� ���� ���Ϸ�
            }
        }

    }


    public void SkipTime(int sleepTime, bool isSleep = false)
    {
        if (isSleep)
        {
            StatusManager.instance.currentSleepState = StatusManager.SleepState.Sleep; //�ڴ� ���·� �ٲ㼭 �Ƿε� �ö󰡰�
        }
        else
        {
            StatusManager.instance.currentSleepState = StatusManager.SleepState.Simulate; //hp ���� ���ϰ�
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
    public void SkipTimeToSix() //6�ñ��� ��ħ
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

    public bool isSiesta() //���� �����߼� �ִ� �ð����� Ȯ��
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

    public void AddActiveSystem() //���̵�ƿ��� �ε� ���� Ÿ�ӽ����� ����
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