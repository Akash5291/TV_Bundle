using UnityEngine;
using System.Collections;

public class GroundFunction : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform endPoint;
    [SerializeField] LayerMask player;

    [Header("Values")]
    [SerializeField] float rayDistance;
    [SerializeField] Vector3 _position;
    [SerializeField] float groundScale = 13.6f;
    [SerializeField] WaitForSeconds twoPointFive = new WaitForSeconds(2.5f);

    [Header("Levels")]
    [SerializeField] GameObject currentLevel;
    [SerializeField] Transform levelSpawnPlase;
    [SerializeField] GameObject[] level1;

    [SerializeField] bool coroutineStarted = false;
    [SerializeField] bool canSpawn = false;
    bool gameOver = false;

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

    private void Start()
    {
        if (canSpawn)
            SpawnLevel();
    }

    private void Update()
    {
        if (coroutineStarted)
            return;
        Debug.DrawRay(endPoint.position, Vector2.up * rayDistance);
        if (Physics2D.Raycast(endPoint.position, Vector2.up, rayDistance, player))
        { 
            Debug.Log($"Player crossed");
            StartCoroutine(nameof(ChangePosition));
        }
    }

    IEnumerator ChangePosition()
    {
        coroutineStarted = true;
        yield return twoPointFive;

        if (!gameOver)
        {
            _position.x = transform.position.x;
            _position.x += groundScale;
            _position.y = transform.position.y;
            _position.z = transform.position.z;

            transform.position = _position;

            SpawnLevel();
            coroutineStarted = false;

        }
    }

    void SpawnLevel()
    {
        if (currentLevel)
        {
            Destroy(currentLevel);
        }
        int _random = Random.Range(0, level1.Length);
        currentLevel = Instantiate(level1[_random], levelSpawnPlase.position, Quaternion.identity);
    }
}
