using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    [SerializeField] GameObject[] dialogs;
    [SerializeField] Sprite[] bgs;
    [SerializeField] Image background;
    [SerializeField] GameObject suckInBg;

    [SerializeField] GameObject mainCam;
    [SerializeField] Animator transiAnim;

    bool isDia1End = false;
    bool isDia2End = false;
    bool isDia3End = false;
    bool isDia4End = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (isDia1End && !dialogs[0].activeSelf)
        {
            isDia1End = false;

            StartCoroutine(Shake(2f, 7f));
            StartCoroutine(DelayDia2());
        }

        if (isDia2End && !dialogs[1].activeSelf)
        {
            isDia2End = false;
            background.sprite = bgs[0];
            StartCoroutine(DelayDia3());
        }
        if (isDia3End && !dialogs[2].activeSelf)
        {
            isDia3End = false;
            background.sprite = bgs[1];
            StartCoroutine(DelayDia4());
        }
        if (isDia4End && !dialogs[3].activeSelf)
        {
            isDia4End = false;
            suckInBg.SetActive(true);
        }
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(2f);
        dialogs[0].SetActive(true);

        yield return new WaitForSeconds(1f);
        isDia1End = true;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = mainCam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCam.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        mainCam.transform.localPosition = originalPosition;
    }

    IEnumerator DelayDia2()
    {
        yield return new WaitForSeconds(3f);
        dialogs[1].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        isDia2End = true;
    }

    IEnumerator DelayDia3()
    {
        yield return new WaitForSeconds(2f);
        dialogs[2].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        isDia3End = true;
    }
    IEnumerator DelayDia4()
    {
        yield return new WaitForSeconds(1f);
        dialogs[3].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        isDia4End = true;
    }

    IEnumerator DelayEnd()
    {
        yield return new WaitForSeconds(1f);
        transiAnim.SetBool("fade in", true);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
    }
}
