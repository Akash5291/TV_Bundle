using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoStreamPlayer : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    //public AudioSource audioSource;

    void Start()
    {
        // Set video URL
        string videoURL = "https://playwifigames.com/TV_Video/nexbox_TV_Onboarding.mp4";
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoURL;

        // Assign Audio
        //videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //videoPlayer.SetTargetAudioSource(0, audioSource);

        // Prepare the video
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnPrepared;
    }

    void OnPrepared(VideoPlayer vp)
    {
        rawImage.texture = vp.texture;
        vp.Play();
        //audioSource.Play();
    }
}
