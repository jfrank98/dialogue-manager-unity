using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogueManager>();

                if (_instance == null)
                {
                    GameObject singleton = new();
                    _instance = singleton.AddComponent<DialogueManager>();
                    singleton.name = typeof(DialogueManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    private string dialogueFilePath;
    private StreamReader currentLine;
    public UnityEvent onDialogueEnd = new();
    public UnityEvent<DialogueLine> onNewDialogueLine = new();
    private List<DialogueLine> dialogueLines = new();
    private string currentTitle;
    private Stack<DialogueLine> dialogueStack = new();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadDialogueFile(string dialogueFileName, string title)
    {

        dialogueFilePath = "Assets/Resources/Dialogues/" + dialogueFileName + ".dlg";
        currentLine = new StreamReader(dialogueFilePath);
        currentTitle = title;
        try{
            GetAllLines();
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError("File not found: " + dialogueFilePath);
            Debug.LogError(e.Message);
            ResetDialogueData();
        }
    }

    private void GetAllLines()
    {
        string line;
        GoTo("#" + currentTitle);
        bool isCommand = false;

        while ((line = currentLine.ReadLine()) != null && !line.Contains("~END~"))
        {
            int indentLevel = GetIndentLevel(line);
            string trimmedLine = line.Trim();

            while (dialogueStack.Count > indentLevel)
            {
                dialogueStack.Pop();
            }

            if (CheckIfChoice(trimmedLine)) {
                continue;
            }
            if (CheckIfEnd(trimmedLine)) {
                break;
            }
            isCommand = CodeInterpreter.CheckIfCode(trimmedLine, currentLine, CreateDialogueLine, GoTo);
            if (!isCommand)
            {
                if (trimmedLine.StartsWith("$"))
                {
                    continue;
                }
            }
            CreateDialogueLine(trimmedLine, isCommand);
        }
    }

    private void GoTo(string wantedLine)
    {
        string line;
        while ((line = currentLine.ReadLine()) != null)
        {
            if (line.StartsWith(wantedLine)) {
                return;
            }
        }
    }

    private void CreateDialogueLine(string line, bool isCommand = false)
    {
        DialogueLine dialogueLine = new(line, currentTitle, isCommand);
        if (dialogueStack.Count > 0)
        {
            DialogueLine parent = dialogueStack.Peek();
            parent.AddChild(dialogueLine);
        }

        dialogueLines.Add(dialogueLine);
        dialogueStack.Push(dialogueLine);
    }

    private bool CheckIfChoice(string line)
    {
        if (line.StartsWith("-"))
        {
            string choice = line.Substring(1).Trim();
            if (dialogueStack.Count > 0)
            {
                DialogueLine parent = dialogueStack.Peek();
                parent.AddChoice(choice);
                parent.isChoiceLine = true;
            }
            return true;
        }
        return false;
    }

    private bool CheckIfEnd(string line)
    {
        if (line.Contains("~END~"))
        {
            return true;
        }
        return false;
    }

    public void GetNextLine(int nextId)
    {
        foreach (DialogueLine line in dialogueLines)
        {
            if (line.id == nextId)
            {
                if (line.isCommand)
                {
                    CodeInterpreter.ExecuteCommand(line.text);
                    GetNextLine(line.nextId);
                    return;
                }
                onNewDialogueLine.Invoke(line);
                return;
            }
        }
        ResetDialogueData();
        onDialogueEnd.Invoke();
    }

    private int GetIndentLevel(string line)
    {
        int indentLevel = 0;
        foreach (char c in line)
        {
            if (c == ' ' || c == '\t')
            {
                indentLevel++;
            }
            else
            {
                break;
            }
        }
        return indentLevel / 4; // Assuming each indent level is 4 spaces or 1 tab
    }

    private void ResetDialogueData()
    {
        dialogueLines.Clear();
        dialogueStack.Clear();
        currentLine = null;
    }
}
