using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        Counting,
        Playing,
        Paused,
        Gameover
    }
    public enum Controls
    {
        Keyboard,
        Touch,
    }

    [Header("Components")]
    public GameState gameState;
    public Controls controls;
    public GameObject touchInputs;

    bool sourceFound = false;

    private void Awake()
    {
        instance = this;
        if (controls == Controls.Touch)
        {
            touchInputs.SetActive(true);
        }

        else if (controls == Controls.Keyboard)
        {
            touchInputs.SetActive(false);
        }
    }

    #region Actions
    private void OnEnable()
    {
        PlayerHitDetection.HitObstacles += PlayerHitObstacles;
    }

    private void OnDisable()
    {
        PlayerHitDetection.HitObstacles -= PlayerHitObstacles;
    }

    void PlayerHitObstacles()
    {
        gameState = GameState.Gameover;
    }
    #endregion

    private void Update()
    {
        if (sourceFound)
            return;

        if(BackgroundSoundManager.instance)
        {
            Debug.Log("Finding...");
            BackgroundSoundManager.instance.audioSource.volume = 0.1f;
            sourceFound = true;
        }
    }
}
