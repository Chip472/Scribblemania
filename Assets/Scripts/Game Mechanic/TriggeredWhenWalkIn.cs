using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredWhenWalkIn : MonoBehaviour
{
    [SerializeField] GameObject triggeredObj;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            triggeredObj.SetActive(true);
            Destroy(gameObject);
        }
    }
}
