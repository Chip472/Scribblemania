using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl1Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] dialogues;
    [SerializeField] private bool[] isCombatDialogue;
    [SerializeField] private GameObject player, alien;
    [SerializeField] private TriggeredWhenInteract triggered;

    private ShapeRecognizer shapeRecognizer;
    private bool[] isDiaEnd;
    private bool boulderTriggerActive = false;

    private PlayerMovement playerMovement;

    void Start()
    {
        shapeRecognizer = GetComponent<ShapeRecognizer>();
        triggered.triggeredObj = dialogues[0];
        isDiaEnd = new bool[dialogues.Length];
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandlePlayerMovement();

        if (dialogues[0].activeSelf && !isDiaEnd[0])
        {
            isDiaEnd[0] = true;
        }
        if (!dialogues[0].activeSelf && isDiaEnd[0])
        {
            isDiaEnd[0] = false;
            triggered.gameObject.SetActive(false);
        }

        if (GameObject.Find("Boulder trig") && !boulderTriggerActive)
        {
            boulderTriggerActive = true;
            triggered.gameObject.SetActive(true);
            triggered.triggeredObj = dialogues[1];
        }
        if (dialogues[1].activeSelf && !isDiaEnd[1])
        {
            isDiaEnd[1] = true;
        }
        if (!dialogues[1].activeSelf && isDiaEnd[1] && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            shapeRecognizer.UnlockShape("rectangle");
            triggered.gameObject.SetActive(false);
            isDiaEnd[1] = false;
            dialogues[2].SetActive(true);
        }

        if (dialogues[2].activeSelf && !isDiaEnd[2])
        {
            isDiaEnd[2] = true;
        }
        if (!dialogues[2].activeSelf && isDiaEnd[2] && shapeRecognizer.drawnShapeName == "rectangle")
        {
            isDiaEnd[2] = false;
            dialogues[3].SetActive(true);
        }

        if (dialogues[3].activeSelf && !isDiaEnd[3])
        {
            isDiaEnd[3] = true;
        }
        if (!dialogues[3].activeSelf && isDiaEnd[3] && !GameObject.Find("Boulder"))
        {
            isDiaEnd[3] = false;
            dialogues[4].SetActive(true);
        }

        if (dialogues[5].activeSelf)
        {
            shapeRecognizer.UnlockShape("arrow");
            alien.SetActive(false);
        }

        if (!GameObject.Find("Tutor 2 trigger") && shapeRecognizer.drawnShapeName == "arrow down" && !isDiaEnd[4])
        {
            shapeRecognizer.UnlockShape("triangle");
            isDiaEnd[4] = true;
            dialogues[6].SetActive(true);
        }
        if (!dialogues[6].activeSelf && isDiaEnd[4] && shapeRecognizer.drawnShapeName == "triangle left")
        {
            isDiaEnd[4] = false;
            dialogues[7].SetActive(true);
        }

        if (!dialogues[8].activeSelf && !GameObject.Find("Cliff trigger") && !isDiaEnd[5])
        {
            shapeRecognizer.drawnShapeName = null;
            isDiaEnd[5] = true;
            isDiaEnd[6] = true;
        }

        if (isDiaEnd[6]) // Them dieu kien isTreeGetHit
        {
            isDiaEnd[6] = false;

            if (shapeRecognizer.drawnShapeName == "arrow right")
            {
                StartCoroutine(DelayCliffScene(2f, 9));
            }
            else if (shapeRecognizer.drawnShapeName == "triangle right")
            {
                StartCoroutine(DelayCliffScene(10f, 10));
            }
        }

    }


    IEnumerator DelayCliffScene(float time, int index)
    {
        yield return new WaitForSeconds(time);
        dialogues[index].SetActive(true);
    }


    private void HandlePlayerMovement()
    {
        bool isAnyNormalDialogueActive = false;

        for (int i = 0; i < dialogues.Length; i++)
        {
            if (dialogues[i].activeSelf && !isCombatDialogue[i])
            {
                isAnyNormalDialogueActive = true;
                break;
            }
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = !isAnyNormalDialogueActive;
            if (isAnyNormalDialogueActive)
            {
                playerMovement.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                playerMovement.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                playerMovement.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                playerMovement.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }
}