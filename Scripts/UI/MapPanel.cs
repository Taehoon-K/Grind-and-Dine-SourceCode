using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class MapPanel : MonoBehaviour
{
    private List<GameObject> currentMarkers = new();
    [SerializeField] GameObject markerPrefab;
    [SerializeField] Transform markerParent;

    [SerializeField] GameObject myLocation;

    [SerializeField] float worldMinX = -190f;
    [SerializeField] float worldMaxX = 120f;
    [SerializeField] float worldMinZ = -170f;
    [SerializeField] float worldMaxZ = 200f;
    private void OnEnable()
    {
        Render();
    }
    public void Render()
    {
        ClearMarkers();
        ShowMyLocation();

        QuestInstance quest = QuestManager.instance?.questUI.selectedQuestInstance;
        if (quest == null || quest.data == null) return;

        var data = quest.data;
        var completion = quest.objectiveCompletion;

        int total = data.objectives.Count;
        int choiceStart = total - data.choiceCount;

        for (int i = 0; i < total; i++)
        {
            bool isCompleted = completion[i];
            bool isSelectableBranch = data.hasChoiceAtEnd && i >= choiceStart;

            // 조건: 이미 완료된 목표는 스킵
            if (isCompleted) continue;

            // 순차형이고 선택지가 아니면 → 현재 목표 하나만 표시
            if (data.isSequential && !isSelectableBranch)
            {
                int nextIndex = completion.FindIndex(b => !b);
                if (i != nextIndex) continue;
            }

            var obj = data.objectives[i];
            // mapPositions가 null이거나 비어있으면 스킵
            if (obj.mapPositions == null || obj.mapPositions.Length == 0)
                continue;

            //ShowQuestMarker(obj.mapPosition, obj.descriptionLocalized);
            // 배열 안의 모든 좌표에 대해 마커 표시
            foreach (var pos in obj.mapPositions)
            {
                ShowQuestMarker(pos, obj.descriptionLocalized);
            }
        }
    }

    public void ShowQuestMarker(Vector2 normalizedPosition, LocalizedString label)
    {
        GameObject marker = Instantiate(markerPrefab, markerParent);

        RectTransform rt = marker.GetComponent<RectTransform>();

        // 앵커는 부모 기준 좌측 상단 (혹은 원하는 기준점)
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0.5f, 0.5f);

        // anchoredPosition을 절대 위치로 사용
        rt.anchoredPosition = normalizedPosition;

        var labelComponent = marker.GetComponentInChildren<LocalizeStringEvent>();
        if (labelComponent != null)
            labelComponent.StringReference = label;

        currentMarkers.Add(marker);
    }

    public void ClearMarkers()
    {
        foreach (var m in currentMarkers)
            Destroy(m);
        currentMarkers.Clear();
    }

    public void ShowMyLocation()
    {
        if(SceneManager.GetActiveScene().name != "Proto")
        {
            myLocation.SetActive(false);
            return;
        }

        myLocation.SetActive(true);

        RectTransform rt = myLocation.GetComponent<RectTransform>();

        // 앵커는 부모 기준 좌측 상단 (혹은 원하는 기준점)
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0.5f, 0.5f);


        Vector3 myPosition = FindObjectOfType<Kupa.Player>().transform.position;
        rt.anchoredPosition = ConvertWorldToMap(myPosition);
    }
    Vector2 ConvertWorldToMap(Vector3 worldPos)
    {

        // 2. 맵 이미지 기준 (UI 공간)
        float mapWidth = 1500f;
        float mapHeight = 1125f;

        // 3. 클램핑: 월드 좌표가 범위 밖이면 보정
        float clampedX = Mathf.Clamp(worldPos.x, worldMinX, worldMaxX);
        float clampedZ = Mathf.Clamp(worldPos.z, worldMinZ, worldMaxZ);

        // 4. 정규화된 비율 계산
        float normalizedY = 1f - (clampedX - worldMinX) / (worldMaxX - worldMinX);
        float normalizedX = (clampedZ - worldMinZ) / (worldMaxZ - worldMinZ);

        // 5. 맵 이미지 좌표로 변환
        float mapX = normalizedX * mapWidth;
        float mapY = normalizedY * mapHeight;

        return new Vector2(mapX, mapY);
    }
}
