using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distance : MonoBehaviour
{
    public static Distance instance;
    public float distance ;
    public Text distanceTxt;
    public Text distanceTxt_1;
    public float pointIncreasedPerSecond;
    
    // Start is called before the first frame update
    void Start()
    {
        distance = 0f;
        pointIncreasedPerSecond = 5f;
        instance = this;
        PlayerPrefs.SetInt("Distance", (int)distance);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameOver.instance.gameOver == false && UIMAnager.instance.distancebool == true)
        {
            distanceTxt.text = ((int)distance).ToString();
            PlayerPrefs.SetInt("Distance", (int)distance);
            distance += pointIncreasedPerSecond * Time.deltaTime;
        }
        else
        {
            distanceTxt.text = ((int)distance).ToString();
        }
        distanceTxt_1.text = distanceTxt.text;
    }
}
