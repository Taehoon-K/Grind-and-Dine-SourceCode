using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP.InventorySystem;
using static UIManager;
using TMPro;

public class ShopListingManager : UiPrompt
{
    //�ν��Ͻ��� �� ������
    public GameObject shopListing;

    public Transform listingGrid;

    private ItemData itemToBuy;

    public Inventory _inventory;

    [SerializeField]
    private TradeUI tradeUI;

    [SerializeField]
    private ItemTooltipUI tooltip; //���� ȭ�鿡 ǥ�õ� ����

    [SerializeField] GameObject noItemText; //�Ĵ� ���� ���ٴ� �ؽ�Ʈ

    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] private AudioSource moneySource;

    public void RenderShop(List<ItemData> shopItems,string filter)
    {
        RenderMoney(); //�� ����

        //���� �ִ��� ����
        noItemText.SetActive(false);
        if (listingGrid.childCount > 0)
        {
            foreach(Transform child in listingGrid)
            {
                Destroy(child.gameObject);
            }
        }

        if (shopItems.Count > 0) //���� �Ĵ� �������� �ִٸ�
        {
            if (StatusManager.instance.GetSelectedPerk(4, 1) == 1) //���� �ŷ� �� ����ٸ�
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

        if (StatusManager.instance.GetSelectedPerk(4, 1) == 1) //���� �ŷ� �� ����ٸ�
        {
            float result = itemToBuy.Price * 0.8f; //80�� ���� ����
            cost = Mathf.RoundToInt(result); // ���� �ݿø��Ͽ� int�� ��ȯ�մϴ�.
        }
        else
        {
            cost = itemToBuy.Price;
        }
        int playerMoneyLeft = PlayerStats.Money - cost;

        if (playerMoneyLeft < 0)
        {
            //�� ���� ��Ʈ ����
            return;
        }
        //ConfirmPurchase();

        moneySource.Play();

        PlayerStats.Spend(cost);
        _inventory.Add(itemToBuy,1,false);
    }

    public void RenderMoney() //�� ������Ʈ
    {
        moneyText.text = PlayerStats.Money.ToString("N0") + PlayerStats.CURRENCY;
    }
}
