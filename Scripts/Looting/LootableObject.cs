using PP.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableObject : MonoBehaviour
{
    [SerializeField] private ItemData[] lootName;
   // [SerializeField] private int lootAmount;
    [SerializeField] private string objectID; // 각 오브젝트를 고유하게 구분할 ID

    [SerializeField] private ItemData[] special; // 낮은 확률로 추가 지급할 특별 아이템 목록

    private bool isLootedToday;

    private void Start()
    {
        // 초기 상태 확인: RewardManager에서 마지막 루팅 시간 가져오기
        GameTimestamp lastLootTime = LootingManager.instance.GetLastLootTime(objectID);
        isLootedToday = !CanLoot(lastLootTime);
    }
    public void GiveReward() //그냥 쓰레기 뒤지기
    {
        if (!isLootedToday)
        {
            int luck = StatusManager.instance.GetLuckLevel(); //운 받아오기
            float luckM = luck * 0.01f;

            // 보상 지급
            int itemCount = DetermineLootCount(luck); //줄 아이템 개수 정하기

            /*for (int i = 0; i < itemCount; i++)
            {
                ItemData randomLoot = lootName[Random.Range(0, lootName.Length)];
                LootingManager.instance.GiveReward(randomLoot, lootAmount);
            }*/
            Dictionary<ItemData, int> lootDistribution = new Dictionary<ItemData, int>();

            for (int i = 0; i < itemCount; i++)
            {
                ItemData randomLoot = lootName[Random.Range(0, lootName.Length)];
                if (lootDistribution.ContainsKey(randomLoot))
                {
                    lootDistribution[randomLoot]++;
                }
                else
                {
                    lootDistribution[randomLoot] = 1;
                }
            }

            foreach (var loot in lootDistribution)
            {
                LootingManager.instance.GiveReward(loot.Key, loot.Value);
            }

            // 특별 아이템 지급 로직
            if (special.Length > 0 && Random.value < (0.2f + luckM) && StatusManager.instance.GetSelectedPerk(2, 0) == 0) //퍽 찍었다면 20% 확률로 추가 지급
            {
                //퍽 발동 효과 넣기
                UIManager.instance.NoticeItemCreate(8, 1);

                ItemData specialLoot = special[Random.Range(0, special.Length)];
                LootingManager.instance.GiveReward(specialLoot, 1);
            }


            // RewardManager에 마지막 루팅 시간 업데이트
            LootingManager.instance.UpdateLootTime(objectID);

            isLootedToday = true;
        }
        else
        {
            UIManager.instance.NoticeCreate("TodayAlreadyLoot_key");
        }
    }
    public void GiveRewardCriminal(System.Action onComplete)
    {
        /*
        if (!isLootedToday)
        {
            UIManager.instance.TriggerCardPanel(0, (bool isCaught) =>
            {
                if (isCaught)
                {
                    ExtraManager.instance.GenPolice(policeGenPoint, policeGenRotation);
                }
                else
                {
                    LootingManager.instance.GiveReward(lootName[0], 1);
                }

                LootingManager.instance.UpdateLootTime(objectID);
                isLootedToday = true; //테스트 시 지울것
                StatusManager.instance.SetCrimianl(2);

                onComplete?.Invoke(); // 루팅 종료 신호
            },true);
        }
        else
        {
            UIManager.instance.NoticeCreate("TodayAlreadyLoot_key");
            onComplete?.Invoke(); // 그냥 종료
        }*/
    }
    private bool CanLoot(GameTimestamp lastLootTime)
    {
        if (lastLootTime == null) return true; // 기록이 없으면 루팅 가능

        // 현재 시간과 마지막 루팅 시간 비교
        GameTimestamp currentTime = TimeManager.instance.GetGameTimestamp();
        return currentTime.IsNextDay(lastLootTime);
    }

    private int DetermineLootCount(int lucky) //그날 운에 따라 몇개 줄건지 결정하는 함수
    {
        //int luck = StatusManager.instance.GetLuckLevel();
        float normalizedLuck = Mathf.InverseLerp(-10, 10, lucky); // -10~10을 0~1로 변환
        float[] probabilities = new float[4];
        probabilities[0] = Mathf.Lerp(0.4f, 0.1f, normalizedLuck); // 1개 확률
        probabilities[1] = Mathf.Lerp(0.3f, 0.2f, normalizedLuck); // 2개 확률
        probabilities[2] = Mathf.Lerp(0.2f, 0.4f, normalizedLuck); // 3개 확률
        probabilities[3] = Mathf.Lerp(0.1f, 0.3f, normalizedLuck); // 4개 확률

        float rand = Random.value;
        float cumulative = 0f;
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (rand < cumulative)
                return i + 1;
        }
        return 1;
    }
}
