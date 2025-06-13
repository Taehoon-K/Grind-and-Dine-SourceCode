using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MapRealTooltip : MapTooltip
{
    /*[SerializeField]
    private LocalizeStringEvent nameLocalizeString;   // ������ �̸� Ű*/
    private List<LocalizeStringEvent> buildingList = new List<LocalizeStringEvent>(); // �ǹ� ��� �ؽ�Ʈ ����Ʈ
    [SerializeField] private GameObject buildingItemPrefab; // �ؽ�Ʈ ������
    [SerializeField] private Transform contentParent; // �θ� �����̳� (���� ����)
    [SerializeField] RectTransform targetRoot; // �θ� UI ��Ʈ

    void LateUpdate()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetRoot);
    }

    /// <summary> ���� UI�� ������ ���� ��� </summary>
    public void SetItemInfo(LocalizedString[] tooltipL)
    {
        // ���� UI ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        buildingList.Clear();

        // ���� ����
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
   