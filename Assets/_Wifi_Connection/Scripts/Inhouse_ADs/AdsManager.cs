using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance = null;

    [SerializeField] GameObject mainParent;
    [SerializeField] GameObject adsParent;
    [SerializeField] GameObject adsPrefabs;
    public List<Button> buttons = new List<Button>();

    int maxAdsLimit = 5;
    int adsAdded = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        buttons.Clear();
        showInhouseAds();
    }

    public void onShowAds(bool value)
    {
        transform.gameObject.SetActive(value);
    }

    void showInhouseAds()
    {
        int cnt = 0;
        if (APIManager.Instance.inHouseAds.ourAds.Count <= 0)
            mainParent.SetActive(false);
        else
        {
            mainParent.SetActive(true);
            APIManager.Instance.inHouseAds.ourAds.OrderBy(ad => ad.priority).ToList();
            int ai = 1;
            foreach (var a in APIManager.Instance.inHouseAds.ourAds)
            {
                APIManager.Instance.inHouseAds.ourAds[ai - 1].index = ai;
                ai++;
            }

            int startIndex = PlayerPrefs.GetInt("AdsShown");
            if (startIndex >= APIManager.Instance.inHouseAds.ourAds.Count)
            {
                startIndex = 0;
                PlayerPrefs.SetInt("AdsShown", 0);
            }
            for (int i = startIndex; i < APIManager.Instance.inHouseAds.ourAds.Count; i++)
            {
                var ad = APIManager.Instance.inHouseAds.ourAds[i];
                Debug.Log($"[{ad.index}] {ad.game_name} - {ad.download_url}");
                if (!ad.game_bundle_name.Equals(Application.identifier))
                {
                    cnt += 1;
                    PlayerPrefs.SetInt("AdsShown", i + 1);
                    var obj = Instantiate(adsPrefabs, adsParent.transform);
                    obj.GetComponent<AdsItem>().onSetGameData(ad);
                    adsAdded += 1;
                }

                if (adsAdded >= maxAdsLimit) break;
            }
            onSetDownloadButtonList();
        }
    }

    void onSetDownloadButtonList()
    {
        for (int i = 0; i < adsParent.transform.childCount; i++)
        {
            buttons.Add(adsParent.transform.GetChild(i).GetComponent<AdsItem>().downloadBtn);
        }
        MenuController.Instance.setAdsGameDownloadButton();
    }

    private void Update()
    {
        if (!mainParent.activeSelf && APIManager.Instance.inHouseAds.ourAds.Count > 0)
            showInhouseAds();
    }
}
