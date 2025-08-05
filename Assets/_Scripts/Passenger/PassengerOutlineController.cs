using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EPOOutline;
using UnityEngine;

public class PassengerOutlineController : MonoBehaviour,IOutlinable
{
    private Outlinable outlinable;

    private void Start()
    {
        outlinable = GetComponent<Outlinable>();
    }

    public void OutlineSet(bool on)
    {
        outlinable.enabled = on;
    }
}
