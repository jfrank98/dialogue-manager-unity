using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public static class CodeInterpreter
{
    public delegate void CreateDialogueLine(string line, bool isCommand = false);
    public delegate void GoTo(string line);
    public static UnityEvent createDialogueLineEvent = new();
    public static bool CheckIfCode(this string line, StreamReader currentLine, CreateDialogueLine createDialogueLine, GoTo goTo)
    {
      if (!line.StartsWith("$")) return false;

      if (line.StartsWith("$IF"))
      {
        HandleIf(line, goTo, currentLine, createDialogueLine);
      }
      else if (line.StartsWith("$ELSE"))
      {
        goTo("$END");
      }
      else if (line.StartsWith("$SET") || line.StartsWith("$EMIT") || line.StartsWith("$CALL"))
      {
          // createDialogueLine(line, isCommand: true); // Store the command as part of the dialogue line
        return true;
      }

      return false;
    }

    public static void ExecuteCommand(string command)
    {
        if (command.StartsWith("$SET"))
        {
            HandleSet(command);
        }
        else if (command.StartsWith("$EMIT"))
        {
            HandleEmit(command);
        }
        else if (command.StartsWith("$CALL"))
        {
            HandleCall(command);
        }
    }

    private static void HandleCall(string line)
    {
        var splitLine = line.Split(' ', 3);
        var callStr = splitLine[1];
        var objStr = callStr.Split('.')[0];
        var methodName = callStr.Split('.')[1];
        var args = splitLine[2].Split(',');
        object obj = DialogueCodeReflection.GetSingletonInstance(objStr);
        DialogueCodeReflection.CallMethod(obj, methodName, args);
    }

    private static void HandleSet(string line)
    {
        var splitLine = line.Split(' ', 3);
        var assignmentStr = splitLine[1];
        var objStr = assignmentStr.Split('.')[0];
        var varStr = assignmentStr.Split('.')[1];
        var varValue = splitLine.Length >= 2 ? splitLine[2] : string.Empty;
        object obj = DialogueCodeReflection.GetSingletonInstance(objStr);
        DialogueCodeReflection.SetFieldValue(obj, varStr, varValue);
    }

    private static void HandleEmit(string line)
    {
        var splitLine = line.Split(' ', 3);
        var eventStr = splitLine[1];
        var objStr = eventStr.Split('.')[0];
        var eventName = eventStr.Split('.')[1];
        var arg = splitLine.Length > 2 ? splitLine[2] : string.Empty;
        object obj = DialogueCodeReflection.GetSingletonInstance(objStr);
        DialogueCodeReflection.EmitEvent(obj, eventName, arg);
    }

    private static void HandleIf(string line, GoTo goTo, StreamReader currentLine, CreateDialogueLine createDialogueLine)
    {
        var splitLine = line.Split(' ', 2);
        var conditionStr = splitLine[1];
        var objStr = conditionStr.Split('.')[0];
        var varStr = conditionStr.Split('.')[1];
        var isNot = objStr.StartsWith("!");
        Debug.Log(objStr.Remove(0, 1));
        if (isNot) objStr = objStr[1..];
        object obj = DialogueCodeReflection.GetSingletonInstance(objStr);
        var condition = (bool)DialogueCodeReflection.GetFieldValue(obj, varStr);
        if (isNot) condition = !condition;

        if (!condition) goTo("$ELSE");

        // while ((line = currentLine.ReadLine()) != null)
        // {
        //     if (line.StartsWith("$ELSE") || line.StartsWith("$END"))
        //     {
        //         if (line.StartsWith("$ELSE")) goTo("$END");
        //         break;
        //     }
        //     if (line.CheckIfCode(currentLine, createDialogueLine, goTo)) continue;
        //     createDialogueLine(line);
        // }
    }
}
