using UnityEngine;
using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class AdjustPluginConfiguration : ScriptableObject
    {
        public string AppToken;
        public List<AdjustEventConfig> EventConfigList;
    }

    [System.Serializable]
    public class AdjustEventConfig
    {
        public string Key;
        public string Token;
    }
}