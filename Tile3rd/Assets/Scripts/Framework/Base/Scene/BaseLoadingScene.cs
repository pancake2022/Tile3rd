using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSFramework
{
    public class BaseLoadingScene : BaseScene
    {
        public float LoadingProgress { get; protected set; }

        public IEnumerator LoadingProcess ()
        {
            yield return on_loading_process();
        }

        protected virtual IEnumerator on_loading_process ()
        {
            return null;
        }
    }
}