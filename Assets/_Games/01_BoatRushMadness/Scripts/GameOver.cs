using System;
using System.Text;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;
    public UIMAnager UimAnager;
    public GameObject player;
    public Transform explosionPoint;
    public GameObject explosion;
    public GameObject wave1;
    public GameObject wave2;
    //private bool explode = false;

    public bool gameOver = false;
    public Text currentScore;
    public Text distanceText;
    public Text starText;
    public GameObject Scoreobject;
    public GameObject bulletsObject;
    public GameObject StarObject;
    public GameObject DistanceObject;

    public CapsuleCollider2D capsuleCollider2D;
    
    
    private AudioManager audioManager;
    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("NO AudioSource FOUND");
        }

        instance = this;

        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("obstacles") || other.CompareTag("rock"))
        {
            Instantiate(explosion, transform);
            audioManager.PlaySound("explosionsound");
            if (PowerShield.instance.shieldActive == false)
            {
                DeactivatePlayer(false);
                gameOver = true;
                Invoke(nameof(GameoverUI), 1f);
                currentScore.text = coinCounter.currentCoins.ToString();
                distanceText.text = (int)Distance.instance.distance + " ";
                starText.text = StarCounter.currentStars.ToString();
                DeactivateScore(false);
                capsuleCollider2D.enabled = false;
            }
            
        }
        
        
    }
    
    public void DeactivatePlayer(bool value)
    {
        player.SetActive(value);
        wave1.SetActive(value);
        wave2.SetActive(value);
    }

    public void DeactivateScore(bool value)
    {
        Scoreobject.SetActive(value);
        bulletsObject.SetActive(value);
        StarObject.SetActive(value);
        DistanceObject.SetActive(value);
    }

    public void GameoverUI()
    {
        
        UimAnager.GameOverUI();
    }
}
