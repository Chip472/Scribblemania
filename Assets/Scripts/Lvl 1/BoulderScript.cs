using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Drawn shape"))
        {
            Destroy(gameObject);
        }
    }
}
