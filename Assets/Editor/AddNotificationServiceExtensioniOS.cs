using UnityEditor;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using System.IO;
using UnityEditor.Callbacks;
using System;
using System.Text.RegularExpressions;

public class AddNotificationServiceExtensioniOS : MonoBehaviour
{
    [PostProcessBuild(999)]
    private static void PostProcessBuild_iOS(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS)
            return;

        //Copy files to xcode project dir
        Directory.CreateDirectory(buildPath + "/NotificationService");
        File.Copy("Assets/NotificationService/NotificationService.h", buildPath + "/NotificationService/NotificationService.h", true);
        File.Copy("Assets/NotificationService/NotificationService.m", buildPath + "/NotificationService/NotificationService.m", true);
        File.Copy("Assets/NotificationService/Info.plist", buildPath + "/NotificationService/Info.plist", true);

        //load xcode project
        PBXProject proj = new PBXProject();
        string projPath = PBXProject.GetPBXProjectPath(buildPath);
        proj.ReadFromFile(projPath);

        string mainTarget = proj.GetUnityMainTargetGuid();

        //Try to retreive bundleId from xcode project
        string bundleId = null;

        string infoPlistPath = "Info.plist";
        try {
            infoPlistPath = proj.GetBuildPropertyForAnyConfig(mainTarget, "INFOPLIST_FILE");
        } catch (Exception e)
        {
            Debug.Log("Can't load Info.plist location: " + e);
        }

        try
        {
            PlistDocument infoPlist = new PlistDocument();
            infoPlist.ReadFromFile(buildPath + "/" + infoPlistPath);
            bundleId = infoPlist.root.values["CFBundleIdentifier"].AsString();
            Regex configParamsMatcher = new Regex(@"\$\{([\w_]*)\}");
            MatchCollection matches = configParamsMatcher.Matches(bundleId);
            bundleId = configParamsMatcher.Replace(bundleId, (match) =>
            {
                return proj.GetBuildPropertyForAnyConfig(mainTarget, match.Groups[1].Value);
            });
        }
        catch (Exception e)
        {
            Debug.Log("Can't load BundleId: " + e);
            if (String.IsNullOrEmpty(bundleId))
            {
                bundleId = "com.unity3d.product";
            }
        }

        //Add extension
        string extensionBundleId = bundleId + ".NotificationService";
        string newTarget = proj.AddAppExtension(mainTarget, "NotificationService", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS) + ".NotificationService", "NotificationService/Info.plist");
        
        // Add source files to the extension target
        proj.AddFileToBuild(newTarget, proj.AddFile("NotificationService/NotificationService.h", "NotificationService/NotificationService.h"));
        proj.AddFileToBuild(newTarget, proj.AddFile("NotificationService/NotificationService.m", "NotificationService/NotificationService.m"));

        // Set build properties
        proj.SetBuildProperty(newTarget, "IPHONEOS_DEPLOYMENT_TARGET", "12.0");
        proj.SetBuildProperty(newTarget, "TARGETED_DEVICE_FAMILY", "1,2");
        proj.SetBuildProperty(newTarget, "PRODUCT_BUNDLE_IDENTIFIER", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS) + ".NotificationService");
        proj.SetBuildProperty(newTarget, "CODE_SIGN_STYLE", "Automatic");
        proj.SetBuildProperty(newTarget, "PRODUCT_NAME", "NotificationService");
        proj.SetBuildProperty(newTarget, "SKIP_INSTALL", "YES");
        
        // Copy development team from main target
        string developmentTeam = proj.GetBuildPropertyForAnyConfig(mainTarget, "DEVELOPMENT_TEAM");
        if (!string.IsNullOrEmpty(developmentTeam))
            proj.SetBuildProperty(newTarget, "DEVELOPMENT_TEAM", developmentTeam);
        
        // Set inherited flags for CocoaPods
        proj.SetBuildProperty(newTarget, "GCC_PREPROCESSOR_DEFINITIONS", "$(inherited)");
        proj.SetBuildProperty(newTarget, "OTHER_LDFLAGS", "$(inherited)");
        proj.SetBuildProperty(newTarget, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
        proj.SetBuildProperty(newTarget, "HEADER_SEARCH_PATHS", "$(inherited)");
        
        // Add system frameworks
        proj.AddFrameworkToProject(newTarget, "UserNotifications.framework", false);
        proj.AddFrameworkToProject(newTarget, "Foundation.framework", false);
        
        // Update Podfile and write project
        UpdatePodfile(buildPath, extensionBundleId);
        proj.WriteToFile(projPath);
    }
    
    private static void UpdatePodfile(string buildPath, string extensionBundleId)
    {
        string podfilePath = buildPath + "/Podfile";
        if (!File.Exists(podfilePath)) return;
        
        string podfileContent = File.ReadAllText(podfilePath);
        if (podfileContent.Contains("target 'NotificationService'")) return;
        
        string target = @"
target 'NotificationService' do
  pod 'PushwooshXCFramework', '6.7.18'
end

";
        
        if (podfileContent.Contains("use_frameworks!"))
            podfileContent = podfileContent.Replace("use_frameworks!", target + "use_frameworks!");
        else
            podfileContent += target;
            
        File.WriteAllText(podfilePath, podfileContent);
    }
}
