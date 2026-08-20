using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header ("Components")]
    [SerializeField] Transform followTarger;

    [Space]
    [Header ("Camera values")]
    [SerializeField] float smoothnessRate;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] Vector3 offset;

    private void FixedUpdate()
    {
        targetPosition.x = followTarger.position.x;
        Vector3 _targerPosition = targetPosition + offset;
        transform.position = Vector3.Lerp(transform.position, _targerPosition, smoothnessRate * Time.fixedDeltaTime);
    }
}
