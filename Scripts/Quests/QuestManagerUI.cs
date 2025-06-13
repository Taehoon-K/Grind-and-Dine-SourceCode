// QuestManagerUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class QuestManagerUI : MonoBehaviour
{
    [Header("Quest List")]
    public Transform activeQuestListParent;
    public Transform completedQuestListParent;
    public GameObject questListItemPrefab;

    [Header("Quest Detail Panel")]
    public LocalizeStringEvent questTitleLocalized;
    public LocalizeStringEvent questDescriptionLocalized;
    public Transform objectiveListParent;
    public GameObject objectiveItemPrefab;

    private QuestData selectedQuest;
    private GameObject selectedListItem = null; //���õ� ����Ʈ ����Ʈ
    public QuestInstance selectedQuestInstance = null;

    [SerializeField] private QuestHud questHud;

    [Header("Text")]
    [SerializeField] GameObject noActiceQuest;
    [SerializeField] GameObject noCompleteQuest;

    private void OnEnable()
    {
        DisplayQuests();
    }

    public void DisplayQuests()//List<QuestInstance> activeQuests, List<QuestInstance> completedQuests)
    {
        noActiceQuest.SetActive(false);
        noCompleteQuest.SetActive(false);

        List<QuestInstance> activeQuests = QuestManager.instance?.activeQuests;
        List<QuestInstance> completedQuests = QuestManager.instance?.completedQuests;

        ClearChildren(activeQuestListParent);
        ClearChildren(completedQuestListParent);

        foreach (QuestInstance q in activeQuests)
        {
            GameObject go = Instantiate(questListItemPrefab, activeQuestListParent);
            SetupQuestListItem(go, q, true);
        }

        foreach (QuestInstance q in completedQuests)
        {
            GameObject go = Instantiate(questListItemPrefab, completedQuestListParent);
            SetupQuestListItem(go, q, false);
        }

        if(activeQuests == null || activeQuests.Count == 0)
        {
            noActiceQuest.SetActive(true);
        }
        if (completedQuests == null || completedQuests.Count == 0)
        {
            noCompleteQuest.SetActive(true);
        }

        // ���� ���� ����
        if (selectedQuestInstance != null && selectedQuestInstance.data != null)
        {
            int selectedId = selectedQuestInstance.data.questId;

            foreach (Transform child in activeQuestListParent)
            {
                var holder = child.GetComponent<QuestUIDataHolder>();
                if (holder != null && holder.questId == selectedId)
                {
                    StartCoroutine(RestoreSelectionNextFrame(child.gameObject, selectedQuestInstance, selectedId));
                    break;
                }
            }
        }

    }
    private IEnumerator RestoreSelectionNextFrame(GameObject item, QuestInstance quest, int id)
    {
        yield return null; // ���� �����ӱ��� ���
        HighlightSelectedQuest(item, quest, true, id);
        ShowQuestDetail(quest);
    }

    private void SetupQuestListItem(GameObject go, QuestInstance q, bool isActive)
    {
        go.AddComponent<QuestUIDataHolder>().questId = q.data.questId;

        // ���� �ؽ�Ʈ ���ö�����
        var localize = go.GetComponentInChildren<LocalizeStringEvent>();
        if (localize != null)
            localize.StringReference = q.data.titleLocalized;

        if (isActive) //Ȱ������ ����Ʈ�� Ŭ�������ϰ�
        {
            // Ŭ�� �� ���� ��� + HUD ǥ��
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                HighlightSelectedQuest(go, q);
                ShowQuestDetail(q); // Ŭ�� �� �󼼴� �׻� ǥ��
            });
        }
        

        EventTrigger trigger = go.AddComponent<EventTrigger>();

        // ���콺 �÷��� ��
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((eventData) =>
        {
            ShowQuestDetail(q);

            var bg = go.GetComponent<Image>();
            if (bg != null) bg.color = new Color(0.8f, 0.8f, 0.8f, 1f); // ���

            var newText = go.GetComponentInChildren<TextMeshProUGUI>();
            if (newText != null) newText.color = Color.black;
        });
        trigger.triggers.Add(entryEnter);

        // ���콺 ������ ��
        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((eventData) =>
        {
            var bg = go.GetComponent<Image>();
            if (bg != null) bg.color = new Color(0.8f, 0.8f, 0.8f, 0f); // ���

            var newText = go.GetComponentInChildren<TextMeshProUGUI>();
            if (newText != null) newText.color = Color.white;
        });
        trigger.triggers.Add(entryExit);
    }

    public void ShowQuestDetail(QuestInstance quest)
    {
        selectedQuest = quest.data;

        questTitleLocalized.StringReference = quest.data.titleLocalized;
        questDescriptionLocalized.StringReference = quest.data.descriptionLocalized;

        ClearChildren(objectiveListParent);

        int nextIndex = quest.objectiveCompletion.FindIndex(b => !b);
        int total = quest.data.objectives.Count;
        int choiceStart = total - quest.data.choiceCount; //����Ʈ�����Ϳ��� ���̽�ī��Ʈ ã��

        bool canShowBranches = true;

        // ������ ǥ�� ����: ���� ���� ��ǥ�� ��� �Ϸ�ž� ��
        if (quest.data.isSequential && quest.data.hasChoiceAtEnd)
        {
            for (int i = 0; i < choiceStart; i++)
            {
                if (!quest.objectiveCompletion[i])
                {
                    canShowBranches = false;
                    break;
                }
            }
        }

        for (int i = 0; i < quest.data.objectives.Count; i++)
        {
            var obj = quest.data.objectives[i];
            bool isCompleted = quest.objectiveCompletion[i];
            bool isSelectableBranch = quest.data.hasChoiceAtEnd && i >= choiceStart;

            // �������ε� ���� ������ �� �Ǹ� ����
            if (isSelectableBranch && !canShowBranches)
                continue;

            // �����ε� ���� �������� ���� ��ǥ�� ����
            if (quest.data.isSequential && !isCompleted && !isSelectableBranch && i > nextIndex)
                continue;

            GameObject go = Instantiate(objectiveItemPrefab, objectiveListParent);

            var objTextLocalize = go.GetComponentInChildren<LocalizeStringEvent>();
            if (objTextLocalize != null)
            {
                objTextLocalize.StringReference = obj.descriptionLocalized;
            }

            // ���� üũ �̹��� ó��
            Image checkImage = go.transform.GetChild(0).GetComponent<Image>();
            if (checkImage != null)
            {
                checkImage.enabled = isCompleted;
            }

            // �Ϸ� �� ���� ��Ӱ� ó��
            Image background = go.GetComponent<Image>();
            if (background != null && isCompleted)
            {
                Color dim = background.color;
                dim.a = 0.5f; // ������ �Ǵ� ��ο� ����
                background.color = dim;
            }
        }

    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void HighlightSelectedQuest(GameObject clickedItem, QuestInstance clickedQuest, bool isRestore = false, int id = 0)
    {

        // �ٽ� Ŭ���� ���: ���� ����
        if (!isRestore && selectedQuestInstance != null && 
            selectedQuestInstance.data != null &&
            clickedQuest?.data != null &&
        selectedQuestInstance.data.questId == clickedQuest.data.questId)
        {
            // ���� ����
            foreach (Transform child in activeQuestListParent)
            {
                var holder = child.GetComponent<QuestUIDataHolder>();
                if (holder != null && holder.questId == id)
                {
                    GameObject newHighlight = child.GetChild(0).gameObject;
                    if (newHighlight != null)
                    {
                        Debug.Log("null �ƾƾƾƾӴ�");
                        newHighlight.SetActive(false);
                    }
                }
            }

            /*GameObject oldHighlight = selectedListItem.transform.GetChild(0).gameObject;
            if (oldHighlight != null) oldHighlight.SetActive(false);*/

            selectedListItem = null;
            selectedQuestInstance = null;

            // HUD ����
            questHud.DisplayQuest(null);

            return;
        }

        /*
        // ���� ���� ����
        if (!isRestore && selectedListItem != null)
        {
            GameObject oldHighlight = selectedListItem.transform.GetChild(0).gameObject;
            if (oldHighlight != null) oldHighlight.SetActive(false);
        }*/
        // ��� �׸񿡼� Highlight ���� (���� ����)
        foreach (Transform child in activeQuestListParent)
        {
            GameObject oldHighlight = child.GetChild(0).gameObject;
            if (oldHighlight != null) oldHighlight.SetActive(false);
        }


        // ���� ����
        //selectedListItem = clickedItem;
        selectedQuestInstance = clickedQuest;

        if (isRestore)
        {
            foreach (Transform child in activeQuestListParent)
            {
                var holder = child.GetComponent<QuestUIDataHolder>();
                if (holder != null && holder.questId == id)
                {
                    GameObject newHighlight = child.GetChild(0).gameObject;
                    if (newHighlight != null)
                    {
                       // Debug.Log("null �ƾƾƾƾӴ�");
                        newHighlight.SetActive(true);
                    }
                }
            }
        }
        else
        {
            //Debug.Log("dpdfpsasfdpfasdpasfdpsfdapsafdpfasdpfsda");
            GameObject newHighlight = clickedItem.transform.GetChild(0).gameObject;
            if (newHighlight != null)
            {
                Debug.Log("null �ƾƾƾƾӴ�");
                newHighlight.SetActive(true);
            }


            // HUD ǥ��
            questHud.DisplayQuest(clickedQuest);
        }

    }
}


