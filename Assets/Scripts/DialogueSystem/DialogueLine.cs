using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueLine
{
    public static Dictionary<string, int> titleCounters = new();

    public int id;
    public int nextId;
    public string title = "";
    public string characterName;
    public string text;
    public bool isChoiceLine = false;
    public List<DialogueChoice> choices;
    public List<DialogueLine> childrenDialogueLines;
    public bool isCommand = false;
    public DialogueLine(string line, string title, bool isCommand)
    {
        this.title = title;
        assignId();
        nextId = id + 1;
        childrenDialogueLines = new();
        if (isCommand)
        {
            this.isCommand = true;
            text = line;
        }
        else
        {
            ParseDialogueLine(line);
        }
    }

    private void ParseDialogueLine(string line)
    {
        List<string> splitNameText = new(line.SplitWithEscape(':'));
        characterName = splitNameText[0];
        text = splitNameText[1];
        choices = new();
    }

    private void assignId()
    {
        if (titleCounters.ContainsKey(title))
        {
            titleCounters[title]++;
            id = titleCounters[title];
        }
        else
        {
            titleCounters.Add(title, 1);
            id = 1;
        }
    }

    public void AddChoice(string choice)
    {
        choices.Add(new DialogueChoice(this, choice));
    }

    public void AddChild(DialogueLine child)
    {
        childrenDialogueLines.Add(child);
    }
}
