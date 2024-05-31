using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundChecker))]
public class Movement : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
	[SerializeField] private float maxSpeed = 18.0f;
    [SerializeField] private float groundAcceleration = 20f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float airAcceleration = 2f;
    private Vector2 moveInput;
    private Rigidbody rb;
    private GroundChecker groundChecker;


    [HideInInspector] public bool isKnocked = false;

    // DEBUG TODO:REMOVE ON RELEASE
    PlayerStats playerStats;
    public Rigidbody RB { get => rb; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponent<GroundChecker>();
        
        playerStats= GetComponent<PlayerStats>(); // REMOVE ON BUILD
    }

    private void Update()
    {
        // TODO: Remove before build
       /* if (appropriatlySpawned) return;
        #region Debug
        if (playerStats == null) return;
        if (playerStats.playerIndex != 0) return;

        int x = 0;
        int z = 0;
        if (Input.GetKey(KeyCode.A))
        {
            x--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x++;
        }

        if (Input.GetKey(KeyCode.W))
        {
            z++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            z--;
        }

        moveInput= new Vector2(x, z);
        #endregion
       */
    }

    void FixedUpdate()
	{
        MoveLogic(); // Physics simulation runs on fixed update, thus our physics based move logic should be ran here too
    }

    private void MoveLogic()
    {
        if (StaticStats.gameOver)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        
        Vector3 preservedFallingVelocity = rb.velocity;

        // Calculate current acceleration
        float currentAcceleration = !groundChecker.IsGrounded ? airAcceleration : groundAcceleration;
        if(isKnocked)
        {
            currentAcceleration *= 0.1f;
        }

        // Acceleration
        if (moveInput != Vector2.zero)
        {
            Vector3 translatedMovement = new Vector3(moveInput.x, 0, moveInput.y);
            rb.velocity += currentAcceleration * maxSpeed * Time.fixedDeltaTime * translatedMovement;
            if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed) rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else if (!isKnocked)
        {
            // Deacceleration
            rb.velocity -= rb.velocity * deceleration * Time.fixedDeltaTime;
            if (rb.velocity.sqrMagnitude < 0.2f) rb.velocity = Vector3.zero;
        }

        Vector3 newVelocity = rb.velocity;
        newVelocity.y = preservedFallingVelocity.y;
        if (Mathf.Abs(newVelocity.y) < 0.1f) newVelocity.y = 0f;

        rb.velocity = newVelocity;
}

    private void OnMovement(InputValue value)
    {
        if (StaticStats.gameOver) return;
        
        moveInput = value.Get<Vector2>();

        float x = moveInput.x;
        float z = moveInput.y;

        if (Mathf.Abs(x) < 0.3f) x = 0;
        if (Mathf.Abs(z) < 0.3f) z = 0;

        moveInput = new Vector2(x, z);
    }
}
