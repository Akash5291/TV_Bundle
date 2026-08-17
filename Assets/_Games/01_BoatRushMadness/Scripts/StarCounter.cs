using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarCounter : MonoBehaviour
{
    public static StarCounter instance;

    public  Text starText;
    public Text starText_1;

    public static int currentStars ;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        starText.text = currentStars.ToString();
        starText_1.text = starText.text;
    }

    public void IncreaseCoins(int v)
    {
        currentStars += v;
        starText.text = currentStars.ToString();
        starText_1.text = starText.text;

    }

    private void Update()
    {
        starText_1.text = starText.text;
    }
}
