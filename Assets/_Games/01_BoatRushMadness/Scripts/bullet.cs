using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed ;

    public Rigidbody2D rb;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb.linearVelocity = new Vector2(speed , 0);
    }

    
}
