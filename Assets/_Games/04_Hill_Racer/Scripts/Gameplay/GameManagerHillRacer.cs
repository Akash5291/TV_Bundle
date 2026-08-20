//-----------------------------------------------
//
//This is  Game Manager that is responsible for manage Coins,Distance,Records and fuel
//Orginally writed for My game In Summer 2016   ALIyerEdon Unity 5.3.6f1234
//-----------------------------------------------/
using UnityEngine;
using System.Collections;


//include this for UI usage
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//-----------------------------------------------


//Game Manager is responsible for Coins,Distance,Fuel...
public class GameManagerHillRacer : MonoBehaviour
{
	public static GameManagerHillRacer Instance = null;

	[SerializeField] bool tryByRemote = false;

	[Header("Informations")]
	// UI-----------------------------------------------
	// For display Coins,Fuel , Record and distance
	public Text DistanceTXT;
	public Text RecordTXT;
	public Text CoinTXT;
	public Text FuelTXT;
	public Text LastRecord;

	[Header("Sliders")]
	// SHow fuel amount
	public Slider FuelSlider;
	// Show passed distance on down of the screen
	public Slider DitanceSlider;
	//-----------------------------------------------

	//Manager-----------------------------------------------
	//player transform to read position as distance
	Transform player;
	//is game started?
	bool Started;

	//----------------------------------------------------

	[Header("Fuel")]
	//fuel-----------------------------------------------
	// Total fuel
	public float TotalFuel = 100f;
	// Fuel reducing time step (delay in coroutine)
	public float FuelTime = .17f;
	// Fuel reduced amount by time
	float FuelVal = .3f;
	//-----------------------------------------------

	//Coin Manager-----------------------------------------------
	// Game total coins
	int Coins;
	//-----------------------------------------------

	[Header("Coins And Awards")]
	//Get Coin Audio Source-----------------------------------------------
	//Receiving coind sound effect
	public AudioSource coinSound;



	// Awarded coin box (award based on distance)
	public GameObject coinAwardedBox;

	// Award text
	public Text awardedText;	

	// Award box animator
	public Animator awardAnimator;


	[Header("Complete Level")]
	// Win and lose windows
	public GameObject youWinMenu ;
	public GameObject youLostMenu;
	public Button nextLevelBtn;

	// End of the level (Level length)
	public float levelLength = 3700f;

	// How much coins give to player when finish the level?
	public int winnerAwardedCoins = 30000;

	void hideLoading()
    {
		WifiManager.Instance.LoadingObj.SetActive(false);
    }

    private void Awake()
    {
		if (Instance == null)
			Instance = this;
    }

