using System.Collections;
using UnityEngine;

public class PotatoeAttack : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 3f;
    public int maxHP = 100;
    private int currentHP;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public bool isAttacking = false;

    [Header("Components")]
    public GameObject friesPrefab;
    public Transform player;
    public ShapeRecognizer shapeRecognizer;

    [Header("Damage Settings")]
    public int arrowDamage = 10;
    public int rectangleDamage = 15;
    public int triangleDamage = 20;

    private Animator animator;
    private Collider2D potatoCollider;
    private bool isDead = false;

    public int potatoAttackDam;
    public AudioSource potatoeDieSFX;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        potatoCollider = GetComponent<Collider2D>();
        animator.SetBool("battle", true);
    }

    void Update()
    {
        if (isDead) return;

        SeparatePotatoes();

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

    private void SeparatePotatoes()
    {
        Collider2D[] nearbyPotatoes = Physics2D.OverlapCircleAll(transform.position, 0.5f); // Adjust radius
        foreach (Collider2D collider in nearbyPotatoes)
        {
            if (collider.gameObject != gameObject && collider.gameObject.tag == "Enemy")
            {
                Vector2 direction = (transform.position - collider.transform.position).normalized;
                transform.position += (Vector3)(direction * Time.deltaTime * 0.1f); // Adjust separation speed
            }
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
                    case "rectangle":
                        damage = rectangleDamage;
                        break;
                    case "triangle left":
                    case "triangle right":
                    case "triangle up":
                    case "triangle down":
                        damage = triangleDamage;
                        break;
                }

                currentHP -= damage;

                if (currentHP <= 0)
                {
                    Die();
                }
            }
        }
    }


    private void Die()
    {
        gameObject.tag = null;

        isDead = true;
        animator.SetBool("dead", true);
        Debug.Log("Enemy died");
        potatoCollider.enabled = false;

        if (shapeRecognizer.drawnShapeName == "triangle left" ||
            shapeRecognizer.drawnShapeName == "triangle right" ||
            shapeRecognizer.drawnShapeName == "triangle up" ||
            shapeRecognizer.drawnShapeName == "triangle down")
        {
            TurnIntoFrenchFries();
        }

        StartCoroutine(DelayDie());
    }

    IEnumerator DelayDie()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void TurnIntoFrenchFries()
    {
        Instantiate(friesPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        potatoeDieSFX.Play();
        Lvl2Manager manager = FindAnyObjectByType<Lvl2Manager>();
        if (manager != null)
        {
            manager.potatoes.Remove(gameObject);
        }
    }
}
