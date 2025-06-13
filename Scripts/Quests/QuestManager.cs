using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using static UnityEngine.Mesh;

public class QuestManager : MonoBehaviour
{
    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public List<QuestInstance> completedQuests = new List<QuestInstance>();

    public QuestManagerUI questUI;
    [SerializeField] private QuestHud questHud;
    //[SerializeField] PlayerYarn yarn;

    QuestData[] allQuestData;
    public static QuestManager instance = null;
    private void Awake()
    {
        if (instance != null && instance != this) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

    }
    private void Start()
    {
        allQuestData = Resources.LoadAll<QuestData>("Quests");
    }

    // ����Ʈ ����
    public void StartQuest(QuestData questData)
    {
        if (IsQuestAlreadyStarted(questData)) return;

        QuestInstance newQuest = new QuestInstance(questData);
        activeQuests.Add(newQuest);
        UIManager.instance.NoticeItemCreate(10, 0, questData.titleLocalized); //ui ��Ƽ�� ����
                                                                              // questUI.DisplayQuests(activeQuests, completedQuests);
        UpdateNpcQuestIcons(); //�̴ϸ� ������Ʈ
        SyncAllQuestProgressToBlackboard(); //����Ʈ ������ ������Ʈ
    }

    private bool IsQuestAlreadyStarted(QuestData data)
    {
        foreach (var q in activeQuests)
            if (q.data == data) return true;
        foreach (var q in completedQuests)
            if (q.data == data) return true;
        return false;
    }

    // ��ǥ �Ϸ� ó��
    public void CompleteObjective(int questId, int objectiveIndex)
    {
        // questId�� QuestInstance ã��
        QuestInstance quest = activeQuests.Find(q => q.data.questId == questId);

        if (quest == null)
        {
            Debug.Log($"����Ʈ ID {questId}�� ã�� �� �����ϴ�.");
            return;
        }

        // �ε��� ��ȿ�� Ȯ��
        if (objectiveIndex < 0 || objectiveIndex >= quest.objectiveCompletion.Count)
        {
            Debug.Log($"����Ʈ ID {questId}�� ��ǥ �ε��� {objectiveIndex}�� �߸��Ǿ����ϴ�.");
            return;
        }

        // ������� �ؾ� �ϴ� ���: ���� ��ǥ�� ���
        if (quest.data.isSequential && !quest.data.hasChoiceAtEnd)
        {
            int nextIndex = quest.objectiveCompletion.FindIndex(done => !done);
            if (objectiveIndex != nextIndex)
            {
                Debug.Log($"����Ʈ {questId}: ������ ���� �ش� ��ǥ�� �Ϸ��� �� �����ϴ�.");
                return;
            }
        }

        // �̹� �Ϸ�� ��ǥ�� ����
        if (quest.objectiveCompletion[objectiveIndex])
            return;

        // �б� ó��: �������� �ٸ� ������ ����
        if (quest.data.hasChoiceAtEnd)
        {
            int count = quest.data.objectives.Count;
            int branchStart = count - quest.data.choiceCount;

            if (objectiveIndex >= branchStart)
            {
                for (int i = 0; i < quest.data.choiceCount; i++)
                    quest.objectiveCompletion[branchStart + i] = false; // �ʱ�ȭ
            }
        }

        // ��ǥ �Ϸ� ó��
        quest.objectiveCompletion[objectiveIndex] = true;

        UpdateNpcQuestIcons(); //�̴ϸ� ������Ʈ

        // ����Ʈ �Ϸ� ���� Ȯ�� �� �Ϸ� ó��
        if (quest.IsCompleted())
        {
            MoveToCompleted(quest);

            return;
        }

        // UI ���� (���õ� ����Ʈ���)
        if (quest == questUI.selectedQuestInstance)
        {
            //FindObjectOfType<QuestManagerUI>()?.ShowQuestDetail(quest);
            questHud.DisplayQuest(quest);
        }
    }


    private void MoveToCompleted(QuestInstance quest) //����Ʈ �Ϸ� ��
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        questHud.DisplayQuest(null); //����Ʈ��� �����

        UIManager.instance.NoticeItemCreate(10, 1, quest.data.titleLocalized); //ui ��Ƽ�� ����
                                                                               //questUI.DisplayQuests(activeQuests, completedQuests);

