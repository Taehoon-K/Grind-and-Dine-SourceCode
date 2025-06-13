using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MapTooltip : MonoBehaviour
{
    /***********************************************************************
    *                           Inspector Option Fields
    ***********************************************************************/
    #region .
    [SerializeField]
    private LocalizeStringEvent nameLocalizeString;   // ������ �̸� Ű
    [SerializeField]
    private LocalizeStringEvent tooltipLocalizeString; // ������ ���� �ؽ�Ʈ


    private int amount1;
    #endregion
    /***********************************************************************
    *                               Private Fields
    ***********************************************************************/
    #region .
    protected RectTransform _rt;
    protected CanvasScaler _canvasScaler;

    protected static readonly Vector2 LeftTop = new Vector2(0f, 1f);
    protected static readonly Vector2 LeftBottom = new Vector2(0f, 0f);
    protected static readonly Vector2 RightTop = new Vector2(1f, 1f);
    protected static readonly Vector2 RightBottom = new Vector2(1f, 0f);

    #endregion
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    protected void Awake()
    {
        Init();
        Hide();
    }

    #endregion
    /***********************************************************************
    *                               Private Methods
    ***********************************************************************/
    #region .
    protected void Init()
    {
        TryGetComponent(out _rt);
        _rt.pivot = LeftTop;
        _canvasScaler = GetComponentInParent<CanvasScaler>();

        DisableAllChildrenRaycastTarget(transform);
    }

    /// <summary> ��� �ڽ� UI�� ����ĳ��Ʈ Ÿ�� ���� </summary>
    protected void DisableAllChildrenRaycastTarget(Transform tr)
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

    #endregion
    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .
    /// <summary> ���� UI�� ������ ���� ��� </summary>
    public void SetItemInfo(LocalizedString nameL, LocalizedString tooltipL)
    {
        if(nameLocalizeString != null)
        {
            nameLocalizeString.StringReference = nameL;
        }
        tooltipLocalizeString.StringReference = tooltipL;
    }

    /// <summary> ������ ��ġ ���� </summary>
    public void SetRectPosition(RectTransform slotRect)
    {
        // ĵ���� �����Ϸ��� ���� �ػ� ����
        float wRatio = Screen.width / _canvasScaler.referenceResolution.x;
        float hRatio = Screen.height / _canvasScaler.referenceResolution.y;
        float ratio =
            wRatio * (1f - _canvasScaler.matchWidthOrHeight) +
            hRatio * (_canvasScaler.matchWidthOrHeight);

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        // ���� �ʱ� ��ġ(���� ���ϴ�) ����
        _rt.localPosition = slotRect.localPosition + new Vector3(-slotWidth - 200, -slotHeight - 300);

        Vector2 pos = _rt.localPosition;

        // ������ ũ��
        float width = _rt.rect.width * ratio;
        float height = _rt.rect.height * ratio;

        // ����, �ϴ��� �߷ȴ��� ����
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        ref bool R = ref rightTruncated;
        ref bool B = ref bottomTruncated;

        // �����ʸ� �߸� => ������ Left Bottom �������� ǥ��
        if (R && !B)
        {
            _rt.localPosition = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        // �Ʒ��ʸ� �߸� => ������ Right Top �������� ǥ��
        else if (!R && B)
        {
            _rt.localPosition = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        // ��� �߸� => ������ Left Top �������� ǥ��
        else if (R && B)
        {
            _rt.localPosition = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
        // �߸��� ���� => ������ Right Bottom �������� ǥ��
        // Do Nothing
        Debug.Log(_rt.localPosition);
    }


    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);

    #endregion
}
