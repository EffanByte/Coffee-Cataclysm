using UnityEngine;
using UnityEditor;
using System.IO;

public class FBXToPrefabConverter : EditorWindow
{
    private string sourceFolder = "Assets/Prefabs/CavePrefabs";
    private string outputFolder = "Assets/Prefabs/CavePrefabs/Converted";

    [MenuItem("Tools/Convert FBX to Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<FBXToPrefabConverter>("Convert FBX to Prefabs");
    }

    void OnGUI()
    {
        sourceFolder = EditorGUILayout.TextField("Source Folder", sourceFolder);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Convert All FBX to Prefabs"))
        {
            ConvertAllFBX();
        }
    }

    void ConvertAllFBX()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { sourceFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (model == null) continue;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            string filename = Path.GetFileNameWithoutExtension(path);
            string newPrefabPath = Path.Combine(outputFolder, filename + ".prefab");

            Directory.CreateDirectory(outputFolder); // ensure folder exists

            PrefabUtility.SaveAsPrefabAsset(instance, newPrefabPath);
            DestroyImmediate(instance);
        }

        AssetDatabase.Refresh();
        Debug.Log("Conversion complete.");
    }
}
