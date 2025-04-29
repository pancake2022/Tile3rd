// using UnityEngine;
// using UnityEditor;
// using CSFramework;
// using System;
// using System.IO;
// using System.Collections.Generic;
// using Newtonsoft.Json;

// public class M3PanelWrapper
// {
//     public M3Panel M3Panel;
//     public int TempID;
//     public bool IsDirty => TempID != M3Panel.ID;

//     public M3PanelWrapper (M3Panel m3_panel)
//     {
//         M3Panel = m3_panel;
//         TempID = m3_panel.ID;
//     }
// }

// public class M3EditorWindow : EditorWindow
// {
//     public static readonly GUILayoutOption LayoutOption_PanelManager = GUILayout.Width(180);
//     public static readonly GUILayoutOption LayoutOption_LayerManager = GUILayout.Width(260);
//     public static readonly GUILayoutOption[] LayoutOptions_Panel = new GUILayoutOption[] { GUILayout.Width(600), GUILayout.Height(600) };

//     private List<M3PanelWrapper> _panel_list;
//     private int _selected_panel_index;
//     private M3PanelWrapper _selected_panel => _selected_panel_index >= 0 && _selected_panel_index < _panel_list.Count ? _panel_list[_selected_panel_index] : null;
//     private int _max_panel_id;
//     private Vector2 _panel_scollview_position;

//     private Texture2D _render_texture;
//     private Texture2D _cell_texture;

//     private void load_panel ()
//     {
//         _panel_list = new List<M3PanelWrapper>();
//         _selected_panel_index = 0;
//         _max_panel_id = 0;
//         _panel_scollview_position = Vector2.zero;

//         var file_list = Utils.GetExtraResourcesList(M3Const.M3PanelConfigPath, include_sub_dir: false);
//         foreach (var file in file_list)
//         {
//             if (file.Extension.ToLower() == ".json")
//             {
//                 M3Panel m3_panel = null;
                
//                 try
//                 {
//                     using (var sr = file.OpenText())
//                     {
//                         var str = sr.ReadToEnd();
//                         m3_panel = JsonUtility.FromJson<M3Panel>(str);
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     CSFramework.Logger.Error(e);
//                 }

//                 if (m3_panel != null)
//                 {
//                     if (m3_panel.ID > _max_panel_id)
//                         _max_panel_id = m3_panel.ID;
//                     _panel_list.Add(new M3PanelWrapper(m3_panel));
//                 }
//             }
//         }
//     }

//     private void load_res ()
//     {
//         _render_texture = new Texture2D(600, 600);

//         _cell_texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/RawResources/EditorRes/cellbg.png");
//         var pixels = _cell_texture.GetPixels();
//         _render_texture.SetPixels(100, 100, _cell_texture.width, _cell_texture.height, pixels);
//     }

//     private void save_panel ()
//     {
//         foreach (var panel in _panel_list)
//         {
//             var file_path = Utils.GetEditorExtraResourcesPath($"{M3Const.M3PanelConfigPath}/{panel.M3Panel.ID}.json");
//             Utils.SaveFile(file_path, JsonUtility.ToJson(panel.M3Panel), true);
//         }
//         AssetDatabase.Refresh();
//     }

//     private void OnEnable ()
//     {
//         load_panel();
//         load_res();
//     }

//     private void OnDisable ()
//     {
//         // CSFramework.Logger.Error("OnDisable");
//     }

//     private void OnGUI() 
//     {
//         using (var horizontal = new EditorGUILayout.HorizontalScope())
//         {
//             draw_panel_manager();
//             draw_layer_manager();
//             draw_panel(horizontal.rect);
//             draw_panel_controller();
//         }
//     }

//     private void draw_panel_manager ()
//     {
//         using (var vertical = new EditorGUILayout.VerticalScope(LayoutOption_PanelManager))
//         {
//             GUILayout.Label($"布局数量: {_panel_list.Count}");

//             if (GUILayout.Button("新建布局"))
//             {
//                 _selected_panel_index = _panel_list.Count;
//                 _panel_list.Add(new M3PanelWrapper(new M3Panel
//                 {
//                     ID = ++_max_panel_id,
//                     LayerList = new List<M3Layer>(),
//                     Collection = new M3Collection(),
//                 })); // todo
//             }

//             using (var scollview = new EditorGUILayout.ScrollViewScope(_panel_scollview_position, new GUILayoutOption[] { GUILayout.MaxHeight(300), LayoutOption_PanelManager}))
//             {
//                 _panel_scollview_position = scollview.scrollPosition;

