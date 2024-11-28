using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggeredNextScene : MonoBehaviour
{
    public Animator transiAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(DelayChangeScene());
        }
    }

    IEnumerator DelayChangeScene()
    {
        transiAnim.SetBool("Out", true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(3);
    }
}
