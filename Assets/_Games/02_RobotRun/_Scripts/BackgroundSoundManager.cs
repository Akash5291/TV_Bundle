using UnityEngine;

public class BackgroundSoundManager : MonoBehaviour
{
    public static BackgroundSoundManager instance;

    public AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
