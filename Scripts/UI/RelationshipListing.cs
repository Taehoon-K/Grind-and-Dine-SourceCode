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
    private Image portraitImage; //�ʻ�ȭ
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
        else //���� ���� �Ⱥ� npc��
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
            heart.sprite = null; //��� ��������Ʈ ����� �ʱ�ȭ
        }

        if (number < 1 && number > -1) //���� ��ĭ �����̰ų� ��ȣ ��ĭ �����̸�
        {
            foreach (Image heart in hearts)
            {
                heart.sprite = emptyUp; //��� ��������Ʈ ����ǥ��
            }
        }

        else if (number >= 9) //���� ��Ʈ�Ͻ�
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

        else if (number <= -1) //���� ��ȣ���Ͻ�
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

        else if (number >= 1) //���� �����Ͻ�
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
