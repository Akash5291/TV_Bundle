using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    GameManager gameManager;
    [Header ("Components")]
    [SerializeField] Rigidbody2D backgroundHolder;

    [Space]
    [Header ("Values")]
    [SerializeField] float effectSpeed;

    Vector3 direction;

    private void Start() => gameManager = GameManager.instance;

    private void Update()
    {
        if (gameManager.gameState == GameManager.GameState.Playing)
        {
            direction.x = effectSpeed;
            direction.y = 0;
            direction.z = 0;

            backgroundHolder.linearVelocity = direction;
        }
        else
        {
            backgroundHolder.linearVelocity = Vector3.zero;
        }
    }
}
