using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public class ConnectedPlayer
{
    public int id;
    public string ip;
    public string uniqueID;
}

[Serializable]
public class GetServerIP
{
    public string unique_id;
}

public enum SupportedGamePlayer
{
    Single,
    Two,
    Multiplayer
}

public class WifiManager : MonoBehaviour
{
    public static WifiManager Instance = null;

    public GameObject LoadingObj;
    public SupportedGamePlayer supportedPlayer;
    public TMP_Text connectPlayerTxt;
    [SerializeField] GameObject player2_img;
    public string gameID;
    public string serverIP = "";
    public bool isLevelGame = false;
    public bool isClientConnected = false;
    public bool isJump = false;

    public int isReady = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        if (supportedPlayer != SupportedGamePlayer.Two)
        {
            connectPlayerTxt.text = "";
            player2_img.SetActive(false);
        }
        else
            player2_img.SetActive(true);
    }

    private void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif
    }

    public void onClientDisconnected()
    {
        CancelInvoke("onClientConnected");
        isClientConnected = false;
        Time.timeScale = 1f;
        MyController.Instance.isGameStart = false;
        //BGMusic.Instance.pauseBGSond(false);
        MyController.Instance.sendMessage(StaticData.DisconnectAll, StaticData.DisconnectAll);
        Debug.Log("GameManager: Disconnect: " + MyController.Instance.isCloseBtnPress);
        if (MyController.Instance.isCloseBtnPress)
        {
            Debug.Log("Quit close");
            Application.Quit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        else
        {
            Debug.Log("normal disconnect");
            APIManager.Instance.QRInstruction.SetActive(true);
            APIManager.Instance.UserProfileObj.SetActive(false);

            if (supportedPlayer == SupportedGamePlayer.Two)
                connectPlayerTxt.text = "Connect player 1";
            else
                connectPlayerTxt.text = "";

            APIManager.Instance.userPofileFound = false;
            MyController.Instance.onStartServers(false);
            if (SceneManager.GetActiveScene().buildIndex != 0)
                SceneManager.LoadScene(0);
        }
    }

    public void onClientConnected()
    {
        Debug.Log("GameManager: Connect");
        CancelInvoke();
        isClientConnected = true;
        if (supportedPlayer == SupportedGamePlayer.Two)
        {
            if (MyController.Instance.androidServer.ConnectedIPs.Count < 2)
            {
                connectPlayerTxt.text = "Connect player 2";
                return;
            }
        }

        if (!isLevelGame)
        {
            APIManager.Instance.QRInstruction.SetActive(false);
            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            if (APIManager.Instance.userPofileFound)
            {
                APIManager.Instance.QRInstruction.SetActive(false);
                SceneManager.LoadSceneAsync(1);
            }
            else
                Invoke("onClientConnected", 1f);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("OnApplicationFocus: " + focus);
        if (!focus)
        {
            Debug.Log("background");
            Application.Quit();
        }
        else
            Debug.Log("foreground");
    }
}
