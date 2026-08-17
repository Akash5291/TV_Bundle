using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMAnager : MonoBehaviour
{
    public GameObject obstacleSpawner;
    public GameObject BridgeSpawner;
    public GameObject GamePlayButtons;
    public GameObject MainMenuPanel;
    public GameObject GameOverPanel;
    public GameObject PausePanel;
    public GameObject pauseBtn;
    public GameObject ShopPanel;
    public GameObject instruction;
    public bool distancebool = false;
    public static UIMAnager instance;

    public BulletCounter BulletCounter;

    private void Start()
    {
        instance = this;
    }

    public void Play()
    {
        MenuController.Instance.onSetState(StaticData.GameArea);
        obstacleSpawner.SetActive(true);
        GamePlayButtons.SetActive(true);
        MainMenuPanel.SetActive(false);
        pauseBtn.SetActive(true);
        GameOverPanel.SetActive(false);
        PausePanel.SetActive(false);
        distancebool = true;
        instruction.SetActive(true);
        Invoke("DeactivateInstruction", 3f);
    }

    public void Pause()
    {
        MenuController.Instance.onSetState(StaticData.GamePause);
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
        pauseBtn.SetActive(false);
    }

    public void Restart()
    {
        Obstacles.instance.lastInstantiatedObjectIndex = -1;
        Obstacles.instance.timeBtwSpawn = 0;
        BulletCounter.currentBullets = 3;
        LoopingBackground.instance.speed = 0.25f;
        BulletCounter.bulletText.text = BulletCounter.currentBullets.ToString();
        Distance.instance.distance = 0f;
        for (int i = 0; i < obstacleSpawner.transform.childCount; i++)
        {
            Destroy(obstacleSpawner.transform.GetChild(i).gameObject);
        }
        for (int j = 0; j < BridgeSpawner.transform.childCount; j++)
        {
            Destroy(BridgeSpawner.transform.GetChild(j).gameObject);
        }
        Play();
        GameOver.instance.DeactivatePlayer(true);
        distancebool = true;
        Time.timeScale = 1f;
        GameOver.instance.gameOver = false;
        GameOver.instance.DeactivateScore(true);
        GameOver.instance.capsuleCollider2D.enabled = true;
        MenuController.Instance.onSetState(StaticData.GameArea);
    }

    public void Home()
    {
        MenuController.Instance.onSetState(StaticData.Home);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        MainMenuPanel.SetActive(true);
        obstacleSpawner.SetActive(false);
        GamePlayButtons.SetActive(false);
        GameOverPanel.SetActive(false);
        PausePanel.SetActive(false);
        pauseBtn.SetActive(false);
        Time.timeScale = 1f;
        GameOver.instance.DeactivateScore(true);
        GameOver.instance.capsuleCollider2D.enabled = true;
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
        pauseBtn.SetActive(true);
        MenuController.Instance.onSetState(StaticData.GameArea);
    }

    public void GameOverUI()
    {
        MenuController.Instance.onSetState(StaticData.GameOver);
        GameOverPanel.SetActive(true);
        pauseBtn.SetActive(false);
        GamePlayButtons.SetActive(false);
        PausePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }

    public void Shop()
    {
        MenuController.Instance.onSetState(StaticData.SelectionScreen);
        ShopPanel.SetActive(true);
    }

    void DeactivateInstruction()
    {
        instruction.SetActive(false);
    }
}
