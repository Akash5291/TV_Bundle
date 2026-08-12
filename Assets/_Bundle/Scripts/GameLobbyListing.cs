using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameLobbyListing : MonoBehaviour
{

    [Header("Feature Game")]
    [SerializeField] Image featureGameImage;
    [SerializeField] TMP_Text featureGameTitle;
    [SerializeField] TMP_Text featureGameDescription;

    [Header("New Arrival")]
    [SerializeField] GameObject newArrivalParent;

    [Header("Our Games")]
    [SerializeField] GameObject ourGamesParent;

    [Header("Common Items")]
    [SerializeField] GameObject gameListingObj;

    void Start()
    {
        inItSetup();
    }

    void inItSetup()
    {
        StartCoroutine(DownloadFeatureImage(BundleAPIManager.Instance.gameBundleData.feature_game.image_url));
        featureGameTitle.text = BundleAPIManager.Instance.gameBundleData.feature_game.title;
        featureGameDescription.text = BundleAPIManager.Instance.gameBundleData.feature_game.description;

        // Set New arrival rail
        for (int i = 0; i < BundleAPIManager.Instance.gameBundleData.new_arrival.Count; i++)
        {
            var obj = Instantiate(gameListingObj, newArrivalParent.transform);
            obj.GetComponent<GameBundleItem>().setGameBundleData(BundleAPIManager.Instance.gameBundleData.new_arrival[i]);
        }

        // Set Our Game rail
        for (int j = 0; j < BundleAPIManager.Instance.gameBundleData.our_games.Count; j++)
        {
            var obj = Instantiate(gameListingObj, ourGamesParent.transform);
            obj.GetComponent<GameBundleItem>().setGameBundleData(BundleAPIManager.Instance.gameBundleData.our_games[j]);
        }
    }

    IEnumerator DownloadFeatureImage(string imageUrl)
    {
        Debug.Log("Feature Game Icon : " + imageUrl);
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
            featureGameImage.sprite = sprite;
        }
    }

   
}
