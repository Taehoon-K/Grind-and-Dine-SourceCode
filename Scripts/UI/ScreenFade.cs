using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField]
    private float fadeDuration = 1f; // Duration of fade effect
    private Image fadeImage;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 1f); // Start with full black
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration); // Reduce alpha from 1 to 0
            fadeImage.color = color;
            yield return null;
        }

        // Make sure it's fully transparent at the end
        color.a = 0f;
        fadeImage.color = color;
        gameObject.SetActive(false); // Optional: disable the image object after fade
    }
}
