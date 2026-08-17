using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopController : MonoBehaviour
{
    [SerializeField] private Image selectedSkin;
    //[SerializeField] private Text coinsText;
    [SerializeField] private SkinManager skinManager;

    void Update()
    {
        //coinsText.text = coinCounter.currentCoins.ToString();
        selectedSkin.sprite = skinManager.GetSelectedSkin().sprite;
    }
}