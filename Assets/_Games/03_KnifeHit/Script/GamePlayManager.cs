using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlayManagerNinjaKnife : MonoBehaviour
{
    public static GamePlayManagerNinjaKnife instance;
    [Header("Circle Setting")]
    public Circle[] circlePrefabs;
    public Bosses[] BossPrefabs;

    public Transform circleSpawnPoint;

    [Header("Knife Setting")]
    public Knife knifePrefab;
    public Transform KnifeSpawnPoint;
    [Range(0f, 1f)] public float knifeHeightByScreen = .1f;

    public GameObject ApplePrefab;
    [Header("UI Object")]
    public Text lblPauseScore;
    public Text lblScore;
    public Text lblStage;
    public List<Image> stageIcons;
    public Color stageIconActiveColor;
    public Color stageIconNormalColor;

    [Header("UI Boss")]

    public GameObject bossFightStart;
    public GameObject bossFightEnd;
    public AudioClip[] bossFightStartSounds;
    public AudioClip[] bossFightEndSounds;
    [Header("Ads Show")]
    public GameObject adsShowView;
    public Image adTimerImage;
    public Text adSocreLbl;


    [Header("GameOver Popup")]
    public GameObject gameOverView;
    public Text gameOverSocreLbl, gameOverStageLbl, secondgameOverSocreLbl, gameOverAppleScore;
    public GameObject newBestScore;
    public GameObject TopRightCorner;
    [Space(50)]

    public int cLevel = 0;
    public bool isDebug = false;
    string currentBossName = "";
    Circle currentCircle;
    Knife currentKnife;

    public int totalSpawnKnife;
    public RectTransform safeAreaTransform, rootCanvasTranform;
    [HideInInspector]
    public float knifeScale;

    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        StartGame();

#if UNITY_IOS
        float bottom = safeAreaTransform.rect.height * -5 / rootCanvasTranform.rect.height;
        KnifeSpawnPoint.position = new Vector3(0, bottom + 1 + 1.7f, 0);
#endif
        TopRightCorner.SetActive(true);
    }

    private void OnEnable()
    {

    }

    public void StartGame()
    {
        GameManagerNinjaKnife.score = 0;
        GameManagerNinjaKnife.Stage = 1;
        GameManagerNinjaKnife.isGameOver = false;
        //usedAdContinue = false;
        if (isDebug)
        {
            GameManagerNinjaKnife.Stage = cLevel;
        }
        SetupGame();
    }

    public void UpdateLable()
    {
        lblPauseScore.text = GameManagerNinjaKnife.score + "";
        lblScore.text = GameManagerNinjaKnife.score + "";
        if (GameManagerNinjaKnife.Stage % 5 == 0)
        {
            for (int i = 0; i < stageIcons.Count - 1; i++)
            {
                stageIcons[i].gameObject.SetActive(false);
            }
            stageIcons[stageIcons.Count - 1].color = stageIconActiveColor;
            lblStage.color = stageIconActiveColor;
            lblStage.text = currentBossName;
        }
        else
        {
            lblStage.text = "STAGE " + GameManagerNinjaKnife.Stage;
            for (int i = 0; i < stageIcons.Count; i++)
            {
                stageIcons[i].gameObject.SetActive(true);
                stageIcons[i].color = GameManagerNinjaKnife.Stage % stageIcons.Count <= i ? stageIconNormalColor : stageIconActiveColor;
            }
            lblStage.color = stageIconNormalColor;
        }
    }

    public void SetupGame()
    {
        SpawnCircle();
        KnifeCounter.intance.setUpCounter(currentCircle.totalKnife);

        totalSpawnKnife = 0;
        StartCoroutine(GenerateKnife());
    }

    public void OnKnifeThrow()
    {
        if (currentKnife == null)
            return;

        if (!currentKnife.isFire && !UIManager.Instance.isGamePaused)
        {
            KnifeCounter.intance.setHitedKnife(totalSpawnKnife);
            currentKnife.ThrowKnife();
            //StartCoroutine(GenerateKnife());
        }
    }

    public void SpawnCircle()
    {
        GameObject tempCircle;
        if (GameManagerNinjaKnife.Stage % 5 == 0)
        {
            Bosses b = BossPrefabs[Random.Range(0, BossPrefabs.Length)];
            tempCircle = Instantiate(b.BossPrefab, circleSpawnPoint.position, Quaternion.identity, circleSpawnPoint).gameObject;
            currentBossName = "Boss : " + b.Bossname;
            UpdateLable();
            OnBossFightStart();
        }
        else
        {
            var index = GameManagerNinjaKnife.Stage > 50 ? Random.Range(11, circlePrefabs.Length - 1) : GameManagerNinjaKnife.Stage - 1;
            tempCircle = Instantiate(circlePrefabs[index], circleSpawnPoint.position, Quaternion.identity, circleSpawnPoint).gameObject;
        }

        float circleScale = (GameManagerNinjaKnife.ScreenWidth * 0.5f) / tempCircle.GetComponent<SpriteRenderer>().bounds.size.x;
        circleScale = Mathf.Min(circleScale, 1);

        tempCircle.transform.localScale = Vector3.one * .2f;
        LeanTween.scale(tempCircle, new Vector3(circleScale, circleScale, circleScale), .3f).setEaseOutBounce();
        currentCircle = tempCircle.GetComponent<Circle>();
        currentCircle.circleScale = circleScale;
    }

    public IEnumerator OnBossFightStart()
    {
        bossFightStart.SetActive(true);
        SoundManager.instance.PlaySingle(bossFightStartSounds[Random.Range(0, bossFightEndSounds.Length - 1)], 1f);
        yield return new WaitForSeconds(2f);
        bossFightStart.SetActive(false);
        SetupGame();
    }

    public IEnumerator OnBossFightEnd()
    {
        bossFightEnd.SetActive(true);
        SoundManager.instance.PlaySingle(bossFightEndSounds[Random.Range(0, bossFightEndSounds.Length - 1)], 1f);
        yield return new WaitForSeconds(2f);
        bossFightEnd.SetActive(false);
        SetupGame();
    }

    public IEnumerator GenerateKnife()
    {
        yield return new WaitUntil(() =>
        {
            return KnifeSpawnPoint.childCount == 0;
        });
        Debug.Log("knife spawn point count: 0");
        if (currentCircle.totalKnife > totalSpawnKnife && !GameManagerNinjaKnife.isGameOver)
        {
            totalSpawnKnife++;
            var prefab = GameManagerNinjaKnife.selectedKnifePrefab ?? knifePrefab;
            GameObject tempKnife = Instantiate(prefab, KnifeSpawnPoint.position + Vector3.down * 2f, Quaternion.identity, KnifeSpawnPoint).gameObject;
            
            knifeScale = (GameManagerNinjaKnife.ScreenHeight * knifeHeightByScreen) / tempKnife.GetComponent<SpriteRenderer>().bounds.size.y;
            tempKnife.transform.localScale = Vector3.one * knifeScale;
            LeanTween.moveLocalY(tempKnife, 0, 0.1f);
            tempKnife.name = "Knife" + totalSpawnKnife;
            currentKnife = tempKnife.GetComponent<Knife>();
            Debug.Log("new knife generated");
        }

    }

    public void NextLevel()
    {
        Debug.Log("Next Level");
        if (currentCircle != null)
        {
            currentCircle.DestroyMeAndAllKnives();
        }
        if (GameManagerNinjaKnife.Stage % 5 == 0)
        {
            GameManagerNinjaKnife.Stage++;
            StartCoroutine(OnBossFightEnd());

        }
        else
        {
            GameManagerNinjaKnife.Stage++;
            if (GameManagerNinjaKnife.Stage % 5 == 0)
            {
                StartCoroutine(OnBossFightStart());
            }
            else
            {
                Invoke("SetupGame", .3f);
            }
        }
    }

    IEnumerator currentShowingAdsPopup;
    public void GameOver()
    {
        GameManagerNinjaKnife.isGameOver = true;

     //   if (usedAdContinue || !IsAdAvailable())
      //  {
            ShowGameOverPopup();
      //  }
       // else
      //  {
          //  currentShowingAdsPopup = ShowAdPopup();
           // StartCoroutine(currentShowingAdsPopup);
       // }
    }

    public IEnumerator ShowAdPopup()
    {
        adsShowView.SetActive(true);
        adSocreLbl.text = GameManagerNinjaKnife.score + "";
        SoundManager.instance.PlayTimerSound();
        for (float i = 1f; i > 0; i -= 0.01f)
        {
            adTimerImage.fillAmount = i;
            yield return new WaitForSeconds(0.1f);
        }
        CancleAdsShow();
        SoundManager.instance.StopTimerSound();
    }

    public void OnShowAds()
    {
     //   doneWatchingAd = false;

        SoundManager.instance.StopTimerSound();
        SoundManager.instance.PlaybtnSfx();
        //usedAdContinue = true;
        StopCoroutine(currentShowingAdsPopup);

//#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
//        AdmobController.instance.ShowRewardBasedVideo();
//#else
//       // HandleRewardBasedVideoRewarded(null, null);
//#endif
    }

    public void AdShowSucessfully()
    {
        adsShowView.SetActive(false);
        totalSpawnKnife--;
        GameManagerNinjaKnife.isGameOver = false;
        KnifeCounter.intance.setHitedKnife(totalSpawnKnife);
        if (KnifeSpawnPoint.childCount == 0)
        {
            StartCoroutine(GenerateKnife());
        }
    }

    public void CancleAdsShow()
    {
        SoundManager.instance.StopTimerSound();
        SoundManager.instance.PlaybtnSfx();
        StopCoroutine(currentShowingAdsPopup);
        adsShowView.SetActive(false);
        ShowGameOverPopup();
    }

    public void ShowGameOverPopup()
    {
        gameOverView.SetActive(true);
        gameOverSocreLbl.text = GameManagerNinjaKnife.score + "";
        secondgameOverSocreLbl.text = GameManagerNinjaKnife.score + "";
        gameOverAppleScore.text = GameManagerNinjaKnife.Apple + "";
        gameOverStageLbl.text = "Stage " + GameManagerNinjaKnife.Stage;
        TopRightCorner.SetActive(false);

        if (GameManagerNinjaKnife.score >= GameManagerNinjaKnife.HighScore)
        {
            GameManagerNinjaKnife.HighScore = GameManagerNinjaKnife.score;
            newBestScore.SetActive(true);
        }
        else
        {
            newBestScore.SetActive(false);
        }
        Invoke("sendGameOverMsg", 0.5f);
        //CUtils.ShowInterstitialAd();
    }

    void sendGameOverMsg()
    {
        PlayerPrefs.SetInt("score", GameManagerNinjaKnife.score);
        MenuController.Instance.onSetState(StaticData.GameOver);
    }

    public void OpenShop()
    {
        SoundManager.instance.PlaybtnSfx();
        KnifeShop.intance.ShowShop();
    }

    public void RestartGame()
    {
        SoundManager.instance.PlaybtnSfx();
        //GeneralFunction.intance.LoadSceneByName("GameScene");
        SceneManager.LoadScene("Ninja_Knife_Game_Scene");
    }

    public void BackToHome()
    {
        SoundManager.instance.PlaybtnSfx();
        //GeneralFunction.intance.LoadSceneByName("HomeScene");
        SceneManager.LoadScene("Ninja_Knife_Main_Scene");


    }

    public void FBClick()
    {
        SoundManager.instance.PlaybtnSfx();
        StartCoroutine(CROneStepSharing());
    }

    public void ShareClick()
    {
        SoundManager.instance.PlaybtnSfx();
        StartCoroutine(CROneStepSharing());
    }

    public void SettingClick()
    {
        SoundManager.instance.PlaybtnSfx();
        SettingUI.intance.ShowUI();
    }

    IEnumerator CROneStepSharing()
    {
        yield return new WaitForEndOfFrame();
        //Sharing.ShareScreenshot("screenshot", "");
    }
}

[System.Serializable]
public class Bosses
{
    public string Bossname;
    public Circle BossPrefab;
}
