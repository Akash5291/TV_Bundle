
using UnityEngine;

public class BulletCollect : MonoBehaviour
{
    public static int  value  = 3;

    //private AudioManager audioManager;
    public BulletCounter BulletCounter;

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
        if (other.CompareTag("bullets"))
        {
            AudioManager.instance.PlaySound("itempick");
            Destroy(other.transform.gameObject);
            BulletCounter.IncreaseCoins(value);
        }
            
    }
}
