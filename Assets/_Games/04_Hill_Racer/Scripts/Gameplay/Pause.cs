using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{

	// Use this for initialization
	public GameObject PauseMen;


	public string menuLevelName = "Hill_Racer_Main_Menu";

	public Text loadingText;
	// Update is called once per frame
	public void Pausing()
	{
		MenuController.Instance.onSetState(StaticData.GamePause);
		Time.timeScale = 0f;
		PauseMen.SetActive (true);
	}

	public void Resume ()
	{
		Time.timeScale = 1f;
		MenuController.Instance.onSetState(StaticData.GameArea);
		PauseMen.SetActive (false);
	}

	public void Retry ()
	{
		Time.timeScale = 1f;
		WifiManager.Instance.LoadingObj.SetActive(true);
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void Exit ()
	{
		//loadingText.text = "Please Wait ...";
		Time.timeScale = 1f;
		WifiManager.Instance.LoadingObj.SetActive(true);
        SceneManager.LoadScene(menuLevelName);
	}

}
