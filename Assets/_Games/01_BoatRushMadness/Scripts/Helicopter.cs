using System;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private GameObject missile;
    [SerializeField] private GameObject helicopter;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Instantiate(missile, helicopter.transform.position,Quaternion.identity, transform.parent.transform);
        }
    }
}
