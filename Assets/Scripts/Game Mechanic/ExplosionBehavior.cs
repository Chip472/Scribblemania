using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    public ParticleSystem explosionEffect;
    public float explosionDuration = 2f;

    public float maxFlyTime = 30f;

    public AudioSource explosionSoundSource;

    private void Start()
    {
        Destroy(gameObject, maxFlyTime);
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
            Debug.Log($"Collision point: {collisionPoint}");

            ParticleSystem explosion = Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            AudioSource audioSource = Instantiate(explosionSoundSource, collisionPoint, Quaternion.identity);

            explosion.Play();

            Destroy(explosion.gameObject, explosionDuration);
            Destroy(audioSource.gameObject, explosionDuration);
        }

        Destroy(gameObject);
    }

}
