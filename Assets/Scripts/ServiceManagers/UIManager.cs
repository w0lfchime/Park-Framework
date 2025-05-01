using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [SerializeField] private UIStylesheet stylesheet;

    private Dictionary<string, GameObject> activeElements = new Dictionary<string, GameObject>();


    public Canvas canvas;




    private void Awake()
    {


        

    }

    public void SetTargetCanvas(Canvas newCanvas)
    {

    }


    private void HandleScreenSizeChanged(float width, float height)
    {



    }





}
