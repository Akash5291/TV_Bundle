using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static SerializableClasses;

public class BundleAPIManager : MonoBehaviour
{
    public static BundleAPIManager Instance = null;

    [SerializeField] string GetBaseURLFrom = "";
    public string base_url = "";

    [SerializeField] GameObject Loading;


    [Header("Internet error")]
    [SerializeField] GameObject internetObj;
    [SerializeField] TMP_Text footerText;

    [Header("Game Bundle Info")]
    public BundleGameList gameBundleData;

    [Header("In-House Ads")]
    public InhouseAds inHouseAds;

    int closeTime = 5;

    private void OnEnable()
    {
        ActionContainer.onSplashScreenAnimComplete += onLoadGameLobby;
    }

    private void OnDisable()
    {
        ActionContainer.onSplashScreenAnimComplete -= onLoadGameLobby;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PlayerPrefs.SetInt("AdsShown", PlayerPrefs.GetInt("AdsShown", 0));
    }

    void Start()
    {
        if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            closeTime = 5;
            internetObj.SetActive(true);
            footerText.text = "";
            Invoke("counterCheck", 1f);
        }
        else
        {
            internetObj.SetActive(false);
            onGetBaseURL();
        }
    }

    void counterCheck()
    {
        if (closeTime <= 0)
        {
#if UNITY_WEBGL
            footerText.text = "";
#else
            Application.Quit();
#endif
        }
        else
        {
            closeTime--;
#if UNITY_WEBGL
            footerText.text = "";
#else
            footerText.text = "0" + closeTime.ToString();
#endif
            Invoke("counterCheck", 1f);
        }
    }

    #region RayCast_Loader
    void RayCastBlock()
    {
        Loading.SetActive(true);
    }

    void RaycastUnblock()
    {
        Loading.SetActive(false);
    }
    #endregion

    #region GetBaseURL
    void onGetBaseURL()
    {
        RayCastBlock();
        StartCoroutine(getBaseURL());
    }

    IEnumerator getBaseURL()
    {
        CommonData user = new CommonData();
        user.device_id = "dummy";
        user.game_id = "";
        string data = JsonUtility.ToJson(user);

        UnityWebRequest request = new UnityWebRequest(GetBaseURLFrom + ".php", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data)) { contentType = "application/json" };
        request.downloadHandler = new DownloadHandlerBuffer();
        //Debug.Log("TVBaseURL: " + TVBaseURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("getBaseURL: " + request.error);
        }
        else
        {
            //Debug.Log("Response: " + request.responseCode + "," + request.downloadHandler.text);
            TVURL tVURL = JsonUtility.FromJson<TVURL>(request.downloadHandler.text);
            base_url = tVURL.base_url;

            StartCoroutine(getGameBundleInfo());
        }
    }
    #endregion

    #region Bundle Game Data Fatch
    IEnumerator getGameBundleInfo()
    {
        UnityWebRequest request = new UnityWebRequest(base_url + StaticData.bundleData_URL, "GET");
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        RaycastUnblock();
        gameBundleData = JsonUtility.FromJson<BundleGameList>(request.downloadHandler.text);

        getInhouseAds();
    }
    #endregion

    #region Inhouse_Ads
    void getInhouseAds()
    {
        StartCoroutine(getAdsData());
    }

    IEnumerator getAdsData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(base_url + "inhouse_ads/our_ads.txt"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("getBaseURL: " + www.error);
            }
            else
            {
                //Debug.Log("Translated info: " + www.downloadHandler.text);
                inHouseAds = JsonUtility.FromJson<InhouseAds>(www.downloadHandler.text);
            }
            //Invoke("jumpToGameLobby", 1f);
        }
    }
    #endregion

    #region Action Receiver Method
    void onLoadGameLobby()
    {
        jumpToGameLobby();
    }
    #endregion

    void jumpToGameLobby()
    {
        SceneManager.LoadScene(1);
    }
}
