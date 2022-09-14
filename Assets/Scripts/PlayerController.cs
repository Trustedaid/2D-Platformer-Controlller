using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;
    private Rigidbody2D rb; // defines Rigidbody as rb
    private Animator anim;
    private int amountOfJumpsLeft;
    private int facingDirection = 1;

    public float movementSpeed = 10f; // publicly shows that character movement speed in panel
    private float jumpForce = 11f;
    public float wallCheckDistance;
    public float groundCheckRadius;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 1f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;
    
    public LayerMask whatIsGround;
    
    public int amountOfJumps = 1;
    private bool isWallSliding;
    private bool isTouchingWall;
    private bool canJump;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isWalking;
    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize(); // means vector itself equals 1
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }
    private void CheckIfWallSliding()
    {
        if(isTouchingWall && !isGrounded && rb.velocity.y <0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }
    private void CheckIfCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0 || isWallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        if(amountOfJumpsLeft <=0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
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
        if(rb.velocity.x != 0)
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
            Jump();
        }
        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

    }
    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
             rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--; 
    
        }
        else if(isWallSliding && movementInputDirection == 0 && canJump) // Wall Hop
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if (isWallSliding || isTouchingWall && movementInputDirection != 0 && canJump) // wall Jump
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }

       }
    private void ApplyMovement()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(forceToAdd);

            if(Mathf.Abs(rb.velocity.x ) > movementSpeed)
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
        }
        else if(!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x* airDragMultiplier, rb.velocity.y);
        }
        
        if (isWallSliding)
        {
            if(rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }
    private void Flip()
    {
        if(!isWallSliding)
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
