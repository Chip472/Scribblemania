using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeadPanel : MonoBehaviour
{
    public string[] alienDia;
    public TMP_Text alienTxt;

    private int chosenDia;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
    }

    void OnEnable()
    {
        chosenDia = Random.Range(0, alienDia.Length);
        alienTxt.text = alienDia[chosenDia];
    }

    

    public void LoadCheckPoint()
    {
        gameManager.LoadCheckpoint();
    }
}
