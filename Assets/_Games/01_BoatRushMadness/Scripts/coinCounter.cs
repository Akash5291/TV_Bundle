using UnityEngine;
using UnityEngine.UI;

public class coinCounter : MonoBehaviour
{

    public static coinCounter instance;

    public  Text scoreText;
    public Text scoreText_1;

    public static int currentCoins ;

    private void Awake()
    {
        instance = this;
        currentCoins = PlayerPrefs.GetInt("coinAdder",0);
    }

    void Start()
    {
        //currentCoins =  5000;
        scoreText.text = currentCoins.ToString();
        scoreText_1.text = scoreText.text;
    }

    public void IncreaseCoins(int v)
    {
        currentCoins += v;
        scoreText.text = currentCoins.ToString();
        scoreText_1.text = scoreText.text;
        PlayerPrefs.SetInt("coinAdder", currentCoins);
    }

    private void Update()
    {
        scoreText_1.text = scoreText.text;
    }
}
