using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshMaterialCombiner : EditorWindow
{
    [MenuItem("Tools/ Merge Materials & Submeshes")]
    static void CombineMaterialsAndSubmeshes()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null || selected.GetComponent<MeshRenderer>() == null || selected.GetComponent<MeshFilter>() == null)
        {
            Debug.LogError(" �޽� ������Ʈ�� ���� �����ϼ���.");
            return;
        }

        MeshFilter mf = selected.GetComponent<MeshFilter>();
        MeshRenderer mr = selected.GetComponent<MeshRenderer>();
        Mesh originalMesh = mf.sharedMesh;

        Material[] originalMaterials = mr.sharedMaterials;
        int subMeshCount = originalMesh.subMeshCount;

        //  �ؽ�ó ������
        List<Texture2D> textures = new List<Texture2D>();
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            Texture2D tex = originalMaterials[i]?.mainTexture as Texture2D;
            if (tex == null)
            {
                tex = CreateSolidColorTexture(originalMaterials[i]?.color ?? Color.gray);
            }
            textures.Add(MakeTextureReadable(tex));
        }

        //  Texture Atlas �����
        Texture2D atlas = new Texture2D(4096, 4096);
        Rect[] uvRects = atlas.PackTextures(textures.ToArray(), 2, 4096);
        atlas.Apply();

        //  �� ��Ƽ���� ����
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.mainTexture = atlas;

        //  ���ο� UVs ����
        Vector2[] oldUVs = originalMesh.uv;
        Vector2[] newUVs = new Vector2[oldUVs.Length];
        List<int> allTriangles = new List<int>();

        for (int sub = 0; sub < subMeshCount; sub++)
        {
            int[] tris = originalMesh.GetTriangles(sub);
            Rect uvRect = uvRects[sub];

            for (int i = 0; i < tris.Length; i++)
            {
                int vertIndex = tris[i];

                // ���� UV  ��Ʋ�� �������� ����
                Vector2 uv = oldUVs[vertIndex];
                uv.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, uv.x);
                uv.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, uv.y);
                newUVs[vertIndex] = uv;

                allTriangles.Add(vertIndex);
            }
        }

        //  �� �޽� ����
        Mesh newMesh = new Mesh();
        newMesh.name = originalMesh.name + "_Combined";
        newMesh.vertices = originalMesh.vertices;
        newMesh.normals = originalMesh.normals;
        newMesh.tangents = originalMesh.tangents;
        newMesh.uv = newUVs;
        newMesh.triangles = allTriangles.ToArray();

        //  �� ������Ʈ ����
        GameObject newObj = Instantiate(selected);
        newObj.name = selected.name + "_Combined";
        newObj.GetComponent<MeshFilter>().sharedMesh = newMesh;
        newObj.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { newMat };

        Debug.Log("��Ƽ���� + SubMesh ���� �Ϸ�: " + newObj.name);
    }

    //  �ؽ�ó �б� �����ϵ��� ����
    static Texture2D MakeTextureReadable(Texture2D tex)
    {
        string path = AssetDatabase.GetAssetPath(tex);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    //  �ܻ� �ؽ�ó ����
    static Texture2D CreateSolidColorTexture(Color color)
    {
        Texture2D tex = new Texture2D(2, 2);
        Color[] colors = new Color[4] { color, color, color, color };
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }
}
