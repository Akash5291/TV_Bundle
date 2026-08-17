
using UnityEngine;

public class StarCollect : MonoBehaviour
{
    public static int  value  = 1;
    public bool topCollider = false;
    public bool downCollider = false;
    //private AudioManager audioManager;

    private void Awake()
    {   
        StarCounter.currentStars = PlayerPrefs.GetInt("starAdder",0);
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
        if (other.CompareTag("star"))
        {
            AudioManager.instance.PlaySound("starsound");
            Destroy(other.transform.gameObject);
            StarCounter.instance.IncreaseCoins(value);
            PlayerPrefs.SetInt("starAdder", StarCounter.currentStars);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("BorderTop"))
        {
            topCollider = true;
        }
        else if (collision.gameObject.name.Equals("BorderDown"))
        {
            downCollider = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("BorderTop"))
        {
            topCollider = false;
        }
        else if (collision.gameObject.name.Equals("BorderDown"))
        {
            downCollider = false;
        }
    }
}
