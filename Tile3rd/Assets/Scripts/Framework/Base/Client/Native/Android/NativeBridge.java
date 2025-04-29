package com.csframework;

import android.Manifest;
import android.app.Activity;
import android.content.ActivityNotFoundException;
import android.content.ComponentName;
import android.content.Context;
import android.content.ServiceConnection;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.Intent;
import android.content.res.AssetFileDescriptor;
import android.content.res.Resources;
import android.content.res.Configuration;
import android.media.AudioManager;
import android.net.Uri;
import android.os.Bundle;
import android.os.IBinder;
import android.os.RemoteException;
// import androidx.annotation.NonNull;
import android.text.method.ScrollingMovementMethod;
// import androidx.core.app.NotificationManagerCompat;

import android.net.wifi.WifiManager;
import android.os.Build;

// import androidx.core.content.ContextCompat;

import android.provider.Settings.Secure;
import android.telephony.TelephonyManager;

import java.io.IOException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

// import androidx.appcompat.app.AlertDialog;
import android.content.DialogInterface;
import android.util.JsonReader;
import android.util.Log;
import android.view.View;


// import com.facebook.share.model.ShareMessengerGenericTemplateContent;
// import com.facebook.share.model.ShareMessengerGenericTemplateElement;
// import com.facebook.share.model.ShareMessengerURLActionButton;
// import com.facebook.share.widget.MessageDialog;
// import com.facebook.AccessToken;
// import com.facebook.login.LoginManager;

import com.unity3d.player.UnityPlayer;

import android.content.ClipData;
import android.content.ClipboardManager;

public class NativeBridge {

    private static String TAG = "NativeBridge";
    private static Context mContext;
    private static Activity mActivity;

    public static void initialize(Context context,Activity activity) {
        mContext = context;
        mActivity = activity;
    }

    public static boolean isAppInstalled(String packageName) {
        System.out.println("NativeBridge: isAppInstalled " + packageName);
        PackageInfo packageInfo = null;
        try {
            synchronized(mContext) {
                packageInfo = mContext.getPackageManager().getPackageInfo(packageName, 0);
            }
        } catch (PackageManager.NameNotFoundException e) {
            packageInfo = null;
            e.printStackTrace();
        }
        if (packageInfo == null) {
            return false;
        } else {
            return true;
        }
    }

    // public static boolean isFacebookInstalled() {
    //     System.out.println("NativeBridge: isFacebookInstalled");
    //     PackageInfo packageInfo = null;
    //     try {
    //         synchronized(mContext) {
    //             packageInfo = mContext.getPackageManager().getPackageInfo("com.facebook.katana", 0);
    //         }
    //     } catch (PackageManager.NameNotFoundException e) {
    //         packageInfo = null;
    //         e.printStackTrace();
    //     }
    //     if (packageInfo == null) {
    //         return false;
    //     } else {
    //         return true;
    //     }
    // }

    // // "https://www.facebook.com/YourPageName";
    // public static void openFacebookPage(String url, String pageId) {
    //     System.out.println("NativeBridge: openFacebookPage " + url + ", " + pageId);
    //     if(url.isEmpty() && pageId.isEmpty()) {
    //         return;
    //     }
    //     Intent facebookIntent = new Intent(Intent.ACTION_VIEW);
    //     facebookIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);

    //     String facebookUrl = getFacebookPageURL(mContext,url,pageId);
    //     facebookIntent.setData(Uri.parse(facebookUrl));

    //     try {
    //         mContext.startActivity(facebookIntent);
    //     } catch(Exception e) {
    //         try {
    //             Intent webIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
    //             webIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    //             mContext.startActivity(webIntent);
    //         } catch (Exception err) {
    //             err.printStackTrace();
    //         }
    //     }
    // }


    // public static void openTwitterPage(String twitterName, String twitterId) {
    //     System.out.println("NativeBridge: openTwitterPage " + twitterName + ", " + twitterId);
    //     if(twitterName.isEmpty() && twitterId.isEmpty()) {
    //         return;
    //     }
    //     try {

    //         Uri uri = null;

    //         if(!twitterId.isEmpty()) {
    //             uri = Uri.parse("twitter://user?user_id=" + twitterId);
    //         } else {
    //             uri = Uri.parse("twitter://user?screen_name==" + twitterName);
    //         }

