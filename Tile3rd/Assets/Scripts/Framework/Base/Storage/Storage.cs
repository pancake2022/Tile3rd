using System;
using UnityEngine;

namespace CSFramework
{
    [Serializable]
    public abstract class Storage
    {
        protected void dirty (bool force_save = false)
        {
            Framework.SetStorageDirty(force_save);
        }
    }
}