using UnityEngine;

public class PlayFootSound : MonoBehaviour
{
    [SerializeField] AudioSource footSoundSource;

    [SerializeField] AudioClip footSoundClip;

    public void B_PlayFootSound()
    {
        footSoundSource.PlayOneShot(footSoundClip);
    }
}
