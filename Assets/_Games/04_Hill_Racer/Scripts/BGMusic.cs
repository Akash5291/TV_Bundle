
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public static BGMusic Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    [SerializeField] AudioSource bg;

    public void pauseBGSond(bool value)
    {
        if (value) bg.mute = true;
        else bg.mute = false;
    }
}
