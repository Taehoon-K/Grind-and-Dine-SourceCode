using System.Collections;
using System.Collections.Generic;
using PP.InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

public class CollecItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemData itemData;
    //private LocalizedString name_key, tooltip_key,informationKey;
    //[SerializeField] private LocalizedString[] buildingName;

    [SerializeField] private ItemTooltipUI itemTooltip;   // ������ ������ ������ ���� UI

    [SerializeField] private GameObject canvas; //�ֻ��� ĵ����

    private RectTransform buttonRectTransform;
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void UpdateTooltipUI()
    {
        // ���� ���� ����
        itemTooltip.SetItemInfo(itemData,true);

        buttonRectTransform = GetComponent<RectTransform>();
        // ���� ��ġ ����
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            buttonRectTransform,
            Input.mousePosition,
            Camera.main,  // ���� �����̽� ĵ���������� ī�޶� �����ؾ� �մϴ�.
            out worldPoint
        );

        RectTransform tooltipRect = itemTooltip.GetComponent<RectTransform>();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(image.color != Color.black) //���� �ݷ��� ���� ��츸
        {
            UpdateTooltipUI();
            itemTooltip.Show();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemTooltip.Hide();
    }
}
