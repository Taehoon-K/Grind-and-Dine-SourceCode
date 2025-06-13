using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.UI;

public class PerkNotice : MonoBehaviour
{
    [SerializeField]
    private Sprite[] perkSprites;
    [SerializeField]
    private LocalizedString[] perkTexts;

    [SerializeField]
    private Image perkImage;
    [SerializeField]
    private LocalizeStringEvent perkText;


    public void Render(int index)
    {
        perkImage.sprite = perkSprites[index];
        perkText.StringReference = perkTexts[index];
    }
}