    //Start-----------------------------------------------
    IEnumerator Start ()
	{
		//Coins Initialization-----------------------------------------------
		Coins = PlayerPrefs.GetInt ("Coins");   //read total scror from saved Coins
		CoinTXT.text = Coins.ToString ();   // Display total coins on Start
											//-----------------------------------------------

		if (WifiManager.Instance.LoadingObj.activeSelf)
            Invoke("hideLoading", 0.5f);

        //Start Main Game   -----------------------------------------------
        yield return new WaitForEndOfFrame();   //Player is Spawned afer milisecond. we wait .3 and then find it
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		Started = true;   // The game is now started. you can run your codes on update function
		//-----------------------------------------------
		////-----------------------------------------------
		/// 
		// Read if distance based award is already gived for current level, Set it to gived
		if (PlayerPrefs.GetInt ("c500" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c500 = true;
			LastRecord.text = "Record:500";
		}
		if (PlayerPrefs.GetInt ("c1000" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c1000 = true;
			LastRecord.text = "Record:1000";
		}
		if (PlayerPrefs.GetInt ("c1500" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c1500 = true;
			LastRecord.text = "Record:1500";
		}
		if (PlayerPrefs.GetInt ("c2000" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c2000 = true;
			LastRecord.text = "Record:2000";
		}
		if (PlayerPrefs.GetInt ("c2500" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c2500 = true;
			LastRecord.text = "Record:2500";
		}
		if (PlayerPrefs.GetInt ("c3000" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c3000 = true;
			LastRecord.text = "Record:3000";
		}
		if (PlayerPrefs.GetInt ("c3500" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c3500 = true;
			LastRecord.text = "Record:3500";
		}
		if (PlayerPrefs.GetInt ("c4000" + PlayerPrefs.GetInt ("SelectedLevel").ToString ()) == 3) {
			c4000 = true;
			LastRecord.text = "Record:4000";
		}




		//Fuel Decreso  r//-----------------------------------------------
		while (true) {//responsible to decrese fuel amount by   time and value read from upgrade   menu
			yield return new WaitForSeconds (FuelTime);
			TotalFuel -= FuelVal;
			FuelSlider.value = TotalFuel;
			if (TotalFuel >= 0)
				FuelTXT.text = Mathf.Floor (TotalFuel).ToString ();
			if (TotalFuel < 0) {
				//youLostMenu.SetActive (true);
				//Time.timeScale = 0;
				fuelFinished = true;
				StartFuelFinish();

			}
		}

	}
	//-----------------------------------------------


	// Update -----------------------------------------------
	float DistanceTemp;

	void Update ()
	{
		
		if (Started) {
			CoinDistance ();
			if (player.position.x >= DistanceTemp) {
				DistanceTXT.text = Mathf.Floor (player.position.x).ToString ();
				DistanceTemp = player.position.x;
				DitanceSlider.value = player.position.x;
			}

		}
	}
	//-----------------------------------------------

	//Add Coin-----------------------------------------------
	public void AddCoin (int value)
	{//add Coin called from coins trigger
		
		StartCoroutine(TakeCoins());

		CoinTXT.transform.localScale = new Vector3 (CoinTXT.transform.localScale.x, CoinTXT.transform.localScale.y+0.7f,
			CoinTXT.transform.localScale.z);

		if (coinSound)
			coinSound.Play ();
		Coins += value;
		CoinTXT.text = Coins.ToString ();
		PlayerPrefs.SetInt ("Coins", Coins);
	}
	//-----------------------------------------------

	IEnumerator TakeCoins()
	{
		yield return new WaitForSeconds (0.03f);
			CoinTXT.transform.localScale = new Vector3 (CoinTXT.transform.localScale.x, CoinTXT.transform.localScale.y-0.7f,
				CoinTXT.transform.localScale.z);
	}
	//Add Fuel-----------------------------------------------
	public void AddFuel (int value)
	{//add fuel called from Fuel Trigger  s
		if (coinSound)
			coinSound.Play ();
		TotalFuel = value;
		//CoinTXT.text = Coins.ToString ();
		//PlayerPrefs.SetInt ("Coins", Coins);
	}
	//-----------------------------------------------


	bool c500, c1000, c1500, c2000, c2500, c3000, c3500, c4000;

	// Distance based award
	void CoinDistance ()
	{
		if (!c500) {
			if (player.transform.position.x >= 500 && player.transform.position.x < 1000) {
				AddCoin (1000);
				coinAwardedBox.SetActive (true);
				awardAnimator.SetBool ("Award", true);
				awardedText.text = "1000 Coins Awarded";
				StartCoroutine (Awardfalse ());
				c500 = true;
				PlayerPrefs.SetInt ("c500" + PlayerPrefs.GetInt ("SelectedLevel").ToString (), 3);// 3 => true | 0 => false
				LastRecord.text = "Record:500";
			}
		}
		if (!c1000 && c500) {
			if (player.transform.position.x >= 1000 && player.transform.position.x < 1500) {
				AddCoin (2000);
				coinAwardedBox.SetActive (true);
				awardAnimator.SetBool ("Award", true);
				awardedText.text = "2000 Coins Awarded";
				StartCoroutine (Awardfalse ());
				c1000 = true;
				PlayerPrefs.SetInt ("c1000" + PlayerPrefs.GetInt ("SelectedLevel").ToString (), 3);// 3 => true | 0 => false
				LastRecord.text = "Record:1000";
			}
		}
		if (!c1500 && c1000) {
			if (player.transform.position.x >= 1500 && player.transform.position.x < 2000) {
				AddCoin (3000);
				coinAwardedBox.SetActive (true);
				awardAnimator.SetBool ("Award", true);
				awardedText.text = "3000 Coins Awarded";
				StartCoroutine (Awardfalse ());
				c1500 = true;
				PlayerPrefs.SetInt ("c1500" + PlayerPrefs.GetInt ("SelectedLevel").ToString (), 3);// 3 => true | 0 => false
				LastRecord.text = "Record:1500";
			}
		}
		if (!c2000 && c1500) {
			if (player.transform.position.x >= 2000 && player.transform.position.x < 2500) {
				AddCoin (4000);
				coinAwardedBox.SetActive (true);
				awardAnimator.SetBool ("Award", true);
				awardedText.text = "4000 Coins Awarded";
				StartCoroutine (Awardfalse ());
				c2000 = true;
				PlayerPrefs.SetInt ("c2000" + PlayerPrefs.GetInt ("SelectedLevel").ToString (), 3);// 3 => true | 0 => false
				LastRecord.text = "Record:2000";
			}
		}
		if (!c2500 && c2000) {
			if (player.transform.position.x >= 2500 && player.transform.position.x < 3000) {
				AddCoin (5000);
				coinAwardedBox.SetActive (true);
				awardAnimator.SetBool ("Award", true);
				awardedText.text = "5000 Coins Awarded";
				StartCoroutine (Awardfalse ());
				c2500 = true;
				PlayerPrefs.SetInt ("c2500" + PlayerPrefs.GetInt ("SelectedLevel").ToString (), 3);// 3 => true | 0 => false
				LastRecord.text = "Record:2500";
			}
		}
		if (!c3000 && c2500) {
			if (player.transform.position.x >= 3000 && player.transform.position.x < 3500) {
				AddCoin (6000);
				coinAwardedBox.SetActive (true);
				awardAnimator.SetBool ("Award", true);
				awardedText.text = "6000 Coins Awarded";
				StartCoroutine (Awardfalse ());
				c3000 = true;
				PlayerPrefs.SetInt ("c3000" + PlayerPrefs.GetInt ("SelectedLevel").ToString (), 3);// 3 => true | 0 => false
				LastRecord.text = "Record:3000";
			}
		}
		if (!c3500 && c3000) {
			if (player.transform.position.x >= 3500 && player.transform.position.x < 4000) {
				AddCoin (7000);
				coinAwardedBox.SetActive (true);
				awardAnimator.SetBool ("Award", true);
				awardedText.text = "7000 Coins Awarded";
				StartCoroutine (Awardfalse ());
				c3500 = true;
				PlayerPrefs.SetInt ("c3500" + PlayerPrefs.GetInt ("SelectedLevel").ToString (), 3);// 3 => true | 0 => false
				LastRecord.text = "Record:3500";
			}
		}
		if (!c4000 && c3500) {
			if (player.transform.position.x >= 3700 && player.transform.position.x < 4500) {
				if (!youWinMenu.activeSelf)
				{
					AddCoin(8000);
					coinAwardedBox.SetActive(true);
					youWinMenu.SetActive(true);
					awardAnimator.SetBool("Award", true);
					awardedText.text = "8000 Coins Awarded";
					StartCoroutine(Awardfalse());
					c4000 = true;
					PlayerPrefs.SetInt("c4000" + PlayerPrefs.GetInt("SelectedLevel").ToString(), 3);// 3 => true | 0 => false
					LastRecord.text = "Record:4000";
					unlockNextLevel();
					AddCoin(winnerAwardedCoins);
					GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().isKinematic = true;
					Debug.Log("win call 1");
					MenuController.Instance.onSetState(StaticData.LevelFinish);
					Time.timeScale = 0;
				}
			}
		}


		// End of the level    
		if (player.transform.position.x >= levelLength) {
			if (!youWinMenu.activeSelf)
			{
				youWinMenu.SetActive(true);
				unlockNextLevel();
				Debug.Log("win call 2");
				MenuController.Instance.onSetState(StaticData.LevelFinish);
				GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().isKinematic = true;
				Time.timeScale = 0;
			}
		}

	}

	public void onNextLevelBtn()
	{
		PlayerPrefs.SetInt("SelectedLevel", PlayerPrefs.GetInt("LevelID") + 1);
		WifiManager.Instance.LoadingObj.SetActive(true);
		Time.timeScale = 1;
		SceneManager.LoadSceneAsync("Hill_Racer_Level" + PlayerPrefs.GetInt("SelectedLevel").ToString());
	}

	void unlockNextLevel()
    {
		int lvl = PlayerPrefs.GetInt("LevelID");
		if ((lvl + 1) <= 6)
		{
			lvl++;
			nextLevelBtn.interactable = true;
		}
		else
		{
			nextLevelBtn.interactable = false;
		}
		PlayerPrefs.SetInt("myLevel" + lvl.ToString(), 1);
	}

	IEnumerator Awardfalse ()
	{
		
		yield return new WaitForSeconds (3f);
		awardAnimator.SetBool ("Award", false);
		yield return new WaitForSeconds (3f);
		coinAwardedBox.SetActive (false);
	}

	[HideInInspector]
	public bool isDead;

	public void StartDead()
	{
		StartCoroutine (Dead ()); 
	}

	IEnumerator Dead()
	{
		yield return new WaitForEndOfFrame();
		if (isDead) {
			youLostMenu.SetActive (true);

			MenuController.Instance.onSetState(StaticData.GameOver);
			Time.timeScale = 0;
		}

	}

	[HideInInspector]
	public bool fuelFinished;


	public void StartFuelFinish()
	{
		StartCoroutine (DeadFuel ());
	}
	IEnumerator DeadFuel()
	{
		yield return new WaitForEndOfFrame();//WaitForSeconds (3f);
		if (fuelFinished) {
			youLostMenu.SetActive (true);

			MenuController.Instance.onSetState(StaticData.GameOver);
			Time.timeScale = 0;
		}

	}
}