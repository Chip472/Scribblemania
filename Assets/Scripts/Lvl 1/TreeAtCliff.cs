using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAtCliff : MonoBehaviour
{
    public AudioSource ahSound;
    public GameObject worm;
    public Animator treeAnim;

    public bool isCompleteTutor = false;
    public bool isHit = false;

    private void Start()
    {
        treeAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Drawn shape") && isCompleteTutor)
        {
            isHit = true;

            if (collision.gameObject.GetComponent<SavedShapeNameAndScore>().shapeName == "arrow right")
            {
                treeAnim.SetBool("fall right", true);
                gameObject.layer = LayerMask.NameToLayer("Ground");
                //fall right
            }
            else if (collision.gameObject.GetComponent<SavedShapeNameAndScore>().shapeName == "triangle left" ||
                collision.gameObject.GetComponent<SavedShapeNameAndScore>().shapeName == "triangle right")
            {
                treeAnim.SetBool("explode", true);
                //explode
            }
            else if (collision.gameObject.GetComponent<SavedShapeNameAndScore>().shapeName == "arrow left")
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
        Explode();
    }

    public void FallShake()
    {
        GameObject.FindAnyObjectByType<CameraFollowWithBoundaries>().StartShake(0.5f, 0.3f);
    }
}
