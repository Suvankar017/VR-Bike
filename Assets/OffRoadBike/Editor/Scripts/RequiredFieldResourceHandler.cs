using System.Linq;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class RequiredFieldResourceHandler
{
    public static Texture2D RequiredIcon => s_RequiredIcon;

    private static bool s_IsInitialized = false;
    private static Texture2D s_RequiredIcon = null;

    private const string c_IconsFolderRelativePath = "Icons";
    private const string c_RequiredIconNameWithExtention = "RequiredIcon.png";

    static RequiredFieldResourceHandler()
    {
        Init();
    }

    private static void Init()
    {
        if (s_IsInitialized)
            return;

        s_IsInitialized = true;

        string editorFolderRelativePath = GetEditorFolderRelativePath();
        if (string.IsNullOrEmpty(editorFolderRelativePath))
        {
            s_IsInitialized = false;
            Debug.LogError("Editor folder not found for RequiredFieldAttribute resource.");
            return;
        }

        string requiredIconRelativePathFromAssetsFolder = $"{editorFolderRelativePath}/{c_IconsFolderRelativePath}/{c_RequiredIconNameWithExtention}";
        s_RequiredIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(requiredIconRelativePathFromAssetsFolder);

        if (s_RequiredIcon == null)
        {
            Debug.LogError($"{requiredIconRelativePathFromAssetsFolder} not found.");
            s_IsInitialized = false;
        }
    }

    private static string GetEditorFolderRelativePath()
    {
        MonoScript[] scripts = AssetDatabase.FindAssets("t:MonoScript").Select(guid => AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();

        foreach (MonoScript script in scripts)
        {
            if (script.GetClass() == typeof(RequiredFieldPropertyDrawer))
            {
                string path = AssetDatabase.GetAssetPath(script);
                return TrimPathToFolder(path, "Editor");
            }
        }

        return string.Empty;
    }

    private static string TrimPathToFolder(string path, string folderName)
    {
        int index = path.Length;
        string[] foldersName = path.Split('/');

        for (int i = foldersName.Length - 1; i >= 0; i--)
        {
            index -= foldersName[i].Length;
            if (string.Compare(foldersName[i], folderName) == 0)
                break;
            index -= 1;
        }

        return (index < 0) ? path : path.Substring(0, index + folderName.Length);
    }

}