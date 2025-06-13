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
// ScriptableObject�� �����Ǵ� �Һ� ����Ʈ ������
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int questId;
    public bool isSequential = true;
    public LocalizedString titleLocalized;
    public LocalizedString descriptionLocalized;
    public List<QuestObjectiveData> objectives = new List<QuestObjectiveData>();

    [Tooltip("������ ���� (�б� ����Ʈ�� ��츸 ���)")]
    public List<QuestReward> rewards = new();

    [Tooltip("�� ����Ʈ�� ������ ��ǥ���� �б� ���������� ����")]
    public bool hasChoiceAtEnd = false;

    [Tooltip("�б� �������� ���, ������ N���� ��ǥ �� �ϳ��� �Ϸ��ؾ� ��")]
    public int choiceCount = 0;

    [Tooltip("����Ʈ ������ �� npc���ؾ� �� �� ���")]
    public NPC npc;
    public int requireRelationship;
    public int day; //�ּ� ��¥
}

[System.Serializable]
public class QuestObjectiveData
{
    public LocalizedString descriptionLocalized;

    [Tooltip("�� �� x/y ��ǥ (0~1 ���� ��)")]
    public Vector2[] mapPositions; // �迭�� ����

    [Header("����Ʈ ������ �䱸 ����")]
    public bool requiresItem = false;
    public int requiredItemID;
    public NPC requiredNpcID;

    public string dialogue; //���� ���� �� ����� ���̿÷α�

    [Header("NPC���� ��ȭ���� ����")]
    public bool requiresNpcInteraction = false; // �� ��ǥ�� NPC�� ��ȭ����?
    public NPC targetNpc; // ��ȭ�ؾ� �� NPC 
}

//����Ʈ ���̺� �κ�
[System.Serializable]
public class QuestSaveData
{
    public int questID; // ScriptableObject ���� �̸�
    public List<bool> objectiveStates; // �� ��ǥ �Ϸ� ����
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
public class QuestReward //����Ʈ ����
{
    public int money; // ��
    public int skillIndex = 0; // ��ų �ε���
    public int skillAmount = 0;

    public NPC relationNpc; //ȣ���� ���� npc
    public int relationAmount; //ȣ���� ��
}