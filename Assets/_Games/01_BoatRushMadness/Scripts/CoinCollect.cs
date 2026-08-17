
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public int  value  = 5;

    public static CoinCollect instance;
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
        if (other.CompareTag("coins"))
        {
            AudioManager.instance.PlaySound("coinsound");
            coinCounter.instance.IncreaseCoins(value);
            Destroy(other.transform.gameObject);
        }
            
    }
    
    
    
}
