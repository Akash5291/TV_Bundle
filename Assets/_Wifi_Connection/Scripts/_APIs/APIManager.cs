using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static SerializableClasses;

public class APIManager : MonoBehaviour
{
#if UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern string setCookie(string name, string value);
    [DllImport("__Internal")]
    private static extern string getCookie(string name);

#endif

    public static APIManager Instance = null;

    public string base_url = "";
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

    string getQRCode_URL = "";
    string setServerIPURL = "";
    string getUserProfile_URL = "";
    string uuid = "";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PlayerPrefs.SetInt("MyLevel", PlayerPrefs.GetInt("MyLevel", 0));
    }

    private void OnEnable()
    {
        e_qrController.onQREncodeFinished += qrEncodeFinished;//Add Finished Event
    }

    private void OnDisable()
    {
        e_qrController.onQREncodeFinished -= qrEncodeFinished;//remove Finished Event
    }

    void Start()
    {
        base_url = BundleAPIManager.Instance.base_url;

        getQRCode_URL = base_url + StaticData.getQRCode_URL;
        setServerIPURL = base_url + StaticData.setServerIPURL;
        getUserProfile_URL = base_url + StaticData.getUserProfile_URL;
        setupUUID();
        LanguageSelector.Instance.onGetTranslatorText();
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
        UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

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

        if (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable)
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

}
