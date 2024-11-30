using UnityEngine;

public class FallingStarWithCollision : MonoBehaviour
{
    [Header("Stick Settings")]
    public ParticleSystem explosionEffect;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TriggerExplosion(collision);
    }

    private void TriggerExplosion(Collision2D collision)
    {
        if (explosionEffect != null)
        {
            Vector2 collisionPoint = collision.contacts[0].point;

            ParticleSystem explosion = Instantiate(explosionEffect, collisionPoint, Quaternion.identity);

            explosion.Play();

            Destroy(explosion.gameObject, 2f);
        }

        Destroy(gameObject);
    }
}
