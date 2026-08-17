using UnityEngine;

public class PowerShield : MonoBehaviour
{
    public static PowerShield instance;
    public GameObject Shield;
    public bool shieldActive = false;


    private void Start()
    {
        instance = this;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("shield"))
        {
            Destroy(other.transform.gameObject);
            
            Shield.SetActive(true);
            shieldActive = true;
            Invoke(nameof(ShieldDeactivate), 10f);
        }
    }

    void ShieldDeactivate()
    {
        Shield.SetActive(false);
        shieldActive = false;
    }
}
