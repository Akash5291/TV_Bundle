using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

[Serializable]
public class InsertData
{
    public string user_id;
    public string server_ip_1;
    public string server_ip_2;
    public string server_ip;
    public string gamecode;
}

[Serializable]
public class UpdateData
{
    public string user_id;
    public string gamecode;
}

[Serializable]
public class GameCode
{
    public string user_id;
    public string server_ip;
}

public class CommonData
{
    public string device_id;
    public string game_id;
}

[Serializable]
public class GetGameProfile
{
    public string score;
    public string max_score;
    public string rating;
    public string avg_rating;
    public string active_users;
}

[Serializable]
public class TVURL
{
    public string base_url;
}


[Serializable]
public class InhouseAds
{
    public List<AdsItemData> ourAds = new List<AdsItemData>();
}

[Serializable]
public class AdsItemData
{
    public int index;
    public int priority;
    public string game_icon;
    public string game_name;
    public string game_bundle_name;
    public string download_url;
    public string preview_video_url;
}

public class APIManager : MonoBehaviour
{
#if UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern string setCookie(string name, string value);
    [DllImport("__Internal")]
    private static extern string getCookie(string name);

#endif

    public static APIManager Instance = null;

    [SerializeField] string TVBaseURL = "";
    [SerializeField] QRCodeEncodeController e_qrController;
    [SerializeField] Image qrIMG;
    [SerializeField] GameObject Loading;
    public bool userPofileFound = false;

    [Header("Instruction")]
    public GameObject QRInstruction;

    [Header("Internet error")]
    [SerializeField] GameObject internetObj;
    [SerializeField] TMP_Text footerText;

    [Header("User Profile Data")]
    public GameObject UserProfileObj;
    [SerializeField] Image ratingImg;
    [SerializeField] TMP_Text highScore;
    [SerializeField] TMP_Text myScore;

    int closeTime = 5;

    public string base_url = "";
    string getQRCode_URL = "";
    string setServerIPURL = "";
    string getUserProfile_URL = "";
    string uuid = "";

    [Header("In-House Ads")]
    public InhouseAds inHouseAds;

    //[Header("Game Video Preview")]
    //[SerializeField] GameObject preview_player;
    //[SerializeField] VideoPlayer videoPlayer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        PlayerPrefs.SetInt("AdsShown", PlayerPrefs.GetInt("AdsShown", 0));
        PlayerPrefs.SetInt("MyLevel", PlayerPrefs.GetInt("MyLevel", 0));
        e_qrController.onQREncodeFinished += qrEncodeFinished;//Add Finished Event
    }

    public void onCallInitApis()
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

    void setupUUID()
    {
#if UNITY_WEBGL
        uuid = onGetUUID();
#else
        uuid = PlayerPrefs.GetString("UUID", "");
        if (string.IsNullOrEmpty(uuid))
        {
            uuid = SystemInfo.deviceUniqueIdentifier;
            PlayerPrefs.SetString("UUID", uuid);
            OnGenerateQR(uuid);
            InsertQRData();
        }
        else
        {
            OnGenerateQR(uuid);
            Invoke("SetServerIP", 2f);
        }
#endif

        //Debug.Log("Return cookies value is: " + uuid);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
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
        user.game_id = WifiManager.Instance.gameID;
        string data = JsonUtility.ToJson(user);

        UnityWebRequest request = new UnityWebRequest(TVBaseURL + ".php", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data)) { contentType = "application/json" };
        request.downloadHandler = new DownloadHandlerBuffer();
        //Debug.Log("TVBaseURL: " + TVBaseURL);
        yield return request.SendWebRequest();

        RaycastUnblock();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("getBaseURL: " + request.error);
        }
        else
        {
            //Debug.Log("Response: " + request.responseCode + "," + request.downloadHandler.text);
            RaycastUnblock();
            TVURL tVURL = JsonUtility.FromJson<TVURL>(request.downloadHandler.text);
            base_url = tVURL.base_url;
            //Debug.Log("base_url: " + base_url);
            getQRCode_URL = base_url + "matchuser_id.php";
            setServerIPURL = base_url + "multipleserver_ip_insert.php";
            getUserProfile_URL = base_url + "getdetails.php";
            setupUUID();

            LanguageSelector.Instance.onGetTranslatorText();
            getInhouseAds();
        }
    }
    #endregion

    #region GenerateUnique_ID

