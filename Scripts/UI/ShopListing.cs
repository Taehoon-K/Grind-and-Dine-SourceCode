using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using PP.InventorySystem;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public class ShopListing : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public Image itemThumbnail;
    public LocalizeStringEvent nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI discountText;
    public GameObject discountImage;
    private ItemTooltipUI _itemTooltip;   // 아이템 정보를 보여줄 툴팁 UI
    ItemData itemData;

    public void Display(ItemData itemData, ItemTooltipUI itemTooltip, bool isSale = false)
    {
        this.itemData = itemData;
        itemThumbnail.sprite = itemData.IconSprite;
        nameText.StringReference = itemData.NameKey;
        _itemTooltip = itemTooltip;

        if (isSale) //만약 할인퍽 있다면
        {
            discountImage.SetActive(true);
            discountText.gameObject.SetActive(true);
            float result = itemData.Price * 0.8f; //80퍼 할인 적용
            int perkCost = Mathf.RoundToInt(result); // 값을 반올림하여 int로 변환합니다.
            discountText.text = perkCost + PlayerStats.CURRENCY;
        }
        costText.text = itemData.Price + PlayerStats.CURRENCY;     
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _itemTooltip.SetItemInfo(itemData,true);
        _itemTooltip.Show();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _itemTooltip.Hide();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        
        UIManager.instance.shopListingManager.ClickItem(itemData);
    }
}
