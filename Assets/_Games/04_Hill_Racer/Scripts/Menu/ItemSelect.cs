using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ItemType
{
	Level,
	Car
}
public class ItemSelect : MonoBehaviour
{

	// Item type , Level or Car
	public ItemType itemType;

	// Current,Prev and next button images + Items icon
	[Header("Icons")]
	public Sprite[] itemIcons;

	//[SerializeField] GameObject currentLock;
	//[SerializeField] GameObject prevLock;
	//[SerializeField] GameObject nextLock;

	[SerializeField] GameObject contentParent;

	// We used animator to animated current item when selected
	[Header("Current Item Animation")]
	public Animator currentAnimator;

	// play error sound clip when player coins is not enough, OK sound clip when player has enough money and buy item then
	[Header("Sounds")]
	public AudioSource audioSource;
	public AudioClip okClip ;
	public AudioClip errorClip;

	// Display cuurent item an total coins
	[Header("Texts")]
	public Text CurentValue;
	public Text coinsTXT;

	// Activate these windows when we need them
	[Header("Windows")]
	public GameObject shopOffer;
	//public GameObject lockIcon;
	public GameObject nextMen;

	// Internal usage
	[SerializeField] int id;
	bool canAnim;
	bool animaState;

	// List of the items price
	public int[] itemsPrice;

    void Start ()
	{

		// Display total coins on start
		coinsTXT.text = PlayerPrefs.GetInt ("Coins").ToString ();

		// Read last selected item ID
		if(itemType == ItemType.Car)
			id = PlayerPrefs.GetInt ("CarID");
		if(itemType == ItemType.Level)
			id = PlayerPrefs.GetInt ("LevelID");

		// Select last selected item by default
		/*if (id != 0)
		{
			PrevCar ();
			NextCar ();
		} else {
			NextCar ();
			PrevCar ();
		}*/

		// Internal usage
		canAnim = false;

		// Check current item is unlocked?
		/*if (itemType == ItemType.Car) {

			if (PlayerPrefs.GetInt ("Car" + id.ToString ()) != 3)
				lockIcon.SetActive (true);
			else
				lockIcon.SetActive (false);
		}
		if (itemType == ItemType.Level) {

			if (PlayerPrefs.GetInt ("Level" + id.ToString ()) != 3)
				lockIcon.SetActive (true);
			else
				lockIcon.SetActive (false);
		}*/

		// Update item prices
		CurentValue.text = itemsPrice [id].ToString ();

		for (int i = 0; i < itemIcons.Length; i++)
		{
			//PlayerPrefs.SetInt("myLevel" + i.ToString(), 1);// uncomment to unlock all level
			if (itemType == ItemType.Car)
			{
				contentParent.transform.GetChild(i).transform.GetChild(0).transform.gameObject.SetActive(false);
			}
			else if(itemType == ItemType.Level)
			{
				if (PlayerPrefs.GetInt("myLevel" + i.ToString(), 0) != 0)
					contentParent.transform.GetChild(i).transform.GetChild(0).transform.gameObject.SetActive(false);
			}
		}
	}

	bool isLocked(int index)
    {
		if (itemType == ItemType.Car)
			return false;

		int n = 0;
		for (int i = 0; i < itemIcons.Length; i++)
		{
			if (i == index)
				n = PlayerPrefs.GetInt("myLevel" + i.ToString());
		}
		Debug.Log("myLevel" + index + ": " + n);
		if (n == 0)
			return true;
		else
			return false;
    }

	/*
	// public function used in ui button to select next car
	public void NextCar ()
	{
		Debug.Log("Next ID: " + id);
		if (id < itemIcons.Length - 1) {
			id++;
			if (canAnim)
				PlayAnim ();
			audioSource.clip = okClip;
			audioSource.Play ();
			if (isLocked(id))
			{	currentLock.SetActive(true); currentItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{	currentLock.SetActive(false); currentItemImage.color = new Color32(255, 255, 255, 255); }
		}
		currentItemImage.sprite = itemIcons [id];

		if (id < itemIcons.Length - 1) {
			prevItemImage.color = new Color (1f, 1f, 1f, 1f);
			nextItemImage.sprite = itemIcons [id + 1];
			prevItemImage.sprite = itemIcons [id - 1];

			if (isLocked(id - 1))
			{ prevLock.SetActive(true); prevItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{	prevLock.SetActive(false); prevItemImage.color = new Color32(255, 255, 255, 255); }


			if (isLocked(id + 1))
			{	nextLock.SetActive(true); nextItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{	nextLock.SetActive(false); nextItemImage.color = new Color32(255, 255, 255, 255); }

		} else {
			nextItemImage.sprite = null;
			nextItemImage.color = new Color (0, 0, 0, 0);
			prevItemImage.sprite = itemIcons [id - 1];

			nextLock.SetActive(false);
			if (isLocked(id - 1))
			{	prevLock.SetActive(true); prevItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{	prevLock.SetActive(false); prevItemImage.color = new Color32(255, 255, 255, 255); }
		}

		if (itemType == ItemType.Car)
			PlayerPrefs.SetInt ("CarID", id);
		if (itemType == ItemType.Level)
			PlayerPrefs.SetInt ("LevelID", id);

		CurentValue.text = itemsPrice [id].ToString ();
	}

	// public function used in ui button to select prev car
	public void PrevCar ()
	{
		Debug.Log("Prev ID: " + id);
		if (id > 0) {
			--id;
			if (canAnim)
				PlayAnim ();

			audioSource.clip = okClip;
			audioSource.Play ();
			if (isLocked(id))
			{	currentLock.SetActive(true); currentItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{	currentLock.SetActive(false); currentItemImage.color = new Color32(255, 255, 255, 255); }
		}
		currentItemImage.sprite = itemIcons [id];
		if (id > 0) {
			nextItemImage.color = new Color (1f, 1f, 1f, 1f);
			prevItemImage.sprite = itemIcons [id - 1];
			nextItemImage.sprite = itemIcons [id + 1];

			if (isLocked(id - 1))
			{	prevLock.SetActive(true); prevItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{ prevLock.SetActive(false); prevItemImage.color = new Color32(255, 255, 255, 255); }


			if (isLocked(id + 1))
			{	nextLock.SetActive(true); nextItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{	nextLock.SetActive(false); nextItemImage.color = new Color32(255, 255, 255, 255); }

		} else {
			prevItemImage.sprite = null;
			prevItemImage.color = new Color (0, 0, 0, 0);
			nextItemImage.sprite = itemIcons [id + 1];

			prevLock.SetActive(false);

			if (isLocked(id + 1))
			{	nextLock.SetActive(true); nextItemImage.color = new Color32(53, 53, 53, 255); }
			else
			{	nextLock.SetActive(false); nextItemImage.color = new Color32(255, 255, 255, 255); }
		}

		if (itemType == ItemType.Level)
			PlayerPrefs.SetInt ("LevelID", id);
		if (itemType == ItemType.Car)
			PlayerPrefs.SetInt ("CarID", id);

		CurentValue.text = itemsPrice [id].ToString ();
	}
	*/

