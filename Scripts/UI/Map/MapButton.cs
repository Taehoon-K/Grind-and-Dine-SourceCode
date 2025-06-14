using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class MapButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private LocalizedString name_key,tooltip_key;
    //[SerializeField] private LocalizedString[] buildingName;

    [SerializeField] private MapTooltip mapTooltip;   // 아이템 정보를 보여줄 툴팁 UI

    [SerializeField] private GameObject canvas; //최상위 캔버스

    private RectTransform buttonRectTransform;
    //private bool isValid;

    /*  private void Update()
      {
          if (isValid)
          {

              UpdateTooltipUI();
              mapTooltip.Show();
          }
          else
              mapTooltip.Hide();
      }*/

    private void UpdateTooltipUI()
    {
        // 툴팁 정보 갱신
        mapTooltip.SetItemInfo(name_key, tooltip_key);

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
        UpdateTooltipUI();
        mapTooltip.Show();
        //여기에 색 변경 추가할것
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mapTooltip.Hide();

    }
}
