using UnityEngine;
using UnityEditor;
using System.IO;

public class EnableLightmapUVs
{
    [MenuItem("Tools/Enable Lightmap UVs for All Models")]
    public static void EnableLightmapUVsForAllModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model"); // 모든 모델(FBX) 찾기
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null && !importer.generateSecondaryUV)
            {
                importer.generateSecondaryUV = true; // 라이트맵 UV 활성화
                EditorUtility.SetDirty(importer);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($" {count}개의 모델에 라이트맵 UV 자동 생성 옵션을 활성화했습니다!");
    }
}
