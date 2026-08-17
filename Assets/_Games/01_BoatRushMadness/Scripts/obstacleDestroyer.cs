using UnityEngine;

public class obstacleDestroyer : MonoBehaviour
{
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("destroy"))
        {
            Destroy(other.transform.parent.transform.gameObject);
            

        }

        
    }
}
