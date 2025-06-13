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
        if (instance != null && instance != this) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
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

    // 퀘스트 시작
    public void StartQuest(QuestData questData)
    {
        if (IsQuestAlreadyStarted(questData)) return;

        QuestInstance newQuest = new QuestInstance(questData);
        activeQuests.Add(newQuest);
        UIManager.instance.NoticeItemCreate(10, 0, questData.titleLocalized); //ui 노티스 띄우기
                                                                              // questUI.DisplayQuests(activeQuests, completedQuests);
        UpdateNpcQuestIcons(); //미니맵 업데이트
        SyncAllQuestProgressToBlackboard(); //퀘스트 블랙보드 업데이트
    }

    private bool IsQuestAlreadyStarted(QuestData data)
    {
        foreach (var q in activeQuests)
            if (q.data == data) return true;
        foreach (var q in completedQuests)
            if (q.data == data) return true;
        return false;
    }

    // 목표 완료 처리
    public void CompleteObjective(int questId, int objectiveIndex)
    {
        // questId로 QuestInstance 찾기
        QuestInstance quest = activeQuests.Find(q => q.data.questId == questId);

        if (quest == null)
        {
            Debug.Log($"퀘스트 ID {questId}를 찾을 수 없습니다.");
            return;
        }

        // 인덱스 유효성 확인
        if (objectiveIndex < 0 || objectiveIndex >= quest.objectiveCompletion.Count)
        {
            Debug.Log($"퀘스트 ID {questId}의 목표 인덱스 {objectiveIndex}가 잘못되었습니다.");
            return;
        }

        // 순서대로 해야 하는 경우: 현재 목표만 허용
        if (quest.data.isSequential && !quest.data.hasChoiceAtEnd)
        {
            int nextIndex = quest.objectiveCompletion.FindIndex(done => !done);
            if (objectiveIndex != nextIndex)
            {
                Debug.Log($"퀘스트 {questId}: 순서상 아직 해당 목표를 완료할 수 없습니다.");
                return;
            }
        }

        // 이미 완료된 목표면 무시
        if (quest.objectiveCompletion[objectiveIndex])
            return;

        // 분기 처리: 선택지면 다른 선택은 막기
        if (quest.data.hasChoiceAtEnd)
        {
            int count = quest.data.objectives.Count;
            int branchStart = count - quest.data.choiceCount;

            if (objectiveIndex >= branchStart)
            {
                for (int i = 0; i < quest.data.choiceCount; i++)
                    quest.objectiveCompletion[branchStart + i] = false; // 초기화
            }
        }

        // 목표 완료 처리
        quest.objectiveCompletion[objectiveIndex] = true;

        UpdateNpcQuestIcons(); //미니맵 업데이트

        // 퀘스트 완료 상태 확인 → 완료 처리
        if (quest.IsCompleted())
        {
            MoveToCompleted(quest);

            return;
        }

        // UI 갱신 (선택된 퀘스트라면)
        if (quest == questUI.selectedQuestInstance)
        {
            //FindObjectOfType<QuestManagerUI>()?.ShowQuestDetail(quest);
            questHud.DisplayQuest(quest);
        }
    }


    private void MoveToCompleted(QuestInstance quest) //퀘스트 완료 시
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        questHud.DisplayQuest(null); //퀘스트허드 지우기

        UIManager.instance.NoticeItemCreate(10, 1, quest.data.titleLocalized); //ui 노티스 띄우기
                                                                               //questUI.DisplayQuests(activeQuests, completedQuests);

        //보상 지급 처리
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
                        Debug.Log($"분기 보상 지급: 선택지 {i}");
                    }
                    break;
                }
            }
        }
        else
        {
            // 일반 퀘스트 보상
            if (quest.data.rewards.Count > 0)
            {
                GiveReward(quest.data.rewards[0]);
                Debug.Log("기본 보상 지급");
            }
        }
        SyncAllQuestProgressToBlackboard(); //퀘스트 블랙보드 업데이트
    }
    #region Save&Load
    // 저장
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

    // 불러오기
    public void LoadQuests(QuestSaveWrapper questData)
    {
        /*if (!PlayerPrefs.HasKey("QuestSave")) return;

        string json = PlayerPrefs.GetString("QuestSave");
        QuestSaveWrapper saveData = JsonUtility.FromJson<QuestSaveWrapper>(json);*/

        activeQuests.Clear();
        completedQuests.Clear();

        // 모든 퀘스트 리소스 불러오기
        QuestData[] allQuestData = Resources.LoadAll<QuestData>("Quests");

        // 도우미 함수: id로 QuestData 찾기
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
        SyncAllQuestProgressToBlackboard(); //퀘스트 블랙보드 업데이트
        //questUI.DisplayQuests(activeQuests, completedQuests);
    }
    #endregion

    private void GiveReward(QuestReward reward)
    {
        if (reward == null) return;

        if (reward.money > 0)
        {
            PlayerStats.Earn(reward.money);
            Debug.Log($"보상: 돈 {reward.money}원 지급!");
        }

       /* if (!string.IsNullOrEmpty(reward.itemId))
        {
            InventorySystem.Instance.AddItem(reward.itemId, reward.itemCount);
            Debug.Log($"보상: 아이템 {reward.itemId} x{reward.itemCount} 지급!");
        }*/
       if(reward.skillAmount > 0) //만약 경험치 보상 있다면
        {
            StatusManager.instance.AddExperience(reward.skillIndex, reward.skillAmount);
        }

       if(reward.relationNpc != null && reward.relationAmount != 0)
        {
            RelationshipStats.AddFriendPoints(reward.relationNpc, reward.relationAmount); //호감도 증가
        }
    }


    private void SyncAllQuestProgressToBlackboard() //블랙보드에 퀘스트 진행상황 등록
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        // 1. ID 추출용 셋 생성
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

        // 2. 모든 퀘스트 대상으로 처리
        foreach (var questData in allQuestData)
        {
            if (questData == null) continue;

            string key = $"quest_{questData.questId}";
            bool isAvailable = !activeIds.Contains(questData.questId) && !completedIds.Contains(questData.questId);

            blackboard.SetValue(key, isAvailable);
        }
    }

    public string CheckItemDeliveryCondition(int itemID, NPC npcID) //퀘스트로 아이템 줬을 때 검사하는 코드
    {
        foreach (var quest in activeQuests)
        {
            var data = quest.data;
            var steps = quest.objectiveCompletion;

            for (int i = 0; i < data.objectives.Count; i++)
            {
                if (steps[i]) continue; // 이미 완료된 목표는 스킵
                var obj = data.objectives[i];

                if (!obj.requiresItem) continue;
                if (obj.requiredItemID != itemID) continue;
                if (npcID != null && obj.requiredNpcID != npcID) continue;

                // 조건 일치 → 완료 처리
                //CompleteObjective(data.questId, i);
                Debug.Log($"퀘스트 {data.questId} 목표 {i} 완료: 아이템 전달됨");
                return obj.dialogue;
            }
        }

        return null;
    }
    public string CheckNpcInteractionCondition(NPC npcID) //퀘스트로 아이템 줬을 때 검사하는 코드
    {
        foreach (var quest in activeQuests)
        {
            var data = quest.data;
            var steps = quest.objectiveCompletion;

            if (data.isSequential)
            {
                int currentStep = steps.FindIndex(b => !b);
                if (currentStep == -1) continue; // 모두 완료

                var obj = data.objectives[currentStep];

                if (!obj.requiresNpcInteraction) continue;
                if (obj.targetNpc != npcID) continue;

                // 조건 충족
                Debug.Log($"퀘스트 {data.questId} 목표 {currentStep} 완료 조건 충족 (NPC: {npcID.name})");
                return obj.dialogue;
            }
            else
            {
                // 순차 진행이 아니므로 모든 미완료 목표 중 검사
                for (int i = 0; i < data.objectives.Count; i++)
                {
                    if (steps[i]) continue;

                    var obj = data.objectives[i];
                    if (!obj.requiresNpcInteraction) continue;
                    if (obj.targetNpc != npcID) continue;

                    // 조건 충족
                    Debug.Log($"퀘스트 {data.questId} 목표 {i} 완료 조건 충족 (비순차)");
                    return obj.dialogue;
                }
            }
        }

        return null;
    }


    #region MinimapMarker
    public bool IsNpcCurrentQuestTarget(NPC npc) //NPC가 현재 목표 대상인지 확인 (느낌표 조건)
    {
        foreach (var quest in activeQuests)
        {
            var data = quest.data;
            var steps = quest.objectiveCompletion;

            // 순차 퀘스트
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
                // 비순차 퀘스트
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
    public bool IsNpcQuestGiver(NPC npc) //NPC가 퀘스트 시작 조건 대상인지 확인 (물음표 조건)
    {
        foreach (var questData in allQuestData)
        {
            if (IsQuestAlreadyStarted(questData)) continue;

            // 퀘스트의 첫 목표가 이 NPC와 대화하는 것이라면
            if (questData.npc != null && questData.npc == npc)
            {
                /*var eventDialogue = yarn.CheckEventDialogue(npc.dialogues,true);
                if (eventDialogue != null && eventDialogue.Equals("Quest"+questData.questId))
                {
                    Debug.Log("일치");
                    return true;
                }     
                Debug.Log(eventDialogue);*/

                int currentRelationship = RelationshipStats.GetRelationshipPoint(npc);
                int currentDay = TimeManager.instance.GetGameTimestamp().GetPassedDay();

                bool relationshipOk = questData.requireRelationship == 0 || currentRelationship >= questData.requireRelationship;
                bool dayOk = questData.day == 0 || currentDay >= questData.day;

                Debug.Log(RelationshipStats.GetRelationshipPoint(npc) +npc.CharacterName()+" 포인트");
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
   /* public string CheckEventDialogue(DialogueCondition[] conditions) //이벤트 만족하는 경우 있는지 체크
    {
        string node = null;
        int highestConditionScore = -1;
        foreach (DialogueCondition condition in conditions)
        {
            //가장 우선순위 높은 컨디션 찾기
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
            //가장 우선순위 높은 컨디션 찾기
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


    private void UpdateNpcQuestIcons() //퀘스트 갱신 시 아이콘 업뎃
    {
        if(NpcManager.Instance != null)
        {
            NpcManager.Instance.UpdateMinimapIcon();
        }
    }
    #endregion
}
