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
    private GameObject selectedListItem = null; //선택된 퀘스트 리스트
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

        // 선택 상태 복원
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
        yield return null; // 다음 프레임까지 대기
        HighlightSelectedQuest(item, quest, true, id);
        ShowQuestDetail(quest);
    }

    private void SetupQuestListItem(GameObject go, QuestInstance q, bool isActive)
    {
        go.AddComponent<QuestUIDataHolder>().questId = q.data.questId;

        // 제목 텍스트 로컬라이즈
        var localize = go.GetComponentInChildren<LocalizeStringEvent>();
        if (localize != null)
            localize.StringReference = q.data.titleLocalized;

        if (isActive) //활동중인 퀘스트만 클릭가능하게
        {
            // 클릭 시 선택 토글 + HUD 표시
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                HighlightSelectedQuest(go, q);
                ShowQuestDetail(q); // 클릭 시 상세는 항상 표시
            });
        }
        

        EventTrigger trigger = go.AddComponent<EventTrigger>();

        // 마우스 올렸을 때
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((eventData) =>
        {
            ShowQuestDetail(q);

            var bg = go.GetComponent<Image>();
            if (bg != null) bg.color = new Color(0.8f, 0.8f, 0.8f, 1f); // 밝게

            var newText = go.GetComponentInChildren<TextMeshProUGUI>();
            if (newText != null) newText.color = Color.black;
        });
        trigger.triggers.Add(entryEnter);

        // 마우스 내렸을 때
        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((eventData) =>
        {
            var bg = go.GetComponent<Image>();
            if (bg != null) bg.color = new Color(0.8f, 0.8f, 0.8f, 0f); // 밝게

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
        int choiceStart = total - quest.data.choiceCount; //퀘스트데이터에서 초이스카운트 찾기

        bool canShowBranches = true;

        // 선택지 표시 조건: 앞의 순차 목표가 모두 완료돼야 함
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

            // 선택지인데 아직 조건이 안 되면 숨김
            if (isSelectableBranch && !canShowBranches)
                continue;

            // 순차인데 아직 도달하지 않은 목표는 숨김
            if (quest.data.isSequential && !isCompleted && !isSelectableBranch && i > nextIndex)
                continue;

            GameObject go = Instantiate(objectiveItemPrefab, objectiveListParent);

            var objTextLocalize = go.GetComponentInChildren<LocalizeStringEvent>();
            if (objTextLocalize != null)
            {
                objTextLocalize.StringReference = obj.descriptionLocalized;
            }

            // 왼쪽 체크 이미지 처리
            Image checkImage = go.transform.GetChild(0).GetComponent<Image>();
            if (checkImage != null)
            {
                checkImage.enabled = isCompleted;
            }

            // 완료 시 색상 어둡게 처리
            Image background = go.GetComponent<Image>();
            if (background != null && isCompleted)
            {
                Color dim = background.color;
                dim.a = 0.5f; // 반투명 또는 어두운 느낌
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

        // 다시 클릭한 경우: 선택 해제
        if (!isRestore && selectedQuestInstance != null && 
            selectedQuestInstance.data != null &&
            clickedQuest?.data != null &&
        selectedQuestInstance.data.questId == clickedQuest.data.questId)
        {
            // 선택 해제
            foreach (Transform child in activeQuestListParent)
            {
                var holder = child.GetComponent<QuestUIDataHolder>();
                if (holder != null && holder.questId == id)
                {
                    GameObject newHighlight = child.GetChild(0).gameObject;
                    if (newHighlight != null)
                    {
                        Debug.Log("null 아아아아앙님");
                        newHighlight.SetActive(false);
                    }
                }
            }

            /*GameObject oldHighlight = selectedListItem.transform.GetChild(0).gameObject;
            if (oldHighlight != null) oldHighlight.SetActive(false);*/

            selectedListItem = null;
            selectedQuestInstance = null;

            // HUD 제거
            questHud.DisplayQuest(null);

            return;
        }

        /*
        // 이전 선택 해제
        if (!isRestore && selectedListItem != null)
        {
            GameObject oldHighlight = selectedListItem.transform.GetChild(0).gameObject;
            if (oldHighlight != null) oldHighlight.SetActive(false);
        }*/
        // 모든 항목에서 Highlight 끄기 (선택 해제)
        foreach (Transform child in activeQuestListParent)
        {
            GameObject oldHighlight = child.GetChild(0).gameObject;
            if (oldHighlight != null) oldHighlight.SetActive(false);
        }


        // 새로 선택
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
                       // Debug.Log("null 아아아아앙님");
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
                Debug.Log("null 아아아아앙님");
                newHighlight.SetActive(true);
            }


            // HUD 표시
            questHud.DisplayQuest(clickedQuest);
        }

    }
}


// 런타임 시 진행상황을 포함하는 퀘스트 인스턴스
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

            // 마지막 선택지 중 하나라도 완료됐으면 OK
            if (objectiveCompletion.GetRange(branchStart, data.choiceCount).Exists(b => b))
                return true;
        }
        Debug.Log(objectiveCompletion.TrueForAll(b => b));
        // 일반 순차 완료
        return objectiveCompletion.TrueForAll(b => b);
    }

    public QuestInstance(QuestData data)
    {
        this.data = data;
        objectiveCompletion = new List<bool>(new bool[data.objectives.Count]);
    }
}

// UI 구성 요소는 Unity Editor에서 아래와 같이 설정:
// - questListItemPrefab: Button + LocalizeStringEvent 포함
// - objectiveItemPrefab: Image (배경) + Image("Checkmark") + LocalizeStringEvent 포함
// - 왼쪽: ScrollView로 active/completed 분리
// - 오른쪽: 제목, 설명, 목표 리스트 영역 배치
