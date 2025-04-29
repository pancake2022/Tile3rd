using System;
using Newtonsoft.Json;

namespace CSFramework
{
    [System.Serializable]
    public class IntEncrypt
    {
        [JsonProperty]
        private float _v0;
        [JsonProperty]
        private int _v;

        public void SetValue (int value)
        {
            if (GetValue() != value)
            {
                if (value > 0)
                {
                    _v = (int)Math.Floor(UnityEngine.Random.Range(0.0f, 1.0f) * value);
                    _v0 = (value - _v) / 16.0f;
                }
                else
                {
                    _v0 = 0.0f;
                    _v = 0;
                }
                Framework.SetStorageDirty(true);
            }
        }

        public int GetValue ()
        {
            return (int)Math.Round(16.0f * _v0 + _v);
        }
    }
}