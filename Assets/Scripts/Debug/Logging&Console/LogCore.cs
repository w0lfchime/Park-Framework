using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public static class LogCore
{
    private static HashSet<string> blacklistedCategories = new HashSet<string>
    {
        "PSM_Flow",
        "PSM_Detail",
    };

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
        logCategoriesAsset = Resources.Load<LogCategories>("Debug/logCategoriesRef");

        if (logCategoriesAsset == null)
        {
            Debug.LogWarning("No LogCategories asset found in Resources. Please create one.");
        }
    }

    public static void Log(string category, string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
    {
        if (loggingEnabled && !blacklistedCategories.Contains(category))
        {
            messageCount++;

            string formattedMessage = $"[{category}] {messageCount} {message} (at {System.IO.Path.GetFileName(file)}:{line} in {member})";

            if (category.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError(formattedMessage);
            }
            else if (category.Contains("Warning", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(formattedMessage);
            }
            else
            {
                Debug.Log(formattedMessage);
            }

            OnLog?.Invoke(formattedMessage);

            // Save new category if not already logged
            TrackNewCategory(category);
        }
    }

	public static void Log(string message)
	{
		if (loggingEnabled && !blacklistedCategories.Contains("NoCategory"))
		{
			Debug.Log(message);
			OnLog?.Invoke(message);
			TrackNewCategory("NoCategory");
		}
	}


	private static void TrackNewCategory(string category)
    {
        if (logCategoriesAsset != null && !logCategoriesAsset.categories.Contains(category))
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

    public static void BlacklistCategory(string category)
    {
        blacklistedCategories.Add(category);
    }

    public static void WhitelistCategory(string category)
    {
        blacklistedCategories.Remove(category);
    }

    public static bool IsCategoryBlacklisted(string category)
    {
        return blacklistedCategories.Contains(category);
    }
}
