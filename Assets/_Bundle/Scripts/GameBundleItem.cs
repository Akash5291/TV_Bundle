using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static SerializableClasses;

public class GameBundleItem : MonoBehaviour
{
    [SerializeField] Image gameBanner;
    [SerializeField] BundleGameData gameItem;

    public void setGameBundleData(BundleGameData data)
    {
        gameItem = data;
        StartCoroutine(DownloadAndSetImage(data.image_url));

        // Resolve install status once up front, rather than re-checking every
        // time this game is opened from the lobby.
        if (!string.IsNullOrEmpty(data.download_link))
        {
            data.installedPackageName = AndroidAppLauncher.ExtractPackageName(data.download_link);
            data.isInstalled = AndroidAppLauncher.IsAppInstalled(data.installedPackageName);
            Debug.Log($"[GameBundleItem] {data.title}: download_link='{data.download_link}' -> package='{data.installedPackageName}' installed={data.isInstalled}");
        }
    }

    IEnumerator DownloadAndSetImage(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(BundleAPIManager.Instance.base_url + imageUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download bundle game icon: " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            // Convert to Sprite
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite sprite = Sprite.Create(texture, rect, pivot);

            // Assign to Image component
            gameBanner.sprite = sprite;
        }
    }

    public void onGameSelecteToPlay()
    {
        ActionContainer.onGameSelectToPlay?.Invoke(gameBanner.sprite, gameItem);
    }

}