//                 for (var i = 0; i < _panel_list.Count; ++i)
//                 {
//                     var panel = _panel_list[i];
//                     var str_dirty = panel.IsDirty ? " *" : "";
//                     if (EditorGUILayout.ToggleLeft($"{panel.M3Panel.ID}{str_dirty}", i == _selected_panel_index, GUILayout.Width(160)))
//                         _selected_panel_index = i;
//                 }
//             }

//             using (var horizontal = new EditorGUILayout.HorizontalScope())
//             {
//                 var page_index = EditorGUILayout.IntField(0);
//                 if (GUILayout.Button("跳转"))
//                 {
//                     CSFramework.Logger.Log("跳转");
//                 }
//             }

//             using (var horizontal = new EditorGUILayout.HorizontalScope())
//             {
//                 if (GUILayout.Button("<"))
//                 {
//                     CSFramework.Logger.Log("<");
//                 }
//                 if (GUILayout.Button(">"))
//                 {
//                     CSFramework.Logger.Log(">");
//                 }
//             }
//         }
//     }

//     private void draw_layer_manager ()
//     {
//         using (var vertical = new EditorGUILayout.VerticalScope(LayoutOption_LayerManager))
//         {
//             var selected_panel = _selected_panel;
//             if (selected_panel == null)
//             {
//                 EditorGUILayout.LabelField($"请先选择需要编辑的布局");
//             }
//             else
//             {
//                 selected_panel.TempID = EditorGUILayout.IntField("布局ID: ", selected_panel.TempID);
//                 var panel_id_exist = false;
//                 foreach (var panel in _panel_list)
//                 {
//                     if (panel != selected_panel && panel.M3Panel.ID == selected_panel.TempID)
//                     {
//                         panel_id_exist = true;
//                         break;
//                     }
//                 }

//                 if (panel_id_exist)
//                 {
//                     var label_style = new GUIStyle();
//                     label_style.normal.textColor = Color.red;
//                     EditorGUILayout.LabelField("布局ID不能重复", label_style);
//                     // EditorGUILayout.LabelField("布局ID不能重复");
//                 }
//                 else
//                 {
//                     selected_panel.M3Panel.ID = selected_panel.TempID;

//                     using (var h = new EditorGUILayout.HorizontalScope())
//                     {
//                         if (GUILayout.Button("保存"))
//                         {
//                             save_panel();
//                         }
//                         if (GUILayout.Button("删除"))
//                         {
//                             _panel_list.RemoveAt(_selected_panel_index);
//                         }
//                     }   
//                 }
//             }
//         }
//     }

//     private void draw_panel (Rect rect)
//     {
//         // RenderTexture

//         // var texture_rect = new Rect(0, 0, 600, 600);
//         // texture_rect.position = rect.position;

//         // var mouse_pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
//         // // rect.position = mouse_pos;
//         // if (rect.Contains(mouse_pos))
//         // {
//         //     // _render_texture.SetPixels(new Color32[] { Color32.white });
//         //     CSFramework.Logger.Log("In");
//         // }
//         // else
//         // {
//         //     CSFramework.Logger.Log("Out");
//         // }

//         // // EditorGUI.DrawPreviewTexture(rect, _texture_dict[0]);
//         // EditorGUI.DrawPreviewTexture(texture_rect, _render_texture);

//         using (var scope = new EditorGUILayout.VerticalScope(LayoutOptions_Panel))
//         {
//             var texture_rect = new Rect(0, 0, 600, 600);
//             texture_rect.position = scope.rect.position;

//             var mouse_pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
//             // rect.position = mouse_pos;
//             var texture_global_rect = new Rect(texture_rect.position + this.position.position, texture_rect.size);
//             if (texture_global_rect.Contains(mouse_pos))
//             {
//                 // _render_texture.GetPixels()
//                 // _render_texture.SetPixels()
//                 // _render_texture.SetPixels(new Color32[] { Color32.white });
//                 // CSFramework.Logger.Log($"{mouse_pos} || {texture_global_rect}  In");
//             }
//             else
//             {
//                 // CSFramework.Logger.Log($"{mouse_pos} || {texture_global_rect}  Out-----------");
//             }

//             // EditorGUI.DrawPreviewTexture(rect, _texture_dict[0]);
//             EditorGUI.DrawPreviewTexture(texture_rect, _render_texture);
//         }

//         // using (var scope = new EditorGUILayout.ScrollViewScope(Vector2.zero, LayoutOptions_Panel))
//         // {
//         //     var rect = new Rect(0, 0, 200, 200);

//         //     var mouse_pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
//         //     rect.position = mouse_pos;

//         //     EditorGUI.DrawPreviewTexture(rect, _texture_dict[0]);
//         //     EditorGUILayout.LabelField("Draw Panel");

//         // }
//     }

//     private void draw_panel_controller ()
//     {

//     }

//     private void Update() {
//         // Repaint();
//     }
// }