using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeOff : UiPrompt
{
    [SerializeField]
    private float fadeDuration = 5f, waitFade = 3f;
    private Image fadeImage;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f); // Start fully transparent
    }

    public void StartFadeOut(Action onComplete = null)
    {
        gameObject.SetActive(true);
        
        StartCoroutine(FadeOut(onComplete));
    }

    public void OffFade() //검은화면 끄기
    {
        gameObject.SetActive(false);
        /*if (gameObject.transform.GetChild(0) != null)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }*/
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
    }


    private IEnumerator FadeOut(Action onComplete = null)
    {
        gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(waitFade);

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        TimeManager.instance.AddActiveSystem(); // 타임스케일 0으로 유지

        if (gameObject.transform.childCount > 0)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        // 페이드 아웃이 완료된 후 콜백 호출
        onComplete?.Invoke();
    }


    public void StartFadeIn(Action onComplete = null)
    {
        StartCoroutine(FadeIn(onComplete));
    }
    private IEnumerator FadeIn(Action onComplete = null) //다시 켜지는 코드
    {
        gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        if (gameObject.transform.GetChild(0) != null)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        float elapsed = 0f;
        Color color = fadeImage.color;

        // 초기 상태를 완전 불투명으로 설정
        color.a = 1f;
        fadeImage.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration); // 알파값을 1에서 0으로 점차 감소
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        // 페이드 인이 완료된 후 콜백 호출
        onComplete?.Invoke();

        gameObject.SetActive(false); // 필요하다면 비활성화
        
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        TimeManager.instance.RemoveActiveSystem(); // 비활성화될 때 타임스케일 조정
    }

}
