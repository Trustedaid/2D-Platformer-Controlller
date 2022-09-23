using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float jumpForce = 11f;
    private float lastDash = -100f;
    private float knockbackStartTime;
    [SerializeField]
    private float knockbackDuration;
   

    private Rigidbody2D rb; // defines Rigidbody as rb
    private Animator anim;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpDirection;

    public float movementSpeed = 10f; // publicly shows that character movement speed in panel
    
    public float wallCheckDistance;
    public float groundCheckRadius;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 1f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet = 0.15f;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCoolDown;
 



    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;
    
    public LayerMask whatIsGround;
    
    public int amountOfJumps = 1; 
    private bool isWallSliding;
    private bool isTouchingWall;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isDashing;
    private bool knockback;
    [SerializeField]
    private Vector2 knockbackSpeed;
    
    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    
    // -------------------------------------- Start is called before the first frame update -----------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize(); // means vector itself equals 1
        wallJumpDirection.Normalize();
    }

    // --------------------------------------- Update is called once per frame ------------------------------------------------------
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckLedgeClimb();
        CheckJump();
        CheckDash();
        CheckKnockback();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

    }
    public bool GetDashStatus()
    {
        return isDashing;
    }
    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }
    public void CheckKnockback()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }
    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;
            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);                       
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);                       
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            canMove = false;
            canFlip = false;
            anim.SetBool("canClimbLedge", canClimbLedge);
        }
         if (canClimbLedge)
         {
             transform.position = ledgePos1;
         }

    }
    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround); // ledge check as origin , transform.right as a direction, 
                                                                                                                   // wallcheckdistance as a distance  whatIsGround as layer mask
    if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }
    private void CheckIfCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        if (isTouchingWall)
        {
            canWallJump = true;
        }
        if(amountOfJumpsLeft <=0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0) // isFacingRight and movementInputDirection if less then 0 it flips the character
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0) // else if NOT isFacingRight and movementInputDirection if less then 0 it flips the character
        {
            Flip();
        }
        if(Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else 
        {
            isWalking = false;
        }
    }
    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))

        {
            if (isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else 
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }
        if (Input.GetButtonDown("Horizontal")&& isTouchingWall)
        {
            if(!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = true;

                turnTimer = turnTimerSet;
            }

        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;
            if(turnTimer <= 0 )
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
        if (Input.GetButtonDown("Dash"))
        {
            if(Time.time >=(lastDash + dashCoolDown))
            AttempToDash();
        }
   
    }
    private void AttempToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    public int GetFacingDirection()
    {
        return facingDirection;
    }
   private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {

            canMove = false;
            canFlip = true;
            rb.velocity = new Vector2(dashSpeed * facingDirection, 0);
            dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
            if(dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;

            }
        } 
    }

    private void CheckJump()
    {
       if(jumpTimer > 0)
        {
           // ---------------------------------------------------------------------------------------------------------WallJump---------------------------------------------------
            if(!isGrounded && isTouchingWall && movementInputDirection !=0 && movementInputDirection != -facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
            if(isAttemptingToJump)
            { 
                jumpTimer -= Time.deltaTime;
            }
            if(wallJumpTimer > 0)
            {
                if(hasWallJumped && movementInputDirection == -lastWallJumpDirection)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                    hasWallJumped = false;
                }else if (wallJumpTimer <= 0)
                {
                    hasWallJumped = false;
                }
                else
                {
                    wallJumpTimer -= Time.deltaTime;

                }
            }
        }   

    }
    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }

    }
    private void WallJump()
    {
        if (canWallJump) // wall Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }
    private void ApplyMovement()
    {
         if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !knockback)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
         else if(canMove && !knockback)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
       
       
        
        if (isWallSliding)
        {
            if(rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }
    public void DisableFlip()
    {
        canFlip = false;
    }
    public void EnableFlip()
    {
        canFlip = true;
    }
    private void Flip()
    {
        if(!isWallSliding && canFlip && !knockback)
        {
            facingDirection *= -1; // changing 1 to 1 everytime we flip the character
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
       
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y + wallCheck.position.z));
    }
}