    //         Intent twitterIntent = new Intent(Intent.ACTION_VIEW,uri);
    //         twitterIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    //         mContext.startActivity(twitterIntent);

    //     } catch (ActivityNotFoundException e) {
    //         try {
    //             Intent webIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("https://twitter.com/" + twitterName));
    //             webIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    //             mContext.startActivity(webIntent);
    //         } catch (Exception err) {
    //             err.printStackTrace();
    //         }
    //     }
    // }

    // public static void openInstagramPage(String instagramPageName, String instagramId) {
    //     System.out.println("NativeBridge: openInstagramPage " + instagramPageName + ", " + instagramId);
    //     if(instagramPageName.isEmpty() || instagramId.isEmpty()) {
    //         return;
    //     }
    //     try {
    //         Uri uri = Uri.parse("http://instagram.com/_u/" + instagramPageName);
    //         Intent likeIng = new Intent(Intent.ACTION_VIEW, uri);
    //         likeIng.setPackage("com.instagram.android");
    //         likeIng.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    //         mContext.startActivity(likeIng);

    //     } catch (ActivityNotFoundException e) {
    //         try {
    //             Intent webIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("http://instagram.com/"+instagramPageName));
    //             webIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    //             mContext.startActivity(webIntent);
    //         } catch (Exception err) {
    //             err.printStackTrace();
    //         }
    //     }
    // }

    // //method to get the right URL to use in the intent
    // private static String getFacebookPageURL(Context context,String facebookUrl, String pageId) {
    //     System.out.println("NativeBridge: getFacebookPageURL " + facebookUrl + ", " + pageId);
    //     PackageManager packageManager = context.getPackageManager();
    //     try {
    //         int versionCode = packageManager.getPackageInfo("com.facebook.katana", 0).versionCode;
    //         System.out.println("NativeBridge: versioncode is " + versionCode);
    //         if (versionCode >= 3002850) { //newer versions of fb app
    //             return "fb://facewebmodal/f?href=" + facebookUrl;
    //         } else { //older versions of fb app
    //             return "fb://profile/" + pageId;
    //         }
    //     } catch (PackageManager.NameNotFoundException e) {
    //         return facebookUrl; //normal web url
    //     }
    // }

    // public static void popupPrivacy(final String title,final String message,final  String linkTitle, final String linkUrl,final String agreeTitle,final String cancelTitle,final String gameObjectName) {
    //     System.out.println("NativeBridge: popupPrivacy " + title + ", " + message + ", linkUrl = " + linkUrl);
    //     if (title == null || message == null || linkTitle == null || linkUrl == null || agreeTitle == null || gameObjectName == null) {
    //         return;
    //     }

    //     AlertDialog.Builder builder = new AlertDialog.Builder(mActivity);

    //     builder.setTitle(title);
    //     builder.setMessage(message);
    //     // Add the buttons
    //     builder.setPositiveButton(agreeTitle, new DialogInterface.OnClickListener() {
    //         public void onClick(DialogInterface dialog, int id) {
    //             // User clicked OK button
    //             UnityPlayer.UnitySendMessage(gameObjectName, "OnPrivacyAccepted","");
    //         }
    //     });
    //     builder.setNeutralButton(linkTitle, new DialogInterface.OnClickListener() {
    //         @Override
    //         public void onClick(DialogInterface dialog, int which) {
    //             //DETAIL
    //             Intent privacyIntent = new Intent(Intent.ACTION_VIEW);
    //             privacyIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    //             privacyIntent.setData(Uri.parse(linkUrl));
    //             mContext.startActivity(privacyIntent);
    //         }
    //     });

    //     final AlertDialog dialog = builder.create();
    //     dialog.setCancelable(false);
    //     dialog.show();

    //     dialog.getButton(AlertDialog.BUTTON_NEUTRAL).setOnClickListener(new View.OnClickListener()
    //     {
    //         @Override
    //         public void onClick(View v)
    //         {
    //             Intent privacyIntent = new Intent(Intent.ACTION_VIEW);
    //             privacyIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    //             privacyIntent.setData(Uri.parse(linkUrl));
    //             mContext.startActivity(privacyIntent);

    //             Boolean wantToCloseDialog = false;
    //             //Do stuff, possibly set wantToCloseDialog to true then...
    //             if(wantToCloseDialog)
    //                 dialog.dismiss();
    //             //else dialog stays open. Make sure you have an obvious way to close the dialog especially if you set cancellable to false.
    //         }
    //     });
    // }


