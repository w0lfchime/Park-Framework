using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal.Commands;

public class DebugVector : MonoBehaviour
{
    //Vector Params
    public float vectorMagRenderCutoff = 0.05f;
    public float inactiveTimeThreshold = 0.05f;
    public float vectorLerpRate = 15.0f;
    public float positionLerpRate = 30.0f;
    public float thickness = 0.1f;
    public float vectorLengthFactor = 0.1f;

    //Object refs
    [SerializeField] private GameObject vectorLine;
    [SerializeField] private GameObject vectorHead;

    //Rendering
    private Renderer lineRenderer;
    private Renderer arrowRenderer;
    private Color vectorColor;

    //Activation Control
    [SerializeField] private float lastUpdateTime;
    [SerializeField] private bool isActive;

    //interpolation
    private Vector3 targetVector;
    private Vector3 lerpVector;
    private Vector3 targetPosition;

    private void Awake()
    {
        if (vectorLine) lineRenderer = vectorLine.GetComponent<Renderer>();
        if (vectorHead) arrowRenderer = vectorHead.GetComponent<Renderer>();
    }

    private void Update()
    {
        VectorLerp();
        ApplyVectorUpdates();

        if (Time.time - lastUpdateTime > inactiveTimeThreshold)
        {
            isActive = false;
        }

        SetActiveState(isActive); //hack shit
    }

    public void Initialize()
    {
        SetActiveState(false);

        //more if needed
    }

    public void VectorLerp()
    {
        lerpVector = Vector3.Lerp(lerpVector, targetVector, vectorLerpRate * Time.deltaTime);
    }

    private void ApplyVectorUpdates()
    {
        isActive = false;

        if (lerpVector.magnitude > vectorMagRenderCutoff)
        {
            isActive = true;

            Vector3 endPos = lerpVector;
            Vector3 midPoint = (endPos) / 2f;

            vectorLine.transform.localPosition = midPoint;
            vectorLine.transform.localRotation = Quaternion.LookRotation(lerpVector.sqrMagnitude > 0 ? lerpVector.normalized : Vector3.forward);
            vectorLine.transform.localScale = new Vector3(thickness, thickness, lerpVector.magnitude);

            if (lineRenderer) lineRenderer.material.color = vectorColor;

 

            if (vectorHead)
            {
                vectorHead.transform.localPosition = endPos;
                vectorHead.transform.localRotation = Quaternion.LookRotation(Vector3.forward, lerpVector);
                if (arrowRenderer) arrowRenderer.material.color = vectorColor;
            }
        }

        SetActiveState(isActive);
    }

    public void UpdateVector(Vector3 vector, Color color)
    {

        targetVector = vector * vectorLengthFactor;
        vectorColor = color;

        lastUpdateTime = Time.time;
    }

    private void SetActiveState(bool active)
    {
        if (vectorLine) vectorLine.SetActive(active);
        if (vectorHead) vectorHead.SetActive(active);
    }
}
