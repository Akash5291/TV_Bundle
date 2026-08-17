using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletPrefab;
    public BulletCounter BulletCounter;
    public Transform bulletPos;

    public void OnShootBtnDown()
    {
        if (BulletCounter.currentBullets > 0)
        {
            StartCoroutine("Shoot");
            transform.GetComponent<Button>().interactable = false;
        }
        else
        {
            transform.GetComponent<Button>().interactable = false;
        }
    }

    IEnumerator Shoot()
    {
        for (int x = 1; x <= 3; x++)
        {
            
            var obj= Instantiate(bulletPrefab , firePoint.transform);
            obj.transform.localPosition = new Vector3(bulletPos.localPosition.x + 50f,bulletPos.localPosition.y,bulletPos.localPosition.z);
            AudioManager.instance.PlaySound("shootsound");
            
            yield return new WaitForSeconds(0.1f);
            
        }
        BulletCounter.currentBullets = BulletCounter.currentBullets - 1;
        BulletCounter.bulletText.text = BulletCounter.currentBullets.ToString();
        transform.GetComponent<Button>().interactable = true;
        
    }
}
