using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSFramework
{
    public abstract class Subsystem : Module<SubsystemContainer>
    {
        public Framework Framework => _main_module.Framework;
    }
}