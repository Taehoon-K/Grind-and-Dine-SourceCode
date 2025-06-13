using System.Collections;
using System.Collections.Generic;
using PP.InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class JobLogButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Act")]
    [SerializeField] private bool isActivity; //만약 활동 사진이면 트루
    [SerializeField] private int actHour; //만약 활동 사진이면 작업시간

    [Header("Default")]
    [SerializeField]
    private LocalizedString nameLocalizeString;   // 작업 이름 키
    [SerializeField]
    private LocalizedString levelLocalizeString; //요구 레벨 키
    [SerializeField]
    private SimulationJobStat jobStat;
    [SerializeField]
    private LocalizedString location;
    [SerializeField]
    private LocalizedString hour;
    [SerializeField]
    private LocalizedString stat; //소모 스탯
    [SerializeField]
    private LocalizedString tooltipLocalizeString;


    [SerializeField] private JobLogTooltip jobTooltip;   // 아이템 정보를 보여줄 툴팁 UI

    [SerializeField] private GameObject canvas; //최상위 캔버스

    private RectTransform buttonRectTransform;
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void UpdateTooltipUI()
    {
        if (isActivity) //만약 활동이면
        {
            // 툴팁 정보 갱신
            jobTooltip.SetActivityInfo(nameLocalizeString, location, actHour, stat, tooltipLocalizeString);
        }
        else //작업이면
        {
            // 툴팁 정보 갱신
            jobTooltip.SetItemInfo(nameLocalizeString, levelLocalizeString, jobStat.BaseSalary, location, hour, stat, tooltipLocalizeString);
        }
        

        buttonRectTransform = GetComponent<RectTransform>();
        // 툴팁 위치 조정
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            buttonRectTransform,
            Input.mousePosition,
            Camera.main,  // 월드 스페이스 캔버스에서는 카메라를 지정해야 합니다.
            out worldPoint
        );

        RectTransform tooltipRect = jobTooltip.GetComponent<RectTransform>();
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
        Vector3 adjustedPosition = worldPoint;

        // 우하단 기본 위치
        adjustedPosition = worldPoint;

        // 잘림 여부 확인
        bool rightTruncated = tooltipCorners[2].x > canvasCorners[2].x; // 오른쪽 잘림
        bool bottomTruncated = tooltipCorners[0].y < canvasCorners[0].y; // 아래쪽 잘림
        bool leftTruncated = tooltipCorners[0].x < canvasCorners[0].x; // 왼쪽 잘림
        bool topTruncated = tooltipCorners[1].y > canvasCorners[1].y; // 위쪽 잘림

        // 잘림에 따른 방향 변경
        if (rightTruncated) // 오른쪽 잘림: 왼쪽으로 이동
        {
            adjustedPosition.x -= (tooltipCorners[2].x - canvasCorners[2].x);
        }
        if (bottomTruncated) // 아래쪽 잘림: 위로 이동
        {
            adjustedPosition.y += (canvasCorners[0].y - tooltipCorners[0].y);
        }
        if (leftTruncated) // 왼쪽 잘림: 오른쪽으로 이동
        {
            adjustedPosition.x += (canvasCorners[0].x - tooltipCorners[0].x);
        }
        if (topTruncated) // 위쪽 잘림: 아래로 이동
        {
            adjustedPosition.y -= (tooltipCorners[1].y - canvasCorners[1].y);
        }

        // 최종 위치 반영
        tooltipRect.position = adjustedPosition;
        tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x, tooltipRect.localPosition.y, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        /*if (image.color == Color.white) //만약 콜렉션 열린 경우만
        {
            UpdateTooltipUI();
            jobTooltip.Show();
        }*/
        UpdateTooltipUI();
        jobTooltip.Show(); //일단은 무조건 툴팁 보이게
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        jobTooltip.Hide();
    }
}
