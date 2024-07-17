using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string npcName;
    TextMeshPro textMeshPro;
    private bool waitingForChoice = false;
    private DialogueLine line;
    public GameObject buttonPrefab;
    public Transform contentContainer;
    public bool inDialogue = false;
    private List<GameObject> btnsToDestroy = new();
    public float typingSpeed = 0.5f;
    private Coroutine typingCoroutine;
    private bool typing = false;
    void Start()
    {
        DialogueManager.Instance.onDialogueEnd.AddListener(OnDialogueEnd);
        DialogueManager.Instance.onNewDialogueLine.AddListener(OnNewDialogueLine);
        DialogueManager.Instance.LoadDialogueFile(npcName, "start");
        textMeshPro = GetComponent<TextMeshPro>();
        DialogueManager.Instance.GetNextLine(1);
        inDialogue = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (typing)
            {
                SkipTyping();
            }
            else if (inDialogue && !waitingForChoice)
            {
                DialogueManager.Instance.GetNextLine(line.nextId);
            }
        }
    }

    private void OnDialogueEnd()
    {
        textMeshPro.text = "";
        inDialogue = false;
    }

    private void OnNewDialogueLine(DialogueLine line)
    {
        this.line = line;
        textMeshPro.text = "";
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(line.text));
        CreateChoiceButtons();
    }

    private IEnumerator TypeLine(string text)
    {
        typing = true;
        foreach (char c in text)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        typing = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        textMeshPro.text = line.text;
        typing = false;
    }

    private void CreateChoiceButtons()
    {
        if (line.choices.Count > 0) waitingForChoice = true;
        foreach (DialogueChoice choice in line.choices)
        {
            GameObject choiceButton = Instantiate(buttonPrefab, contentContainer);
            choiceButton.GetComponentInChildren<TMP_Text>().text = choice.text;
            btnsToDestroy.Add(choiceButton);

            choiceButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (GameObject btn in btnsToDestroy)
                {
                    Destroy(btn);
                }
                btnsToDestroy.Clear();
                waitingForChoice = false;
                DialogueManager.Instance.GetNextLine(choice.nextId);
            });
        }
    }
}
