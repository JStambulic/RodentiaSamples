using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

/*
 Things to fix/change:
- change jump velocity based on predective destination for angled terrain
 */
public enum MovementState
{
    idle,
    running,
    hopping,
    dodging,
    inAir,
    wallSliding,
    wallJumping
}

/// <summary>Script for the player movement and core movement abilities.</summary>
public class PlayerMovement : MonoBehaviour
{
    #region Member Variables

    public MovementState state;

    // Inputs
    [Header("Player Input")]
    public PlayerInput playerInput;

    // Component Refs
    [Header("References")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform playerFront;
    [SerializeField] Transform playerBack;
    Rigidbody rb;
    public Rigidbody Rigidbody => rb;
    TrailRenderer tr;
    TailComponent tailComp;

    [Header("VFX")]
    [SerializeField] public VisualEffect VFXJump;
    [SerializeField] public GameObject VFXRun;
    VisualEffect VFXRun_Effect;
    GameObject VFXRun_Instance;
    [SerializeField] public float dustSpawnFequency = 0.3f;
    [SerializeField] GameObject dustSpawnPoint;
    float moveSoundFrequency = 0.0f;
    [Space(10)]


    [Header("Mass")]
    [SerializeField] float playerMass = 175f;
    [Space(10)]

    // Core Movement
    [Header("Movement")]
    Vector2 moveDirection;
    public Vector2 MoveDirection => moveDirection;
    float acceleration = 39.0f;
    public const float defaultMaxMoveSpeed = 18.0f;
    [SerializeField] float maxSpeed = 18.0f;
    [SerializeField] float speedReductionMultiplier = 0.95f;
    float smoothTurnVelocity;
    bool isGrounded;
    [Space(10)]

    public float slowMultiplier = 1.0f;

    //Slopes
    float maxSlopeAngle = 60.0f;
    RaycastHit slopeHit;

    // Dodging
    [Header("Dodge Roll")]
    [SerializeField] bool canDodge = true;
    public bool CanDodge => canDodge;

    [SerializeField] bool isDodging = false;
    public bool IsDodging => isDodging;

    float dodgePower = 30.0f;
    float dodgeTime = 0.5f;
    float dodgeCooldown = 0.8f;
    [Space(10)]

    // Jumping
    public const float defaultGravScale = 1.0f;
    public const float zeroGravScale = 0.1f;
    [Header("Jump")]
    [SerializeField, Range(0.0f, 1.0f)] float gravityScale = 1.0f;
    public float GravityScale => gravityScale;
    float fallGravityMultiplier = 1.5f;
    float jumpPower = 19.3f;
    float jumpCutoff = 0.71f;
    float maxVerticalSpeed = 50.0f;
    //float inAirDrag = 0f;

    // Double Jump
    [Header("Double Jump")]
    [SerializeField] bool canDoubleJump;
    float doubleJumpPower = 15.0f;

    // Jump Buffer
    [Header("Jump Buffer")]
    [SerializeField] float jumpBufferCounter;
    const float jumpBufferTime = 0.1f;

    // Coyote Time
    [Header("Coyote Time")]
    [SerializeField] float coyoteTimeCounter;
    [Space(10)]
    const float coyoteTime = 0.15f;

    // Wall Slide & Jump
    [Header("Wall Slide")]
    [SerializeField] bool isWallSliding = false;
    float wallSlideSpeed = 3.0f;
    float wallCheckDist = 0.7f;

    [Header("Wall Jump")]
    [SerializeField] bool isWallJumping = false;
    float wallJumpDuration = 0.2f;
    float wallJumpUpForce = 300;
    float wallJumpSideForce = 100;
    float playerSpinSpeed = 400.0f;

    RaycastHit wallHit = new RaycastHit();
    RaycastHit previousWallHit = new RaycastHit();

    // Animation Handler
    [Header("Animation")]
    [SerializeField] Animator animator;

    bool lockPlayerMovement = false;

    #endregion

    #region Start, Awake, Update, FixedUpdate

    void Awake()
    {
        if (GameManager.Get())
        {
            GameManager.Get().GetUIManager().CreatePlayerUI();
            GameManager.Get().GetUIManager().CreateDialogueUI();
        }

        if (playerInput)
        {
            playerInput.SwitchCurrentActionMap("Player");
            playerInput.currentActionMap.Enable();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main != null)
        {
            playerInput.camera = Camera.main;
        }

        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
        tailComp = GetComponent<TailComponent>();

        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
        }

        state = MovementState.idle;

        groundLayer = LayerMask.GetMask("Ground", "Land", "Tree");

        //wallLayer = LayerMask.GetMask("Wall", "Land", "Tree");
        wallLayer = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDodging)
        {
            return;
        }

