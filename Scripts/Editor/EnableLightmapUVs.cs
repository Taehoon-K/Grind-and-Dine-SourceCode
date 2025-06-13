using UnityEngine;
using UnityEditor;
using System.IO;

public class EnableLightmapUVs
{
    [MenuItem("Tools/Enable Lightmap UVs for All Models")]
    public static void EnableLightmapUVsForAllModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model"); // ��� ��(FBX) ã��
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null && !importer.generateSecondaryUV)
            {
                importer.generateSecondaryUV = true; // ����Ʈ�� UV Ȱ��ȭ
                EditorUtility.SetDirty(importer);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($" {count}���� �𵨿� ����Ʈ�� UV �ڵ� ���� �ɼ��� Ȱ��ȭ�߽��ϴ�!");
    }
}
