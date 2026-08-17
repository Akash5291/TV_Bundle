using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    
    void Update()
    {
        transform.localPosition = new Vector2(transform.localPosition.x - (Time.deltaTime * LoopingBackground.instance.speed * 1000f), transform.localPosition.y);
    }
}
