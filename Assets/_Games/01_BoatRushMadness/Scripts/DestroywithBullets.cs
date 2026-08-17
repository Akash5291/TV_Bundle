using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroywithBullets : MonoBehaviour
{
    [SerializeField]private GameObject explosion;
    //public Transform bullet;
    
    private AudioManager audioManager;
     private void Start()
     {
         audioManager = AudioManager.instance;
         if (audioManager == null)
         {
             Debug.LogError("NO AudioSource FOUND");
         }
         
     }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            Instantiate(explosion, gameObject.transform.parent.transform);
            Destroy(other.transform.gameObject);
            Destroy(gameObject);
            //Invoke(nameof(Destroy),0.2f);
            audioManager.PlaySound("explosionsound");
            
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
