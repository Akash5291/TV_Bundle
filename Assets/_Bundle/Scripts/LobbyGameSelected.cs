using TMPro;
using UnityEngine;
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
    string installedPackageName;

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
    }

    private void LobbyGameSelect(Sprite bannerSprite, SerializableClasses.BundleGameData data)
    {
        gameBanner.sprite = bannerSprite;
        gameTitle.text = data.title;
        gameDescription.text = data.description;
        currentGameData = data;
        installedPackageName = null;
        playBtn.SetActive(false);
        downloadBtn.SetActive(false);

        if (string.IsNullOrEmpty(data.download_link))
        {
            // No download link - this game ships inside the app bundle itself.
            playBtn.SetActive(true);
        }
        else
        {
            string packageName = AndroidAppLauncher.ExtractPackageName(data.download_link);
            Debug.LogFormat("Package name : " + packageName);
            if (AndroidAppLauncher.IsAppInstalled(packageName))
            {
                installedPackageName = packageName;
                playBtn.SetActive(true);
            }
            else
            {
                downloadBtn.SetActive(true);
            }
        }

        gameSelectedScreenObj.SetActive(true);
    }

    public void onPlayBtnClick()
    {
        if (!string.IsNullOrEmpty(installedPackageName))
        {
            AndroidAppLauncher.LaunchApp(installedPackageName);
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
