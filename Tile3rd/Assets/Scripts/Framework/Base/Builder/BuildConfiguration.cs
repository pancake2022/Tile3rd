using UnityEngine;
using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class BuildConfiguration : ScriptableObject
    {
        [Header("Android Key Store")]
        public bool AndroidKeyStoreUseConfiguration = false;
        public string AndroidKeyStorePath = "";
        public string AndroidKeyStorePass = "";
        public string AndroidKeyStoreAlias = "";
        public string AndroidKeyStoreAliasPass = "";

        [Header("Facebook")]
        public int FB_SelectedAppIndex_Release = 0;
        public int FB_SelectedAppIndex_Debug = 0;
    }
}