using UnityEngine;
using UnityEngine.UI;
using System;

namespace CSFramework
{
    public class BaseEffectUI : BaseUI
    {
        public void BindWindow (WindowUI window_ui)
        {
            refresh_child_node_order(window_ui.SortingOrder, true);
        }

        public void AboveLayer (UILayer ui_layer)
        {
            var ui_layer_names = Enum.GetNames(typeof(UILayer));
            var current_index = ui_layer_names.IndexOf(ui_layer.ToString());
            var next_index = current_index + 1;
            if (next_index < ui_layer_names.Length)
            {
                var next_layer = (UILayer)Enum.Parse(typeof(UILayer), ui_layer_names[next_index]);
                refresh_child_node_order(_ui_manager.GetWindowLayerSortingOrder(next_layer), true);
            }
            else
            {
                refresh_child_node_order(32767, true);
            }
        }
    }
}