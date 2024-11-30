using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChargeTabletToSaveGame : MonoBehaviour
{
    public float saveDelay = 2f;
    public GameObject battery;

    private Vector3 savePosition;
    private GameObject playerObj;
    private PlayerMovement player;
    private bool isSaving = false;

    public bool check;
    public AudioSource chargeSFX;

    public float maxBattery = 100f;
    public float currentBattery = 100f;

    public Slider batterySlider;

    void Start()
    {
        currentBattery = PlayerPrefs.GetFloat("Battery", 100);
        check = false;
        UpdateBatteryUI();
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

    private IEnumerator DelayJump()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator SaveGameRoutine()
    {
        isSaving = true;
        player.DisableMovement();
        player.MoveToPosition(savePosition);
        battery.SetActive(true);
        chargeSFX.Play();

        yield return new WaitForSeconds(saveDelay);

        battery.SetActive(false);
        GameObject.FindAnyObjectByType<GameManager>().SaveCheckpoint();
        RechargeBattery();
        player.EnableMovement();
        isSaving = false;
    }

    private void RechargeBattery()
    {
        PlayerPrefs.SetFloat("Battery", maxBattery);
        currentBattery = maxBattery;
        Debug.Log($"Battery recharged: {currentBattery}/{maxBattery}");
        UpdateBatteryUI();
    }

    private void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery / maxBattery;
        }
    }
}
