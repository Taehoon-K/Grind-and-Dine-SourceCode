using PP.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableObject : MonoBehaviour
{
    [SerializeField] private ItemData[] lootName;
   // [SerializeField] private int lootAmount;
    [SerializeField] private string objectID; // �� ������Ʈ�� �����ϰ� ������ ID

    [SerializeField] private ItemData[] special; // ���� Ȯ���� �߰� ������ Ư�� ������ ���

    private bool isLootedToday;

    private void Start()
    {
        // �ʱ� ���� Ȯ��: RewardManager���� ������ ���� �ð� ��������
        GameTimestamp lastLootTime = LootingManager.instance.GetLastLootTime(objectID);
        isLootedToday = !CanLoot(lastLootTime);
    }
    public void GiveReward() //�׳� ������ ������
    {
        if (!isLootedToday)
        {
            int luck = StatusManager.instance.GetLuckLevel(); //�� �޾ƿ���
            float luckM = luck * 0.01f;

            // ���� ����
            int itemCount = DetermineLootCount(luck); //�� ������ ���� ���ϱ�

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

            // Ư�� ������ ���� ����
            if (special.Length > 0 && Random.value < (0.2f + luckM) && StatusManager.instance.GetSelectedPerk(2, 0) == 0) //�� ����ٸ� 20% Ȯ���� �߰� ����
            {
                //�� �ߵ� ȿ�� �ֱ�
                UIManager.instance.NoticeItemCreate(8, 1);

                ItemData specialLoot = special[Random.Range(0, special.Length)];
                LootingManager.instance.GiveReward(specialLoot, 1);
            }


            // RewardManager�� ������ ���� �ð� ������Ʈ
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
                isLootedToday = true; //�׽�Ʈ �� �����
                StatusManager.instance.SetCrimianl(2);

                onComplete?.Invoke(); // ���� ���� ��ȣ
            },true);
        }
        else
        {
            UIManager.instance.NoticeCreate("TodayAlreadyLoot_key");
            onComplete?.Invoke(); // �׳� ����
        }*/
    }
    private bool CanLoot(GameTimestamp lastLootTime)
    {
        if (lastLootTime == null) return true; // ����� ������ ���� ����

        // ���� �ð��� ������ ���� �ð� ��
        GameTimestamp currentTime = TimeManager.instance.GetGameTimestamp();
        return currentTime.IsNextDay(lastLootTime);
    }

    private int DetermineLootCount(int lucky) //�׳� � ���� � �ٰ��� �����ϴ� �Լ�
    {
        //int luck = StatusManager.instance.GetLuckLevel();
        float normalizedLuck = Mathf.InverseLerp(-10, 10, lucky); // -10~10�� 0~1�� ��ȯ
        float[] probabilities = new float[4];
        probabilities[0] = Mathf.Lerp(0.4f, 0.1f, normalizedLuck); // 1�� Ȯ��
        probabilities[1] = Mathf.Lerp(0.3f, 0.2f, normalizedLuck); // 2�� Ȯ��
        probabilities[2] = Mathf.Lerp(0.2f, 0.4f, normalizedLuck); // 3�� Ȯ��
        probabilities[3] = Mathf.Lerp(0.1f, 0.3f, normalizedLuck); // 4�� Ȯ��

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
