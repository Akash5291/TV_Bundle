using UnityEngine;

// Talks to the Android package manager so the game-detail screen can tell
// whether a downloadable game is already installed on the TV, and either
// launch it directly or fall back to opening its Play Store page.
public static class AndroidAppLauncher
{
    public static bool IsAppInstalled(string packageName)
    {
        if (string.IsNullOrEmpty(packageName)) return false;

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager"))
            {
                packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
                return true;
            }
        }
        catch (AndroidJavaException)
        {
            return false;
        }
#else
        return false;
#endif
    }

    public static bool LaunchApp(string packageName)
    {
        if (string.IsNullOrEmpty(packageName)) return false;

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager"))
            using (var launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName))
            {
                if (launchIntent == null) return false;

                currentActivity.Call("startActivity", launchIntent);
                return true;
            }
        }
        catch (AndroidJavaException e)
        {
            Debug.LogError($"Failed to launch installed app {packageName}: {e.Message}");
            return false;
        }
#else
        Debug.Log($"[Editor] Would launch installed app: {packageName}");
        return false;
#endif
    }

    // Play Store links look like https://play.google.com/store/apps/details?id=<package> -
    // pull the package name back out so it can be used with the package manager above.
    public static string ExtractPackageName(string playStoreUrl)
    {
        if (string.IsNullOrEmpty(playStoreUrl)) return null;

        const string marker = "id=";
        int idx = playStoreUrl.IndexOf(marker);
        if (idx < 0) return null;

        int start = idx + marker.Length;
        int end = playStoreUrl.IndexOfAny(new[] { '&', '#' }, start);
        return end < 0 ? playStoreUrl.Substring(start) : playStoreUrl.Substring(start, end - start);
    }
}
