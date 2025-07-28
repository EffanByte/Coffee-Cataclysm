using UnityEngine;
using UnityEditor;
using System.IO;

public class BatchScalePrefabs : EditorWindow
{
    private string folderPath = "Assets/Prefabs/CavePrefabs"; // Change this
    private Vector3 newScale = Vector3.one;

    [MenuItem("Tools/Batch Scale Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<BatchScalePrefabs>("Batch Scale Prefabs");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Folder Path (Relative to Assets/):", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField(folderPath);
        newScale = EditorGUILayout.Vector3Field("New Scale:", newScale);

        if (GUILayout.Button("Apply Scale to All Prefabs"))
        {
            ApplyScaleToPrefabs();
        }
    }

    void ApplyScaleToPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        Debug.Log($"Found {guids.Length} prefabs in {folderPath}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.localScale = newScale;

            PrefabUtility.SaveAsPrefabAsset(instance, path);
            DestroyImmediate(instance);
        }

        Debug.Log("Scaling complete.");
    }
}
