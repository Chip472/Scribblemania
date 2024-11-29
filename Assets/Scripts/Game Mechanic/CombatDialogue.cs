using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatDialogue : MonoBehaviour
{
    [SerializeField] private GameObject textbox;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float textSpeed;
    [SerializeField] private float soundSpeed;
    [SerializeField] private AudioSource dialogueSFX;
    [SerializeField] private float timeBetweenLines = 1.5f; // Time between lines automatically

    private int currentLineIndex;
    private bool isTyping;
    private string currentText = string.Empty;

    [System.Serializable]
    public class DialogueNode
    {
        public string line;
        public int nextLineIndex = -1; // Use -1 for end of dialogue
    }

    [SerializeField] private DialogueNode[] dialogueNodes;

    void Start()
    {
        textComponent.text = string.Empty;
        currentLineIndex = 0;
        DisplayLine(currentLineIndex);
    }

    private void DisplayLine(int lineIndex)
    {
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
            if (textToType[charIndex] == '<') // Skip over rich text tags
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
        yield return new WaitForSeconds(timeBetweenLines);

        GoToNextLine();
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

    private void GoToNextLine()
    {
        DialogueNode currentNode = dialogueNodes[currentLineIndex];

        if (currentNode.nextLineIndex != -1)
        {
            currentLineIndex = currentNode.nextLineIndex; // Go to the specified next line
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
        Debug.Log("Combat dialogue has ended.");
    }
}
