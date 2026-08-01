using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[Serializable]
public class UIButtons
{
    public List<Button> buttonsColumn = new List<Button>();
}

[Serializable]
public class UIWindow
{
    public string windowName;
    public List<UIButtons> buttonsRow = new List<UIButtons>();
}

public class MenuController : MonoBehaviour
{

    public static MenuController Instance = null;

    [SerializeField] Button[] gameAreaBtn;
    public UIWindow[] uiButtons;

    string currentState = "";
    Button currentBtn;
    int screenIndex = 0;
    int currentRow = 0;
    int currentClm = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        MyController.onNextButton += onNext;
        MyController.onPreviousButton += onPrevious;
        MyController.onUpButton += onUp;
        MyController.onDownButton += onDown;
        MyController.onSelectButton += onSelect;
        MyController.onGameButton += onGameButtonHit;
    }

    private void OnDisable()
    {
        MyController.onNextButton -= onNext;
        MyController.onPreviousButton -= onPrevious;
        MyController.onUpButton -= onUp;
        MyController.onDownButton -= onDown;
        MyController.onSelectButton -= onSelect;
        MyController.onGameButton -= onGameButtonHit;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            onSetState(StaticData.Home);
        else if (SceneManager.GetActiveScene().buildIndex > 1)// game mode
            onSetState(StaticData.GameArea);
    }

    public void setAdsGameDownloadButton()
    {
        var adsCanvas = GameObject.Find("Ads-Canvas");
        UIButtons adsBtn = new UIButtons();
        for (int i = 0; i < adsCanvas.GetComponent<AdsManager>().buttons.Count; i++)
        {
            adsBtn.buttonsColumn.Add(adsCanvas.GetComponent<AdsManager>().buttons[i]);
        }
        uiButtons[0].buttonsRow.Add(adsBtn);
    }

    int getScreenIndex(string name)
    {
        int n = -1;
        for (int i = 0; i < uiButtons.Length; i++)
        {
            if (string.Equals(uiButtons[i].windowName, name))
                n = i;
        }
        return n;
    }

    public void onSetState(string state)
    {
        if (currentState.Equals(state))
            return;

        if (state.Equals("Home"))
            APIManager.Instance.UserProfileObj.SetActive(true);
        else
            APIManager.Instance.UserProfileObj.SetActive(false);

        currentState = state;
        switch (state)
        {
            case "Home":
                {
                    //BGMusic.Instance.pauseBGSond(false);
                    onSetCurrentButtonDetails(getScreenIndex(StaticData.Home), 0, 0);// parameters = screenName, btn default row index, btn default column index
                    if (MyController.Instance.isGameStart)
                        MyController.Instance.sendMessage(StaticData.Home, StaticData.Home);
                }
                break;
            case "Selection":
                {
                    onSetCurrentButtonDetails(getScreenIndex(StaticData.SelectionScreen), 0, 0);
                    MyController.Instance.sendMessage(StaticData.SelectionScreen, StaticData.SelectionScreen);
                }
                break;
            case "Tutorial":
                {
                    onSetCurrentButtonDetails(getScreenIndex("Tutorial"), 0, 0);
                    MyController.Instance.sendMessage("Tutorial", "Tutorial");
                }
                break;
            case "LevelFinish":
                {
                    onSetCurrentButtonDetails(getScreenIndex(StaticData.LevelFinish), 0, 0);
                    MyController.Instance.sendMessage(StaticData.LevelFinish, "");
                }
                break;
            case "GameScene":
                {
                    //BGMusic.Instance.pauseBGSond(true);
                    //onSetCurrentButtonDetails(getScreenIndex(StaticData.GameArea), 0, 0);
                    MyController.Instance.sendMessage(StaticData.GameArea, StaticData.GameArea);
                }
                break;
            case "GamePause":
                {
                    onSetCurrentButtonDetails(getScreenIndex(StaticData.GamePause), 0, 0);
                    MyController.Instance.sendMessage(StaticData.GamePause, StaticData.GamePause);
                }
                break;
            case "Level":
                {
                    onSetCurrentButtonDetails(getScreenIndex(StaticData.LevelScreen), 0, 0);
                    MyController.Instance.sendMessage(StaticData.LevelScreen, "");
                }
                break;
            case "GameOver":
                {
                    onSetCurrentButtonDetails(getScreenIndex(StaticData.GameOver), 0, 0);
                    MyController.Instance.sendMessage(StaticData.GameOver, "");
                }
                break;
        }
    }

    void onSetCurrentButtonDetails(int screenRowIdx, int r, int c)
    {
        screenIndex = screenRowIdx;
        currentRow = r;
        currentClm = c;
        Debug.Log("screenIndex: " + screenIndex + ", row: " + r + ", clm: " + c);
        currentBtn = uiButtons[screenIndex].buttonsRow[currentRow].buttonsColumn[currentClm];
        Debug.Log("currentBtn: " + currentBtn.name);
        currentBtn.Select();
    }

    private void onUp()
    {
        if (currentRow != 0)
        {
            currentRow--;
            if (currentClm < uiButtons[screenIndex].buttonsRow[currentRow].buttonsColumn.Count)
                onSetCurrentButtonDetails(screenIndex, currentRow, currentClm);
            else
                onSetCurrentButtonDetails(screenIndex, currentRow, 0);
        }
    }

    private void onDown()
    {
        if ((currentRow + 1) < uiButtons[screenIndex].buttonsRow.Count)
        {
            currentRow++;
            if (currentClm < uiButtons[screenIndex].buttonsRow[currentRow].buttonsColumn.Count)
                onSetCurrentButtonDetails(screenIndex, currentRow, currentClm);
            else
                onSetCurrentButtonDetails(screenIndex, currentRow, 0);
        }
    }

    private void onSelect()
    {
        if (currentBtn.interactable)
        {
            currentBtn.transform.GetComponent<SelectAnimation>().onSelect(false);
            currentBtn.onClick.Invoke();
        }
    }

    private void onPrevious()
    {
        if ((currentClm - 1) >= 0)
        {
            currentClm--;
            onSetCurrentButtonDetails(screenIndex, currentRow, currentClm);
        }
    }

    private void onNext()
    {
        if ((currentClm + 1) < uiButtons[screenIndex].buttonsRow[currentRow].buttonsColumn.Count)
        {
            currentClm++;
            onSetCurrentButtonDetails(screenIndex, currentRow, currentClm);
        }
    }

    private void onGameButtonHit(string msg)
    {
        Controller controllerDataReceived = JsonUtility.FromJson<Controller>(msg.Trim());
        string btnName = controllerDataReceived.buttonName;
        Debug.Log("Button Name: " + btnName);

        for (int i = 0; i < gameAreaBtn.Length; i++)
        {
            if (btnName.Equals(gameAreaBtn[i].name))
            {
                /*if (!controllerDataReceived.isPointerUP && !btnName.Equals("Pause"))
                    CarInput.Instance.ReleaseGasBrake();
                else
                {
                    if (btnName.Equals("Speed"))
                        CarInput.Instance.Gas();
                    else if (btnName.Equals("Brake"))
                        CarInput.Instance.Brake();
                    else
                        gameAreaBtn[i].onClick.Invoke();
                }*/

                break;
            }
        }
    }

    private void Update()
    {
        if (currentBtn != null && !currentState.Equals(StaticData.GameArea))
        {
            if(!currentBtn.transform.GetComponent<SelectAnimation>().isSelect)
                currentBtn.transform.GetComponent<SelectAnimation>().isSelect = true;
        }
    }
}
