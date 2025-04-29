using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CSFramework
{
    [CustomEditor(typeof(MeshEditor))]
    public class MeshSceneHelper : UnityEditor.Editor
    {
        SpriteRenderer _selectedVertex;

        void OnSceneGUI ()
        {
            var e = Event.current;
            var mousePosition = e.mousePosition;
            var sceneView = SceneView.currentDrawingSceneView;
            var camera = sceneView.camera;
            var rect = sceneView.position;
            var viewPosition = new Vector3(mousePosition.x / rect.width, 1 - mousePosition.y / rect.height, 0);
            var worldPosition = camera.ViewportToWorldPoint(viewPosition);
            worldPosition.z = 0;
            
            var meshEditor = (target as MeshEditor);
            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    // Debug.LogError("down worldPosition: " + worldPosition);
                    _selectedVertex = meshEditor.NearVertex(worldPosition);
                    if (_selectedVertex)
                        e.Use();
                }
            }
            // else if (e.type == EventType.MouseMove)
            else if (e.type == EventType.MouseDrag)
            {
                if (_selectedVertex)
                {
                    // Debug.LogError("move worldPosition: " + worldPosition);
                    _selectedVertex.transform.position = worldPosition;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                if (_selectedVertex)
                {
                    // Debug.LogError("up worldPosition: " + worldPosition);
                    _selectedVertex = null;
                    e.Use();
                }
            }
        }
    }
}