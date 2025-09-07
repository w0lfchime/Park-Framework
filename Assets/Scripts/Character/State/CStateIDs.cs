using System;
using System.Collections.Generic;
using System.Reflection;

public enum CharacterID
{
	None = 0,
	Ric = 1000,
	Storm = 2000,
}

public static class CStateIDs
{
	public const int Null = 0;
	public const int Suspended = 1;
	public const int Flight = 2;

	public const int GroundedIdle = 3;
	public const int Walk = 4;
	public const int Run = 5;
	public const int Jump = 6;
	public const int IdleAirborne = 7;

	public static class Ric
	{

	}

	private static readonly Dictionary<int, string> reverseLookup;
	private static readonly Dictionary<string, int> forwardLookup;

	static CStateIDs()
	{
		reverseLookup = new Dictionary<int, string>();
		forwardLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		// Scan top-level and nested static classes
		var typesToScan = new List<Type> { typeof(CStateIDs) };
		typesToScan.AddRange(typeof(CStateIDs).GetNestedTypes(BindingFlags.Public | BindingFlags.Static));

		foreach (var t in typesToScan)
		{
			foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
			{
				if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(int))
				{
					int value = (int)field.GetRawConstantValue();
					string name = t == typeof(CStateIDs) ? field.Name : $"{t.Name}.{field.Name}";
					reverseLookup[value] = name;
					forwardLookup[name] = value;
				}
			}
		}
	}

	public static int GenericStateCount() 
	{
		int count = 0;

		// Scan top-level and nested static classes
		var typesToScan = new List<Type> { typeof(CStateIDs) };
		typesToScan.AddRange(typeof(CStateIDs).GetNestedTypes(BindingFlags.Public | BindingFlags.Static));

		foreach (var t in typesToScan)
		{
			foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
			{
				if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(int))
				{
					int value = (int)field.GetRawConstantValue();
					if (value >= 0 && value <= 999)
					{
						count++;
					}
				}
			}
		}

		return count;
	}

	public static string GetStateName(int id)
	{
		return reverseLookup.TryGetValue(id, out var name) ? name : null;
	}

	public static int? GetStateId(string name)
	{
		return forwardLookup.TryGetValue(name, out var id) ? id : (int?)null;
	}
}
