using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button continueButton;

    public GameObject confirmationPopup; // Popup for New Game confirmation
    public GameObject optionsPanel, menuPanel;
    public string saveFileName = "checkpoint.json";
    private string saveFilePath;

    Transform player;

    private void Start()
    {
        saveFilePath = Path.Combine(Application.streamingAssetsPath, saveFileName);

        continueButton.interactable = File.Exists(saveFilePath);
        if (!continueButton.interactable)
        {
            continueButton.gameObject.transform.parent.GetComponent<Image>().color = new Color(255,255,255,50);
        }
        else
        {
            continueButton.gameObject.transform.parent.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
    }

    public void OnNewGameClicked()
    {
        if (File.Exists(saveFilePath))
        {
            confirmationPopup.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }

    public void ConfirmNewGame(bool confirm)
    {
        confirmationPopup.SetActive(false);
        if (confirm)
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
            StartNewGame();
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnContinueClicked()
    {
        LoadCheckpoint();
    }

    public void OnOptionsClicked()
    {
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }
    public void OnOptionsBack()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void LoadCheckpoint()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No save file found.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data.sceneName != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(data.sceneName);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                player = FindFirstObjectByType<PlayerMovement>().transform;
                MovePlayerToCheckpoint(data);
            };
        }
    }

    private void MovePlayerToCheckpoint(SaveData data)
    {
        if (player == null)
        {
            FindOrAssignPlayer();
        }

        if (player != null)
        {
            player.position = data.position;
            Debug.Log("Checkpoint loaded.");
        }
        else
        {
            Debug.LogError("Player object not found after scene load.");
        }
    }
    private void FindOrAssignPlayer()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("Player object could not be found in the scene.");
            }
        }
    }
}

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public Vector3 position;
}
