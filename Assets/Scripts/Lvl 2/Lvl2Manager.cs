using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl2Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] dialogues;
    [SerializeField] private bool[] isCombatDialogue;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject potatoeChief, encounter;
    [SerializeField] public List<GameObject> potatoes;
    [SerializeField] private GroundCollisionDialogue ground;

    private ShapeRecognizer shapeRecognizer;
    private bool[] isDiaEnd;
    private PlayerMovement playerMovement;

    bool isLvl2Done = false;

    void Start()
    {
        shapeRecognizer = GetComponent<ShapeRecognizer>();
        isDiaEnd = new bool[dialogues.Length];
        playerMovement = player.GetComponent<PlayerMovement>();

        shapeRecognizer.UnlockShape("rectangle");
        shapeRecognizer.UnlockShape("triangle");
        shapeRecognizer.UnlockShape("arrow");
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerMovement();

        if (encounter.activeSelf && !isDiaEnd[0])
        {
            isDiaEnd[0] = true;
            potatoeChief.GetComponent<Animator>().SetBool("turn", true);
            dialogues[0].SetActive(true);
            isDiaEnd[1] = true;
        }

        if (!dialogues[0].activeSelf && isDiaEnd[1])
        {
            isDiaEnd[1] = false;
            StartCoroutine(DelayDiedChief());
        }
        if (!dialogues[1].activeSelf && isDiaEnd[2])
        {
            isDiaEnd[2] = false;

            Flip(potatoes[0]);
            Flip(potatoes[2]);
            Flip(potatoes[3]);
            foreach (var potato in potatoes)
            {
                potato.GetComponent<PotatoeAttack>().enabled = true;
            }

            StartCoroutine(DelayDiaRun());
            dialogues[2].SetActive(true);
            isDiaEnd[3] = true;
        }

        if (isDiaEnd[3])
        {
            if ((shapeRecognizer.drawnShapeName == "arrow right" || shapeRecognizer.drawnShapeName == "arrow left") && !isDiaEnd[4]
                && !dialogues[3].activeSelf && !dialogues[2].activeSelf && !dialogues[4].activeSelf)
            {
                isDiaEnd[4] = true;
                StartCoroutine(DelayDiaTriangle());
            }
            if (!dialogues[2].activeSelf && !dialogues[4].activeSelf && !dialogues[5].activeSelf && ground.isHitByShape && !isDiaEnd[5])
            {
                isDiaEnd[5] = true;
                dialogues[3].SetActive(true);
            }
            if (!dialogues[2].activeSelf && !dialogues[3].activeSelf && !dialogues[5].activeSelf && ground.isHitByShape && isDiaEnd[6])
            {
                isDiaEnd[6] = false;
                dialogues[3].SetActive(true);
            }
        }

        if (potatoes.Count == 0 && !isDiaEnd[7])
        {
            isDiaEnd[7] = true;
            dialogues[3].SetActive(false);
            dialogues[4].SetActive(false);
            dialogues[5].SetActive(false);
            dialogues[6].SetActive(true);

            isLvl2Done = true;
        }

        if (isLvl2Done && !dialogues[6].activeSelf)
        {
            gameObject.GetComponent<Lvl3Manager>().enabled = true;
            gameObject.GetComponent<Lvl2Manager>().enabled = false;
        }
    }

    IEnumerator DelayDiaTriangle()
    {
        yield return new WaitForSeconds(1f);
        dialogues[5].SetActive(true);
    }

    IEnumerator DelayDiaRun()
    {
        yield return new WaitForSeconds(7f);
        isDiaEnd[6] = true;
    }

    void Flip(GameObject obj)
    {
        Vector3 scale = obj.transform.localScale;
        scale.x *= -1;
        obj.transform.localScale = scale;
    }

    IEnumerator DelayDiedChief()
    {
        potatoeChief.GetComponent<Animator>().SetBool("dead", true);
        Flip(potatoes[0]);
        Flip(potatoes[2]);
        Flip(potatoes[3]);

        foreach (var potato in potatoes)
        {
            potato.GetComponent<Animator>().enabled = true;
        }

        yield return new WaitForSeconds(4f);
        dialogues[1].SetActive(true);
        isDiaEnd[2] = true;
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
