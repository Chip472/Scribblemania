using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenu : MonoBehaviour
{
    public GameObject menuScene, optionsMenu;
    public GameManager check;

    public void BackToGame()
    {
        Time.timeScale = 1f;
        check.isEscMenuShowed = false;
        gameObject.SetActive(false);
    }

    public void OptionsOpen()
    {
        menuScene.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OptionsClose()
    {
        menuScene.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
