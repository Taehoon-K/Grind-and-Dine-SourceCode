#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ExtractMesh : MonoBehaviour
{
    private Mesh mesh;

    [ContextMenu("ExtractMesh")]
    private void ExtractMeshFrom3DModel()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found!");
            return;
        }

        mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogError("No mesh found in MeshFilter component!");
            return;
        }

        string path = $"Assets/Meshes/{gameObject.name}.asset";
        CreateDirectoryIfNotExists(path);

        if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null)
        {
            path = AssetDatabase.GenerateUniqueAssetPath(path);
        }

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }

    private void CreateDirectoryIfNotExists(string path)
    {
        string directoryPath = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }
    }
}
#endif
