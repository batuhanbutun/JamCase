using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class Passenger : MonoBehaviour,IColorable
{
    [SerializeField] private ObjColor passengerColor;
    
    public ObjColor ObjColor => passengerColor;

    private void Start()
    {
        ColorSetup(passengerColor);
    }

    public void ColorSetup(ObjColor color)
    {
       var renderer = GetComponent<Renderer>();
       renderer.material.color = ColorUtils.FromObjColor(color);
    }
}
