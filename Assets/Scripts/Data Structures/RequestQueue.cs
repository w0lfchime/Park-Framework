using System;
using System.Collections.Generic;
using System.Linq;

public class RequestQueue
{
	private Dictionary<SetStateRequest, int> stateDict = new();

	public int Count => stateDict.Count;

	public void Add(SetStateRequest request, int lifeTime)
	{
		stateDict[request] = lifeTime;
	}

	public bool TryGetHighestPriority(out SetStateRequest request)
	{
		if (stateDict.Count == 0)
		{
			request = default;
			return false;
		}

		request = stateDict.Keys.OrderByDescending(r => r.PushForce).First();
		return true;
	}

	public void FixedFrameUpdate()
	{
		List<SetStateRequest> toRemove = new();

		foreach (var kvp in stateDict.ToList())
		{
			int nextFrameLifetime = kvp.Value - 1;
			if (nextFrameLifetime <= 0)
			{
				toRemove.Add(kvp.Key);
			}
			else
			{
				stateDict[kvp.Key] = nextFrameLifetime;
			}
		}

		// Remove expired entries
		foreach (var key in toRemove)
		{
			stateDict.Remove(key);
		}
	}

	public void ClearClearOnSetState()
	{
		var keysToRemove = stateDict.Keys.Where(k => k.ClearOnSetState).ToList();
		foreach (var key in keysToRemove)
		{
			stateDict.Remove(key);
		}
	}

	public void Clear() => stateDict.Clear();
}
