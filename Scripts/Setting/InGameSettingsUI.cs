using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettingsUI : MonoBehaviour
{
    [SerializeField] private SliderManager masterSlider;
    [SerializeField] private SliderManager musicSlider;
    [SerializeField] private SliderManager sfxSlider;
    [SerializeField] private SliderManager mouseSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private HorizontalSelector languageSelector;
    [SerializeField] private HorizontalSelector windowSelector;

    private IEnumerator Start()
    {
        // QualityManager가 완전히 초기화될 때까지 대기
        yield return new WaitUntil(() => QualityManager.Instance != null);

        QualityManager.Instance.AssignInGameUI(
            masterSlider, musicSlider, sfxSlider, mouseSlider, resolutionDropdown
        );
    }

    private void OnEnable()
    {
        if (QualityManager.Instance == null) return;

        // 기존 리스너 제거 후 새로 연결
        masterSlider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
        masterSlider.GetComponent<Slider>().onValueChanged.AddListener(QualityManager.Instance.VolumeSetMaster);

        musicSlider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
        musicSlider.GetComponent<Slider>().onValueChanged.AddListener(QualityManager.Instance.VolumeSetMusic);

        sfxSlider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
        sfxSlider.GetComponent<Slider>().onValueChanged.AddListener(QualityManager.Instance.VolumeSetSFX);

        mouseSlider.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
        mouseSlider.GetComponent<Slider>().onValueChanged.AddListener(QualityManager.Instance.SetMouseSensitivity);

        languageSelector.onValueChanged.RemoveAllListeners();
        languageSelector.onValueChanged.AddListener(QualityManager.Instance.SwitchLanguage);

        if(windowSelector != null)
        {
            windowSelector.onValueChanged.RemoveAllListeners();
            windowSelector.onValueChanged.AddListener(index =>
            {
                switch (index)
                {
                    case 0:
                        QualityManager.Instance.WindowWindowed();
                        break;
                    case 1:
                        QualityManager.Instance.WindowFullscreen();
                        break;
                    case 2:
                        QualityManager.Instance.WindowBorderless();
                        break;
                    default:
                        Debug.LogWarning("Unknown window mode index: " + index);
                        break;
                }
            });
        }
    }
}
