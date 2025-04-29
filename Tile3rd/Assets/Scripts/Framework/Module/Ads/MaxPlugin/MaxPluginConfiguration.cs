using UnityEngine;
using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class MaxPluginConfiguration : ScriptableObject
    {
        public string SDKKey;
        public List<AdsConfigItem> ConfigItemList;
    }
}