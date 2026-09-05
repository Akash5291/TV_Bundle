using UnityEngine;
using UnityEngine.SceneManagement;
using static SerializableClasses;

public class BundleManager : MonoBehaviour
{
    [SerializeField] GameObject wifiServerController;
    [SerializeField] GameObject pairing_screen;

    [Header("Game Scene Name")]
    [SerializeField] BundleGameSceneInfo[] gameSceneInfo;

    private void OnEnable()
    {
        ActionContainer.onClientDisconnected += clientDisconnected;
        ActionContainer.onStartGame += startGame;
        ActionContainer.onShowPairingScreenUI += showPairingScreen;
    }

    private void OnDestroy()
    {
        ActionContainer.onClientDisconnected -= clientDisconnected;
        ActionContainer.onStartGame -= startGame;
        ActionContainer.onShowPairingScreenUI -= showPairingScreen;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pairing_screen.activeSelf || WifiManager.Instance.isClientConnected)
            {
                pairing_screen.SetActive(false);
                MyController.Instance.backToGameLobby();
            }
            SceneManager.LoadSceneAsync("GameListing");
        }
    }

    string getSceneName()
    {
        string str = "";
        for (int i = 0; i < gameSceneInfo.Length; i++)
        {
            if (gameSceneInfo[i].Game_Id.Equals(WifiManager.Instance.gameID))
            {
                str = gameSceneInfo[i].SceneName;
                break;
            }
        }
        return str;
    }

    void clientDisconnected()
    {
        quitGameByCloseBtn();
    }

    void startGame()
    {
        pairing_screen.SetActive(false);
        // load game's home scene here
        SceneManager.LoadSceneAsync(getSceneName());
    }

    void showPairingScreen()
    {
        wifiServerController.SetActive(true);
        pairing_screen.SetActive(true);
        MyController.Instance.onStartServers(true);
        WifiManager.Instance.gameID = PlayerPrefs.GetString("SelectedGameID");
        APIManager.Instance.InsertQRData();
    }

    void quitGameByCloseBtn()
    {
        //close connectin, off android server
        SceneManager.LoadSceneAsync("GameListing_New");
    }
}