// ��Ÿ�� �� �����Ȳ�� �����ϴ� ����Ʈ �ν��Ͻ�
[System.Serializable]
public class QuestInstance
{
    public QuestData data;
    public List<bool> objectiveCompletion = new List<bool>();

    public bool IsCompleted()
    {
        int count = objectiveCompletion.Count;

        if (data.hasChoiceAtEnd && data.choiceCount > 0)
        {
            int branchStart = count - data.choiceCount;

            // ������ ������ �� �ϳ��� �Ϸ������ OK
            if (objectiveCompletion.GetRange(branchStart, data.choiceCount).Exists(b => b))
                return true;
        }
        Debug.Log(objectiveCompletion.TrueForAll(b => b));
        // �Ϲ� ���� �Ϸ�
        return objectiveCompletion.TrueForAll(b => b);
    }

    public QuestInstance(QuestData data)
    {
        this.data = data;
        objectiveCompletion = new List<bool>(new bool[data.objectives.Count]);
    }
}

// UI ���� ��Ҵ� Unity Editor���� �Ʒ��� ���� ����:
// - questListItemPrefab: Button + LocalizeStringEvent ����
// - objectiveItemPrefab: Image (���) + Image("Checkmark") + LocalizeStringEvent ����
// - ����: ScrollView�� active/completed �и�
// - ������: ����, ����, ��ǥ ����Ʈ ���� ��ġ
