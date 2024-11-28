using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAtCliff : MonoBehaviour
{
    public AudioSource ahSound;
    public GameObject worm;
    private Animator treeAnim;

    private void Start()
    {
        treeAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Drawn shape"))
        {
            if (GameObject.FindAnyObjectByType<ShapeRecognizer>().drawnShapeName == "arrow right")
            {
                treeAnim.SetBool("fall right", true);
                gameObject.layer = LayerMask.NameToLayer("Ground");
                //fall right
            }
            else if (GameObject.FindAnyObjectByType<ShapeRecognizer>().drawnShapeName == "triangle left" ||
                GameObject.FindAnyObjectByType<ShapeRecognizer>().drawnShapeName == "triangle right")
            {
                treeAnim.SetBool("explode", true);
                //explode
            }
            else if (GameObject.FindAnyObjectByType<ShapeRecognizer>().drawnShapeName == "arrow left")
            {
                treeAnim.SetBool("fall left", true);

                gameObject.tag = "Funny Tree";
                gameObject.layer = LayerMask.NameToLayer("Ground");

                StartCoroutine(DelayFallLeft());
                //fall left and kill player
            }
        }
    }

    public void Explode()
    {
        worm.SetActive(true);
        Destroy(gameObject);
    }

    IEnumerator DelayFallLeft()
    {
        yield return new WaitForSeconds(1.5f);
        FallShake();
        ahSound.Play();
        Destroy(gameObject);
    }

    public void FallShake()
    {
        GameObject.FindAnyObjectByType<CameraFollowWithBoundaries>().StartShake(0.5f, 0.3f);
    }
}
