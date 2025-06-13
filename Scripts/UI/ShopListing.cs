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
    private ItemTooltipUI _itemTooltip;   // ������ ������ ������ ���� UI
    ItemData itemData;

    public void Display(ItemData itemData, ItemTooltipUI itemTooltip, bool isSale = false)
    {
        this.itemData = itemData;
        itemThumbnail.sprite = itemData.IconSprite;
        nameText.StringReference = itemData.NameKey;
        _itemTooltip = itemTooltip;

        if (isSale) //���� ������ �ִٸ�
        {
            discountImage.SetActive(true);
            discountText.gameObject.SetActive(true);
            float result = itemData.Price * 0.8f; //80�� ���� ����
            int perkCost = Mathf.RoundToInt(result); // ���� �ݿø��Ͽ� int�� ��ȯ�մϴ�.
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
