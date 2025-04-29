using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework.Editor
{
    public static class PluginEditor
    {
        [MenuItem("CSFramework/Plugin/CreateMaxPluginConfiguration")]
        public static void CreateMaxPluginConfiguration ()
        {
            EditorUtils.CreateAsset<MaxPluginConfiguration>(Environment.MaxPluginConfigurationPath);
        }

        [MenuItem("CSFramework/Plugin/CreateAdjustPluginConfiguration")]
        public static void CreateAdjustPluginConfiguration ()
        {
            EditorUtils.CreateAsset<AdjustPluginConfiguration>(Environment.AdjustPluginConfigurationPath);
        }
    }
}