using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

namespace CSFramework.Editor
{
    public class PostprocessBuild
    {
        [PostProcessBuildAttribute(1000)]
        public static void OnPostprocessBuild(BuildTarget target, string path_to_built_project) 
        {
#if UNITY_IOS
        CSFramework.Logger.Log("OnPostprocessBuild: " + target + ", Path: " + path_to_built_project);
        if (target != BuildTarget.iOS)
        {
            return;
        }

        // check pbxproj
        string proj_path = path_to_built_project + "/Unity-iPhone.xcodeproj/project.pbxproj";
        var proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(proj_path));
        string target_guid = proj.GetUnityMainTargetGuid();
        proj.AddBuildProperty(target_guid, "OTHER_LDFLAGS", "-lxml2");

        proj.AddFrameworkToProject(target_guid, "UserNotificationsUI.framework", true);
        proj.AddFrameworkToProject(target_guid, "iAd.framework", true);
        proj.AddFrameworkToProject(target_guid, "AdSupport.framework", true);
        proj.AddFrameworkToProject(target_guid, "AppTrackingTransparency.framework", true);
        // adds the AuthenticationServices.framework as an Optional framework, preventing crashes in
        // iOS versions previous to 13.0
        proj.AddFrameworkToProject(target_guid, "AuthenticationServices.framework", true); 

        var framework_guid = proj.GetUnityFrameworkTargetGuid();

        proj.AddFrameworkToProject(framework_guid, "AuthenticationServices.framework", true); 
        proj.AddFrameworkToProject(framework_guid, "AppTrackingTransparency.framework", true);

        proj.SetBuildProperty(target_guid, "USYM_UPLOAD_AUTH_TOKEN", "FakeToken");
        proj.SetBuildProperty(framework_guid, "USYM_UPLOAD_AUTH_TOKEN", "FakeToken");

        proj.SetBuildProperty(target_guid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        proj.SetBuildProperty(target_guid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");

        File.WriteAllText(proj_path, proj.WriteToString());

        // check plist
        var plist_path = Path.Combine(path_to_built_project, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plist_path);

        // Update value
        PlistElementDict root_dict = plist.root;
        // if (root_dict.values.TryGetValue("LSApplicationQueriesSchemes", out var schemes))
        //     schemes.AsArray().AddString("fb-messenger-share-api");

        // remove exit on suspend if it exists.
        root_dict.values.Remove("UIApplicationExitsOnSuspend");

        // remove NSAllowsArbitraryLoadsInWebContent if it exists.
        if (root_dict.values.TryGetValue("NSAppTransportSecurity", out var app_transport_security))
            app_transport_security.AsDict().values.Remove("NSAllowsArbitraryLoadsInWebContent");

        if (!root_dict.values.ContainsKey("NSUserTrackingUsageDescription"))
            root_dict.values.Add("NSUserTrackingUsageDescription", new PlistElementString("Your data will be used to deliver personalized ads to you."));

        if (!root_dict.values.ContainsKey("ITSAppUsesNonExemptEncryption"))
            root_dict.values.Add("ITSAppUsesNonExemptEncryption", new PlistElementBoolean(false));

        // Write plist
        File.WriteAllText(plist_path, plist.WriteToString());

        string entitle_path = "Unity-iPhone/" + Application.productName + ".entitlements";
        var project_capability_manager = new ProjectCapabilityManager(proj_path, entitle_path, "Unity-iPhone", target_guid);
        project_capability_manager.AddInAppPurchase();
        project_capability_manager.AddSignInWithApple();
        // if (ConfigurationController.Instance.iOSAddSignInWithApple)
        // {
        //     project_capability_manager.AddSignInWithApple();
        // }
        // if (ConfigurationController.Instance.iOSAddPushNotification)
        // {
        //     bool isDebug = true;
        //     if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
        //     {
        //         isDebug = false;
        //     }

#if DEBUG || DEVELOPMENT_BUILD
            project_capability_manager.AddPushNotifications(true);
#else
            project_capability_manager.AddPushNotifications(false);
#endif
        // }
        project_capability_manager.WriteToFile();
#endif
        }
    }
}