using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.UI;


public class MoodleTooltipText : MonoBehaviour
{
    [SerializeField]
    private LocalizeStringEvent tooltipString;
    [SerializeField]
    private TextMeshProUGUI timeLeft; //�����ð�

    private RectTransform _rt;
    private CanvasScaler _canvasScaler;

    private static readonly Vector2 LeftTop = new Vector2(0f, 1f);


    private void Awake()
    {
        Init();
        //Hide();
    }

    public void Render(int index, LocalizedString tooltip) //���� �ε��� �ޱ�
    {
        tooltipString.StringReference = tooltip;

        Moodle[] mood = StatusManager.instance.GetMoodle();
        int totalMinutes = mood[index].timeLeft;
        int hours = totalMinutes / 60;   // �ð��� ����
        int minutes = totalMinutes % 60; // ���� ���� ����

        // "00h 00m" �������� ������
        timeLeft.text = $"{hours:D2}h {minutes:D2}m";
    }

    private void Init()
    {
        TryGetComponent(out _rt);
        _rt.pivot = LeftTop;
        _canvasScaler = GetComponentInParent<CanvasScaler>();

        DisableAllChildrenRaycastTarget(transform);
    }

    /// <summary> ��� �ڽ� UI�� ����ĳ��Ʈ Ÿ�� ���� </summary>
    private void DisableAllChildrenRaycastTarget(Transform tr)
    {
        // ������ Graphic(UI)�� ����ϸ� ����ĳ��Ʈ Ÿ�� ����
        tr.TryGetComponent(out Graphic gr);
        if (gr != null)
            gr.raycastTarget = false;

        // �ڽ��� ������ ����
        int childCount = tr.childCount;
        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            DisableAllChildrenRaycastTarget(tr.GetChild(i));
        }
    }
}
