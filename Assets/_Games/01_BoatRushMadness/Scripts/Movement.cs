using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Movement : MonoBehaviour
{
    private bool isPressed = false;
    public float speed;
    public GameObject player;
    bool isMoveUp = false;
    RectTransform boat;
    [SerializeField] StarCollect playerCollide;
    [SerializeField] float rotateSpeed = 0f;


    private void Start()
    {
        boat = player.transform.GetComponent<RectTransform>();
    }
    
    private void Update()
    {
        if (isPressed)
        {
            if (isMoveUp && !playerCollide.topCollider)
            {
                player.transform.localPosition = new Vector2(player.transform.localPosition.x, player.transform.localPosition.y + (Time.deltaTime * speed));
                if ((int)boat.localEulerAngles.z >= 340 || (int)boat.localEulerAngles.z < 20)
                    player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, player.transform.localEulerAngles.y, player.transform.localEulerAngles.z + rotateSpeed);
            }
            else if (!isMoveUp && !playerCollide.downCollider)
            {
                player.transform.localPosition = new Vector2(player.transform.localPosition.x, player.transform.localPosition.y - (Time.deltaTime * speed));
                if ((int)boat.localEulerAngles.z > 340 || (int)boat.localEulerAngles.z <= 20)
                    player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, player.transform.localEulerAngles.y, player.transform.localEulerAngles.z - rotateSpeed);
            }
        }
        else
        {
            player.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    public void OnPointerDown(bool value)
    {
        isPressed = true;
        isMoveUp = value;
    }

    public void OnPointerUp(bool value)
    {
        isPressed = false;
        isMoveUp = value;
    }
   
}
