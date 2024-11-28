using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    public bool isGrounded;

    private Animator animator;
    private bool isFacingRight = true;

    private float velocityThreshold = 3.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!isGrounded)
        {
            animator.SetFloat("MoveType", 2f);

            if (rb.velocity.y > velocityThreshold)
            {
                animator.SetFloat("JumpDirection", 0f);
            }
            else if (rb.velocity.y < -velocityThreshold)
            {
                animator.SetFloat("JumpDirection", 1f);
            }
            else
            {
                animator.SetFloat("JumpDirection", 0.5f);
            }
        }
        else if (Mathf.Abs(moveInput) > 0.1f)
        {
            animator.SetFloat("MoveType", 1f);
        }
        else
        {
            animator.SetFloat("MoveType", 0f);
        }

        if (moveInput > 0 && !isFacingRight)
        {
            animator.SetFloat("Direction", 1);
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            animator.SetFloat("Direction", -1);
            Flip();
        }

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
