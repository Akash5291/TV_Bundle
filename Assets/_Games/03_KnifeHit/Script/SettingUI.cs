using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour {

	[Header("Setting View")]
	public  Toggle soundToggle;
	public  Toggle vibrationToggle;
	public  GameObject UIParent;
    public Text removeAdPriceText;
    public GameObject bg;
    public GameObject removeAdSection;

	public static SettingUI intance;

	void Awake()
	{
		if (intance == null) 
		{
			intance = this;
		}
	}

	void Start()
	{

		soundToggle.onValueChanged.RemoveAllListeners ();
		vibrationToggle.onValueChanged.RemoveAllListeners ();
		UpdateUI ();
		soundToggle.onValueChanged.AddListener ((arg0) =>{ 
			GameManagerNinjaKnife.Sound=arg0;
			if(arg0)
				SoundManager.instance.PlaybtnSfx ();
		} );
		vibrationToggle.onValueChanged.AddListener ((arg0) =>{ 
			GameManagerNinjaKnife.Vibration=arg0;
			if(arg0)
				SoundManager.instance.playVibrate();
		} );


#if IAP && UNITY_PURCHASING
        Purchaser.instance.onItemPurchased += OnItemPurchased;
        removeAdPriceText.text = "$" + Purchaser.instance.iapItems[0].price;
#endif

        removeAdSection.SetActive(Purchaser.instance.isEnabled && !CUtils.IsAdsRemoved());
    }

    public void ShowUI()
	{
		UIParent.SetActive (true);
        bg.SetActive(true);
        CUtils.ShowInterstitialAd();
	}

    public void CloseUI()
    {
        UIParent.SetActive(false);
        bg.SetActive(false);
    }

	public void UpdateUI()
	{
		soundToggle.isOn = GameManagerNinjaKnife.Sound;
		vibrationToggle.isOn = GameManagerNinjaKnife.Vibration;
	}

	public void OnRestorPurchases()
	{
#if IAP && UNITY_PURCHASING
        Purchaser.instance.RestorePurchases();
#endif
    }

    public void OnRemoveAdCall()
	{
#if IAP && UNITY_PURCHASING
        SoundManager.instance.PlaybtnSfx();
        Purchaser.instance.BuyProduct(0);
#else
        Debug.LogError("Please enable, import and install Unity IAP to use this function");
#endif
    }


#if IAP && UNITY_PURCHASING
    private void OnItemPurchased(IAPItem item, int index)
    {
        // A consumable product has been purchased by this user.
        if (item.productType == PType.Consumable)
        {
            
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (item.productType == PType.NonConsumable)
        {
            CUtils.SetRemoveAds(true);
            Toast.instance.ShowMessage("Removing ads is successful");

            removeAdSection.SetActive(false);
        }
        // Or ... a subscription product has been purchased by this user.
        else if (item.productType == PType.Subscription)
        {
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
    }
#endif

#if IAP && UNITY_PURCHASING
    private void OnDestroy()
    {
        Purchaser.instance.onItemPurchased -= OnItemPurchased;
    }
#endif
}
