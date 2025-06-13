using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MoodleNotice : MonoBehaviour
{
    [SerializeField]
    GameObject gained, removed;
    [SerializeField]
    private Sprite[] moodleSprites;
    [SerializeField]
    private LocalizedString[] moodleTexts;

    [SerializeField]
    private Image moodleImage;
    [SerializeField]
    private LocalizeStringEvent moodleText;

    [SerializeField]
    private Image crossImage;
    [SerializeField]
    private Color gainColor,removeColor;

    public void Render(int index,bool isGained)
    {
        if (isGained) //»πµÊ Ω√
        {
            gained.SetActive(true);
            removed.SetActive(false);
            crossImage.color = gainColor;
        }
        else //ªÛ≈¬ «ÿ¡¶Ω√
        {
            gained.SetActive(false);
            removed.SetActive(true);
            crossImage.color = removeColor;
        }
        moodleImage.sprite = moodleSprites[index];
        moodleText.StringReference = moodleTexts[index];
    }

}
