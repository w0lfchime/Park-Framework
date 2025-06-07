using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public static class LogCore
{
    private static int messageCount = 0;

    public static event Action<string> OnLog;

    private static LogCategories logCategoriesAsset;

    public static bool loggingEnabled = true;

    static LogCore()
    {
        LoadLogCategoriesAsset();
    }

    private static void LoadLogCategoriesAsset()
    {
        var foundAssets = Resources.LoadAll<LogCategories>("");

        if (foundAssets.Length > 0)
        {
            logCategoriesAsset = foundAssets[0]; // or use LINQ to filter more specifically
        }
        else
        {
            Debug.LogWarning("No LogCategories asset found in Resources. Please create one.");
        }
    }

    public static void Log(string category, string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
    {
        if (!loggingEnabled || !ShouldLogCategory(category)) return;

        messageCount++;
        string formattedMessage = $"({messageCount}) [{category}] {message} (at {System.IO.Path.GetFileName(file)}:{line} in {member})";

        if (category.Contains("Error", StringComparison.OrdinalIgnoreCase))
            Debug.LogError(formattedMessage);
        else if (category.Contains("Warning", StringComparison.OrdinalIgnoreCase))
            Debug.LogWarning(formattedMessage);
        else
            Debug.Log(formattedMessage);

        OnLog?.Invoke(formattedMessage);

        TrackNewCategory(category);
    }

    public static void Log(string message)
    {
        const string defaultCategory = "NoCategory";
        if (!loggingEnabled || !ShouldLogCategory(defaultCategory)) return;

        Debug.Log(message);
        OnLog?.Invoke(message);

        TrackNewCategory(defaultCategory);
    }

    private static bool ShouldLogCategory(string category)
    {
        if (logCategoriesAsset == null) return true;

        var cat = logCategoriesAsset.GetCategory(category);
        return cat == null || cat.enabled; // default: log if not present
    }

    private static void TrackNewCategory(string category)
    {
        if (logCategoriesAsset == null) return;

        if (!logCategoriesAsset.ContainsCategory(category))
        {
            logCategoriesAsset.AddCategory(category);
            SaveLoggedCategories();
        }
    }

    private static void SaveLoggedCategories()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(logCategoriesAsset);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}
