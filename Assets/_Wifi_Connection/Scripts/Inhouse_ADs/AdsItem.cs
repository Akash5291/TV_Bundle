using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdsItem : MonoBehaviour
{

    [SerializeField] Image icon;
    [SerializeField] TMPro.TMP_Text gName;
    public Button downloadBtn;

    string dURL = "";

    public void onSetGameData(AdsItemData ads)
    {
        gName.text = ads.game_name;
        dURL = ads.download_url;
        StartCoroutine(DownloadAndSetImage(ads.game_icon));
    }

    IEnumerator DownloadAndSetImage(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download ads game icon: " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            // Convert to Sprite
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite sprite = Sprite.Create(texture, rect, pivot);

            // Assign to Image component
            icon.sprite = sprite;
        }
    }

    public void onDonwload()
    {
        Application.OpenURL(dURL);
    }

}
