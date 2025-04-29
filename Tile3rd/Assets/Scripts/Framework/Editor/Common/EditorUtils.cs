using UnityEngine;
using UnityEditor;
using System.IO;

namespace CSFramework.Editor
{
    public static class EditorUtils
    {
        public static void CreateAsset<T>(string path) where T : ScriptableObject
        {
            var folder = Path.GetDirectoryName(Application.dataPath + "/Resources/" + path);
            Utils.MakeSureDirectoryExist(folder);
            T instance = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(instance, "Assets/Resources/" + path + ".asset");
        }

        public static void CreateAssetWithFullPath<T>(string full_path) where T : ScriptableObject
        {
            var folder = Path.GetDirectoryName(full_path);
            Utils.MakeSureDirectoryExist(folder);
            T instance = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(instance, full_path + ".asset");
        }
        
        [MenuItem("GameObject/CSFrameworkUtils/CopyNodePath", priority = 10)]
        public static void CopyNodePath ()
        {
            var obj = Selection.activeGameObject;
            if (obj)
            {
                var prefab_stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                var prefab_root = prefab_stage == null ? null : prefab_stage.prefabContentsRoot.transform;
                var node_path = obj.name;
                var node = obj.transform.parent;
                while (node != null && node != prefab_root)
                {
                    node_path = string.Format("{0}/{1}", node.name, node_path);
                    node = node.transform.parent;
                }
                
                GUIUtility.systemCopyBuffer = node_path;
                CSFramework.Logger.Log("NodePath: " + node_path);
            }
        }

        [MenuItem("GameObject/CSFrameworkUtils/PrintLocalRotationInfo", priority = 10)]
        public static void PrintLocalRotationInfo ()
        {
            var obj = Selection.activeGameObject;
            if (obj)
            {
                CSFramework.Logger.Log("LocalRotation: " + obj.transform.localRotation);
                CSFramework.Logger.Log("LocalEulerAngles: " + obj.transform.localEulerAngles);
            }
        }
    }
}