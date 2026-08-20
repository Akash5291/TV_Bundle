using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnifeShop : MonoBehaviour
{
    [SerializeField] GameObject LuckyWheelObj;
    public GameObject shopUIParent;
    public ShopItem shopKnifePrefab;
    public Transform shopPageContent;
    public Text unlockKnifeCounterLbl;
    public Button unlockNowBtn, unlockRandomBtn;
    public Image selectedKnifeImageUnlock;
    public Image selectedKnifeImageLock;
    public GameObject knifeBackeffect1, knifeBackeffect2;
    public int UnlockPrice = 250, UnlockRandomPrice = 250;
    public Text KnifePrizeText;
    public List<Knife> shopKnifeList;

    public static KnifeShop intance;
    public static ShopItem selectedItem;
    public AudioClip onUnlocksfx, RandomUnlockSfx;
    public GameObject bg;

    public GameObject NotEnoughWheelAppleText;
    public GameObject NotEnoughAppleText;
    public GameObject AlredyBuyedText;

    [SerializeField] MenuController menuController;

    List<ShopItem> shopItems;
    ShopItem selectedShopItem
    {
        get
        {
            return shopItems.Find((obj) => { return obj.selected; });
        }
    }

    void Start()
    {
        if (intance == null)
        {
            intance = this;
            SetupShop();
        }
    }

    public void checkEnoughApple()
    {
        SoundManager.instance.PlaybtnSfx();
        if (GameManagerNinjaKnife.Apple >= 10)
        {
            shopUIParent.SetActive(false);
            LuckyWheelObj.SetActive(true);
            MenuController.Instance.onSetState(StaticData.SpinWheel);
        }
        else
        {
            NotEnoughWheelAppleText.SetActive(true);
            Invoke("DisableText", 1.5f);
        }
    }

    [ContextMenu("Clear PlayerPref")]
    void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [ContextMenu("Add Apple")]
    void AddApple()
    {
        GameManagerNinjaKnife.Apple += 500;
    }

    public void ShowShop()
    {
        shopUIParent.SetActive(true);
        bg.SetActive(true);
        if (!shopItems[GameManagerNinjaKnife.SelectedKnifeIndex].selected)
        {
            shopItems[GameManagerNinjaKnife.SelectedKnifeIndex].selected = true;
        }
        UpdateUI();

        CUtils.ShowInterstitialAd();
    }

    public void CloseShop()
    {
        SoundManager.instance.PlaybtnSfx();
        shopUIParent.SetActive(false);
        bg.SetActive(false);
        MenuController.Instance.onSetState(StaticData.Home);
    }

    void SetupShop()
    {
        //unlockNowBtn.GetComponentInChildren<Text>().text = UnlockPrice + "";
        //unlockRandomBtn.GetComponentInChildren<Text>().text = UnlockRandomPrice + "";
        KnifePrizeText.text = UnlockPrice + "";
        shopItems = new List<ShopItem>();
        int n = 0;
        int r = 1;
        for (int i = 0; i < shopKnifeList.Count; i++)
        {
            ShopItem temp = Instantiate<ShopItem>(shopKnifePrefab, shopPageContent);
            temp.setup(i, this);
            temp.name = i + "";
            shopItems.Add(temp);

            if (menuController != null)
                menuController.uiButtons[1].buttonsRow[r].buttonsColumn.Add(temp.GetComponent<Button>());

            if (n >= 3) { r++; n = 0; }
            else n++;
        }

        shopItems[GameManagerNinjaKnife.SelectedKnifeIndex].OnClick();
    }

    public void onShowPriceTag(bool value, Sprite knifeSprite)
    {
        KnifePrizeText.transform.gameObject.SetActive(!value);
        selectedKnifeImageUnlock.sprite = knifeSprite;
    }

    public void UpdateUI()
    {
        selectedKnifeImageUnlock.sprite = selectedShopItem.knifeImage.sprite;
        selectedKnifeImageLock.sprite = selectedShopItem.knifeImage.sprite;
        selectedKnifeImageUnlock.gameObject.SetActive(selectedShopItem.KnifeUnlock);
        selectedKnifeImageLock.gameObject.SetActive(!selectedShopItem.KnifeUnlock);

        knifeBackeffect1.SetActive(selectedShopItem.KnifeUnlock);
        knifeBackeffect2.SetActive(selectedShopItem.KnifeUnlock);

        int unlockCount = 0;
        if (shopItems.FindAll((obj) => { return obj.KnifeUnlock; }) != null)
        {
            unlockCount = shopItems.FindAll((obj) =>
            {
                return obj.KnifeUnlock;
            }).Count;
        }
        unlockKnifeCounterLbl.text = unlockCount + "/" + shopKnifeList.Count;
        if (unlockCount == shopKnifeList.Count)
        {
            unlockNowBtn.interactable = false;
            unlockRandomBtn.interactable = false;
        }

        GameManagerNinjaKnife.selectedKnifePrefab = shopKnifeList[GameManagerNinjaKnife.SelectedKnifeIndex];
        if (MainMenu.intance != null)
        {
            MainMenu.intance.selectedKnifeImage.sprite = GameManagerNinjaKnife.selectedKnifePrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void UnlockKnife()
    {
        if (unlockingRandom)
            return;

        if (GameManagerNinjaKnife.Apple < UnlockPrice)
        {
            NotEnoughAppleText.SetActive(true);
            Invoke("DisableText", 1.5f);
            Toast.instance.ShowMessage("Opps! Don't have enough apples");
            SoundManager.instance.PlaybtnSfx();
            return;
        }
        if (selectedShopItem.KnifeUnlock)
        {
            AlredyBuyedText.SetActive(true);
            Invoke("DisableText", 1.5f);
            Toast.instance.ShowMessage("It's already unlocked!");
            SoundManager.instance.PlaybtnSfx();
            return;
        }
        GameManagerNinjaKnife.Apple -= UnlockPrice;
        selectedShopItem.KnifeUnlock = true;
        selectedShopItem.UpdateUIColor();
        GameManagerNinjaKnife.SelectedKnifeIndex = selectedShopItem.index;
        UpdateUI();
        SoundManager.instance.PlaySingle(onUnlocksfx);

    }

    void DisableText()
    {
        NotEnoughWheelAppleText.SetActive(false);
        NotEnoughAppleText.SetActive(false);
        AlredyBuyedText.SetActive(false);
    }

    bool unlockingRandom = false;
    public void UnlockRandomKnife()
    {
        if (GameManagerNinjaKnife.Apple < UnlockRandomPrice)
        {
            Toast.instance.ShowMessage("Opps! Don't have enough apples");
            SoundManager.instance.PlaybtnSfx();
            return;
        }
        if (unlockingRandom)
        {
            return;
        }
        StartCoroutine(UnlockRandomCoKnife());

    }

    IEnumerator UnlockRandomCoKnife()
    {
        unlockingRandom = true;
        List<ShopItem> lockedItems = shopItems.FindAll((obj) => { return !obj.KnifeUnlock; });
        ShopItem randomSelect = null;
        for (int i = 0; i < lockedItems.Count * 2; i++)
        {
            randomSelect = lockedItems[Random.Range(0, lockedItems.Count)];

            if (!randomSelect.selected)
            {
                randomSelect.selected = true;
                SoundManager.instance.PlaySingle(RandomUnlockSfx);
            }
            yield return new WaitForSeconds(.2f);
        }

        GameManagerNinjaKnife.Apple -= UnlockRandomPrice;
        randomSelect.KnifeUnlock = true;
        randomSelect.UpdateUIColor();
        GameManagerNinjaKnife.SelectedKnifeIndex = randomSelect.index;
        UpdateUI();
        unlockingRandom = false;
        SoundManager.instance.PlaySingle(onUnlocksfx);

    }
}
