using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace CSFramework
{
    public class ConvertObjWindow : EditorWindow
    {
        [MenuItem("CSFramework/Mesh/OpenConvertObjWindow")]
        static void Open ()
        {
            ConvertObjWindow window = EditorWindow.GetWindow(typeof(ConvertObjWindow), false, "Convert Obj") as ConvertObjWindow;
        }

        // private 
        private string _saveDir = "ExtraResources/Mesh";
        private string _saveName = "Obj_Mesh_1";
        private TextAsset _vertex_asset;
        private TextAsset _color_asset;

        private void OnGUI() 
        {
            _saveDir = EditorGUILayout.TextField("Save Path", _saveDir);
            _saveName = EditorGUILayout.TextField("Mesh Name", _saveName);
            _vertex_asset = EditorGUILayout.ObjectField("vertex", _vertex_asset, typeof(TextAsset), false) as TextAsset;
            _color_asset = EditorGUILayout.ObjectField("color", _color_asset, typeof(TextAsset), false) as TextAsset;

            if (GUILayout.Button("Create Mesh"))
            {
                if (!string.IsNullOrEmpty(_saveName))
                {
                    CreateMesh(_saveDir, _saveName, _vertex_asset, _color_asset);
                    if (int.TryParse(_saveName[_saveName.Length - 1].ToString(), out var index))
                    {
                        _saveName = _saveName.Substring(0, _saveName.Length - 1) + (index + 1).ToString();
                    }
                    else
                    {
                        _saveName = _saveName + 1;
                    }
                }
            }
        }

        private void CreateMesh (string dir, string name, TextAsset vertex_asset, TextAsset color_asset)
        {
            var path = string.Format("Assets/{0}/{1}.asset", dir, name);

            var mesh = new Mesh();

            var verticle_list = new List<Vector3>();
            var uv_list = new List<Vector2>();
            var triangle_list = new List<int>();
            var color_list = new List<Color>();

            var vertext_line_list = vertex_asset.text.Split('\n');
            foreach (var vertex_line in vertext_line_list)
            {
                var data = vertex_line.Split(' ');
                if (data.Length > 0)
                {
                    if (data[0] == "v")
                    {
                        verticle_list.Add(new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3])));
                    }
                    else if (data[0] == "f")
                    {
                        triangle_list.Add(int.Parse(data[1]) - 1);
                        triangle_list.Add(int.Parse(data[2]) - 1);
                        triangle_list.Add(int.Parse(data[3]) - 1);
                    }
                }
                else
                {
                    break;
                }
            }

            var color_line_list = color_asset.text.Split('\n');
            foreach (var color_line in color_line_list)
            {
                var data = color_line.Split(' ');
                if (data.Length > 0)
                {
                    if (data[0] == "v")
                    {
                        color_list.Add(new Color(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]), float.Parse(data[4])));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            mesh.vertices = verticle_list.ToArray();
            // mesh.uv = uv_list.ToArray();
            mesh.triangles = triangle_list.ToArray();
            mesh.colors = color_list.ToArray();

            // GameObject meshEditorObject = new GameObject(string.Format("MeshEditor_{0}", name));
            // var meshFilter =  meshEditorObject.AddComponent<MeshFilter>();
            // meshFilter.mesh = mesh;
            // var meshRenderer = meshEditorObject.AddComponent<MeshRenderer>();
            // meshRenderer.material = _material;
            // var editor = meshEditorObject.AddComponent<MeshEditor>();
            // editor.sprite = _sprite;

            var path_dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(path_dir))
                Directory.CreateDirectory(path_dir);

            AssetDatabase.CreateAsset(mesh, path);
        }
    }
}