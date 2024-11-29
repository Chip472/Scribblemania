using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffFallCollision : MonoBehaviour
{
    public GameManager gameManager;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameManager.isPlayerDead = true;
        }
    }
}
