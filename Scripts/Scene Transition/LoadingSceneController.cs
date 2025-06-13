using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Components;

public class LoadingSceneController : MonoBehaviour
{
    public static LoadingSceneController instance;


    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Image progressBar;

    [Header("Tip")]
    [SerializeField]
    private int tipCount;
    [SerializeField]
    protected LocalizeStringEvent tipString; //시간 텍스트랑 같은 옵젝

    [Header("Image")]
    [SerializeField]
    private Sprite[] bgImages;
    [SerializeField]
    private Image image;
    private float audioVolume;
    public static LoadingSceneController Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<LoadingSceneController>();
                if(obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = Create();
                }
            }
            return instance;
        }
    }
    private static LoadingSceneController Create()
    {
        return Instantiate(Resources.Load<LoadingSceneController>("LoadingUI"));
    }
    private void Awake()
    {
        if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        int randomInt = Random.Range(0, tipCount) + 1;
        tipString.StringReference.TableEntryReference = "tip" + randomInt.ToString();

        int randomInt2 = Random.Range(0, bgImages.Length);
        image.sprite = bgImages[randomInt2];

        if(TimeManager.instance != null)
        {
            TimeManager.instance.AddActiveSystem(); // 타임스케일 0으로 유지
        }
        
    }
    private void OnDisable()
    {
        if (TimeManager.instance != null)
        {
            TimeManager.instance.RemoveActiveSystem(); // 비활성화될 때 타임스케일 조정
        } 
    }

    private string loadSceneName;

    public void LoadScene(string sceneName,System.Action afterLoad = null)
    {
        gameObject.SetActive(true);

        audioVolume = AudioListener.volume;
        AudioListener.volume = 0f; // 일단 뮤트
        //SceneManager.sceneLoaded += OnSceneLoaded;
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess(afterLoad));
    }
    private IEnumerator LoadSceneProcess(System.Action afterLoad = null)
    {
        progressBar.fillAmount = 0f;
        //Debug.Log("same name");
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime * 0.3f;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;

                    // 추가: 씬 활성화 후 1프레임 대기
                    // 추가: 씬이 완전히 전환된 후 0.5초 기다린 후 로딩 화면 끄기
                    yield return new WaitForSecondsRealtime(0.5f);

                    Debug.Log("first frame loaded");
                    afterLoad?.Invoke();

                    // 추가: 페이드 아웃이 완료될 때까지 대기
                    yield return StartCoroutine(Fade(false));
                    yield break;
                }
            }
            yield return null;
        }
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        
        if (arg0.name == loadSceneName)
        {
            
            StartCoroutine(Fade(false));
            //SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }
        if (!isFadeIn)
        {
            AudioListener.volume = audioVolume; //오디오 다시 원래대로
            gameObject.SetActive(false);
        }
    }
}
