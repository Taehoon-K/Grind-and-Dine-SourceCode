using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PerkButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int skillIndex; // 스킬 인덱스
    public int perkLevelIndex; // 퍽 레벨 인덱스

    [SerializeField]
    private GameObject lockObject, unlockObject; // 잠금 이미지
    [SerializeField] private LocalizeStringEvent selectObject;   // 아이템 정보를 보여줄 툴팁 UI
    private LocalizedString skillDes;
    private Image backgroundImage; // 버튼 이미지
    private Button button; // 버튼 컴포넌트

    [SerializeField] private MapTooltip mapTooltip;   // 아이템 정보를 보여줄 툴팁 UI
    [SerializeField] private GameObject canvas; //최상위 캔버스
    private RectTransform buttonRectTransform;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }
    private void OnEnable()
    {
        Render();
    }

    /// <summary>
    /// PerkManager를 활용해 버튼 상태를 업데이트합니다.
    /// </summary>
    public void Render()
    {

        // 현재 상태 가져오기
        int perkState = StatusManager.instance.GetPerkState(skillIndex, perkLevelIndex);

        switch (perkState)
        {
            case 0: // 잠금 상태
                button.interactable = false;
                lockObject.SetActive(true);
                unlockObject.SetActive(false);
                selectObject.gameObject.SetActive(false);
                backgroundImage.color = new Color(0f, 0f, 0f); // 어두운 색
                break;

            case 1: // 선택 가능 상태
                button.interactable = true;
                lockObject.SetActive(false); //자물쇠 비활성화시키고
                unlockObject.SetActive(true);
                selectObject.gameObject.SetActive(false);
                backgroundImage.color = new Color(1f, 1f, 1f); // 원래 색
                break;

            case 2: // 선택 완료 상태
                button.interactable = false;
                lockObject.SetActive(false);
                unlockObject.SetActive(false);
                selectObject.gameObject.SetActive(true);
                selectObject.StringReference = StatusManager.instance.GetSelectedPerkString(skillIndex, perkLevelIndex);
                skillDes = StatusManager.instance.GetSelectedPerkStringDes(skillIndex, perkLevelIndex);
                backgroundImage.sprite = StatusManager.instance.GetSelectedPerkImage(skillIndex, perkLevelIndex); // 선택한 이미지 설정
                backgroundImage.color = new Color(1f, 1f, 1f); // 원래 색
                break;

            default:
                Debug.LogError("Invalid perk state.");
                break;
        }
    }

    /// <summary>
    /// 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void SelectPerk()
    {
        int perkState = StatusManager.instance.GetPerkState(skillIndex, perkLevelIndex);

        if (perkState == 1) // 선택 가능 상태
        {
            // UIManager를 통해 프롬프트 호출
            UIManager.instance.TriggerSkillPrompt(skillIndex, perkLevelIndex);
        }
        else
        {
            Debug.LogWarning("Perk is not selectable.");
        }
    }

    #region Tooltip
    private void UpdateTooltipUI()
    {
        // 툴팁 정보 갱신
        mapTooltip.SetItemInfo(null, skillDes);

        buttonRectTransform = GetComponent<RectTransform>();
        // 툴팁 위치 조정
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            buttonRectTransform,
            Input.mousePosition,
            Camera.main,  // 월드 스페이스 캔버스에서는 카메라를 지정해야 합니다.
            out worldPoint
        );

        RectTransform tooltipRect = mapTooltip.GetComponent<RectTransform>();
        tooltipRect.position = worldPoint;

        // 툴팁의 월드 크기 계산
        Vector3[] tooltipCorners = new Vector3[4];
        tooltipRect.GetWorldCorners(tooltipCorners);

        Vector3[] canvasCorners = new Vector3[4];
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.GetWorldCorners(canvasCorners);

        // 툴팁 크기
        float tooltipWidth = tooltipCorners[2].x - tooltipCorners[0].x;
        float tooltipHeight = tooltipCorners[1].y - tooltipCorners[0].y;

        // 화면 경계를 고려한 방향 변경
        Vector3 adjustedPosition = tooltipRect.position;

        // 우하단 기본 위치
        adjustedPosition = worldPoint;

        // 잘림 여부 확인
        bool rightTruncated = tooltipCorners[2].x > canvasCorners[2].x; // 오른쪽 잘림
        bool bottomTruncated = tooltipCorners[0].y < canvasCorners[0].y; // 아래쪽 잘림
        bool leftTruncated = tooltipCorners[0].x < canvasCorners[0].x; // 왼쪽 잘림
        bool topTruncated = tooltipCorners[1].y > canvasCorners[1].y; // 위쪽 잘림

        // 잘림에 따른 방향 변경
        if (rightTruncated && bottomTruncated) // 오른쪽 & 아래쪽 잘림: 좌상단
        {
            adjustedPosition = worldPoint + new Vector3(-tooltipWidth, tooltipHeight, 0);
        }
        else if (rightTruncated) // 오른쪽만 잘림: 좌하단
        {
            adjustedPosition = worldPoint + new Vector3(-tooltipWidth, 0, 0);
        }
        else if (bottomTruncated) // 아래쪽만 잘림: 우상단
        {
            adjustedPosition = worldPoint + new Vector3(0, tooltipHeight, 0);
        }
        else if (leftTruncated && bottomTruncated) // 왼쪽 & 아래쪽 잘림: 우상단
        {
            adjustedPosition = worldPoint + new Vector3(tooltipWidth, tooltipHeight, 0);
        }
        else if (leftTruncated) // 왼쪽만 잘림: 우하단
        {
            adjustedPosition = worldPoint + new Vector3(tooltipWidth, 0, 0);
        }
        else if (topTruncated) // 위쪽만 잘림: 좌하단
        {
            adjustedPosition = worldPoint + new Vector3(-tooltipWidth, -tooltipHeight, 0);
        }

        // 최종 위치 반영
        tooltipRect.position = adjustedPosition;
        tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x, tooltipRect.localPosition.y, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectObject.gameObject.activeSelf) //만약 퍽 선택된 상태면
        {
            UpdateTooltipUI();
            mapTooltip.Show();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectObject.gameObject.activeSelf) //만약 퍽 선택된 상태면
        {
            mapTooltip.Hide();
        }
    }
    #endregion
}
