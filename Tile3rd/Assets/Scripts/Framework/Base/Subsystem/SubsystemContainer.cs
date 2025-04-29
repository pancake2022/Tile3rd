using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSFramework
{
    public class SubsystemContainer : Module<Framework>
    {
        public Framework Framework { get { return _main_module; } }

        public T Subsystem<T> () where T: Subsystem
        {
            return submodule<T>();
        }

        public bool TryGetSubsystem<T> (out T subsystem) where T: Subsystem
        {
            subsystem = submodule<T>();
            return subsystem != null;
        }

        public IEnumerator RegisterSubsystem<T> (params object[] param_list) where T: Subsystem
        {
            yield return register_submodule<T>(param_list);
        }
        public IEnumerator DeregisterSubsystem<T> () where T: Subsystem
        {
            yield return deregister_submodule<T>();
        }
    }
}