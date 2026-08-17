
using UnityEngine;

public class PowerUpCollect : MonoBehaviour
{
    public int  value ;
    public int  value1 ;
    public int value2;
    //public Sprite hundred;
    //public Sprite twohundred;
    //public Sprite fivehundred;

    public static PowerUpCollect instance;
    //private AudioManager audioManager;


    private void Awake()
    {
        instance = this;
    }

    /*private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("NO AudioSource FOUND");
        }
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("power"))
        {
            //Instantiate(hundred, gameObject.transform.parent.transform);
            AudioManager.instance.PlaySound("itempick");
            coinCounter.instance.IncreaseCoins(value);
            Destroy(other.transform.gameObject);
        }
        
        if (other.CompareTag("power200"))
        {
            //Instantiate(twohundred, gameObject.transform.parent.transform);
            AudioManager.instance.PlaySound("itempick");
            coinCounter.instance.IncreaseCoins(value1);
            Destroy(other.transform.gameObject);
        }
        
        if (other.CompareTag("power500"))
        {
            //Instantiate(fivehundred, gameObject.transform.parent.transform);
            AudioManager.instance.PlaySound("itempick");
            coinCounter.instance.IncreaseCoins(value2);
            Destroy(other.transform.gameObject);
        }
            
    }
}
