using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerScript : MonoBehaviour
{
    public GameObject buddyTextbox, alienTextbox;
    public Animator buddy, alien;

    bool check;

    private void Start()
    {
        StartCoroutine(DelayStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (!buddyTextbox.activeSelf && check)
        {
            check = false;
            buddy.SetBool("fade", true);
            StartCoroutine(DelayAlien());
        }
        if (alienTextbox.GetComponent<DialogueSystem>().currentLineIndex == 1)
        {
            alien.SetBool("close up", true);
        }
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(5f);
        buddyTextbox.SetActive(true);
        check = true;
    }

    IEnumerator DelayAlien()
    {
        yield return new WaitForSeconds(3f);
        alien.gameObject.SetActive(true);
        alienTextbox.SetActive(true);
    }
}
