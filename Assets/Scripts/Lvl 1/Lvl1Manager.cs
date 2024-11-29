using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl1Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] dialogues;
    [SerializeField] private bool[] isCombatDialogue;
    [SerializeField] private GameObject player, alien;
    [SerializeField] private TriggeredWhenInteract triggered;

    [SerializeField] private GameObject newBoulder;

    private ShapeRecognizer shapeRecognizer;
    private bool[] isDiaEnd;
    private bool boulderTriggerActive = false;

    private PlayerMovement playerMovement;

    bool checkCliffTrigger = false;

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

        if (!newBoulder.activeSelf)
        {
            if (dialogues[2].activeSelf && !isDiaEnd[2])
            {
                isDiaEnd[2] = true;
            }
            if (!dialogues[2].activeSelf && isDiaEnd[2] && shapeRecognizer.drawnShapeName == "rectangle" && shapeRecognizer.drawnShapeScore >= 0.9f)
            {
                isDiaEnd[2] = false;
                dialogues[3].SetActive(true);
            }
        }
        else
        {
            dialogues[3].SetActive(false);
            triggered.gameObject.SetActive(false);

            if (!isDiaEnd[3])
            {
                isDiaEnd[3] = true;
                dialogues[4].SetActive(true);
            }
        }

        if (dialogues[5].activeSelf)
        {
            shapeRecognizer.UnlockShape("arrow");
            alien.SetActive(false);
        }

        if (!GameObject.Find("Tutor 2 trigger") && shapeRecognizer.drawnShapeName == "arrow down" && shapeRecognizer.drawnShapeScore >= 0.9f && !isDiaEnd[4])
        {
            shapeRecognizer.UnlockShape("triangle");
            isDiaEnd[4] = true;
            dialogues[6].SetActive(true);
        }
        if (!dialogues[6].activeSelf && isDiaEnd[4] && shapeRecognizer.drawnShapeName == "triangle left" && shapeRecognizer.drawnShapeScore >= 0.9f)
        {
            isDiaEnd[4] = false;
            dialogues[7].SetActive(true);
            GameObject.Find("Triggers").transform.GetChild(1).gameObject.SetActive(true);
            checkCliffTrigger = true;
        }
         
        if (!dialogues[8].activeSelf && GameObject.Find("Triggers").transform.GetChild(1).gameObject.name != "Cliff trigger" && !isDiaEnd[5] && checkCliffTrigger)
        {
            shapeRecognizer.drawnShapeName = null;
            shapeRecognizer.drawnShapeScore = 0;
            GameObject.Find("Tree can be shoot").GetComponent<TreeAtCliff>().isCompleteTutor = true;

            isDiaEnd[5] = true;
            isDiaEnd[6] = true;
        }

        if (isDiaEnd[6] && FindFirstObjectByType<TreeAtCliff>().isHit)
        {
            isDiaEnd[6] = false;

            if (shapeRecognizer.drawnShapeName == "arrow right" && shapeRecognizer.drawnShapeScore >= 0.9f)
            {
                StartCoroutine(DelayCliffScene(2f, 9));
            }
            else if (GameObject.Find("Worm").activeSelf && !FindFirstObjectByType<GameManager>().isPlayerDead)
            {
                StartCoroutine(DelayCliffScene(9f, 10));
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
                playerMovement.gameObject.GetComponent<Animator>().SetFloat("MoveType", 0f);
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
