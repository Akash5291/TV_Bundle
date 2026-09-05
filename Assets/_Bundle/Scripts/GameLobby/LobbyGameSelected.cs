using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyGameSelected : MonoBehaviour
{
    [SerializeField] GameObject gameSelectedScreenObj;
    [SerializeField] Image gameBanner;
    [SerializeField] TMP_Text gameTitle;
    [SerializeField] TMP_Text gameDescription;
    [SerializeField] GameObject playBtn;
    [SerializeField] GameObject downloadBtn;
    [SerializeField] BackgroundFocusLock backgroundLock;
    [SerializeField] Sprite loadingSprite;
    [SerializeField] Image ss_1_img;
    [SerializeField] Image ss_2_img;
    [SerializeField] Image ss_3_img;

    SerializableClasses.BundleGameData currentGameData;
    GameObject previouslySelected;

    private void OnEnable()
    {
        ActionContainer.onGameSelectToPlay += LobbyGameSelect;
    }

    private void OnDestroy()
    {
        ActionContainer.onGameSelectToPlay -= LobbyGameSelect;
    }

    public void backFromGameSelected()
    {
        EventSystem.current?.SetSelectedGameObject(previouslySelected);
        ss_1_img.sprite = loadingSprite;
        ss_2_img.sprite = loadingSprite;
        ss_3_img.sprite = loadingSprite;
        gameSelectedScreenObj.SetActive(false);
        backgroundLock?.SetLocked(false);
    }

    private void LobbyGameSelect(Sprite bannerSprite, SerializableClasses.BundleGameData data)
    {
        gameSelectedScreenObj.SetActive(true);
        gameBanner.sprite = bannerSprite;
        gameTitle.text = data.title;
        gameDescription.text = data.description;
        currentGameData = data;
        BundleAPIManager.Instance.currentGame = data;
        playBtn.SetActive(false);
        downloadBtn.SetActive(false);

        ss_1_img.enabled = true;
        ss_2_img.enabled = true;
        ss_3_img.enabled = true;

        if (!string.IsNullOrEmpty(data.ss_1))
            ss_1_img.GetComponent<DownloadAndSetImage>().onSetScreenShot(data.ss_1);
        else
            ss_1_img.enabled = false;

        if (!string.IsNullOrEmpty(data.ss_2))
            ss_2_img.GetComponent<DownloadAndSetImage>().onSetScreenShot(data.ss_2);
        else
            ss_2_img.enabled = false;

        if (!string.IsNullOrEmpty(data.ss_3))
            ss_3_img.GetComponent<DownloadAndSetImage>().onSetScreenShot(data.ss_3);
        else
            ss_3_img.enabled = false;

        // No download link means this game ships inside the app bundle itself;
        // otherwise install status was already resolved when the lobby item
        // was set up (see GameBundleItem.setGameBundleData).
        bool showPlay = string.IsNullOrEmpty(data.download_link) || data.isInstalled;
        playBtn.SetActive(showPlay);
        downloadBtn.SetActive(!showPlay);

        previouslySelected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        backgroundLock?.SetLocked(true);
        EventSystem.current?.SetSelectedGameObject(showPlay ? playBtn : downloadBtn);
    }

    public void onPlayBtnClick()
    {
        if (currentGameData != null && currentGameData.isInstalled && !string.IsNullOrEmpty(currentGameData.installedPackageName))
        {
            AndroidAppLauncher.LaunchApp(currentGameData.installedPackageName);
        }
        else
        {
            // Bundled game: hook up its in-app entry point here once defined.
            PlayerPrefs.SetString("SelectedGameID", currentGameData.game_id);
            SceneManager.LoadSceneAsync("GamePairingScreen");
        }
    }

    public void onDownloadBtnClick()
    {
        if (currentGameData != null && !string.IsNullOrEmpty(currentGameData.download_link))
            Application.OpenURL(currentGameData.download_link);
    }
}
