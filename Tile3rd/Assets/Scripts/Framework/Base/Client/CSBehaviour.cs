using UnityEngine;
using System;

namespace CSFramework
{
    public class CSBehaviour : MonoBehaviour 
    {
        public T Create<T> () where T: CSBehaviour
        {
            return gameObject.AddComponent<T>();
        }

        protected void log (object obj, params object[] args)
        {
            var str = GetType().ToString() + " " + (obj == null ? "NULL" : obj.ToString());
            Logger.Log(str, args);
        }
        protected void warning (object obj, params object[] args)
        {
            var str = GetType().ToString() + " " + (obj == null ? "NULL" : obj.ToString());
            Logger.Warning(str, args);
        }
        protected void error (object obj, params object[] args)
        {
            var str = GetType().ToString() + " " + (obj == null ? "NULL" : obj.ToString());
            Logger.Error(str, args);
        }
    }
}