        //���� ���� ó��
        if (quest.data.hasChoiceAtEnd && quest.data.choiceCount > 0)
        {
            int total = quest.objectiveCompletion.Count;
            int branchStart = total - quest.data.choiceCount;

            for (int i = 0; i < quest.data.choiceCount; i++)
            {
                int globalIndex = branchStart + i;

                if (quest.objectiveCompletion[globalIndex])
                {
                    if (i < quest.data.rewards.Count)
                    {
                        GiveReward(quest.data.rewards[i]);
                        Debug.Log($"�б� ���� ����: ������ {i}");
                    }
                    break;
                }
            }
        }
        else
        {
            // �Ϲ� ����Ʈ ����
            if (quest.data.rewards.Count > 0)
            {
                GiveReward(quest.data.rewards[0]);
                Debug.Log("�⺻ ���� ����");
            }
        }
        SyncAllQuestProgressToBlackboard(); //����Ʈ ������ ������Ʈ
    }
    #region Save&Load
    // ����
    public QuestSaveWrapper SaveQuests()
    {
        QuestSaveWrapper saveData = new QuestSaveWrapper();

        foreach (var quest in activeQuests)
        {
            saveData.activeQuests.Add(new QuestSaveData
            {
                questID = quest.data.questId,
                objectiveStates = new List<bool>(quest.objectiveCompletion)
            });
        }

        foreach (var quest in completedQuests)
        {
            saveData.completedQuests.Add(new QuestSaveData
            {
                questID = quest.data.questId,
                objectiveStates = new List<bool>(quest.objectiveCompletion)
            });
        }

        return saveData;
    }

    // �ҷ�����
    public void LoadQuests(QuestSaveWrapper questData)
    {
        /*if (!PlayerPrefs.HasKey("QuestSave")) return;

        string json = PlayerPrefs.GetString("QuestSave");
        QuestSaveWrapper saveData = JsonUtility.FromJson<QuestSaveWrapper>(json);*/

        activeQuests.Clear();
        completedQuests.Clear();

        // ��� ����Ʈ ���ҽ� �ҷ�����
        QuestData[] allQuestData = Resources.LoadAll<QuestData>("Quests");

        // ����� �Լ�: id�� QuestData ã��
        QuestData GetQuestById(int id)
        {
            foreach (var qd in allQuestData)
            {
                if (qd.questId == id)
                    return qd;
            }
            return null;
        }

        foreach (var saved in questData.activeQuests)
        {
            QuestData data = GetQuestById(saved.questID);
            if (data != null)
            {
                QuestInstance qi = new QuestInstance(data);
                qi.objectiveCompletion = new List<bool>(saved.objectiveStates);
                activeQuests.Add(qi);
            }
        }

        foreach (var saved in questData.completedQuests)
        {
            QuestData data = GetQuestById(saved.questID);
            if (data != null)
            {
                QuestInstance qi = new QuestInstance(data);
                qi.objectiveCompletion = new List<bool>(saved.objectiveStates);
                completedQuests.Add(qi);
            }
        }
        SyncAllQuestProgressToBlackboard(); //����Ʈ ������ ������Ʈ
        //questUI.DisplayQuests(activeQuests, completedQuests);
    }
    #endregion

    private void GiveReward(QuestReward reward)
    {
        if (reward == null) return;

        if (reward.money > 0)
        {
            PlayerStats.Earn(reward.money);
            Debug.Log($"����: �� {reward.money}�� ����!");
        }

       /* if (!string.IsNullOrEmpty(reward.itemId))
        {
            InventorySystem.Instance.AddItem(reward.itemId, reward.itemCount);
            Debug.Log($"����: ������ {reward.itemId} x{reward.itemCount} ����!");
        }*/
       if(reward.skillAmount > 0) //���� ����ġ ���� �ִٸ�
        {
            StatusManager.instance.AddExperience(reward.skillIndex, reward.skillAmount);
        }

       if(reward.relationNpc != null && reward.relationAmount != 0)
        {
            RelationshipStats.AddFriendPoints(reward.relationNpc, reward.relationAmount); //ȣ���� ����
        }
    }


    private void SyncAllQuestProgressToBlackboard() //�����忡 ����Ʈ �����Ȳ ���
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        // 1. ID ����� �� ����
        HashSet<int> activeIds = new();
        HashSet<int> completedIds = new();

        foreach (var quest in activeQuests)
        {
            if (quest?.data != null)
                activeIds.Add(quest.data.questId);
        }

        foreach (var quest in completedQuests)
        {
            if (quest?.data != null)
                completedIds.Add(quest.data.questId);
        }

        // 2. ��� ����Ʈ ������� ó��
        foreach (var questData in allQuestData)
        {
            if (questData == null) continue;

            string key = $"quest_{questData.questId}";
            bool isAvailable = !activeIds.Contains(questData.questId) && !completedIds.Contains(questData.questId);

            blackboard.SetValue(key, isAvailable);
        }
    }

    public string CheckItemDeliveryCondition(int itemID, NPC npcID) //����Ʈ�� ������ ���� �� �˻��ϴ� �ڵ�
    {
        foreach (var quest in activeQuests)
        {
            var data = quest.data;
            var steps = quest.objectiveCompletion;

            for (int i = 0; i < data.objectives.Count; i++)
            {
                if (steps[i]) continue; // �̹� �Ϸ�� ��ǥ�� ��ŵ
                var obj = data.objectives[i];

                if (!obj.requiresItem) continue;
                if (obj.requiredItemID != itemID) continue;
                if (npcID != null && obj.requiredNpcID != npcID) continue;

                // ���� ��ġ �� �Ϸ� ó��
                //CompleteObjective(data.questId, i);
                Debug.Log($"����Ʈ {data.questId} ��ǥ {i} �Ϸ�: ������ ���޵�");
                return obj.dialogue;
            }
        }

        return null;
    }
    public string CheckNpcInteractionCondition(NPC npcID) //����Ʈ�� ������ ���� �� �˻��ϴ� �ڵ�
    {
        foreach (var quest in activeQuests)
        {
            var data = quest.data;
            var steps = quest.objectiveCompletion;

            if (data.isSequential)
            {
                int currentStep = steps.FindIndex(b => !b);
                if (currentStep == -1) continue; // ��� �Ϸ�

                var obj = data.objectives[currentStep];

                if (!obj.requiresNpcInteraction) continue;
                if (obj.targetNpc != npcID) continue;

                // ���� ����
                Debug.Log($"����Ʈ {data.questId} ��ǥ {currentStep} �Ϸ� ���� ���� (NPC: {npcID.name})");
                return obj.dialogue;
            }
            else
            {
                // ���� ������ �ƴϹǷ� ��� �̿Ϸ� ��ǥ �� �˻�
                for (int i = 0; i < data.objectives.Count; i++)
                {
                    if (steps[i]) continue;

                    var obj = data.objectives[i];
                    if (!obj.requiresNpcInteraction) continue;
                    if (obj.targetNpc != npcID) continue;

                    // ���� ����
                    Debug.Log($"����Ʈ {data.questId} ��ǥ {i} �Ϸ� ���� ���� (�����)");
                    return obj.dialogue;
                }
            }
        }

        return null;
    }


    #region MinimapMarker
    public bool IsNpcCurrentQuestTarget(NPC npc) //NPC�� ���� ��ǥ ������� Ȯ�� (����ǥ ����)
    {
        foreach (var quest in activeQuests)
        {
            var data = quest.data;
            var steps = quest.objectiveCompletion;

            // ���� ����Ʈ
            if (data.isSequential)
            {
                int currentStep = steps.FindIndex(b => !b);
                if (currentStep == -1) continue;

                var obj = data.objectives[currentStep];
                if (obj.requiresNpcInteraction && obj.targetNpc == npc) return true;
                if (obj.requiresItem && obj.requiredNpcID == npc) return true;
            }
            else
            {
                // ����� ����Ʈ
                for (int i = 0; i < steps.Count; i++)
                {
                    if (steps[i]) continue;

                    var obj = data.objectives[i];
                    if (obj.requiresNpcInteraction && obj.targetNpc == npc) return true;
                    if (obj.requiresItem && obj.requiredNpcID == npc) return true;
                }
            }
        }
        return false;
    }
    public bool IsNpcQuestGiver(NPC npc) //NPC�� ����Ʈ ���� ���� ������� Ȯ�� (����ǥ ����)
    {
        foreach (var questData in allQuestData)
        {
            if (IsQuestAlreadyStarted(questData)) continue;

            // ����Ʈ�� ù ��ǥ�� �� NPC�� ��ȭ�ϴ� ���̶��
            if (questData.npc != null && questData.npc == npc)
            {
                /*var eventDialogue = yarn.CheckEventDialogue(npc.dialogues,true);
                if (eventDialogue != null && eventDialogue.Equals("Quest"+questData.questId))
                {
                    Debug.Log("��ġ");
                    return true;
                }     
                Debug.Log(eventDialogue);*/

                int currentRelationship = RelationshipStats.GetRelationshipPoint(npc);
                int currentDay = TimeManager.instance.GetGameTimestamp().GetPassedDay();

                bool relationshipOk = questData.requireRelationship == 0 || currentRelationship >= questData.requireRelationship;
                bool dayOk = questData.day == 0 || currentDay >= questData.day;

                Debug.Log(RelationshipStats.GetRelationshipPoint(npc) +npc.CharacterName()+" ����Ʈ");
                if (relationshipOk && dayOk)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
   /* public string CheckEventDialogue(DialogueCondition[] conditions) //�̺�Ʈ �����ϴ� ��� �ִ��� üũ
    {
        string node = null;
        int highestConditionScore = -1;
        foreach (DialogueCondition condition in conditions)
        {
            //���� �켱���� ���� ����� ã��
            if (condition.CheckConditions(out int score))
            {
                if (score > highestConditionScore)
                {
                    highestConditionScore = score;
                    node = condition.id;
                    Debug.Log("Will Play: " + condition.id);
                }
            }
        }

        foreach (DialogueCondition condition in conditions)
        {
            //���� �켱���� ���� ����� ã��
            if (condition.CheckConditions(out int score))
            {
                if (score > highestConditionScore)
                {
                    highestConditionScore = score;
                    node = condition.id;
                    Debug.Log("Will Play: " + condition.id);
                }
            }
        }

        return node;
    }*/


    private void UpdateNpcQuestIcons() //����Ʈ ���� �� ������ ����
    {
        if(NpcManager.Instance != null)
        {
            NpcManager.Instance.UpdateMinimapIcon();
        }
    }
    #endregion
}
