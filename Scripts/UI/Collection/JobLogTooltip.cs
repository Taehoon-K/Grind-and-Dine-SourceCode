using System.Collections;
using System.Collections.Generic;
using PP.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class JobLogTooltip : MonoBehaviour
{
    /***********************************************************************
    *                           Inspector Option Fields
    ***********************************************************************/
    #region .

    [SerializeField]
    private LocalizeStringEvent nameLocalizeString;   // 작업 이름 키

    [SerializeField]
    private LocalizeStringEvent levelLocalizeString; //요구 레벨 키
    [SerializeField]
    private Text wage;
    [SerializeField]
    private LocalizeStringEvent location;
    [SerializeField]
    private LocalizeStringEvent hour;
    [SerializeField]
    private LocalizeStringEvent stat; //소모 스탯
    [SerializeField]
    private LocalizeStringEvent tooltipLocalizeString;


    private int amount1;
    #endregion
    /***********************************************************************
    *                               Private Fields
    ***********************************************************************/
    #region .
    private RectTransform _rt;
    private CanvasScaler _canvasScaler;

    private static readonly Vector2 LeftTop = new Vector2(0f, 1f);
    private static readonly Vector2 LeftBottom = new Vector2(0f, 0f);
    private static readonly Vector2 RightTop = new Vector2(1f, 1f);
    private static readonly Vector2 RightBottom = new Vector2(1f, 0f);

    #endregion
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Awake()
    {
        Init();
        Hide();
    }

    #endregion
    /***********************************************************************
    *                               Private Methods
    ***********************************************************************/
    #region .
    private void Init()
    {
        TryGetComponent(out _rt);
        _rt.pivot = LeftTop;
        _canvasScaler = GetComponentInParent<CanvasScaler>();

        DisableAllChildrenRaycastTarget(transform);
    }

    /// <summary> 모든 자식 UI에 레이캐스트 타겟 해제 </summary>
    private void DisableAllChildrenRaycastTarget(Transform tr)
    {
        // 본인이 Graphic(UI)를 상속하면 레이캐스트 타겟 해제
        tr.TryGetComponent(out Graphic gr);
        if (gr != null)
            gr.raycastTarget = false;

        // 자식이 없으면 종료
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
    /// <summary> 툴팁 UI에 아이템 정보 등록 </summary>
    public void SetItemInfo(LocalizedString nameL, LocalizedString levelL, int wageL, LocalizedString locationL,LocalizedString hourL, LocalizedString statL, LocalizedString toolL)
    {
        if (nameL != null)
            nameLocalizeString.StringReference = nameL;

        if (levelL != null)
            levelLocalizeString.StringReference = levelL;

        if (wageL != 0)
            wage.text = "￦ "+ wageL.ToString("N0");

        if (locationL != null)
            location.StringReference = locationL;

        if (statL != null)
            stat.StringReference = statL;

        if (toolL != null)
            tooltipLocalizeString.StringReference = toolL;

        if (hourL != null)
            hour.StringReference = hourL;
    }
    public void SetActivityInfo(LocalizedString nameL, LocalizedString locationL, int hourL, LocalizedString statL, LocalizedString toolL) //활동용
    {
        if (nameL != null)
            nameLocalizeString.StringReference = nameL;

        if (locationL != null)
            location.StringReference = locationL;

        if (statL != null)
            stat.StringReference = statL;

        if (toolL != null)
            tooltipLocalizeString.StringReference = toolL;

        hour.gameObject.GetComponent<TextMeshProUGUI>().text = hourL.ToString();
    }
    /// <summary> 툴팁의 위치 조정 </summary>
    public void SetRectPosition(RectTransform slotRect)
    {
        // 캔버스 스케일러에 따른 해상도 대응
        float wRatio = Screen.width / _canvasScaler.referenceResolution.x;
        float hRatio = Screen.height / _canvasScaler.referenceResolution.y;
        float ratio =
            wRatio * (1f - _canvasScaler.matchWidthOrHeight) +
            hRatio * (_canvasScaler.matchWidthOrHeight);

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        // 툴팁 초기 위치(슬롯 우하단) 설정
        _rt.localPosition = slotRect.localPosition + new Vector3(-slotWidth - 200, -slotHeight - 300);

        Vector2 pos = _rt.localPosition;

        // 툴팁의 크기
        float width = _rt.rect.width * ratio;
        float height = _rt.rect.height * ratio;

        // 우측, 하단이 잘렸는지 여부
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        ref bool R = ref rightTruncated;
        ref bool B = ref bottomTruncated;

        // 오른쪽만 잘림 => 슬롯의 Left Bottom 방향으로 표시
        if (R && !B)
        {
            _rt.localPosition = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        // 아래쪽만 잘림 => 슬롯의 Right Top 방향으로 표시
        else if (!R && B)
        {
            _rt.localPosition = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        // 모두 잘림 => 슬롯의 Left Top 방향으로 표시
        else if (R && B)
        {
            _rt.localPosition = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
        // 잘리지 않음 => 슬롯의 Right Bottom 방향으로 표시
        // Do Nothing
        //Debug.Log(_rt.localPosition);
    }


    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);

    #endregion
}
