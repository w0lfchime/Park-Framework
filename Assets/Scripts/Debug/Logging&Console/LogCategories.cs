using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LogCategories", menuName = "Logging/LogCategories")]
public class LogCategories : ScriptableObject
{
    public List<string> categories = new List<string>();

    public void AddCategory(string category)
    {
        if (!categories.Contains(category))
        {
            categories.Add(category);
        }
    }
}
