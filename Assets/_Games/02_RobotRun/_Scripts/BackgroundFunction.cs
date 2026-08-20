using UnityEngine;
using System.Collections;

public class BackgroundFunction : MonoBehaviour
{
    [Header ("Components")]
    [SerializeField] LayerMask player;
    [SerializeField] Transform nextPosition;
    [SerializeField] Transform endPoint;

    [Space]
    [Header ("Values")]
    [SerializeField] float rayDistance;
    [SerializeField] bool coroutineStarted = false;
    [SerializeField] bool gameOver = false;

    WaitForSeconds fiveSeconds = new WaitForSeconds(5);

    #region Actions
    private void OnEnable()
    {
        PlayerHitDetection.HitObstacles += PlayerHitObstacles;
    }

    private void OnDisable()
    {
        PlayerHitDetection.HitObstacles -= PlayerHitObstacles;
    }

    void PlayerHitObstacles()
    {
        gameOver = true;
    }
    #endregion

    private void Update()
    {
        if (coroutineStarted)
            return;
        Debug.DrawRay(endPoint.position, Vector2.down * rayDistance, color: Color.red);
        if (Physics2D.Raycast(endPoint.position, Vector2.down, rayDistance, player))
        {
            Debug.Log($"Player crossed");
            StartCoroutine(nameof(ChangePosition));
        }
    }

    IEnumerator ChangePosition()
    {
        coroutineStarted = true;
        yield return fiveSeconds;

        if (!gameOver)
        {
            transform.position = nextPosition.position;
            coroutineStarted = false;
        }
    }
}
