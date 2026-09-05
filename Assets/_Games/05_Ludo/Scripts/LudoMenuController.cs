using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LudoMenuController : MonoBehaviour
{
    [SerializeField] int entryFee = 75;

    [SerializeField] TMP_Text coinTxt;
    [Header("Pawn Selector")]
    [SerializeField] GameObject colorSelectScreen;
    [SerializeField] GameObject colorSelectPanel;

    [Header("Player Searching")]
    [SerializeField] GameObject searchingScreen;

    [Header("Player Count")]
    [SerializeField] GameObject playerCountScreen;
    [SerializeField] GameObject playerCountPanel;
    [SerializeField] TMP_Text playerCountEntryFee;

    private void Start()
    {
        PlayerPrefs.SetInt("entryFee", 0);
        PlayerPrefs.SetInt("playerCount", 0);
        PlayerPrefs.SetString("tokenColor", "");
        PlayerPrefs.SetInt("ludoCoin", PlayerPrefs.GetInt("ludoCoin", 500));

        coinTxt.text = PlayerPrefs.GetInt("ludoCoin").ToString();
    }

    public void MainVsOnlineBtn()
    {
        LudoAudioManager.Instance.onButtonClick();
        PlayerPrefs.SetString("mode", "computer");
        PlayerPrefs.SetString("isOnline", "true");
        PlayerPrefs.Save();
        MenuController.Instance.onSetState(StaticData.SelectionScreen);
        ColorSelectShow();
    }

    public void MainVsComputerBtn()
    {
        LudoAudioManager.Instance.onButtonClick();
        PlayerPrefs.SetString("mode", "computer");
        PlayerPrefs.SetString("isOnline", "false");
        PlayerPrefs.Save();
        MenuController.Instance.onSetState(StaticData.SelectionScreen);
        ColorSelectShow();
    }

    #region Color Select
    void ColorSelectShow()
    {
        colorSelectPanel.transform.localScale = Vector2.zero;
        colorSelectScreen.SetActive(true);

        LeanTween.scale(colorSelectPanel, Vector2.one, 0.2f).setEaseOutBack();
    }

    public void ColorSelectItemBtn(string pawnColor)
    {
        LudoAudioManager.Instance.onButtonClick();
        PlayerPrefs.SetString("tokenColor", pawnColor);
        PlayerPrefs.Save();
        ColorSelectCloseBtn();
    }

    public void ColorSelectCloseBtn()
    {
        //if (LeanTween.isTweening(colorSelectPanel)) return;
        CloseSelectClose();
    }

    void CloseSelectClose()
    {
        LeanTween.scale(colorSelectPanel, Vector2.zero, 0.2f).setEaseInBack().setOnComplete(() => {
            colorSelectScreen.SetActive(false);

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("tokenColor")))
            {
                PlayerCountShow();
            }
            else
                MenuController.Instance.onSetState(StaticData.Home);
        });
    }
    #endregion

    #region Player Count
    void PlayerCountShow()
    {
        playerCountPanel.transform.localScale = Vector2.zero;
        playerCountEntryFee.text = entryFee.ToString();
        playerCountScreen.SetActive(true);
        MenuController.Instance.onSetState(StaticData.LevelScreen);

        LeanTween.scale(playerCountPanel, Vector2.one, 0.2f).setEaseOutBack();
    }

    public void PlayerCountItemBtn(int playerCount)
    {
        LudoAudioManager.Instance.onButtonClick();
        if (PlayerPrefs.GetInt("ludoCoin") < entryFee)
        {
            Debug.Log("Not enough coin");
            playerCountScreen.SetActive(false);
            //StoreShow();
            return;
        }
        else
        {
            //PlayerPrefs.SetInt("ludoCoin", PlayerPrefs.GetInt("ludoCoin") - entryFee);
            coinTxt.text = PlayerPrefs.GetInt("ludoCoin").ToString();
        }
        PlayerPrefs.SetInt("entryFee", entryFee);
        PlayerPrefs.SetInt("playerCount", playerCount);
        PlayerPrefs.Save();
        if (PlayerPrefs.GetString("isOnline").Equals("true"))
        {
            playerCountScreen.SetActive(false);
            searchingScreen.SetActive(true);
            MenuController.Instance.onSetState(StaticData.LoadingScreen);
            //OnlineShow();
            //PhotonController.Instance.Connect();
        }
        else
        {
            SceneManager.LoadScene("Ludo_Game");
        }
    }

    public void PlayerCountCloseBtn()
    {
        if (LeanTween.isTweening(playerCountPanel)) return;

        PlayerPrefs.SetString("tokenColor", "");
        PlayerCountClose();
    }

    void PlayerCountClose()
    {
        LeanTween.scale(playerCountPanel, Vector2.zero, 0.2f).setEaseInBack().setOnComplete(() => {
            playerCountScreen.SetActive(false);
            MenuController.Instance.onSetState(StaticData.Home);
        });
    }
    #endregion
}
