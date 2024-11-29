using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public string defaultSceneName = "StartScene";
    public Vector3 defaultStartPosition;
    private string saveFilePath;

    [SerializeField] private GameObject deadPanel;
    public bool isPlayerDead = false;
    bool isDeadPanelShowed = false;

    private void Start()
    {
        saveFilePath = Application.dataPath + "/StreamingAssets/checkpoint.json";
    }

    void Update()
    {
        if (isPlayerDead && !isDeadPanelShowed)
        {
            isDeadPanelShowed = true;
            StartCoroutine(DelayDie());
        }
    }

    IEnumerator DelayDie()
    {
        if (player != null)
        {
            player.gameObject.GetComponent<PlayerMovement>().DiedAnim();
        }

        yield return new WaitForSeconds(1.5f);
        deadPanel.SetActive(true);
    }

    [System.Serializable]
    private class SaveData
    {
        public string sceneName;
        public Vector3 position;
    }

    public void SaveCheckpoint()
    {
        if (player == null) return;

        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            position = player.position,
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Checkpoint saved.");
    }

    public void LoadCheckpoint()
    {
        deadPanel.SetActive(false);

        if (player != null)
        {
            player.gameObject.GetComponent<Animator>().SetBool("Died", false);
        }

        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found. Loading default checkpoint.");
            LoadDefaultCheckpoint();
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data.sceneName != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(data.sceneName);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                FindOrAssignPlayer();
                MovePlayerToCheckpoint(data);
            };
        }
        else
        {
            MovePlayerToCheckpoint(data);
        }
    }

    private void LoadDefaultCheckpoint()
    {
        if (SceneManager.GetActiveScene().name != defaultSceneName)
        {
            SceneManager.LoadScene(defaultSceneName);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                FindOrAssignPlayer();
                MovePlayerToDefaultPosition();
            };
        }
        else
        {
            MovePlayerToDefaultPosition();
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

    private void MovePlayerToDefaultPosition()
    {
        if (player == null)
        {
            FindOrAssignPlayer();
        }

        if (player != null)
        {
            player.position = defaultStartPosition;
            Debug.Log("Default checkpoint loaded.");
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

    public void OnPlayerDeath()
    {
        Debug.Log("Player died. Loading checkpoint...");
        LoadCheckpoint();
    }
}
