using UnityEngine;

public class InGameSoundManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject jetPackSound;

    [SerializeField] AudioClip coinSound;
    [SerializeField] AudioClip dieSound;
    [SerializeField] AudioClip clickSound;

    private void OnEnable()
    {
        PlayerHitDetection.CoinCollected += PlayCoinSound;
        PlayerHitDetection.HitObstacles += PlayDeadSound;
    }

    private void OnDisable()
    {
        PlayerHitDetection.CoinCollected -= PlayCoinSound;
        PlayerHitDetection.HitObstacles -= PlayDeadSound;
    }

    void PlayCoinSound()
    {
        audioSource.PlayOneShot(coinSound);
    }

    void PlayDeadSound()
    {
        audioSource.PlayOneShot(dieSound);
    }

    public void B_PlayButtonSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void PlayJetSound() => jetPackSound.SetActive(true);
    public void StopJetSound() => jetPackSound.SetActive(false);
}
