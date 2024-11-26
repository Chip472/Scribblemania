using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredWhenInteract : MonoBehaviour
{
    [SerializeField] private GameObject interactText;
    [SerializeField] public GameObject triggeredObj;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            interactText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(DelayDisappear());
        }
    }

    IEnumerator DelayDisappear()
    {
        interactText.GetComponent<Animator>().SetBool("fade out", true);

        yield return new WaitForSeconds(0.6f);
        interactText.SetActive(false);
    }

    private void Update()
    {
        if (interactText.activeSelf && Input.GetKeyDown(KeyCode.E)) 
        { 
            triggeredObj.SetActive(true);
            interactText.SetActive(false);
        }
    }
}
