using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueChoice
{
    public string text;
    public DialogueLine parent;
    public int nextId;
    public int indentLevel;

    public DialogueChoice(DialogueLine parent, string text)
    {
        this.parent = parent;
        this.text = text;
        nextId = DialogueLine.titleCounters[parent.title] + 1;
    }
}
