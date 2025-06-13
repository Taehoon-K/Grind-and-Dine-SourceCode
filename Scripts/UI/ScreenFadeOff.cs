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

    public void OffFade() //����ȭ�� ����
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

        TimeManager.instance.AddActiveSystem(); // Ÿ�ӽ����� 0���� ����

        if (gameObject.transform.childCount > 0)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        // ���̵� �ƿ��� �Ϸ�� �� �ݹ� ȣ��
        onComplete?.Invoke();
    }


    public void StartFadeIn(Action onComplete = null)
    {
        StartCoroutine(FadeIn(onComplete));
    }
    private IEnumerator FadeIn(Action onComplete = null) //�ٽ� ������ �ڵ�
    {
        gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        if (gameObject.transform.GetChild(0) != null)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        float elapsed = 0f;
        Color color = fadeImage.color;

        // �ʱ� ���¸� ���� ���������� ����
        color.a = 1f;
        fadeImage.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration); // ���İ��� 1���� 0���� ���� ����
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        // ���̵� ���� �Ϸ�� �� �ݹ� ȣ��
        onComplete?.Invoke();

        gameObject.SetActive(false); // �ʿ��ϴٸ� ��Ȱ��ȭ
        
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        TimeManager.instance.RemoveActiveSystem(); // ��Ȱ��ȭ�� �� Ÿ�ӽ����� ����
    }

}
