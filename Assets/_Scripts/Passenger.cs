using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class Passenger : MonoBehaviour,IColorable
{
    [SerializeField] private Animator passengerAnimator;
    [SerializeField] private ObjColor passengerColor;
    public Renderer passengerRenderer;
    
    public ObjColor ObjColor => passengerColor;

    
    public void ColorSetup(ObjColor color)
    { 
        passengerRenderer.material.color = ColorUtils.FromObjColor(color);
        passengerColor = color;
    }

    public void SetAnimator(string trigger)
    {
        passengerAnimator.SetTrigger(trigger);
    }
    
}
