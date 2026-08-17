using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopItem : MonoBehaviour
{
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private int skinIndex;
    [SerializeField] private GameObject buyButton;
    private Skin skin;

    void Start()
    {
        skin = skinManager.skins[skinIndex];
        //GetComponent<Image>().sprite = skin.sprite;
        
        if (skinManager.IsUnlocked(skinIndex))
        {
            buyButton.SetActive(false);
        }
        else
        {
            buyButton.SetActive(true);
        }
    }

    void OnSkinPressed()
    {
        if (skinManager.IsUnlocked(skinIndex))
        {
            skinManager.SelectSkin(skinIndex);
        }
    }

    public void OnBuyButtonPressed()
    {
       int coins ;

        // Unlock the skin
        if (skinManager.IsUnlocked(skinIndex))
            OnSkinPressed();
        else if (coinCounter.currentCoins >= skin.cost && !skinManager.IsUnlocked(skinIndex))
        {
            coins = coinCounter.currentCoins - skin.cost;
            coinCounter.instance.scoreText.text = coins.ToString();
            PlayerPrefs.SetInt("coinAdder", coins);
            skinManager.Unlock(skinIndex);
            buyButton.SetActive(false);
            skinManager.SelectSkin(skinIndex);
        }
        else
        {
            Debug.Log("Not enough coins :(");
        }
    }
}