using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private GameObject textbox;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private float textSpeed;
    [SerializeField] private float soundSpeed;
    [SerializeField] private AudioSource dialogueSFX;

    private int currentLineIndex;
    private bool isTyping;
    private string currentText = string.Empty;

    [System.Serializable]
    public class DialogueNode
    {
        public string line;
        public Choice[] choices;
        public int nextLineIndex = -1;
    }


    [System.Serializable]
    public class Choice
    {
        public string choiceText;
        public int nextLineIndex; 
    }

    [SerializeField] private DialogueNode[] dialogueNodes;

    void Start()
    {
        textComponent.text = string.Empty;
        currentLineIndex = 0;
        DisplayLine(currentLineIndex);
    }

    void Update()
    {
        if (isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            StopAllCoroutines();
            textComponent.text = currentText;
            isTyping = false;

            DialogueNode currentNode = dialogueNodes[currentLineIndex];
            if (currentNode.choices.Length > 0)
            {
                CreateButtons(currentNode.choices);
            }
        }
        else if (!isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            DialogueNode currentNode = dialogueNodes[currentLineIndex];
            if (currentNode.choices.Length == 0)
            {
                GoToNextLine();
            }
        }
    }

    private void DisplayLine(int lineIndex)
    {
        ClearButtons();

        if (lineIndex >= dialogueNodes.Length)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = dialogueNodes[lineIndex];
        currentText = node.line;
        textComponent.text = string.Empty;

        StartCoroutine(TypeLine(node));
        StartCoroutine(DialogueSFX(node));
    }

    IEnumerator TypeLine(DialogueNode node)
    {
        isTyping = true;
        textComponent.text = string.Empty;

        string textToType = node.line;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            if (textToType[charIndex] == '<')
            {
                int tagEndIndex = textToType.IndexOf('>', charIndex);
                if (tagEndIndex != -1)
                {
                    textComponent.text += textToType.Substring(charIndex, tagEndIndex - charIndex + 1);
                    charIndex = tagEndIndex + 1;
                    continue;
                }
            }

            textComponent.text += textToType[charIndex];
            charIndex++;

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        if (node.choices.Length > 0)
        {
            CreateButtons(node.choices);
        }
    }


    IEnumerator DialogueSFX(DialogueNode node)
    {
        foreach (char c in node.line.ToCharArray())
        {
            if (isTyping)
            {
                dialogueSFX.Play();
                yield return new WaitForSeconds(soundSpeed);
            }
        }
    }

    private void CreateButtons(Choice[] choices)
    {
        foreach (Choice choice in choices)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice.nextLineIndex));
        }
    }

    private void OnChoiceSelected(int nextLineIndex)
    {
        ClearButtons();
        currentLineIndex = nextLineIndex;
        DisplayLine(currentLineIndex);
    }

    private void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void GoToNextLine()
    {
        ClearButtons(); 

        DialogueNode currentNode = dialogueNodes[currentLineIndex];

        if (currentNode.nextLineIndex != -1)
        {
            currentLineIndex = currentNode.nextLineIndex;
        }
        else
        {
            currentLineIndex++;
        }

        if (currentLineIndex < dialogueNodes.Length)
        {
            DisplayLine(currentLineIndex);
        }
        else
        {
            EndDialogue();
        }
    }


    private void EndDialogue()
    {
        textbox.SetActive(false);
        Debug.Log("Dialogue has ended.");
    }
}
