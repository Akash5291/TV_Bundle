using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyGameSelected : MonoBehaviour
{
    [SerializeField] GameObject gameSelectedScreenObj;
    [SerializeField] Image gameBanner;
    [SerializeField] TMP_Text gameTitle;
    [SerializeField] TMP_Text gameDescription;
    [SerializeField] GameObject playBtn;
    [SerializeField] GameObject downloadBtn;

    SerializableClasses.BundleGameData currentGameData;
    GameObject previouslySelected;

    private void OnEnable()
    {
        ActionContainer.onGameSelectToPlay += LobbyGameSelect;
    }

    private void OnDisable()
    {
        ActionContainer.onGameSelectToPlay -= LobbyGameSelect;
    }

    public void backFromGameSelected()
    {
        gameSelectedScreenObj.SetActive(false);
        EventSystem.current?.SetSelectedGameObject(previouslySelected);
    }

    private void LobbyGameSelect(Sprite bannerSprite, SerializableClasses.BundleGameData data)
    {
        gameBanner.sprite = bannerSprite;
        gameTitle.text = data.title;
        gameDescription.text = data.description;
        currentGameData = data;
        playBtn.SetActive(false);
        downloadBtn.SetActive(false);

        // No download link means this game ships inside the app bundle itself;
        // otherwise install status was already resolved when the lobby item
        // was set up (see GameBundleItem.setGameBundleData).
        bool showPlay = string.IsNullOrEmpty(data.download_link) || data.isInstalled;
        playBtn.SetActive(showPlay);
        downloadBtn.SetActive(!showPlay);

        previouslySelected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        gameSelectedScreenObj.SetActive(true);
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
        }
    }

    public void onDownloadBtnClick()
    {
        if (currentGameData != null && !string.IsNullOrEmpty(currentGameData.download_link))
            Application.OpenURL(currentGameData.download_link);
    }
}
