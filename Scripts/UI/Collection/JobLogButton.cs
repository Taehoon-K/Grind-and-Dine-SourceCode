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
    [SerializeField] private bool isActivity; //���� Ȱ�� �����̸� Ʈ��
    [SerializeField] private int actHour; //���� Ȱ�� �����̸� �۾��ð�

    [Header("Default")]
    [SerializeField]
    private LocalizedString nameLocalizeString;   // �۾� �̸� Ű
    [SerializeField]
    private LocalizedString levelLocalizeString; //�䱸 ���� Ű
    [SerializeField]
    private SimulationJobStat jobStat;
    [SerializeField]
    private LocalizedString location;
    [SerializeField]
    private LocalizedString hour;
    [SerializeField]
    private LocalizedString stat; //�Ҹ� ����
    [SerializeField]
    private LocalizedString tooltipLocalizeString;


    [SerializeField] private JobLogTooltip jobTooltip;   // ������ ������ ������ ���� UI

    [SerializeField] private GameObject canvas; //�ֻ��� ĵ����

    private RectTransform buttonRectTransform;
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void UpdateTooltipUI()
    {
        if (isActivity) //���� Ȱ���̸�
        {
            // ���� ���� ����
            jobTooltip.SetActivityInfo(nameLocalizeString, location, actHour, stat, tooltipLocalizeString);
        }
        else //�۾��̸�
        {
            // ���� ���� ����
            jobTooltip.SetItemInfo(nameLocalizeString, levelLocalizeString, jobStat.BaseSalary, location, hour, stat, tooltipLocalizeString);
        }
        

        buttonRectTransform = GetComponent<RectTransform>();
        // ���� ��ġ ����
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            buttonRectTransform,
            Input.mousePosition,
            Camera.main,  // ���� �����̽� ĵ���������� ī�޶� �����ؾ� �մϴ�.
            out worldPoint
        );

        RectTransform tooltipRect = jobTooltip.GetComponent<RectTransform>();
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
        /*if (image.color == Color.white) //���� �ݷ��� ���� ��츸
        {
            UpdateTooltipUI();
            jobTooltip.Show();
        }*/
        UpdateTooltipUI();
        jobTooltip.Show(); //�ϴ��� ������ ���� ���̰�
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        jobTooltip.Hide();
    }
}
