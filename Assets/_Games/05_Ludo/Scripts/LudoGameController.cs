using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LudoGameController : MonoBehaviour
{
    public static LudoGameController Instance;
    public enum GameState { NONE, READY, DICE, MOVE, MOVING, WAIT, FINISHED };
    public GameState gameState;
    public Transform waypointParent;
    public Transform playersParent;
    public Transform colorWayParent;
    public Transform pawnParent;
    public LudoTokenController greenPawn;
    public LudoTokenController bluePawn;
    public LudoTokenController yellowPawn;
    public LudoTokenController redPawn;
    public LudoTokenController myPawnController;
    public LudoTokenController currentPawnController;
    public List<LudoTokenController> activePawnControllers;
    public LudoToken[] greenPawns;
    public LudoToken[] bluePawns;
    public LudoToken[] yellowPawns;
    public LudoToken[] redPawns;
    public LudoToken[] allPawns;// use at photon
    public Sprite[] avatars;
    LudoToken[] myPawns;
    public string myPlayerColor;
    RaycastHit2D hit2D;
    public int currentDice;
    string[] pawnColors = new string[] { "Green", "Yellow", "Blue", "Red" };
    public int currentPawnID = 0;
    public bool isLocal;
    //public PhotonView photonView;
    public GameObject pauseScreen;
    LudoToken selectedPawn;
    [Header("Finished")]
    [SerializeField] GameObject winParticles;
    public GameObject finishedScreen;
    public GameObject finishedPanel;
    public Transform finishedPlayersParent;
    string winnerColor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    void OnEnable()
    {
        isLocal = PlayerPrefs.GetString("mode").Equals("computer");
        //photonView = GetComponent<PhotonView>();

        myPlayerColor = getMyPawnColor();
        myPawnController = getMyPawn();
        myPawns = myPawnController.token;
        for (int i = 0; i < myPawns.Length; i++)
            myPawns[i].transform.GetChild(0).gameObject.SetActive(true);

        activePawnControllers = new List<LudoTokenController>();

        //PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") - PhotonController.Instance.gameEntryPrice());
        PlayerPrefs.Save();

        DisableNotActivePawns();
    }

    void Update()
    {
        if (gameState != GameState.MOVE || gameState == GameState.WAIT) return;

        if (Input.GetMouseButtonDown(0) && currentPawnController == myPawnController)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
            if (hit2D.collider != null)
            {
                if (hit2D.collider.tag == "Token" && hit2D.collider.name.StartsWith(myPlayerColor))
                {
                    if (hit2D.collider.GetComponent<LudoToken>().inBase && currentDice != 5) return;

                    if (hit2D.collider.GetComponent<LudoToken>().moveCount + (currentDice + 1) > 56) return;

                    myPawnController.HighlightDices(false);
                    gameState = GameState.MOVING;
                    selectedPawn = hit2D.collider.GetComponent<LudoToken>();
                    hit2D.collider.GetComponent<LudoToken>().Move(currentDice + 1);
                }
            }
        }
    }

    #region TV_Input
    public void onTV_DiceBtn()
    {
        GameDiceBtn(myPlayerColor);
    }
    public void onOption_A()
    {
        if (gameState != GameState.MOVE || gameState == GameState.WAIT) return;

        if (currentPawnController == myPawnController)
        {
            myPawnController.HighlightDices(false);
            gameState = GameState.MOVING;
            myPawns[0].Move(currentDice + 1);
        }
    }
    public void onOption_B()
    {
        if (gameState != GameState.MOVE || gameState == GameState.WAIT) return;

        if (currentPawnController == myPawnController)
        {
            myPawnController.HighlightDices(false);
            gameState = GameState.MOVING;
            myPawns[1].Move(currentDice + 1);
        }
    }
    public void onOption_C()
    {
        if (gameState != GameState.MOVE || gameState == GameState.WAIT) return;

        if (currentPawnController == myPawnController)
        {
            myPawnController.HighlightDices(false);
            gameState = GameState.MOVING;
            myPawns[2].Move(currentDice + 1);
        }
    }
    public void onOption_D()
    {
        if (gameState != GameState.MOVE || gameState == GameState.WAIT) return;

        if (currentPawnController == myPawnController)
        {
            myPawnController.HighlightDices(false);
            gameState = GameState.MOVING;
            myPawns[3].Move(currentDice + 1);
        }
    }

    #endregion

    public void PauseBtn()
    {
        LudoAudioManager.Instance.onButtonClick();
        pauseScreen.SetActive(true);
        MenuController.Instance.onSetState(StaticData.GamePause);
        Time.timeScale = 0f;
    }

    public void PauseYesBtn()
    {
        LudoAudioManager.Instance.onButtonClick();
        Time.timeScale = 1f;
        //PhotonNetwork.AutomaticallySyncScene = false;
        //PhotonNetwork.Disconnect();
        LeanTween.cancelAll();
        //AdsManager.Instance.ShowInterstitialAd();
        SceneManager.LoadScene("Ludo_Home");
    }

    public void PauseNoBtn()
    {
        LudoAudioManager.Instance.onButtonClick();
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        MenuController.Instance.onSetState(StaticData.GameArea);
    }

    /*[PunRPC]
    void RPCPawnSelect(string arg)
    {
        Pawn p = allPawns.Where(x => x.name == arg).FirstOrDefault();
        if (p == null) return;

        selectedPawn = p;
        currentPawnController.HighlightDices(false);
        gameState = GameState.MOVING;
        p.Move(currentDice + 1);
    }*/

    string getMyPawnColor()
    {
        if (isLocal)
        {
            return PlayerPrefs.GetString("tokenColor");
        }
        else
        {
            //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            //{
            //    if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
            //    {
            //        return pawnColors[i];
            //    }
            //}
        }

        return "";
    }

    int getPlayerCount()
    {
        return PlayerPrefs.GetInt("playerCount");
        //if (isLocal)
        //{
        //    return PlayerPrefs.GetInt("playerCount");
        //}
        //else
        //{
        //    return PhotonNetwork.PlayerList.Length;
        //}
    }

    LudoTokenController getMyPawn()
    {
        if (myPlayerColor == "Green") return greenPawn;
        if (myPlayerColor == "Yellow") return yellowPawn;
        if (myPlayerColor == "Blue") return bluePawn;
        if (myPlayerColor == "Red") return redPawn;

        return null;
    }

    void DisableNotActivePawns()
    {
        List<string> colors = pawnColors.ToList();
        List<string> disabledColors = new List<string>();

        int removeCount = 4 - getPlayerCount();

        if (isLocal)
        {
            int rand = Random.Range(0, colors.Count);

            for (int i = 0; i < removeCount; i++)
            {
                rand = Random.Range(0, colors.Count);

                while (colors[rand] == myPlayerColor)
                {
                    rand = Random.Range(0, colors.Count);
                }

                disabledColors.Add(colors[rand]);
                colors.Remove(colors[rand]);
            }
        }
        else
        {
            for (int i = getPlayerCount(); i < 4; i++)
            {
                if (i < colors.Count)
                {
                    disabledColors.Add(colors[i]);
                    colors.RemoveAt(i);
                    i--;
                }
            }
        }

        for (int i = 0; i < disabledColors.Count; i++)
        {
            if (disabledColors[i] == "Green")
            {
                greenPawn.DisablePawn();
            }
            else if (disabledColors[i] == "Yellow")
            {
                yellowPawn.DisablePawn();
            }
            else if (disabledColors[i] == "Blue")
            {
                bluePawn.DisablePawn();
            }
            else if (disabledColors[i] == "Red")
            {
                redPawn.DisablePawn();
            }
        }

        for (int i = 0; i < colors.Count; i++)
        {
            if (colors[i] == "Green")
            {
                activePawnControllers.Add(greenPawn);
                greenPawn.isBot = colors[i] != myPlayerColor;//!PhotonNetwork.IsConnectedAndReady &&
                //if (!isLocal)
                //{
                //    greenPawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                //}
                //else
                {
                    greenPawn.SetUserInfo();
                }
            }
            else if (colors[i] == "Yellow")
            {
                activePawnControllers.Add(yellowPawn);
                yellowPawn.isBot = colors[i] != myPlayerColor;//!PhotonNetwork.IsConnectedAndReady &&
                //if (!isLocal)
                //{
                //    yellowPawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                //}
                //else
                {
                    yellowPawn.SetUserInfo();
                }
            }
            else if (colors[i] == "Blue")
            {
                activePawnControllers.Add(bluePawn);
                bluePawn.isBot = colors[i] != myPlayerColor;//!PhotonNetwork.IsConnectedAndReady && 
                //if (!isLocal)
                //{
                //    bluePawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                //}
                //else
                {
                    bluePawn.SetUserInfo();
                }
            }
            else if (colors[i] == "Red")
            {
                activePawnControllers.Add(redPawn);
                redPawn.isBot = colors[i] != myPlayerColor;//!PhotonNetwork.IsConnectedAndReady && 
                //if (!isLocal)
                //{
                //    redPawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                //}
                //else
                {
                    redPawn.SetUserInfo();
                }
            }
        }

        foreach (LudoTokenController pController in activePawnControllers)
        {
            if (pController != myPawnController)
            {
                pController.DisableColliders();
            }
        }

        SetActivePawn();
    }

    void SetActivePawn()
    {
        if (currentPawnController != null)
        {
            currentPawnController.time = 10;
            currentPawnController.canPlayAgain = false;
        }

        if (isLocal)
        {
            currentPawnController = activePawnControllers[currentPawnID];
        }
        //else
        //{
        //    currentPawnController = getCurrentPawnController();
        //}

        currentPawnController.time = 10;
        currentPawnController.canPlayAgain = false;

        ChangeGameState(GameState.READY);

        if (isLocal)
        {
            if (currentPawnController != myPawnController)
            {
                currentPawnController.StartTimer(true);
                currentPawnController.Play();
            }
            else
            {
                currentPawnController.StartTimer(true);
            }
        }
        else if (currentPawnController == myPawnController)
        {
            currentPawnController.StartTimer(true);
        }
    }

    LudoTokenController getCurrentPawnController()
    {
        int colorID = 0;// (int)PhotonNetwork.MasterClient.CustomProperties["colorID"];

        if (colorID == 0)
        {
            return greenPawn;
        }
        else if (colorID == 1)
        {
            return yellowPawn;
        }
        else if (colorID == 2)
        {
            return bluePawn;
        }
        else if (colorID == 3)
        {
            return redPawn;
        }

        return null;
    }

    public void ChangeGameState(GameState newState)
    {
        gameState = newState;

        if (newState == GameState.FINISHED)
        {
            //if (!isLocal)
            //{
            //    if (PhotonNetwork.IsMasterClient)
            //    {
            //        photonView.RPC("WinnerColorRPC", RpcTarget.OthersBuffered, winnerColor);
            //    }
            //}

            Invoke("FinishedShow", 1f);
        }
    }

    public void CheckForFinish(string color = "")
    {
        if (!string.IsNullOrEmpty(color))
        {
            winnerColor = color;
        }
        else
        {
            winnerColor = isSomeoneFinished();
        }

        if (!string.IsNullOrEmpty(winnerColor))
        {
            ChangeGameState(GameState.FINISHED);
        }
        else
        {
            if (currentPawnController.canPlayAgain)
            {
                currentPawnController.time = 10;
                ChangeGameState(GameState.READY);
                if (currentPawnController != myPawnController && isLocal)
                {
                    currentPawnController.Play();
                }
                return;
            }

            ChangePlayer();
        }
    }

    string isSomeoneFinished()
    {
        LudoToken[] collectedGreenPaws = greenPawns.Where(x => x.isCollected).ToArray();
        LudoToken[] collectedBluePaws = bluePawns.Where(x => x.isCollected).ToArray();
        LudoToken[] collectedYellowPaws = yellowPawns.Where(x => x.isCollected).ToArray();
        LudoToken[] collectedRedPaws = redPawns.Where(x => x.isCollected).ToArray();

        if (collectedGreenPaws.Length == 4)
        {
            return "Green";
        }

        if (collectedYellowPaws.Length == 4)
        {
            return "Yellow";
        }

        if (collectedBluePaws.Length == 4)
        {
            return "Blue";
        }


        if (collectedRedPaws.Length == 4)
        {
            return "Red";
        }

        return "";
    }

    public void ChangePlayer()
    {
        currentPawnController.profileTimeImg.fillAmount = 0;
        currentPawnController.time = 10;
        currentPawnController.canPlayAgain = false;
        currentPawnController.StopAnimation();

        if (isLocal)
        {
            currentPawnID = (currentPawnID + 1) % getPlayerCount();
            SetActivePawn();
        }
        //else
        //{
        //    if (photonView.IsMine && PhotonNetwork.IsMasterClient && currentPawnController == myPawnController)
        //    {
        //        StartCoroutine(SwitchMasterDelay());
        //    }
        //}
    }

    public void CheckGameStatus()
    {
        ChangeGameState(GameState.WAIT);

        StartCoroutine(CheckPawnsForSameWay());
    }

    IEnumerator CheckPawnsForSameWay()
    {
        bool wait = false;

        foreach (LudoTokenController pController in activePawnControllers)
        {
            if (pController == currentPawnController) continue;

            LudoToken[] currentPawns = currentPawnController.token;
            LudoToken[] activePawns = pController.token;

            foreach (LudoToken currentPawn in currentPawns)
            {
                foreach (LudoToken activePawn in activePawns)
                {
                    if (currentPawn.currentWayID != activePawn.currentWayID) continue;
                    if (currentPawn.inBase || activePawn.inBase) continue;
                    if (currentPawn.isProtected || activePawn.isProtected) continue;
                    if (currentPawn.isCollected || activePawn.isCollected) continue;
                    if (currentPawn.inColorWay || activePawn.inColorWay) continue;

                    wait = true;
                    activePawn.ReturnToBase();
                    currentPawnController.canPlayAgain = true;
                }
            }
        }

        if (!wait)
        {
            yield return new WaitForSeconds(0.5f);
            CheckForFinish();
        }
    }

    public void GameDiceBtn(string color)
    {
        if (currentPawnController.tokenColor != color) return;
        if (gameState != GameState.READY) return;
        //if (!isLocal && !PhotonNetwork.IsMasterClient) return;

        gameState = GameState.DICE;
        currentDice = Random.Range(0, 6);
        LudoAudioManager.Instance.onDiceRoll();

        if (isLocal)
        {
            currentPawnController.PlayDiceAnimation();
            LeanTween.value(0, 1, 0.5f).setOnComplete(() =>
            {
                currentPawnController.CheckAvailableMovements(currentDice == 5);
            });
        }
        //else
        //{
        //    photonView.RPC("RPCDice", RpcTarget.AllBuffered, currentDice);
        //}
    }

    /*[PunRPC]
    void WinnerColorRPC(string color)
    {
        CheckForFinish(color);
    }

    IEnumerator SwitchMasterDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
    }

    public void MasterClientChanged()
    {
        SetActivePawn();
    }

    [PunRPC]
    void RPCDice(int arg)
    {
        currentDice = arg;
        currentPawnController.PlayDiceAnimation();

        if (photonView.IsMine)
        {
            currentPawnController.CheckAvailableMovements(currentDice == 5);
        }
    }

    public void CheckRoomPlayers(Player leftPlayer)
    {
        if (gameState == GameState.FINISHED) return;

        int colorID = (int)leftPlayer.CustomProperties["colorID"];

        if (colorID == 0)
        {
            if (activePawnControllers.Contains(greenPawn))
            {
                activePawnControllers.Remove(greenPawn);
            }
            greenPawn.DisablePawn();
        }
        else if (colorID == 1)
        {
            if (activePawnControllers.Contains(yellowPawn))
            {
                activePawnControllers.Remove(yellowPawn);
            }
            yellowPawn.DisablePawn();
        }
        else if (colorID == 2)
        {
            if (activePawnControllers.Contains(bluePawn))
            {
                activePawnControllers.Remove(bluePawn);
            }
            bluePawn.DisablePawn();
        }
        else if (colorID == 3)
        {
            if (activePawnControllers.Contains(redPawn))
            {
                activePawnControllers.Remove(redPawn);
            }
            redPawn.DisablePawn();
        }

        if (PhotonNetwork.PlayerList.Length == 1)
        {
            winnerColor = myPlayerColor;
            ChangeGameState(GameState.FINISHED);
        }
    }*/

    void FinishedShow()
    {
        if (pauseScreen.activeInHierarchy)
        {
            pauseScreen.SetActive(false);
        }

        List<GameObject> activePlayerForPanel = new List<GameObject>();
        //PhotonNetwork.AutomaticallySyncScene = false;
        winParticles.SetActive(true);
        finishedPanel.transform.localScale = Vector3.zero;
        finishedScreen.SetActive(true);
        MenuController.Instance.onSetState(StaticData.LevelFinish);

        GameObject winnerObject = null;

        foreach (LudoTokenController p in activePawnControllers)
        {
            if (p.tokenColor == "Green")
            {
                activePlayerForPanel.Add(finishedPlayersParent.GetChild(0).gameObject);
                finishedPlayersParent.GetChild(0).gameObject.SetActive(true);
                finishedPlayersParent.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = greenPawn.usernameText.text;
                finishedPlayersParent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = greenPawn.avatarImg.sprite;

                if (winnerColor == p.tokenColor)
                {
                    winnerObject = finishedPlayersParent.GetChild(0).gameObject;
                    LeanTween.scale(finishedPlayersParent.GetChild(0).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                }
            }
            else if (p.tokenColor == "Yellow")
            {
                activePlayerForPanel.Add(finishedPlayersParent.GetChild(2).gameObject);
                finishedPlayersParent.GetChild(2).gameObject.SetActive(true);
                finishedPlayersParent.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = yellowPawn.usernameText.text;
                finishedPlayersParent.GetChild(2).GetChild(0).GetComponent<Image>().sprite = yellowPawn.avatarImg.sprite;

                if (winnerColor == p.tokenColor)
                {
                    winnerObject = finishedPlayersParent.GetChild(2).gameObject;
                    LeanTween.scale(finishedPlayersParent.GetChild(2).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                }
            }
            else if (p.tokenColor == "Blue")
            {
                activePlayerForPanel.Add(finishedPlayersParent.GetChild(1).gameObject);
                finishedPlayersParent.GetChild(1).gameObject.SetActive(true);
                finishedPlayersParent.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = bluePawn.usernameText.text;
                finishedPlayersParent.GetChild(1).GetChild(0).GetComponent<Image>().sprite = bluePawn.avatarImg.sprite;

                if (winnerColor == p.tokenColor)
                {
                    winnerObject = finishedPlayersParent.GetChild(1).gameObject;
                    LeanTween.scale(finishedPlayersParent.GetChild(1).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                }
            }
            else if (p.tokenColor == "Red")
            {
                activePlayerForPanel.Add(finishedPlayersParent.GetChild(3).gameObject);
                finishedPlayersParent.GetChild(3).gameObject.SetActive(true);
                finishedPlayersParent.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = redPawn.usernameText.text;
                finishedPlayersParent.GetChild(3).GetChild(0).GetComponent<Image>().sprite = redPawn.avatarImg.sprite;

                if (winnerColor == p.tokenColor)
                {
                    winnerObject = finishedPlayersParent.GetChild(3).gameObject;
                    LeanTween.scale(finishedPlayersParent.GetChild(3).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                }
            }
        }

        LeanTween.scale(finishedPanel, Vector3.one, 0.2f).setEaseOutBack().setOnStart(() =>
        {
            for (int i = 0; i < activePlayerForPanel.Count; i++)
            {
                GameObject g = activePlayerForPanel[i];
                LeanTween.alphaCanvas(g.GetComponent<CanvasGroup>(), 1, 0.5f).setDelay(i * 0.25f);
            }
        });

        int gamePrice = PlayerPrefs.GetInt("entryFee");//PhotonController.Instance.gameEntryPrice();
        int winnerPrice = PlayerPrefs.GetInt("playerCount") * gamePrice;

        if (winnerColor.Equals(myPlayerColor))
        {
            PlayerPrefs.SetInt("ludoCoin", PlayerPrefs.GetInt("ludoCoin") + winnerPrice);
            PlayerPrefs.Save();
        }
        Debug.Log("winnerObject: " + winnerObject.name);
        for (int i = 0; i < activePlayerForPanel.Count; i++)
        {
            GameObject g = activePlayerForPanel[i];
            Transform txt = g.transform.Find("Coin");

            //txt.GetComponent<TMP_Text>().text = gamePrice.ToString("###,###,###");
            if (g == winnerObject)
            {
                LeanTween.value(gamePrice, winnerPrice, 2f).setOnUpdate((float var) =>
                {
                    txt.GetComponent<TMP_Text>().text = var.ToString("###,###");
                });
            }
            else
            {
                LeanTween.value(gamePrice, 0, 2f).setOnUpdate((float var) =>
                {
                    txt.GetComponent<TMP_Text>().text = var.ToString("###,###");
                }).setOnComplete(() =>
                {
                    txt.GetComponent<TMP_Text>().text = "0";
                });
            }
        }
    }

    public void FinishedMenuBtn()
    {
        //PhotonNetwork.AutomaticallySyncScene = false;
        //PhotonNetwork.Disconnect();
        LeanTween.cancelAll();
        //AdsManager.Instance.ShowInterstitialAd();
        SceneManager.LoadScene("Ludo_Home");
    }
}
