using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using TMPro;

public class ToHouseListing : MonoBehaviour
{
    //public Image itemThumbnail;
    public LocalizeStringEvent nameText;
    //public TextMeshProUGUI costText;
    SceneTransitionManager.Location name;
    public void Display(string npcName,SceneTransitionManager.Location location)
    {
        name = location;
        //itemThumbnail.sprite = itemData.IconSprite;
        nameText.StringReference.TableEntryReference = npcName + "H_key";
       // costText.text = itemData.Price + PlayerStats.CURRENCY;
    }

    public void MoveScene() //버튼 눌렀을시 이동하는 함수
    {
        SceneTransitionManager.Instance.SwitchLocation(name);
        UIManager.instance.ExitHousePrompt();
    }
}
