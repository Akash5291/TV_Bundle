using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance = null;

    GameManager gameManager;
    [SerializeField] InGameSoundManager ingameSoundInstance;
    enum GroundStatus
    {
        Grounded,
        NotGrounded
    }
    [Header("Components")]
    [SerializeField] GroundStatus groundStatus;
    [SerializeField] Rigidbody2D player;
    [SerializeField] Animator playerAnimation;
    [SerializeField] Transform legs;
    [SerializeField] LayerMask groundLayer;

    [Space]
    [Header("Animation tags")]
    [SerializeField] string runTag;
    [SerializeField] string deadTag;

    [Space]
    [Header ("Values")]
    [Range (0, 5)]
    [SerializeField] float playerSpeed;
    [Range (0, 5)]
    [SerializeField] float jetpackForce;
    [Range (0f, 1f)]
    [SerializeField] float groundCheckDistance;
    [Space]

    Vector2 runDirection;
    bool poinerDown = false;

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
        playerAnimation.SetTrigger(deadTag);
    }
    #endregion

    private void Start()
    {
        if (Instance == null)
            Instance = this;

        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (gameManager.gameState == GameManager.GameState.Playing)
        {
            PlayerRun(playerSpeed);
            GroundCheck();
            ManageAnimation();

            if (poinerDown)
                PlayerFly();
        }
        else PlayerRun(0);
    }

    void PlayerRun(float _speed)
    {
        runDirection.x = _speed;
        runDirection.y = player.linearVelocity.y;

        player.linearVelocity = runDirection;
    }

    void ManageAnimation()
    {
        if (groundStatus == GroundStatus.Grounded)
        {
            playerAnimation.SetBool(runTag, true);
            ingameSoundInstance.StopJetSound();
        }

        else
        {
            playerAnimation.SetBool(runTag, false);
            ingameSoundInstance.PlayJetSound();
        }
    }

    void GroundCheck()
    {
        if (Physics2D.Raycast(legs.position, -Vector2.up, groundCheckDistance, groundLayer))
            groundStatus = GroundStatus.Grounded;
        
        else
            groundStatus = GroundStatus.NotGrounded;
    }

    void PlayerFly()
    {
        runDirection.x = player.linearVelocity.x;
        runDirection.y = jetpackForce;

        player.linearVelocity = runDirection;
    }

    #region Mobile inputs
    public void B_PointerDown()
    {
        poinerDown = true;
    }

    public void B_PointerUp()
    {
        poinerDown = false;
    }
    #endregion
}
