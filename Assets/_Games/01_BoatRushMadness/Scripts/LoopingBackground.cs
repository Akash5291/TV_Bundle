using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopingBackground : MonoBehaviour
{
    
    [SerializeField] private RawImage img;
    public float speed = 0.3f;
    public static LoopingBackground instance;

    private void Awake()
    {
        instance = this;
    }


    void Update()
    {

        img.uvRect = new Rect(img.uvRect.position + new Vector2(speed * Time.deltaTime,0) , img.uvRect.size);
        
    }
}
