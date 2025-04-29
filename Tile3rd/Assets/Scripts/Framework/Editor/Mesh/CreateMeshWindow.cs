using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace CSFramework
{
    public class CreateMeshWindow : EditorWindow
    {
        [MenuItem("CSFramework/Mesh/OpenCreateMeshWindow")]
        static void Open ()
        {
            CreateMeshWindow window = EditorWindow.GetWindow(typeof(CreateMeshWindow), false, "Create Mesh") as CreateMeshWindow;
        }

        // private 
        private string _saveDir = "ExtraResources/Mesh";
        private string _saveName = "Mesh_1";
        private int _meshWidth = 10;
        private int _meshHeight = 10;
        private float _meshGridWidth = 1;
        private float _meshGridHeight = 1;
        private bool _random_color;
        private Material _material = null;
        private Sprite _sprite = null;

        private void OnGUI() 
        {
            _saveDir = EditorGUILayout.TextField("Save Path", _saveDir);
            _saveName = EditorGUILayout.TextField("Mesh Name", _saveName);
            _meshWidth = EditorGUILayout.IntSlider("Mesh Width", _meshWidth, 1, 200);
            _meshHeight = EditorGUILayout.IntSlider("Mesh Height", _meshHeight, 1, 200);
            _meshGridWidth = EditorGUILayout.Slider("Mesh Grid Width", _meshGridWidth, 0.1f, 10f);
            _meshGridHeight = EditorGUILayout.Slider("Mesh Grid Height", _meshGridHeight, 0.1f, 10f);
            _material = EditorGUILayout.ObjectField("Material", _material, typeof(Material), false) as Material;
            _sprite = EditorGUILayout.ObjectField("Sprite", _sprite, typeof(Sprite), false) as Sprite;
            _random_color = true;

            if (GUILayout.Button("Create Mesh"))
            {
                if (!string.IsNullOrEmpty(_saveName))
                {
                    CreateMesh(_saveDir, _saveName, _meshWidth, _meshHeight, _meshGridWidth, _meshGridHeight, _random_color);
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

        private void CreateMesh (string dir, string name, int width, int height, float gridWidth, float gridHeight, bool random_color)
        {
            var path = string.Format("Assets/{0}/{1}.asset", dir, name);

            var mesh = new Mesh();

            var verticle_list = new List<Vector3>();
            var uv_list = new List<Vector2>();
            var triangle_list = new List<int>();
            var color_list = new List<Color>();

            var index = 0;
            var dict = new Dictionary<Vector2, int>();
            for (var x = 0; x <= width; ++x)
            {
                for (var y = 0; y <= height; ++y)
                {
                    var verticle = new Vector3(x * gridWidth, y * gridHeight, 0);
                    var uv = new Vector2(x / (float)width, y / (float)height);
                    verticle_list.Add(verticle);
                    uv_list.Add(uv);
                    dict[new Vector2(x, y)] = index;
                    ++index;

                    if (random_color)
                        color_list.Add(new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
                }
            }

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var i_0 = dict[new Vector2(x, y)];
                    var i_1 = dict[new Vector2(x, y + 1)];
                    var i_2 = dict[new Vector2(x + 1, y + 1)];
                    var i_3 = dict[new Vector2(x + 1, y)];
                    // leftdown
                    triangle_list.Add(i_0);
                    triangle_list.Add(i_1);
                    triangle_list.Add(i_2);
                    // rightup
                    triangle_list.Add(i_1);
                    triangle_list.Add(i_3);
                    triangle_list.Add(i_2);
                }
            }

            mesh.vertices = verticle_list.ToArray();
            mesh.uv = uv_list.ToArray();
            mesh.triangles = triangle_list.ToArray();
            if (random_color)
                mesh.colors = color_list.ToArray();

            GameObject meshEditorObject = new GameObject(string.Format("MeshEditor_{0}", name));
            var meshFilter =  meshEditorObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            var meshRenderer = meshEditorObject.AddComponent<MeshRenderer>();
            meshRenderer.material = _material;
            var editor = meshEditorObject.AddComponent<MeshEditor>();
            editor.sprite = _sprite;

            var path_dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(path_dir))
                Directory.CreateDirectory(path_dir);

            AssetDatabase.CreateAsset(mesh, path);
        }
    }
}