using System;
using System.Collections.Generic;
using UnityEngine;

public static class CommandHandler
{
    private static readonly Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();

    public static void RegisterCommand(string command, Action<string[]> callback)
    {
        if (!commands.ContainsKey(command))
        {
            commands.Add(command, callback);
            LogCore.Log("CommandMeta", $"Command '{command}' registered.");
        }
        else
        {
            LogCore.Log("CommandMeta", $"Command '{command}' is already registered.");
        }
    }

    public static void ExecuteCommand(string commandInput)
    {
        if (string.IsNullOrWhiteSpace(commandInput))
        {
            LogCore.Log("CommandMeta", "Command cannot be empty.");
            return;
        }

        // Check if the command starts with a slash
        if (!commandInput.StartsWith("/"))
        {
            LogCore.Log("CommandMeta", "Invalid command. All commands must start with a '/'.");
            return;
        }

        // Remove the leading slash for processing
        string trimmedInput = commandInput.Substring(1);
        string[] splitInput = trimmedInput.Split(' ');
        string command = splitInput[0];
        string[] args = new string[splitInput.Length - 1];
        Array.Copy(splitInput, 1, args, 0, args.Length);

        // Execute the command if it's registered
        if (commands.TryGetValue(command, out var action))
        {
            action.Invoke(args);
        }
        else
        {
            LogCore.Log("CommandMeta", $"Command '{command}' not recognized.");
        }
    }

}
