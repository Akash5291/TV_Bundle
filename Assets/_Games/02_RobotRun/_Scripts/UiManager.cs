using TMPro;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    GameManager gameManager;
    public static Action CountDownCompleted;

    [Header("Components")]
    [SerializeField] Animator countingTextAnimation;
    [SerializeField] string pulsTag;

    // Start is called before the first frame update
    [Space]
    [Header ("Ui panels")]
    [SerializeField] GameObject informationPanel;
    [SerializeField] GameObject countingPanel;
    [SerializeField] GameObject ingamePanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] GameObject exitPanel;

    [Space]
    [Header("Values")]
    WaitForSeconds oneSecond = new WaitForSeconds(1);

    [Space]
    [Header("User interface")]
    [SerializeField] TMP_Text T_countDown;

    private void Awake() => ChangePanel(informationPanel);

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
        MenuController.Instance.onSetState(StaticData.GameOver);
        ChangePanel(gameoverPanel);
    }
    #endregion

    private void Start()
    {
        Time.timeScale = 1;
        gameManager = GameManager.instance;
        gameManager.gameState = GameManager.GameState.Counting;
    }

    // change panel by closing all the panels and turning on the deisred panel
    void ChangePanel(GameObject _desirePanel)
    {
        CloseAllPanels();
        _desirePanel.SetActive(true);
        if (_desirePanel.name.Equals("Information panel"))
            Invoke("B_OkayInformation", 2f);
    }

    void CloseAllPanels()
    {
        informationPanel.SetActive(false);
        countingPanel.SetActive(false);
        ingamePanel.SetActive(false);
        pausePanel.SetActive(false);
        gameoverPanel.SetActive(false);
        exitPanel.SetActive(false);
    }

    #region InformationPanel
    // close the information panel and start counting panel
    public void B_OkayInformation()
    {
        ChangePanel(countingPanel);
        MenuController.Instance.onSetState(StaticData.GameArea);
        StartCoroutine(nameof(CountingFunction));
    }
    #endregion

    #region CountingPane;
    // start the countdown when the game starts
    IEnumerator CountingFunction()
    {
        CountingFunction("3", true);
        yield return oneSecond;

        CountingFunction("2", true);
        yield return oneSecond;

        CountingFunction("1", true);
        yield return oneSecond;

        CountingFunction("GO...", true);

        yield return oneSecond;
        countingPanel.SetActive(false);
        CountDownCompleted?.Invoke();
        gameManager.gameState = GameManager.GameState.Playing;
        ChangePanel(ingamePanel);
    }

    void CountingFunction (string _count, bool _animation)
    {
        T_countDown.text = _count;
        if (!_animation) 
            return;
        
        countingTextAnimation.SetTrigger(pulsTag);
    }
    #endregion

    #region IngamePanel
    public void B_Pause()
    {
        ChangePanel(pausePanel);
        MenuController.Instance.onSetState(StaticData.GamePause);
        Time.timeScale = 0;
    }

    public void B_Exit()
    {
        ChangePanel(exitPanel);
        Time.timeScale = 0;
    }
    #endregion

    #region PauseMenu
    public  void B_Restart()
    {
        RestartFunction ();
    }

    public void B_Resume()
    {
        ChangePanel(ingamePanel);
        MenuController.Instance.onSetState(StaticData.GameArea);
        Time.timeScale = 1;
    }
    void RestartFunction() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    #endregion

    #region GameoverPanel
    public void B_DirectHomeButton()
    {
        Time.timeScale = 1;
        BackToMainScreen();
    }

    // NOTE: restart button from pause menu
    void BackToMainScreen() => SceneManager.LoadScene("Robot_Run_Main_Menu");
    #endregion

    // NOTE: exit panel have reume button from pause menu and direct exit from gameover panel
}
