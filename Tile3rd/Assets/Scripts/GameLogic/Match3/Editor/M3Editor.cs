using UnityEngine;
using UnityEditor;

public class M3Editor: UnityEditor.Editor
{
    [MenuItem("M3/LaunchGame")]
    public static void LaunchGame ()
    {
        EditorApplication.isPlaying = false;
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/LaunchScene.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("M3/LaunchEditor")]
    public static void LaunchEditor()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.playModeStateChanged += open_editor_scene;
            EditorApplication.isPlaying = false;
        }
        else
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/EditorScene.unity");
            EditorApplication.isPlaying = true;
        }
    }

    [MenuItem("M3/清除存档")]
    public static void ClearStorage ()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("存档清除成功");
    }

    private static void open_editor_scene (PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/EditorScene.unity");
            EditorApplication.playModeStateChanged -= open_editor_scene;
            EditorApplication.isPlaying = true;
        }
    }

    // [MenuItem("M3/Editor/ShowEditorWindow")]
    // public static void ShowEditorWindow()
    // {
    //     EditorWindow.GetWindow<M3EditorWindow>("M3编辑器", true).Show();
    // }
}