    public static String getVersionName() {
        System.out.println("NativeBridge: getVersionName");
        try {
            PackageManager manager = mContext.getPackageManager();
            PackageInfo info = manager.getPackageInfo(mContext.getPackageName(), 0);
            return info.versionName;
        } catch (Exception e) {
            return "Unknown";
        }
    }

    public static int getVersionCode() {
        System.out.println("NativeBridge: getVersionCode");
        try {
            PackageManager manager = mContext.getPackageManager();
            PackageInfo info = manager.getPackageInfo(mContext.getPackageName(), 0);
            return info.versionCode;
        } catch (Exception e) {
            return -1;
        }
    }

    public static String getMacAddress() {
        System.out.println("NativeBridge: getMacAddress");

        WifiManager wifi = (WifiManager) mContext.getSystemService(
                Context.WIFI_SERVICE);

        String wifiAddress = wifi.getConnectionInfo().getMacAddress();
        if (wifiAddress != null) {
            return wifiAddress;
        }
        try {
            boolean isWifiEnable = wifi.isWifiEnabled();
            if (!isWifiEnable) {
                wifi.setWifiEnabled(true);
            }
            for (int i = 0; i < 10; i++) {
                try {
                    Thread.sleep(1);
                    wifiAddress = wifi.getConnectionInfo().getMacAddress();
                } catch (InterruptedException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
            if (!isWifiEnable) {
                wifi.setWifiEnabled(false);
            }
            if (wifiAddress == null) {
                return "";
            }
            return wifiAddress;
        } catch (Exception ex) {
            return "";
        }
    }

    public static String getAndroidID() {
        System.out.println("NativeBridge: getAndroidID");
        return Secure.getString(mContext.getContentResolver(), Secure.ANDROID_ID);
    }

    public static String getDevIDShort() {
        System.out.println("NativeBridge: getDevIDShort");
        String m_szDevIDShort = "35" + // we make this look like a valid IMEI
                Build.BOARD.length() % 10+ Build.BRAND.length() % 10 +
                Build.CPU_ABI.length() % 10 + Build.DEVICE.length() % 10 +
                Build.DISPLAY.length() % 10 + Build.HOST.length() % 10 +
                Build.ID.length() % 10 + Build.MANUFACTURER.length() % 10 +
                Build.MODEL.length() % 10 + Build.PRODUCT.length() % 10 +
                Build.TAGS.length() % 10 + Build.TYPE.length() % 10 +
                Build.USER.length() % 10 ; // 13 digits
        return m_szDevIDShort;
    }

    public static String getIMEI() {
        System.out.println("NativeBridge: getIMEI");
//         TelephonyManager TelephonyMgr = (TelephonyManager)mContext.getSystemService(Activity.TELEPHONY_SERVICE);
//         if(TelephonyMgr != null) {
//             if (ContextCompat.checkSelfPermission(mContext,
//                     Manifest.permission.READ_PHONE_STATE)
//                     != PackageManager.PERMISSION_GRANTED) {

//                 // No explanation needed, we can request the permission.
// //暂不申请权限
// //              ActivityCompat.requestPermissions((Activity) mContext,new String[]{
// //                              Manifest.permission.READ_PHONE_STATE,},
// //                      1010);

//                 return "";

//             }
//             if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
//                 return TelephonyMgr.getImei();
//             } else {
//                 return TelephonyMgr.getDeviceId(); // Requires READ_PHONE_STATE
//             }
//         }
        return "";
    }

    public static String getDeviceId() {
        String m_szLongID = getIMEI() + getDevIDShort() + getAndroidID() + getMacAddress();

        System.out.println("NativeBridge: getIMEI : " + getIMEI());
        System.out.println("NativeBridge: getDevIDShort : " + getDevIDShort());
        System.out.println("NativeBridge: getAndroidID : " + getAndroidID());
        System.out.println("NativeBridge: getMacAddress : " + getMacAddress());

        // compute md5
        MessageDigest m = null;
        try {
            m = MessageDigest.getInstance("MD5");
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        }
        m.update(m_szLongID.getBytes(), 0, m_szLongID.length());
        // get md5 bytes
        byte p_md5Data[] = m.digest();
        // create a hex string
        String m_szUniqueID = new String();
        for (int i = 0; i < p_md5Data.length; i++) {
            int b = (0xFF & p_md5Data[i]);
            // if it is a single digit, make sure it have 0 in front (proper
            // padding)
            if (b <= 0xF)
                m_szUniqueID += "0";
            // add number to string
            m_szUniqueID += Integer.toHexString(b);
        } // hex string to uppercase
        m_szUniqueID = m_szUniqueID.toUpperCase();
        return m_szUniqueID;
    }

    public static String getDeviceType() {
        System.out.println("NativeBridge: getDeviceType");
        Resources resources = mContext.getResources();
        Configuration configuration = resources.getConfiguration();
        int screenLayout = configuration.screenLayout;
        int screenSize = screenLayout & Configuration.SCREENLAYOUT_SIZE_MASK;

        switch (screenSize) {
            case Configuration.SCREENLAYOUT_SIZE_SMALL:
            case Configuration.SCREENLAYOUT_SIZE_NORMAL:
                return "phone";
            case Configuration.SCREENLAYOUT_SIZE_LARGE:
            case 4:
                return "tablet";
            default:
                return "phone";
        }
    }

    // public static boolean FBMessagerShare(String linkUrl, String imageUrl, String pageId, String title, String subTitle, String buttonText) {
    //     System.out.println("NativeBridge: FBMessagerShare " + linkUrl + ", " + imageUrl);
    //     ShareMessengerURLActionButton actionButton =
    //             new ShareMessengerURLActionButton.Builder()
    //                     .setTitle(buttonText)
    //                     .setUrl(Uri.parse(linkUrl))
    //                     .build();
    //     ShareMessengerGenericTemplateElement genericTemplateElement =
    //             new ShareMessengerGenericTemplateElement.Builder()
    //                     .setTitle(title)
    //                     .setSubtitle(subTitle)
    //                     .setImageUrl(Uri.parse(imageUrl))
    //                     .setButton(actionButton)
    //                     .build();
    //     ShareMessengerGenericTemplateContent genericTemplateContent =
    //             new ShareMessengerGenericTemplateContent.Builder()
    //                     .setPageId(pageId) // Your page ID, required
    //                     .setGenericTemplateElement(genericTemplateElement)
    //                     .build();

    //     MessageDialog dialog = new MessageDialog(mActivity);

    //     if (dialog.canShow(genericTemplateContent)) {
    //         dialog.show(mActivity, genericTemplateContent);
    //         return true;
    //     }

    //     return false;
    // }

//    /**
//      * 检查Facebook AccessToken 是否有效
//      */
//     public static boolean isFaceBookAccessTokenActive() {
//         System.out.println("NativeBridge: isFaceBookAccessTokenActive");
//         return AccessToken.isCurrentAccessTokenActive();
//     }

//     /**
//      * 检查Facebook DataAccess 是否授权是否过期
//      */
//     public static boolean isFacebookDataAccessExpired() {
//         System.out.println("NativeBridge: isFacebookDataAccessExpired");
//         AccessToken cur = AccessToken.getCurrentAccessToken();
//         if(cur != null)
//         {
//             return cur.isDataAccessExpired();
//         }
//         else
//         {
//             return true;
//         }
//     }
    
//     /**
//      * 刷新 Facebook DataAccess 授权
//      */
//     public static void reauthorizeFacebookDataAccess() {
//         System.out.println("NativeBridge: reauthorizeFacebookDataAccess");
//         LoginManager.getInstance().reauthorizeDataAccess(mActivity);
//     }

    // public static boolean isUserNotificationEnabled() {
    //     System.out.println("NativeBridge: isUserNotificationEnabled");
    //     return NotificationManagerCompat.from(mContext).areNotificationsEnabled();
    // }

    public static void copy(String text) {
        System.out.println("NativeBridge: copy");
        ClipboardManager clipboard = (ClipboardManager) UnityPlayer.currentActivity.getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clipData = ClipData.newPlainText("text", text);
        clipboard.setPrimaryClip(clipData);
    }

    public static String paste() {
        System.out.println("NativeBridge: paste");
        ClipboardManager clipboard = (ClipboardManager) UnityPlayer.currentActivity.getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clipData = clipboard.getPrimaryClip();
        return clipData.getItemAt(0).getText().toString();
    }
}