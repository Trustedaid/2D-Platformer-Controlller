using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;
    private Rigidbody2D rb; // defines Rigidbody as rb
    private Animator anim;
    private int amountOfJumpsLeft;

    public float movementspeed = 10f; // publicly shows that character movement speed in panel
    private float jumpForce = 10f;
    public float wallCheckDistance;
    public float groundCheckRadius;
    public float wallSlideSpeed;
   
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
        if(isGrounded && rb.velocity.y <= 0)
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
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))

        {
            Jump();
        }

    }
    private void Jump()
    {
        if (canJump)
        {
             rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--; 
    
        }
       }
    private void ApplyMovement()
    {
        rb.velocity = new Vector2(movementspeed * movementInputDirection, rb.velocity.y);
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
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y + wallCheck.position.z));
    }
}
