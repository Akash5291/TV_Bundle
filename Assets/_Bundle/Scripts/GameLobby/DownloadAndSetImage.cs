using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadAndSetImage : MonoBehaviour
{
    [SerializeField] Image imageRef;

    public void onSetScreenShot(string url)
    {
        StartCoroutine(DownloadToSetImage(url));
    }

    IEnumerator DownloadToSetImage(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(BundleAPIManager.Instance.base_url + imageUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download SS: " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            // Convert to Sprite
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite sprite = Sprite.Create(texture, rect, pivot);

            // Assign to Image component
            imageRef.sprite = sprite;
        }
    }
}
