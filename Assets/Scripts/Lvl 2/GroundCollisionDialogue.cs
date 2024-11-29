using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollisionDialogue : MonoBehaviour
{
    public bool isHitByShape;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Drawn shape") && collision.gameObject.GetComponent<SavedShapeNameAndScore>().shapeName != "rectangle")
        {
            isHitByShape = true;
        }
    }
}
