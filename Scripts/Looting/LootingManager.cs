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
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
            DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }
    public GameTimestamp GetLastLootTime(string objectID)
    {
        if (lootableObjectStatus.TryGetValue(objectID, out GameTimestamp lastLootTime))
        {
            return lastLootTime;
        }
        return null; // 기록이 없으면 null 반환
    }

    // 마지막 루팅 시간 업데이트
    public void UpdateLootTime(string objectID)
    {
        lootableObjectStatus[objectID] = TimeManager.instance.GetGameTimestamp();
    }

    // 오브젝트 상태 초기화 (디버깅이나 리셋용)
    public void ResetLootableObjects()
    {
        lootableObjectStatus.Clear();
    }

    public void GiveReward(ItemData itemName, int amount)
    {
       // Debug.Log($"Received {amount} x {itemName}");
        // 여기에 실제 보상 처리 로직 추가 (예: 인벤토리에 추가)
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
