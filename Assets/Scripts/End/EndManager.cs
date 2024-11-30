using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public GameObject theEndText;
    public PlayableDirector playable;

    bool check = false;

    // Update is called once per frame
    void Update()
    {
        if (!check && theEndText.activeSelf)
        {
            check = true;
            StartCoroutine(DelayEnd());
        }
    }

    IEnumerator DelayEnd()
    {
        yield return new WaitForSeconds(2f);
        playable.Stop();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