#if UNITY_WEBGL
    string onGetUUID()
    {
        string str = "";
#if UNITY_EDITOR
        str = SystemInfo.deviceUniqueIdentifier;
#else
        str = getCookie("UUID");
#endif
        if (string.IsNullOrEmpty(str))
        {
            string final = "";
            str = Guid.NewGuid().ToString();
            int len = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (i % 3 == 0 && len < WifiManager.Instance.gameID.Length)
                    final += WifiManager.Instance.gameID[len++];
                
                final += str[i];
            }
            str = final;

            setCookie("UUID", str);
            InsertQRData();
        }
        else
        {
            Invoke("SetServerIP", 2f);
        }
        return str;
    }
#endif

        void OnGenerateQR(string code)
    {
        e_qrController.Encode(code);
    }

    void qrEncodeFinished(Texture2D tex)
    {
        if (tex != null && tex != null)
        {
            qrIMG.transform.GetChild(0).transform.gameObject.SetActive(false);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            qrIMG.sprite = sprite;
        }
        else
        {
            Debug.Log("QR Finish tex == null");
        }
    }
#endregion

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

#region CheckConnection
    IEnumerator CheckInternetConnection(Action<bool> action)
    {
        RayCastBlock();
        yield return new WaitForSeconds(1f);
        /*UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            RaycastUnblock();
            action(false);
        }
        else*/
        {
            action(true);
        }
    }
#endregion

#region InsertQRData_in_Server
    void InsertQRData()
    {
        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                StartCoroutine(InsertQR());
            }
        }));
    }

    IEnumerator InsertQR()
    {
        InsertData code = new InsertData();
        code.user_id = uuid;
        code.gamecode = WifiManager.Instance.gameID;
        code.server_ip_1 = "";
        code.server_ip_2 = "";
        code.server_ip = "";

        string data = JsonUtility.ToJson(code);
        Debug.Log("URL: " + getQRCode_URL);
        Debug.Log("InsertQR: " + data);

        UnityWebRequest request = new UnityWebRequest(getQRCode_URL, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data)) { contentType = "application/json" };
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            RaycastUnblock();
            Invoke("SetServerIP", 1f);
        }
    }
#endregion

#region SetServerIP
    public void SetServerIP()
    {
        CancelInvoke();
        Debug.Log("SetServerIP: " + WifiManager.Instance.isClientConnected);
        if (WifiManager.Instance.isClientConnected)
            return;

        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                StartCoroutine(SetServer());
            }
        }));
    }

    IEnumerator SetServer()
    {
        GameCode code = new GameCode();
        code.server_ip = MyController.Instance.getMyIP();
        code.user_id = uuid;
        //WifiManager.Instance.connectPlayerTxt.text = code.server_ip + " : " + code.user_id;
        string data = JsonUtility.ToJson(code);
        //Debug.Log("server_ip send: " + data);

        UnityWebRequest request = new UnityWebRequest(setServerIPURL, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data)) { contentType = "application/json" };
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        //Debug.Log("setServerIP Response: " + request.downloadHandler.text);
        RaycastUnblock();
        //WifiManager.Instance.connectPlayerTxt.text = request.downloadHandler.text;
        if (request.result == UnityWebRequest.Result.ConnectionError)// || response.status.ToLower().Equals("fail"))
        {
            Debug.Log(request.error);
        }
        else
        {
            OnGenerateQR(uuid);
        }
    }
    #endregion

