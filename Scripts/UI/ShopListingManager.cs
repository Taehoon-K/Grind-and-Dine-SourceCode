using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP.InventorySystem;
using static UIManager;
using TMPro;

public class ShopListingManager : UiPrompt
{
    //인스턴스할 샵 리스팅
    public GameObject shopListing;

    public Transform listingGrid;

    private ItemData itemToBuy;

    public Inventory _inventory;

    [SerializeField]
    private TradeUI tradeUI;

    [SerializeField]
    private ItemTooltipUI tooltip; //상점 화면에 표시될 툴팁

    [SerializeField] GameObject noItemText; //파는 물건 없다는 텍스트

    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] private AudioSource moneySource;

    public void RenderShop(List<ItemData> shopItems,string filter)
    {
        RenderMoney(); //돈 업뎃

        //원래 있던거 리셋
        noItemText.SetActive(false);
        if (listingGrid.childCount > 0)
        {
            foreach(Transform child in listingGrid)
            {
                Destroy(child.gameObject);
            }
        }

        if (shopItems.Count > 0) //만약 파는 아이템이 있다면
        {
            if (StatusManager.instance.GetSelectedPerk(4, 1) == 1) //만약 거래 퍽 찍었다면
            {
                foreach (ItemData shopItem in shopItems)
                {
                    GameObject listingGameObject = Instantiate(shopListing, listingGrid);

                    listingGameObject.GetComponent<ShopListing>().Display(shopItem, tooltip, true);

                }
            }
            else
            {
                foreach (ItemData shopItem in shopItems)
                {
                    GameObject listingGameObject = Instantiate(shopListing, listingGrid);

                    listingGameObject.GetComponent<ShopListing>().Display(shopItem, tooltip);
                }

            }
            
        }
        else
        {
            noItemText.SetActive(true);
        }
        

        tradeUI.InitToggleEvents(filter);
    }
    public void ClickItem(ItemData item)
    {
        itemToBuy = item;
        int cost;

        if (StatusManager.instance.GetSelectedPerk(4, 1) == 1) //만약 거래 퍽 찍었다면
        {
            float result = itemToBuy.Price * 0.8f; //80퍼 할인 적용
            cost = Mathf.RoundToInt(result); // 값을 반올림하여 int로 변환합니다.
        }
        else
        {
            cost = itemToBuy.Price;
        }
        int playerMoneyLeft = PlayerStats.Money - cost;

        if (playerMoneyLeft < 0)
        {
            //돈 부족 멘트 띄우기
            return;
        }
        //ConfirmPurchase();

        moneySource.Play();

        PlayerStats.Spend(cost);
        _inventory.Add(itemToBuy,1,false);
    }

    public void RenderMoney() //돈 업데이트
    {
        moneyText.text = PlayerStats.Money.ToString("N0") + PlayerStats.CURRENCY;
    }
}
