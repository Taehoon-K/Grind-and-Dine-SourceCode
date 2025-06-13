using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuestHud : MonoBehaviour
{
    public LocalizeStringEvent questTitleText;
    public Transform objectiveListParent;
    public GameObject objectiveItemPrefab;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void DisplayQuest(QuestInstance quest)
    {
       
        if (quest == null || quest.data == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        questTitleText.StringReference = quest.data.titleLocalized;

        // 기존 목표 제거
        foreach (Transform child in objectiveListParent)
        {
            Destroy(child.gameObject);
        }

        int nextIndex = quest.objectiveCompletion.FindIndex(b => !b);
        int total = quest.data.objectives.Count;
        int choiceStart = total - quest.data.choiceCount;

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

        // 목표 표시
        for (int i = 0; i < quest.data.objectives.Count; i++)
        {
            var objData = quest.data.objectives[i];
            bool isDone = quest.objectiveCompletion[i];

            // 순차 퀘스트: 다음 순서까지 도달하지 않으면 표시 안함
            bool isCompleted = quest.objectiveCompletion[i];
            bool isSelectableBranch = quest.data.hasChoiceAtEnd && i >= choiceStart;

            // 선택지인데 아직 조건이 안 되면 숨김
            if (isSelectableBranch && !canShowBranches)
                continue;

            // 순차인데 아직 도달하지 않은 목표는 숨김
            if (quest.data.isSequential && !isCompleted && !isSelectableBranch && i > nextIndex)
                continue;


            GameObject go = Instantiate(objectiveItemPrefab, objectiveListParent);

            var textLocalizer = go.GetComponentInChildren<LocalizeStringEvent>();
            if (textLocalizer != null)
                textLocalizer.StringReference = objData.descriptionLocalized;

            // 왼쪽 체크 이미지 처리
            /*Image checkImage = go.transform.GetChild(0).GetComponent<Image>();
            if (checkImage != null)
            {
                checkImage.enabled = isDone;
            }*/
            GameObject checkImage = go.transform.GetChild(0).gameObject;
            if (checkImage != null) checkImage.SetActive(isDone);


            var oldText = go.GetComponentInChildren<TextMeshProUGUI>();
            if (oldText != null && isDone)
            {
                oldText.color = Color.gray;
            }

            /*
            var background = go.GetComponent<Image>();
            if (background != null && isDone)
            {
                Color dim = background.color;
                dim.a = 0.5f;
                background.color = dim;
            }*/
        }
    }
}
