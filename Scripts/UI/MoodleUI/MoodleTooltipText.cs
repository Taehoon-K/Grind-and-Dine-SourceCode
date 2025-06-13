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
    private TextMeshProUGUI timeLeft; //남은시간

    private RectTransform _rt;
    private CanvasScaler _canvasScaler;

    private static readonly Vector2 LeftTop = new Vector2(0f, 1f);


    private void Awake()
    {
        Init();
        //Hide();
    }

    public void Render(int index, LocalizedString tooltip) //무들 인덱스 받기
    {
        tooltipString.StringReference = tooltip;

        Moodle[] mood = StatusManager.instance.GetMoodle();
        int totalMinutes = mood[index].timeLeft;
        int hours = totalMinutes / 60;   // 시간을 구함
        int minutes = totalMinutes % 60; // 남은 분을 구함

        // "00h 00m" 형식으로 포맷팅
        timeLeft.text = $"{hours:D2}h {minutes:D2}m";
    }

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
}
