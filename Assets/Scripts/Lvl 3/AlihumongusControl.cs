using System.Collections;
using UnityEngine;

public class AlihumongusControl : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 5f; // Faster than potatoes
    public int maxHP = 50; // Smaller HP since they are fragile
    private int currentHP;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f; // Attacks faster
    public bool isAttacking = false;

    [Header("Components")]
    public Transform player;

    [Header("Damage Settings")]
    public int arrowDamage = 10;
    public int triangleDamage = 10;
    public int rectangleDamage = 0; // No direct damage but slows down
    public int starDamage = 100; // Massive damage from star

    private Animator animator;
    private Collider2D alihumongusCollider;
    private bool isDead = false;
    private float originalSpeed;

    public AudioSource alihumongusDieSFX;
    public int AliAttackDam;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        alihumongusCollider = GetComponent<Collider2D>();
        originalSpeed = moveSpeed;
        animator.SetBool("battle", true);
    }

    void Update()
    {
        if (isDead) return;

        if (isAttacking)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            RunToPlayer();
        }
        else
        {
            StopAndAttack();
        }
    }

    void RunToPlayer()
    {
        animator.SetBool("attack", false);

        Vector2 direction = player.position - transform.position;

        if (direction.x < 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (direction.x > 0 && transform.localScale.x > 0)
        {
            Flip();
        }

        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void StopAndAttack()
    {
        animator.SetBool("attack", true);
        if (!isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Drawn shape"))
        {
            SavedShapeNameAndScore shape = collision.gameObject.GetComponent<SavedShapeNameAndScore>();
            if (shape != null && shape.shapeScore >= 0.9f)
            {
                int damage = 0;

                switch (shape.shapeName)
                {
                    case "arrow left":
                    case "arrow right":
                    case "arrow up":
                    case "arrow down":
                        damage = arrowDamage;
                        break;

                    case "triangle left":
                    case "triangle right":
                    case "triangle up":
                    case "triangle down":
                        damage = triangleDamage;
                        break;

                    case "rectangle":
                        StartCoroutine(SlowDown());
                        break;

                    case "star":
                        damage = starDamage; // Massive damage from star
                        break;
                }

                if (damage > 0)
                {
                    currentHP -= damage;
                    if (currentHP <= 0)
                    {
                        Die();
                    }
                }
            }
        }
    }

    private IEnumerator SlowDown()
    {
        moveSpeed = moveSpeed / 2; // Slow down to half
        yield return new WaitForSeconds(2f); // Slows down for 2 seconds
        moveSpeed = originalSpeed; // Restore speed
    }

    private void Die()
    {
        gameObject.tag = null;

        isDead = true;
        Debug.Log("Alihumongus died");
        alihumongusCollider.enabled = false;

        StartCoroutine(DelayDie());
    }

    IEnumerator DelayDie()
    {
        yield return new WaitForSeconds(1f); // Alihumongus dies quicker
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        alihumongusDieSFX.Play();
        Lvl3Manager manager = FindAnyObjectByType<Lvl3Manager>();
        if (manager != null)
        {
            manager.alihumongus.Remove(gameObject); // Replace with the correct list if needed
        }
    }
}
