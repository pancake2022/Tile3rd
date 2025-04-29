using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace CSFramework
{
    public class CreateSkewMeshWindow : EditorWindow
    {
        [MenuItem("CSFramework/Mesh/OpenCreateSkewMeshWindow")]
        static void Open ()
        {
            CreateSkewMeshWindow window = EditorWindow.GetWindow(typeof(CreateSkewMeshWindow), false, "Create Shadow") as CreateSkewMeshWindow;
        }

        // private 
        private string _saveDir = "ExtraResources/Mesh";
        private string _saveName = "SkewMesh_1";
        private int _meshWidth = 10;
        private int _meshHeight = 10;
        private float _meshGridSize = 1;
        private float _meshSkewX = 0;
        private float _meshSkewY = 0;
        private float _meshScaleX = 1;
        private float _meshScaleY = 1;

        private void OnGUI() 
        {
            _saveDir = EditorGUILayout.TextField("Save Path", _saveDir);
            _saveName = EditorGUILayout.TextField("Mesh Name", _saveName);
            _meshWidth = EditorGUILayout.IntSlider("Mesh Width", _meshWidth, 1, 200);
            _meshHeight = EditorGUILayout.IntSlider("Mesh Height", _meshHeight, 1, 200);
            _meshGridSize = EditorGUILayout.Slider("Mesh Grid Size", _meshGridSize, 0.1f, 10f);
            _meshSkewX = EditorGUILayout.Slider("Mesh Skew X", _meshSkewX, -1, 1);
            _meshSkewY = EditorGUILayout.Slider("Mesh Skew Y", _meshSkewY, -1, 1);
            _meshScaleX = EditorGUILayout.Slider("Mesh Scale X", _meshScaleX, 0, 10);
            _meshScaleY = EditorGUILayout.Slider("Mesh Scale Y", _meshScaleY, 0, 10);

            if (GUILayout.Button("Create Shadow"))
            {
                if (!string.IsNullOrEmpty(_saveName))
                {
                    CreateMesh(_saveDir, _saveName, _meshWidth, _meshHeight, _meshGridSize, _meshSkewX, _meshSkewY, _meshScaleX, _meshScaleY);
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

        private void CreateMesh (string dir, string name, int width, int height, float gridSize, float skewX, float skewY, float scaleX, float scaleY)
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
                    // var verticle = new Vector3(x * gridWidth, y * gridHeight, 0);
                    var verticle = new Vector3(
                        (x + (y * (1 + skewX))) * gridSize * scaleX,
                        (y - (x * (1 + skewY))) * gridSize * scaleY
                    ); // 改成斜四边形
                    var uv = new Vector2(x / (float)width, y / (float)height);
                    verticle_list.Add(verticle);
                    uv_list.Add(uv);
                    color_list.Add(new Color(0.5058824f, 0.1960784f, 0.5019608f, 1));
                    dict[new Vector2(x, y)] = index;
                    ++index;
                }
            }

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var i_1 = dict[new Vector2(x, y)];
                    var i_3 = dict[new Vector2(x, y + 1)];
                    var i_2 = dict[new Vector2(x + 1, y + 1)];
                    var i_0 = dict[new Vector2(x + 1, y)];
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
            mesh.colors = color_list.ToArray();

            // GameObject meshEditorObject = new GameObject(string.Format("AreaShadow_{0}", name));
            // var meshFilter =  meshEditorObject.AddComponent<MeshFilter>();
            // meshFilter.mesh = mesh;
            // var meshRenderer = meshEditorObject.AddComponent<MeshRenderer>();
            // meshRenderer.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Export/MapArea/Hidden_DarkAreaShader.mat");
            // meshEditorObject.AddComponent<MeshEditor>();

            var parentDir = Path.GetDirectoryName(path);
            if (!Directory.Exists(parentDir))
                Directory.CreateDirectory(parentDir);

            AssetDatabase.CreateAsset(mesh, path);
        }
    }
}