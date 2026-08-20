using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    public GameObject PauseScreen;

    public bool isGamePaused = false;

    private void Start()
    {
        Instance = this;
    }

    public void PauseButton()
    {
        SoundManager.instance.PlaybtnSfx();
        PauseScreen.SetActive(true);

        MenuController.Instance.onSetState(StaticData.GamePause);
        isGamePaused = true;

        Time.timeScale = 0f;
    }

    public void ResumeButton()
    {
        
        PauseScreen.SetActive(false);

        isGamePaused = false;

        Time.timeScale = 1f;
        MenuController.Instance.onSetState(StaticData.GameArea);
        SoundManager.instance.PlaybtnSfx();
    }

    public void HomeButton()
    {
      
        PauseScreen.SetActive(false);
        SoundManager.instance.PlaybtnSfx();
        isGamePaused = false;

        Time.timeScale = 1f;
       

        SceneManager.LoadScene("Ninja_Knife_Main_Scene");
    }
}
