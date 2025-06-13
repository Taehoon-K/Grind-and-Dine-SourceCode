using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

/*[System.Serializable]
public class Quest
{
    public LocalizedString titleLocalized;
    public LocalizedString descriptionLocalized;
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public bool IsCompleted => objectives.TrueForAll(o => o.isCompleted);
}

[System.Serializable]
public class QuestObjective
{
    public LocalizedString descriptionLocalized;
    public bool isCompleted;
}*/
// ScriptableObject로 관리되는 불변 퀘스트 데이터
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int questId;
    public bool isSequential = true;
    public LocalizedString titleLocalized;
    public LocalizedString descriptionLocalized;
    public List<QuestObjectiveData> objectives = new List<QuestObjectiveData>();

    [Tooltip("선택지 보상 (분기 퀘스트인 경우만 사용)")]
    public List<QuestReward> rewards = new();

    [Tooltip("이 퀘스트의 마지막 목표들이 분기 선택형인지 여부")]
    public bool hasChoiceAtEnd = false;

    [Tooltip("분기 선택형일 경우, 마지막 N개의 목표 중 하나만 완료해야 함")]
    public int choiceCount = 0;

    [Tooltip("퀘스트 시작이 이 npc통해야 될 시 등록")]
    public NPC npc;
    public int requireRelationship;
    public int day; //최소 날짜
}

[System.Serializable]
public class QuestObjectiveData
{
    public LocalizedString descriptionLocalized;

    [Tooltip("맵 상 x/y 좌표 (0~1 비율 값)")]
    public Vector2[] mapPositions; // 배열로 변경

    [Header("퀘스트 아이템 요구 조건")]
    public bool requiresItem = false;
    public int requiredItemID;
    public NPC requiredNpcID;

    public string dialogue; //선물 줬을 시 실행될 다이올로그

    [Header("NPC와의 대화인지 조건")]
    public bool requiresNpcInteraction = false; // 이 목표가 NPC와 대화인지?
    public NPC targetNpc; // 대화해야 할 NPC 
}

//퀘스트 세이브 부분
[System.Serializable]
public class QuestSaveData
{
    public int questID; // ScriptableObject 파일 이름
    public List<bool> objectiveStates; // 각 목표 완료 여부
}
[System.Serializable]
public class QuestSaveWrapper
{
    public List<QuestSaveData> activeQuests = new List<QuestSaveData>();
    public List<QuestSaveData> completedQuests = new List<QuestSaveData>();
}


public class QuestUIDataHolder : MonoBehaviour
{
    public int questId;
}

[System.Serializable]
public class QuestReward //퀘스트 보상
{
    public int money; // 돈
    public int skillIndex = 0; // 스킬 인덱스
    public int skillAmount = 0;

    public NPC relationNpc; //호감도 오를 npc
    public int relationAmount; //호감도 양
}