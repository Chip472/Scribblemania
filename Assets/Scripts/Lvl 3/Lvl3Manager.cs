using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lvl3Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] dialogues;
    [SerializeField] private bool[] isCombatDialogue;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject battleTrigger;
    [SerializeField] public List<GameObject> alihumongus;

    public Animator tabletAnim;
    public RuntimeAnimatorController normalTablet, ultiTablet;

    private ShapeRecognizer shapeRecognizer;
    private bool[] isDiaEnd;
    private PlayerMovement playerMovement;

    bool isUltimateDone = false;
    public GameObject worm;
    public GameObject wormTrigger;

    public AudioSource battleSFX;
    public Image battery, fill;

    void Start()
    {
        shapeRecognizer = GetComponent<ShapeRecognizer>();
        isDiaEnd = new bool[dialogues.Length + 5];
        playerMovement = player.GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        HandlePlayerMovement();

        if (battleTrigger.activeSelf && !isDiaEnd[0])
        {
            isDiaEnd[0] = true;
            foreach (var ali in alihumongus)
            {
                ali.GetComponent<AlihumongusControl>().enabled = true;
            }
        }

        if (battleTrigger.activeSelf)
        {
            if (!dialogues[3].activeSelf && !dialogues[4].activeSelf && !dialogues[1].activeSelf && !isDiaEnd[7])
            {
                isDiaEnd[7] = true;
                dialogues[2].SetActive(true);
            }

            if (playerMovement.currentHealth <= 60 && !dialogues[2].activeSelf && !dialogues[4].activeSelf && !dialogues[1].activeSelf && !isDiaEnd[5])
            {
                isDiaEnd[5] = true;
                dialogues[3].SetActive(true);
            }

            if ((shapeRecognizer.drawnShapeName == "triangle right" || shapeRecognizer.drawnShapeName == "triangle left" ||
                shapeRecognizer.drawnShapeName == "triangle up" || shapeRecognizer.drawnShapeName == "triangle down") &&
                !dialogues[2].activeSelf && !dialogues[3].activeSelf && !dialogues[1].activeSelf && !isDiaEnd[6])
            {
                isDiaEnd[6] = true;
                dialogues[4].SetActive(true);
            }

            if (alihumongus.Count <= 6 && !isDiaEnd[1])
            {
                tabletAnim.runtimeAnimatorController = ultiTablet;
                isDiaEnd[1] = true;
                StartCoroutine(DelayUltimate());
            }

            if (!dialogues[1].activeSelf && isDiaEnd[2])
            {
                isDiaEnd[2] = false;
                foreach (var ali in alihumongus)
                {
                    ali.GetComponent<AlihumongusControl>().enabled = true;
                }
            }

            if (alihumongus.Count == 0 && !isDiaEnd[3] && isUltimateDone)
            {
                isDiaEnd[3] = true;
                dialogues[5].SetActive(true);
                battleSFX.Stop();
            }

            if (shapeRecognizer.isUsingUltimate && !isDiaEnd[4])
            {
                isDiaEnd[4] = true;
                StartCoroutine(DelayEndUltimate());
            }

            if (dialogues[6].activeSelf && !isDiaEnd[8])
            {
                isDiaEnd[8] = true;
            }
            if (!dialogues[6].activeSelf && isDiaEnd[8] && !isDiaEnd[9])
            {
                isDiaEnd[9] = true;
                worm.SetActive(true);
                FindAnyObjectByType<CameraFollowWithBoundaries>().StartShake(0.3f, 0.2f);
                StartCoroutine(DelayWormDrive());
            }

            if (!dialogues[7].activeSelf && isDiaEnd[9] && !isDiaEnd[10] && shapeRecognizer.drawnShapeName == "heart" && shapeRecognizer.drawnShapeScore >= 0.9f)
            {
                isDiaEnd[10] = true;
                worm.GetComponent<Animator>().SetBool("accept", true);
                worm.GetComponent<Collider2D>().enabled = true;
            }

            if (wormTrigger.activeSelf && !isDiaEnd[11])
            {
                isDiaEnd[11] = true;
                player.SetActive(false);
                worm.GetComponent<Animator>().SetBool("drive", true);
                StartCoroutine(DelayEndScene2());
            }
        }
    }

    IEnumerator DelayEndScene2()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(4);
    }

    IEnumerator DelayWormDrive()
    {
        yield return new WaitForSeconds(1.5f);
        dialogues[7].SetActive(true);
        shapeRecognizer.UnlockShape("heart");
    }

    IEnumerator DelayUltimate()
    {
        yield return new WaitForSeconds(1f);

        foreach (var ali in alihumongus)
        {
            ali.GetComponent<AlihumongusControl>().enabled = false;
        }

        shapeRecognizer.UnlockShape("star");
        dialogues[1].SetActive(true);
        isDiaEnd[2] = true;
    }

    IEnumerator DelayEndUltimate()
    {
        yield return new WaitForSeconds(5f);
        shapeRecognizer.isUsingUltimate = false;
        tabletAnim.gameObject.GetComponent<RawImage>().color = Color.white;
        battery.color = Color.white;
        fill.color = Color.white;
        tabletAnim.runtimeAnimatorController = normalTablet;
        isUltimateDone = true;
        shapeRecognizer.LockShape("star");
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
