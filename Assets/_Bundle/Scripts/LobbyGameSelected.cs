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
        playBtn.SetActive(false);
        downloadBtn.SetActive(false);

        if (!string.IsNullOrEmpty(data.download_link))
            downloadBtn.SetActive(true);
        else
            playBtn.SetActive(true);

            gameSelectedScreenObj.SetActive(true);
    }

    public void onPlayBtnClick()
    {

    }

    public void onDownloadBtnClick() 
    {

    }
}
