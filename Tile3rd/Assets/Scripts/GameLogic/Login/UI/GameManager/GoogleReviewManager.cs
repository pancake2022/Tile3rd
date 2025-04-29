using System.Collections;
using UnityEngine;
using Google.Play.Review;
using CSFramework;

public class GoogleReviewManager : WindowUI
{
    public static GoogleReviewManager Instance { get; private set; }
    private ReviewManager _reviewManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _reviewManager = new ReviewManager();
        }
    }
    public void TryPromptReview()
    {
        StartCoroutine(RequestAndLaunchReview());
    }

    private IEnumerator RequestAndLaunchReview()
    {
        Debug.Log("unity-script: reviewOK");
        var requestFlow = _reviewManager.RequestReviewFlow();
        yield return requestFlow;

        if (requestFlow.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning("unity-script: RequestReviewFlow failed: " + requestFlow.Error);
            yield break;
        }

        var reviewInfo = requestFlow.GetResult();

        var launchFlow = _reviewManager.LaunchReviewFlow(reviewInfo);
        yield return launchFlow;

        if (launchFlow.Error == ReviewErrorCode.NoError)
        {
            Debug.Log("unity-script: Review launched successfully");

            //review请求成功后，重制review刷新数据
            var commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
            var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
            commonStorage.Android_Reviewed = 10;
            //shareDataGlobalConfig._is_interstitial = false;
        }
        else
        {
            Debug.LogWarning("unity-script: LaunchReviewFlow failed: " + launchFlow.Error);
        }
    }
}