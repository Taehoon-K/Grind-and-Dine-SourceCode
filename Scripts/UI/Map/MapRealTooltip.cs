using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MapRealTooltip : MapTooltip
{
    /*[SerializeField]
    private LocalizeStringEvent nameLocalizeString;   // 아이템 이름 키*/
    private List<LocalizeStringEvent> buildingList = new List<LocalizeStringEvent>(); // 건물 목록 텍스트 리스트
    [SerializeField] private GameObject buildingItemPrefab; // 텍스트 프리팹
    [SerializeField] private Transform contentParent; // 부모 컨테이너 (세로 정렬)
    [SerializeField] RectTransform targetRoot; // 부모 UI 루트

    void LateUpdate()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetRoot);
    }

    /// <summary> 툴팁 UI에 아이템 정보 등록 </summary>
    public void SetItemInfo(LocalizedString[] tooltipL)
    {
        // 기존 UI 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        buildingList.Clear();

        // 새로 생성
        for (int i = 0; i < tooltipL.Length; i++)
        {
            GameObject item = Instantiate(buildingItemPrefab, contentParent);
            LocalizeStringEvent localize = item.GetComponent<LocalizeStringEvent>();

            if (localize != null)
            {
                localize.StringReference = tooltipL[i];
                buildingList.Add(localize);
            }
        }
    }


}
   