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
    public int skillIndex; // ��ų �ε���
    public int perkLevelIndex; // �� ���� �ε���

    [SerializeField]
    private GameObject lockObject, unlockObject; // ��� �̹���
    [SerializeField] private LocalizeStringEvent selectObject;   // ������ ������ ������ ���� UI
    private LocalizedString skillDes;
    private Image backgroundImage; // ��ư �̹���
    private Button button; // ��ư ������Ʈ

    [SerializeField] private MapTooltip mapTooltip;   // ������ ������ ������ ���� UI
    [SerializeField] private GameObject canvas; //�ֻ��� ĵ����
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
    /// PerkManager�� Ȱ���� ��ư ���¸� ������Ʈ�մϴ�.
    /// </summary>
    public void Render()
    {

        // ���� ���� ��������
        int perkState = StatusManager.instance.GetPerkState(skillIndex, perkLevelIndex);

        switch (perkState)
        {
            case 0: // ��� ����
                button.interactable = false;
                lockObject.SetActive(true);
                unlockObject.SetActive(false);
                selectObject.gameObject.SetActive(false);
                backgroundImage.color = new Color(0f, 0f, 0f); // ��ο� ��
                break;

            case 1: // ���� ���� ����
                button.interactable = true;
                lockObject.SetActive(false); //�ڹ��� ��Ȱ��ȭ��Ű��
                unlockObject.SetActive(true);
                selectObject.gameObject.SetActive(false);
                backgroundImage.color = new Color(1f, 1f, 1f); // ���� ��
                break;

            case 2: // ���� �Ϸ� ����
                button.interactable = false;
                lockObject.SetActive(false);
                unlockObject.SetActive(false);
                selectObject.gameObject.SetActive(true);
                selectObject.StringReference = StatusManager.instance.GetSelectedPerkString(skillIndex, perkLevelIndex);
                skillDes = StatusManager.instance.GetSelectedPerkStringDes(skillIndex, perkLevelIndex);
                backgroundImage.sprite = StatusManager.instance.GetSelectedPerkImage(skillIndex, perkLevelIndex); // ������ �̹��� ����
                backgroundImage.color = new Color(1f, 1f, 1f); // ���� ��
                break;

            default:
                Debug.LogError("Invalid perk state.");
                break;
        }
    }

    /// <summary>
    /// ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void SelectPerk()
    {
        int perkState = StatusManager.instance.GetPerkState(skillIndex, perkLevelIndex);

        if (perkState == 1) // ���� ���� ����
        {
            // UIManager�� ���� ������Ʈ ȣ��
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
        // ���� ���� ����
        mapTooltip.SetItemInfo(null, skillDes);

        buttonRectTransform = GetComponent<RectTransform>();
        // ���� ��ġ ����
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            buttonRectTransform,
            Input.mousePosition,
            Camera.main,  // ���� �����̽� ĵ���������� ī�޶� �����ؾ� �մϴ�.
            out worldPoint
        );

        RectTransform tooltipRect = mapTooltip.GetComponent<RectTransform>();
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
        Vector3 adjustedPosition = tooltipRect.position;

        // ���ϴ� �⺻ ��ġ
        adjustedPosition = worldPoint;

        // �߸� ���� Ȯ��
        bool rightTruncated = tooltipCorners[2].x > canvasCorners[2].x; // ������ �߸�
        bool bottomTruncated = tooltipCorners[0].y < canvasCorners[0].y; // �Ʒ��� �߸�
        bool leftTruncated = tooltipCorners[0].x < canvasCorners[0].x; // ���� �߸�
        bool topTruncated = tooltipCorners[1].y > canvasCorners[1].y; // ���� �߸�

        // �߸��� ���� ���� ����
        if (rightTruncated && bottomTruncated) // ������ & �Ʒ��� �߸�: �»��
        {
            adjustedPosition = worldPoint + new Vector3(-tooltipWidth, tooltipHeight, 0);
        }
        else if (rightTruncated) // �����ʸ� �߸�: ���ϴ�
        {
            adjustedPosition = worldPoint + new Vector3(-tooltipWidth, 0, 0);
        }
        else if (bottomTruncated) // �Ʒ��ʸ� �߸�: ����
        {
            adjustedPosition = worldPoint + new Vector3(0, tooltipHeight, 0);
        }
        else if (leftTruncated && bottomTruncated) // ���� & �Ʒ��� �߸�: ����
        {
            adjustedPosition = worldPoint + new Vector3(tooltipWidth, tooltipHeight, 0);
        }
        else if (leftTruncated) // ���ʸ� �߸�: ���ϴ�
        {
            adjustedPosition = worldPoint + new Vector3(tooltipWidth, 0, 0);
        }
        else if (topTruncated) // ���ʸ� �߸�: ���ϴ�
        {
            adjustedPosition = worldPoint + new Vector3(-tooltipWidth, -tooltipHeight, 0);
        }

        // ���� ��ġ �ݿ�
        tooltipRect.position = adjustedPosition;
        tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x, tooltipRect.localPosition.y, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectObject.gameObject.activeSelf) //���� �� ���õ� ���¸�
        {
            UpdateTooltipUI();
            mapTooltip.Show();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectObject.gameObject.activeSelf) //���� �� ���õ� ���¸�
        {
            mapTooltip.Hide();
        }
    }
    #endregion
}
