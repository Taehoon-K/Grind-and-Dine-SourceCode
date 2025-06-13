using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class MoodleTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private int index;
    [SerializeField]
    private LocalizedString tooltipKey; //�ش� ���� ����Ű
    [SerializeField]
    private MoodleTooltipText tooltipObject; // �������� ����� UI ������Ʈ

    [SerializeField] private GameObject canvas; //�ֻ��� ĵ����

    private RectTransform buttonRectTransform; 

   private void UpdateTooltipUI()
    {


        buttonRectTransform = GetComponent<RectTransform>();
        // ���� ��ġ ����
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            buttonRectTransform,
            Input.mousePosition,
            Camera.main,  // ���� �����̽� ĵ���������� ī�޶� �����ؾ� �մϴ�.
            out worldPoint
        );

        RectTransform tooltipRect = tooltipObject.GetComponent<RectTransform>();
        tooltipRect.position = worldPoint;

        // ������ ���� ũ�� ���
        Vector3[] tooltipCorners = new Vector3[4];
        tooltipRect.GetWorldCorners(tooltipCorners);

        Vector3[] canvasCorners = new Vector3[4];
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.GetWorldCorners(canvasCorners);

        // ���� ũ��
        float tooltipWidth = tooltipCorners[2].x - tooltipCorners[0].x;
        float tooltipHeight = tooltipCorners[1].y - tooltipCorners[0].y;

        // ȭ�� ��踦 ����� ���� ����
        Vector3 adjustedPosition = worldPoint;

        // ���ϴ� �⺻ ��ġ
        adjustedPosition = worldPoint;

        // �߸� ���� Ȯ��
        bool rightTruncated = tooltipCorners[2].x > canvasCorners[2].x; // ������ �߸�
        bool bottomTruncated = tooltipCorners[0].y < canvasCorners[0].y; // �Ʒ��� �߸�
        bool leftTruncated = tooltipCorners[0].x < canvasCorners[0].x; // ���� �߸�
        bool topTruncated = tooltipCorners[1].y > canvasCorners[1].y; // ���� �߸�

        // �߸��� ���� ���� ����
        if (rightTruncated) // ������ �߸�: �������� �̵�
        {
            adjustedPosition.x -= (tooltipCorners[2].x - canvasCorners[2].x);
        }
        if (bottomTruncated) // �Ʒ��� �߸�: ���� �̵�
        {
            adjustedPosition.y += (canvasCorners[0].y - tooltipCorners[0].y);
        }
        if (leftTruncated) // ���� �߸�: ���������� �̵�
        {
            adjustedPosition.x += (canvasCorners[0].x - tooltipCorners[0].x);
        }
        if (topTruncated) // ���� �߸�: �Ʒ��� �̵�
        {
            adjustedPosition.y -= (tooltipCorners[1].y - canvasCorners[1].y);
        }

        // ���� ��ġ �ݿ�
        tooltipRect.position = adjustedPosition;
        tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x, tooltipRect.localPosition.y, 0);
    }


    // Ŀ���� ���� ������Ʈ ���� �ö��� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipObject.gameObject.SetActive(true);
        UpdateTooltipUI();
        tooltipObject.GetComponent<MoodleTooltipText>().Render(index, tooltipKey); // ������ Ȱ��ȭ
    }

    // Ŀ���� ���� ������Ʈ���� ����� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipObject.gameObject.SetActive(false); // ������ ��Ȱ��ȭ
    }
}