        isGrounded = IsGrounded();

        if (isGrounded || isWallJumping)
        {
            canDoubleJump = true;
        }

        if (state != MovementState.hopping)
        {
            animator.SetBool("IsFalling", !isGrounded);
        }

        animator.SetInteger("MoveState", (int)state);
    }

    /// <summary>
    /// FixedUpdate for physics calculations.
    /// </summary>
    void FixedUpdate()
    {
        //Update player position variable for Shader Graph
        Shader.SetGlobalVector("_Player", transform.position);

        ModifyGravity();
        ApplyGravity();

        // IF is dodging, no movement code should be called.
        if (isDodging || lockPlayerMovement)
        {
            return;
        }

        if (!isWallJumping)
            ChangeDirectionFacing();

        // Movement
        MovePlayer();

        // Check if player is wall sliding.
        WallSlide();

        // Animator
        animator.SetFloat("RunVelocity", rb.velocity.magnitude);
    }

    #endregion

    #region Base Player Movement

    /// <summary>
    /// Event necessary for base player movement. Setup in Player Input component Events.
    /// </summary>
    /// <param name="context">The information sent by the Player Input component containing the Vector3 movement data.</param>
    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }

        if (context.canceled)
        {
            moveDirection = Vector2.zero;
        }
    }

    public void LockPlayerMovement()
    {
        lockPlayerMovement = true;
    }

    public void UnlockPlayerMovement()
    {
        lockPlayerMovement = false;
    }

    public void LockPlayerMovement(float time)
    {
        if(!lockPlayerMovement)
            StartCoroutine(TimedMovementLock(time));
    }

    IEnumerator TimedMovementLock(float time)
    {
        lockPlayerMovement = true;

        yield return new WaitForSeconds(time);

        lockPlayerMovement = false;
    }

    public void ApplyForce(float forceMultiplier, bool isSlash = true, Vector3 direction = default)
    {
        Vector3 forwardFromCam = ForwardDirFromCamera();
        moveForce = direction == Vector3.zero ? forwardFromCam == Vector3.zero ? transform.forward : forwardFromCam : direction; //ternaryception 
        
        if (isSlash)
        {
            StopPlayerHorizontalMomentum();
            transform.rotation = Quaternion.LookRotation(new Vector3(moveForce.x, 0.0f, moveForce.z) * 2, Vector3.up);
            moveForce = OnSlope() ? GetSlopeMoveDir(moveForce) : moveForce;
        }

        rb.AddForce(forceMultiplier * slowMultiplier * moveForce, ForceMode.Impulse);
    }

    private Vector3 moveForce;
    /// <summary>
    /// Applies force to player to simulate acceleration to max speed. 
    /// Modifies Drag depending on whether player is grounded or not.
    /// </summary>
    void MovePlayer()
    {
        if (lockPlayerMovement)
        {
            moveForce = Vector3.zero;
        }
        else
        {
            moveForce = ForwardDirFromCamera();
        }

        // If no input, or you are pressed against a wall
        if (moveForce == Vector3.zero || IsWalled())
        {
            if (state == MovementState.running)
                state = MovementState.idle;

            DampHorizontalVelocity();

            return;
        }

        if (isGrounded && state != MovementState.inAir)
        {
            GroundMovement(moveForce);
        }
        else if (!isWallSliding && !isWallJumping)
        {
            AirMovement(moveForce);
        }

        // VFX
        if (state == MovementState.running)
        {
            if (VFXRun)
            {
                VFXRun_Effect = VFXRun.GetComponent<VisualEffect>();
                if (VFXRun_Effect)
                {
                    VFXEventAttribute EventSettings = VFXRun_Effect.CreateVFXEventAttribute();
                    VFXRun_Effect.SendEvent("Run", EventSettings);

                    if (dustSpawnFequency > 0.1f)
                    {
                        VFXRun_Instance = ObjectPoolManager.Get(VFXRun);
                        VFXRun_Instance.transform.position = dustSpawnPoint.transform.position;

                        StartCoroutine(HelperFunctions.DisableAfterTime(VFXRun_Instance, 0.5f));
                        dustSpawnFequency = 0.0f;
                    }
                    else
                    {
                        dustSpawnFequency += Time.fixedDeltaTime;
                    }
                }
            }

            //if (moveSoundFrequency > 0.3f)
            //{
            //    AudioManager.PlaySound(SFXType.BinkRun, true, false, 0.75f);
            //    moveSoundFrequency = 0.0f;
            //}
            //else
            //{
            //    moveSoundFrequency += Time.fixedDeltaTime;
            //}
        }

        ControlMoveSpeed();
    }

    private Vector3 flatVel;
    private Vector3 hopVelocity;
    /// <summary>
    /// Movement when on ground.
    /// </summary>
    /// <param name="moveForce">Forward direction from camera.</param>
    void GroundMovement(Vector3 moveForce)
    {
        // On Slope Running.
        if (OnSlope()/* && state != MovementState.hopping*/)
        {
            rb.AddForce(2.0f * acceleration * GetSlopeMoveDir(moveForce) * slowMultiplier, ForceMode.Acceleration);
            state = MovementState.running;
        }
        // Running.
        else/* if (state != MovementState.hopping && hopStep == 1)*/
        {
            rb.AddForce(moveForce * acceleration * slowMultiplier, ForceMode.Acceleration);
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 0.0001f, rb.velocity.z);
            state = MovementState.running;
        }
    }

    /// <summary>
    /// Movement for when in air.
    /// </summary>
    /// <param name="moveForce">Forward direction from camera.</param>
    void AirMovement(Vector3 moveForce)
    {
        rb.AddForce(moveForce * acceleration * slowMultiplier, ForceMode.Acceleration);
    }

    private Vector3 cappedVel;
    private Vector3 newFlatVel;
    /// <summary>
    /// Caps the rigidbody's velocity to maxSpeed.
    /// </summary>
    void ControlMoveSpeed()
    {
        newFlatVel = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

        // Limit velocity to maxSpeed
        if (newFlatVel.magnitude > maxSpeed)
        {
            //Debug.Log("Capping Velocity: " + flatVel.magnitude.ToString());
            cappedVel = newFlatVel.normalized * maxSpeed * slowMultiplier;
            rb.velocity = new Vector3(cappedVel.x, rb.velocity.y, cappedVel.z);
        }
    }

    /// <summary>
    /// Slows the player's horizontal velocity when called.
    /// </summary>
    void DampHorizontalVelocity()
    {
        rb.velocity = new(rb.velocity.x * speedReductionMultiplier, rb.velocity.y, rb.velocity.z * speedReductionMultiplier);
    }

    private Vector3 camForward;
    private Vector3 camRight;
    private Vector3 newRight;
    private Vector3 newForward;

    private Vector3 forwardDirMovement;
    /// <summary>
    /// Translates player moveDirection with camera position to ensure W always moves you forward.
    /// </summary>
    /// <returns>Vector2 of new movementDirection in relation to the camera.</returns>
    Vector3 ForwardDirFromCamera()
    {
        if (playerInput)
        {
            camForward = playerInput.camera.transform.forward;
            camRight = playerInput.camera.transform.right;

            // Zero out the Y values!
            camForward.y = 0.0f;
            camRight.y = 0.0f;

            camForward.Normalize();
            camRight.Normalize();

            newRight = moveDirection.x * camRight;
            newForward = moveDirection.y * camForward;

            forwardDirMovement = newRight + newForward;
            forwardDirMovement.Normalize();

            return new(forwardDirMovement.x, 0.0f, forwardDirMovement.z);
        }
        return new(moveDirection.x, 0.0f, moveDirection.y);
    }

    Vector3 facingMovement;
    /// <summary>
    /// Changes the direction the player mesh is facing.
    /// </summary>
    void ChangeDirectionFacing()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            // Faces moveDirection.
            facingMovement = ForwardDirFromCamera();

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(facingMovement, Vector3.up), 0.2f);
        }
    }

    Quaternion targetRot;
    public void LockDirectionFacing(Transform target)
    {
        if (target)
        {
            targetRot = Quaternion.LookRotation(target.transform.position - transform.position);

            targetRot.x = 0.0f;
            targetRot.z = 0.0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * playerSpinSpeed);
        }
    }

    public void LockDirectionFacing(Vector3 target)
    {
        targetRot = Quaternion.LookRotation(target - transform.position);

        targetRot.x = 0.0f;
        targetRot.z = 0.0f;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * playerSpinSpeed);
    }

    /// <summary>
    /// Flips the direction the player mesh is facing by 180 degrees over time.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    IEnumerator FlipDirectionFacing()
    {
        float rotationAmount = 0.0f;
        while (rotationAmount < 180.0f)
        {
            float rotationStep = playerSpinSpeed * 2.0f * Time.deltaTime;
            rotationAmount += rotationStep;
            transform.Rotate(0.0f, rotationStep, 0.0f);

            yield return null;
        }
    }

    Vector3 GetSlopeMoveDir(Vector3 moveForce)
    {
        return Vector3.ProjectOnPlane(moveForce, slopeHit.normal).normalized;
    }

    /// <summary>
    /// Cuts off all current Player movement.
    /// </summary>
    public void StopPlayerMovement()
    {
        rb.velocity = Vector3.zero;
        moveDirection = Vector2.zero;
    }

    /// <summary>
    /// Cuts off all current Player horizontal movement.
    /// </summary>
    public void StopPlayerHorizontalMomentum()
    {
        rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
    }

    /// <summary>
    /// Increases Max Speed to allow for much faster movement.
    /// </summary>
    public void BoostSpeed(float time)
    {
        StartCoroutine(ApplySpeedBoost(time));
        StartCoroutine(GameManager.Get().GetUIManager().GetPlayerUIComp().HUDEffects.EnableSpeedBoostVisual(time));
    }

    IEnumerator ApplySpeedBoost(float time)
    {
        maxSpeed *= 2.5f;
        //Debug.Log(maxSpeed.ToString());

        yield return new WaitForSeconds(time);

        maxSpeed = defaultMaxMoveSpeed;
        //Debug.Log(maxSpeed.ToString());
    }

    #endregion

    #region Jumping & Gravity

    /// <summary>
    /// Checks whether player is currently falling and increases gravity scale.
    /// </summary>
    void ModifyGravity()
    {
        if (isDodging)
        {
            return;
        }

        if (tailComp.isSlinging == true)
        {
            gravityScale = zeroGravScale;
        }
        else if (rb.velocity.y < -5.0f && state != MovementState.hopping && tailComp.isGrappling == false)
        {
            gravityScale = defaultGravScale * fallGravityMultiplier;
        }
        else
        {
            gravityScale = defaultGravScale;
        }
    }

    /// <summary>
    /// Applies custom gravity on the RigidBody.
    /// </summary>
    void ApplyGravity()
    {
        // Custom RB Gravity
        rb.AddForce(gravityScale * playerMass * Time.fixedDeltaTime * Physics.gravity, ForceMode.Acceleration);

        // Limit how fast player can fall and ascend.
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxVerticalSpeed, maxVerticalSpeed), rb.velocity.z);
    }

    /// <summary>
    /// Changes the Player gravity scale.
    /// </summary>
    /// <param name="gravity">New gravity scale.</param>
    public void ModifyGravityScale(float gravity)
    {
        gravityScale = gravity;
    }

    /// <summary>
    /// Event for when Jump action is performed. Height achieved is variable depending on how long jump input is held.
    /// </summary>
    /// <param name="context">The information sent by the Player Input component containing the jump input data.</param>
    public void Jump(InputAction.CallbackContext context)
    {
        SetCoyoteTime(context);

        SetJumpBuffer(context);

        if (context.performed && GetComponent<TailComponent>().isGrappling == true)
        {
            return;
        }

        if (context.performed && isWallSliding)
        {
            GameManager.Get().CombatManager.InterruptCombo();

            state = MovementState.wallJumping;
            WallJump();
            return;
        }

        if (context.performed && (canDoubleJump || (coyoteTimeCounter > 0.0f && jumpBufferCounter > 0.0f)))
        {
            if (isGrounded || canDoubleJump)
            {
                state = MovementState.inAir;

                // Zero out the Y vel before applying jump force.
                rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
                rb.velocity = new Vector3(rb.velocity.x, canDoubleJump ? doubleJumpPower : jumpPower, rb.velocity.z);

                canDoubleJump = !canDoubleJump;

                GameManager.Get().CombatManager.InterruptCombo();
                GameManager.Get().CombatManager.SetIsAirborne(true);

                AudioManager.PlaySound(SFXType.BinkJump, true);

                // Vfx Caller
                if (VFXJump)
                {
                    VFXEventAttribute EventSettings = VFXJump.CreateVFXEventAttribute();
                    VFXJump.SendEvent("Jumping", EventSettings);
                }

                animator.SetTrigger("Jump");
            }

            jumpBufferCounter = 0.0f;
        }
    }

    /// <summary>
    /// Sets the jump buffer, for when jump is pressed slightly before touching ground. Stores the input to be used the immediate grounded frame.
    /// </summary>
    /// <param name="context">The information sent by the Player Input component containing the jump input data.</param>
    void SetJumpBuffer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Coyote time for allowing jumps even slightly after walking off a ledge.
    /// </summary>
    /// <param name="context">The information sent by the Player Input component containing the jump input data.</param>
    void SetCoyoteTime(InputAction.CallbackContext context)
    {
        if (isGrounded && !context.performed)
        {
            coyoteTimeCounter = coyoteTime;
            canDoubleJump = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Public function for resetting Double Jump.
    /// </summary>
    public void DetachFromGrapple()
    {
        canDoubleJump = true;
    }

    #endregion

    #region Wall Slide and Wall Jump

    /// <summary>Allows player to slowly slide down a wall when pressed against it.</summary>
    /// <returns>Void.</returns>
    void WallSlide()
    {
        if (IsWalled() && !isGrounded)
        {
            //Debug.Log("Is Wall Sliding.");

            isWallSliding = true;
            rb.velocity = new Vector3(rb.velocity.x / 2.0f, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, maxVerticalSpeed), rb.velocity.z / 2.0f);

            if (state == MovementState.wallSliding && VFXRun)
            {
                VFXRun_Effect = VFXRun.GetComponent<VisualEffect>();
                if (VFXRun_Effect)
                {
                    VFXEventAttribute EventSettings = VFXRun_Effect.CreateVFXEventAttribute();
                    VFXRun_Effect.SendEvent("Run", EventSettings);

                    if (dustSpawnFequency > 0.1f)
                    {
                        if (AudioManager.instance != null)
                            AudioManager.PlaySound(SFXType.BinkWallSlide, true);

                        VFXRun_Instance = ObjectPoolManager.Get(VFXRun);
                        VFXRun_Instance.transform.position = dustSpawnPoint.transform.position;

                        StartCoroutine(HelperFunctions.DisableAfterTime(VFXRun_Instance, 0.5f));
                        dustSpawnFequency = 0.0f;
                    }
                    else
                    {
                        dustSpawnFequency += Time.fixedDeltaTime;
                    }
                }
            }

            state = MovementState.wallSliding;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private Vector3 wallNormal;
    private Vector3 wallJumpForce;
    /// <summary>When wall sliding, a wall jump can be performed, launching the player in the direction opposite to the wall.</summary>
    /// <returns>Void.</returns>
    void WallJump()
    {
        if (previousWallHit.collider != null)
        {
            if (wallHit.transform.gameObject == previousWallHit.transform.gameObject)
            {
                return;
            }
        }

        if (AudioManager.instance != null)
            AudioManager.PlaySound(SFXType.BinkJump, true);

        StartCoroutine(FlipDirectionFacing());

        previousWallHit = wallHit;

        isWallJumping = true;
        state = MovementState.wallJumping;

        wallNormal = wallHit.normal;

        wallJumpForce = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        rb.AddForce(wallJumpForce, ForceMode.Impulse);

        Invoke(nameof(StopWallJumping), wallJumpDuration);
    }

    /// <summary>Invoked when wall climb ends.</summary>
    /// <returns>Void.</returns>
    void StopWallJumping()
    {
        isWallJumping = false;
        state = MovementState.inAir;
    }

    /// <summary>Resets the wall hits when Player becomes grounded.</summary>
    /// <returns>Void.</returns>
    void ResetWalls()
    {
        wallHit = new RaycastHit();
        previousWallHit = new RaycastHit();
    }

    #endregion

    #region Dodge Roll

    /// <summary>
    /// Event called when Dodge input is performed. Runs the Dodge Coroutine.
    /// </summary>
    /// <param name="context">The information sent by the Player Input component containing the dodge roll input data.</param>
    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.performed && canDodge && state != MovementState.inAir)
        {
            StartCoroutine(Dodge());

            AudioManager.PlaySound(SFXType.BinkDash, true);

            animator.SetTrigger("Dodge");
            GameManager.Get().CombatManager.InterruptCombo();
        }
    }

    private Vector3 dodgeForward;
    private Vector3 dodgeForce;
    /// <summary>
    /// Coroutine called by Dodge event. Performs the dodge and runs timers for the amount of time to dodge, and the following cooldown.
    /// </summary>
    IEnumerator Dodge()
    {
        state = MovementState.dodging;

        canDodge = false;
        isDodging = true;

        if (tr != null) { tr.emitting = true; }

        gravityScale = defaultGravScale / 2.0f;

        dodgeForward = ForwardDirFromCamera();
        if (dodgeForward.magnitude.Equals(0))
        {
            dodgeForward = -transform.forward;
        }

        GameManager.Get().CombatManager.InterruptCombo();

        // Launch at a very slight upward angle, so you don't clip the ground.
        StopPlayerHorizontalMomentum();
        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        dodgeForce = new Vector3(dodgeForward.x * dodgePower * 35, 250.0f, dodgeForward.z * dodgePower * 35);
        rb.AddForce(dodgeForce, ForceMode.Acceleration);

        dodgeForce.y = 0.0f;
        dodgeForce.Normalize();

        dodgeForce = Quaternion.Inverse(transform.rotation) * dodgeForce;

        animator.SetFloat("DashX", dodgeForce.x);
        animator.SetFloat("DashZ", dodgeForce.z);

        yield return new WaitForSeconds(dodgeTime);

        state = MovementState.idle;
        gravityScale = defaultGravScale;
        isDodging = false;
        if (tr != null) { tr.emitting = false; }

        GameManager.Get().CombatManager.InterruptCombo();

        if (lockPlayerMovement)
            UnlockPlayerMovement();

        GameManager.Get().CombatManager.SetLockAttack(false);

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    #endregion

    #region Checks

    /// <summary>
    /// Ground check using a CheckSphere.
    /// </summary>
    public bool IsGrounded()
    {
        bool yes = Physics.Raycast(transform.position, -transform.up, out RaycastHit groundHit, 1.5f, groundLayer);

        if (yes)
        {
            ResetWalls();
        }

        return yes;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.0f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0.0f;
        }

        return false;
    }

    /// <summary>
    /// Wall check using a Raycast.
    /// </summary>
    public bool IsWalled()
    {
        if (playerFront)
        {
            return Physics.SphereCast(playerFront.position, 0.25f, playerFront.forward, out wallHit, 1.0f, wallLayer);
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Land")) && IsGrounded())
        {

            GameManager.Get().CombatManager.SetIsAirborne(false);

            isGrounded = true;

            if(state == MovementState.inAir)
            {
                //StopPlayerHorizontalMomentum();
                state = MovementState.idle;
            }

            //if (lockPlayerMovement)
            //    UnlockPlayerMovement();
        }
    }

    #endregion
}
