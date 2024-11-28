using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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

    public GameManager gameManager;

    bool isCharging = false;

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
            if (!isCharging)
            {
                animator.SetFloat("MoveType", 0f);
            }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Funny Tree")
        {
            gameManager.isPlayerDead = true;
        }
    }

    #region Saving stuff
    private bool isMovementDisabled = false;

    public void DisableMovement()
    {
        isMovementDisabled = true;
        rb.velocity = Vector2.zero;
        animator.SetFloat("MoveType", 0f);
    }

    public void EnableMovement()
    {
        animator.SetBool("Charge", false);
        isMovementDisabled = false;
        isCharging = false;
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        StartCoroutine(MoveTowardsAndSave(targetPosition));
    }

    private IEnumerator MoveTowardsAndSave(Vector3 targetPosition)
    {
        isCharging = true;
        animator.SetFloat("MoveType", 1f);

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            float step = speed * Time.deltaTime;
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

            if (direction.x > 0 && !isFacingRight) Flip();
            if (direction.x < 0 && isFacingRight) Flip();

            yield return null;
        }

        rb.velocity = Vector2.zero;
        PlaySaveAnimation();
    }

    public void PlaySaveAnimation()
    {
        animator.SetBool("Charge", true);
    }

    #endregion
}
