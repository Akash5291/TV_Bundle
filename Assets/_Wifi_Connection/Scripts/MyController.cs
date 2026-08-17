using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[Serializable]
public class Controller
{
    public int playerID;
    public string screenName;
    public string screenValue;
    public string buttonName;
    public string buttonValue;
    public bool isPointerUP = false;
    public List<Vector2> pos = new List<Vector2>();
}

public class MyController : MonoBehaviour
{
    public static MyController Instance = null;

    public static Action onNextButton;
    public static Action onPreviousButton;
    public static Action onUpButton;
    public static Action onDownButton;
    public static Action onSelectButton;
    public static Action<string> onGameButton;

    public int openLevel = 0;
    [Header("Wifi Clients")]
    public MyWebServer webServer;
    public MyAndroidServer androidServer;

    public bool isGameStart = false;
    public bool isCloseBtnPress = false;
    [SerializeField] UIWindow[] uiButtons;
    public int[] playersScore = new int[2];

    [SerializeField] Controller controllerDataReceived;

    string currentScreen = "";
    string previousScreen = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void onStartServers(bool value)
    {
        if (value)
        {
#if UNITY_WEBGL
        webServer.transform.gameObject.SetActive(value);
        webServer.Connect();
#else
            isCloseBtnPress = false;
            androidServer.transform.gameObject.SetActive(value);
            androidServer.maxConnectionAllow(BundleAPIManager.Instance.currentGame.numberOfPlayer);
            androidServer.Connect();
#endif
            //APIManager.Instance.onCallInitApis();Akash
        }
        else
        {
#if UNITY_WEBGL
        webServer.transform.gameObject.SetActive(value);
#else
            androidServer.transform.gameObject.SetActive(value);
#endif
        }
    }

    // return player connected index i.e. 1 player or 2 player
    int getPlayerConnectedID(string ip)
    {
        int n = 0;
        for (int i = 0; i < androidServer.ConnectedIPs.Count; i++)
        {
            if (androidServer.ConnectedIPs[i].ip.Equals(ip))
            { n = androidServer.ConnectedIPs[i].id; break; }
        }
        return n;
    }

    public string getMyIP()
    {
#if UNITY_WEBGL
        return webServer.GetMyIP();
#else
        return androidServer.GetMyIP();
#endif
    }

    public void onSetUpDownButtonsData(UIWindow[] data)
    {
        uiButtons = data;
    }

    string getJsonData(string name, string msg, List<Vector2> d)
    {
        Controller c = new Controller();
        c.screenName = name;

        if (name.Equals(StaticData.GameArea) || name.Equals(StaticData.GameQuizWheel))
        {
            if (!string.IsNullOrEmpty(msg))
                c.screenValue = msg;
            if (d != null)
            {
                if (d.Count != 0)
                    c.pos = d;
            }
        }

        if (WifiManager.Instance.supportedPlayer == SupportedGamePlayer.Two && name.Equals(StaticData.LevelFinish))
        {
            if (msg.Equals("1"))
            {
                msg = playersScore[0].ToString();
                c.playerID = 1;
            }
            else if (msg.Equals("2"))
            {
                msg = playersScore[1].ToString();
                c.playerID = 2;
            }
        }

        if (name.Equals("PlayerID"))
            c.screenValue = msg;
        else if (msg.Equals(WifiManager.Instance.gameID) || name.Equals(StaticData.LevelFinish) || name.Equals(StaticData.GameOver) || name.Equals(StaticData.LevelUp))
            c.screenValue = msg;

        return JsonUtility.ToJson(c);
    }

    public void sendProfileData(string str)
    {
#if UNITY_WEBGL
        webServer.Send(str);
#else
        androidServer.Send(str);
#endif
    }

    public void sendMessage(string name, string msg, List<Vector2> data = null)
    {
#if UNITY_WEBGL
        webServer.Send(getJsonData(name, msg, data));
#else
        androidServer.Send(getJsonData(name, msg, data));
#endif
    }

    public void myMessageReceived(string msg)
    {
        if (WifiManager.Instance.isClientConnected && !string.IsNullOrEmpty(msg))
        {
            Debug.Log("Received: " + msg);
            controllerDataReceived = JsonUtility.FromJson<Controller>(msg.Trim());

            if (string.Equals(controllerDataReceived.screenName, "differentIP"))
            {
#if UNITY_WEBGL
        webClient.Send(getJsonData(name, msg, data));
#else
                androidServer.updateNewConnectionDeviceID(controllerDataReceived.buttonValue, controllerDataReceived.buttonName);
#endif
            }
            else if (string.Equals(controllerDataReceived.screenName, "sendPlayerProfile"))
                APIManager.Instance.callGetProfile(controllerDataReceived.buttonName);
            else if (string.Equals(controllerDataReceived.screenName, "sendGameID"))
                callGameID();

            if (controllerDataReceived.screenName != StaticData.GameArea && !string.Equals(controllerDataReceived.buttonName, "Ready")) 
            {
                if (controllerDataReceived.playerID == 2 && WifiManager.Instance.supportedPlayer == SupportedGamePlayer.Two)
                    return;
            }

            if (string.Equals(controllerDataReceived.buttonName, "Ready"))
            {
                WifiManager.Instance.isReady++;
                if (WifiManager.Instance.isReady >= 2)
                {
                    if (!isGameStart)
                    {
                        if (WifiManager.Instance.supportedPlayer != SupportedGamePlayer.Single)
                        {
                            if (androidServer.ConnectedIPs.Count < 2) return;
                        }

                        isGameStart = true;
                        CancelInvoke("callGameID");
                        ActionContainer.onStartGame?.Invoke();
                        //APIManager.Instance.QRInstruction.SetActive(false);
                        //SceneManager.LoadScene(1);
                    }
                }
            }
            else if (string.Equals(controllerDataReceived.buttonName, StaticData.SelectBtn))
                onSelectButton?.Invoke();
            else if (string.Equals(controllerDataReceived.buttonName, StaticData.UpBtn))
                onUpButton?.Invoke();
            else if (string.Equals(controllerDataReceived.buttonName, StaticData.DownBtn))
                onDownButton?.Invoke();
            else if (string.Equals(controllerDataReceived.buttonName, StaticData.NextBtn))
                onNextButton?.Invoke();
            else if (string.Equals(controllerDataReceived.buttonName, StaticData.PreviousBtn))
                onPreviousButton?.Invoke();
            else if (string.Equals(controllerDataReceived.buttonName, StaticData.CloseBtn))
            {
                Debug.Log("Close Application");
                backToGameLobby();
            }
            else
                onGameButton?.Invoke(msg);

            controllerDataReceived = null;
        }
    }

    void callGameID()
    {
        Debug.Log("inside callGameID: " + isGameStart);
        if (isGameStart)
            return;

        sendMessage("GameID", WifiManager.Instance.gameID);
    }

    public void backToGameLobby()
    {
        isCloseBtnPress = true;
        androidServer.Disconnect();
        isGameStart = false;
    }
}
