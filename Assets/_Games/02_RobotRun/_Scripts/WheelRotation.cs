using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [Range (0, 10)]
    [SerializeField] float speed;

    private void Update()
    {
        transform.Rotate(0, 0, speed);
    }
}
