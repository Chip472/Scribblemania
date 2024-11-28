using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeTabletToSaveGame : MonoBehaviour
{
    public float saveDelay = 2f;
    public GameObject battery;

    private Vector3 savePosition;
    private GameObject playerObj;
    private PlayerMovement player;
    private bool isSaving = false;

    public bool check;

    void Start()
    {
        check = false;
    }

    private void Update()
    {
        if (check)
        {
            check = false;
            playerObj = GameObject.FindGameObjectWithTag("Player");
            player = playerObj.GetComponent<PlayerMovement>();

            savePosition = new Vector3(transform.parent.transform.position.x, playerObj.transform.position.y, playerObj.transform.position.z);

            if (player.isGrounded)
            {
                StartCoroutine(SaveGameRoutine());
            }
            else
            {
                StartCoroutine(DelayJump());
                if (player.isGrounded)
                {
                    StartCoroutine(SaveGameRoutine());
                }
            }
        }
    }

    IEnumerator DelayJump()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator SaveGameRoutine()
    {
        isSaving = true;
        player.DisableMovement();
        player.MoveToPosition(savePosition);
        battery.SetActive(true);

        yield return new WaitForSeconds(saveDelay);

        battery.SetActive(false);
        GameObject.FindAnyObjectByType<GameManager>().SaveCheckpoint();
        player.EnableMovement();
        isSaving = false;
    }


}
