using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBehavior : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform player;
    public float cirSpd = 5f;
    public float stoppingDistance = 0.5f;

    private Transform targetEnemy;

    private void FixedUpdate()
    {
        GameObject enemy = FindClosestEnemy();

        if (enemy != null)
        {
            targetEnemy = enemy.transform;

            Vector2 direction = ((Vector2)targetEnemy.position - rb.position).normalized;

            rb.velocity = direction * cirSpd;

            if (Vector2.Distance(rb.position, targetEnemy.position) <= stoppingDistance)
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            float directionX = (transform.position.x > player.position.x) ? 1 : -1;
            rb.velocity = new Vector2(directionX * cirSpd, 0);
        }

        Debug.Log(enemy.name);
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                closest = enemy;
                minDistance = distance;
            }
        }

        return closest;
    }
}
