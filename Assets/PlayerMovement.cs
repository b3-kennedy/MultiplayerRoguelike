using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public enum PlayerState { NORMAL, SPRINT };
    public PlayerState state;




    public float normalSpeed = 5f;
    public float sprintSpeed = 7f;
    public float acceleration = 3f;
    public float groundDrag = 4f;

    public Transform groundCheck;

    public Transform orientation;

    public float jumpForce = 5f;
    public float airMultiplier = 0.1f;

    float horizontal;
    float vertical;



    bool normalOnLand = false;

    bool wasAirborne;

    Vector3 moveDir;
    Rigidbody rb;

    public float landingSlowdownFactor = 0.5f; // Percentage of speed after landing
    public float landingSlowdownDuration = 1.0f; // Time in seconds to recover full speed
    private float currentSlowdownTime = 0f;
    private bool isRecoveringSpeed = false;

    public bool isOnLadder;

    bool jumpOffLadder = false;

    float targetSpeedOnLand;

    bool isSprinting = false;
    public float sprintMultiplier = 1.5f;
    float smoothVel;


    [Header("Debugging")]
    public float targetSpeed;
    public float speed;
    public float magnitude;
    PlayerLook playerLook;
    Transform cam;

    string objectId;

    string camId;



    // Start is called before the first frame update
    void Start()
    {
        speed = normalSpeed;
        targetSpeed = normalSpeed;
        state = PlayerState.NORMAL;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerLook = GetComponent<PlayerLook>();
        if (playerLook.cam)
        {
            cam = playerLook.cam;
        }

    }

    void LadderControl()
    {
        isOnLadder = CheckIfOnLadder();

        if (isOnLadder && IsGrounded() && vertical < 0)
        {
            isOnLadder = false;
        }

        if (isOnLadder && Input.GetKeyDown(KeyCode.Space))
        {
            jumpOffLadder = true;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;


        LadderControl();


        magnitude = rb.linearVelocity.magnitude;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Calculate movement direction
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        // Check if the player is moving forward relative to their orientation
        bool isMovingForward = Input.GetKey(KeyCode.W);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }



        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (isMovingForward && IsGrounded())
            {
                isSprinting = true;
                state = PlayerState.SPRINT;
                targetSpeed = sprintSpeed;
            }
            else if (!isMovingForward && IsGrounded())
            {
                isSprinting = false;
                state = PlayerState.NORMAL;
                targetSpeed = normalSpeed;
                speed = normalSpeed;
            }

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && IsGrounded())
        {
            isSprinting = false;
            state = PlayerState.NORMAL;
            speed = normalSpeed;
            targetSpeed = normalSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && !IsGrounded())
        {
            normalOnLand = true;
        }

        if (IsGrounded() && normalOnLand)
        {
            isSprinting = false;
            state = PlayerState.NORMAL;
            speed = normalSpeed;
            targetSpeed = normalSpeed;
            normalOnLand = false;
        }



        bool grounded = IsGrounded();



        if (wasAirborne && grounded)
        {
            //OnLand();
            wasAirborne = false;
        }

        if (!grounded)
        {
            wasAirborne = true;
        }

        if (isSprinting)
        {
            speed = Mathf.MoveTowards(speed, sprintSpeed, Time.deltaTime * sprintMultiplier);
        }

        // Gradually recover speed after landing
        if (isRecoveringSpeed)
        {
            currentSlowdownTime += Time.deltaTime;
            speed = Mathf.Lerp(targetSpeedOnLand * landingSlowdownFactor, targetSpeedOnLand, currentSlowdownTime / landingSlowdownDuration);

            if (currentSlowdownTime >= landingSlowdownDuration)
            {
                isRecoveringSpeed = false;
                speed = targetSpeedOnLand; // Ensure full speed is restored
            }
        }

        SpeedControl();

        if (IsGrounded())
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }


    bool CheckIfOnLadder()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1.1f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Ladder>())
            {
                return true;
            }
        }
        return false;
    }

    void OnLand()
    {
        landingSlowdownFactor = (1 - (targetSpeed / 10));
        targetSpeedOnLand = targetSpeed;
        //rb.velocity = Vector3.zero;
        StartLandingSlowdown();
    }

    void StartLandingSlowdown()
    {
        isRecoveringSpeed = true;
        currentSlowdownTime = 0f;
        speed = targetSpeedOnLand * landingSlowdownFactor; // Initial slowdown
    }

    public bool IsGrounded()
    {
        bool rayHit = Physics.Raycast(groundCheck.position, -transform.up, 0.51f);

        return (isOnSlope() || rayHit);
    }

    public bool isOnSlope()
    {
        if (Physics.Raycast(groundCheck.position, -transform.up, out RaycastHit hit, 1f))
        {
            if (hit.normal.y != 1)
            {
                return true;
            }
        }
        return false;
        //Debug.Log()
        //return );
    }

    private void FixedUpdate()
    {

        if (isOnLadder)
        {
            LadderMovement();
            if (jumpOffLadder)
            {
                JumpOffLadder();
                jumpOffLadder = false;
            }
            return;
        }

        MovePlayer();



    }

    void JumpOffLadder()
    {
        isOnLadder = false;

        // Cancel current vertical motion
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Apply jump force upward
        rb.AddForce(-orientation.forward * jumpForce/2f, ForceMode.Impulse);
    }

    void LadderMovement()
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = vertical * speed;
        rb.linearVelocity = vel;
    }

    void MovePlayer()
    {

        moveDir = orientation.forward * vertical + orientation.right * horizontal;

        if (IsGrounded())
        {
            rb.AddForce(moveDir.normalized * speed * 10f, ForceMode.Force);

        }
        else
        {
            rb.AddForce(moveDir.normalized * speed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}