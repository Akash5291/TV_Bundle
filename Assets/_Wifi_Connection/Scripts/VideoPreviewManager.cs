using UnityEngine;
using UnityEngine.Video;

public class VideoPreviewManager : MonoBehaviour
{
    [SerializeField] VideoPlayer player;

    private void Start()
    {
        //startPlayer();
    }

    private void OnEnable()
    {
        startPlayer();
    }

    void startPlayer()
    {
        Debug.Log("startPlayer");
        foreach (var ad in APIManager.Instance.inHouseAds.ourAds)
        {
            if (ad.game_bundle_name.Equals(Application.identifier))
                player.url = ad.preview_video_url;
        }

        if (!player.isPlaying)
            player.Play();
    }

    public void onCloseBtn()
    {
        if (player.isPlaying)
            player.Stop();

        player.targetTexture.Release();
        transform.gameObject.SetActive(false);
    }
}
