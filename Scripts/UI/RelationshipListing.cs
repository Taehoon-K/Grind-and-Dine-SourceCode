using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PP.InventorySystem;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public class RelationshipListing : MonoBehaviour
{


    [Header("Sprites")]
    public Sprite emptyHearts,fullHearts,emptyUp,fullUp,emptyDown,fullDown,think;

    [Header("UI Elements")]
    [SerializeField]
    private Image portraitImage; //초상화
    [SerializeField]
    private LocalizeStringEvent nameText;
    [SerializeField]
    private Image[] hearts;

    public void Display(NPC npc,NPCRelationshipState relationship)
    {
        if(relationship != null)
        {
            portraitImage.sprite = npc.portraitImage;
            nameText.StringReference = npc.name_key;

            DisplayHearts(relationship.Hearts);
        }
        else //만약 아직 안본 npc면
        {
            portraitImage.sprite = npc.portraitImage;
            portraitImage.color = Color.black;
            nameText.GetComponent<TextMeshProUGUI>().text = "???";

            DisplayHearts(0);
        }
    }

    private void DisplayHearts(float number)
    {
        foreach (Image heart in hearts)
        {
            heart.sprite = null; //모든 스프라이트 지우고 초기화
        }

        if (number < 1 && number > -1) //따봉 한칸 안쪽이거나 비호 한칸 안쪽이면
        {
            foreach (Image heart in hearts)
            {
                heart.sprite = emptyUp; //모든 스프라이트 물음표로
            }
        }

        else if (number >= 9) //만약 하트일시
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (i <= number - 8)
                {
                    hearts[i].sprite = fullHearts;
                }
                else
                {
                    hearts[i].sprite = emptyHearts;
                }
            }
        }

        else if (number <= -1) //만약 비호감일시
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (i <= number *-1)
                {
                    hearts[i].sprite = fullDown;
                }
                else
                {
                    hearts[i].sprite = emptyDown;
                }
            }
        }

        else if (number >= 1) //만약 따봉일시
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (i <= number)
                {
                    hearts[i].sprite = fullUp;
                }
                else
                {
                    hearts[i].sprite = emptyUp;
                }
            }
        }
    }

}
