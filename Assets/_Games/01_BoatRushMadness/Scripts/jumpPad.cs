using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpPad : MonoBehaviour
{
    private bool jump = false;
    
    [SerializeField] private bool isRock = false;
    private GameObject player = null;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jump = true;
            player = other.transform.gameObject;
            //other.gameObject.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
            Invoke(nameof(OriginalSize),2f);
            other.gameObject.transform.position += new Vector3(0f, 0f, -3f);
            
            if (isRock)
            {
                GameObject rock = GameObject.FindWithTag("rock");
                rock.transform.GetComponent<BoxCollider2D>().enabled = false;
            }
            
        }
    }

    public void OriginalSize()
    {
        
        //player.transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f);
        player.transform.position -= new Vector3(0f, 0f, -3f);
        if (isRock)
        {
            GameObject rock = GameObject.FindWithTag("rock");
            rock.transform.GetComponent<BoxCollider2D>().enabled = true;
        }
        
        jump = false;

    }

    private void Update()
    {
        if (jump)
        {
            if (player.transform.localScale.x < 1.5f)
            {
                float per = (Time.deltaTime * 1.5f);
                player.transform.localScale = new Vector3(player.transform.localScale.x + per,player.transform.localScale.y+per,player.transform.localScale.z+per);
                player.GetComponent<Animator>().enabled = false;
                
                
            }
        }
        else if(player != null)
        {
            if (player.transform.localScale.x > 1f)
            {
                float per = (Time.deltaTime * 1.5f);
                player.transform.localScale = new Vector3(player.transform.localScale.x - per,player.transform.localScale.y-per,player.transform.localScale.z-per);
                player.GetComponent<Animator>().enabled = true;
                
            }
        }
    }
}
