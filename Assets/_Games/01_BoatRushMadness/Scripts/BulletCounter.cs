using System;
using UnityEngine;
using UnityEngine.UI;
public class BulletCounter : MonoBehaviour
{
    

    public Text bulletText;
    public Text bulletText_1;


    public static int currentBullets  ;

    private void Awake()
    {
        currentBullets = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletText.text = currentBullets.ToString();
        bulletText_1.text = bulletText.text;


    }

    private void Update()
    {
        if (currentBullets < 0)
        {
            currentBullets = 0;
            bulletText.text = currentBullets.ToString();
            bulletText_1.text = bulletText.text;
        }
        bulletText_1.text = bulletText.text;
    }

    public void IncreaseCoins(int v)
    {
        currentBullets += v;
        bulletText.text = currentBullets.ToString();
        bulletText_1.text = bulletText.text;
    }
}