#region GetUserGameProfile
    public void callGetProfile(string ids)
    {
        StartCoroutine(OnUserGameDatas(ids));
    }

    IEnumerator OnUserGameDatas(string id)
    {
        // check internet
        UnityWebRequest request;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Internet issuse");
        }
        else
        {
            CommonData user = new CommonData();
            user.device_id = id;
            user.game_id = WifiManager.Instance.gameID;

            string data = JsonUtility.ToJson(user);
            Debug.Log("call get user data api: " + data);
            Debug.Log("URL: " + getUserProfile_URL);

            request = new UnityWebRequest(getUserProfile_URL, "POST");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data)) { contentType = "application/json" };
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            Debug.Log("responseCode: " + request.responseCode);

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error in get game details api: " + request.error);
                callGetProfile(id);
            }
            else
            {
                GetGameProfile profileData = JsonUtility.FromJson<GetGameProfile>(request.downloadHandler.text.Trim());
                string s = request.downloadHandler.text.Trim();
                Debug.Log("GameDetails are: " + s);
                string st = s.Substring(1, s.Length - 1);
                string str = "{\"id\":\"" + id + "\", \"screenName\":\"sendPlayerProfile\"," + st;

                int Gscore = 0;
                int mScore = 0;
                if (string.IsNullOrEmpty(profileData.avg_rating))
                    ratingImg.fillAmount = 0f;
                else
                    ratingImg.fillAmount = (float.Parse(profileData.avg_rating) / 10 * 2);

                if (string.IsNullOrEmpty(profileData.score))
                    Gscore = 0;
                else
                    Gscore = int.Parse(profileData.score);

                if (string.IsNullOrEmpty(profileData.max_score))
                    mScore = 0;
                else
                    mScore = int.Parse(profileData.max_score);

                highScore.text = mScore.ToString();
                myScore.text = Gscore.ToString();

                if (WifiManager.Instance.isLevelGame)
                {
                    highScore.text = "Level " + mScore.ToString();
                    if (Gscore <= 0)
                        myScore.text = "Level 1";
                    else
                        myScore.text = "Level " + Gscore.ToString();

                    if (string.IsNullOrEmpty(profileData.score))
                        MyController.Instance.openLevel = 0;
                    else
                        MyController.Instance.openLevel = int.Parse(profileData.score);

                    PlayerPrefs.SetInt("0_unlockedLevel", 0);
                    PlayerPrefs.SetInt("1_unlockedLevel", 0);
                    PlayerPrefs.SetInt("2_unlockedLevel", 0);
                    PlayerPrefs.SetInt("3_unlockedLevel", 0);

                    if (MyController.Instance.openLevel <= 100)
                        PlayerPrefs.SetInt("0_unlockedLevel", MyController.Instance.openLevel);
                    else if (MyController.Instance.openLevel > 300)
                    {
                        PlayerPrefs.SetInt("0_unlockedLevel", 100);
                        PlayerPrefs.SetInt("1_unlockedLevel", 100);
                        PlayerPrefs.SetInt("2_unlockedLevel", 100);
                        PlayerPrefs.SetInt("3_unlockedLevel", MyController.Instance.openLevel - 300);
                    }
                    else if (MyController.Instance.openLevel > 200)
                    {
                        PlayerPrefs.SetInt("0_unlockedLevel", 100);
                        PlayerPrefs.SetInt("1_unlockedLevel", 100);
                        PlayerPrefs.SetInt("2_unlockedLevel", MyController.Instance.openLevel - 200);
                    }
                    else if (MyController.Instance.openLevel > 100)
                    {
                        PlayerPrefs.SetInt("0_unlockedLevel", 100);
                        PlayerPrefs.SetInt("1_unlockedLevel", MyController.Instance.openLevel - 100);
                    }
                }

                userPofileFound = true;
                MyController.Instance.sendProfileData(str);
            }
        }
        //request.Dispose();
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
        }
    }

    #endregion
}
