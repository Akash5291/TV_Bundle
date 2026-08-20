using UnityEngine;

public class BGSoundManager : MonoBehaviour
{
    public static BGSoundManager Instance = null;

    [SerializeField] AudioSource bgAudio;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void onBGSoundPlay(bool value)
    {
        if (!value)
            bgAudio.volume = 0f;
        else
            bgAudio.volume = 0.8f;
    }
}