	// Play animation when player select next or prev item
	void PlayAnim ()
	{
		animaState = !animaState;
		if (animaState)
			currentAnimator.CrossFade ("Next", .003f);
		else
			currentAnimator.CrossFade ("Prev", .003f);

	}


	// Select current item and go to the next menu
	public void SelectCurrent ()
	{
		/*if (itemType == ItemType.Level) {
			if (PlayerPrefs.GetInt ("Level" + id.ToString ()) == 3) {
				if (currentLock.activeSelf)
					return;
				gameObject.SetActive (false);
				nextMen.SetActive (true);
				PlayerPrefs.SetInt ("SelectedLevel", id);

				MenuController.Instance.onSetState(StaticData.SelectionScreen);
			} else {
				audioSource.clip = errorClip;
				audioSource.Play ();
			}
		}

		if (itemType == ItemType.Car) {
			if (PlayerPrefs.GetInt ("Car" + id.ToString ()) == 3) {
				//gameObject.SetActive (false);
				//nextMen.SetActive (true);
				PlayerPrefs.SetInt ("SelectedCar", id);
				//MyController.Instance.loading.SetActive(true);akash
				PlayerPrefs.SetInt("AllScoreTemp", PlayerPrefs.GetInt("Coins"));

				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Level" + PlayerPrefs.GetInt("SelectedLevel").ToString());
			} else {
				audioSource.clip = errorClip;
				audioSource.Play ();
			}
		}*/
	}

	public void SelectCurrentCar(int id)
	{
		Debug.Log("SelectCurrentCar: " + id);
		if (PlayerPrefs.GetInt("Car" + id.ToString()) == 3)
		{
			Debug.Log("SelectCurrentCar inside if");
			//gameObject.SetActive (false);
			//nextMen.SetActive (true);
			PlayerPrefs.SetInt("SelectedCar", id);
			WifiManager.Instance.LoadingObj.SetActive(true);
			PlayerPrefs.SetInt("AllScoreTemp", PlayerPrefs.GetInt("Coins"));

			UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Hill_Racer_Level" + PlayerPrefs.GetInt("SelectedLevel").ToString());
		}
		else
		{
			Debug.Log("SelectCurrentCar inside else");
			audioSource.clip = errorClip;
			audioSource.Play();
		}
	}

	public void SelectCurrentLevel(int id)
    {
		Debug.Log("SelectCurrentLevel: " + id);
		if (PlayerPrefs.GetInt("Level" + id.ToString()) == 3)
		{
			Debug.Log("SelectCurrentLevel inside if");
			if (contentParent.transform.GetChild(id).transform.GetChild(0).transform.gameObject.activeSelf)
                return;

            gameObject.SetActive(false);
			nextMen.SetActive(true);
			PlayerPrefs.SetInt("SelectedLevel", id);

			MenuController.Instance.onSetState(StaticData.SelectionScreen);
		}
		else
		{
			Debug.Log("SelectCurrentLevel inside else");
			audioSource.clip = errorClip;
			audioSource.Play();
		}
	}

	// Public function used in current selected button (ui button ) 
	public void Buy ()
	{

		if (itemType == ItemType.Level) {
			if (PlayerPrefs.GetInt ("Level" + id.ToString ()) != 3) {
				if (PlayerPrefs.GetInt ("Coins") >= itemsPrice [id]) {
					PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - itemsPrice [id]);
					PlayerPrefs.SetInt ("Level" + id.ToString (), 3);
					//lockIcon.SetActive (false);
					coinsTXT.text = PlayerPrefs.GetInt ("Coins").ToString ();
				} else
					shopOffer.SetActive (true);
			}

		}

		if (itemType == ItemType.Car) {
			if (PlayerPrefs.GetInt ("Car" + id.ToString ()) != 3) {
				if (PlayerPrefs.GetInt ("Coins") >= itemsPrice [id]) {
					PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - itemsPrice [id]);
					PlayerPrefs.SetInt ("Car" + id.ToString (), 3);
					//lockIcon.SetActive (false);
					coinsTXT.text = PlayerPrefs.GetInt ("Coins").ToString ();
				} else
					shopOffer.SetActive (true);
			}

		}

	}
}
