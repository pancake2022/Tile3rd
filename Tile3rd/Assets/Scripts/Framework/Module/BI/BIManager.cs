using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    public class BIContext
    {
        public string FirebaseUserID;
        public string PurchaseEventName;
        public string LoginEventName;
    }

    public class BIManager : Subsystem
    {
        public BIContext Context { get; private set; }
        // public AdjustManager AdjustManager => _adjust_manager;
        // public FirebaseManager FirebaseManager => _firebase_manager;

        public Action<BI> OnAddBI;
        
        private List<BI> _bi_list;
        private object _bi_lock;
        // private AdjustManager _adjust_manager;
        // private FirebaseManager _firebase_manager;

        protected override IEnumerator on_init (params object[] param_list)
        {
            if (param_list.Length > 0)
                Context = param_list[0] as BIContext;

            if (Context == null)
            {
                CSFramework.Logger.Error("BIManager Context Was Null");
                Context = new BIContext();
            }

            _bi_list = new List<BI>();
            _bi_lock = new object();

            // yield return register_submodule<AdjustManager>();
            // _adjust_manager = submodule<AdjustManager>();

            // yield return register_submodule<FirebaseManager>();
            // _firebase_manager = submodule<FirebaseManager>();

            yield return null;
        }

        public void SetFirebaseUserID (string user_id)
        {
            Context.FirebaseUserID = user_id;
            // if (_firebase_manager)
            //     _firebase_manager.SetUserID(user_id);
        }

        public void AddTrackEvent (string event_name, Dictionary<string, string> info_dict = null)
        {
            // _adjust_manager.AddTrackEvent(event_name, info_dict);
            // if (_firebase_manager)
            //     _firebase_manager.AddTrackEvent(event_name, info_dict);
        }

        public void AddPurchaseTrackEvent (double price, string currency, string transaction_id, bool is_test, Dictionary<string, string> info_dict = null)
        {
            CSFramework.Logger.Log(string.Format("AddPurchaseTrackEvent: {0}, {1}, {2}, {3}", Context.PurchaseEventName, price, currency, transaction_id));
            // if (_adjust_manager)
            //     _adjust_manager.AddPurchaseTrackEvent(Context.PurchaseEventName, price, currency, transaction_id, is_test, info_dict);
            // if (_firebase_manager)
            //     _firebase_manager.AddTrackEvent(Context.PurchaseEventName, info_dict);
        }

        public void AddLoginTrackEvent ()
        {
            CSFramework.Logger.Log(string.Format("AddLoginTrackEvent: {0}", Context.LoginEventName));
            // if (_adjust_manager)
            //     _adjust_manager.AddLoginTrackEvent(Context.LoginEventName);
            // if (_firebase_manager)
            //     _firebase_manager.AddTrackEvent(Context.LoginEventName);
        }

        public void AddBI (BI bi)
        {
            bi.Timestamp = Utils.CurrentTimestamp();
            lock (_bi_lock)
                _bi_list.Add(bi);

            var info_dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(bi.EventParam))
                info_dict["Param"] = bi.EventParam;
            if (!string.IsNullOrEmpty(bi.Target))
                info_dict["Target"] = bi.Target;
            if (!string.IsNullOrEmpty(bi.Reason))
                info_dict["Reason"] = bi.Reason;
                
            AddTrackEvent(bi.EventType, info_dict);

            OnAddBI?.Invoke(bi);
            // CSFramework.Logger.Log(string.Format("add bi: {0}, {1}, {2}, {3}", bi.BIType, bi.EventType, bi.Target, bi.Reason));
        }

        public List<BI> PackBI ()
        {
            lock (_bi_lock)
            {
                var package = _bi_list;
                _bi_list = new List<BI>();
                return package;
            }
        }
    }
}