using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using EPOOutline;
using UnityEngine;

public class Passenger : MonoBehaviour,IColorable
{
    [SerializeField] private Animator passengerAnimator;
    [SerializeField] private ObjColor passengerColor;
    public Renderer passengerRenderer;
    
    public ObjColor ObjColor => passengerColor;

    private IOutlinable outlinable;

    private PassengerStateMachine stateMachine;
    private void Awake()
    {
        outlinable = GetComponent<IOutlinable>();
        stateMachine = GetComponent<PassengerStateMachine>();
    }

    private void Start()
    {
        stateMachine.Initialize(new WaitingOnGridState(this));
    }

    public void ColorSetup(ObjColor color)
    { 
        passengerRenderer.material.color = ColorUtils.FromObjColor(color);
        passengerColor = color;
    }

    public void SetAnimator(string stateName,bool isOn)
    {
        passengerAnimator.SetBool(stateName, isOn);
    }
    
    public void SetAnimator(string stateName)
    {
        passengerAnimator.SetTrigger(stateName);
    }

    public void PassengerPathControl(bool isPathClear)
    {
        outlinable?.OutlineSet(isPathClear);
    }
    
}
