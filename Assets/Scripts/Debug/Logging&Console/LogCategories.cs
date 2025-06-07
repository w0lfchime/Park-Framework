using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LogCategories", menuName = "Logging/Log Categories")]
public class LogCategories : ScriptableObject
{
    [System.Serializable]
    public class LogCategory
    {
        public string name;
        public bool enabled = true;
    }

    public List<LogCategory> categories = new List<LogCategory>();

    public LogCategory GetCategory(string name)
    {
        return categories.Find(c => c.name == name);
    }

    public bool ContainsCategory(string name)
    {
        return categories.Exists(c => c.name == name);
    }

    public void AddCategory(string name)
    {
        categories.Add(new LogCategory { name = name, enabled = true });
    }
}
