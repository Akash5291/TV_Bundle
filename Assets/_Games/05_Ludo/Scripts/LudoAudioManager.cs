using UnityEngine;

public class LudoAudioManager : MonoBehaviour
{
    public static LudoAudioManager Instance = null;

    [SerializeField] AudioSource btnClick;
    [SerializeField] AudioSource diceRole;
    [SerializeField] AudioSource tokenMove;
    [SerializeField] AudioSource tokenKilled;
    [SerializeField] AudioSource tokenReachLimit;
    [SerializeField] AudioSource enterIntoSafeZone;
    [SerializeField] AudioSource enterIntoColorWay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
            Destroy(gameObject);
    }

    public void onButtonClick()
    {
        btnClick.Play();
    }

    public void onDiceRoll()
    {
        diceRole.Play();
    }

    public void onTokenMove()
    {
        tokenMove.Play();
    }

    public void onTokenKilled()
    {
        tokenKilled.Play();
    }

    public void onTokenReachLimit()
    {
        tokenReachLimit.Play();
    }

    public void onEnterSafeZone()
    {
        enterIntoSafeZone.Play();
    }

    public void onEnterColorWay()
    {
        enterIntoColorWay.Play();
    }
}
