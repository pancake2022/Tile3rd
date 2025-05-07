//using CSFramework;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Google.Play.Review;

//public class GoogleReview : WindowUI//MonoBehaviour
//{
//    // Start is called before the first frame update

//    private ReviewManager _reviewManager;
//    private PlayReviewInfo _playReviewInfo;

//    //void Start()
//    //{
//    //    var levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();//获取通用关卡存档
//    //    var commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
//    //    var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
//    //    var globalconfig = gameConfigGroup.GlobalConfigList[0];

//    //    //解锁判断
//    //    if (levelStorage.LevelCount == globalconfig.Unlock_GoogleReview)
//    //    {
//    //        if (commonStorage.Android_Reviewed <= 0)
//    //        {
//    //            StartCoroutine(GoogleReviewStart());
//    //            commonStorage.Android_Reviewed = 10;
//    //        }
//    //    }

//    //    //后续判断
//    //    if (levelStorage.LevelCount > globalconfig.Unlock_GoogleReview)
//    //    {
//    //        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
//    //        if (shareDataGlobalConfig._is_interstitial == false)
//    //        {
//    //            if (commonStorage.Android_Reviewed <= 0)
//    //            {
//    //                StartCoroutine(GoogleReviewStart());
//    //                commonStorage.Android_Reviewed = 10;
//    //            }
//    //        }
//    //    }
//    //}
//    public void ActiveGoogleReview()
//    {
//        StartCoroutine(GoogleReviewStart());
//    }

//    IEnumerator GoogleReviewStart()
//    {
//        Debug.Log("开始评分");
//        _reviewManager = new ReviewManager();

//        //第一步
//        var requestFlowOperation = _reviewManager.RequestReviewFlow();
//        yield return requestFlowOperation;
//        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
//        {
//            // Log error. For example, using requestFlowOperation.Error.ToString().
//            yield break;
//        }
//        _playReviewInfo = requestFlowOperation.GetResult();


//        //第二步
//        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
//        yield return launchFlowOperation;
//        _playReviewInfo = null; // Reset the object
//        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
//        {
//            // Log error. For example, using requestFlowOperation.Error.ToString().
//            yield break;
//        }
//        // The flow has finished. The API does not indicate whether the user
//        // reviewed or not, or even whether the review dialog was shown. Thus, no
//        // matter the result, we continue our app flow.
//    }
//}
