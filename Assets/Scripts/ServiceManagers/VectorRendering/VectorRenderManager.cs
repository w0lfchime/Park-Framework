using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class VectorRenderManager : MonoBehaviour
{
    private Dictionary<string, DebugVector> debugVectors = new Dictionary<string, DebugVector>();

    [SerializeField] private GameObject debugVectorPrefab;

    public void UpdateVector(string name, Transform parent, Vector3 offset, Vector3 vector, Color color)
    {

        if (!debugVectors.TryGetValue(name, out DebugVector debugVector))
        {
            GameObject vectorObj = Instantiate(debugVectorPrefab, parent);
            vectorObj.name = name;
            debugVector = vectorObj.GetComponent<DebugVector>();
            debugVector.Initialize();
            debugVectors[name] = debugVector;
        }

        debugVector.UpdateVector(vector, color);
    }

    public void ResetVectors()
    {
        foreach (var kvp in debugVectors)
        {
            Destroy(kvp.Value.gameObject);
        }
        debugVectors.Clear();
    }

    public void DestroyVector(string name)
    {
        if (debugVectors.TryGetValue(name, out DebugVector debugVector))
        {
            Destroy(debugVector.gameObject);
            debugVectors.Remove(name);
        }
    }

    public void StampVector(string name, Vector3 startPos, Vector3 vector, Color color, float duration)
    {
        StartCoroutine(StampVectorCoroutine(name, startPos, vector, color, duration));
    }

    private IEnumerator StampVectorCoroutine(string name, Vector3 startPos, Vector3 vector, Color color, float duration)
    {
        GameObject vectorObj = Instantiate(debugVectorPrefab, transform);
        DebugVector tempVector = vectorObj.GetComponent<DebugVector>();
        tempVector.Initialize();
        tempVector.UpdateVector(vector, color);

        yield return new WaitForSeconds(duration);

        Destroy(tempVector.gameObject);
    }
}
