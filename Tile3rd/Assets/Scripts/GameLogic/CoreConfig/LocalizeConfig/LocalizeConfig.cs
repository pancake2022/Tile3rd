using CSFramework;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    [Serializable]
    public class LocalizeConfig
    {
        public String Key { get; set; }
        public String Value { get; set; }
    }
}