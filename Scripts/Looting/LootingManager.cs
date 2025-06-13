using PP.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootingManager : MonoBehaviour
{
    public static LootingManager instance = null;

    private Dictionary<string, GameTimestamp> lootableObjectStatus = new Dictionary<string, GameTimestamp>();
    private void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
            DontDestroyOnLoad(gameObject); //OnLoad(���� �ε� �Ǿ�����) �ڽ��� �ı����� �ʰ� ����
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }
    public GameTimestamp GetLastLootTime(string objectID)
    {
        if (lootableObjectStatus.TryGetValue(objectID, out GameTimestamp lastLootTime))
        {
            return lastLootTime;
        }
        return null; // ����� ������ null ��ȯ
    }

    // ������ ���� �ð� ������Ʈ
    public void UpdateLootTime(string objectID)
    {
        lootableObjectStatus[objectID] = TimeManager.instance.GetGameTimestamp();
    }

    // ������Ʈ ���� �ʱ�ȭ (������̳� ���¿�)
    public void ResetLootableObjects()
    {
        lootableObjectStatus.Clear();
    }

    public void GiveReward(ItemData itemName, int amount)
    {
       // Debug.Log($"Received {amount} x {itemName}");
        // ���⿡ ���� ���� ó�� ���� �߰� (��: �κ��丮�� �߰�)
        if (FindObjectOfType<Inventory>().Add(itemName, amount) == 0)
        {
           // FindObjectOfType
        }
        else
        {
            
        }
    }

    #region Save&Load
    public Dictionary<string, GameTimestamp> GetLootingObjects()
    {
        return lootableObjectStatus;
    }
    public void LoadLootingObjects(Dictionary<string, GameTimestamp> lootableObjectStatus)
    {
        if (lootableObjectStatus == null)
        {
            this.lootableObjectStatus = new Dictionary<string, GameTimestamp>();
            return;
        }
        this.lootableObjectStatus = lootableObjectStatus;
    }
    #endregion
}
